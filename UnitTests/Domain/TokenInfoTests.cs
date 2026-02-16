using System.Net;
using Domain.Enums;
using Domain.User;
using Domain.ValueObjects;

namespace UnitTests.Domain
{
    public class TokenInfoTests
    {
        #region Constructor (Token) Tests - Cas Nominaux

        [Fact]
        public void Constructor_WithValidTokenParameters_ShouldCreateTokenInfo()
        {
            // Arrange
            var user = CreateValidUser();
            var access = Token.Create("valid-access-token");
            var refresh = Token.Create("valid-refresh-token");
            var accessExpires = DateTime.UtcNow.AddMinutes(30);
            var refreshExpires = DateTime.UtcNow.AddDays(30);
            var ip = IPAddress.Parse("192.168.1.1");

            // Act
            var tokenInfo = new TokenInfo(access, refresh, refreshExpires, accessExpires, user, "Chrome", ip);

            // Assert
            Assert.Equal(Guid.Empty, tokenInfo.Id);
            Assert.Equal(access, tokenInfo.AccessToken);
            Assert.Equal(refresh, tokenInfo.RefreshToken);
            Assert.Equal(accessExpires, tokenInfo.AccessExpiresAt);
            Assert.Equal(refreshExpires, tokenInfo.RefreshExpiresAt);
            Assert.Equal(user, tokenInfo.User);
            Assert.False(tokenInfo.IsRevoked);
            Assert.Equal("Chrome", tokenInfo.DeviceName);
            Assert.Equal(ip, tokenInfo.IpAdress);
        }

        [Fact]
        public void Constructor_WithId_ShouldCreateTokenInfoWithId()
        {
            // Arrange
            var id = Guid.NewGuid();
            var user = CreateValidUser();
            var access = Token.Create("valid-access-token");
            var refresh = Token.Create("valid-refresh-token");
            var accessExpires = DateTime.UtcNow.AddMinutes(30);
            var refreshExpires = DateTime.UtcNow.AddDays(30);
            var ip = IPAddress.Parse("192.168.1.1");

            // Act
            var tokenInfo = new TokenInfo(id, access, refresh, refreshExpires, accessExpires, user, "Chrome", ip);

            // Assert
            Assert.Equal(id, tokenInfo.Id);
            Assert.Equal(access, tokenInfo.AccessToken);
            Assert.Equal(refresh, tokenInfo.RefreshToken);
            Assert.Equal(user, tokenInfo.User);
            Assert.False(tokenInfo.IsRevoked);
        }

        [Fact]
        public void Constructor_WithEmptyGuidId_ShouldKeepEmptyGuid()
        {
            // Arrange
            var id = Guid.Empty;
            var user = CreateValidUser();
            var access = Token.Create("valid-access-token");
            var refresh = Token.Create("valid-refresh-token");
            var accessExpires = DateTime.UtcNow.AddMinutes(30);
            var refreshExpires = DateTime.UtcNow.AddDays(30);
            var ip = IPAddress.Parse("192.168.1.1");

            // Act
            var tokenInfo = new TokenInfo(id, access, refresh, refreshExpires, accessExpires, user, "Chrome", ip);

            // Assert
            Assert.Equal(Guid.Empty, tokenInfo.Id);
        }

        #endregion

        #region Constructor (string) Tests - Cas Nominaux

        [Fact]
        public void Constructor_WithValidStringParameters_ShouldCreateTokenInfo()
        {
            // Arrange
            var user = CreateValidUser();
            var accessExpires = DateTime.UtcNow.AddMinutes(30);
            var refreshExpires = DateTime.UtcNow.AddDays(30);
            var ip = IPAddress.Parse("10.0.0.1");

            // Act
            var tokenInfo = new TokenInfo("valid-access-token", "valid-refresh-token", refreshExpires, accessExpires, user, "Firefox", ip);

            // Assert
            Assert.Equal(Guid.Empty, tokenInfo.Id);
            Assert.NotNull(tokenInfo.AccessToken);
            Assert.Equal("valid-access-token", tokenInfo.AccessToken.Value);
            Assert.NotNull(tokenInfo.RefreshToken);
            Assert.Equal("valid-refresh-token", tokenInfo.RefreshToken.Value);
            Assert.Equal(accessExpires, tokenInfo.AccessExpiresAt);
            Assert.Equal(refreshExpires, tokenInfo.RefreshExpiresAt);
            Assert.Equal(user, tokenInfo.User);
            Assert.False(tokenInfo.IsRevoked);
            Assert.Equal("Firefox", tokenInfo.DeviceName);
            Assert.Equal(ip, tokenInfo.IpAdress);
        }

        [Fact]
        public void Constructor_WithStringAndId_ShouldCreateTokenInfoWithId()
        {
            // Arrange
            var id = Guid.NewGuid();
            var user = CreateValidUser();
            var accessExpires = DateTime.UtcNow.AddMinutes(30);
            var refreshExpires = DateTime.UtcNow.AddDays(30);
            var ip = IPAddress.Parse("172.16.0.1");

            // Act
            var tokenInfo = new TokenInfo(id, "valid-access-token", "valid-refresh-token", refreshExpires, accessExpires, user, "Safari", ip);

            // Assert
            Assert.Equal(id, tokenInfo.Id);
            Assert.NotNull(tokenInfo.AccessToken);
            Assert.NotNull(tokenInfo.RefreshToken);
            Assert.Equal(user, tokenInfo.User);
            Assert.False(tokenInfo.IsRevoked);
        }

        [Fact]
        public void Constructor_WithStringAndEmptyGuidId_ShouldKeepEmptyGuid()
        {
            // Arrange
            var id = Guid.Empty;
            var user = CreateValidUser();
            var accessExpires = DateTime.UtcNow.AddMinutes(30);
            var refreshExpires = DateTime.UtcNow.AddDays(30);
            var ip = IPAddress.Parse("192.168.1.1");

            // Act
            var tokenInfo = new TokenInfo(id, "valid-access-token", "valid-refresh-token", refreshExpires, accessExpires, user, "Chrome", ip);

            // Assert
            Assert.Equal(Guid.Empty, tokenInfo.Id);
        }

        #endregion

        #region Constructor Tests - Cas Limites et Erreurs (User)

        [Fact]
        public void Constructor_WithNullUser_ShouldThrowArgumentNullException()
        {
            // Arrange
            var access = Token.Create("valid-access-token");
            var refresh = Token.Create("valid-refresh-token");
            var accessExpires = DateTime.UtcNow.AddMinutes(30);
            var refreshExpires = DateTime.UtcNow.AddDays(30);
            var ip = IPAddress.Parse("192.168.1.1");

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new TokenInfo(access, refresh, refreshExpires, accessExpires, null, "Chrome", ip));
        }

        [Fact]
        public void Constructor_String_WithNullUser_ShouldThrowArgumentNullException()
        {
            // Arrange
            var accessExpires = DateTime.UtcNow.AddMinutes(30);
            var refreshExpires = DateTime.UtcNow.AddDays(30);
            var ip = IPAddress.Parse("192.168.1.1");

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new TokenInfo("valid-access-token", "valid-refresh-token", refreshExpires, accessExpires, null, "Chrome", ip));
        }

        #endregion

        #region Constructor Tests - Cas Limites et Erreurs (Token strings)

        [Fact]
        public void Constructor_String_WithNullAccessToken_ShouldThrowArgumentException()
        {
            // Arrange
            var user = CreateValidUser();
            var accessExpires = DateTime.UtcNow.AddMinutes(30);
            var refreshExpires = DateTime.UtcNow.AddDays(30);
            var ip = IPAddress.Parse("192.168.1.1");

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new TokenInfo(null, "valid-refresh-token", refreshExpires, accessExpires, user, "Chrome", ip));
        }

        [Fact]
        public void Constructor_String_WithEmptyAccessToken_ShouldThrowArgumentException()
        {
            // Arrange
            var user = CreateValidUser();
            var accessExpires = DateTime.UtcNow.AddMinutes(30);
            var refreshExpires = DateTime.UtcNow.AddDays(30);
            var ip = IPAddress.Parse("192.168.1.1");

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new TokenInfo("", "valid-refresh-token", refreshExpires, accessExpires, user, "Chrome", ip));
        }

        [Fact]
        public void Constructor_String_WithNullRefreshToken_ShouldThrowArgumentException()
        {
            // Arrange
            var user = CreateValidUser();
            var accessExpires = DateTime.UtcNow.AddMinutes(30);
            var refreshExpires = DateTime.UtcNow.AddDays(30);
            var ip = IPAddress.Parse("192.168.1.1");

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new TokenInfo("valid-access-token", null, refreshExpires, accessExpires, user, "Chrome", ip));
        }

        [Fact]
        public void Constructor_String_WithEmptyRefreshToken_ShouldThrowArgumentException()
        {
            // Arrange
            var user = CreateValidUser();
            var accessExpires = DateTime.UtcNow.AddMinutes(30);
            var refreshExpires = DateTime.UtcNow.AddDays(30);
            var ip = IPAddress.Parse("192.168.1.1");

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new TokenInfo("", "valid-refresh-token", refreshExpires, accessExpires, user, "Chrome", ip));
        }

        #endregion

        #region Constructor Tests - Cas Limites et Erreurs (DeviceName)

        [Fact]
        public void Constructor_WithNullDeviceName_ShouldThrowArgumentNullException()
        {
            // Arrange
            var user = CreateValidUser();
            var access = Token.Create("valid-access-token");
            var refresh = Token.Create("valid-refresh-token");
            var accessExpires = DateTime.UtcNow.AddMinutes(30);
            var refreshExpires = DateTime.UtcNow.AddDays(30);
            var ip = IPAddress.Parse("192.168.1.1");

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new TokenInfo(access, refresh, refreshExpires, accessExpires, user, null, ip));
        }

        [Fact]
        public void Constructor_WithEmptyDeviceName_ShouldThrowArgumentNullException()
        {
            // Arrange
            var user = CreateValidUser();
            var access = Token.Create("valid-access-token");
            var refresh = Token.Create("valid-refresh-token");
            var accessExpires = DateTime.UtcNow.AddMinutes(30);
            var refreshExpires = DateTime.UtcNow.AddDays(30);
            var ip = IPAddress.Parse("192.168.1.1");

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new TokenInfo(access, refresh, refreshExpires, accessExpires, user, "", ip));
        }

        [Fact]
        public void Constructor_WithWhitespaceDeviceName_ShouldThrowArgumentNullException()
        {
            // Arrange
            var user = CreateValidUser();
            var access = Token.Create("valid-access-token");
            var refresh = Token.Create("valid-refresh-token");
            var accessExpires = DateTime.UtcNow.AddMinutes(30);
            var refreshExpires = DateTime.UtcNow.AddDays(30);
            var ip = IPAddress.Parse("192.168.1.1");

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new TokenInfo(access, refresh, refreshExpires, accessExpires, user, "   ", ip));
        }

        #endregion

        #region Constructor Tests - Cas Limites et Erreurs (IpAddress)

        [Fact]
        public void Constructor_WithNullIpAddress_ShouldThrowArgumentNullException()
        {
            // Arrange
            var user = CreateValidUser();
            var access = Token.Create("valid-access-token");
            var refresh = Token.Create("valid-refresh-token");
            var accessExpires = DateTime.UtcNow.AddMinutes(30);
            var refreshExpires = DateTime.UtcNow.AddDays(30);

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
                new TokenInfo(access, refresh, refreshExpires, accessExpires, user, "Chrome", (IPAddress)null));
        }

        [Theory]
        [InlineData("192.168.1.1")]
        [InlineData("10.0.0.1")]
        [InlineData("172.16.0.1")]
        [InlineData("0.0.0.0")]
        [InlineData("255.255.255.255")]
        [InlineData("127.0.0.1")]
        [InlineData("::1")]
        [InlineData("2001:0db8:85a3:0000:0000:8a2e:0370:7334")]
        [InlineData("fe80::1")]
        public void Constructor_WithValidIpAddress_ShouldSetIpAddress(string validIp)
        {
            // Arrange
            var user = CreateValidUser();
            var access = Token.Create("valid-access-token");
            var refresh = Token.Create("valid-refresh-token");
            var accessExpires = DateTime.UtcNow.AddMinutes(30);
            var refreshExpires = DateTime.UtcNow.AddDays(30);
            var ip = IPAddress.Parse(validIp);

            // Act
            var tokenInfo = new TokenInfo(access, refresh, refreshExpires, accessExpires, user, "Chrome", ip);

            // Assert
            Assert.Equal(ip, tokenInfo.IpAdress);
        }

        #endregion

        #region Constructor Tests - Cas Limites et Erreurs (Dates)

        [Fact]
        public void Constructor_WithAccessExpiresInPast_ShouldThrowInvalidDataException()
        {
            // Arrange
            var user = CreateValidUser();
            var access = Token.Create("valid-access-token");
            var refresh = Token.Create("valid-refresh-token");
            var accessExpires = DateTime.UtcNow.AddMinutes(-10);
            var refreshExpires = DateTime.UtcNow.AddDays(30);
            var ip = IPAddress.Parse("192.168.1.1");

            // Act & Assert
            Assert.Throws<InvalidDataException>(() =>
                new TokenInfo(access, refresh, refreshExpires, accessExpires, user, "Chrome", ip));
        }

        [Fact]
        public void Constructor_WithRefreshExpiresInPast_ShouldThrowInvalidDataException()
        {
            // Arrange
            var user = CreateValidUser();
            var access = Token.Create("valid-access-token");
            var refresh = Token.Create("valid-refresh-token");
            var accessExpires = DateTime.UtcNow.AddMinutes(30);
            var refreshExpires = DateTime.UtcNow.AddDays(-1);
            var ip = IPAddress.Parse("192.168.1.1");

            // Act & Assert
            Assert.Throws<InvalidDataException>(() =>
                new TokenInfo(access, refresh, refreshExpires, accessExpires, user, "Chrome", ip));
        }

        [Fact]
        public void Constructor_WithAccessExpiresAfterRefreshExpires_ShouldThrowInvalidDataException()
        {
            // Arrange
            var user = CreateValidUser();
            var access = Token.Create("valid-access-token");
            var refresh = Token.Create("valid-refresh-token");
            var accessExpires = DateTime.UtcNow.AddDays(60);
            var refreshExpires = DateTime.UtcNow.AddDays(30);
            var ip = IPAddress.Parse("192.168.1.1");

            // Act & Assert
            Assert.Throws<InvalidDataException>(() =>
                new TokenInfo(access, refresh, refreshExpires, accessExpires, user, "Chrome", ip));
        }

        #endregion

        #region RevokeToken Tests - Cas Nominaux

        [Fact]
        public void RevokeToken_WhenNotRevoked_ShouldSetIsRevokedToTrue()
        {
            // Arrange
            var tokenInfo = CreateValidTokenInfo();
            Assert.False(tokenInfo.IsRevoked);

            // Act
            tokenInfo.RevokeToken();

            // Assert
            Assert.True(tokenInfo.IsRevoked);
        }

        [Fact]
        public void RevokeToken_WhenAlreadyRevoked_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var tokenInfo = CreateValidTokenInfo();
            tokenInfo.RevokeToken();
            Assert.True(tokenInfo.IsRevoked);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => tokenInfo.RevokeToken());
        }

        #endregion

        #region IsAccessExpires Tests

        [Fact]
        public void IsAccessExpires_WhenAccessTokenNotExpired_ShouldReturnFalse()
        {
            // Arrange
            var tokenInfo = CreateValidTokenInfo();

            // Act
            var result = tokenInfo.IsAccessExpires();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsAccessExpires_WhenAccessTokenExpired_ShouldReturnTrue()
        {
            // Arrange
            var user = CreateValidUser();
            var access = Token.Create("valid-access-token");
            var refresh = Token.Create("valid-refresh-token");
            var accessExpires = DateTime.UtcNow.AddSeconds(1);
            var refreshExpires = DateTime.UtcNow.AddDays(30);
            var ip = IPAddress.Parse("192.168.1.1");

            var tokenInfo = new TokenInfo(access, refresh, refreshExpires, accessExpires, user, "Chrome", ip);

            // Wait for access to expire
            Thread.Sleep(1500);

            // Act
            var result = tokenInfo.IsAccessExpires();

            // Assert
            Assert.True(result);
        }

        #endregion

        #region IsRefreshExpires Tests

        [Fact]
        public void IsRefreshExpires_WhenRefreshTokenNotExpired_ShouldReturnFalse()
        {
            // Arrange
            var tokenInfo = CreateValidTokenInfo();

            // Act
            var result = tokenInfo.IsRefreshExpires();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsRefreshExpires_WhenRefreshTokenExpired_ShouldReturnTrue()
        {
            // Arrange
            var user = CreateValidUser();
            var access = Token.Create("valid-access-token");
            var refresh = Token.Create("valid-refresh-token");
            var accessExpires = DateTime.UtcNow.AddSeconds(1);
            var refreshExpires = DateTime.UtcNow.AddSeconds(1);
            var ip = IPAddress.Parse("192.168.1.1");

            var tokenInfo = new TokenInfo(access, refresh, refreshExpires, accessExpires, user, "Chrome", ip);

            // Wait for refresh to expire
            Thread.Sleep(1500);

            // Act
            var result = tokenInfo.IsRefreshExpires();

            // Assert
            Assert.True(result);
        }

        #endregion

        #region Helper Methods

        private AuthUser CreateValidUser()
        {
            return new AuthUser("TestUser", "avatar.png", "test@test.com", "P@ssword1", RoleUser.User);
        }

        private TokenInfo CreateValidTokenInfo()
        {
            var user = CreateValidUser();
            var access = Token.Create("valid-access-token");
            var refresh = Token.Create("valid-refresh-token");
            var accessExpires = DateTime.UtcNow.AddMinutes(30);
            var refreshExpires = DateTime.UtcNow.AddDays(30);
            var ip = IPAddress.Parse("192.168.1.1");

            return new TokenInfo(access, refresh, refreshExpires, accessExpires, user, "Chrome", ip);
        }

        #endregion
    }
}
