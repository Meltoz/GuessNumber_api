namespace Infrastructure.Entities
{
    public class TokenEntity : BaseEntity
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }

        public bool IsRevoked { get; set; }

        public DateTime AccessExpiresAt { get; set; }

        public DateTime RefreshExpiresAt { get; set; }

        public string DeviceName { get; set; }

        public string IpAddress { get; set; }

        public Guid UserId { get; set; }

        public AuthUserEntity User { get; set; }
    }
}
