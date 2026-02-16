using Domain.Enums;
using Domain.User;
using Infrastructure.Entities;
using Infrastructure.Repositories;
using Meltix.IntegrationTests;

namespace IntegrationTests.Repository
{
    public class TokenRepositoryTests
    {
        private AuthUserEntity CreateAuthUserEntity(string pseudo = "TestUser", string email = "test@example.com")
        {
            return new AuthUserEntity
            {
                Pseudo = pseudo,
                Avatar = "avatar.png",
                Email = email,
                Password = "hashedpassword",
                Role = RoleUser.User,
                Created = DateTime.UtcNow
            };
        }

        private TokenEntity CreateTokenEntity(Guid userId, string device = "Chrome", bool isRevoked = false)
        {
            return new TokenEntity
            {
                AccessToken = "access-token-" + Guid.NewGuid(),
                RefreshToken = "refresh-token-" + Guid.NewGuid(),
                IsRevoked = isRevoked,
                AccessExpiresAt = DateTime.UtcNow.AddMinutes(30),
                RefreshExpiresAt = DateTime.UtcNow.AddDays(30),
                DeviceName = device,
                IpAddress = "127.0.0.1",
                UserId = userId,
                Created = DateTime.UtcNow
            };
        }

        #region GetTokens(userId) Tests

        [Fact]
        public async Task GetTokens_ByUserId_WithNoTokens_ShouldReturnEmpty()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new TokenRepository(context, mapper);

            var user = CreateAuthUserEntity();
            context.AuthUsers.Add(user);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetTokens(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetTokens_ByUserId_WithActiveTokens_ShouldReturnAll()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new TokenRepository(context, mapper);

            var user = CreateAuthUserEntity();
            context.AuthUsers.Add(user);
            await context.SaveChangesAsync();

            context.Set<TokenEntity>().AddRange(
                CreateTokenEntity(user.Id, "Chrome"),
                CreateTokenEntity(user.Id, "Firefox")
            );
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetTokens(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetTokens_ByUserId_ShouldExcludeRevokedTokens()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new TokenRepository(context, mapper);

            var user = CreateAuthUserEntity();
            context.AuthUsers.Add(user);
            await context.SaveChangesAsync();

            context.Set<TokenEntity>().AddRange(
                CreateTokenEntity(user.Id, "Chrome", isRevoked: false),
                CreateTokenEntity(user.Id, "Firefox", isRevoked: true)
            );
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetTokens(user.Id);

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task GetTokens_ByUserId_ShouldNotReturnOtherUserTokens()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new TokenRepository(context, mapper);

            var user1 = CreateAuthUserEntity("User1", "user1@example.com");
            var user2 = CreateAuthUserEntity("User2", "user2@example.com");
            context.AuthUsers.AddRange(user1, user2);
            await context.SaveChangesAsync();

            context.Set<TokenEntity>().AddRange(
                CreateTokenEntity(user1.Id, "Chrome"),
                CreateTokenEntity(user2.Id, "Firefox")
            );
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetTokens(user1.Id);

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task GetTokens_ByUserId_WithAllRevokedTokens_ShouldReturnEmpty()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new TokenRepository(context, mapper);

            var user = CreateAuthUserEntity();
            context.AuthUsers.Add(user);
            await context.SaveChangesAsync();

            context.Set<TokenEntity>().AddRange(
                CreateTokenEntity(user.Id, "Chrome", isRevoked: true),
                CreateTokenEntity(user.Id, "Firefox", isRevoked: true)
            );
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetTokens(user.Id);

            // Assert
            Assert.Empty(result);
        }

        #endregion

        #region GetTokens(userId, deviceName) Tests

        [Fact]
        public async Task GetTokens_ByUserIdAndDevice_ShouldReturnOnlyMatchingDevice()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new TokenRepository(context, mapper);

            var user = CreateAuthUserEntity();
            context.AuthUsers.Add(user);
            await context.SaveChangesAsync();

            context.Set<TokenEntity>().AddRange(
                CreateTokenEntity(user.Id, "Chrome"),
                CreateTokenEntity(user.Id, "Firefox"),
                CreateTokenEntity(user.Id, "Chrome")
            );
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetTokens(user.Id, "Chrome");

            // Assert
            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetTokens_ByUserIdAndDevice_ShouldBeCaseInsensitive()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new TokenRepository(context, mapper);

            var user = CreateAuthUserEntity();
            context.AuthUsers.Add(user);
            await context.SaveChangesAsync();

            context.Set<TokenEntity>().Add(CreateTokenEntity(user.Id, "Chrome"));
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetTokens(user.Id, "chrome");

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task GetTokens_ByUserIdAndDevice_ShouldExcludeRevokedTokens()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new TokenRepository(context, mapper);

            var user = CreateAuthUserEntity();
            context.AuthUsers.Add(user);
            await context.SaveChangesAsync();

            context.Set<TokenEntity>().AddRange(
                CreateTokenEntity(user.Id, "Chrome", isRevoked: false),
                CreateTokenEntity(user.Id, "Chrome", isRevoked: true)
            );
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetTokens(user.Id, "Chrome");

            // Assert
            Assert.Single(result);
        }

        [Fact]
        public async Task GetTokens_ByUserIdAndDevice_WithNoMatchingDevice_ShouldReturnEmpty()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new TokenRepository(context, mapper);

            var user = CreateAuthUserEntity();
            context.AuthUsers.Add(user);
            await context.SaveChangesAsync();

            context.Set<TokenEntity>().Add(CreateTokenEntity(user.Id, "Chrome"));
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetTokens(user.Id, "Safari");

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetTokens_ByUserIdAndDevice_ShouldNotReturnOtherUserTokens()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new TokenRepository(context, mapper);

            var user1 = CreateAuthUserEntity("User1", "user1@example.com");
            var user2 = CreateAuthUserEntity("User2", "user2@example.com");
            context.AuthUsers.AddRange(user1, user2);
            await context.SaveChangesAsync();

            context.Set<TokenEntity>().AddRange(
                CreateTokenEntity(user1.Id, "Chrome"),
                CreateTokenEntity(user2.Id, "Chrome")
            );
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetTokens(user1.Id, "Chrome");

            // Assert
            Assert.Single(result);
        }

        #endregion

        #region InsertAsync Tests

        [Fact]
        public async Task InsertAsync_ShouldPersistToken()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new TokenRepository(context, mapper);

            var user = CreateAuthUserEntity();
            context.AuthUsers.Add(user);
            await context.SaveChangesAsync();

            var domainUser = new AuthUser(user.Id, user.Pseudo, user.Avatar, user.Email, "Password1@", RoleUser.User);
            var tokenInfo = new TokenInfo(
                "access-token-value",
                "refresh-token-value",
                DateTime.UtcNow.AddDays(30),
                DateTime.UtcNow.AddMinutes(30),
                domainUser,
                "Chrome",
                System.Net.IPAddress.Parse("127.0.0.1")
            );

            // Act
            var result = await repository.InsertAsync(tokenInfo);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal("Chrome", result.DeviceName);
        }

        [Fact]
        public async Task InsertAsync_ThenGetById_ShouldReturnSameToken()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new TokenRepository(context, mapper);

            var user = CreateAuthUserEntity();
            context.AuthUsers.Add(user);
            await context.SaveChangesAsync();

            var domainUser = new AuthUser(user.Id, user.Pseudo, user.Avatar, user.Email, "Password1@", RoleUser.User);
            var tokenInfo = new TokenInfo(
                "access-token-value",
                "refresh-token-value",
                DateTime.UtcNow.AddDays(30),
                DateTime.UtcNow.AddMinutes(30),
                domainUser,
                "Chrome",
                System.Net.IPAddress.Parse("192.168.1.1")
            );

            // Act
            var inserted = await repository.InsertAsync(tokenInfo);
            var retrieved = await repository.GetByIdAsync(inserted.Id);

            // Assert
            Assert.NotNull(retrieved);
            Assert.Equal(inserted.Id, retrieved.Id);
            Assert.Equal("Chrome", retrieved.DeviceName);
            Assert.False(retrieved.IsRevoked);
        }

        #endregion

        #region UpdateAsync Tests

        [Fact]
        public async Task UpdateAsync_RevokeToken_ShouldPersistRevocation()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new TokenRepository(context, mapper);

            var user = CreateAuthUserEntity();
            context.AuthUsers.Add(user);
            await context.SaveChangesAsync();

            var domainUser = new AuthUser(user.Id, user.Pseudo, user.Avatar, user.Email, "Password1@", RoleUser.User);
            var tokenInfo = new TokenInfo(
                "access-token-value",
                "refresh-token-value",
                DateTime.UtcNow.AddDays(30),
                DateTime.UtcNow.AddMinutes(30),
                domainUser,
                "Chrome",
                System.Net.IPAddress.Parse("127.0.0.1")
            );

            var inserted = await repository.InsertAsync(tokenInfo);

            // Act
            inserted.RevokeToken();
            var updated = await repository.UpdateAsync(inserted);

            // Assert
            Assert.True(updated.IsRevoked);
            var entityInDb = context.Set<TokenEntity>().FirstOrDefault(t => t.Id == inserted.Id);
            Assert.NotNull(entityInDb);
            Assert.True(entityInDb.IsRevoked);
        }

        [Fact]
        public async Task UpdateAsync_RevokeToken_ShouldExcludeFromGetTokens()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new TokenRepository(context, mapper);

            var user = CreateAuthUserEntity();
            context.AuthUsers.Add(user);
            await context.SaveChangesAsync();

            var domainUser = new AuthUser(user.Id, user.Pseudo, user.Avatar, user.Email, "Password1@", RoleUser.User);
            var tokenInfo = new TokenInfo(
                "access-token-value",
                "refresh-token-value",
                DateTime.UtcNow.AddDays(30),
                DateTime.UtcNow.AddMinutes(30),
                domainUser,
                "Chrome",
                System.Net.IPAddress.Parse("127.0.0.1")
            );

            var inserted = await repository.InsertAsync(tokenInfo);

            // Act
            inserted.RevokeToken();
            await repository.UpdateAsync(inserted);

            var tokens = await repository.GetTokens(user.Id);

            // Assert
            Assert.Empty(tokens);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task Delete_ShouldRemoveToken()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new TokenRepository(context, mapper);

            var user = CreateAuthUserEntity();
            context.AuthUsers.Add(user);
            await context.SaveChangesAsync();

            var token = CreateTokenEntity(user.Id);
            context.Set<TokenEntity>().Add(token);
            await context.SaveChangesAsync();

            // Act
            repository.Delete(token.Id);
            await repository.SaveAsync();

            // Assert
            var retrieved = await repository.GetByIdAsync(token.Id);
            Assert.Null(retrieved);
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact]
        public async Task GetByIdAsync_WithNonExistentId_ShouldReturnNull()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new TokenRepository(context, mapper);

            // Act
            var result = await repository.GetByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingToken_ShouldReturnCorrectData()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new TokenRepository(context, mapper);

            var user = CreateAuthUserEntity();
            context.AuthUsers.Add(user);
            await context.SaveChangesAsync();

            var token = CreateTokenEntity(user.Id, "Safari");
            context.Set<TokenEntity>().Add(token);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByIdAsync(token.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(token.Id, result.Id);
            Assert.Equal("Safari", result.DeviceName);
            Assert.False(result.IsRevoked);
        }

        #endregion

        #region Cascade Delete Tests

        [Fact]
        public async Task DeleteUser_ShouldCascadeDeleteTokens()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();

            var user = CreateAuthUserEntity();
            context.AuthUsers.Add(user);
            await context.SaveChangesAsync();

            context.Set<TokenEntity>().AddRange(
                CreateTokenEntity(user.Id, "Chrome"),
                CreateTokenEntity(user.Id, "Firefox")
            );
            await context.SaveChangesAsync();

            Assert.Equal(2, context.Set<TokenEntity>().Count());

            // Act
            context.AuthUsers.Remove(user);
            await context.SaveChangesAsync();

            // Assert
            Assert.Equal(0, context.Set<TokenEntity>().Count());
        }

        #endregion
    }
}
