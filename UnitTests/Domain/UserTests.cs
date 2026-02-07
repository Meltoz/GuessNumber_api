using Domain.User;
using Domain.ValueObjects;

namespace UnitTests.Domain
{
    /// <summary>
    /// Tests for the abstract User class functionality.
    /// Since User is abstract, we use GuestUser (which inherits from User) to test the base class behavior.
    /// For GuestUser-specific tests, see GuestUserTests.cs
    /// </summary>
    public class UserTests
    {
        #region Constructor Tests - Cas Nominaux (via GuestUser)

        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateUser()
        {
            // Arrange
            var pseudo = "TestUser";
            var avatar = "avatar.png";

            // Act - Using GuestUser since User is abstract
            var user = new GuestUser(pseudo, avatar);

            // Assert
            Assert.Equal(Guid.Empty, user.Id);
            Assert.Equal(avatar, user.Avatar);
            Assert.NotNull(user.Pseudo);
            Assert.Equal(pseudo, user.Pseudo.Value);
        }

        [Fact]
        public void Constructor_WithId_ShouldCreateUserWithId()
        {
            // Arrange
            var id = Guid.NewGuid();
            var pseudo = "TestUser";
            var avatar = "avatar.png";

            // Act - Using GuestUser since User is abstract
            var user = new GuestUser(id, pseudo, avatar);

            // Assert
            Assert.Equal(id, user.Id);
            Assert.Equal(avatar, user.Avatar);
            Assert.NotNull(user.Pseudo);
            Assert.Equal(pseudo, user.Pseudo.Value);
        }

        [Fact]
        public void Constructor_WithEmptyGuidId_ShouldKeepEmptyGuid()
        {
            // Arrange
            var id = Guid.Empty;
            var pseudo = "TestUser";
            var avatar = "avatar.png";

            // Act - Using GuestUser since User is abstract
            var user = new GuestUser(id, pseudo, avatar);

            // Assert
            Assert.Equal(Guid.Empty, user.Id);
        }

        [Fact]
        public void Constructor_WithEmptyAvatar_ShouldCreateUserWithEmptyAvatar()
        {
            // Arrange
            var pseudo = "TestUser";
            var avatar = "";

            // Act - Using GuestUser since User is abstract
            var user = new GuestUser(pseudo, avatar);

            // Assert
            Assert.Equal(avatar, user.Avatar);
            Assert.NotNull(user.Pseudo);
        }

        [Fact]
        public void Constructor_WithWhitespacePseudo_ShouldTrimPseudo()
        {
            // Arrange
            var pseudo = "  TestUser  ";
            var avatar = "avatar.png";

            // Act - Using GuestUser since User is abstract
            var user = new GuestUser(pseudo, avatar);

            // Assert
            Assert.Equal("TestUser", user.Pseudo.Value);
        }

        #endregion

        #region ChangePseudo Tests - Cas Nominaux

        [Fact]
        public void ChangePseudo_WithValidPseudo_ShouldUpdatePseudo()
        {
            // Arrange
            var user = CreateValidUser();
            var newPseudo = "NewPseudo";

            // Act
            user.ChangePseudo(newPseudo);

            // Assert
            Assert.Equal(newPseudo, user.Pseudo.Value);
        }

        [Fact]
        public void ChangePseudo_WithWhitespacePseudo_ShouldTrimAndUpdate()
        {
            // Arrange
            var user = CreateValidUser();
            var newPseudo = "  NewPseudo  ";

            // Act
            user.ChangePseudo(newPseudo);

            // Assert
            Assert.Equal("NewPseudo", user.Pseudo.Value);
        }

        #endregion

        #region ChangeAvatar Tests - Cas Nominaux

        [Fact]
        public void ChangeAvatar_WithValidAvatar_ShouldUpdateAvatar()
        {
            // Arrange
            var user = CreateValidUser();
            var newAvatar = "new_avatar.png";

            // Act
            user.ChangeAvatar(newAvatar);

            // Assert
            Assert.Equal(newAvatar, user.Avatar);
        }

        [Fact]
        public void ChangeAvatar_WithEmptyAvatar_ShouldUpdateToEmptyAvatar()
        {
            // Arrange
            var user = CreateValidUser();
            var newAvatar = "";

            // Act
            user.ChangeAvatar(newAvatar);

            // Assert
            Assert.Equal(newAvatar, user.Avatar);
        }

        [Fact]
        public void ChangeAvatar_WithNullAvatar_ShouldUpdateToNull()
        {
            // Arrange
            var user = CreateValidUser();
            string newAvatar = null;

            // Act
            user.ChangeAvatar(newAvatar);

            // Assert
            Assert.Null(user.Avatar);
        }

        #endregion

        #region Constructor Tests - Cas Limites et Erreurs

        [Fact]
        public void Constructor_WithNullPseudo_ShouldThrowArgumentException()
        {
            // Arrange
            string pseudo = null;
            var avatar = "avatar.png";

            // Act & Assert - Using GuestUser since User is abstract
            var exception = Assert.Throws<ArgumentException>(() => new GuestUser(pseudo, avatar));
            Assert.Equal("Pseudo can't be empty", exception.Message);
        }

        [Fact]
        public void Constructor_WithEmptyPseudo_ShouldThrowArgumentException()
        {
            // Arrange
            var pseudo = "";
            var avatar = "avatar.png";

            // Act & Assert - Using GuestUser since User is abstract
            var exception = Assert.Throws<ArgumentException>(() => new GuestUser(pseudo, avatar));
            Assert.Equal("Pseudo can't be empty", exception.Message);
        }

        [Fact]
        public void Constructor_WithWhitespacePseudoOnly_ShouldThrowArgumentException()
        {
            // Arrange
            var pseudo = "   ";
            var avatar = "avatar.png";

            // Act & Assert - Using GuestUser since User is abstract
            var exception = Assert.Throws<ArgumentException>(() => new GuestUser(pseudo, avatar));
            Assert.Equal("Pseudo can't be empty", exception.Message);
        }

        #endregion

        #region ChangePseudo Tests - Cas Limites et Erreurs

        [Fact]
        public void ChangePseudo_WithNullPseudo_ShouldThrowArgumentException()
        {
            // Arrange
            var user = CreateValidUser();
            string newPseudo = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => user.ChangePseudo(newPseudo));
            Assert.Equal("Pseudo can't be empty", exception.Message);
        }

        [Fact]
        public void ChangePseudo_WithEmptyPseudo_ShouldThrowArgumentException()
        {
            // Arrange
            var user = CreateValidUser();
            var newPseudo = "";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => user.ChangePseudo(newPseudo));
            Assert.Equal("Pseudo can't be empty", exception.Message);
        }

        [Fact]
        public void ChangePseudo_WithWhitespacePseudoOnly_ShouldThrowArgumentException()
        {
            // Arrange
            var user = CreateValidUser();
            var newPseudo = "   ";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => user.ChangePseudo(newPseudo));
            Assert.Equal("Pseudo can't be empty", exception.Message);
        }

        #endregion

        #region Helper Methods

        private GuestUser CreateValidUser()
        {
            return new GuestUser("TestUser", "avatar.png");
        }

        #endregion
    }
}
