using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.User
{
    public class AuthUser : User
    {

        public Mail Mail { get; private set; }

        public Password Password { get; private set; }

        public DateTime? LastChangePassword { get; private set; }

        public DateTime? LastLogin { get; private set; }

        public RoleUser Role { get; private set; }

        public bool PasswordMustBeChanged { get; private set; }

        public AuthUser() : base()
        {

        }

        public AuthUser(string pseudo, string avatar, string mail, string password, RoleUser role) : base(avatar, pseudo)
        {
            ChangeMail(mail);
            Password = Password.Create(password);
            ChangeRole(role);
            PasswordMustBeChanged = false;
        }

        public AuthUser(Guid id, string pseudo, string avatar, string mail, string password, RoleUser role) : base(id, avatar, pseudo)
        {
            ChangeMail(mail);
            Password = Password.Create(password);
            ChangeRole(role);
            PasswordMustBeChanged = false;
        }

        public void ChangeMail(string mail)
        {
            Mail = Mail.Create(mail);
        }

        public void ChangePassword(string password)
        {
            Password = Password.Create(password);
            LastChangePassword = DateTime.UtcNow;
            PasswordMustBeChanged = false;
        }

        public void ChangeRole(RoleUser role)
        {
            Role = role;
        }

        public void Login()
        {
            LastLogin = DateTime.UtcNow;
        }

        public void ChangePasswordNextTime()
        {
            PasswordMustBeChanged = true;
        }
    }
}
