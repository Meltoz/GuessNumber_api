using Application.Services;
using Microsoft.Extensions.Options;
using Shared.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace UnitTests.Application
{
    public class JwtServiceTests
    {
        private readonly JwtService _service;
        private readonly string _issuer = "TestIssuer";
        private readonly string _audience = "TestAudience";

        public JwtServiceTests()
        {
            var options = Options.Create(new AuthConfiguration
            {
                Key = "ThisIsAVeryLongSecretKeyForTestingPurposesOnly1234567890!",
                Issuer = _issuer,
                Audience = _audience
            });
            _service = new JwtService(options);
        }

        #region CreateAccessToken Tests

        [Fact]
        public void CreateAccessToken_ShouldReturnNonEmptyString()
        {
            // Arrange
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "TestUser") };
            var expiresAt = DateTime.UtcNow.AddMinutes(30);

            // Act
            var result = _service.CreateAccessToken(claims, expiresAt);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(result));
        }

        [Fact]
        public void CreateAccessToken_ShouldReturnValidJwtToken()
        {
            // Arrange
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "TestUser") };
            var expiresAt = DateTime.UtcNow.AddMinutes(30);

            // Act
            var result = _service.CreateAccessToken(claims, expiresAt);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            Assert.True(handler.CanReadToken(result));
        }

        [Fact]
        public void CreateAccessToken_ShouldContainCorrectIssuer()
        {
            // Arrange
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "TestUser") };
            var expiresAt = DateTime.UtcNow.AddMinutes(30);

            // Act
            var result = _service.CreateAccessToken(claims, expiresAt);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(result);
            Assert.Equal(_issuer, token.Issuer);
        }

        [Fact]
        public void CreateAccessToken_ShouldContainCorrectAudience()
        {
            // Arrange
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "TestUser") };
            var expiresAt = DateTime.UtcNow.AddMinutes(30);

            // Act
            var result = _service.CreateAccessToken(claims, expiresAt);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(result);
            Assert.Contains(_audience, token.Audiences);
        }

        [Fact]
        public void CreateAccessToken_ShouldContainProvidedClaims()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "TestUser"),
                new Claim(ClaimTypes.Email, "test@example.com")
            };
            var expiresAt = DateTime.UtcNow.AddMinutes(30);

            // Act
            var result = _service.CreateAccessToken(claims, expiresAt);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(result);
            Assert.Contains(token.Claims, c => c.Type == ClaimTypes.Name && c.Value == "TestUser");
            Assert.Contains(token.Claims, c => c.Type == ClaimTypes.Email && c.Value == "test@example.com");
        }

        [Fact]
        public void CreateAccessToken_ShouldSetCorrectExpiration()
        {
            // Arrange
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "TestUser") };
            var expiresAt = DateTime.UtcNow.AddMinutes(30);

            // Act
            var result = _service.CreateAccessToken(claims, expiresAt);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(result);
            Assert.True(Math.Abs((token.ValidTo - expiresAt).TotalSeconds) < 2);
        }

        [Fact]
        public void CreateAccessToken_ShouldUseHmacSha256Algorithm()
        {
            // Arrange
            var claims = new List<Claim> { new Claim(ClaimTypes.Name, "TestUser") };
            var expiresAt = DateTime.UtcNow.AddMinutes(30);

            // Act
            var result = _service.CreateAccessToken(claims, expiresAt);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(result);
            Assert.Equal("HS256", token.Header.Alg);
        }

        [Fact]
        public void CreateAccessToken_WithEmptyClaims_ShouldReturnValidToken()
        {
            // Arrange
            var claims = new List<Claim>();
            var expiresAt = DateTime.UtcNow.AddMinutes(30);

            // Act
            var result = _service.CreateAccessToken(claims, expiresAt);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            Assert.True(handler.CanReadToken(result));
        }

        [Fact]
        public void CreateAccessToken_WithJtiClaim_ShouldContainJti()
        {
            // Arrange
            var jti = Guid.NewGuid().ToString();
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, jti)
            };
            var expiresAt = DateTime.UtcNow.AddMinutes(30);

            // Act
            var result = _service.CreateAccessToken(claims, expiresAt);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(result);
            Assert.Contains(token.Claims, c => c.Type == JwtRegisteredClaimNames.Jti && c.Value == jti);
        }

        [Fact]
        public void CreateAccessToken_MultipleCalls_ShouldReturnDifferentTokens_WithDifferentClaims()
        {
            // Arrange
            var claims1 = new List<Claim> { new Claim(ClaimTypes.Name, "User1") };
            var claims2 = new List<Claim> { new Claim(ClaimTypes.Name, "User2") };
            var expiresAt = DateTime.UtcNow.AddMinutes(30);

            // Act
            var result1 = _service.CreateAccessToken(claims1, expiresAt);
            var result2 = _service.CreateAccessToken(claims2, expiresAt);

            // Assert
            Assert.NotEqual(result1, result2);
        }

        #endregion

        #region GenerateRefreshToken Tests

        [Fact]
        public void GenerateRefreshToken_ShouldReturnNonEmptyString()
        {
            // Act
            var result = _service.GenerateRefreshToken();

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(result));
        }

        [Fact]
        public void GenerateRefreshToken_ShouldReturnBase64String()
        {
            // Act
            var result = _service.GenerateRefreshToken();

            // Assert
            var bytes = Convert.FromBase64String(result);
            Assert.NotEmpty(bytes);
        }

        [Fact]
        public void GenerateRefreshToken_ShouldReturn32BytesAsBase64()
        {
            // Act
            var result = _service.GenerateRefreshToken();

            // Assert
            var bytes = Convert.FromBase64String(result);
            Assert.Equal(32, bytes.Length);
        }

        [Fact]
        public void GenerateRefreshToken_MultipleCalls_ShouldReturnDifferentTokens()
        {
            // Act
            var result1 = _service.GenerateRefreshToken();
            var result2 = _service.GenerateRefreshToken();
            var result3 = _service.GenerateRefreshToken();

            // Assert
            Assert.NotEqual(result1, result2);
            Assert.NotEqual(result2, result3);
            Assert.NotEqual(result1, result3);
        }

        #endregion
    }
}
