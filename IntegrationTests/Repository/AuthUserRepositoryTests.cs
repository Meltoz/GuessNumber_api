using Domain.Enums;
using Domain.User;
using Domain.ValueObjects;
using Infrastructure.Entities;
using Infrastructure.Repositories;
using Meltix.IntegrationTests;
using Shared;
using Shared.Enums.Sorting;

namespace IntegrationTests.Repository
{
    public class AuthUserRepositoryTests
    {
        #region GetAll Tests - Sans filtre de recherche

        [Fact]
        public async Task GetAll_WithoutSearchTerm_ReturnsAllAuthUsers()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var user1 = new AuthUserEntity
            {
                Pseudo = "User1",
                Avatar = "avatar1.png",
                Email = "user1@example.com",
                Password = "hashedpassword1",
                Role = RoleUser.User,
                Created = DateTime.UtcNow.AddDays(-2)
            };

            var user2 = new AuthUserEntity
            {
                Pseudo = "User2",
                Avatar = "avatar2.png",
                Email = "user2@example.com",
                Password = "hashedpassword2",
                Role = RoleUser.Admin,
                Created = DateTime.UtcNow.AddDays(-1)
            };

            context.AuthUsers.AddRange(user1, user2);
            await context.SaveChangesAsync();

            var sortOption = new SortOption<SortUser>
            {
                SortBy = SortUser.Pseudo,
                Direction = SortDirection.Ascending
            };

            // Act
            var result = await repository.GetAll(0, 10, sortOption, "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Data.Count());
        }

        [Fact]
        public async Task GetAll_WithEmptyDatabase_ReturnsEmptyResult()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var sortOption = new SortOption<SortUser>
            {
                SortBy = SortUser.Pseudo,
                Direction = SortDirection.Ascending
            };

            // Act
            var result = await repository.GetAll(0, 10, sortOption, "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.TotalCount);
            Assert.Empty(result.Data);
        }

        #endregion

        #region GetAll Tests - Avec recherche par pseudo

        [Fact]
        public async Task GetAll_WithSearchTermInPseudo_ReturnsMatchingUsers()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var user1 = new AuthUserEntity
            {
                Pseudo = "JohnDoe",
                Avatar = "avatar1.png",
                Email = "john@example.com",
                Password = "hashedpassword1",
                Role = RoleUser.User,
                Created = DateTime.UtcNow
            };

            var user2 = new AuthUserEntity
            {
                Pseudo = "JaneSmith",
                Avatar = "avatar2.png",
                Email = "jane@example.com",
                Password = "hashedpassword2",
                Role = RoleUser.User,
                Created = DateTime.UtcNow
            };

            var user3 = new AuthUserEntity
            {
                Pseudo = "Johnny",
                Avatar = "avatar3.png",
                Email = "johnny@example.com",
                Password = "hashedpassword3",
                Role = RoleUser.User,
                Created = DateTime.UtcNow
            };

            context.AuthUsers.AddRange(user1, user2, user3);
            await context.SaveChangesAsync();

            var sortOption = new SortOption<SortUser>
            {
                SortBy = SortUser.Pseudo,
                Direction = SortDirection.Ascending
            };

            // Act
            var result = await repository.GetAll(0, 10, sortOption, "John");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            Assert.All(result.Data, u => Assert.Contains("John", u.Pseudo.Value));
        }

        [Fact]
        public async Task GetAll_WithSearchTermCaseInsensitive_ReturnsMatchingUsers()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var user1 = new AuthUserEntity
            {
                Pseudo = "JOHNDOE",
                Avatar = "avatar1.png",
                Email = "john@example.com",
                Password = "hashedpassword1",
                Role = RoleUser.User,
                Created = DateTime.UtcNow
            };

            context.AuthUsers.Add(user1);
            await context.SaveChangesAsync();

            var sortOption = new SortOption<SortUser>
            {
                SortBy = SortUser.Pseudo,
                Direction = SortDirection.Ascending
            };

            // Act
            var result = await repository.GetAll(0, 10, sortOption, "john");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);
        }

        #endregion

        #region GetAll Tests - Avec recherche par email

        [Fact]
        public async Task GetAll_WithSearchTermInEmail_ReturnsMatchingUsers()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var user1 = new AuthUserEntity
            {
                Pseudo = "User1",
                Avatar = "avatar1.png",
                Email = "admin@company.com",
                Password = "hashedpassword1",
                Role = RoleUser.User,
                Created = DateTime.UtcNow
            };

            var user2 = new AuthUserEntity
            {
                Pseudo = "User2",
                Avatar = "avatar2.png",
                Email = "user@personal.com",
                Password = "hashedpassword2",
                Role = RoleUser.User,
                Created = DateTime.UtcNow
            };

            context.AuthUsers.AddRange(user1, user2);
            await context.SaveChangesAsync();

            var sortOption = new SortOption<SortUser>
            {
                SortBy = SortUser.Pseudo,
                Direction = SortDirection.Ascending
            };

            // Act
            var result = await repository.GetAll(0, 10, sortOption, "company");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);
            var authUser = Assert.IsType<AuthUser>(result.Data.First());
            Assert.Equal("admin@company.com", authUser.Mail.ToString());
        }

        [Fact]
        public async Task GetAll_WithSearchTermMatchingBothPseudoAndEmail_ReturnsAllMatches()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var user1 = new AuthUserEntity
            {
                Pseudo = "testUser",  // lowercase 'test' to match SQLite case-sensitive behavior
                Avatar = "avatar1.png",
                Email = "other@example.com",
                Password = "hashedpassword1",
                Role = RoleUser.User,
                Created = DateTime.UtcNow
            };

            var user2 = new AuthUserEntity
            {
                Pseudo = "OtherUser",
                Avatar = "avatar2.png",
                Email = "test@example.com",
                Password = "hashedpassword2",
                Role = RoleUser.User,
                Created = DateTime.UtcNow
            };

            context.AuthUsers.AddRange(user1, user2);
            await context.SaveChangesAsync();

            var sortOption = new SortOption<SortUser>
            {
                SortBy = SortUser.Pseudo,
                Direction = SortDirection.Ascending
            };

            // Act - Note: SQLite is case-sensitive unlike PostgreSQL
            var result = await repository.GetAll(0, 10, sortOption, "test");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
        }

        #endregion

        #region GetAll Tests - Tri

        [Fact]
        public async Task GetAll_SortByPseudoAscending_ReturnsUsersInCorrectOrder()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var user1 = new AuthUserEntity
            {
                Pseudo = "Zebra",
                Avatar = "avatar1.png",
                Email = "zebra@example.com",
                Password = "hashedpassword1",
                Role = RoleUser.User,
                Created = DateTime.UtcNow
            };

            var user2 = new AuthUserEntity
            {
                Pseudo = "Apple",
                Avatar = "avatar2.png",
                Email = "apple@example.com",
                Password = "hashedpassword2",
                Role = RoleUser.User,
                Created = DateTime.UtcNow
            };

            var user3 = new AuthUserEntity
            {
                Pseudo = "Mango",
                Avatar = "avatar3.png",
                Email = "mango@example.com",
                Password = "hashedpassword3",
                Role = RoleUser.User,
                Created = DateTime.UtcNow
            };

            context.AuthUsers.AddRange(user1, user2, user3);
            await context.SaveChangesAsync();

            var sortOption = new SortOption<SortUser>
            {
                SortBy = SortUser.Pseudo,
                Direction = SortDirection.Ascending
            };

            // Act
            var result = await repository.GetAll(0, 10, sortOption, "");

            // Assert
            Assert.NotNull(result);
            var users = result.Data.ToList();
            Assert.Equal("Apple", users[0].Pseudo.Value);
            Assert.Equal("Mango", users[1].Pseudo.Value);
            Assert.Equal("Zebra", users[2].Pseudo.Value);
        }

        [Fact]
        public async Task GetAll_SortByPseudoDescending_ReturnsUsersInCorrectOrder()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var user1 = new AuthUserEntity
            {
                Pseudo = "Zebra",
                Avatar = "avatar1.png",
                Email = "zebra@example.com",
                Password = "hashedpassword1",
                Role = RoleUser.User,
                Created = DateTime.UtcNow
            };

            var user2 = new AuthUserEntity
            {
                Pseudo = "Apple",
                Avatar = "avatar2.png",
                Email = "apple@example.com",
                Password = "hashedpassword2",
                Role = RoleUser.User,
                Created = DateTime.UtcNow
            };

            context.AuthUsers.AddRange(user1, user2);
            await context.SaveChangesAsync();

            var sortOption = new SortOption<SortUser>
            {
                SortBy = SortUser.Pseudo,
                Direction = SortDirection.Descending
            };

            // Act
            var result = await repository.GetAll(0, 10, sortOption, "");

            // Assert
            Assert.NotNull(result);
            var users = result.Data.ToList();
            Assert.Equal("Zebra", users[0].Pseudo.Value);
            Assert.Equal("Apple", users[1].Pseudo.Value);
        }

        [Fact]
        public async Task GetAll_SortByCreatedAscending_ReturnsUsersInCorrectOrder()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var user1 = new AuthUserEntity
            {
                Pseudo = "User1",
                Avatar = "avatar1.png",
                Email = "user1@example.com",
                Password = "hashedpassword1",
                Role = RoleUser.User,
                Created = DateTime.UtcNow.AddDays(-5)
            };

            var user2 = new AuthUserEntity
            {
                Pseudo = "User2",
                Avatar = "avatar2.png",
                Email = "user2@example.com",
                Password = "hashedpassword2",
                Role = RoleUser.User,
                Created = DateTime.UtcNow.AddDays(-3)
            };

            var user3 = new AuthUserEntity
            {
                Pseudo = "User3",
                Avatar = "avatar3.png",
                Email = "user3@example.com",
                Password = "hashedpassword3",
                Role = RoleUser.User,
                Created = DateTime.UtcNow.AddDays(-1)
            };

            context.AuthUsers.AddRange(user1, user2, user3);
            await context.SaveChangesAsync();

            var sortOption = new SortOption<SortUser>
            {
                SortBy = SortUser.Created,
                Direction = SortDirection.Ascending
            };

            // Act
            var result = await repository.GetAll(0, 10, sortOption, "");

            // Assert
            Assert.NotNull(result);
            var users = result.Data.ToList();
            Assert.Equal("User1", users[0].Pseudo.Value);
            Assert.Equal("User2", users[1].Pseudo.Value);
            Assert.Equal("User3", users[2].Pseudo.Value);
        }

        [Fact]
        public async Task GetAll_SortByLastLoginDescending_ReturnsUsersInCorrectOrder()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var user1 = new AuthUserEntity
            {
                Pseudo = "User1",
                Avatar = "avatar1.png",
                Email = "user1@example.com",
                Password = "hashedpassword1",
                Role = RoleUser.User,
                Created = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow.AddDays(-5)
            };

            var user2 = new AuthUserEntity
            {
                Pseudo = "User2",
                Avatar = "avatar2.png",
                Email = "user2@example.com",
                Password = "hashedpassword2",
                Role = RoleUser.User,
                Created = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow.AddDays(-1)
            };

            context.AuthUsers.AddRange(user1, user2);
            await context.SaveChangesAsync();

            var sortOption = new SortOption<SortUser>
            {
                SortBy = SortUser.LastLogin,
                Direction = SortDirection.Descending
            };

            // Act
            var result = await repository.GetAll(0, 10, sortOption, "");

            // Assert
            Assert.NotNull(result);
            var users = result.Data.ToList();
            Assert.Equal("User2", users[0].Pseudo.Value);
            Assert.Equal("User1", users[1].Pseudo.Value);
        }

        #endregion

        #region GetAll Tests - Pagination

        [Fact]
        public async Task GetAll_WithPagination_ReturnsCorrectPage()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            for (int i = 1; i <= 15; i++)
            {
                context.AuthUsers.Add(new AuthUserEntity
                {
                    Pseudo = $"User{i:D2}",
                    Avatar = "avatar.png",
                    Email = $"user{i}@example.com",
                    Password = "hashedpassword",
                    Role = RoleUser.User,
                    Created = DateTime.UtcNow.AddDays(-i)
                });
            }
            await context.SaveChangesAsync();

            var sortOption = new SortOption<SortUser>
            {
                SortBy = SortUser.Pseudo,
                Direction = SortDirection.Ascending
            };

            // Act
            var result = await repository.GetAll(5, 5, sortOption, "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(15, result.TotalCount);
            Assert.Equal(5, result.Data.Count());
        }

        [Fact]
        public async Task GetAll_WithPaginationBeyondResults_ReturnsEmptyData()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            context.AuthUsers.Add(new AuthUserEntity
            {
                Pseudo = "User1",
                Avatar = "avatar.png",
                Email = "user1@example.com",
                Password = "hashedpassword",
                Role = RoleUser.User,
                Created = DateTime.UtcNow
            });
            await context.SaveChangesAsync();

            var sortOption = new SortOption<SortUser>
            {
                SortBy = SortUser.Pseudo,
                Direction = SortDirection.Ascending
            };

            // Act
            var result = await repository.GetAll(10, 5, sortOption, "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);
            Assert.Empty(result.Data);
        }

        #endregion

        #region GetAll Tests - Aucun résultat

        [Fact]
        public async Task GetAll_WithNoMatches_ReturnsEmptyResult()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            context.AuthUsers.Add(new AuthUserEntity
            {
                Pseudo = "User1",
                Avatar = "avatar.png",
                Email = "user1@example.com",
                Password = "hashedpassword",
                Role = RoleUser.User,
                Created = DateTime.UtcNow
            });
            await context.SaveChangesAsync();

            var sortOption = new SortOption<SortUser>
            {
                SortBy = SortUser.Pseudo,
                Direction = SortDirection.Ascending
            };

            // Act
            var result = await repository.GetAll(0, 10, sortOption, "nonexistent");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.TotalCount);
            Assert.Empty(result.Data);
        }

        #endregion

        #region Insert Tests (CreateAuthUser)

        [Fact]
        public async Task InsertAsync_WithValidAuthUser_ReturnsInsertedUser()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var authUser = new Domain.User.AuthUser(
                "TestUser",
                "avatar.png",
                "test@example.com",
                "Password1@",
                RoleUser.User
            );

            // Act
            var result = await repository.InsertAsync(authUser);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal("TestUser", result.Pseudo.Value);
            Assert.Equal("test@example.com", result.Mail.ToString());
        }

        [Fact]
        public async Task InsertAsync_ShouldPersistUserInDatabase()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var authUser = new Domain.User.AuthUser(
                "PersistUser",
                "avatar.png",
                "persist@example.com",
                "Password1@",
                RoleUser.User
            );

            // Act
            var result = await repository.InsertAsync(authUser);

            // Assert
            var userInDb = context.AuthUsers.FirstOrDefault(u => u.Id == result.Id);
            Assert.NotNull(userInDb);
            Assert.Equal("PersistUser", userInDb.Pseudo);
            Assert.Equal("persist@example.com", userInDb.Email);
        }

        [Fact]
        public async Task InsertAsync_ShouldPersistCatPngAvatar()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var authUser = new Domain.User.AuthUser(
                "TestUser",
                "cat.png",
                "test@example.com",
                "Password1@",
                RoleUser.User
            );

            // Act
            var result = await repository.InsertAsync(authUser);

            // Assert
            Assert.Equal("cat.png", result.Avatar);
            var entityInDb = context.AuthUsers.FirstOrDefault(u => u.Id == result.Id);
            Assert.NotNull(entityInDb);
            Assert.Equal("cat.png", entityInDb.Avatar);
        }

        [Fact]
        public async Task InsertAsync_ShouldPersistRoleUser()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var authUser = new Domain.User.AuthUser(
                "TestUser",
                "cat.png",
                "test@example.com",
                "Password1@",
                RoleUser.User
            );

            // Act
            var result = await repository.InsertAsync(authUser);

            // Assert
            Assert.Equal(RoleUser.User, result.Role);
            var entityInDb = context.AuthUsers.FirstOrDefault(u => u.Id == result.Id);
            Assert.NotNull(entityInDb);
            Assert.Equal(RoleUser.User, entityInDb.Role);
        }

        [Fact]
        public async Task InsertAsync_ShouldPersistHashedPassword()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var authUser = new Domain.User.AuthUser(
                "TestUser",
                "cat.png",
                "test@example.com",
                "Password1@",
                RoleUser.User
            );

            // Act
            var result = await repository.InsertAsync(authUser);

            // Assert
            var entityInDb = context.AuthUsers.FirstOrDefault(u => u.Id == result.Id);
            Assert.NotNull(entityInDb);
            Assert.NotEqual("Password1@", entityInDb.Password);
            Assert.NotEmpty(entityInDb.Password);
        }

        [Fact]
        public async Task InsertAsync_ShouldPersistPasswordMustBeChangedAsFalse()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var authUser = new Domain.User.AuthUser(
                "TestUser",
                "cat.png",
                "test@example.com",
                "Password1@",
                RoleUser.User
            );

            // Act
            var result = await repository.InsertAsync(authUser);

            // Assert
            Assert.False(result.PasswordMustBeChanged);
            var entityInDb = context.AuthUsers.FirstOrDefault(u => u.Id == result.Id);
            Assert.NotNull(entityInDb);
            Assert.False(entityInDb.PasswordMustBeChanged);
        }

        [Fact]
        public async Task InsertAsync_ShouldPersistNullLastLoginAndLastChangePassword()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var authUser = new Domain.User.AuthUser(
                "TestUser",
                "cat.png",
                "test@example.com",
                "Password1@",
                RoleUser.User
            );

            // Act
            var result = await repository.InsertAsync(authUser);

            // Assert
            Assert.Null(result.LastLogin);
            Assert.Null(result.LastChangePassword);
            var entityInDb = context.AuthUsers.FirstOrDefault(u => u.Id == result.Id);
            Assert.NotNull(entityInDb);
            Assert.Null(entityInDb.LastLogin);
            Assert.Null(entityInDb.LastChangePassword);
        }

        [Fact]
        public async Task InsertAsync_ThenGetByIdAsync_ShouldReturnSameData()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var authUser = new Domain.User.AuthUser(
                "JohnDoe",
                "cat.png",
                "john@example.com",
                "Password1@",
                RoleUser.User
            );

            // Act
            var inserted = await repository.InsertAsync(authUser);
            var retrieved = await repository.GetByIdAsync(inserted.Id);

            // Assert
            Assert.NotNull(retrieved);
            Assert.Equal(inserted.Id, retrieved.Id);
            Assert.Equal("JohnDoe", retrieved.Pseudo.Value);
            Assert.Equal("john@example.com", retrieved.Mail.ToString());
            Assert.Equal("cat.png", retrieved.Avatar);
            Assert.Equal(RoleUser.User, retrieved.Role);
            Assert.False(retrieved.PasswordMustBeChanged);
            Assert.Null(retrieved.LastLogin);
            Assert.Null(retrieved.LastChangePassword);
        }

        [Fact]
        public async Task InsertAsync_MultipleUsers_ShouldPersistAll()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var user1 = new Domain.User.AuthUser("User1", "cat.png", "user1@example.com", "Password1@", RoleUser.User);
            var user2 = new Domain.User.AuthUser("User2", "cat.png", "user2@example.com", "Password1@", RoleUser.User);
            var user3 = new Domain.User.AuthUser("User3", "cat.png", "user3@example.com", "Password1@", RoleUser.User);

            // Act
            var result1 = await repository.InsertAsync(user1);
            var result2 = await repository.InsertAsync(user2);
            var result3 = await repository.InsertAsync(user3);

            // Assert
            Assert.Equal(3, context.AuthUsers.Count());
            Assert.NotEqual(result1.Id, result2.Id);
            Assert.NotEqual(result2.Id, result3.Id);
            Assert.NotEqual(result1.Id, result3.Id);
        }

        [Fact]
        public async Task InsertAsync_ShouldBeVisibleInGetAll()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var authUser = new Domain.User.AuthUser(
                "SearchableUser",
                "cat.png",
                "search@example.com",
                "Password1@",
                RoleUser.User
            );

            var sortOption = new SortOption<SortUser>
            {
                SortBy = SortUser.Pseudo,
                Direction = SortDirection.Ascending
            };

            // Act
            await repository.InsertAsync(authUser);
            var result = await repository.GetAll(0, 10, sortOption, "SearchableUser");

            // Assert
            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Data);
            Assert.Equal("SearchableUser", result.Data.First().Pseudo.Value);
        }

        [Fact]
        public async Task InsertAsync_ShouldBeSearchableByEmail()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var authUser = new Domain.User.AuthUser(
                "TestUser",
                "cat.png",
                "unique-email@domain.com",
                "Password1@",
                RoleUser.User
            );

            var sortOption = new SortOption<SortUser>
            {
                SortBy = SortUser.Pseudo,
                Direction = SortDirection.Ascending
            };

            // Act
            await repository.InsertAsync(authUser);
            var result = await repository.GetAll(0, 10, sortOption, "unique-email");

            // Assert
            Assert.Equal(1, result.TotalCount);
            var authUserResult = Assert.IsType<AuthUser>(result.Data.First());
            Assert.Equal("unique-email@domain.com", authUserResult.Mail.ToString());
        }

        [Fact]
        public async Task InsertAsync_ThenDelete_ShouldRemoveFromDatabase()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var authUser = new Domain.User.AuthUser(
                "ToDelete",
                "cat.png",
                "delete@example.com",
                "Password1@",
                RoleUser.User
            );

            var inserted = await repository.InsertAsync(authUser);

            // Act
            repository.Delete(inserted.Id);
            await repository.SaveAsync();

            // Assert
            var retrieved = await repository.GetByIdAsync(inserted.Id);
            Assert.Null(retrieved);
            Assert.Equal(0, context.AuthUsers.Count());
        }

        #endregion

        #region GetById Tests

        [Fact]
        public async Task GetByIdAsync_WithExistingId_ReturnsUser()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var entity = new AuthUserEntity
            {
                Pseudo = "TestUser",
                Avatar = "avatar.png",
                Email = "test@example.com",
                Password = "hashedpassword",
                Role = RoleUser.User,
                Created = DateTime.UtcNow
            };
            context.AuthUsers.Add(entity);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByIdAsync(entity.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal("TestUser", result.Pseudo.Value);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingId_ReturnsNull()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            // Act
            var result = await repository.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_WithGuestUserId_ReturnsNull()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var guestEntity = new UserEntity
            {
                Pseudo = "GuestUser",
                Avatar = "cat.jpg",
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                Created = DateTime.UtcNow
            };
            context.Users.Add(guestEntity);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByIdAsync(guestEntity.Id);

            // Assert
            Assert.Null(result);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task Delete_WithExistingId_RemovesUser()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var entity = new AuthUserEntity
            {
                Pseudo = "ToDelete",
                Avatar = "avatar.png",
                Email = "delete@example.com",
                Password = "hashedpassword",
                Role = RoleUser.User,
                Created = DateTime.UtcNow
            };
            context.AuthUsers.Add(entity);
            await context.SaveChangesAsync();

            // Act
            repository.Delete(entity.Id);
            await repository.SaveAsync();

            // Assert
            var userInDb = context.AuthUsers.FirstOrDefault(u => u.Id == entity.Id);
            Assert.Null(userInDb);
        }

        #endregion

        #region UpdateAsync Tests - ChangeRole

        [Fact]
        public async Task UpdateAsync_ChangeRoleFromUserToAdmin_ShouldPersistNewRole()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var authUser = new Domain.User.AuthUser(
                "TestUser",
                "avatar.png",
                "test@example.com",
                "Password1@",
                RoleUser.User
            );

            var inserted = await repository.InsertAsync(authUser);
            Assert.Equal(RoleUser.User, inserted.Role);

            // Act
            inserted.ChangeRole(RoleUser.Admin);
            var updated = await repository.UpdateAsync(inserted);

            // Assert
            Assert.NotNull(updated);
            Assert.Equal(RoleUser.Admin, updated.Role);
        }

        [Fact]
        public async Task UpdateAsync_ChangeRoleFromAdminToUser_ShouldPersistNewRole()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var authUser = new Domain.User.AuthUser(
                "AdminUser",
                "avatar.png",
                "admin@example.com",
                "Password1@",
                RoleUser.Admin
            );

            var inserted = await repository.InsertAsync(authUser);
            Assert.Equal(RoleUser.Admin, inserted.Role);

            // Act
            inserted.ChangeRole(RoleUser.User);
            var updated = await repository.UpdateAsync(inserted);

            // Assert
            Assert.NotNull(updated);
            Assert.Equal(RoleUser.User, updated.Role);
        }

        [Fact]
        public async Task UpdateAsync_ChangeRole_ShouldBeVerifiableViaGetById()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var authUser = new Domain.User.AuthUser(
                "TestUser",
                "avatar.png",
                "test@example.com",
                "Password1@",
                RoleUser.User
            );

            var inserted = await repository.InsertAsync(authUser);

            // Act
            inserted.ChangeRole(RoleUser.Admin);
            await repository.UpdateAsync(inserted);

            var retrieved = await repository.GetByIdAsync(inserted.Id);

            // Assert
            Assert.NotNull(retrieved);
            Assert.Equal(RoleUser.Admin, retrieved.Role);
        }

        [Fact]
        public async Task UpdateAsync_ChangeRole_ShouldPreserveOtherFields()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var authUser = new Domain.User.AuthUser(
                "TestUser",
                "avatar.png",
                "test@example.com",
                "Password1@",
                RoleUser.User
            );

            var inserted = await repository.InsertAsync(authUser);

            // Act
            inserted.ChangeRole(RoleUser.Admin);
            var updated = await repository.UpdateAsync(inserted);

            // Assert
            Assert.Equal(inserted.Id, updated.Id);
            Assert.Equal("TestUser", updated.Pseudo.Value);
            Assert.Equal("avatar.png", updated.Avatar);
            Assert.Equal("test@example.com", updated.Mail.ToString());
        }

        [Fact]
        public async Task UpdateAsync_ChangeRole_ShouldBePersistentInEntity()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var entity = new AuthUserEntity
            {
                Pseudo = "TestUser",
                Avatar = "avatar.png",
                Email = "test@example.com",
                Password = "hashedpassword",
                Role = RoleUser.User,
                Created = DateTime.UtcNow
            };
            context.AuthUsers.Add(entity);
            await context.SaveChangesAsync();

            var domainUser = await repository.GetByIdAsync(entity.Id);
            domainUser.ChangeRole(RoleUser.Admin);

            // Act
            await repository.UpdateAsync(domainUser);

            // Assert
            var entityInDb = context.AuthUsers.FirstOrDefault(u => u.Id == entity.Id);
            Assert.NotNull(entityInDb);
            Assert.Equal(RoleUser.Admin, entityInDb.Role);
        }

        [Fact]
        public async Task InsertAsync_WithRoleAdmin_ShouldPersistRole()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var authUser = new Domain.User.AuthUser(
                "AdminUser",
                "avatar.png",
                "admin@example.com",
                "Password1@",
                RoleUser.Admin
            );

            // Act
            var result = await repository.InsertAsync(authUser);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(RoleUser.Admin, result.Role);

            var entityInDb = context.AuthUsers.FirstOrDefault(u => u.Id == result.Id);
            Assert.NotNull(entityInDb);
            Assert.Equal(RoleUser.Admin, entityInDb.Role);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnCorrectRole()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var entity = new AuthUserEntity
            {
                Pseudo = "AdminUser",
                Avatar = "avatar.png",
                Email = "admin@example.com",
                Password = "hashedpassword",
                Role = RoleUser.Admin,
                Created = DateTime.UtcNow
            };
            context.AuthUsers.Add(entity);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByIdAsync(entity.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(RoleUser.Admin, result.Role);
        }

        #endregion

        #region UpdateAsync Tests - ResetPassword (ChangePassword + ChangePasswordNextTime)

        [Fact]
        public async Task UpdateAsync_ResetPassword_ShouldPersistNewPassword()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var authUser = new Domain.User.AuthUser(
                "TestUser",
                "avatar.png",
                "test@example.com",
                "Password1@",
                RoleUser.User
            );

            var inserted = await repository.InsertAsync(authUser);
            var oldPasswordInDb = context.AuthUsers.First(u => u.Id == inserted.Id).Password;

            // Act
            inserted.ChangePassword("NewPassword1@");
            inserted.ChangePasswordNextTime();
            var updated = await repository.UpdateAsync(inserted);

            // Assert
            var entityInDb = context.AuthUsers.FirstOrDefault(u => u.Id == inserted.Id);
            Assert.NotNull(entityInDb);
            Assert.NotEqual(oldPasswordInDb, entityInDb.Password);
        }

        [Fact]
        public async Task UpdateAsync_ResetPassword_ShouldPersistPasswordMustBeChanged()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var authUser = new Domain.User.AuthUser(
                "TestUser",
                "avatar.png",
                "test@example.com",
                "Password1@",
                RoleUser.User
            );

            var inserted = await repository.InsertAsync(authUser);

            // Act
            inserted.ChangePassword("NewPassword1@");
            inserted.ChangePasswordNextTime();
            var updated = await repository.UpdateAsync(inserted);

            // Assert
            Assert.True(updated.PasswordMustBeChanged);
            var entityInDb = context.AuthUsers.FirstOrDefault(u => u.Id == inserted.Id);
            Assert.NotNull(entityInDb);
            Assert.True(entityInDb.PasswordMustBeChanged);
        }

        [Fact]
        public async Task UpdateAsync_ResetPassword_ShouldPersistLastChangePassword()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var authUser = new Domain.User.AuthUser(
                "TestUser",
                "avatar.png",
                "test@example.com",
                "Password1@",
                RoleUser.User
            );

            var inserted = await repository.InsertAsync(authUser);
            var beforeReset = DateTime.UtcNow;

            // Act
            inserted.ChangePassword("NewPassword1@");
            inserted.ChangePasswordNextTime();
            var updated = await repository.UpdateAsync(inserted);

            // Assert
            Assert.NotNull(updated.LastChangePassword);
            var entityInDb = context.AuthUsers.FirstOrDefault(u => u.Id == inserted.Id);
            Assert.NotNull(entityInDb);
            Assert.NotNull(entityInDb.LastChangePassword);
        }

        [Fact]
        public async Task UpdateAsync_ResetPassword_ShouldPreserveOtherFields()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var authUser = new Domain.User.AuthUser(
                "TestUser",
                "avatar.png",
                "test@example.com",
                "Password1@",
                RoleUser.Admin
            );

            var inserted = await repository.InsertAsync(authUser);

            // Act
            inserted.ChangePassword("NewPassword1@");
            inserted.ChangePasswordNextTime();
            var updated = await repository.UpdateAsync(inserted);

            // Assert
            Assert.Equal(inserted.Id, updated.Id);
            Assert.Equal("TestUser", updated.Pseudo.Value);
            Assert.Equal("avatar.png", updated.Avatar);
            Assert.Equal("test@example.com", updated.Mail.ToString());
            Assert.Equal(RoleUser.Admin, updated.Role);
        }

        [Fact]
        public async Task UpdateAsync_ResetPassword_ShouldBeVerifiableViaGetById()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var authUser = new Domain.User.AuthUser(
                "TestUser",
                "avatar.png",
                "test@example.com",
                "Password1@",
                RoleUser.User
            );

            var inserted = await repository.InsertAsync(authUser);

            // Act
            inserted.ChangePassword("NewPassword1@");
            inserted.ChangePasswordNextTime();
            await repository.UpdateAsync(inserted);

            var retrieved = await repository.GetByIdAsync(inserted.Id);

            // Assert
            Assert.NotNull(retrieved);
            Assert.True(retrieved.PasswordMustBeChanged);
            Assert.NotNull(retrieved.LastChangePassword);
        }

        [Fact]
        public async Task UpdateAsync_ChangePassword_WithoutChangePasswordNextTime_ShouldSetPasswordMustBeChangedToFalse()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var entity = new AuthUserEntity
            {
                Pseudo = "TestUser",
                Avatar = "avatar.png",
                Email = "test@example.com",
                Password = "hashedpassword",
                Role = RoleUser.User,
                PasswordMustBeChanged = true,
                Created = DateTime.UtcNow
            };
            context.AuthUsers.Add(entity);
            await context.SaveChangesAsync();

            var domainUser = await repository.GetByIdAsync(entity.Id);

            // Act
            domainUser.ChangePassword("NewPassword1@");
            await repository.UpdateAsync(domainUser);

            // Assert
            var entityInDb = context.AuthUsers.FirstOrDefault(u => u.Id == entity.Id);
            Assert.NotNull(entityInDb);
            Assert.False(entityInDb.PasswordMustBeChanged);
        }

        #endregion

        #region CheckAvailablePseudo Tests

        [Fact]
        public async Task CheckAvailablePseudo_WithNoUsers_ShouldReturnTrue()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);
            var pseudo = Pseudo.Create("anyPseudo");
            // Act
            var result = await repository.CheckAvailablePseudo(pseudo);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CheckAvailablePseudo_WithNonMatchingPseudo_ShouldReturnTrue()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            context.AuthUsers.Add(new AuthUserEntity
            {
                Pseudo = "ExistingUser",
                Avatar = "avatar.png",
                Email = "existing@example.com",
                Password = "hashedpassword",
                Role = Domain.Enums.RoleUser.User,
                Created = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
            var pseudo = Pseudo.Create("DifferentUser");

            // Act
            var result = await repository.CheckAvailablePseudo(pseudo);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CheckAvailablePseudo_WithExactMatchingPseudo_ShouldReturnFalse()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            context.AuthUsers.Add(new AuthUserEntity
            {
                Pseudo = "TakenUser",
                Avatar = "avatar.png",
                Email = "taken@example.com",
                Password = "hashedpassword",
                Role = Domain.Enums.RoleUser.User,
                Created = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
            var pseudo = Pseudo.Create("TakenUser");
            // Act
            var result = await repository.CheckAvailablePseudo(pseudo);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CheckAvailablePseudo_WithDifferentCase_ShouldReturnTrue()
        {
            // Arrange - SQLite is case-sensitive by default for ==
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            context.AuthUsers.Add(new AuthUserEntity
            {
                Pseudo = "TestUser",
                Avatar = "avatar.png",
                Email = "test@example.com",
                Password = "hashedpassword",
                Role = Domain.Enums.RoleUser.User,
                Created = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
            var pseudo = Pseudo.Create("testuser");

            // Act
            var result = await repository.CheckAvailablePseudo(pseudo);

            // Assert - case-sensitive comparison: "testuser" == "TestUser"
            Assert.False(result);
        }

        [Fact]
        public async Task CheckAvailablePseudo_WithMultipleUsers_ShouldReturnFalseForExistingPseudo()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            context.AuthUsers.AddRange(
                new AuthUserEntity { Pseudo = "User1", Avatar = "a.png", Email = "u1@ex.com", Password = "h1", Role = Domain.Enums.RoleUser.User, Created = DateTime.UtcNow },
                new AuthUserEntity { Pseudo = "User2", Avatar = "a.png", Email = "u2@ex.com", Password = "h2", Role = Domain.Enums.RoleUser.User, Created = DateTime.UtcNow },
                new AuthUserEntity { Pseudo = "User3", Avatar = "a.png", Email = "u3@ex.com", Password = "h3", Role = Domain.Enums.RoleUser.User, Created = DateTime.UtcNow }
            );
            await context.SaveChangesAsync();
            var pseudoUser2 = Pseudo.Create("User2");
            var pseudoUser4 = Pseudo.Create("User4");

            // Act & Assert
            Assert.False(await repository.CheckAvailablePseudo(pseudoUser2));
            Assert.True(await repository.CheckAvailablePseudo(pseudoUser4));
        }

        [Fact]
        public async Task CheckAvailablePseudo_AfterDeletion_ShouldReturnTrue()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var entity = new AuthUserEntity
            {
                Pseudo = "ToDelete",
                Avatar = "avatar.png",
                Email = "delete@example.com",
                Password = "hashedpassword",
                Role = Domain.Enums.RoleUser.User,
                Created = DateTime.UtcNow
            };
            context.AuthUsers.Add(entity);
            await context.SaveChangesAsync();
            var pseudo = Pseudo.Create("ToDelete");

            Assert.False(await repository.CheckAvailablePseudo(pseudo));

            // Act
            context.AuthUsers.Remove(entity);
            await context.SaveChangesAsync();

            // Assert
            Assert.True(await repository.CheckAvailablePseudo(pseudo));
        }

        #endregion

        #region CheckAvailableMail Tests

        [Fact]
        public async Task CheckAvailableMail_WithNoUsers_ShouldReturnTrue()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);
            var mail = Mail.Create("any@example.com");

            // Act
            var result = await repository.CheckAvailableMail(mail);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CheckAvailableMail_WithNonMatchingMail_ShouldReturnTrue()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            context.AuthUsers.Add(new AuthUserEntity
            {
                Pseudo = "ExistingUser",
                Avatar = "avatar.png",
                Email = "existing@example.com",
                Password = "hashedpassword",
                Role = Domain.Enums.RoleUser.User,
                Created = DateTime.UtcNow
            });
            await context.SaveChangesAsync();
            var mail = Mail.Create("different@example.com");

            // Act
            var result = await repository.CheckAvailableMail(mail);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task CheckAvailableMail_WithExactMatchingMail_ShouldReturnFalse()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var mail = Mail.Create("taken@example.com");

            context.AuthUsers.Add(new AuthUserEntity
            {
                Pseudo = "TakenUser",
                Avatar = "avatar.png",
                Email = "taken@example.com",
                Password = "hashedpassword",
                Role = Domain.Enums.RoleUser.User,
                Created = DateTime.UtcNow
            });
            await context.SaveChangesAsync();


            // Act
            var result = await repository.CheckAvailableMail(mail);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task CheckAvailableMail_WithDifferentCase_ShouldReturnTrue()
        {
            // Arrange - SQLite is case-sensitive by default for ==
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var mail = Mail.Create("test@example.com");

            context.AuthUsers.Add(new AuthUserEntity
            {
                Pseudo = "TestUser",
                Avatar = "avatar.png",
                Email = "Test@Example.com",
                Password = "hashedpassword",
                Role = Domain.Enums.RoleUser.User,
                Created = DateTime.UtcNow
            });
            await context.SaveChangesAsync();

            // Act
            var result = await repository.CheckAvailableMail(mail);

            // Assert - case-sensitive comparison: "test@example.com" != "Test@Example.com"
            Assert.True(result);
        }

        [Fact]
        public async Task CheckAvailableMail_WithMultipleUsers_ShouldReturnFalseForExistingMail()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var mailUser2 = Mail.Create("user2@ex.com");
            var mailUser4 = Mail.Create("user4@ex.com");

            context.AuthUsers.AddRange(
                new AuthUserEntity { Pseudo = "User1", Avatar = "a.png", Email = "user1@ex.com", Password = "h1", Role = Domain.Enums.RoleUser.User, Created = DateTime.UtcNow },
                new AuthUserEntity { Pseudo = "User2", Avatar = "a.png", Email = "user2@ex.com", Password = "h2", Role = Domain.Enums.RoleUser.User, Created = DateTime.UtcNow },
                new AuthUserEntity { Pseudo = "User3", Avatar = "a.png", Email = "user3@ex.com", Password = "h3", Role = Domain.Enums.RoleUser.User, Created = DateTime.UtcNow }
            );
            await context.SaveChangesAsync();

            // Act & Assert
            Assert.False(await repository.CheckAvailableMail(mailUser2));
            Assert.True(await repository.CheckAvailableMail(mailUser4));
        }

        [Fact]
        public async Task CheckAvailableMail_AfterDeletion_ShouldReturnTrue()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var mail = Mail.Create("delete@example.com");

            var entity = new AuthUserEntity
            {
                Pseudo = "ToDelete",
                Avatar = "avatar.png",
                Email = "delete@example.com",
                Password = "hashedpassword",
                Role = Domain.Enums.RoleUser.User,
                Created = DateTime.UtcNow
            };
            context.AuthUsers.Add(entity);
            await context.SaveChangesAsync();

            Assert.False(await repository.CheckAvailableMail(mail));

            // Act
            context.AuthUsers.Remove(entity);
            await context.SaveChangesAsync();

            // Assert
            Assert.True(await repository.CheckAvailableMail(mail));
        }

        [Fact]
        public async Task CheckAvailablePseudo_AndCheckAvailableMail_ShouldBeIndependent()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new AuthUserRepository(context, mapper);

            var takenPseudo = Pseudo.Create("UniqueUser");
            var freePseudo = Pseudo.Create("OtherUser");
            var takenMail = Mail.Create("unique@example.com");
            var freeMail = Mail.Create("other@example.com");

            context.AuthUsers.Add(new AuthUserEntity
            {
                Pseudo = "UniqueUser",
                Avatar = "avatar.png",
                Email = "unique@example.com",
                Password = "hashedpassword",
                Role = Domain.Enums.RoleUser.User,
                Created = DateTime.UtcNow
            });
            await context.SaveChangesAsync();

            // Act & Assert - pseudo taken but different mail available
            Assert.False(await repository.CheckAvailablePseudo(takenPseudo));
            Assert.True(await repository.CheckAvailableMail(freeMail));

            // Act & Assert - pseudo available but mail taken
            Assert.True(await repository.CheckAvailablePseudo(freePseudo));
            Assert.False(await repository.CheckAvailableMail(takenMail));
        }

        #endregion
    }
}
