namespace Web.ViewModels.Admin
{
    public class UserAdminVM
    {
        public Guid Id { get; set; }

        public string Pseudo { get; set; }

        public string Avatar { get; set; }

        public string Email { get; set; }

        public string Role { get; set; }

        public DateTime LastChangePassword { get; set; }

        public DateTime LastLogin { get; set; }
    }
}
