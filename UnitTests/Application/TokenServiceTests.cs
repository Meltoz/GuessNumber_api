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
    }
}
