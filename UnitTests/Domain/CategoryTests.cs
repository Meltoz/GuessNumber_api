using Domain.Party;

namespace UnitTests.Domain
{
    public class CategoryTests
    {
        #region Constructor Tests - Cas Nominaux

        [Fact]
        public void Constructor_WithValidName_ShouldCreateCategory()
        {
            // Arrange
            var name = "Electronics";

            // Act
            var category = new Category(name);

            // Assert
            Assert.Equal(name, category.Name);
            Assert.Equal(Guid.Empty, category.Id);
        }

        [Fact]
        public void Constructor_WithIdAndValidName_ShouldCreateCategoryWithId()
        {
            // Arrange
            var id = Guid.NewGuid();
            var name = "Books";

            // Act
            var category = new Category(id, name);

            // Assert
            Assert.Equal(id, category.Id);
            Assert.Equal(name, category.Name);
        }

        [Theory]
        [InlineData("A")]
        [InlineData("Ab")]
        [InlineData("Category Name")]
        [InlineData("Very Long Category Name With Multiple Words")]
        public void Constructor_WithVariousValidNames_ShouldSucceed(string name)
        {
            // Act
            var category = new Category(name);

            // Assert
            Assert.Equal(name, category.Name);
        }

        #endregion

        #region Constructor Tests - Cas Limites

        [Fact]
        public void Constructor_WithNameExactly500Characters_ShouldSucceed()
        {
            // Arrange
            var name = new string('A', 500);

            // Act
            var category = new Category(name);

            // Assert
            Assert.Equal(name, category.Name);
            Assert.Equal(500, category.Name.Length);
        }

        [Fact]
        public void Constructor_WithEmptyGuid_ShouldNotSetId()
        {
            // Arrange
            var emptyId = Guid.Empty;
            var name = "Test";

            // Act
            var category = new Category(emptyId, name);

            // Assert
            Assert.Equal(Guid.Empty, category.Id);
        }

        [Fact]
        public void Constructor_WithNameContainingSpecialCharacters_ShouldSucceed()
        {
            // Arrange
            var name = "Catégorie avec accents & caractères spéciaux @#$%";

            // Act
            var category = new Category(name);

            // Assert
            Assert.Equal(name, category.Name);
        }

        [Fact]
        public void Constructor_WithNameContainingWhitespaceInMiddle_ShouldSucceed()
        {
            // Arrange
            var name = "Category   With   Multiple   Spaces";

            // Act
            var category = new Category(name);

            // Assert
            Assert.Equal(name, category.Name);
        }

        #endregion

        #region Constructor Tests - Cas d'Erreur

        [Fact]
        public void Constructor_WithNullName_ShouldThrowArgumentException()
        {
            // Arrange
            string name = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Category(name));
            Assert.Equal("Name must be set", exception.Message);
        }

        [Fact]
        public void Constructor_WithEmptyName_ShouldThrowArgumentException()
        {
            // Arrange
            var name = string.Empty;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Category(name));
            Assert.Equal("Name must be set", exception.Message);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("   ")]
        [InlineData("\t")]
        [InlineData("\n")]
        [InlineData("\r\n")]
        public void Constructor_WithWhitespaceName_ShouldThrowArgumentException(string name)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Category(name));
            Assert.Equal("Name must be set", exception.Message);
        }

        [Fact]
        public void Constructor_WithNameOver500Characters_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var name = new string('A', 501);

            // Act & Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => new Category(name));
        }

        [Fact]
        public void Constructor_WithIdAndNullName_ShouldThrowArgumentException()
        {
            // Arrange
            var id = Guid.NewGuid();
            string name = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Category(id, name));
            Assert.Equal("Name must be set", exception.Message);
        }

        #endregion

        #region ChangeName Tests - Cas Nominaux

        [Fact]
        public void ChangeName_WithValidName_ShouldUpdateName()
        {
            // Arrange
            var category = new Category("Original Name");
            var newName = "Updated Name";

            // Act
            category.ChangeName(newName);

            // Assert
            Assert.Equal(newName, category.Name);
        }

        [Fact]
        public void ChangeName_MultipleTimes_ShouldKeepLastValue()
        {
            // Arrange
            var category = new Category("First");

            // Act
            category.ChangeName("Second");
            category.ChangeName("Third");
            category.ChangeName("Final");

            // Assert
            Assert.Equal("Final", category.Name);
        }

        [Fact]
        public void ChangeName_WithSameName_ShouldSucceed()
        {
            // Arrange
            var name = "Same Name";
            var category = new Category(name);

            // Act
            category.ChangeName(name);

            // Assert
            Assert.Equal(name, category.Name);
        }

        #endregion

        #region ChangeName Tests - Cas Limites

        [Fact]
        public void ChangeName_WithNameExactly500Characters_ShouldSucceed()
        {
            // Arrange
            var category = new Category("Initial");
            var newName = new string('B', 500);

            // Act
            category.ChangeName(newName);

            // Assert
            Assert.Equal(newName, category.Name);
            Assert.Equal(500, category.Name.Length);
        }

        [Fact]
        public void ChangeName_FromLongToShortName_ShouldSucceed()
        {
            // Arrange
            var category = new Category(new string('A', 400));
            var shortName = "Short";

            // Act
            category.ChangeName(shortName);

            // Assert
            Assert.Equal(shortName, category.Name);
        }

        #endregion

        #region ChangeName Tests - Cas d'Erreur

        [Fact]
        public void ChangeName_WithNullName_ShouldThrowArgumentException()
        {
            // Arrange
            var category = new Category("Original");

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => category.ChangeName(null));
            Assert.Equal("Name must be set", exception.Message);
            Assert.Equal("Original", category.Name); // Name should remain unchanged
        }

        [Fact]
        public void ChangeName_WithEmptyName_ShouldThrowArgumentException()
        {
            // Arrange
            var category = new Category("Original");

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => category.ChangeName(string.Empty));
            Assert.Equal("Name must be set", exception.Message);
            Assert.Equal("Original", category.Name);
        }

        [Theory]
        [InlineData(" ")]
        [InlineData("    ")]
        [InlineData("\t")]
        [InlineData("\n")]
        public void ChangeName_WithWhitespaceName_ShouldThrowArgumentException(string name)
        {
            // Arrange
            var category = new Category("Original");

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => category.ChangeName(name));
            Assert.Equal("Name must be set", exception.Message);
            Assert.Equal("Original", category.Name);
        }

        [Fact]
        public void ChangeName_WithNameOver500Characters_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var category = new Category("Original");
            var longName = new string('X', 501);

            // Act & Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => category.ChangeName(longName));
            Assert.Equal("Original", category.Name);
        }

        [Fact]
        public void ChangeName_WithNameMuchOver500Characters_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var category = new Category("Original");
            var veryLongName = new string('Y', 1000);

            // Act & Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => category.ChangeName(veryLongName));
        }

        #endregion

        #region Id Property Tests

        [Fact]
        public void Id_WhenCreatedWithoutId_ShouldBeEmpty()
        {
            // Arrange & Act
            var category = new Category("Test");

            // Assert
            Assert.Equal(Guid.Empty, category.Id);
        }

        [Fact]
        public void Id_WhenCreatedWithId_ShouldNotChange()
        {
            // Arrange
            var id = Guid.NewGuid();
            var category = new Category(id, "Test");

            // Act - Trying to change name shouldn't affect Id
            category.ChangeName("New Name");

            // Assert
            Assert.Equal(id, category.Id);
        }

        [Fact]
        public void Id_IsImmutable_CannotBeChangedAfterCreation()
        {
            // Arrange
            var firstId = Guid.NewGuid();
            var category = new Category(firstId, "Test");

            // Assert - Id should remain the same
            Assert.Equal(firstId, category.Id);

            // Note: Since Id is private set, there's no way to change it
            // This test documents the immutability behavior
        }

        #endregion
    }
}
