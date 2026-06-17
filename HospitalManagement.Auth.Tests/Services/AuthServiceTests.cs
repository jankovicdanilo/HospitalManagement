using AutoMapper;
using HospitalManagement.Auth.Models.Domain;
using HospitalManagement.Auth.Models.DTOs;
using HospitalManagement.Auth.Models.Enums;
using HospitalManagement.Auth.Repositories.Interfaces;
using HospitalManagement.Auth.Services.Implementations;
using HospitalManagement.Auth.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace HospitalManagement.Auth.Tests.Services
{
    internal class AuthServiceTests
    {
        private Mock<IAuthRepository> authRepositoryMock;
        private Mock<IMapper> mapperMock;
        private Mock<ILogger<AuthService>> loggerMock;
        private JwtSettings testJwtSettings;
        private AuthService authService;

        [SetUp]
        public void SetUp()
        {
            authRepositoryMock = new Mock<IAuthRepository>();
            mapperMock = new Mock<IMapper>();
            loggerMock = new Mock<ILogger<AuthService>>();

            testJwtSettings = new JwtSettings
            {
                Key = "test-signing-key-minimum-32-characters-long",
                Issuer = "TestIssuer",
                Audience = "TestAudience",
                ExpiryMinutes = 60
            };

            var options = Options.Create(testJwtSettings);

            authService = new AuthService
                (
                    authRepositoryMock.Object,
                    options,
                    mapperMock.Object,
                    loggerMock.Object
                );
        }

        [Test]
        public async Task RegisterAsync_UsernameAlreadyExists_ReturnsFail()
        {
            var request = new RegisterRequestDto
            {
                Username = "existinguser",
                Email = "new@example.com",
                Password = "Password123"
            };

            authRepositoryMock.Setup(r => r.GetByUsernameAsync(request.Username))
                .ReturnsAsync(new User { Id = 1, Username = "existinguser" });

            var result = await authService.RegisterAsync(request);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("USERNAME_TAKEN"));
            authRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Never);
        }

        [Test]
        public async Task RegisterAsync_EmailAlreadyExists_ReturnsFail()
        {
            var request = new RegisterRequestDto
            {
                Username = "newuser",
                Email = "taken@example.com",
                Password = "Password123"
            };

            authRepositoryMock.Setup(r => r.GetByUsernameAsync(request.Username))
                .ReturnsAsync((User?)null);

            authRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email))
                .ReturnsAsync(new User { Id = 1, Email = "taken@example.com" });

            var result = await authService.RegisterAsync(request);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("EMAIL_TAKEN"));
            authRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Never);
        }

        [Test]
        public async Task RegisterAsync_ValidRequest_ReturnsSuccessWithToken()
        {
            var request = new RegisterRequestDto
            {
                Username = "newuser",
                Email = "newuser@example.com",
                Password = "Password123",
                Role = UserRole.Doctor
            };
            var user = new User
            {
                Id = 1,
                Username = request.Username,
                Email = request.Email,
                Role = UserRole.Doctor
            };
            var userDto = new AuthResponseDto
            {
                Username = user.Username,
                Email = user.Email
            };

            authRepositoryMock.Setup(r => r.GetByUsernameAsync(request.Username))
                .ReturnsAsync((User?)null);

            authRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email))
                .ReturnsAsync((User?)null);

            mapperMock.Setup(m => m.Map<User>(It.IsAny<RegisterRequestDto>()))
                .Returns(user);

            authRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<User>()))
                .ReturnsAsync(user);

            mapperMock.Setup(m => m.Map<AuthResponseDto>(It.IsAny<User>()))
                .Returns(userDto);

            var result = await authService.RegisterAsync(request);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data.Username, Is.EqualTo(request.Username));
            Assert.That(result.Data.Token, Is.Not.Null.And.Not.Empty);
            authRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Once);
        }

        [Test]
        public async Task LoginAsync_UsernameNotFound_ReturnsFail()
        {
            var request = new LoginRequestDto
            {
                Username = "nonexistent",
                Password = "Password123"
            };

            authRepositoryMock.Setup(r => r.GetByUsernameAsync(request.Username)).ReturnsAsync((User?)null);

            var result = await authService.LoginAsync(request);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_CREDENTIALS"));
        }

        [Test]
        public async Task LoginAsync_InvalidPassword_ReturnsFail()
        {
            var request = new LoginRequestDto
            {
                Username = "nonexistent",
                Password = "Password123"
            };
            var user = new User
            {
                Id = 1,
                Username = "existinguser",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword123")
            };

            authRepositoryMock.Setup(r => r.GetByUsernameAsync(request.Username)).ReturnsAsync(user);

            var result = await authService.LoginAsync(request);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("INVALID_CREDENTIALS"));
        }

        [Test]
        public async Task LoginAsync_ValidCredentials_ReturnsSuccessWithToken()
        {
            var request = new LoginRequestDto
            {
                Username = "existinguser",
                Password = "Password123"
            };
            var user = new User
            {
                Id = 1,
                Username = "existinguser",
                Email = "user@example.com",
                Role = UserRole.Doctor,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123")
            };
            var userDto = new AuthResponseDto
            {
                Username = user.Username,
                Email = user.Email
            };

            authRepositoryMock.Setup(r => r.GetByUsernameAsync(request.Username)).ReturnsAsync(user);

            mapperMock.Setup(m => m.Map<AuthResponseDto>(It.IsAny<User>())).Returns(userDto);

            var result = await authService.LoginAsync(request);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data.Username, Is.EqualTo(user.Username));
            Assert.That(result.Data.Token, Is.Not.Null.And.Not.Empty);
        }

        [Test]
        public async Task GetCurrentUserAsync_UserNotFound_ReturnsFail()
        {
            int id = 1;

            authRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((User?)null);

            var result = await authService.GetCurrentUserAsync(id);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("USER_NOT_FOUND"));
        }

        [Test]
        public async Task GetCurrentUserAsync_UserFound_ReturnsSuccess()
        {
            var user = new User
            {
                Id = 1,
                Username = "existinguser",
                Email = "user@example.com",
                Role = UserRole.Doctor
            };
            var currentUser = new CurrentUserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role
            };
            authRepositoryMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);

            mapperMock.Setup(m => m.Map<CurrentUserDto>(It.IsAny<User>())).Returns(currentUser);

            var result = await authService.GetCurrentUserAsync(user.Id);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data.Username, Is.EqualTo(user.Username));
        }

        [Test]
        public async Task DeleteUser_UserNotFound_ReturnsFail()
        {
            int id = 1;

            authRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((User?)null);

            var result = await authService.DeleteUser(id);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("USER_NOT_FOUND"));
            authRepositoryMock.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task DeleteUser_UserExists_ReturnsSuccess()
        {
            var user = new User { Id = 1, Username = "existinguser" };

            authRepositoryMock
                .Setup(r => r.GetByIdAsync(user.Id))
                .ReturnsAsync(user);

            var result = await authService.DeleteUser(user.Id);

            Assert.That(result.Success, Is.True);
            authRepositoryMock.Verify(r => r.Delete(1), Times.Once);
        }
    }
}
