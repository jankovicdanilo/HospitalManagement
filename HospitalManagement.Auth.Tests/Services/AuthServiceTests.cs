using AutoMapper;
using HospitalManagement.Auth.Models.Domain;
using HospitalManagement.Auth.Models.DTOs;
using HospitalManagement.Auth.Models.Enums;
using HospitalManagement.Auth.Repositories.Interfaces;
using HospitalManagement.Auth.Services.Implementations;
using HospitalManagement.Shared.Common;
using HospitalManagement.Shared.Security;
using HospitalManagement.Shared.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

        private void SetupSuccessfulUpdate(User user)
        {
            authRepositoryMock.Setup(r => r.GetByIdAsync(user.Id)).ReturnsAsync(user);
            authRepositoryMock.Setup(r => r.UpdateAsync(user)).ReturnsAsync(user);
            mapperMock.Setup(m => m.Map<AuthResponseUpdateDto>(user))
                .Returns(new AuthResponseUpdateDto { Id = user.Id, Username = user.Username });
        }

        private void SetupSuccessfulRegister(User user)
        {
            authRepositoryMock.Setup(r => r.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            authRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
            mapperMock.Setup(m => m.Map<User>(It.IsAny<RegisterRequestDto>())).Returns(user);
            authRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<User>())).ReturnsAsync(user);
            mapperMock.Setup(m => m.Map<AuthResponseDto>(It.IsAny<User>()))
                .Returns(new AuthResponseDto { Username = user.Username, Email = user.Email });
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
        public async Task RegisterAsync_DoctorAlreadyHasAnAccount_ReturnsConflictAndDoesNotCreate()
        {
            var request = new RegisterRequestDto
            {
                Username = "seconddoctor6",
                Email = "second@example.com",
                Password = "Password123!",
                Role = UserRole.Doctor,
                DoctorId = 6
            };
            var existing = new User { Id = 3, Username = "doctor6", DoctorId = 6, Role = UserRole.Doctor };

            authRepositoryMock.Setup(r => r.GetByUsernameAsync(request.Username)).ReturnsAsync((User?)null);
            authRepositoryMock.Setup(r => r.GetByEmailAsync(request.Email)).ReturnsAsync((User?)null);
            authRepositoryMock.Setup(r => r.GetByDoctorIdAsync(6)).ReturnsAsync(existing);

            var result = await authService.RegisterAsync(request);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.Conflict));
            Assert.That(result.ErrorCode, Is.EqualTo("DOCTOR_ALREADY_LINKED"));
            authRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Never);
        }

        [Test]
        public async Task RegisterAsync_DoctorWithoutAnAccountYet_ReturnsSuccess()
        {
            var request = new RegisterRequestDto
            {
                Username = "doctor9",
                Email = "doctor9@example.com",
                Password = "Password123!",
                Role = UserRole.Doctor,
                DoctorId = 9
            };
            var user = new User { Id = 5, Username = "doctor9", Email = "doctor9@example.com", Role = UserRole.Doctor, DoctorId = 9 };

            SetupSuccessfulRegister(user);
            authRepositoryMock.Setup(r => r.GetByDoctorIdAsync(9)).ReturnsAsync((User?)null);

            var result = await authService.RegisterAsync(request);

            Assert.That(result.Success, Is.True);
            authRepositoryMock.Verify(r => r.GetByDoctorIdAsync(9), Times.Once);
            authRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<User>()), Times.Once);
        }

        [Test]
        public async Task RegisterAsync_UserWithoutDoctorId_DoesNotLookUpTheDoctorLink()
        {
            var request = new RegisterRequestDto
            {
                Username = "manager2",
                Email = "manager2@example.com",
                Password = "Password123!",
                Role = UserRole.Manager
            };
            var user = new User { Id = 6, Username = "manager2", Email = "manager2@example.com", Role = UserRole.Manager };

            SetupSuccessfulRegister(user);

            var result = await authService.RegisterAsync(request);

            Assert.That(result.Success, Is.True);
            authRepositoryMock.Verify(r => r.GetByDoctorIdAsync(It.IsAny<int>()), Times.Never);
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
        public async Task LoginAsync_ValidCredentials_TokenContainsCorrectClaims()
        {
            var request = new LoginRequestDto
            {
                Username = "existinguser",
                Password = "CorrectPassword123"
            };
            var user = new User
            {
                Id = 42,
                Username = "existinguser",
                Email = "user@example.com",
                Role = UserRole.Doctor,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword123")
            };
            var userDto = new AuthResponseDto { Username = user.Username, Email = user.Email };

            authRepositoryMock.Setup(r => r.GetByUsernameAsync(request.Username)).ReturnsAsync(user);

            mapperMock.Setup(m => m.Map<AuthResponseDto>(It.IsAny<User>()))
                .Returns(userDto);

            var result = await authService.LoginAsync(request);

            Assert.That(result.Success, Is.True);

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(result.Data.Token);

            Assert.That(jwt.Issuer, Is.EqualTo(testJwtSettings.Issuer));
            Assert.That(jwt.Audiences, Does.Contain(testJwtSettings.Audience));
            Assert.That(jwt.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value, Is.EqualTo("42"));
            Assert.That(jwt.Claims.First(c => c.Type == ClaimTypes.Name).Value, Is.EqualTo("existinguser"));
            Assert.That(jwt.Claims.First(c => c.Type == ClaimTypes.Email).Value, Is.EqualTo("user@example.com"));
            Assert.That(jwt.Claims.First(c => c.Type == ClaimTypes.Role).Value, Is.EqualTo("Doctor"));
        }

        [Test]
        public async Task LoginAsync_DoctorWithDoctorId_TokenContainsRoleAndDoctorIdClaims()
        {
            var request = new LoginRequestDto { Username = "doctor6", Password = "Password123" };
            var user = new User
            {
                Id = 3,
                Username = "doctor6",
                Email = "doctor6@example.com",
                Role = UserRole.Doctor,
                DoctorId = 6,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123")
            };
            var userDto = new AuthResponseDto { Username = user.Username, Email = user.Email };

            authRepositoryMock.Setup(r => r.GetByUsernameAsync(request.Username)).ReturnsAsync(user);
            mapperMock.Setup(m => m.Map<AuthResponseDto>(It.IsAny<User>())).Returns(userDto);

            var result = await authService.LoginAsync(request);

            Assert.That(result.Success, Is.True);

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(result.Data.Token);

            Assert.That(jwt.Claims.First(c => c.Type == ClaimTypes.Role).Value, Is.EqualTo("Doctor"));
            Assert.That(jwt.Claims.First(c => c.Type == AppClaims.DoctorId).Value, Is.EqualTo("6"));
        }

        [TestCase(UserRole.Admin)]
        [TestCase(UserRole.Manager)]
        public async Task LoginAsync_UserWithoutDoctorId_TokenHasRoleAndNoDoctorIdClaim(UserRole role)
        {
            var request = new LoginRequestDto { Username = "staffuser", Password = "Password123" };
            var user = new User
            {
                Id = 4,
                Username = "staffuser",
                Email = "staff@example.com",
                Role = role,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123")
            };
            var userDto = new AuthResponseDto { Username = user.Username, Email = user.Email };

            authRepositoryMock.Setup(r => r.GetByUsernameAsync(request.Username)).ReturnsAsync(user);
            mapperMock.Setup(m => m.Map<AuthResponseDto>(It.IsAny<User>())).Returns(userDto);

            var result = await authService.LoginAsync(request);

            Assert.That(result.Success, Is.True);

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(result.Data.Token);

            Assert.That(jwt.Claims.First(c => c.Type == ClaimTypes.Role).Value, Is.EqualTo(role.ToString()));
            Assert.That(jwt.Claims.Any(c => c.Type == AppClaims.DoctorId), Is.False);
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
            int existingUserId = 99;

            authRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((User?)null);

            var result = await authService.DeleteUser(id, existingUserId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("USER_NOT_FOUND"));
            authRepositoryMock.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task DeleteUser_UserExists_ReturnsSuccess()
        {
            var user = new User { Id = 1, Username = "existinguser" };
            int existingUserId = 99;

            authRepositoryMock
                .Setup(r => r.GetByIdAsync(user.Id))
                .ReturnsAsync(user);

            var result = await authService.DeleteUser(user.Id, existingUserId);

            Assert.That(result.Success, Is.True);
            authRepositoryMock.Verify(r => r.Delete(1), Times.Once);
        }

        [Test]
        public async Task DeleteUser_OwnAccount_ReturnsConflictAndDoesNotDelete()
        {
            int id = 5;

            var result = await authService.DeleteUser(id, id);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.Conflict));
            Assert.That(result.ErrorCode, Is.EqualTo("CANNOT_DELETE_SELF"));
            authRepositoryMock.Verify(r => r.Delete(It.IsAny<int>()), Times.Never);
        }

        [Test]
        public async Task UpdateUserAsync_UserNotFound_ReturnsFail()
        {
            int id = 1;
            int existingUserId = 99;
            var request = new UpdateUserRequestDto { Id = id };

            authRepositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((User?)null);

            var result = await authService.UpdateUserAsync(request, existingUserId);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorCode, Is.EqualTo("USER_NOT_FOUND"));
            authRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Test]
        public async Task UpdateUserAsync_UserExists_ReturnsSuccess()
        {
            var request = new UpdateUserRequestDto { Id = 1, Username = "updatedname" };
            int existingUserId = 99;
            var user = new User
            {
                Id = 1,
                Username = "oldname",
                Email = "user@example.com",
                Role = UserRole.Doctor
            };
            var userDto = new AuthResponseUpdateDto { Id = 1, Username = "updatedname" };

            authRepositoryMock.Setup(r => r.GetByIdAsync(request.Id)).ReturnsAsync(user);

            mapperMock.Setup(m => m.Map(request, user))
                .Callback<UpdateUserRequestDto, User>((src, dest) =>
                {
                    dest.Username = src.Username;
                });

            authRepositoryMock.Setup(r => r.UpdateAsync(It.Is<User>(u => u.Username == "updatedname")))
                .ReturnsAsync(user);

            mapperMock.Setup(m => m.Map<AuthResponseUpdateDto>(user)).Returns(userDto);

            var result = await authService.UpdateUserAsync(request, existingUserId);

            Assert.That(result.Success, Is.True);
            Assert.That(result.Data.Username, Is.EqualTo("updatedname"));
            Assert.That(user.Username, Is.EqualTo("updatedname"));
            authRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
        }

        [Test]
        public async Task UpdateUserAsync_ChangingOwnRole_ReturnsConflictAndDoesNotUpdate()
        {
            var user = new User { Id = 5, Username = "admin", Email = "admin@example.com", Role = UserRole.Admin };
            var request = new UpdateUserRequestDto { Id = 5, Username = "admin", Email = "admin@example.com", Role = UserRole.Manager };

            authRepositoryMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(user);

            var result = await authService.UpdateUserAsync(request, 5);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.Conflict));
            Assert.That(result.ErrorCode, Is.EqualTo("CANNOT_CHANGE_OWN_ROLE"));
            mapperMock.Verify(m => m.Map(request, user), Times.Never);
            authRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Test]
        public async Task UpdateUserAsync_OwnAccountKeepingSameRole_ReturnsSuccess()
        {
            var user = new User { Id = 5, Username = "admin", Email = "admin@example.com", Role = UserRole.Admin };
            var request = new UpdateUserRequestDto { Id = 5, Username = "admin", Email = "new@example.com", Role = UserRole.Admin };

            SetupSuccessfulUpdate(user);

            var result = await authService.UpdateUserAsync(request, 5);

            Assert.That(result.Success, Is.True);
            authRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
        }

        [Test]
        public async Task UpdateUserAsync_ChangingAnotherUsersRole_ReturnsSuccess()
        {
            var user = new User { Id = 1, Username = "doctor6", Email = "doctor6@example.com", Role = UserRole.Doctor };
            var request = new UpdateUserRequestDto { Id = 1, Username = "doctor6", Email = "doctor6@example.com", Role = UserRole.Manager };

            SetupSuccessfulUpdate(user);

            var result = await authService.UpdateUserAsync(request, 99);

            Assert.That(result.Success, Is.True);
            authRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
        }

        [Test]
        public async Task UpdateUserAsync_UsernameTakenByAnotherUser_ReturnsConflictAndDoesNotUpdate()
        {
            var user = new User { Id = 1, Username = "doctor7", Email = "doctor7@example.com", Role = UserRole.Doctor };
            var otherUser = new User { Id = 2, Username = "doctor6", Email = "doctor6@example.com" };
            var request = new UpdateUserRequestDto { Id = 1, Username = "doctor6", Email = "doctor7@example.com", Role = UserRole.Doctor };

            authRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
            authRepositoryMock.Setup(r => r.GetByUsernameAsync("doctor6")).ReturnsAsync(otherUser);

            var result = await authService.UpdateUserAsync(request, 99);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.Conflict));
            Assert.That(result.ErrorCode, Is.EqualTo("USERNAME_TAKEN"));
            mapperMock.Verify(m => m.Map(request, user), Times.Never);
            authRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Test]
        public async Task UpdateUserAsync_EmailTakenByAnotherUser_ReturnsConflictAndDoesNotUpdate()
        {
            var user = new User { Id = 1, Username = "doctor7", Email = "doctor7@example.com", Role = UserRole.Doctor };
            var otherUser = new User { Id = 2, Username = "doctor6", Email = "doctor6@example.com" };
            var request = new UpdateUserRequestDto { Id = 1, Username = "doctor7", Email = "doctor6@example.com", Role = UserRole.Doctor };

            authRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
            authRepositoryMock.Setup(r => r.GetByUsernameAsync("doctor7")).ReturnsAsync(user);
            authRepositoryMock.Setup(r => r.GetByEmailAsync("doctor6@example.com")).ReturnsAsync(otherUser);

            var result = await authService.UpdateUserAsync(request, 99);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.Conflict));
            Assert.That(result.ErrorCode, Is.EqualTo("EMAIL_TAKEN"));
            mapperMock.Verify(m => m.Map(request, user), Times.Never);
            authRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Test]
        public async Task UpdateUserAsync_UnchangedUsernameAndEmail_DoesNotTreatTheUserAsItsOwnDuplicate()
        {
            var user = new User { Id = 1, Username = "doctor6", Email = "doctor6@example.com", Role = UserRole.Doctor };
            var request = new UpdateUserRequestDto { Id = 1, Username = "doctor6", Email = "doctor6@example.com", Role = UserRole.Doctor };

            SetupSuccessfulUpdate(user);
            authRepositoryMock.Setup(r => r.GetByUsernameAsync("doctor6")).ReturnsAsync(user);
            authRepositoryMock.Setup(r => r.GetByEmailAsync("doctor6@example.com")).ReturnsAsync(user);

            var result = await authService.UpdateUserAsync(request, 99);

            Assert.That(result.Success, Is.True);
            authRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
        }

        [Test]
        public async Task UpdateUserAsync_DoctorLinkedToAnotherUser_ReturnsConflictAndDoesNotUpdate()
        {
            var user = new User { Id = 1, Username = "doctor7", Email = "doctor7@example.com", Role = UserRole.Doctor, DoctorId = 7 };
            var otherUser = new User { Id = 2, Username = "doctor6", Email = "doctor6@example.com", Role = UserRole.Doctor, DoctorId = 6 };
            var request = new UpdateUserRequestDto
            {
                Id = 1,
                Username = "doctor7",
                Email = "doctor7@example.com",
                Role = UserRole.Doctor,
                DoctorId = 6
            };

            authRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(user);
            authRepositoryMock.Setup(r => r.GetByDoctorIdAsync(6)).ReturnsAsync(otherUser);

            var result = await authService.UpdateUserAsync(request, 99);

            Assert.That(result.Success, Is.False);
            Assert.That(result.ErrorType, Is.EqualTo(ErrorType.Conflict));
            Assert.That(result.ErrorCode, Is.EqualTo("DOCTOR_ALREADY_LINKED"));
            mapperMock.Verify(m => m.Map(request, user), Times.Never);
            authRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Test]
        public async Task UpdateUserAsync_KeepingTheUsersOwnDoctorLink_DoesNotTreatItAsADuplicate()
        {
            var user = new User { Id = 1, Username = "doctor6", Email = "doctor6@example.com", Role = UserRole.Doctor, DoctorId = 6 };
            var request = new UpdateUserRequestDto
            {
                Id = 1,
                Username = "doctor6",
                Email = "doctor6@example.com",
                Role = UserRole.Doctor,
                DoctorId = 6
            };

            SetupSuccessfulUpdate(user);
            authRepositoryMock.Setup(r => r.GetByDoctorIdAsync(6)).ReturnsAsync(user);

            var result = await authService.UpdateUserAsync(request, 99);

            Assert.That(result.Success, Is.True);
            authRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
        }

        [Test]
        public async Task UpdateUserAsync_LinkingToADoctorWithoutAnAccount_ReturnsSuccess()
        {
            var user = new User { Id = 1, Username = "doctor6", Email = "doctor6@example.com", Role = UserRole.Doctor, DoctorId = 6 };
            var request = new UpdateUserRequestDto
            {
                Id = 1,
                Username = "doctor6",
                Email = "doctor6@example.com",
                Role = UserRole.Doctor,
                DoctorId = 9
            };

            SetupSuccessfulUpdate(user);
            authRepositoryMock.Setup(r => r.GetByDoctorIdAsync(9)).ReturnsAsync((User?)null);

            var result = await authService.UpdateUserAsync(request, 99);

            Assert.That(result.Success, Is.True);
            authRepositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
        }

        [Test]
        public async Task UpdateUserAsync_RequestWithoutDoctorId_DoesNotLookUpTheDoctorLink()
        {
            var user = new User { Id = 2, Username = "manager", Email = "manager@example.com", Role = UserRole.Manager };
            var request = new UpdateUserRequestDto
            {
                Id = 2,
                Username = "manager",
                Email = "manager@example.com",
                Role = UserRole.Manager
            };

            SetupSuccessfulUpdate(user);

            var result = await authService.UpdateUserAsync(request, 99);

            Assert.That(result.Success, Is.True);
            authRepositoryMock.Verify(r => r.GetByDoctorIdAsync(It.IsAny<int>()), Times.Never);
        }
    }
}
