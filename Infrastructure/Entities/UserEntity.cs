namespace Infrastructure.Entities
{
    public class UserEntity : BaseEntity
    {
        public string Avatar { get; set; }

        public string Pseudo { get; set; }

        public DateTime? ExpiresAt { get; set; }
    }
}
