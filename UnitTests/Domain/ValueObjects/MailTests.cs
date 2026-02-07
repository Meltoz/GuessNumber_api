using Domain.ValueObjects;

namespace UnitTests.Domain.ValueObjects
{
    public class MailTests
    {
        #region Create Tests - Cas Nominaux

        [Fact]
        public void Create_WithValidMail_ShouldCreateMail()
        {
            // Arrange
            var mailValue = "test@example.com";

            // Act
            var mail = Mail.Create(mailValue);

            // Assert
            Assert.NotNull(mail);
            Assert.Equal(mailValue, mail.ToString());
        }

        [Fact]
        public void Create_WithMailWithWhitespace_ShouldTrimMail()
        {
            // Arrange
            var mailValue = "  test@example.com  ";

            // Act
            var mail = Mail.Create(mailValue);

            // Assert
            Assert.Equal("test@example.com", mail.ToString());
        }

        [Fact]
        public void Create_WithMailWithSubdomain_ShouldCreateMail()
        {
            // Arrange
            var mailValue = "test@mail.example.com";

            // Act
            var mail = Mail.Create(mailValue);

            // Assert
            Assert.Equal(mailValue, mail.ToString());
        }

        [Fact]
        public void Create_WithMailWithPlusSign_ShouldCreateMail()
        {
            // Arrange
            var mailValue = "test+tag@example.com";

            // Act
            var mail = Mail.Create(mailValue);

            // Assert
            Assert.Equal(mailValue, mail.ToString());
        }

        [Fact]
        public void Create_WithMailWithDots_ShouldCreateMail()
        {
            // Arrange
            var mailValue = "first.last@example.com";

            // Act
            var mail = Mail.Create(mailValue);

            // Assert
            Assert.Equal(mailValue, mail.ToString());
        }

        [Fact]
        public void Create_WithMailWithNumbers_ShouldCreateMail()
        {
            // Arrange
            var mailValue = "user123@example123.com";

            // Act
            var mail = Mail.Create(mailValue);

            // Assert
            Assert.Equal(mailValue, mail.ToString());
        }

        [Fact]
        public void Create_WithMailWithHyphen_ShouldCreateMail()
        {
            // Arrange
            var mailValue = "test-user@example-mail.com";

            // Act
            var mail = Mail.Create(mailValue);

            // Assert
            Assert.Equal(mailValue, mail.ToString());
        }

        [Fact]
        public void Create_WithMailWithUnderscore_ShouldCreateMail()
        {
            // Arrange
            var mailValue = "test_user@example.com";

            // Act
            var mail = Mail.Create(mailValue);

            // Assert
            Assert.Equal(mailValue, mail.ToString());
        }

        #endregion

        #region ToString Tests

        [Fact]
        public void ToString_ShouldReturnMailValue()
        {
            // Arrange
            var mailValue = "test@example.com";
            var mail = Mail.Create(mailValue);

            // Act
            var result = mail.ToString();

            // Assert
            Assert.Equal(mailValue, result);
        }

        #endregion

        #region Contains Tests

        [Fact]
        public void Contains_WithMatchingSubstring_ShouldReturnTrue()
        {
            // Arrange
            var mail = Mail.Create("test@example.com");

            // Act
            var result = mail.Contains("test");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WithMatchingSubstringDifferentCase_ShouldReturnTrue()
        {
            // Arrange
            var mail = Mail.Create("test@example.com");

            // Act
            var result = mail.Contains("TEST");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WithMatchingDomain_ShouldReturnTrue()
        {
            // Arrange
            var mail = Mail.Create("test@example.com");

            // Act
            var result = mail.Contains("example");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WithNonMatchingSubstring_ShouldReturnFalse()
        {
            // Arrange
            var mail = Mail.Create("test@example.com");

            // Act
            var result = mail.Contains("admin");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Contains_WithEmptyString_ShouldReturnTrue()
        {
            // Arrange
            var mail = Mail.Create("test@example.com");

            // Act
            var result = mail.Contains("");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WithFullMatch_ShouldReturnTrue()
        {
            // Arrange
            var mail = Mail.Create("test@example.com");

            // Act
            var result = mail.Contains("test@example.com");

            // Assert
            Assert.True(result);
        }

        #endregion

        #region Equality Tests

        [Fact]
        public void Equals_WithSameMailValue_ShouldReturnTrue()
        {
            // Arrange
            var mail1 = Mail.Create("test@example.com");
            var mail2 = Mail.Create("test@example.com");

            // Act
            var result = mail1.Equals(mail2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_WithSameMailValueDifferentCase_ShouldReturnTrue()
        {
            // Arrange
            var mail1 = Mail.Create("test@example.com");
            var mail2 = Mail.Create("TEST@EXAMPLE.COM");

            // Act
            var result = mail1.Equals(mail2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_WithDifferentMailValues_ShouldReturnFalse()
        {
            // Arrange
            var mail1 = Mail.Create("test@example.com");
            var mail2 = Mail.Create("other@example.com");

            // Act
            var result = mail1.Equals(mail2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_WithNull_ShouldReturnFalse()
        {
            // Arrange
            var mail = Mail.Create("test@example.com");

            // Act
            var result = mail.Equals(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void OperatorEquals_WithSameMailValue_ShouldReturnTrue()
        {
            // Arrange
            var mail1 = Mail.Create("test@example.com");
            var mail2 = Mail.Create("test@example.com");

            // Act
            var result = mail1 == mail2;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void OperatorNotEquals_WithDifferentMailValues_ShouldReturnTrue()
        {
            // Arrange
            var mail1 = Mail.Create("test@example.com");
            var mail2 = Mail.Create("other@example.com");

            // Act
            var result = mail1 != mail2;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void GetHashCode_WithSameMailValue_ShouldReturnSameHashCode()
        {
            // Arrange
            var mail1 = Mail.Create("test@example.com");
            var mail2 = Mail.Create("test@example.com");

            // Act
            var hash1 = mail1.GetHashCode();
            var hash2 = mail2.GetHashCode();

            // Assert
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void GetHashCode_WithSameMailValueDifferentCase_ShouldReturnSameHashCode()
        {
            // Arrange
            var mail1 = Mail.Create("test@example.com");
            var mail2 = Mail.Create("TEST@EXAMPLE.COM");

            // Act
            var hash1 = mail1.GetHashCode();
            var hash2 = mail2.GetHashCode();

            // Assert
            Assert.Equal(hash1, hash2);
        }

        #endregion

        #region Implicit Operator Tests

        [Fact]
        public void ImplicitOperator_ShouldConvertToString()
        {
            // Arrange
            var mail = Mail.Create("test@example.com");

            // Act
            string result = mail;

            // Assert
            Assert.Equal("test@example.com", result);
        }

        #endregion

        #region Create Tests - Cas Limites et Erreurs

        [Fact]
        public void Create_WithNullMail_ShouldThrowArgumentException()
        {
            // Arrange
            string mailValue = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Mail.Create(mailValue));
            Assert.Equal("Mail can't be empty", exception.Message);
        }

        [Fact]
        public void Create_WithEmptyMail_ShouldThrowArgumentException()
        {
            // Arrange
            var mailValue = "";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Mail.Create(mailValue));
            Assert.Equal("Mail can't be empty", exception.Message);
        }

        [Fact]
        public void Create_WithWhitespaceMailOnly_ShouldThrowArgumentException()
        {
            // Arrange
            var mailValue = "   ";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Mail.Create(mailValue));
            Assert.Equal("Mail can't be empty", exception.Message);
        }

        [Fact]
        public void Create_WithInvalidMailFormat_ShouldThrowArgumentException()
        {
            // Arrange
            var mailValue = "invalid-email";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Mail.Create(mailValue));
            Assert.Equal("L'adresse email n'est pas valide.", exception.Message);
        }

        [Fact]
        public void Create_WithMailMissingAtSymbol_ShouldThrowArgumentException()
        {
            // Arrange
            var mailValue = "testexample.com";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Mail.Create(mailValue));
            Assert.Equal("L'adresse email n'est pas valide.", exception.Message);
        }

        [Fact]
        public void Create_WithMailMissingDomain_ShouldThrowArgumentException()
        {
            // Arrange
            var mailValue = "test@";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Mail.Create(mailValue));
            Assert.Equal("L'adresse email n'est pas valide.", exception.Message);
        }

        [Fact]
        public void Create_WithMailMissingUsername_ShouldThrowArgumentException()
        {
            // Arrange
            var mailValue = "@example.com";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Mail.Create(mailValue));
            Assert.Equal("L'adresse email n'est pas valide.", exception.Message);
        }

        [Fact]
        public void Create_WithMailMissingTopLevelDomain_ShouldThrowArgumentException()
        {
            // Arrange
            var mailValue = "test@example";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Mail.Create(mailValue));
            Assert.Equal("L'adresse email n'est pas valide.", exception.Message);
        }

        [Fact]
        public void Create_WithMailWithSpaces_ShouldThrowArgumentException()
        {
            // Arrange
            var mailValue = "test user@example.com";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Mail.Create(mailValue));
            Assert.Equal("L'adresse email n'est pas valide.", exception.Message);
        }

        [Fact]
        public void Create_WithMailWithMultipleAtSymbols_ShouldThrowArgumentException()
        {
            // Arrange
            var mailValue = "test@@example.com";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Mail.Create(mailValue));
            Assert.Equal("L'adresse email n'est pas valide.", exception.Message);
        }

        [Fact]
        public void Create_WithMailStartingWithDot_ShouldCreateMailDueToSimpleRegex()
        {
            // Note: The current regex allows dots at the start of local part
            // This documents the current behavior (could be improved in production)
            // Arrange
            var mailValue = ".test@example.com";

            // Act
            var mail = Mail.Create(mailValue);

            // Assert
            Assert.NotNull(mail);
            Assert.Equal(mailValue, mail.ToString());
        }

        [Fact]
        public void Create_WithMailEndingWithDot_ShouldCreateMailDueToSimpleRegex()
        {
            // Note: The current regex allows dots at the end of local part
            // This documents the current behavior (could be improved in production)
            // Arrange
            var mailValue = "test.@example.com";

            // Act
            var mail = Mail.Create(mailValue);

            // Assert
            Assert.NotNull(mail);
            Assert.Equal(mailValue, mail.ToString());
        }

        #endregion
    }
}
