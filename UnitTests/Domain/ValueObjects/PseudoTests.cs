using Domain.ValueObjects;

namespace UnitTests.Domain.ValueObjects
{
    public class PseudoTests
    {
        #region Create Tests - Cas Nominaux

        [Fact]
        public void Create_WithValidPseudo_ShouldCreatePseudo()
        {
            // Arrange
            var pseudoValue = "TestUser";

            // Act
            var pseudo = Pseudo.Create(pseudoValue);

            // Assert
            Assert.NotNull(pseudo);
            Assert.Equal(pseudoValue, pseudo.Value);
        }

        [Fact]
        public void Create_WithPseudoWithWhitespace_ShouldTrimPseudo()
        {
            // Arrange
            var pseudoValue = "  TestUser  ";

            // Act
            var pseudo = Pseudo.Create(pseudoValue);

            // Assert
            Assert.Equal("TestUser", pseudo.Value);
        }

        [Fact]
        public void Create_WithPseudoWithSpecialCharacters_ShouldCreatePseudo()
        {
            // Arrange
            var pseudoValue = "User_123!@#";

            // Act
            var pseudo = Pseudo.Create(pseudoValue);

            // Assert
            Assert.Equal(pseudoValue, pseudo.Value);
        }

        [Fact]
        public void Create_WithNumericPseudo_ShouldCreatePseudo()
        {
            // Arrange
            var pseudoValue = "12345";

            // Act
            var pseudo = Pseudo.Create(pseudoValue);

            // Assert
            Assert.Equal(pseudoValue, pseudo.Value);
        }

        [Fact]
        public void Create_WithSingleCharacterPseudo_ShouldCreatePseudo()
        {
            // Arrange
            var pseudoValue = "A";

            // Act
            var pseudo = Pseudo.Create(pseudoValue);

            // Assert
            Assert.Equal(pseudoValue, pseudo.Value);
        }

        #endregion

        #region ToString Tests

        [Fact]
        public void ToString_ShouldReturnValue()
        {
            // Arrange
            var pseudoValue = "TestUser";
            var pseudo = Pseudo.Create(pseudoValue);

            // Act
            var result = pseudo.ToString();

            // Assert
            Assert.Equal(pseudoValue, result);
        }

        #endregion

        #region Contains Tests

        [Fact]
        public void Contains_WithMatchingSubstring_ShouldReturnTrue()
        {
            // Arrange
            var pseudo = Pseudo.Create("TestUser");

            // Act
            var result = pseudo.Contains("Test");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WithMatchingSubstringDifferentCase_ShouldReturnTrue()
        {
            // Arrange
            var pseudo = Pseudo.Create("TestUser");

            // Act
            var result = pseudo.Contains("test");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WithNonMatchingSubstring_ShouldReturnFalse()
        {
            // Arrange
            var pseudo = Pseudo.Create("TestUser");

            // Act
            var result = pseudo.Contains("Admin");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Contains_WithEmptyString_ShouldReturnTrue()
        {
            // Arrange
            var pseudo = Pseudo.Create("TestUser");

            // Act
            var result = pseudo.Contains("");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WithFullMatch_ShouldReturnTrue()
        {
            // Arrange
            var pseudo = Pseudo.Create("TestUser");

            // Act
            var result = pseudo.Contains("TestUser");

            // Assert
            Assert.True(result);
        }

        #endregion

        #region Equality Tests

        [Fact]
        public void Equals_WithSamePseudoValue_ShouldReturnTrue()
        {
            // Arrange
            var pseudo1 = Pseudo.Create("TestUser");
            var pseudo2 = Pseudo.Create("TestUser");

            // Act
            var result = pseudo1.Equals(pseudo2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_WithSamePseudoValueDifferentCase_ShouldReturnTrue()
        {
            // Arrange
            var pseudo1 = Pseudo.Create("TestUser");
            var pseudo2 = Pseudo.Create("testuser");

            // Act
            var result = pseudo1.Equals(pseudo2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_WithDifferentPseudoValues_ShouldReturnFalse()
        {
            // Arrange
            var pseudo1 = Pseudo.Create("TestUser");
            var pseudo2 = Pseudo.Create("OtherUser");

            // Act
            var result = pseudo1.Equals(pseudo2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_WithNull_ShouldReturnFalse()
        {
            // Arrange
            var pseudo = Pseudo.Create("TestUser");

            // Act
            var result = pseudo.Equals(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void OperatorEquals_WithSamePseudoValue_ShouldReturnTrue()
        {
            // Arrange
            var pseudo1 = Pseudo.Create("TestUser");
            var pseudo2 = Pseudo.Create("TestUser");

            // Act
            var result = pseudo1 == pseudo2;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void OperatorNotEquals_WithDifferentPseudoValues_ShouldReturnTrue()
        {
            // Arrange
            var pseudo1 = Pseudo.Create("TestUser");
            var pseudo2 = Pseudo.Create("OtherUser");

            // Act
            var result = pseudo1 != pseudo2;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void GetHashCode_WithSamePseudoValue_ShouldReturnSameHashCode()
        {
            // Arrange
            var pseudo1 = Pseudo.Create("TestUser");
            var pseudo2 = Pseudo.Create("TestUser");

            // Act
            var hash1 = pseudo1.GetHashCode();
            var hash2 = pseudo2.GetHashCode();

            // Assert
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void GetHashCode_WithSamePseudoValueDifferentCase_ShouldReturnSameHashCode()
        {
            // Arrange
            var pseudo1 = Pseudo.Create("TestUser");
            var pseudo2 = Pseudo.Create("testuser");

            // Act
            var hash1 = pseudo1.GetHashCode();
            var hash2 = pseudo2.GetHashCode();

            // Assert
            Assert.Equal(hash1, hash2);
        }

        #endregion

        #region Implicit Operator Tests

        [Fact]
        public void ImplicitOperator_ShouldConvertToString()
        {
            // Arrange
            var pseudo = Pseudo.Create("TestUser");

            // Act
            string result = pseudo;

            // Assert
            Assert.Equal("TestUser", result);
        }

        #endregion

        #region Create Tests - Cas Limites et Erreurs

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("\t\t\t")]
        [InlineData("\n\n\n")]
        [InlineData("\r\n")]
        [InlineData(" \t \n ")]
        public void Create_WithNullOrEmptyOrWhitespace_ShouldThrowArgumentException(string pseudoValue)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Pseudo.Create(pseudoValue));
            Assert.Equal("Pseudo can't be empty", exception.Message);
        }

        [Fact]
        public void Create_WithLongPseudo_ShouldCreatePseudo()
        {
            // Arrange
            var pseudoValue = new string('A', 200);

            // Act
            var pseudo = Pseudo.Create(pseudoValue);

            // Assert
            Assert.Equal(pseudoValue, pseudo.Value);
        }

        [Fact]
        public void Create_WithUnicodeCharacters_ShouldCreatePseudo()
        {
            // Arrange
            var pseudoValue = "Utilisateur_éàü";

            // Act
            var pseudo = Pseudo.Create(pseudoValue);

            // Assert
            Assert.Equal(pseudoValue, pseudo.Value);
        }

        [Fact]
        public void Create_WithLeadingWhitespace_ShouldTrimLeft()
        {
            // Arrange
            var pseudoValue = "   User";

            // Act
            var pseudo = Pseudo.Create(pseudoValue);

            // Assert
            Assert.Equal("User", pseudo.Value);
        }

        [Fact]
        public void Create_WithTrailingWhitespace_ShouldTrimRight()
        {
            // Arrange
            var pseudoValue = "User   ";

            // Act
            var pseudo = Pseudo.Create(pseudoValue);

            // Assert
            Assert.Equal("User", pseudo.Value);
        }

        [Fact]
        public void Create_WithWhitespaceInMiddle_ShouldPreserveInternalSpaces()
        {
            // Arrange
            var pseudoValue = "User Name";

            // Act
            var pseudo = Pseudo.Create(pseudoValue);

            // Assert
            Assert.Equal("User Name", pseudo.Value);
        }

        [Theory]
        [InlineData("ValidUser")]
        [InlineData("User123")]
        [InlineData("a")]
        [InlineData("User_Name")]
        [InlineData("user-name")]
        [InlineData("123")]
        public void Create_WithVariousValidPseudos_ShouldCreatePseudo(string pseudoValue)
        {
            // Act
            var pseudo = Pseudo.Create(pseudoValue);

            // Assert
            Assert.Equal(pseudoValue, pseudo.Value);
        }

        #endregion
    }
}
