using Domain.ValueObjects;

namespace UnitTests.Domain.ValueObjects
{
    public class AuthorTests
    {
        #region Create Tests - Cas Nominaux

        [Fact]
        public void Create_WithValidAuthor_ShouldCreateAuthor()
        {
            // Arrange
            var authorValue = "John Doe";

            // Act
            var author = Author.Create(authorValue);

            // Assert
            Assert.NotNull(author);
            Assert.Equal(authorValue, author.Value);
        }

        [Fact]
        public void Create_WithMinimumLength_ShouldCreateAuthor()
        {
            // Arrange
            var authorValue = "ABC";

            // Act
            var author = Author.Create(authorValue);

            // Assert
            Assert.NotNull(author);
            Assert.Equal(authorValue, author.Value);
        }

        [Fact]
        public void Create_WithMaximumLength_ShouldCreateAuthor()
        {
            // Arrange
            var authorValue = new string('A', 100);

            // Act
            var author = Author.Create(authorValue);

            // Assert
            Assert.NotNull(author);
            Assert.Equal(100, author.Value.Length);
        }

        [Fact]
        public void Create_WithAuthorWithNumbers_ShouldCreateAuthor()
        {
            // Arrange
            var authorValue = "Author 123";

            // Act
            var author = Author.Create(authorValue);

            // Assert
            Assert.Equal(authorValue, author.Value);
        }

        [Fact]
        public void Create_WithAuthorWithSpecialCharacters_ShouldCreateAuthor()
        {
            // Arrange
            var authorValue = "Jean-Paul O'Connor";

            // Act
            var author = Author.Create(authorValue);

            // Assert
            Assert.Equal(authorValue, author.Value);
        }

        [Fact]
        public void Create_WithAuthorWithAccents_ShouldCreateAuthor()
        {
            // Arrange
            var authorValue = "François Édouard";

            // Act
            var author = Author.Create(authorValue);

            // Assert
            Assert.Equal(authorValue, author.Value);
        }

        [Fact]
        public void Create_WithLongName_ShouldCreateAuthor()
        {
            // Arrange
            var authorValue = "Dr. Alexander Montgomery Wellington III";

            // Act
            var author = Author.Create(authorValue);

            // Assert
            Assert.Equal(authorValue, author.Value);
        }

        #endregion

        #region ToString Tests

        [Fact]
        public void ToString_ShouldReturnValue()
        {
            // Arrange
            var authorValue = "John Doe";
            var author = Author.Create(authorValue);

            // Act
            var result = author.ToString();

            // Assert
            Assert.Equal(authorValue, result);
        }

        #endregion

        #region Contains Tests

        [Fact]
        public void Contains_WithMatchingSubstring_ShouldReturnTrue()
        {
            // Arrange
            var author = Author.Create("John Doe");

            // Act
            var result = author.Contains("John");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WithMatchingSubstringDifferentCase_ShouldReturnTrue()
        {
            // Arrange
            var author = Author.Create("John Doe");

            // Act
            var result = author.Contains("JOHN");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WithNonMatchingSubstring_ShouldReturnFalse()
        {
            // Arrange
            var author = Author.Create("John Doe");

            // Act
            var result = author.Contains("Smith");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Contains_WithEmptyString_ShouldReturnTrue()
        {
            // Arrange
            var author = Author.Create("John Doe");

            // Act
            var result = author.Contains("");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WithFullMatch_ShouldReturnTrue()
        {
            // Arrange
            var author = Author.Create("John Doe");

            // Act
            var result = author.Contains("John Doe");

            // Assert
            Assert.True(result);
        }

        #endregion

        #region Equality Tests

        [Fact]
        public void Equals_WithSameAuthorValue_ShouldReturnTrue()
        {
            // Arrange
            var author1 = Author.Create("John Doe");
            var author2 = Author.Create("John Doe");

            // Act
            var result = author1.Equals(author2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_WithSameAuthorValueDifferentCase_ShouldReturnTrue()
        {
            // Arrange
            var author1 = Author.Create("John Doe");
            var author2 = Author.Create("john doe");

            // Act
            var result = author1.Equals(author2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_WithDifferentAuthorValues_ShouldReturnFalse()
        {
            // Arrange
            var author1 = Author.Create("John Doe");
            var author2 = Author.Create("Jane Smith");

            // Act
            var result = author1.Equals(author2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_WithNull_ShouldReturnFalse()
        {
            // Arrange
            var author = Author.Create("John Doe");

            // Act
            var result = author.Equals(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void OperatorEquals_WithSameAuthorValue_ShouldReturnTrue()
        {
            // Arrange
            var author1 = Author.Create("John Doe");
            var author2 = Author.Create("John Doe");

            // Act
            var result = author1 == author2;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void OperatorNotEquals_WithDifferentAuthorValues_ShouldReturnTrue()
        {
            // Arrange
            var author1 = Author.Create("John Doe");
            var author2 = Author.Create("Jane Smith");

            // Act
            var result = author1 != author2;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void GetHashCode_WithSameAuthorValue_ShouldReturnSameHashCode()
        {
            // Arrange
            var author1 = Author.Create("John Doe");
            var author2 = Author.Create("John Doe");

            // Act
            var hash1 = author1.GetHashCode();
            var hash2 = author2.GetHashCode();

            // Assert
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void GetHashCode_WithSameAuthorValueDifferentCase_ShouldReturnSameHashCode()
        {
            // Arrange
            var author1 = Author.Create("John Doe");
            var author2 = Author.Create("john doe");

            // Act
            var hash1 = author1.GetHashCode();
            var hash2 = author2.GetHashCode();

            // Assert
            Assert.Equal(hash1, hash2);
        }

        #endregion

        #region Implicit Operator Tests

        [Fact]
        public void ImplicitOperator_ShouldConvertToString()
        {
            // Arrange
            var author = Author.Create("John Doe");

            // Act
            string result = author;

            // Assert
            Assert.Equal("John Doe", result);
        }

        #endregion

        #region Create Tests - Cas Limites et Erreurs

        [Fact]
        public void Create_WithNullAuthor_ShouldThrowArgumentException()
        {
            // Arrange
            string authorValue = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Author.Create(authorValue));
            Assert.Equal("Pseudo can't be empty", exception.Message);
        }

        [Fact]
        public void Create_WithEmptyAuthor_ShouldThrowArgumentException()
        {
            // Arrange
            var authorValue = "";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Author.Create(authorValue));
            Assert.Equal("Pseudo can't be empty", exception.Message);
        }

        [Fact]
        public void Create_WithWhitespaceAuthorOnly_ShouldThrowArgumentException()
        {
            // Arrange
            var authorValue = "   ";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Author.Create(authorValue));
            Assert.Equal("Pseudo can't be empty", exception.Message);
        }

        [Fact]
        public void Create_WithAuthorTooShort_ShouldThrowArgumentException()
        {
            // Arrange
            var authorValue = "AB";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Author.Create(authorValue));
            Assert.Equal("Must be between 3 and 100 character", exception.Message);
        }

        [Fact]
        public void Create_WithAuthorTooLong_ShouldThrowArgumentException()
        {
            // Arrange
            var authorValue = new string('A', 101);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Author.Create(authorValue));
            Assert.Equal("Must be between 3 and 100 character", exception.Message);
        }

        [Fact]
        public void Create_WithSingleCharacter_ShouldThrowArgumentException()
        {
            // Arrange
            var authorValue = "A";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Author.Create(authorValue));
            Assert.Equal("Must be between 3 and 100 character", exception.Message);
        }

        [Fact]
        public void Create_WithTwoCharacters_ShouldThrowArgumentException()
        {
            // Arrange
            var authorValue = "AB";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Author.Create(authorValue));
            Assert.Equal("Must be between 3 and 100 character", exception.Message);
        }

        [Fact]
        public void Create_WithTabsAuthorOnly_ShouldThrowArgumentException()
        {
            // Arrange
            var authorValue = "\t\t\t";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Author.Create(authorValue));
            Assert.Equal("Pseudo can't be empty", exception.Message);
        }

        [Fact]
        public void Create_WithNewlinesAuthorOnly_ShouldThrowArgumentException()
        {
            // Arrange
            var authorValue = "\n\n\n";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Author.Create(authorValue));
            Assert.Equal("Pseudo can't be empty", exception.Message);
        }

        #endregion
    }
}
