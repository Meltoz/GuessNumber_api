using Domain.ValueObjects;

namespace UnitTests.Domain.ValueObjects
{
    public class PasswordTests
    {
        #region Create Tests - Cas Nominaux

        [Fact]
        public void Create_WithValidPassword_ShouldCreatePassword()
        {
            // Arrange
            var passwordValue = "Password1@";

            // Act
            var password = Password.Create(passwordValue);

            // Assert
            Assert.NotNull(password);
            Assert.NotNull(password.ToString());
        }

        [Fact]
        public void Create_WithValidPassword_ShouldHashPassword()
        {
            // Arrange
            var passwordValue = "Password1@";

            // Act
            var password = Password.Create(passwordValue);

            // Assert
            // Le mot de passe doit être haché, donc différent du mot de passe original
            Assert.NotEqual(passwordValue, password.ToString());
        }

        [Fact]
        public void Create_WithMinimumRequirements_ShouldCreatePassword()
        {
            // Arrange
            var passwordValue = "Abcdef1@";

            // Act
            var password = Password.Create(passwordValue);

            // Assert
            Assert.NotNull(password);
        }

        [Fact]
        public void Create_WithAcceptedSpecialCharacters_ShouldCreatePassword()
        {
            // Arrange & Act & Assert - Test only characters that match the regex [@$!%*?&]
            var password1 = Password.Create("Password1@");
            Assert.NotNull(password1);

            var password2 = Password.Create("Password1$");
            Assert.NotNull(password2);

            var password3 = Password.Create("Password1!");
            Assert.NotNull(password3);

            var password4 = Password.Create("Password1%");
            Assert.NotNull(password4);

            var password5 = Password.Create("Password1*");
            Assert.NotNull(password5);

            var password6 = Password.Create("Password1&");
            Assert.NotNull(password6);
        }

        [Fact]
        public void Create_WithLongPassword_ShouldCreatePassword()
        {
            // Arrange
            var passwordValue = "VeryLongPassword123@WithManyCharacters";

            // Act
            var password = Password.Create(passwordValue);

            // Assert
            Assert.NotNull(password);
        }

        [Fact]
        public void Create_WithMultipleDigits_ShouldCreatePassword()
        {
            // Arrange
            var passwordValue = "Password123456@";

            // Act
            var password = Password.Create(passwordValue);

            // Assert
            Assert.NotNull(password);
        }

        [Fact]
        public void Create_WithMultipleSpecialChars_ShouldCreatePassword()
        {
            // Arrange - Use only accepted special chars [@$!%*&]
            var passwordValue = "Password1@!$%";

            // Act
            var password = Password.Create(passwordValue);

            // Assert
            Assert.NotNull(password);
        }

        #endregion

        #region FromPlainText Tests

        [Fact]
        public void FromPlainText_WithValidPassword_ShouldCreatePassword()
        {
            // Arrange
            var passwordValue = "PlainTextPassword123@";

            // Act
            var password = Password.FromPlainText(passwordValue);

            // Assert
            Assert.NotNull(password);
            Assert.NotNull(password.ToString());
        }

        [Fact]
        public void FromPlainText_WithSamePassword_ShouldProduceSameHash()
        {
            // Arrange
            var passwordValue = "Password1@";

            // Act
            var password1 = Password.FromPlainText(passwordValue);
            var password2 = Password.FromPlainText(passwordValue);

            // Assert
            Assert.Equal(password1.ToString(), password2.ToString());
        }

        [Fact]
        public void FromPlainText_WithDifferentPasswords_ShouldProduceDifferentHashes()
        {
            // Arrange
            var passwordValue1 = "Password1@";
            var passwordValue2 = "Password2@";

            // Act
            var password1 = Password.FromPlainText(passwordValue1);
            var password2 = Password.FromPlainText(passwordValue2);

            // Assert
            Assert.NotEqual(password1.ToString(), password2.ToString());
        }

        #endregion

        #region ToString Tests

        [Fact]
        public void ToString_ShouldReturnHashedValue()
        {
            // Arrange
            var passwordValue = "Password1@";
            var password = Password.Create(passwordValue);

            // Act
            var result = password.ToString();

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(passwordValue, result);
            // Hash SHA512 en hexadécimal = 128 caractères
            Assert.Equal(128, result.Length);
        }

        #endregion

        #region Equality Tests

        [Fact]
        public void Equals_WithSamePasswordValue_ShouldReturnTrue()
        {
            // Arrange
            var password1 = Password.Create("Password1@");
            var password2 = Password.Create("Password1@");

            // Act
            var result = password1.Equals(password2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_WithDifferentPasswordValues_ShouldReturnFalse()
        {
            // Arrange
            var password1 = Password.Create("Password1@");
            var password2 = Password.Create("Password2@");

            // Act
            var result = password1.Equals(password2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_WithNull_ShouldReturnFalse()
        {
            // Arrange
            var password = Password.Create("Password1@");

            // Act
            var result = password.Equals(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void OperatorEquals_WithSamePasswordValue_ShouldReturnTrue()
        {
            // Arrange
            var password1 = Password.Create("Password1@");
            var password2 = Password.Create("Password1@");

            // Act
            var result = password1 == password2;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void OperatorNotEquals_WithDifferentPasswordValues_ShouldReturnTrue()
        {
            // Arrange
            var password1 = Password.Create("Password1@");
            var password2 = Password.Create("Password2@");

            // Act
            var result = password1 != password2;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void GetHashCode_WithSamePasswordValue_ShouldReturnSameHashCode()
        {
            // Arrange
            var password1 = Password.Create("Password1@");
            var password2 = Password.Create("Password1@");

            // Act
            var hash1 = password1.GetHashCode();
            var hash2 = password2.GetHashCode();

            // Assert
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void GetHashCode_WithDifferentPasswordValues_ShouldReturnDifferentHashCodes()
        {
            // Arrange
            var password1 = Password.Create("Password1@");
            var password2 = Password.Create("Password2@");

            // Act
            var hash1 = password1.GetHashCode();
            var hash2 = password2.GetHashCode();

            // Assert
            Assert.NotEqual(hash1, hash2);
        }

        #endregion

        #region Create Tests - Cas Limites et Erreurs

        [Fact]
        public void Create_WithNullPassword_ShouldThrowArgumentException()
        {
            // Arrange
            string passwordValue = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Password.Create(passwordValue));
            Assert.Equal("Password can't be empty", exception.Message);
        }

        [Fact]
        public void Create_WithEmptyPassword_ShouldThrowArgumentException()
        {
            // Arrange
            var passwordValue = "";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Password.Create(passwordValue));
            Assert.Equal("Password can't be empty", exception.Message);
        }

        [Fact]
        public void Create_WithWhitespacePasswordOnly_ShouldThrowArgumentException()
        {
            // Arrange
            var passwordValue = "   ";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Password.Create(passwordValue));
            Assert.Equal("Password can't be empty", exception.Message);
        }

        [Fact]
        public void Create_WithPasswordTooShort_ShouldThrowArgumentException()
        {
            // Arrange
            var passwordValue = "Ab";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Password.Create(passwordValue));
            Assert.Equal("Password must be between 3 and 50 character", exception.Message);
        }

        [Fact]
        public void Create_WithPasswordMissingDigit_ShouldThrowArgumentException()
        {
            // Arrange
            var passwordValue = "Password@";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Password.Create(passwordValue));
            Assert.Equal("Password must contains atleast a minus, a capital, a digit and a minimum of 8 characters", exception.Message);
        }

        [Fact]
        public void Create_WithPasswordMissingSpecialChar_ShouldThrowArgumentException()
        {
            // Arrange
            var passwordValue = "Password1";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Password.Create(passwordValue));
            Assert.Equal("Password must contains atleast a minus, a capital, a digit and a minimum of 8 characters", exception.Message);
        }

        [Fact]
        public void Create_WithPasswordLessThan8Characters_ShouldThrowArgumentException()
        {
            // Arrange
            var passwordValue = "Pass1@";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Password.Create(passwordValue));
            Assert.Equal("Password must contains atleast a minus, a capital, a digit and a minimum of 8 characters", exception.Message);
        }

        [Fact]
        public void Create_WithPasswordOnlyLowercase_ShouldThrowArgumentException()
        {
            // Arrange
            var passwordValue = "abcdefgh";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Password.Create(passwordValue));
            Assert.Equal("Password must contains atleast a minus, a capital, a digit and a minimum of 8 characters", exception.Message);
        }

        [Fact]
        public void Create_WithPasswordOnlyUppercase_ShouldThrowArgumentException()
        {
            // Arrange
            var passwordValue = "ABCDEFGH";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Password.Create(passwordValue));
            Assert.Equal("Password must contains atleast a minus, a capital, a digit and a minimum of 8 characters", exception.Message);
        }

        [Fact]
        public void Create_WithPasswordOnlyDigits_ShouldThrowArgumentException()
        {
            // Arrange
            var passwordValue = "12345678";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Password.Create(passwordValue));
            Assert.Equal("Password must contains atleast a minus, a capital, a digit and a minimum of 8 characters", exception.Message);
        }

        [Fact]
        public void Create_WithPasswordOnlySpecialChars_ShouldThrowArgumentException()
        {
            // Arrange
            var passwordValue = "@!#$%&*?";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Password.Create(passwordValue));
            Assert.Equal("Password must contains atleast a minus, a capital, a digit and a minimum of 8 characters", exception.Message);
        }

        [Fact]
        public void Create_WithPasswordWithInvalidSpecialChar_ShouldThrowArgumentException()
        {
            // Arrange
            var passwordValue = "Password1~";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Password.Create(passwordValue));
            Assert.Equal("Password must contains atleast a minus, a capital, a digit and a minimum of 8 characters", exception.Message);
        }

        #endregion

        #region FromPlainText Tests - Cas Limites et Erreurs

        [Fact]
        public void FromPlainText_WithNullPassword_ShouldThrowArgumentException()
        {
            // Arrange
            string passwordValue = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Password.FromPlainText(passwordValue));
            Assert.Equal("Password can't be empty", exception.Message);
        }

        [Fact]
        public void FromPlainText_WithEmptyPassword_ShouldThrowArgumentException()
        {
            // Arrange
            var passwordValue = "";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Password.FromPlainText(passwordValue));
            Assert.Equal("Password can't be empty", exception.Message);
        }

        [Fact]
        public void FromPlainText_WithWhitespacePasswordOnly_ShouldThrowArgumentException()
        {
            // Arrange
            var passwordValue = "   ";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Password.FromPlainText(passwordValue));
            Assert.Equal("Password can't be empty", exception.Message);
        }

        #endregion
    }
}
