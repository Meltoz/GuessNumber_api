using Domain.Enums;
using Infrastructure;
using Infrastructure.Entities;
using Meltix.IntegrationTests;
using System.Net;
using System.Text.Json;
using Web.ViewModels.Admin;

namespace IntegrationTests.Web
{
    public class AdminUserControllerTests
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        private readonly GuessNumberContext _context;

        public AdminUserControllerTests()
        {
            _context = DbContextProvider.SetupContext();
            _factory = new CustomWebApplicationFactory(_context);
            _client = _factory.CreateClient();
        }

        #region SearchUser Tests - Cas Nominaux

        [Fact]
        public async Task SearchUser_ShouldReturnOk_WithValidParameters()
        {
            // Arrange
            var user = new AuthUserEntity
            {
                Pseudo = "TestUser",
                Avatar = "avatar.png",
                Email = "test@example.com",
                Password = "hashedpassword",
                Role = RoleUser.User,
                Created = DateTime.UtcNow
            };
            _context.AuthUsers.Add(user);
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/AdminUser/SearchUser?pageIndex=0&pageSize=10&sort=pseudo_ascending&search=");

            // Assert
            response.EnsureSuccessStatusCode();
            var users = JsonSerializer.Deserialize<List<UserAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(users);
            Assert.Single(users);
            Assert.Equal("TestUser", users.First().Pseudo);
        }

        [Fact]
        public async Task SearchUser_ShouldReturnTotalCountInHeader()
        {
            // Arrange
            _context.AuthUsers.AddRange(
                new AuthUserEntity
                {
                    Pseudo = "User1",
                    Avatar = "avatar1.png",
                    Email = "user1@example.com",
                    Password = "hashedpassword",
                    Role = RoleUser.User,
                    Created = DateTime.UtcNow
                },
                new AuthUserEntity
                {
                    Pseudo = "User2",
                    Avatar = "avatar2.png",
                    Email = "user2@example.com",
                    Password = "hashedpassword",
                    Role = RoleUser.Admin,
                    Created = DateTime.UtcNow
                },
                new AuthUserEntity
                {
                    Pseudo = "User3",
                    Avatar = "avatar3.png",
                    Email = "user3@example.com",
                    Password = "hashedpassword",
                    Role = RoleUser.User,
                    Created = DateTime.UtcNow
                }
            );
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/AdminUser/SearchUser?pageIndex=0&pageSize=10&sort=pseudo_ascending&search=");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.True(response.Headers.Contains("X-Total-Count"));
            var totalCount = response.Headers.GetValues("X-Total-Count").FirstOrDefault();
            Assert.Equal("3", totalCount);
        }

        [Fact]
        public async Task SearchUser_WithPagination_ShouldReturnCorrectPage()
        {
            // Arrange
            for (int i = 0; i < 15; i++)
            {
                _context.AuthUsers.Add(new AuthUserEntity
                {
                    Pseudo = $"User{i:D2}",
                    Avatar = "avatar.png",
                    Email = $"user{i}@example.com",
                    Password = "hashedpassword",
                    Role = RoleUser.User,
                    Created = DateTime.UtcNow.AddDays(-i)
                });
            }
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/AdminUser/SearchUser?pageIndex=1&pageSize=5&sort=pseudo_ascending&search=");

            // Assert
            response.EnsureSuccessStatusCode();
            var users = JsonSerializer.Deserialize<List<UserAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(users);
            Assert.Equal(5, users.Count);
        }

        [Fact]
        public async Task SearchUser_WithEmptyDatabase_ShouldReturnEmptyList()
        {
            // Act
            var response = await _client.GetAsync("/api/AdminUser/SearchUser?pageIndex=0&pageSize=10&sort=pseudo_ascending&search=");

            // Assert
            response.EnsureSuccessStatusCode();
            var users = JsonSerializer.Deserialize<List<UserAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(users);
            Assert.Empty(users);
        }

        [Fact]
        public async Task SearchUser_WithSearchTerm_ShouldReturnMatchingUsers()
        {
            // Arrange
            _context.AuthUsers.AddRange(
                new AuthUserEntity
                {
                    Pseudo = "JohnDoe",
                    Avatar = "avatar1.png",
                    Email = "john@example.com",
                    Password = "hashedpassword",
                    Role = RoleUser.User,
                    Created = DateTime.UtcNow
                },
                new AuthUserEntity
                {
                    Pseudo = "JaneSmith",
                    Avatar = "avatar2.png",
                    Email = "jane@example.com",
                    Password = "hashedpassword",
                    Role = RoleUser.User,
                    Created = DateTime.UtcNow
                },
                new AuthUserEntity
                {
                    Pseudo = "Johnny",
                    Avatar = "avatar3.png",
                    Email = "johnny@example.com",
                    Password = "hashedpassword",
                    Role = RoleUser.User,
                    Created = DateTime.UtcNow
                }
            );
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/AdminUser/SearchUser?pageIndex=0&pageSize=10&sort=pseudo_ascending&search=john");

            // Assert
            response.EnsureSuccessStatusCode();
            var users = JsonSerializer.Deserialize<List<UserAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(users);
            Assert.Equal(2, users.Count);
            Assert.All(users, u => Assert.Contains("John", u.Pseudo, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task SearchUser_WithSearchTermInEmail_ShouldReturnMatchingUsers()
        {
            // Arrange
            _context.AuthUsers.AddRange(
                new AuthUserEntity
                {
                    Pseudo = "User1",
                    Avatar = "avatar1.png",
                    Email = "admin@company.com",
                    Password = "hashedpassword",
                    Role = RoleUser.User,
                    Created = DateTime.UtcNow
                },
                new AuthUserEntity
                {
                    Pseudo = "User2",
                    Avatar = "avatar2.png",
                    Email = "user@personal.com",
                    Password = "hashedpassword",
                    Role = RoleUser.User,
                    Created = DateTime.UtcNow
                }
            );
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/AdminUser/SearchUser?pageIndex=0&pageSize=10&sort=pseudo_ascending&search=company");

            // Assert
            response.EnsureSuccessStatusCode();
            var users = JsonSerializer.Deserialize<List<UserAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(users);
            Assert.Single(users);
            Assert.Equal("admin@company.com", users.First().Email);
        }

        #endregion

        #region SearchUser Tests - Tri

        [Fact]
        public async Task SearchUser_SortByPseudoAscending_ShouldReturnUsersInCorrectOrder()
        {
            // Arrange
            _context.AuthUsers.AddRange(
                new AuthUserEntity
                {
                    Pseudo = "Zebra",
                    Avatar = "avatar.png",
                    Email = "zebra@example.com",
                    Password = "hashedpassword",
                    Role = RoleUser.User,
                    Created = DateTime.UtcNow
                },
                new AuthUserEntity
                {
                    Pseudo = "Apple",
                    Avatar = "avatar.png",
                    Email = "apple@example.com",
                    Password = "hashedpassword",
                    Role = RoleUser.User,
                    Created = DateTime.UtcNow
                },
                new AuthUserEntity
                {
                    Pseudo = "Mango",
                    Avatar = "avatar.png",
                    Email = "mango@example.com",
                    Password = "hashedpassword",
                    Role = RoleUser.User,
                    Created = DateTime.UtcNow
                }
            );
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/AdminUser/SearchUser?pageIndex=0&pageSize=10&sort=pseudo_ascending&search=");

            // Assert
            response.EnsureSuccessStatusCode();
            var users = JsonSerializer.Deserialize<List<UserAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(users);
            Assert.Equal(3, users.Count);
            Assert.Equal("Apple", users[0].Pseudo);
            Assert.Equal("Mango", users[1].Pseudo);
            Assert.Equal("Zebra", users[2].Pseudo);
        }

        [Fact]
        public async Task SearchUser_SortByPseudoDescending_ShouldReturnUsersInCorrectOrder()
        {
            // Arrange
            _context.AuthUsers.AddRange(
                new AuthUserEntity
                {
                    Pseudo = "Zebra",
                    Avatar = "avatar.png",
                    Email = "zebra@example.com",
                    Password = "hashedpassword",
                    Role = RoleUser.User,
                    Created = DateTime.UtcNow
                },
                new AuthUserEntity
                {
                    Pseudo = "Apple",
                    Avatar = "avatar.png",
                    Email = "apple@example.com",
                    Password = "hashedpassword",
                    Role = RoleUser.User,
                    Created = DateTime.UtcNow
                }
            );
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/AdminUser/SearchUser?pageIndex=0&pageSize=10&sort=pseudo_descending&search=");

            // Assert
            response.EnsureSuccessStatusCode();
            var users = JsonSerializer.Deserialize<List<UserAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(users);
            Assert.Equal(2, users.Count);
            Assert.Equal("Zebra", users[0].Pseudo);
            Assert.Equal("Apple", users[1].Pseudo);
        }

        [Fact]
        public async Task SearchUser_SortByCreatedAscending_ShouldReturnUsersInCorrectOrder()
        {
            // Arrange
            var now = DateTime.UtcNow;
            _context.AuthUsers.AddRange(
                new AuthUserEntity
                {
                    Pseudo = "Third",
                    Avatar = "avatar.png",
                    Email = "third@example.com",
                    Password = "hashedpassword",
                    Role = RoleUser.User,
                    Created = now.AddDays(-1)
                },
                new AuthUserEntity
                {
                    Pseudo = "First",
                    Avatar = "avatar.png",
                    Email = "first@example.com",
                    Password = "hashedpassword",
                    Role = RoleUser.User,
                    Created = now.AddDays(-3)
                },
                new AuthUserEntity
                {
                    Pseudo = "Second",
                    Avatar = "avatar.png",
                    Email = "second@example.com",
                    Password = "hashedpassword",
                    Role = RoleUser.User,
                    Created = now.AddDays(-2)
                }
            );
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/AdminUser/SearchUser?pageIndex=0&pageSize=10&sort=created_ascending&search=");

            // Assert
            response.EnsureSuccessStatusCode();
            var users = JsonSerializer.Deserialize<List<UserAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(users);
            Assert.Equal(3, users.Count);
            Assert.Equal("First", users[0].Pseudo);
            Assert.Equal("Second", users[1].Pseudo);
            Assert.Equal("Third", users[2].Pseudo);
        }

        [Fact]
        public async Task SearchUser_SortByCreatedDescending_ShouldReturnUsersInCorrectOrder()
        {
            // Arrange
            var now = DateTime.UtcNow;
            _context.AuthUsers.AddRange(
                new AuthUserEntity
                {
                    Pseudo = "Third",
                    Avatar = "avatar.png",
                    Email = "third@example.com",
                    Password = "hashedpassword",
                    Role = RoleUser.User,
                    Created = now.AddDays(-1)
                },
                new AuthUserEntity
                {
                    Pseudo = "First",
                    Avatar = "avatar.png",
                    Email = "first@example.com",
                    Password = "hashedpassword",
                    Role = RoleUser.User,
                    Created = now.AddDays(-3)
                }
            );
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/AdminUser/SearchUser?pageIndex=0&pageSize=10&sort=created_descending&search=");

            // Assert
            response.EnsureSuccessStatusCode();
            var users = JsonSerializer.Deserialize<List<UserAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(users);
            Assert.Equal(2, users.Count);
            Assert.Equal("Third", users[0].Pseudo);
            Assert.Equal("First", users[1].Pseudo);
        }

        [Fact]
        public async Task SearchUser_SortByLastLoginAscending_ShouldReturnUsersInCorrectOrder()
        {
            // Arrange
            var now = DateTime.UtcNow;
            _context.AuthUsers.AddRange(
                new AuthUserEntity
                {
                    Pseudo = "RecentLogin",
                    Avatar = "avatar.png",
                    Email = "recent@example.com",
                    Password = "hashedpassword",
                    Role = RoleUser.User,
                    Created = now,
                    LastLogin = now.AddDays(-1)
                },
                new AuthUserEntity
                {
                    Pseudo = "OldLogin",
                    Avatar = "avatar.png",
                    Email = "old@example.com",
                    Password = "hashedpassword",
                    Role = RoleUser.User,
                    Created = now,
                    LastLogin = now.AddDays(-5)
                }
            );
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/AdminUser/SearchUser?pageIndex=0&pageSize=10&sort=lastlogin_ascending&search=");

            // Assert
            response.EnsureSuccessStatusCode();
            var users = JsonSerializer.Deserialize<List<UserAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(users);
            Assert.Equal(2, users.Count);
            Assert.Equal("OldLogin", users[0].Pseudo);
            Assert.Equal("RecentLogin", users[1].Pseudo);
        }

        [Fact]
        public async Task SearchUser_SortByLastLoginDescending_ShouldReturnUsersInCorrectOrder()
        {
            // Arrange
            var now = DateTime.UtcNow;
            _context.AuthUsers.AddRange(
                new AuthUserEntity
                {
                    Pseudo = "RecentLogin",
                    Avatar = "avatar.png",
                    Email = "recent@example.com",
                    Password = "hashedpassword",
                    Role = RoleUser.User,
                    Created = now,
                    LastLogin = now.AddDays(-1)
                },
                new AuthUserEntity
                {
                    Pseudo = "OldLogin",
                    Avatar = "avatar.png",
                    Email = "old@example.com",
                    Password = "hashedpassword",
                    Role = RoleUser.User,
                    Created = now,
                    LastLogin = now.AddDays(-5)
                }
            );
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/AdminUser/SearchUser?pageIndex=0&pageSize=10&sort=lastlogin_descending&search=");

            // Assert
            response.EnsureSuccessStatusCode();
            var users = JsonSerializer.Deserialize<List<UserAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(users);
            Assert.Equal(2, users.Count);
            Assert.Equal("RecentLogin", users[0].Pseudo);
            Assert.Equal("OldLogin", users[1].Pseudo);
        }

        #endregion

        #region SearchUser Tests - Cas d'Erreurs

        [Fact]
        public async Task SearchUser_WithNegativePageIndex_ShouldReturnBadRequest()
        {
            // Act
            var response = await _client.GetAsync("/api/AdminUser/SearchUser?pageIndex=-1&pageSize=10&sort=pseudo_ascending&search=");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task SearchUser_WithZeroPageSize_ShouldReturnBadRequest()
        {
            // Act
            var response = await _client.GetAsync("/api/AdminUser/SearchUser?pageIndex=0&pageSize=0&sort=pseudo_ascending&search=");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task SearchUser_WithNegativePageSize_ShouldReturnBadRequest()
        {
            // Act
            var response = await _client.GetAsync("/api/AdminUser/SearchUser?pageIndex=0&pageSize=-5&sort=pseudo_ascending&search=");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task SearchUser_WithInvalidSortFormat_ShouldReturnBadRequest()
        {
            // Act
            var response = await _client.GetAsync("/api/AdminUser/SearchUser?pageIndex=0&pageSize=10&sort=invalid_sort&search=");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task SearchUser_WithInvalidSortField_ShouldReturnBadRequest()
        {
            // Act
            var response = await _client.GetAsync("/api/AdminUser/SearchUser?pageIndex=0&pageSize=10&sort=invalidfield_ascending&search=");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task SearchUser_WithInvalidSortDirection_ShouldReturnBadRequest()
        {
            // Act
            var response = await _client.GetAsync("/api/AdminUser/SearchUser?pageIndex=0&pageSize=10&sort=pseudo_invalid&search=");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task SearchUser_WithEmptySortParameter_ShouldReturnBadRequest()
        {
            // Act
            var response = await _client.GetAsync("/api/AdminUser/SearchUser?pageIndex=0&pageSize=10&sort=&search=");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task SearchUser_WithMissingSeparator_ShouldReturnBadRequest()
        {
            // Act
            var response = await _client.GetAsync("/api/AdminUser/SearchUser?pageIndex=0&pageSize=10&sort=pseudoascending&search=");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        #endregion

        #region CreateUser Tests

        [Fact]
        public async Task CreateUser_ShouldReturnOk()
        {
            // Act
            var response = await _client.GetAsync("/api/AdminUser/CreateUser");

            // Assert
            response.EnsureSuccessStatusCode();
            var user = JsonSerializer.Deserialize<UserAdminVM>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(user);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnUserWithPseudoStartingWithUser()
        {
            // Act
            var response = await _client.GetAsync("/api/AdminUser/CreateUser");

            // Assert
            response.EnsureSuccessStatusCode();
            var user = JsonSerializer.Deserialize<UserAdminVM>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(user);
            Assert.StartsWith("User", user.Pseudo);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnUserWithCatAvatar()
        {
            // Act
            var response = await _client.GetAsync("/api/AdminUser/CreateUser");

            // Assert
            response.EnsureSuccessStatusCode();
            var user = JsonSerializer.Deserialize<UserAdminVM>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(user);
            Assert.Equal("cat.jpg", user.Avatar);
        }

        [Fact]
        public async Task CreateUser_ShouldPersistUserInDatabase()
        {
            // Act
            var response = await _client.GetAsync("/api/AdminUser/CreateUser");

            // Assert
            response.EnsureSuccessStatusCode();
            var user = JsonSerializer.Deserialize<UserAdminVM>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(user);

            var userInDb = _context.Users.FirstOrDefault(u => u.Id == user.Id);
            Assert.NotNull(userInDb);
            Assert.StartsWith("User", userInDb.Pseudo);
            Assert.Equal("cat.jpg", userInDb.Avatar);
        }

        [Fact]
        public async Task CreateUser_MultipleCalls_ShouldCreateDifferentUsers()
        {
            // Act
            var response1 = await _client.GetAsync("/api/AdminUser/CreateUser");
            var response2 = await _client.GetAsync("/api/AdminUser/CreateUser");
            var response3 = await _client.GetAsync("/api/AdminUser/CreateUser");

            // Assert
            response1.EnsureSuccessStatusCode();
            response2.EnsureSuccessStatusCode();
            response3.EnsureSuccessStatusCode();

            var user1 = JsonSerializer.Deserialize<UserAdminVM>(
                await response1.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            var user2 = JsonSerializer.Deserialize<UserAdminVM>(
                await response2.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            var user3 = JsonSerializer.Deserialize<UserAdminVM>(
                await response3.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            Assert.NotNull(user1);
            Assert.NotNull(user2);
            Assert.NotNull(user3);

            // All IDs should be different
            Assert.NotEqual(user1.Id, user2.Id);
            Assert.NotEqual(user2.Id, user3.Id);
            Assert.NotEqual(user1.Id, user3.Id);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnCorrectContentType()
        {
            // Act
            var response = await _client.GetAsync("/api/AdminUser/CreateUser");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public async Task SearchUser_WithLargePageIndex_ShouldReturnEmptyList()
        {
            // Arrange
            _context.AuthUsers.Add(new AuthUserEntity
            {
                Pseudo = "User1",
                Avatar = "avatar.png",
                Email = "user1@example.com",
                Password = "hashedpassword",
                Role = RoleUser.User,
                Created = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/AdminUser/SearchUser?pageIndex=100&pageSize=10&sort=pseudo_ascending&search=");

            // Assert
            response.EnsureSuccessStatusCode();
            var users = JsonSerializer.Deserialize<List<UserAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(users);
            Assert.Empty(users);
        }

        [Fact]
        public async Task SearchUser_WithSearchTermNoMatch_ShouldReturnEmptyList()
        {
            // Arrange
            _context.AuthUsers.Add(new AuthUserEntity
            {
                Pseudo = "User1",
                Avatar = "avatar.png",
                Email = "user1@example.com",
                Password = "hashedpassword",
                Role = RoleUser.User,
                Created = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/AdminUser/SearchUser?pageIndex=0&pageSize=10&sort=pseudo_ascending&search=nonexistent");

            // Assert
            response.EnsureSuccessStatusCode();
            var users = JsonSerializer.Deserialize<List<UserAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(users);
            Assert.Empty(users);
        }

        [Fact]
        public async Task SearchUser_ReturnsCorrectIdForEachUser()
        {
            // Arrange
            var user1 = new AuthUserEntity
            {
                Pseudo = "User1",
                Avatar = "avatar.png",
                Email = "user1@example.com",
                Password = "hashedpassword",
                Role = RoleUser.User,
                Created = DateTime.UtcNow
            };
            var user2 = new AuthUserEntity
            {
                Pseudo = "User2",
                Avatar = "avatar.png",
                Email = "user2@example.com",
                Password = "hashedpassword",
                Role = RoleUser.Admin,
                Created = DateTime.UtcNow
            };
            _context.AuthUsers.AddRange(user1, user2);
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/AdminUser/SearchUser?pageIndex=0&pageSize=10&sort=pseudo_ascending&search=");

            // Assert
            response.EnsureSuccessStatusCode();
            var users = JsonSerializer.Deserialize<List<UserAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(users);
            Assert.Equal(2, users.Count);
            Assert.All(users, u => Assert.NotEqual(Guid.Empty, u.Id));
            Assert.Contains(users, u => u.Pseudo == "User1");
            Assert.Contains(users, u => u.Pseudo == "User2");
        }

        #endregion
    }
}
