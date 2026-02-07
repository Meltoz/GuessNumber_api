using Domain.ValueObjects;

namespace UnitTests.Domain.ValueObjects
{
    public class LibelleTests
    {
        #region Create Tests - Cas Nominaux

        [Fact]
        public void Create_WithValidLibelle_ShouldCreateLibelle()
        {
            // Arrange
            var libelleValue = "Question title";

            // Act
            var libelle = Libelle.Create(libelleValue);

            // Assert
            Assert.NotNull(libelle);
            Assert.Equal(libelleValue, libelle.Value);
        }

        [Fact]
        public void Create_WithNullLibelle_ShouldCreateLibelleWithNull()
        {
            // Arrange
            string libelleValue = null;

            // Act
            var libelle = Libelle.Create(libelleValue);

            // Assert
            Assert.NotNull(libelle);
            Assert.Null(libelle.Value);
        }

        [Fact]
        public void Create_WithEmptyLibelle_ShouldCreateLibelleWithEmpty()
        {
            // Arrange
            var libelleValue = "";

            // Act
            var libelle = Libelle.Create(libelleValue);

            // Assert
            Assert.NotNull(libelle);
            Assert.Equal(string.Empty, libelle.Value);
        }

        [Fact]
        public void Create_WithWhitespaceLibelle_ShouldCreateLibelle()
        {
            // Arrange
            var libelleValue = "   ";

            // Act
            var libelle = Libelle.Create(libelleValue);

            // Assert
            Assert.NotNull(libelle);
            Assert.Equal(libelleValue, libelle.Value);
        }

        [Fact]
        public void Create_WithShortLibelle_ShouldCreateLibelle()
        {
            // Arrange
            var libelleValue = "A";

            // Act
            var libelle = Libelle.Create(libelleValue);

            // Assert
            Assert.Equal(libelleValue, libelle.Value);
        }

        [Fact]
        public void Create_WithLongLibelle_ShouldCreateLibelle()
        {
            // Arrange
            var libelleValue = new string('A', 500);

            // Act
            var libelle = Libelle.Create(libelleValue);

            // Assert
            Assert.Equal(500, libelle.Value.Length);
        }

        [Fact]
        public void Create_WithLibelleWithNumbers_ShouldCreateLibelle()
        {
            // Arrange
            var libelleValue = "Question 123";

            // Act
            var libelle = Libelle.Create(libelleValue);

            // Assert
            Assert.Equal(libelleValue, libelle.Value);
        }

        [Fact]
        public void Create_WithLibelleWithSpecialCharacters_ShouldCreateLibelle()
        {
            // Arrange
            var libelleValue = "Question: What's the answer?";

            // Act
            var libelle = Libelle.Create(libelleValue);

            // Assert
            Assert.Equal(libelleValue, libelle.Value);
        }

        [Fact]
        public void Create_WithLibelleWithAccents_ShouldCreateLibelle()
        {
            // Arrange
            var libelleValue = "Quelle est la réponse à cette question?";

            // Act
            var libelle = Libelle.Create(libelleValue);

            // Assert
            Assert.Equal(libelleValue, libelle.Value);
        }

        [Fact]
        public void Create_WithMultilineLibelle_ShouldCreateLibelle()
        {
            // Arrange
            var libelleValue = "Line 1\nLine 2\nLine 3";

            // Act
            var libelle = Libelle.Create(libelleValue);

            // Assert
            Assert.Equal(libelleValue, libelle.Value);
        }

        [Fact]
        public void Create_WithLibelleWithTabs_ShouldCreateLibelle()
        {
            // Arrange
            var libelleValue = "Column1\tColumn2\tColumn3";

            // Act
            var libelle = Libelle.Create(libelleValue);

            // Assert
            Assert.Equal(libelleValue, libelle.Value);
        }

        [Fact]
        public void Create_WithUnicodeCharacters_ShouldCreateLibelle()
        {
            // Arrange
            var libelleValue = "Question with emoji 😀 and symbols ∑∏∫";

            // Act
            var libelle = Libelle.Create(libelleValue);

            // Assert
            Assert.Equal(libelleValue, libelle.Value);
        }

        #endregion

        #region ToString Tests

        [Fact]
        public void ToString_ShouldReturnValue()
        {
            // Arrange
            var libelleValue = "Question title";
            var libelle = Libelle.Create(libelleValue);

            // Act
            var result = libelle.ToString();

            // Assert
            Assert.Equal(libelleValue, result);
        }

        [Fact]
        public void ToString_WithNullValue_ShouldReturnNull()
        {
            // Arrange
            var libelle = Libelle.Create(null);

            // Act
            var result = libelle.ToString();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void ToString_WithEmptyValue_ShouldReturnEmpty()
        {
            // Arrange
            var libelle = Libelle.Create("");

            // Act
            var result = libelle.ToString();

            // Assert
            Assert.Equal(string.Empty, result);
        }

        #endregion

        #region Contains Tests

        [Fact]
        public void Contains_WithMatchingSubstring_ShouldReturnTrue()
        {
            // Arrange
            var libelle = Libelle.Create("What is the capital of France");

            // Act
            var result = libelle.Contains("capital");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WithMatchingSubstringDifferentCase_ShouldReturnTrue()
        {
            // Arrange
            var libelle = Libelle.Create("What is the capital of France");

            // Act
            var result = libelle.Contains("CAPITAL");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WithNonMatchingSubstring_ShouldReturnFalse()
        {
            // Arrange
            var libelle = Libelle.Create("What is the capital of France");

            // Act
            var result = libelle.Contains("Germany");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Contains_WithEmptyString_ShouldReturnTrue()
        {
            // Arrange
            var libelle = Libelle.Create("Question");

            // Act
            var result = libelle.Contains("");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WithFullMatch_ShouldReturnTrue()
        {
            // Arrange
            var libelle = Libelle.Create("Question");

            // Act
            var result = libelle.Contains("Question");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Contains_WithPartialWord_ShouldReturnTrue()
        {
            // Arrange
            var libelle = Libelle.Create("Question");

            // Act
            var result = libelle.Contains("Quest");

            // Assert
            Assert.True(result);
        }

        #endregion

        #region Equality Tests

        [Fact]
        public void Equals_WithSameLibelleValue_ShouldReturnTrue()
        {
            // Arrange
            var libelle1 = Libelle.Create("Question");
            var libelle2 = Libelle.Create("Question");

            // Act
            var result = libelle1.Equals(libelle2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_WithSameLibelleValueDifferentCase_ShouldReturnTrue()
        {
            // Arrange
            var libelle1 = Libelle.Create("Question");
            var libelle2 = Libelle.Create("question");

            // Act
            var result = libelle1.Equals(libelle2);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void Equals_WithDifferentLibelleValues_ShouldReturnFalse()
        {
            // Arrange
            var libelle1 = Libelle.Create("Question 1");
            var libelle2 = Libelle.Create("Question 2");

            // Act
            var result = libelle1.Equals(libelle2);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Equals_WithNull_ShouldReturnFalse()
        {
            // Arrange
            var libelle = Libelle.Create("Question");

            // Act
            var result = libelle.Equals(null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void OperatorEquals_WithSameLibelleValue_ShouldReturnTrue()
        {
            // Arrange
            var libelle1 = Libelle.Create("Question");
            var libelle2 = Libelle.Create("Question");

            // Act
            var result = libelle1 == libelle2;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void OperatorNotEquals_WithDifferentLibelleValues_ShouldReturnTrue()
        {
            // Arrange
            var libelle1 = Libelle.Create("Question 1");
            var libelle2 = Libelle.Create("Question 2");

            // Act
            var result = libelle1 != libelle2;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void GetHashCode_WithSameLibelleValue_ShouldReturnSameHashCode()
        {
            // Arrange
            var libelle1 = Libelle.Create("Question");
            var libelle2 = Libelle.Create("Question");

            // Act
            var hash1 = libelle1.GetHashCode();
            var hash2 = libelle2.GetHashCode();

            // Assert
            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void GetHashCode_WithSameLibelleValueDifferentCase_ShouldReturnSameHashCode()
        {
            // Arrange
            var libelle1 = Libelle.Create("Question");
            var libelle2 = Libelle.Create("question");

            // Act
            var hash1 = libelle1.GetHashCode();
            var hash2 = libelle2.GetHashCode();

            // Assert
            Assert.Equal(hash1, hash2);
        }

        #endregion

        #region Implicit Operator Tests

        [Fact]
        public void ImplicitOperator_ShouldConvertToString()
        {
            // Arrange
            var libelle = Libelle.Create("Question");

            // Act
            string result = libelle;

            // Assert
            Assert.Equal("Question", result);
        }

        [Fact]
        public void ImplicitOperator_WithNullValue_ShouldReturnNull()
        {
            // Arrange
            var libelle = Libelle.Create(null);

            // Act
            string result = libelle;

            // Assert
            Assert.Null(result);
        }

        #endregion
    }
}
