using Domain.Enums;
using Domain.User;
using Domain.ValueObjects;
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

        private TokenEntity CreateTokenEntity(Guid userId, string device = "Chrome", bool isRevoked = false, string refreshToken = null)
        {
            return new TokenEntity
            {
                RefreshToken = refreshToken ?? "refresh-token-" + Guid.NewGuid(),
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

        #region RevokeAsync Tests

        [Fact]
        public async Task RevokeAsync_ShouldSetIsRevokedToTrue()
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
            await repository.RevokeAsync(token.Id);

            // Assert
            context.ChangeTracker.Clear();
            var entityInDb = context.Set<TokenEntity>().First(t => t.Id == token.Id);
            Assert.True(entityInDb.IsRevoked);
        }

        [Fact]
        public async Task RevokeAsync_ShouldExcludeTokenFromGetTokens()
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
            await repository.RevokeAsync(token.Id);
            var tokens = await repository.GetTokens(user.Id);

            // Assert
            Assert.Empty(tokens);
        }

        [Fact]
        public async Task RevokeAsync_WithNonExistentId_ShouldNotThrow()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new TokenRepository(context, mapper);

            // Act & Assert
            var exception = await Record.ExceptionAsync(() => repository.RevokeAsync(Guid.NewGuid()));
            Assert.Null(exception);
        }

        [Fact]
        public async Task RevokeAsync_ShouldNotAffectOtherTokens()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new TokenRepository(context, mapper);

            var user = CreateAuthUserEntity();
            context.AuthUsers.Add(user);
            await context.SaveChangesAsync();

            var token1 = CreateTokenEntity(user.Id, "Chrome");
            var token2 = CreateTokenEntity(user.Id, "Firefox");
            context.Set<TokenEntity>().AddRange(token1, token2);
            await context.SaveChangesAsync();

            // Act
            await repository.RevokeAsync(token1.Id);

            // Assert
            var entityToken2 = context.Set<TokenEntity>().First(t => t.Id == token2.Id);
            Assert.False(entityToken2.IsRevoked);
        }

        #endregion

        #region RevokeAllAsync(userId) Tests

        [Fact]
        public async Task RevokeAllAsync_ByUserId_ShouldRevokeAllActiveTokens()
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
            await repository.RevokeAllAsync(user.Id);

            // Assert
            context.ChangeTracker.Clear();
            var tokens = context.Set<TokenEntity>().Where(t => t.UserId == user.Id).ToList();
            Assert.All(tokens, t => Assert.True(t.IsRevoked));
        }

        [Fact]
        public async Task RevokeAllAsync_ByUserId_ShouldNotAffectOtherUsers()
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
            await repository.RevokeAllAsync(user1.Id);

            // Assert
            var user2Token = context.Set<TokenEntity>().First(t => t.UserId == user2.Id);
            Assert.False(user2Token.IsRevoked);
        }

        [Fact]
        public async Task RevokeAllAsync_ByUserId_ShouldNotRevokeAlreadyRevokedTokens_IsIdempotent()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new TokenRepository(context, mapper);

            var user = CreateAuthUserEntity();
            context.AuthUsers.Add(user);
            await context.SaveChangesAsync();

            context.Set<TokenEntity>().Add(CreateTokenEntity(user.Id, isRevoked: true));
            await context.SaveChangesAsync();

            // Act & Assert - should not throw
            var exception = await Record.ExceptionAsync(() => repository.RevokeAllAsync(user.Id));
            Assert.Null(exception);
        }

        #endregion

        #region RevokeAllAsync(userId, deviceName) Tests

        [Fact]
        public async Task RevokeAllAsync_ByUserIdAndDevice_ShouldRevokeMatchingTokens()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new TokenRepository(context, mapper);

            var user = CreateAuthUserEntity();
            context.AuthUsers.Add(user);
            await context.SaveChangesAsync();

            var chromeToken1 = CreateTokenEntity(user.Id, "Chrome");
            var chromeToken2 = CreateTokenEntity(user.Id, "Chrome");
            var firefoxToken = CreateTokenEntity(user.Id, "Firefox");
            context.Set<TokenEntity>().AddRange(chromeToken1, chromeToken2, firefoxToken);
            await context.SaveChangesAsync();

            // Act
            await repository.RevokeAllAsync(user.Id, "Chrome");

            // Assert
            context.ChangeTracker.Clear();
            var chrome1 = context.Set<TokenEntity>().First(t => t.Id == chromeToken1.Id);
            var chrome2 = context.Set<TokenEntity>().First(t => t.Id == chromeToken2.Id);
            Assert.True(chrome1.IsRevoked);
            Assert.True(chrome2.IsRevoked);
        }

        [Fact]
        public async Task RevokeAllAsync_ByUserIdAndDevice_ShouldNotAffectOtherDevices()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new TokenRepository(context, mapper);

            var user = CreateAuthUserEntity();
            context.AuthUsers.Add(user);
            await context.SaveChangesAsync();

            var firefoxToken = CreateTokenEntity(user.Id, "Firefox");
            context.Set<TokenEntity>().AddRange(
                CreateTokenEntity(user.Id, "Chrome"),
                firefoxToken
            );
            await context.SaveChangesAsync();

            // Act
            await repository.RevokeAllAsync(user.Id, "Chrome");

            // Assert
            var firefox = context.Set<TokenEntity>().First(t => t.Id == firefoxToken.Id);
            Assert.False(firefox.IsRevoked);
        }

        [Fact]
        public async Task RevokeAllAsync_ByUserIdAndDevice_ShouldNotAffectOtherUsers()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new TokenRepository(context, mapper);

            var user1 = CreateAuthUserEntity("User1", "user1@example.com");
            var user2 = CreateAuthUserEntity("User2", "user2@example.com");
            context.AuthUsers.AddRange(user1, user2);
            await context.SaveChangesAsync();

            var user2Token = CreateTokenEntity(user2.Id, "Chrome");
            context.Set<TokenEntity>().AddRange(
                CreateTokenEntity(user1.Id, "Chrome"),
                user2Token
            );
            await context.SaveChangesAsync();

            // Act
            await repository.RevokeAllAsync(user1.Id, "Chrome");

            // Assert
            var user2Entity = context.Set<TokenEntity>().First(t => t.Id == user2Token.Id);
            Assert.False(user2Entity.IsRevoked);
        }

        #endregion

        #region GetByRefreshToken Tests

        [Fact]
        public async Task GetByRefreshToken_WithMatchingToken_ShouldReturnToken()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new TokenRepository(context, mapper);

            var user = CreateAuthUserEntity();
            context.AuthUsers.Add(user);
            await context.SaveChangesAsync();

            var refreshTokenValue = "my-unique-refresh-token";
            context.Set<TokenEntity>().Add(CreateTokenEntity(user.Id, refreshToken: refreshTokenValue));
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByRefreshToken(Token.Create(refreshTokenValue));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(refreshTokenValue, result.RefreshToken.Value);
        }

        [Fact]
        public async Task GetByRefreshToken_WithNonExistentToken_ShouldReturnNull()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new TokenRepository(context, mapper);

            // Act
            var result = await repository.GetByRefreshToken(Token.Create("non-existent-token"));

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByRefreshToken_ShouldIncludeUser()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new TokenRepository(context, mapper);

            var user = CreateAuthUserEntity("TestUser");
            context.AuthUsers.Add(user);
            await context.SaveChangesAsync();

            var refreshTokenValue = "refresh-with-user";
            context.Set<TokenEntity>().Add(CreateTokenEntity(user.Id, refreshToken: refreshTokenValue));
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByRefreshToken(Token.Create(refreshTokenValue));

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.User);
            Assert.Equal(user.Id, result.User.Id);
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
