using Infrastructure;
using Infrastructure.Entities;
using Meltix.IntegrationTests;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Web.Controllers;
using Web.ViewModels;

namespace IntegrationTests.Web
{
    public class UserControllerTests
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        private readonly GuessNumberContext _context;

        public UserControllerTests()
        {
            _context = DbContextProvider.SetupContext();
            _factory = new CustomWebApplicationFactory(_context);
            _client = _factory.CreateClient();
        }

        #region Register Tests - Cas Nominaux

        [Fact]
        public async Task Register_WithValidData_ShouldReturnOk()
        {
            // Arrange
            var request = new CreateAuthUserVM
            {
                Pseudo = "TestUser",
                Mail = "test@example.com",
                Password = "Password1@"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/User/Register", request);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Register_WithValidData_ShouldReturnAuthUserDetailVM()
        {
            // Arrange
            var request = new CreateAuthUserVM
            {
                Pseudo = "TestUser",
                Mail = "test@example.com",
                Password = "Password1@"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/User/Register", request);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<AuthUserDetailVM>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Id);
        }

        [Fact]
        public async Task Register_ShouldReturnCorrectPseudo()
        {
            // Arrange
            var request = new CreateAuthUserVM
            {
                Pseudo = "JohnDoe",
                Mail = "john@example.com",
                Password = "Password1@"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/User/Register", request);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<AuthUserDetailVM>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(result);
            Assert.Equal("JohnDoe", result.Pseudo);
        }

        [Fact]
        public async Task Register_ShouldReturnCorrectEmail()
        {
            // Arrange
            var request = new CreateAuthUserVM
            {
                Pseudo = "JohnDoe",
                Mail = "john@example.com",
                Password = "Password1@"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/User/Register", request);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<AuthUserDetailVM>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(result);
            Assert.Equal("john@example.com", result.Email);
        }

        [Fact]
        public async Task Register_ShouldReturnCatPngAvatar()
        {
            // Arrange
            var request = new CreateAuthUserVM
            {
                Pseudo = "JohnDoe",
                Mail = "john@example.com",
                Password = "Password1@"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/User/Register", request);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<AuthUserDetailVM>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(result);
            Assert.Equal("cat.png", result.Avatar);
        }

        [Fact]
        public async Task Register_ShouldReturnNullLastLoginAndLastChangePassword()
        {
            // Arrange
            var request = new CreateAuthUserVM
            {
                Pseudo = "JohnDoe",
                Mail = "john@example.com",
                Password = "Password1@"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/User/Register", request);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<AuthUserDetailVM>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(result);
            Assert.Null(result.LastLogin);
            Assert.Null(result.LastChangePassword);
        }

        [Fact]
        public async Task Register_ShouldPersistUserInDatabase()
        {
            // Arrange
            var request = new CreateAuthUserVM
            {
                Pseudo = "PersistUser",
                Mail = "persist@example.com",
                Password = "Password1@"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/User/Register", request);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<AuthUserDetailVM>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(result);

            var userInDb = _context.AuthUsers.FirstOrDefault(u => u.Id == result.Id);
            Assert.NotNull(userInDb);
            Assert.Equal("PersistUser", userInDb.Pseudo);
            Assert.Equal("persist@example.com", userInDb.Email);
            Assert.Equal("cat.png", userInDb.Avatar);
        }

        [Fact]
        public async Task Register_ShouldPersistUserWithRoleUser()
        {
            // Arrange
            var request = new CreateAuthUserVM
            {
                Pseudo = "RoleUser",
                Mail = "role@example.com",
                Password = "Password1@"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/User/Register", request);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<AuthUserDetailVM>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(result);

            var userInDb = _context.AuthUsers.FirstOrDefault(u => u.Id == result.Id);
            Assert.NotNull(userInDb);
            Assert.Equal(Domain.Enums.RoleUser.User, userInDb.Role);
        }

        [Fact]
        public async Task Register_ShouldPersistHashedPassword()
        {
            // Arrange
            var request = new CreateAuthUserVM
            {
                Pseudo = "HashUser",
                Mail = "hash@example.com",
                Password = "Password1@"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/User/Register", request);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<AuthUserDetailVM>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(result);

            var userInDb = _context.AuthUsers.FirstOrDefault(u => u.Id == result.Id);
            Assert.NotNull(userInDb);
            Assert.NotEqual("Password1@", userInDb.Password);
            Assert.NotEmpty(userInDb.Password);
        }

        [Fact]
        public async Task Register_MultipleCalls_ShouldCreateDifferentUsers()
        {
            // Arrange
            var request1 = new CreateAuthUserVM { Pseudo = "User1", Mail = "user1@example.com", Password = "Password1@" };
            var request2 = new CreateAuthUserVM { Pseudo = "User2", Mail = "user2@example.com", Password = "Password1@" };

            // Act
            var response1 = await _client.PostAsJsonAsync("/api/User/Register", request1);
            var response2 = await _client.PostAsJsonAsync("/api/User/Register", request2);

            // Assert
            response1.EnsureSuccessStatusCode();
            response2.EnsureSuccessStatusCode();

            var user1 = JsonSerializer.Deserialize<AuthUserDetailVM>(
                await response1.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            var user2 = JsonSerializer.Deserialize<AuthUserDetailVM>(
                await response2.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            Assert.NotNull(user1);
            Assert.NotNull(user2);
            Assert.NotEqual(user1.Id, user2.Id);
            Assert.Equal("User1", user1.Pseudo);
            Assert.Equal("User2", user2.Pseudo);
        }

        [Fact]
        public async Task Register_ShouldReturnCorrectContentType()
        {
            // Arrange
            var request = new CreateAuthUserVM
            {
                Pseudo = "TestUser",
                Mail = "test@example.com",
                Password = "Password1@"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/User/Register", request);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        }

        #endregion

        #region Register Tests - Validation du ModelState

        [Fact]
        public async Task Register_WithMissingPseudo_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new { Mail = "test@example.com", Password = "Password1@" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/User/Register", request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Register_WithMissingMail_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new { Pseudo = "TestUser", Password = "Password1@" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/User/Register", request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Register_WithMissingPassword_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new { Pseudo = "TestUser", Mail = "test@example.com" };

            // Act
            var response = await _client.PostAsJsonAsync("/api/User/Register", request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Register_WithPseudoTooShort_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new CreateAuthUserVM
            {
                Pseudo = "AB",
                Mail = "test@example.com",
                Password = "Password1@"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/User/Register", request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Register_WithEmptyBody_ShouldReturnBadRequest()
        {
            // Arrange
            var request = new { };

            // Act
            var response = await _client.PostAsJsonAsync("/api/User/Register", request);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        #endregion

        #region Register Tests - Validation du Domaine

        [Fact]
        public async Task Register_WithInvalidEmailFormat_ShouldReturnInternalServerError()
        {
            // Arrange
            var request = new CreateAuthUserVM
            {
                Pseudo = "TestUser",
                Mail = "invalid-email",
                Password = "Password1@"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/User/Register", request);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task Register_WithWeakPassword_ShouldReturnInternalServerError()
        {
            // Arrange
            var request = new CreateAuthUserVM
            {
                Pseudo = "TestUser",
                Mail = "test@example.com",
                Password = "weak"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/User/Register", request);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task Register_WithPasswordNoUppercase_ShouldReturnInternalServerError()
        {
            // Arrange
            var request = new CreateAuthUserVM
            {
                Pseudo = "TestUser",
                Mail = "test@example.com",
                Password = "password1@"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/User/Register", request);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task Register_WithPasswordNoSpecialChar_ShouldReturnInternalServerError()
        {
            // Arrange
            var request = new CreateAuthUserVM
            {
                Pseudo = "TestUser",
                Mail = "test@example.com",
                Password = "Password1"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/User/Register", request);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task Register_WithInvalidEmail_ShouldNotPersistUser()
        {
            // Arrange
            var countBefore = _context.AuthUsers.Count();
            var request = new CreateAuthUserVM
            {
                Pseudo = "TestUser",
                Mail = "invalid-email",
                Password = "Password1@"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/User/Register", request);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Equal(countBefore, _context.AuthUsers.Count());
        }

        [Fact]
        public async Task Register_WithWeakPassword_ShouldNotPersistUser()
        {
            // Arrange
            var countBefore = _context.AuthUsers.Count();
            var request = new CreateAuthUserVM
            {
                Pseudo = "TestUser",
                Mail = "test@example.com",
                Password = "weak"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/User/Register", request);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
            Assert.Equal(countBefore, _context.AuthUsers.Count());
        }

        #endregion

        #region ValidatePseudo Tests - Cas Nominaux

        [Fact]
        public async Task ValidatePseudo_WithAvailablePseudo_ShouldReturnOkWithTrue()
        {
            // Act
            var response = await _client.GetAsync("/api/User/ValidatePseudo?pseudo=AvailableUser");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<bool>(await response.Content.ReadAsStringAsync());
            Assert.True(result);
        }

        [Fact]
        public async Task ValidatePseudo_WithTakenPseudo_ShouldReturnOkWithFalse()
        {
            // Arrange
            _context.AuthUsers.Add(new AuthUserEntity
            {
                Pseudo = "TakenPseudo",
                Avatar = "avatar.png",
                Email = "taken@example.com",
                Password = "hashedpassword",
                Role = Domain.Enums.RoleUser.User,
                Created = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/User/ValidatePseudo?pseudo=TakenPseudo");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<bool>(await response.Content.ReadAsStringAsync());
            Assert.False(result);
        }

        [Fact]
        public async Task ValidatePseudo_WithDifferentPseudo_ShouldReturnOkWithTrue()
        {
            // Arrange
            _context.AuthUsers.Add(new AuthUserEntity
            {
                Pseudo = "ExistingUser",
                Avatar = "avatar.png",
                Email = "existing@example.com",
                Password = "hashedpassword",
                Role = Domain.Enums.RoleUser.User,
                Created = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/User/ValidatePseudo?pseudo=OtherUser");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<bool>(await response.Content.ReadAsStringAsync());
            Assert.True(result);
        }

        [Fact]
        public async Task ValidatePseudo_ShouldReturnCorrectContentType()
        {
            // Act
            var response = await _client.GetAsync("/api/User/ValidatePseudo?pseudo=TestUser");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        }

        [Fact]
        public async Task ValidatePseudo_WithMultipleUsers_ShouldReturnFalseForExistingPseudo()
        {
            // Arrange
            _context.AuthUsers.AddRange(
                new AuthUserEntity { Pseudo = "Alpha", Avatar = "a.png", Email = "alpha@ex.com", Password = "h1", Role = Domain.Enums.RoleUser.User, Created = DateTime.UtcNow },
                new AuthUserEntity { Pseudo = "Beta", Avatar = "a.png", Email = "beta@ex.com", Password = "h2", Role = Domain.Enums.RoleUser.User, Created = DateTime.UtcNow }
            );
            await _context.SaveChangesAsync();

            // Act
            var responseAlpha = await _client.GetAsync("/api/User/ValidatePseudo?pseudo=Alpha");
            var responseBeta = await _client.GetAsync("/api/User/ValidatePseudo?pseudo=Beta");
            var responseGamma = await _client.GetAsync("/api/User/ValidatePseudo?pseudo=Gamma");

            // Assert
            Assert.False(JsonSerializer.Deserialize<bool>(await responseAlpha.Content.ReadAsStringAsync()));
            Assert.False(JsonSerializer.Deserialize<bool>(await responseBeta.Content.ReadAsStringAsync()));
            Assert.True(JsonSerializer.Deserialize<bool>(await responseGamma.Content.ReadAsStringAsync()));
        }

        [Fact]
        public async Task ValidatePseudo_AfterRegistration_ShouldReturnFalse()
        {
            // Arrange - first validate that pseudo is available
            var checkBefore = await _client.GetAsync("/api/User/ValidatePseudo?pseudo=NewUser");
            Assert.True(JsonSerializer.Deserialize<bool>(await checkBefore.Content.ReadAsStringAsync()));

            // Register the user
            var request = new CreateAuthUserVM { Pseudo = "NewUser", Mail = "newuser@example.com", Password = "Password1@" };
            await _client.PostAsJsonAsync("/api/User/Register", request);

            // Act - check again
            var checkAfter = await _client.GetAsync("/api/User/ValidatePseudo?pseudo=NewUser");

            // Assert
            Assert.False(JsonSerializer.Deserialize<bool>(await checkAfter.Content.ReadAsStringAsync()));
        }

        #endregion

        #region ValidateMail Tests - Cas Nominaux

        [Fact]
        public async Task ValidateMail_WithAvailableMail_ShouldReturnOkWithTrue()
        {
            // Act
            var response = await _client.GetAsync("/api/User/ValidateMail?mail=available@example.com");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<bool>(await response.Content.ReadAsStringAsync());
            Assert.True(result);
        }

        [Fact]
        public async Task ValidateMail_WithTakenMail_ShouldReturnOkWithFalse()
        {
            // Arrange
            _context.AuthUsers.Add(new AuthUserEntity
            {
                Pseudo = "ExistingUser",
                Avatar = "avatar.png",
                Email = "taken@example.com",
                Password = "hashedpassword",
                Role = Domain.Enums.RoleUser.User,
                Created = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/User/ValidateMail?mail=taken@example.com");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<bool>(await response.Content.ReadAsStringAsync());
            Assert.False(result);
        }

        [Fact]
        public async Task ValidateMail_WithDifferentMail_ShouldReturnOkWithTrue()
        {
            // Arrange
            _context.AuthUsers.Add(new AuthUserEntity
            {
                Pseudo = "ExistingUser",
                Avatar = "avatar.png",
                Email = "existing@example.com",
                Password = "hashedpassword",
                Role = Domain.Enums.RoleUser.User,
                Created = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/User/ValidateMail?mail=other@example.com");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<bool>(await response.Content.ReadAsStringAsync());
            Assert.True(result);
        }

        [Fact]
        public async Task ValidateMail_ShouldReturnCorrectContentType()
        {
            // Act
            var response = await _client.GetAsync("/api/User/ValidateMail?mail=test@example.com");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        }

        [Fact]
        public async Task ValidateMail_WithMultipleUsers_ShouldReturnFalseForExistingMail()
        {
            // Arrange
            _context.AuthUsers.AddRange(
                new AuthUserEntity { Pseudo = "User1", Avatar = "a.png", Email = "alpha@ex.com", Password = "h1", Role = Domain.Enums.RoleUser.User, Created = DateTime.UtcNow },
                new AuthUserEntity { Pseudo = "User2", Avatar = "a.png", Email = "beta@ex.com", Password = "h2", Role = Domain.Enums.RoleUser.User, Created = DateTime.UtcNow }
            );
            await _context.SaveChangesAsync();

            // Act
            var responseAlpha = await _client.GetAsync("/api/User/ValidateMail?mail=alpha@ex.com");
            var responseBeta = await _client.GetAsync("/api/User/ValidateMail?mail=beta@ex.com");
            var responseGamma = await _client.GetAsync("/api/User/ValidateMail?mail=gamma@ex.com");

            // Assert
            Assert.False(JsonSerializer.Deserialize<bool>(await responseAlpha.Content.ReadAsStringAsync()));
            Assert.False(JsonSerializer.Deserialize<bool>(await responseBeta.Content.ReadAsStringAsync()));
            Assert.True(JsonSerializer.Deserialize<bool>(await responseGamma.Content.ReadAsStringAsync()));
        }

        [Fact]
        public async Task ValidateMail_AfterRegistration_ShouldReturnFalse()
        {
            // Arrange - first validate that mail is available
            var checkBefore = await _client.GetAsync("/api/User/ValidateMail?mail=newuser@example.com");
            Assert.True(JsonSerializer.Deserialize<bool>(await checkBefore.Content.ReadAsStringAsync()));

            // Register the user
            var request = new CreateAuthUserVM { Pseudo = "NewUser", Mail = "newuser@example.com", Password = "Password1@" };
            await _client.PostAsJsonAsync("/api/User/Register", request);

            // Act - check again
            var checkAfter = await _client.GetAsync("/api/User/ValidateMail?mail=newuser@example.com");

            // Assert
            Assert.False(JsonSerializer.Deserialize<bool>(await checkAfter.Content.ReadAsStringAsync()));
        }

        #endregion
    }
}
