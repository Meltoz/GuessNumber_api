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

        [Fact]
        public void Create_WithNullPseudo_ShouldThrowArgumentException()
        {
            // Arrange
            string pseudoValue = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Pseudo.Create(pseudoValue));
            Assert.Equal("Pseudo can't be empty", exception.Message);
        }

        [Fact]
        public void Create_WithEmptyPseudo_ShouldThrowArgumentException()
        {
            // Arrange
            var pseudoValue = "";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Pseudo.Create(pseudoValue));
            Assert.Equal("Pseudo can't be empty", exception.Message);
        }

        [Fact]
        public void Create_WithWhitespacePseudoOnly_ShouldThrowArgumentException()
        {
            // Arrange
            var pseudoValue = "   ";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Pseudo.Create(pseudoValue));
            Assert.Equal("Pseudo can't be empty", exception.Message);
        }

        [Fact]
        public void Create_WithTabsPseudoOnly_ShouldThrowArgumentException()
        {
            // Arrange
            var pseudoValue = "\t\t\t";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Pseudo.Create(pseudoValue));
            Assert.Equal("Pseudo can't be empty", exception.Message);
        }

        [Fact]
        public void Create_WithNewlinesPseudoOnly_ShouldThrowArgumentException()
        {
            // Arrange
            var pseudoValue = "\n\n\n";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Pseudo.Create(pseudoValue));
            Assert.Equal("Pseudo can't be empty", exception.Message);
        }

        #endregion
    }
}
