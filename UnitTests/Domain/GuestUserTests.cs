using Domain.User;

namespace UnitTests.Domain
{
    public class GuestUserTests
    {
        #region Constructor Tests - Cas Nominaux

        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateGuestUser()
        {
            // Arrange
            var pseudo = "TestUser";
            var avatar = "avatar.png";

            // Act
            var guestUser = new GuestUser(pseudo, avatar);

            // Assert
            Assert.Equal(Guid.Empty, guestUser.Id);
            Assert.Equal(avatar, guestUser.Avatar);
            Assert.NotNull(guestUser.Pseudo);
            Assert.Equal(pseudo, guestUser.Pseudo.Value);
        }

        [Fact]
        public void Constructor_WithValidParameters_ShouldSetExpiresAtToOneDayFromNow()
        {
            // Arrange
            var pseudo = "TestUser";
            var avatar = "avatar.png";
            var beforeCreation = DateTime.UtcNow;

            // Act
            var guestUser = new GuestUser(pseudo, avatar);

            // Assert
            var afterCreation = DateTime.UtcNow;
            var expectedMinExpiry = beforeCreation.AddDays(1);
            var expectedMaxExpiry = afterCreation.AddDays(1);

            Assert.True(guestUser.ExpiresAt >= expectedMinExpiry);
            Assert.True(guestUser.ExpiresAt <= expectedMaxExpiry);
        }

        [Fact]
        public void Constructor_WithId_ShouldCreateGuestUserWithId()
        {
            // Arrange
            var id = Guid.NewGuid();
            var pseudo = "TestUser";
            var avatar = "avatar.png";

            // Act
            var guestUser = new GuestUser(id, pseudo, avatar);

            // Assert
            Assert.Equal(id, guestUser.Id);
            Assert.Equal(avatar, guestUser.Avatar);
            Assert.NotNull(guestUser.Pseudo);
            Assert.Equal(pseudo, guestUser.Pseudo.Value);
        }

        [Fact]
        public void Constructor_WithId_ShouldSetExpiresAtToOneDayFromNow()
        {
            // Arrange
            var id = Guid.NewGuid();
            var pseudo = "TestUser";
            var avatar = "avatar.png";
            var beforeCreation = DateTime.UtcNow;

            // Act
            var guestUser = new GuestUser(id, pseudo, avatar);

            // Assert
            var afterCreation = DateTime.UtcNow;
            var expectedMinExpiry = beforeCreation.AddDays(1);
            var expectedMaxExpiry = afterCreation.AddDays(1);

            Assert.True(guestUser.ExpiresAt >= expectedMinExpiry);
            Assert.True(guestUser.ExpiresAt <= expectedMaxExpiry);
        }

        [Fact]
        public void Constructor_WithEmptyGuidId_ShouldKeepEmptyGuid()
        {
            // Arrange
            var id = Guid.Empty;
            var pseudo = "TestUser";
            var avatar = "avatar.png";

            // Act
            var guestUser = new GuestUser(id, pseudo, avatar);

            // Assert
            Assert.Equal(Guid.Empty, guestUser.Id);
        }

        [Fact]
        public void Constructor_WithEmptyAvatar_ShouldCreateGuestUserWithEmptyAvatar()
        {
            // Arrange
            var pseudo = "TestUser";
            var avatar = "";

            // Act
            var guestUser = new GuestUser(pseudo, avatar);

            // Assert
            Assert.Equal(avatar, guestUser.Avatar);
            Assert.NotNull(guestUser.Pseudo);
        }

        [Fact]
        public void Constructor_WithWhitespacePseudo_ShouldTrimPseudo()
        {
            // Arrange
            var pseudo = "  TestUser  ";
            var avatar = "avatar.png";

            // Act
            var guestUser = new GuestUser(pseudo, avatar);

            // Assert
            Assert.Equal("TestUser", guestUser.Pseudo.Value);
        }

        #endregion

        #region Inherited Tests - ChangePseudo

        [Fact]
        public void ChangePseudo_WithValidPseudo_ShouldUpdatePseudo()
        {
            // Arrange
            var guestUser = CreateValidGuestUser();
            var newPseudo = "NewPseudo";

            // Act
            guestUser.ChangePseudo(newPseudo);

            // Assert
            Assert.Equal(newPseudo, guestUser.Pseudo.Value);
        }

        [Fact]
        public void ChangePseudo_WithWhitespacePseudo_ShouldTrimAndUpdate()
        {
            // Arrange
            var guestUser = CreateValidGuestUser();
            var newPseudo = "  NewPseudo  ";

            // Act
            guestUser.ChangePseudo(newPseudo);

            // Assert
            Assert.Equal("NewPseudo", guestUser.Pseudo.Value);
        }

        #endregion

        #region Inherited Tests - ChangeAvatar

        [Fact]
        public void ChangeAvatar_WithValidAvatar_ShouldUpdateAvatar()
        {
            // Arrange
            var guestUser = CreateValidGuestUser();
            var newAvatar = "new_avatar.png";

            // Act
            guestUser.ChangeAvatar(newAvatar);

            // Assert
            Assert.Equal(newAvatar, guestUser.Avatar);
        }

        [Fact]
        public void ChangeAvatar_WithEmptyAvatar_ShouldUpdateToEmptyAvatar()
        {
            // Arrange
            var guestUser = CreateValidGuestUser();
            var newAvatar = "";

            // Act
            guestUser.ChangeAvatar(newAvatar);

            // Assert
            Assert.Equal(newAvatar, guestUser.Avatar);
        }

        [Fact]
        public void ChangeAvatar_WithNullAvatar_ShouldUpdateToNull()
        {
            // Arrange
            var guestUser = CreateValidGuestUser();
            string newAvatar = null;

            // Act
            guestUser.ChangeAvatar(newAvatar);

            // Assert
            Assert.Null(guestUser.Avatar);
        }

        #endregion

        #region Constructor Tests - Cas Limites et Erreurs

        [Fact]
        public void Constructor_WithNullPseudo_ShouldThrowArgumentException()
        {
            // Arrange
            string pseudo = null;
            var avatar = "avatar.png";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new GuestUser(pseudo, avatar));
            Assert.Equal("Pseudo can't be empty", exception.Message);
        }

        [Fact]
        public void Constructor_WithEmptyPseudo_ShouldThrowArgumentException()
        {
            // Arrange
            var pseudo = "";
            var avatar = "avatar.png";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new GuestUser(pseudo, avatar));
            Assert.Equal("Pseudo can't be empty", exception.Message);
        }

        [Fact]
        public void Constructor_WithWhitespacePseudoOnly_ShouldThrowArgumentException()
        {
            // Arrange
            var pseudo = "   ";
            var avatar = "avatar.png";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new GuestUser(pseudo, avatar));
            Assert.Equal("Pseudo can't be empty", exception.Message);
        }

        #endregion

        #region ChangePseudo Tests - Cas Limites et Erreurs

        [Fact]
        public void ChangePseudo_WithNullPseudo_ShouldThrowArgumentException()
        {
            // Arrange
            var guestUser = CreateValidGuestUser();
            string newPseudo = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => guestUser.ChangePseudo(newPseudo));
            Assert.Equal("Pseudo can't be empty", exception.Message);
        }

        [Fact]
        public void ChangePseudo_WithEmptyPseudo_ShouldThrowArgumentException()
        {
            // Arrange
            var guestUser = CreateValidGuestUser();
            var newPseudo = "";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => guestUser.ChangePseudo(newPseudo));
            Assert.Equal("Pseudo can't be empty", exception.Message);
        }

        [Fact]
        public void ChangePseudo_WithWhitespacePseudoOnly_ShouldThrowArgumentException()
        {
            // Arrange
            var guestUser = CreateValidGuestUser();
            var newPseudo = "   ";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => guestUser.ChangePseudo(newPseudo));
            Assert.Equal("Pseudo can't be empty", exception.Message);
        }

        #endregion

        #region Helper Methods

        private GuestUser CreateValidGuestUser()
        {
            return new GuestUser("TestUser", "avatar.png");
        }

        #endregion
    }
}
