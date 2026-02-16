using Domain.Enums;

namespace Infrastructure.Entities
{
    public class AuthUserEntity : UserEntity
    {
        public string Email { get; set; }

        public string Password { get; set; }

        public RoleUser Role { get; set; }

        public bool PasswordMustBeChanged { get; set; }

        public DateTime? LastLogin { get; set; }

        public DateTime? LastChangePassword { get; set; }

        public ICollection<TokenEntity> Tokens { get; set; }


    }
}
