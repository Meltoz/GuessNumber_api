using Domain.ValueObjects;

namespace Domain.User
{
    public abstract class User
    {
        public Guid Id { get; private set; }

        public string Avatar { get; private set; }

        public Pseudo Pseudo { get; private set; }

        protected User() { }

        public User(string avatar, string pseudo)
        {
            ChangeAvatar(avatar);
            ChangePseudo(pseudo);
        }

        public User(Guid id,  string avatar, string pseudo): this(avatar, pseudo)
        {
            if(id != Guid.Empty)
                Id = id;
        }

        public void ChangePseudo(string pseudo)
        {
            Pseudo = Pseudo.Create(pseudo);
        }

        public void ChangeAvatar(string avatar)
        {
            Avatar = avatar;
        }
    }
}
