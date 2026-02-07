namespace Domain.User
{
    public class GuestUser : User
    {
        public DateTime ExpiresAt { get; private set; }

        public GuestUser(string pseudo, string avatar) : base(avatar, pseudo)
        {
            ExpiresAt = DateTime.UtcNow.AddDays(1);
        }

        public GuestUser(Guid id, string pseudo, string avatar) : base(id, avatar, pseudo)
        {
            ExpiresAt = DateTime.UtcNow.AddDays(1);
        }
        private GuestUser() : base()
        {

        }
    }
}
