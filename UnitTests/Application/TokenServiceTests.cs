using Application.Exceptions;
using Application.Interfaces.Repository;
using Application.Services;
using Domain.Enums;
using Domain.User;
using Domain.ValueObjects;
using Microsoft.Extensions.Options;
using Moq;
using Shared.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace UnitTests.Application
{
    public class TokenServiceTests
    {
        private readonly Mock<ITokenRepository> _tokenRepoMock;
        private readonly JwtService _jwtService;
        private readonly TokenService _service;

        public TokenServiceTests()
        {
            _tokenRepoMock = new Mock<ITokenRepository>();

            var authOptions = Options.Create(new AuthConfiguration
            {
                Key = "ThisIsAVeryLongSecretKeyForTestingPurposesOnly1234567890!",
                Issuer = "TestIssuer",
                Audience = "TestAudience"
            });
            _jwtService = new JwtService(authOptions);

            _service = new TokenService(_jwtService, _tokenRepoMock.Object);
        }

        private AuthUser CreateTestUser()
        {
            return new AuthUser(Guid.NewGuid(), "TestUser", "avatar.png", "test@example.com", "Password1@", RoleUser.User);
        }

        #region CreateTokenAsync Tests

        [Fact]
        public async Task CreateTokenAsync_ShouldCallTokenRepositoryInsertAsync()
        {
            // Arrange
            var user = CreateTestUser();
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "TestUser") };
            var device = "Chrome";
            var ip = IPAddress.Parse("127.0.0.1");

            _tokenRepoMock.Setup(r => r.InsertAsync(It.IsAny<TokenInfo>()))
                .ReturnsAsync((TokenInfo t) => t);

            // Act
            await _service.CreateTokenAsync(user, claims, device, ip);

            // Assert
            _tokenRepoMock.Verify(r => r.InsertAsync(It.IsAny<TokenInfo>()), Times.Once);
        }

        [Fact]
        public async Task CreateTokenAsync_ShouldReturnTokenInfo()
        {
            // Arrange
            var user = CreateTestUser();
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "TestUser") };
            var device = "Chrome";
            var ip = IPAddress.Parse("127.0.0.1");

            _tokenRepoMock.Setup(r => r.InsertAsync(It.IsAny<TokenInfo>()))
                .ReturnsAsync((TokenInfo t) => t);

            // Act
            var result = await _service.CreateTokenAsync(user, claims, device, ip);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TokenInfo>(result);
        }

        [Fact]
        public async Task CreateTokenAsync_ShouldSetAccessToken()
        {
            // Arrange
            var user = CreateTestUser();
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "TestUser") };
            var device = "Chrome";
            var ip = IPAddress.Parse("127.0.0.1");

            TokenInfo capturedToken = null;
            _tokenRepoMock.Setup(r => r.InsertAsync(It.IsAny<TokenInfo>()))
                .Callback<TokenInfo>(t => capturedToken = t)
                .ReturnsAsync((TokenInfo t) => t);

            // Act
            await _service.CreateTokenAsync(user, claims, device, ip);

            // Assert
            Assert.NotNull(capturedToken);
            Assert.NotNull(capturedToken.AccessToken);
            Assert.False(string.IsNullOrWhiteSpace(capturedToken.AccessToken.Value));
        }

        [Fact]
        public async Task CreateTokenAsync_ShouldSetRefreshToken()
        {
            // Arrange
            var user = CreateTestUser();
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "TestUser") };
            var device = "Chrome";
            var ip = IPAddress.Parse("127.0.0.1");

            TokenInfo capturedToken = null;
            _tokenRepoMock.Setup(r => r.InsertAsync(It.IsAny<TokenInfo>()))
                .Callback<TokenInfo>(t => capturedToken = t)
                .ReturnsAsync((TokenInfo t) => t);

            // Act
            await _service.CreateTokenAsync(user, claims, device, ip);

            // Assert
            Assert.NotNull(capturedToken);
            Assert.NotNull(capturedToken.RefreshToken);
            Assert.False(string.IsNullOrWhiteSpace(capturedToken.RefreshToken.Value));
        }

        [Fact]
        public async Task CreateTokenAsync_ShouldSetCorrectUser()
        {
            // Arrange
            var user = CreateTestUser();
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "TestUser") };
            var device = "Chrome";
            var ip = IPAddress.Parse("127.0.0.1");

            TokenInfo capturedToken = null;
            _tokenRepoMock.Setup(r => r.InsertAsync(It.IsAny<TokenInfo>()))
                .Callback<TokenInfo>(t => capturedToken = t)
                .ReturnsAsync((TokenInfo t) => t);

            // Act
            await _service.CreateTokenAsync(user, claims, device, ip);

            // Assert
            Assert.NotNull(capturedToken);
            Assert.Equal(user.Id, capturedToken.User.Id);
        }

        [Fact]
        public async Task CreateTokenAsync_ShouldSetCorrectDevice()
        {
            // Arrange
            var user = CreateTestUser();
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "TestUser") };
            var device = "Firefox";
            var ip = IPAddress.Parse("127.0.0.1");

            TokenInfo capturedToken = null;
            _tokenRepoMock.Setup(r => r.InsertAsync(It.IsAny<TokenInfo>()))
                .Callback<TokenInfo>(t => capturedToken = t)
                .ReturnsAsync((TokenInfo t) => t);

            // Act
            await _service.CreateTokenAsync(user, claims, device, ip);

            // Assert
            Assert.NotNull(capturedToken);
            Assert.Equal("Firefox", capturedToken.DeviceName);
        }

        [Fact]
        public async Task CreateTokenAsync_ShouldSetCorrectIpAddress()
        {
            // Arrange
            var user = CreateTestUser();
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "TestUser") };
            var device = "Chrome";
            var ip = IPAddress.Parse("192.168.1.1");

            TokenInfo capturedToken = null;
            _tokenRepoMock.Setup(r => r.InsertAsync(It.IsAny<TokenInfo>()))
                .Callback<TokenInfo>(t => capturedToken = t)
                .ReturnsAsync((TokenInfo t) => t);

            // Act
            await _service.CreateTokenAsync(user, claims, device, ip);

            // Assert
            Assert.NotNull(capturedToken);
            Assert.Equal(ip, capturedToken.IpAdress);
        }

        [Fact]
        public async Task CreateTokenAsync_ShouldSetIsRevokedToFalse()
        {
            // Arrange
            var user = CreateTestUser();
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "TestUser") };
            var device = "Chrome";
            var ip = IPAddress.Parse("127.0.0.1");

            TokenInfo capturedToken = null;
            _tokenRepoMock.Setup(r => r.InsertAsync(It.IsAny<TokenInfo>()))
                .Callback<TokenInfo>(t => capturedToken = t)
                .ReturnsAsync((TokenInfo t) => t);

            // Act
            await _service.CreateTokenAsync(user, claims, device, ip);

            // Assert
            Assert.NotNull(capturedToken);
            Assert.False(capturedToken.IsRevoked);
        }

        [Fact]
        public async Task CreateTokenAsync_ShouldSetRefreshExpiresAt_30DaysFromNow()
        {
            // Arrange
            var user = CreateTestUser();
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "TestUser") };
            var device = "Chrome";
            var ip = IPAddress.Parse("127.0.0.1");
            var beforeCall = DateTime.UtcNow;

            TokenInfo capturedToken = null;
            _tokenRepoMock.Setup(r => r.InsertAsync(It.IsAny<TokenInfo>()))
                .Callback<TokenInfo>(t => capturedToken = t)
                .ReturnsAsync((TokenInfo t) => t);

            // Act
            await _service.CreateTokenAsync(user, claims, device, ip);

            // Assert
            var afterCall = DateTime.UtcNow;
            Assert.NotNull(capturedToken);
            Assert.True(capturedToken.RefreshExpiresAt >= beforeCall.AddDays(30));
            Assert.True(capturedToken.RefreshExpiresAt <= afterCall.AddDays(30));
        }

        [Fact]
        public async Task CreateTokenAsync_ShouldSetAccessExpiresAt_30MinutesFromNow()
        {
            // Arrange
            var user = CreateTestUser();
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "TestUser") };
            var device = "Chrome";
            var ip = IPAddress.Parse("127.0.0.1");
            var beforeCall = DateTime.UtcNow;

            TokenInfo capturedToken = null;
            _tokenRepoMock.Setup(r => r.InsertAsync(It.IsAny<TokenInfo>()))
                .Callback<TokenInfo>(t => capturedToken = t)
                .ReturnsAsync((TokenInfo t) => t);

            // Act
            await _service.CreateTokenAsync(user, claims, device, ip);

            // Assert
            var afterCall = DateTime.UtcNow;
            Assert.NotNull(capturedToken);
            Assert.True(capturedToken.AccessExpiresAt >= beforeCall.AddMinutes(30));
            Assert.True(capturedToken.AccessExpiresAt <= afterCall.AddMinutes(30));
        }

        [Fact]
        public async Task CreateTokenAsync_ShouldAddJtiClaimToAccessToken()
        {
            // Arrange
            var user = CreateTestUser();
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "TestUser") };
            var device = "Chrome";
            var ip = IPAddress.Parse("127.0.0.1");

            TokenInfo capturedToken = null;
            _tokenRepoMock.Setup(r => r.InsertAsync(It.IsAny<TokenInfo>()))
                .Callback<TokenInfo>(t => capturedToken = t)
                .ReturnsAsync((TokenInfo t) => t);

            // Act
            await _service.CreateTokenAsync(user, claims, device, ip);

            // Assert
            Assert.NotNull(capturedToken);
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(capturedToken.AccessToken.Value);
            Assert.Contains(jwt.Claims, c => c.Type == JwtRegisteredClaimNames.Jti);
        }

        [Fact]
        public async Task CreateTokenAsync_ShouldSetNonEmptyId()
        {
            // Arrange
            var user = CreateTestUser();
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "TestUser") };
            var device = "Chrome";
            var ip = IPAddress.Parse("127.0.0.1");

            TokenInfo capturedToken = null;
            _tokenRepoMock.Setup(r => r.InsertAsync(It.IsAny<TokenInfo>()))
                .Callback<TokenInfo>(t => capturedToken = t)
                .ReturnsAsync((TokenInfo t) => t);

            // Act
            await _service.CreateTokenAsync(user, claims, device, ip);

            // Assert
            Assert.NotNull(capturedToken);
            Assert.NotEqual(Guid.Empty, capturedToken.Id);
        }

        [Fact]
        public async Task CreateTokenAsync_JtiInTokenShouldMatchTokenInfoId()
        {
            // Arrange
            var user = CreateTestUser();
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "TestUser") };
            var device = "Chrome";
            var ip = IPAddress.Parse("127.0.0.1");

            TokenInfo capturedToken = null;
            _tokenRepoMock.Setup(r => r.InsertAsync(It.IsAny<TokenInfo>()))
                .Callback<TokenInfo>(t => capturedToken = t)
                .ReturnsAsync((TokenInfo t) => t);

            // Act
            await _service.CreateTokenAsync(user, claims, device, ip);

            // Assert
            Assert.NotNull(capturedToken);
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(capturedToken.AccessToken.Value);
            var jtiClaim = jwt.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti);
            Assert.Equal(capturedToken.Id.ToString(), jtiClaim.Value);
        }

        [Fact]
        public async Task CreateTokenAsync_AccessTokenShouldBeValidJwt()
        {
            // Arrange
            var user = CreateTestUser();
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "TestUser") };
            var device = "Chrome";
            var ip = IPAddress.Parse("127.0.0.1");

            TokenInfo capturedToken = null;
            _tokenRepoMock.Setup(r => r.InsertAsync(It.IsAny<TokenInfo>()))
                .Callback<TokenInfo>(t => capturedToken = t)
                .ReturnsAsync((TokenInfo t) => t);

            // Act
            await _service.CreateTokenAsync(user, claims, device, ip);

            // Assert
            Assert.NotNull(capturedToken);
            var handler = new JwtSecurityTokenHandler();
            Assert.True(handler.CanReadToken(capturedToken.AccessToken.Value));
        }

        [Fact]
        public async Task CreateTokenAsync_ShouldThrow_WhenRepositoryThrows()
        {
            // Arrange
            var user = CreateTestUser();
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "TestUser") };
            var device = "Chrome";
            var ip = IPAddress.Parse("127.0.0.1");

            _tokenRepoMock.Setup(r => r.InsertAsync(It.IsAny<TokenInfo>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.CreateTokenAsync(user, claims, device, ip));
        }

        [Fact]
        public async Task CreateTokenAsync_MultipleCalls_ShouldGenerateDifferentIds()
        {
            // Arrange
            var user = CreateTestUser();
            var device = "Chrome";
            var ip = IPAddress.Parse("127.0.0.1");
            var capturedTokens = new List<TokenInfo>();

            _tokenRepoMock.Setup(r => r.InsertAsync(It.IsAny<TokenInfo>()))
                .Callback<TokenInfo>(t => capturedTokens.Add(t))
                .ReturnsAsync((TokenInfo t) => t);

            // Act
            await _service.CreateTokenAsync(user, new List<Claim> { new Claim(ClaimTypes.Name, "User1") }, device, ip);
            await _service.CreateTokenAsync(user, new List<Claim> { new Claim(ClaimTypes.Name, "User2") }, device, ip);

            // Assert
            Assert.Equal(2, capturedTokens.Count);
            Assert.NotEqual(capturedTokens[0].Id, capturedTokens[1].Id);
        }

        [Fact]
        public async Task CreateTokenAsync_MultipleCalls_ShouldGenerateDifferentRefreshTokens()
        {
            // Arrange
            var user = CreateTestUser();
            var device = "Chrome";
            var ip = IPAddress.Parse("127.0.0.1");
            var capturedTokens = new List<TokenInfo>();

            _tokenRepoMock.Setup(r => r.InsertAsync(It.IsAny<TokenInfo>()))
                .Callback<TokenInfo>(t => capturedTokens.Add(t))
                .ReturnsAsync((TokenInfo t) => t);

            // Act
            await _service.CreateTokenAsync(user, new List<Claim> { new Claim(ClaimTypes.Name, "User1") }, device, ip);
            await _service.CreateTokenAsync(user, new List<Claim> { new Claim(ClaimTypes.Name, "User2") }, device, ip);

            // Assert
            Assert.Equal(2, capturedTokens.Count);
            Assert.NotEqual(capturedTokens[0].RefreshToken.Value, capturedTokens[1].RefreshToken.Value);
        }

        #endregion

        #region RevokeAllTokens Tests

        [Fact]
        public async Task RevokeAllTokens_ShouldThrow_WhenUserIdIsEmpty()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.RevokeAllTokens(Guid.Empty));
        }

        [Fact]
        public async Task RevokeAllTokens_ShouldCallGetTokens()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _tokenRepoMock.Setup(r => r.GetTokens(userId))
                .ReturnsAsync(Enumerable.Empty<TokenInfo>());

            // Act
            await _service.RevokeAllTokens(userId);

            // Assert
            _tokenRepoMock.Verify(r => r.GetTokens(userId), Times.Once);
        }

        [Fact]
        public async Task RevokeAllTokens_ShouldRevokeEachToken()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = CreateTestUser();
            var token1 = new TokenInfo(
                Guid.NewGuid(), "access1", "refresh1",
                DateTime.UtcNow.AddDays(30), DateTime.UtcNow.AddMinutes(30),
                user, "Chrome", IPAddress.Loopback);
            var token2 = new TokenInfo(
                Guid.NewGuid(), "access2", "refresh2",
                DateTime.UtcNow.AddDays(30), DateTime.UtcNow.AddMinutes(30),
                user, "Firefox", IPAddress.Loopback);

            _tokenRepoMock.Setup(r => r.GetTokens(userId))
                .ReturnsAsync(new[] { token1, token2 });

            // Act
            await _service.RevokeAllTokens(userId);

            // Assert
            Assert.True(token1.IsRevoked);
            Assert.True(token2.IsRevoked);
        }

        [Fact]
        public async Task RevokeAllTokens_ShouldCallUpdateWithOutSaveForEachToken()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = CreateTestUser();
            var token1 = new TokenInfo(
                Guid.NewGuid(), "access1", "refresh1",
                DateTime.UtcNow.AddDays(30), DateTime.UtcNow.AddMinutes(30),
                user, "Chrome", IPAddress.Loopback);
            var token2 = new TokenInfo(
                Guid.NewGuid(), "access2", "refresh2",
                DateTime.UtcNow.AddDays(30), DateTime.UtcNow.AddMinutes(30),
                user, "Firefox", IPAddress.Loopback);

            _tokenRepoMock.Setup(r => r.GetTokens(userId))
                .ReturnsAsync(new[] { token1, token2 });

            // Act
            await _service.RevokeAllTokens(userId);

            // Assert
            _tokenRepoMock.Verify(r => r.UpdateWithOutSaveAsync(It.IsAny<TokenInfo>()), Times.Exactly(2));
            _tokenRepoMock.Verify(r => r.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task RevokeAllTokens_WithNoTokens_ShouldStillCallSave()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _tokenRepoMock.Setup(r => r.GetTokens(userId))
                .ReturnsAsync(Enumerable.Empty<TokenInfo>());

            // Act
            await _service.RevokeAllTokens(userId);

            // Assert
            _tokenRepoMock.Verify(r => r.UpdateWithOutSaveAsync(It.IsAny<TokenInfo>()), Times.Never);
            _tokenRepoMock.Verify(r => r.SaveAsync(), Times.Once);
        }

        #endregion

        #region RevokeTokenById Tests

        [Fact]
        public async Task RevokeTokenById_ShouldThrow_WhenTokenIdIsEmpty()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.RevokeTokenById(Guid.Empty));
        }

        [Fact]
        public async Task RevokeTokenById_ShouldThrow_WhenTokenNotFound()
        {
            // Arrange
            var tokenId = Guid.NewGuid();
            _tokenRepoMock.Setup(r => r.GetByIdAsync(tokenId))
                .ReturnsAsync((TokenInfo?)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(
                () => _service.RevokeTokenById(tokenId));
        }

        [Fact]
        public async Task RevokeTokenById_ShouldRevokeToken()
        {
            // Arrange
            var user = CreateTestUser();
            var tokenId = Guid.NewGuid();
            var token = new TokenInfo(
                tokenId, "access", "refresh",
                DateTime.UtcNow.AddDays(30), DateTime.UtcNow.AddMinutes(30),
                user, "Chrome", IPAddress.Loopback);

            _tokenRepoMock.Setup(r => r.GetByIdAsync(tokenId))
                .ReturnsAsync(token);
            _tokenRepoMock.Setup(r => r.UpdateAsync(It.IsAny<TokenInfo>()))
                .ReturnsAsync((TokenInfo t) => t);

            // Act
            await _service.RevokeTokenById(tokenId);

            // Assert
            Assert.True(token.IsRevoked);
        }

        [Fact]
        public async Task RevokeTokenById_ShouldCallUpdateAsync()
        {
            // Arrange
            var user = CreateTestUser();
            var tokenId = Guid.NewGuid();
            var token = new TokenInfo(
                tokenId, "access", "refresh",
                DateTime.UtcNow.AddDays(30), DateTime.UtcNow.AddMinutes(30),
                user, "Chrome", IPAddress.Loopback);

            _tokenRepoMock.Setup(r => r.GetByIdAsync(tokenId))
                .ReturnsAsync(token);
            _tokenRepoMock.Setup(r => r.UpdateAsync(It.IsAny<TokenInfo>()))
                .ReturnsAsync((TokenInfo t) => t);

            // Act
            await _service.RevokeTokenById(tokenId);

            // Assert
            _tokenRepoMock.Verify(r => r.UpdateAsync(It.IsAny<TokenInfo>()), Times.Once);
        }

        #endregion

        #region RevokeSpecificTokens Tests

        [Fact]
        public async Task RevokeSpecificTokens_ShouldThrow_WhenUserIdIsEmpty()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _service.RevokeSpecificTokens(Guid.Empty, "Chrome"));
        }

        [Fact]
        public async Task RevokeSpecificTokens_ShouldThrow_WhenDeviceNameIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _service.RevokeSpecificTokens(Guid.NewGuid(), null!));
        }

        [Fact]
        public async Task RevokeSpecificTokens_ShouldThrow_WhenDeviceNameIsEmpty()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _service.RevokeSpecificTokens(Guid.NewGuid(), ""));
        }

        [Fact]
        public async Task RevokeSpecificTokens_ShouldThrow_WhenDeviceNameIsWhitespace()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                () => _service.RevokeSpecificTokens(Guid.NewGuid(), "   "));
        }

        [Fact]
        public async Task RevokeSpecificTokens_ShouldCallGetTokensWithDeviceName()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var deviceName = "Chrome";
            _tokenRepoMock.Setup(r => r.GetTokens(userId, deviceName))
                .ReturnsAsync(Enumerable.Empty<TokenInfo>());

            // Act
            await _service.RevokeSpecificTokens(userId, deviceName);

            // Assert
            _tokenRepoMock.Verify(r => r.GetTokens(userId, deviceName), Times.Once);
        }

        [Fact]
        public async Task RevokeSpecificTokens_ShouldRevokeAllMatchingTokens()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = CreateTestUser();
            var token1 = new TokenInfo(
                Guid.NewGuid(), "access1", "refresh1",
                DateTime.UtcNow.AddDays(30), DateTime.UtcNow.AddMinutes(30),
                user, "Chrome", IPAddress.Loopback);
            var token2 = new TokenInfo(
                Guid.NewGuid(), "access2", "refresh2",
                DateTime.UtcNow.AddDays(30), DateTime.UtcNow.AddMinutes(30),
                user, "Chrome", IPAddress.Loopback);

            _tokenRepoMock.Setup(r => r.GetTokens(userId, "Chrome"))
                .ReturnsAsync(new[] { token1, token2 });

            // Act
            await _service.RevokeSpecificTokens(userId, "Chrome");

            // Assert
            Assert.True(token1.IsRevoked);
            Assert.True(token2.IsRevoked);
        }

        [Fact]
        public async Task RevokeSpecificTokens_ShouldCallUpdateWithOutSaveAndSave()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = CreateTestUser();
            var token = new TokenInfo(
                Guid.NewGuid(), "access", "refresh",
                DateTime.UtcNow.AddDays(30), DateTime.UtcNow.AddMinutes(30),
                user, "Chrome", IPAddress.Loopback);

            _tokenRepoMock.Setup(r => r.GetTokens(userId, "Chrome"))
                .ReturnsAsync(new[] { token });

            // Act
            await _service.RevokeSpecificTokens(userId, "Chrome");

            // Assert
            _tokenRepoMock.Verify(r => r.UpdateWithOutSaveAsync(It.IsAny<TokenInfo>()), Times.Once);
            _tokenRepoMock.Verify(r => r.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task RevokeSpecificTokens_WithNoMatchingTokens_ShouldStillCallSave()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _tokenRepoMock.Setup(r => r.GetTokens(userId, "Safari"))
                .ReturnsAsync(Enumerable.Empty<TokenInfo>());

            // Act
            await _service.RevokeSpecificTokens(userId, "Safari");

            // Assert
            _tokenRepoMock.Verify(r => r.UpdateWithOutSaveAsync(It.IsAny<TokenInfo>()), Times.Never);
            _tokenRepoMock.Verify(r => r.SaveAsync(), Times.Once);
        }

        #endregion
    }
}
