using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels
{
    public class AuthUserSummaryVM
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string Pseudo { get; set; }

        [Required]
        public string Avatar { get; set; }
    }

    public class AuthUserDetailVM : AuthUserSummaryVM
    {
        public DateTime? LastChangePassword { get; set; }

        public DateTime? LastLogin { get; set; }

        public string Email { get; set; }
    }
}
