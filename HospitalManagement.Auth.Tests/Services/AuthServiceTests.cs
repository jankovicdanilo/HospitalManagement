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
    }
}
