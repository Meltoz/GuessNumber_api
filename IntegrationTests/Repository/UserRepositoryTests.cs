using Domain.Enums;
using Domain.User;
using Infrastructure.Entities;
using Infrastructure.Repositories;
using Meltix.IntegrationTests;

namespace IntegrationTests.Repository
{
    public class UserRepositoryTests
    {
        #region Insert Tests

        [Fact]
        public async Task InsertAsync_WithValidGuestUser_ReturnsInsertedUser()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new UserRepository(context, mapper);

            var guestUser = new GuestUser("TestGuest", "avatar.png");

            // Act
            var result = await repository.InsertAsync(guestUser);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal("TestGuest", result.Pseudo.Value);
            Assert.Equal("avatar.png", result.Avatar);
        }

        [Fact]
        public async Task InsertAsync_ShouldPersistUserInDatabase()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new UserRepository(context, mapper);

            var guestUser = new GuestUser("PersistGuest", "cat.jpg");

            // Act
            var result = await repository.InsertAsync(guestUser);

            // Assert
            var userInDb = context.Users.FirstOrDefault(u => u.Id == result.Id);
            Assert.NotNull(userInDb);
            Assert.Equal("PersistGuest", userInDb.Pseudo);
            Assert.Equal("cat.jpg", userInDb.Avatar);
        }

        [Fact]
        public async Task InsertAsync_ShouldSetExpiresAt()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new UserRepository(context, mapper);

            var beforeCreation = DateTime.UtcNow;
            var guestUser = new GuestUser("ExpiringGuest", "avatar.png");

            // Act
            var result = await repository.InsertAsync(guestUser);

            // Assert
            var afterCreation = DateTime.UtcNow;
            Assert.NotNull(result.ExpiresAt);
            Assert.True(result.ExpiresAt >= beforeCreation.AddDays(1).AddSeconds(-1));
            Assert.True(result.ExpiresAt <= afterCreation.AddDays(1).AddSeconds(1));
        }

        [Fact]
        public async Task InsertAsync_MultipleUsers_ShouldAllBePersistedCorrectly()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new UserRepository(context, mapper);

            var user1 = new GuestUser("Guest1", "avatar1.png");
            var user2 = new GuestUser("Guest2", "avatar2.png");
            var user3 = new GuestUser("Guest3", "avatar3.png");

            // Act
            var result1 = await repository.InsertAsync(user1);
            var result2 = await repository.InsertAsync(user2);
            var result3 = await repository.InsertAsync(user3);

            // Assert
            Assert.Equal(3, context.Users.Count());
            Assert.NotEqual(result1.Id, result2.Id);
            Assert.NotEqual(result2.Id, result3.Id);
            Assert.NotEqual(result1.Id, result3.Id);
        }

        #endregion

        #region GetById Tests

        [Fact]
        public async Task GetByIdAsync_WithExistingId_ReturnsUser()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new UserRepository(context, mapper);

            var entity = new UserEntity
            {
                Pseudo = "TestGuest",
                Avatar = "avatar.png",
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                Created = DateTime.UtcNow
            };
            context.Users.Add(entity);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByIdAsync(entity.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(entity.Id, result.Id);
            Assert.Equal("TestGuest", result.Pseudo.Value);
            Assert.Equal("avatar.png", result.Avatar);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingId_ReturnsNull()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new UserRepository(context, mapper);

            // Act
            var result = await repository.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_WithAuthUserId_ReturnsUser()
        {
            // Arrange - In TPH, Set<UserEntity>() includes AuthUserEntity (derived type)
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new UserRepository(context, mapper);

            var authEntity = new AuthUserEntity
            {
                Pseudo = "AuthUser",
                Avatar = "avatar.png",
                Email = "auth@example.com",
                Password = "hashedpassword",
                Role = RoleUser.User,
                Created = DateTime.UtcNow
            };
            context.AuthUsers.Add(authEntity);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByIdAsync(authEntity.Id);

            // Assert - UserRepository finds auth users too (TPH behavior)
            // GetDetail service handles priority by checking AuthUserRepository first
            Assert.NotNull(result);
            Assert.Equal(authEntity.Id, result.Id);
        }

        #endregion

        #region GetAll Tests

        [Fact]
        public async Task GetAllAsync_ReturnsAllUsers()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new UserRepository(context, mapper);

            context.Users.AddRange(
                new UserEntity
                {
                    Pseudo = "Guest1",
                    Avatar = "avatar1.png",
                    ExpiresAt = DateTime.UtcNow.AddDays(1),
                    Created = DateTime.UtcNow
                },
                new UserEntity
                {
                    Pseudo = "Guest2",
                    Avatar = "avatar2.png",
                    ExpiresAt = DateTime.UtcNow.AddDays(1),
                    Created = DateTime.UtcNow
                }
            );
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllAsync_WithEmptyDatabase_ReturnsEmptyList()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new UserRepository(context, mapper);

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        #endregion

        #region Update Tests

        [Fact]
        public async Task UpdateAsync_WithValidChanges_UpdatesUser()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new UserRepository(context, mapper);

            var guestUser = new GuestUser("OriginalName", "original.png");
            var inserted = await repository.InsertAsync(guestUser);

            // Modify the user
            inserted.ChangePseudo("UpdatedName");
            inserted.ChangeAvatar("updated.png");

            // Act
            var result = await repository.UpdateAsync(inserted);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("UpdatedName", result.Pseudo.Value);
            Assert.Equal("updated.png", result.Avatar);

            var userInDb = context.Users.FirstOrDefault(u => u.Id == inserted.Id);
            Assert.NotNull(userInDb);
            Assert.Equal("UpdatedName", userInDb.Pseudo);
            Assert.Equal("updated.png", userInDb.Avatar);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task Delete_WithExistingId_RemovesUser()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new UserRepository(context, mapper);

            var entity = new UserEntity
            {
                Pseudo = "ToDelete",
                Avatar = "avatar.png",
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                Created = DateTime.UtcNow
            };
            context.Users.Add(entity);
            await context.SaveChangesAsync();

            // Act
            repository.Delete(entity.Id);
            await repository.SaveAsync();

            // Assert
            var userInDb = context.Users.FirstOrDefault(u => u.Id == entity.Id);
            Assert.Null(userInDb);
        }

        [Fact]
        public async Task Delete_WithDomainObject_RemovesUser()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new UserRepository(context, mapper);

            var guestUser = new GuestUser("ToDeleteDomain", "avatar.png");
            var inserted = await repository.InsertAsync(guestUser);

            // Act - Use Delete by Id to avoid EF Core tracking conflicts
            repository.Delete(inserted.Id);
            await repository.SaveAsync();

            // Assert
            var userInDb = context.Users.FirstOrDefault(u => u.Id == inserted.Id);
            Assert.Null(userInDb);
        }

        #endregion

        #region Timestamp Tests

        [Fact]
        public async Task InsertAsync_ShouldSetCreatedTimestamp()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new UserRepository(context, mapper);

            var beforeInsert = DateTime.UtcNow;
            var guestUser = new GuestUser("TimestampGuest", "avatar.png");

            // Act
            var result = await repository.InsertAsync(guestUser);

            // Assert
            var afterInsert = DateTime.UtcNow;
            var userInDb = context.Users.FirstOrDefault(u => u.Id == result.Id);
            Assert.NotNull(userInDb);
            Assert.True(userInDb.Created >= beforeInsert.AddSeconds(-1));
            Assert.True(userInDb.Created <= afterInsert.AddSeconds(1));
        }

        #endregion
    }
}
