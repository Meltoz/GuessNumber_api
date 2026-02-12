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

        #region Create Tests - Cas Limites et Erreurs (Null/Empty)

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        [InlineData(" \t \n ")]
        public void Create_WithNullOrEmptyOrWhitespace_ShouldThrowMailCantBeEmpty(string mailValue)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Mail.Create(mailValue));
            Assert.Equal("Mail can't be empty", exception.Message);
        }

        #endregion

        #region Create Tests - Cas Limites et Erreurs (Format invalide)

        [Theory]
        [InlineData("invalid-email")]
        [InlineData("testexample.com")]
        [InlineData("test@")]
        [InlineData("@example.com")]
        [InlineData("test@example")]
        [InlineData("test user@example.com")]
        [InlineData("test@@example.com")]
        [InlineData("test@exam ple.com")]
        [InlineData("@")]
        [InlineData("test@.com")]
        [InlineData("test@com.")]
        public void Create_WithInvalidFormat_ShouldThrowMailNotValid(string mailValue)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Mail.Create(mailValue));
            Assert.Equal("L'adresse email n'est pas valide.", exception.Message);
        }

        #endregion

        #region Create Tests - Formats valides acceptés par la regex

        [Theory]
        [InlineData("a@b.c")]
        [InlineData("test+tag@example.com")]
        [InlineData("first.last@example.com")]
        [InlineData("user123@example123.com")]
        [InlineData("test-user@example-mail.com")]
        [InlineData("test_user@example.com")]
        [InlineData("test@mail.example.com")]
        [InlineData("TEST@EXAMPLE.COM")]
        [InlineData("user@sub.domain.example.com")]
        public void Create_WithVariousValidFormats_ShouldCreateMail(string mailValue)
        {
            // Act
            var mail = Mail.Create(mailValue);

            // Assert
            Assert.NotNull(mail);
            Assert.Equal(mailValue, mail.ToString());
        }

        [Fact]
        public void Create_WithMailStartingWithDot_ShouldCreateMailDueToSimpleRegex()
        {
            // Note: The current regex allows dots at the start of local part
            // This documents the current behavior (could be improved in production)
            var mail = Mail.Create(".test@example.com");
            Assert.NotNull(mail);
            Assert.Equal(".test@example.com", mail.ToString());
        }

        [Fact]
        public void Create_WithMailEndingWithDot_ShouldCreateMailDueToSimpleRegex()
        {
            // Note: The current regex allows dots at the end of local part
            var mail = Mail.Create("test.@example.com");
            Assert.NotNull(mail);
            Assert.Equal("test.@example.com", mail.ToString());
        }

        [Fact]
        public void Create_WithLongEmail_ShouldCreateMail()
        {
            // Arrange
            var localPart = new string('a', 50);
            var mailValue = $"{localPart}@example.com";

            // Act
            var mail = Mail.Create(mailValue);

            // Assert
            Assert.Equal(mailValue, mail.ToString());
        }

        #endregion
    }
}
