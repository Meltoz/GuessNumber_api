using Domain.Enums;
using Domain.User;
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

        #region Insert Tests

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
    }
}
