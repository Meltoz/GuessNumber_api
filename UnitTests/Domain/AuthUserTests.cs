using Domain.Enums;
using Domain.User;
using Domain.ValueObjects;

namespace UnitTests.Domain
{
    public class AuthUserTests
    {
        #region Constructor Tests - Cas Nominaux

        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateAuthUser()
        {
            // Arrange
            var pseudo = "TestUser";
            var avatar = "avatar.png";
            var mail = "test@example.com";
            var password = "Password1@";

            // Act
            var authUser = new AuthUser(pseudo, avatar, mail, password);

            // Assert
            Assert.Equal(Guid.Empty, authUser.Id);
            Assert.Equal(avatar, authUser.Avatar);
            Assert.NotNull(authUser.Pseudo);
            Assert.Equal(pseudo, authUser.Pseudo.Value);
            Assert.NotNull(authUser.Mail);
            Assert.Equal(mail, authUser.Mail.ToString());
            Assert.NotNull(authUser.Password);
            Assert.Null(authUser.LastChangePassword);
            Assert.Null(authUser.LastLogin);
            Assert.Equal(RoleUser.User, authUser.Role);
        }

        [Fact]
        public void Constructor_WithId_ShouldCreateAuthUserWithId()
        {
            // Arrange
            var id = Guid.NewGuid();
            var pseudo = "TestUser";
            var avatar = "avatar.png";
            var mail = "test@example.com";
            var password = "Password1@";

            // Act
            var authUser = new AuthUser(id, pseudo, avatar, mail, password);

            // Assert
            Assert.Equal(id, authUser.Id);
            Assert.Equal(avatar, authUser.Avatar);
            Assert.NotNull(authUser.Pseudo);
            Assert.Equal(pseudo, authUser.Pseudo.Value);
            Assert.NotNull(authUser.Mail);
            Assert.NotNull(authUser.Password);
        }

        [Fact]
        public void Constructor_WithEmptyGuidId_ShouldKeepEmptyGuid()
        {
            // Arrange
            var id = Guid.Empty;
            var pseudo = "TestUser";
            var avatar = "avatar.png";
            var mail = "test@example.com";
            var password = "Password1@";

            // Act
            var authUser = new AuthUser(id, pseudo, avatar, mail, password);

            // Assert
            Assert.Equal(Guid.Empty, authUser.Id);
        }

        [Fact]
        public void Constructor_WithValidEmail_ShouldCreateAuthUserWithTrimmedEmail()
        {
            // Arrange
            var pseudo = "TestUser";
            var avatar = "avatar.png";
            var mail = "  test@example.com  ";
            var password = "Password1@";

            // Act
            var authUser = new AuthUser(pseudo, avatar, mail, password);

            // Assert
            Assert.Equal("test@example.com", authUser.Mail.ToString());
        }

        #endregion

        #region ChangeMail Tests - Cas Nominaux

        [Fact]
        public void ChangeMail_WithValidMail_ShouldUpdateMail()
        {
            // Arrange
            var authUser = CreateValidAuthUser();
            var newMail = "newemail@example.com";

            // Act
            authUser.ChangeMail(newMail);

            // Assert
            Assert.Equal(newMail, authUser.Mail.ToString());
        }

        [Fact]
        public void ChangeMail_WithValidMailWithWhitespace_ShouldTrimAndUpdate()
        {
            // Arrange
            var authUser = CreateValidAuthUser();
            var newMail = "  newemail@example.com  ";

            // Act
            authUser.ChangeMail(newMail);

            // Assert
            Assert.Equal("newemail@example.com", authUser.Mail.ToString());
        }

        #endregion

        #region ChangePassword Tests - Cas Nominaux

        [Fact]
        public void ChangePassword_WithValidPassword_ShouldUpdatePassword()
        {
            // Arrange
            var authUser = CreateValidAuthUser();
            var initialLastChangePassword = authUser.LastChangePassword;
            var newPassword = "NewPassword1@";

            // Wait a small amount to ensure time difference
            Thread.Sleep(10);

            // Act
            authUser.ChangePassword(newPassword);

            // Assert
            Assert.NotNull(authUser.Password);
            Assert.NotNull(authUser.LastChangePassword);
            Assert.NotEqual(initialLastChangePassword, authUser.LastChangePassword);
            Assert.True(authUser.LastChangePassword > initialLastChangePassword || initialLastChangePassword == null);
        }

        [Fact]
        public void ChangePassword_ShouldSetLastChangePasswordToUtcNow()
        {
            // Arrange
            var authUser = CreateValidAuthUser();
            var beforeChange = DateTime.UtcNow;

            // Act
            authUser.ChangePassword("NewPassword1@");

            // Assert
            var afterChange = DateTime.UtcNow;
            Assert.NotNull(authUser.LastChangePassword);
            Assert.True(authUser.LastChangePassword >= beforeChange);
            Assert.True(authUser.LastChangePassword <= afterChange);
        }

        [Fact]
        public void ChangePassword_MultipleTimes_ShouldUpdateLastChangePasswordEachTime()
        {
            // Arrange
            var authUser = CreateValidAuthUser();

            // Act
            authUser.ChangePassword("FirstPassword1@");
            var firstChangeTime = authUser.LastChangePassword;

            Thread.Sleep(10);

            authUser.ChangePassword("SecondPassword1@");
            var secondChangeTime = authUser.LastChangePassword;

            // Assert
            Assert.NotNull(firstChangeTime);
            Assert.NotNull(secondChangeTime);
            Assert.True(secondChangeTime > firstChangeTime);
        }

        #endregion

        #region Login Tests - Cas Nominaux

        [Fact]
        public void Login_ShouldSetLastLoginToUtcNow()
        {
            // Arrange
            var authUser = CreateValidAuthUser();
            var beforeLogin = DateTime.UtcNow;

            // Act
            authUser.Login();

            // Assert
            var afterLogin = DateTime.UtcNow;
            Assert.NotNull(authUser.LastLogin);
            Assert.True(authUser.LastLogin >= beforeLogin);
            Assert.True(authUser.LastLogin <= afterLogin);
        }

        [Fact]
        public void Login_MultipleTimes_ShouldUpdateLastLoginEachTime()
        {
            // Arrange
            var authUser = CreateValidAuthUser();

            // Act
            authUser.Login();
            var firstLoginTime = authUser.LastLogin;

            Thread.Sleep(10);

            authUser.Login();
            var secondLoginTime = authUser.LastLogin;

            // Assert
            Assert.NotNull(firstLoginTime);
            Assert.NotNull(secondLoginTime);
            Assert.True(secondLoginTime > firstLoginTime);
        }

        [Fact]
        public void Login_WhenLastLoginWasNull_ShouldSetLastLogin()
        {
            // Arrange
            var authUser = CreateValidAuthUser();
            Assert.Null(authUser.LastLogin);

            // Act
            authUser.Login();

            // Assert
            Assert.NotNull(authUser.LastLogin);
        }

        #endregion

        #region Inherited Tests - ChangePseudo

        [Fact]
        public void ChangePseudo_WithValidPseudo_ShouldUpdatePseudo()
        {
            // Arrange
            var authUser = CreateValidAuthUser();
            var newPseudo = "NewPseudo";

            // Act
            authUser.ChangePseudo(newPseudo);

            // Assert
            Assert.Equal(newPseudo, authUser.Pseudo.Value);
        }

        #endregion

        #region Inherited Tests - ChangeAvatar

        [Fact]
        public void ChangeAvatar_WithValidAvatar_ShouldUpdateAvatar()
        {
            // Arrange
            var authUser = CreateValidAuthUser();
            var newAvatar = "new_avatar.png";

            // Act
            authUser.ChangeAvatar(newAvatar);

            // Assert
            Assert.Equal(newAvatar, authUser.Avatar);
        }

        #endregion

        #region Constructor Tests - Cas Limites et Erreurs

        [Fact]
        public void Constructor_WithNullMail_ShouldThrowArgumentException()
        {
            // Arrange
            var pseudo = "TestUser";
            var avatar = "avatar.png";
            string mail = null;
            var password = "Password1@";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new AuthUser(pseudo, avatar, mail, password));
            Assert.Equal("Mail can't be empty", exception.Message);
        }

        [Fact]
        public void Constructor_WithEmptyMail_ShouldThrowArgumentException()
        {
            // Arrange
            var pseudo = "TestUser";
            var avatar = "avatar.png";
            var mail = "";
            var password = "Password1@";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new AuthUser(pseudo, avatar, mail, password));
            Assert.Equal("Mail can't be empty", exception.Message);
        }

        [Fact]
        public void Constructor_WithWhitespaceMail_ShouldThrowArgumentException()
        {
            // Arrange
            var pseudo = "TestUser";
            var avatar = "avatar.png";
            var mail = "   ";
            var password = "Password1@";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new AuthUser(pseudo, avatar, mail, password));
            Assert.Equal("Mail can't be empty", exception.Message);
        }

        [Fact]
        public void Constructor_WithInvalidMailFormat_ShouldThrowArgumentException()
        {
            // Arrange
            var pseudo = "TestUser";
            var avatar = "avatar.png";
            var mail = "invalid-email";
            var password = "Password1@";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new AuthUser(pseudo, avatar, mail, password));
            Assert.Equal("L'adresse email n'est pas valide.", exception.Message);
        }

        [Fact]
        public void Constructor_WithMailMissingAtSymbol_ShouldThrowArgumentException()
        {
            // Arrange
            var pseudo = "TestUser";
            var avatar = "avatar.png";
            var mail = "testexample.com";
            var password = "Password1@";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new AuthUser(pseudo, avatar, mail, password));
            Assert.Equal("L'adresse email n'est pas valide.", exception.Message);
        }

        [Fact]
        public void Constructor_WithMailMissingDomain_ShouldThrowArgumentException()
        {
            // Arrange
            var pseudo = "TestUser";
            var avatar = "avatar.png";
            var mail = "test@";
            var password = "Password1@";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new AuthUser(pseudo, avatar, mail, password));
            Assert.Equal("L'adresse email n'est pas valide.", exception.Message);
        }

        [Fact]
        public void Constructor_WithNullPassword_ShouldThrowArgumentException()
        {
            // Arrange
            var pseudo = "TestUser";
            var avatar = "avatar.png";
            var mail = "test@example.com";
            string password = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new AuthUser(pseudo, avatar, mail, password));
            Assert.Equal("Password can't be empty", exception.Message);
        }

        [Fact]
        public void Constructor_WithEmptyPassword_ShouldThrowArgumentException()
        {
            // Arrange
            var pseudo = "TestUser";
            var avatar = "avatar.png";
            var mail = "test@example.com";
            var password = "";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new AuthUser(pseudo, avatar, mail, password));
            Assert.Equal("Password can't be empty", exception.Message);
        }

        [Fact]
        public void Constructor_WithWhitespacePassword_ShouldThrowArgumentException()
        {
            // Arrange
            var pseudo = "TestUser";
            var avatar = "avatar.png";
            var mail = "test@example.com";
            var password = "   ";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new AuthUser(pseudo, avatar, mail, password));
            Assert.Equal("Password can't be empty", exception.Message);
        }

        [Fact]
        public void Constructor_WithPasswordTooShort_ShouldThrowArgumentException()
        {
            // Arrange
            var pseudo = "TestUser";
            var avatar = "avatar.png";
            var mail = "test@example.com";
            var password = "Ab";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new AuthUser(pseudo, avatar, mail, password));
            Assert.Equal("Password must be between 3 and 50 character", exception.Message);
        }

        [Fact]
        public void Constructor_WithPasswordMissingUppercase_ShouldThrowArgumentException()
        {
            // Arrange
            var pseudo = "TestUser";
            var avatar = "avatar.png";
            var mail = "test@example.com";
            var password = "password1@";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new AuthUser(pseudo, avatar, mail, password));
            Assert.Equal("Password must contains atleast a minus, a capital, a digit and a minimum of 8 characters", exception.Message);
        }

        [Fact]
        public void Constructor_WithPasswordMissingLowercase_ShouldThrowArgumentException()
        {
            // Arrange
            var pseudo = "TestUser";
            var avatar = "avatar.png";
            var mail = "test@example.com";
            var password = "PASSWORD1@";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new AuthUser(pseudo, avatar, mail, password));
            Assert.Equal("Password must contains atleast a minus, a capital, a digit and a minimum of 8 characters", exception.Message);
        }

        [Fact]
        public void Constructor_WithPasswordMissingDigit_ShouldThrowArgumentException()
        {
            // Arrange
            var pseudo = "TestUser";
            var avatar = "avatar.png";
            var mail = "test@example.com";
            var password = "Password@";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new AuthUser(pseudo, avatar, mail, password));
            Assert.Equal("Password must contains atleast a minus, a capital, a digit and a minimum of 8 characters", exception.Message);
        }

        [Fact]
        public void Constructor_WithPasswordMissingSpecialChar_ShouldThrowArgumentException()
        {
            // Arrange
            var pseudo = "TestUser";
            var avatar = "avatar.png";
            var mail = "test@example.com";
            var password = "Password1";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new AuthUser(pseudo, avatar, mail, password));
            Assert.Equal("Password must contains atleast a minus, a capital, a digit and a minimum of 8 characters", exception.Message);
        }

        [Fact]
        public void Constructor_WithPasswordLessThan8Characters_ShouldThrowArgumentException()
        {
            // Arrange
            var pseudo = "TestUser";
            var avatar = "avatar.png";
            var mail = "test@example.com";
            var password = "Pass1@";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new AuthUser(pseudo, avatar, mail, password));
            Assert.Equal("Password must contains atleast a minus, a capital, a digit and a minimum of 8 characters", exception.Message);
        }

        [Fact]
        public void Constructor_WithNullPseudo_ShouldThrowArgumentException()
        {
            // Arrange
            string pseudo = null;
            var avatar = "avatar.png";
            var mail = "test@example.com";
            var password = "Password1@";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new AuthUser(pseudo, avatar, mail, password));
            Assert.Equal("Pseudo can't be empty", exception.Message);
        }

        #endregion

        #region ChangeMail Tests - Cas Limites et Erreurs

        [Fact]
        public void ChangeMail_WithNullMail_ShouldThrowArgumentException()
        {
            // Arrange
            var authUser = CreateValidAuthUser();
            string newMail = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => authUser.ChangeMail(newMail));
            Assert.Equal("Mail can't be empty", exception.Message);
        }

        [Fact]
        public void ChangeMail_WithEmptyMail_ShouldThrowArgumentException()
        {
            // Arrange
            var authUser = CreateValidAuthUser();
            var newMail = "";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => authUser.ChangeMail(newMail));
            Assert.Equal("Mail can't be empty", exception.Message);
        }

        [Fact]
        public void ChangeMail_WithWhitespaceMail_ShouldThrowArgumentException()
        {
            // Arrange
            var authUser = CreateValidAuthUser();
            var newMail = "   ";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => authUser.ChangeMail(newMail));
            Assert.Equal("Mail can't be empty", exception.Message);
        }

        [Fact]
        public void ChangeMail_WithInvalidMailFormat_ShouldThrowArgumentException()
        {
            // Arrange
            var authUser = CreateValidAuthUser();
            var newMail = "invalid-email";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => authUser.ChangeMail(newMail));
            Assert.Equal("L'adresse email n'est pas valide.", exception.Message);
        }

        #endregion

        #region ChangePassword Tests - Cas Limites et Erreurs

        [Fact]
        public void ChangePassword_WithNullPassword_ShouldThrowArgumentException()
        {
            // Arrange
            var authUser = CreateValidAuthUser();
            string newPassword = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => authUser.ChangePassword(newPassword));
            Assert.Equal("Password can't be empty", exception.Message);
        }

        [Fact]
        public void ChangePassword_WithEmptyPassword_ShouldThrowArgumentException()
        {
            // Arrange
            var authUser = CreateValidAuthUser();
            var newPassword = "";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => authUser.ChangePassword(newPassword));
            Assert.Equal("Password can't be empty", exception.Message);
        }

        [Fact]
        public void ChangePassword_WithWhitespacePassword_ShouldThrowArgumentException()
        {
            // Arrange
            var authUser = CreateValidAuthUser();
            var newPassword = "   ";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => authUser.ChangePassword(newPassword));
            Assert.Equal("Password can't be empty", exception.Message);
        }

        [Fact]
        public void ChangePassword_WithPasswordTooShort_ShouldThrowArgumentException()
        {
            // Arrange
            var authUser = CreateValidAuthUser();
            var newPassword = "Ab";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => authUser.ChangePassword(newPassword));
            Assert.Equal("Password must be between 3 and 50 character", exception.Message);
        }

        [Fact]
        public void ChangePassword_WithInvalidPassword_ShouldThrowArgumentException()
        {
            // Arrange
            var authUser = CreateValidAuthUser();
            var newPassword = "weakpassword";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => authUser.ChangePassword(newPassword));
            Assert.Equal("Password must contains atleast a minus, a capital, a digit and a minimum of 8 characters", exception.Message);
        }

        #endregion

        #region Inherited Tests - ChangePseudo Errors

        [Fact]
        public void ChangePseudo_WithNullPseudo_ShouldThrowArgumentException()
        {
            // Arrange
            var authUser = CreateValidAuthUser();
            string newPseudo = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => authUser.ChangePseudo(newPseudo));
            Assert.Equal("Pseudo can't be empty", exception.Message);
        }

        #endregion

        #region Helper Methods

        private AuthUser CreateValidAuthUser()
        {
            return new AuthUser("TestUser", "avatar.png", "test@example.com", "Password1@");
        }

        #endregion
    }
}
