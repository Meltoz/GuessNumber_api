using Domain.ValueObjects;
using System.Net;

namespace Domain.User
{
    public class TokenInfo
    {
        public Guid Id { get; private set; }

        public Token AccessToken { get; private set; }

        public Token RefreshToken { get; private set; }

        public DateTime RefreshExpiresAt { get; private set; }

        public DateTime AccessExpiresAt { get; private set; }

        public bool IsRevoked { get; private set; }

        public AuthUser User { get; private set; }

        public string DeviceName { get; private set; }

        public IPAddress IpAdress { get; private set; }

        private TokenInfo()
        {

        }

        public TokenInfo(Token access, Token refresh, DateTime refreshExpires, DateTime accessExpires, AuthUser user, string deviceName, IPAddress ipAddress)
        {
            AccessToken = access;
            RefreshToken = refresh;
            IsRevoked = false;

            if (user is null)
                throw new ArgumentNullException(nameof(user));
            User = user;

            DefineDeviceName(deviceName);
            DefineIpAddress(ipAddress);

            ValidDate(accessExpires, refreshExpires);
        }

        public TokenInfo(Guid id, Token access, Token refresh, DateTime refreshExpires, DateTime accessExpires, AuthUser user, string deviceName, IPAddress ipAddress) :
            this(access, refresh, refreshExpires, accessExpires, user, deviceName, ipAddress)
        {
            if (id != Guid.Empty)
                Id = id;
        }

        public TokenInfo(string access, string refresh, DateTime refreshExpires, DateTime accessExpires, AuthUser user, string deviceName, IPAddress ipAddress)
        {
            AccessToken = Token.Create(access);
            RefreshToken = Token.Create(refresh);
            IsRevoked = false;

            if (user is null)
                throw new ArgumentNullException(nameof(user));
            User = user;

            DefineDeviceName(deviceName);
            DefineIpAddress(ipAddress);

            ValidDate(accessExpires, refreshExpires);
        }

        public TokenInfo(Guid id, string access, string refresh, DateTime refreshExpires, DateTime accessExpires, AuthUser user, string deviceName, IPAddress ipAddress) :
            this(access, refresh, refreshExpires, accessExpires, user, deviceName, ipAddress)
        {
            if (id != Guid.Empty)
                Id = id;
        }

        public void RevokeToken()
        {
            if (IsRevoked)
                throw new InvalidOperationException("Token already revokes");

            IsRevoked = true;
        }

        public bool IsRefreshExpires()
        {
            var today = DateTime.UtcNow;

            return RefreshExpiresAt < today;
        }

        public bool IsAccessExpires()
        {
            var today = DateTime.UtcNow;

            return AccessExpiresAt < today;
        }

        #region Private methods
        private void ValidDate(DateTime accessExpires, DateTime refreshExpires)
        {
            var now = DateTime.UtcNow;

            if (accessExpires < now)
                throw new InvalidDataException();

            if (refreshExpires < now)
                throw new InvalidDataException();

            if (accessExpires > refreshExpires)
                throw new InvalidDataException();

            RefreshExpiresAt = refreshExpires;
            AccessExpiresAt = accessExpires;
        }

        private void DefineDeviceName(string deviceName)
        {
            if (string.IsNullOrWhiteSpace(deviceName))
                throw new ArgumentNullException(nameof(deviceName));

            DeviceName = deviceName;
        }

        private void DefineIpAddress(IPAddress ipAddress)
        {
            if (ipAddress is null)
                throw new ArgumentNullException(nameof(ipAddress));

            IpAdress = ipAddress;
        }

        #endregion
    }
}
