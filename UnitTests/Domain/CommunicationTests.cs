using Domain;

namespace UnitTests.Domain
{
    public class CommunicationTests
    {
        #region Constructor

        [Fact]
        public void Communication_ShouldCreate_WhenCommunicationCorrect()
        {
            // Arrange
            var content = "Content";
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(1);

            // Act
            var communication = new Communication(content, startDate, endDate);

            // Assert
            Assert.NotNull(communication);
            Assert.Equal(content, communication.Content);
            Assert.Equal(startDate, communication.StartDate);
            Assert.Equal(endDate, communication.EndDate);
            Assert.Equal(Guid.Empty, communication.Id);
        }

        [Fact]
        public void Communication_ShouldCreate_WhenOnlyStartDate()
        {
            // Arrange
            var content = "Content";
            var startDate = DateTime.Now;
            DateTime? endDate = null;

            // Act
            var communication = new Communication(content, startDate, endDate);

            // Assert
            Assert.NotNull(communication);
            Assert.Equal(content, communication.Content);
            Assert.Equal(startDate, communication.StartDate);
            Assert.Null(communication.EndDate);
        }

        [Fact]
        public void Communication_ShouldCreate_WhenOnlyEndDate()
        {
            // Arrange
            var content = "Content";
            DateTime? startDate = null;
            var endDate = DateTime.Now.AddDays(1);

            // Act
            var communication = new Communication(content, startDate, endDate);

            // Assert
            Assert.NotNull(communication);
            Assert.Null(communication.StartDate);
            Assert.NotNull(communication.EndDate);
            Assert.Equal(endDate, communication.EndDate);
        }

        [Fact]
        public void Communication_ShouldCreate_WithId_WhenAllParametersCorrect()
        {
            // Arrange
            var id = Guid.NewGuid();
            var content = "Content";
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(1);

            // Act
            var communication = new Communication(id, content, startDate, endDate);

            // Assert
            Assert.NotNull(communication);
            Assert.Equal(id, communication.Id);
            Assert.Equal(content, communication.Content);
            Assert.Equal(startDate, communication.StartDate);
            Assert.Equal(endDate, communication.EndDate);
        }

        [Fact]
        public void Communication_ShouldCreate_WithEmptyGuid_WhenIdIsEmpty()
        {
            // Arrange
            var id = Guid.Empty;
            var content = "Content";
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(1);

            // Act
            var communication = new Communication(id, content, startDate, endDate);

            // Assert
            Assert.Equal(Guid.Empty, communication.Id);
        }

        [Fact]
        public void Communication_ShouldCreate_WithDefaultConstructor()
        {
            // Act
            var communication = new Communication();

            // Assert
            Assert.NotNull(communication);
            Assert.Equal(Guid.Empty, communication.Id);
            Assert.Equal(string.Empty, communication.Content);
            Assert.Null(communication.StartDate);
            Assert.Null(communication.EndDate);
        }
#endregion

        #region ChangeContent

        [Fact]
        public void ChangeContent_ShouldUpdateContent_WhenContentValid()
        {
            // Arrange
            var communication = new Communication("Initial Content", DateTime.Now, null);
            var newContent = "Updated Content";

            // Act
            communication.ChangeContent(newContent);

            // Assert
            Assert.Equal(newContent, communication.Content);
        }

        [Fact]
        public void ChangeContent_ShouldThrowException_WhenContentEmpty()
        {
            // Arrange
            var communication = new Communication("Initial Content", DateTime.Now, null);
            var newContent = string.Empty;

            // Act
            var caughtException = Assert.ThrowsAny<Exception>(() => communication.ChangeContent(newContent));

            // Assert
            Assert.NotNull(caughtException);
            Assert.IsType<ArgumentException>(caughtException);
            Assert.Equal("Content must containt value", caughtException.Message);
        }

        [Fact]
        public void ChangeContent_ShouldThrowException_WhenContentNull()
        {
            // Arrange
            var communication = new Communication("Initial Content", DateTime.Now, null);
            string newContent = null;

            // Act
            var caughtException = Assert.ThrowsAny<Exception>(() => communication.ChangeContent(newContent));

            // Assert
            Assert.NotNull(caughtException);
            Assert.IsType<ArgumentException>(caughtException);
            Assert.Equal("Content must containt value", caughtException.Message);
        }

        [Fact]
        public void ChangeContent_ShouldThrowException_WhenContentWhitespace()
        {
            // Arrange
            var communication = new Communication("Initial Content", DateTime.Now, null);
            var newContent = "   ";

            // Act
            var caughtException = Assert.ThrowsAny<Exception>(() => communication.ChangeContent(newContent));

            // Assert
            Assert.NotNull(caughtException);
            Assert.IsType<ArgumentException>(caughtException);
            Assert.Equal("Content must containt value", caughtException.Message);
        }

        [Fact]
        public void ChangeContent_ShouldThrowException_WhenContentSame()
        {
            // Arrange
            var content = "Same Content";
            var communication = new Communication(content, DateTime.Now, null);

            // Act
            var caughtException = Assert.ThrowsAny<Exception>(() => communication.ChangeContent(content));

            // Assert
            Assert.NotNull(caughtException);
            Assert.IsType<ArgumentException>(caughtException);
        }

        [Fact]
        public void ChangeContent_ShouldAllowMultipleChanges()
        {
            // Arrange
            var communication = new Communication("Content 1", DateTime.Now, null);

            // Act
            communication.ChangeContent("Content 2");
            communication.ChangeContent("Content 3");

            // Assert
            Assert.Equal("Content 3", communication.Content);
        }

        [Fact]
        public void ChangeContent_ShouldUpdateContent_WhenContentIsVeryLong()
        {
            // Arrange
            var communication = new Communication("Initial", DateTime.Now, null);
            var newContent = new string('B', 100000);

            // Act
            communication.ChangeContent(newContent);

            // Assert
            Assert.Equal(newContent, communication.Content);
        }

        [Fact]
        public void ChangeContent_ShouldUpdateContent_WhenContentHasSpecialCharacters()
        {
            // Arrange
            var communication = new Communication("Initial", DateTime.Now, null);
            var newContent = "New content with special: é à ü ñ 中文 🎉";

            // Act
            communication.ChangeContent(newContent);

            // Assert
            Assert.Equal(newContent, communication.Content);
        }

        [Fact]
        public void ChangeContent_ShouldThrowException_WhenContentSameIgnoringCase()
        {
            // Arrange
            var content = "Same Content";
            var communication = new Communication(content, DateTime.Now, null);

            // Act
            var caughtException = Assert.ThrowsAny<Exception>(() => communication.ChangeContent("SAME CONTENT"));

            // Assert
            Assert.NotNull(caughtException);
            Assert.IsType<ArgumentException>(caughtException);
            Assert.Equal("The new content must be different from the current content.", caughtException.Message);
        }

        [Fact]
        public void ChangeContent_ShouldThrowException_WhenContentSameWithMixedCase()
        {
            // Arrange
            var content = "Hello World";
            var communication = new Communication(content, DateTime.Now, null);

            // Act
            var caughtException = Assert.ThrowsAny<Exception>(() => communication.ChangeContent("hELLO wORLD"));

            // Assert
            Assert.NotNull(caughtException);
            Assert.IsType<ArgumentException>(caughtException);
            Assert.Equal("The new content must be different from the current content.", caughtException.Message);
        }

        [Fact]
        public void ChangeContent_ShouldThrowException_WhenContentSameWithDifferentCasing()
        {
            // Arrange
            var content = "test content";
            var communication = new Communication(content, DateTime.Now, null);

            // Act
            var caughtException = Assert.ThrowsAny<Exception>(() => communication.ChangeContent("Test Content"));

            // Assert
            Assert.NotNull(caughtException);
            Assert.IsType<ArgumentException>(caughtException);
            Assert.Equal("The new content must be different from the current content.", caughtException.Message);
        }

        [Fact]
        public void ChangeContent_ShouldThrowException_WhenContentSameAllUppercase()
        {
            // Arrange
            var content = "content";
            var communication = new Communication(content, DateTime.Now, null);

            // Act
            var caughtException = Assert.ThrowsAny<Exception>(() => communication.ChangeContent("CONTENT"));

            // Assert
            Assert.NotNull(caughtException);
            Assert.IsType<ArgumentException>(caughtException);
            Assert.Equal("The new content must be different from the current content.", caughtException.Message);
        }

        [Fact]
        public void ChangeContent_ShouldThrowException_WhenContentSameAllLowercase()
        {
            // Arrange
            var content = "CONTENT";
            var communication = new Communication(content, DateTime.Now, null);

            // Act
            var caughtException = Assert.ThrowsAny<Exception>(() => communication.ChangeContent("content"));

            // Assert
            Assert.NotNull(caughtException);
            Assert.IsType<ArgumentException>(caughtException);
            Assert.Equal("The new content must be different from the current content.", caughtException.Message);
        }

        #region Errors

        [Fact]
        public void Communication_ShouldThrowException_WhenContentEmpty()
        {
            // Arrange
            var content = string.Empty;
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(1);

            // Act
            var caughtException = Assert.ThrowsAny<Exception>(() => new Communication(content, startDate, endDate));

            // Assert
            Assert.NotNull(caughtException);
            Assert.IsType<ArgumentException>(caughtException);
            Assert.Equal("Content must containt value", caughtException.Message);
        }

        [Fact]
        public void Communication_ShouldThrowException_WhenContentNull()
        {
            // Arrange
            string content = null;
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(1);

            // Act
            var caughtException = Assert.ThrowsAny<Exception>(() => new Communication(content, startDate, endDate));

            // Assert
            Assert.NotNull(caughtException);
            Assert.IsType<ArgumentException>(caughtException);
            Assert.Equal("Content must containt value", caughtException.Message);
        }

        [Fact]
        public void Communication_ShouldThrowException_WhenContentWhitespace()
        {
            // Arrange
            var content = "   ";
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(1);

            // Act
            var caughtException = Assert.ThrowsAny<Exception>(() => new Communication(content, startDate, endDate));

            // Assert
            Assert.NotNull(caughtException);
            Assert.IsType<ArgumentException>(caughtException);
            Assert.Equal("Content must containt value", caughtException.Message);
        }

        [Fact]
        public void Communication_ShouldThrowException_WhenContentOnlyTabs()
        {
            // Arrange
            var content = "\t\t\t";
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(1);

            // Act
            var caughtException = Assert.ThrowsAny<Exception>(() => new Communication(content, startDate, endDate));

            // Assert
            Assert.NotNull(caughtException);
            Assert.IsType<ArgumentException>(caughtException);
            Assert.Equal("Content must containt value", caughtException.Message);
        }

        [Fact]
        public void Communication_ShouldThrowException_WhenContentOnlyNewlines()
        {
            // Arrange
            var content = "\n\n\n";
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(1);

            // Act
            var caughtException = Assert.ThrowsAny<Exception>(() => new Communication(content, startDate, endDate));

            // Assert
            Assert.NotNull(caughtException);
            Assert.IsType<ArgumentException>(caughtException);
            Assert.Equal("Content must containt value", caughtException.Message);
        }

        #endregion

        #region Limite 

        [Fact]
        public void Communication_ShouldCreate_WhenContentIsOneCharacter()
        {
            // Arrange
            var content = "A";
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(1);

            // Act
            var communication = new Communication(content, startDate, endDate);

            // Assert
            Assert.NotNull(communication);
            Assert.Equal(content, communication.Content);
        }

        [Fact]
        public void Communication_ShouldCreate_WhenContentIsVeryLong()
        {
            // Arrange
            var content = new string('A', 100000);
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(1);

            // Act
            var communication = new Communication(content, startDate, endDate);

            // Assert
            Assert.NotNull(communication);
            Assert.Equal(content, communication.Content);
        }

        [Fact]
        public void Communication_ShouldCreate_WhenContentContainsSpecialCharacters()
        {
            // Arrange
            var content = "Content with special chars: @#$%^&*()_+-={}[]|\\:;\"'<>,.?/~`";
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(1);

            // Act
            var communication = new Communication(content, startDate, endDate);

            // Assert
            Assert.NotNull(communication);
            Assert.Equal(content, communication.Content);
        }

        [Fact]
        public void Communication_ShouldCreate_WhenContentContainsUnicodeCharacters()
        {
            // Arrange
            var content = "Content with unicode: é à ü ñ 中文 🎉";
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(1);

            // Act
            var communication = new Communication(content, startDate, endDate);

            // Assert
            Assert.NotNull(communication);
            Assert.Equal(content, communication.Content);
        }

        [Fact]
        public void Communication_ShouldCreate_WhenContentHasLeadingAndTrailingSpaces()
        {
            // Arrange
            var content = "  Content with spaces  ";
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(1);

            // Act
            var communication = new Communication(content, startDate, endDate);

            // Assert
            Assert.NotNull(communication);
            Assert.Equal(content, communication.Content);
        }

        #endregion

        #endregion

        #region ChangeDates

        [Fact]
        public void ChangeDates_ShouldUpdateDates_WhenDatesValid()
        {
            // Arrange
            var communication = new Communication("Content", DateTime.Now, null);
            var newStartDate = DateTime.Now.AddDays(10);
            var newEndDate = DateTime.Now.AddDays(15);

            // Act
            communication.ChangeDates(newStartDate, newEndDate);

            // Assert
            Assert.Equal(newStartDate, communication.StartDate);
            Assert.Equal(newEndDate, communication.EndDate);
        }

        [Fact]
        public void ChangeDates_ShouldUpdateToOnlyStartDate_WhenEndDateNull()
        {
            // Arrange
            var communication = new Communication("Content", DateTime.Now, DateTime.Now.AddDays(5));
            var newStartDate = DateTime.Now.AddDays(10);
            DateTime? newEndDate = null;

            // Act
            communication.ChangeDates(newStartDate, newEndDate);

            // Assert
            Assert.Equal(newStartDate, communication.StartDate);
            Assert.Null(communication.EndDate);
        }

        [Fact]
        public void ChangeDates_ShouldThrowException_WhenChangingToOnlyEndDate()
        {
            // Arrange
            var communication = new Communication("Content", DateTime.Now, null);
            DateTime? newStartDate = null;
            var newEndDate = DateTime.Now.AddDays(10);

            // Act
            communication.ChangeDates(newStartDate, newEndDate);

            // Assert
            Assert.Null(communication.StartDate);
            Assert.NotNull(communication.EndDate);
            Assert.Equal(newEndDate, communication.EndDate);
        }

        [Fact]
        public void ChangeDates_ShouldThrowException_WhenBothDatesNull()
        {
            // Arrange
            var communication = new Communication("Content", DateTime.Now, null);
            DateTime? startDate = null;
            DateTime? endDate = null;

            // Act
            var caughtException = Assert.ThrowsAny<Exception>(() => communication.ChangeDates(startDate, endDate));

            // Assert
            Assert.NotNull(caughtException);
            Assert.IsType<ArgumentException>(caughtException);
            Assert.Equal("At least one of the two dates (start or end) must be defined.", caughtException.Message);
        }

        [Fact]
        public void ChangeDates_ShouldThrowException_WhenStartDateNotEarlierThanEndDate()
        {
            // Arrange
            var communication = new Communication("Content", DateTime.Now, null);
            var startDate = DateTime.Now.AddDays(5);
            var endDate = DateTime.Now.AddDays(3);

            // Act
            var caughtException = Assert.ThrowsAny<Exception>(() => communication.ChangeDates(startDate, endDate));

            // Assert
            Assert.NotNull(caughtException);
            Assert.IsType<ArgumentException>(caughtException);
            Assert.Equal("The start date must be strictly earlier than the end date.", caughtException.Message);
        }

        [Fact]
        public void ChangeDates_ShouldThrowException_WhenStartDateEqualsEndDate()
        {
            // Arrange
            var communication = new Communication("Content", DateTime.Now, null);
            var date = new DateTime(2025, 1, 1, 12, 0, 0);

            // Act
            var caughtException = Assert.ThrowsAny<Exception>(() => communication.ChangeDates(date, date));

            // Assert
            Assert.NotNull(caughtException);
            Assert.IsType<ArgumentException>(caughtException);
            Assert.Equal("The start date must be strictly earlier than the end date.", caughtException.Message);
        }

        [Fact]
        public void ChangeDates_ShouldAllowMultipleChanges()
        {
            // Arrange
            var communication = new Communication("Content", DateTime.Now, null);
            var date1 = DateTime.Now.AddDays(1);
            var date2 = DateTime.Now.AddDays(5);
            var date3 = DateTime.Now.AddDays(10);

            // Act
            communication.ChangeDates(date1, date2);
            communication.ChangeDates(date2, date3);

            // Assert
            Assert.Equal(date2, communication.StartDate);
            Assert.Equal(date3, communication.EndDate);
        }

        [Fact]
        public void ChangeDates_ShouldUpdate_WhenDatesAreMinAndMaxValue()
        {
            // Arrange
            var communication = new Communication("Content", DateTime.Now, null);
            var startDate = DateTime.MinValue;
            var endDate = DateTime.MaxValue;

            // Act
            communication.ChangeDates(startDate, endDate);

            // Assert
            Assert.Equal(startDate, communication.StartDate);
            Assert.Equal(endDate, communication.EndDate);
        }

        [Fact]
        public void ChangeDates_ShouldUpdate_WhenDatesAreOneMillisecondApart()
        {
            // Arrange
            var communication = new Communication("Content", DateTime.Now, null);
            var startDate = new DateTime(2025, 1, 1, 12, 0, 0, 0);
            var endDate = startDate.AddMilliseconds(1);

            // Act
            communication.ChangeDates(startDate, endDate);

            // Assert
            Assert.Equal(startDate, communication.StartDate);
            Assert.Equal(endDate, communication.EndDate);
        }

        #region Error

        [Fact]
        public void Communication_ShouldThrowException_WhenNoDate()
        {
            // Arrange
            var content = "Content";
            DateTime? startDate = null;
            DateTime? endDate = null;

            // Act
            var caughtException = Assert.ThrowsAny<Exception>(() => new Communication(content, startDate, endDate));

            // Assert
            Assert.NotNull(caughtException);
            Assert.IsType<ArgumentException>(caughtException);
            Assert.Equal("At least one of the two dates (start or end) must be defined.", caughtException.Message);
        }

        [Fact]
        public void Communication_ShouldThrowException_WhenDateNotCorrect()
        {
            // Arrange
            var content = "Content";
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(-1);

            // Act
            var caughtException = Assert.ThrowsAny<Exception>(() => new Communication(content, startDate, endDate));

            // Assert
            Assert.NotNull(caughtException);
            Assert.IsType<ArgumentException>(caughtException);
            Assert.Equal("The start date must be strictly earlier than the end date.", caughtException.Message);
        }

        [Fact]
        public void Communication_ShouldThrowException_WhenStartDateEqualsEndDate()
        {
            // Arrange
            var content = "Content";
            var startDate = new DateTime(2025, 1, 1, 12, 0, 0);
            var endDate = new DateTime(2025, 1, 1, 12, 0, 0);

            // Act
            var caughtException = Assert.ThrowsAny<Exception>(() => new Communication(content, startDate, endDate));

            // Assert
            Assert.NotNull(caughtException);
            Assert.IsType<ArgumentException>(caughtException);
            Assert.Equal("The start date must be strictly earlier than the end date.", caughtException.Message);
        }

        [Fact]
        public void Communication_ShouldThrowException_WhenStartDateOneMillisecondBeforeEndDate()
        {
            // Arrange
            var content = "Content";
            var endDate = new DateTime(2025, 1, 1, 12, 0, 0);
            var startDate = endDate.AddMilliseconds(-1);

            // Act
            var communication = new Communication(content, startDate, endDate);

            // Assert
            Assert.NotNull(communication);
            Assert.Equal(startDate, communication.StartDate);
            Assert.Equal(endDate, communication.EndDate);
        }

        #endregion

        #region Limites

        [Fact]
        public void Communication_ShouldCreate_WhenDatesAreFarApart()
        {
            // Arrange
            var content = "Content";
            var startDate = new DateTime(2000, 1, 1);
            var endDate = new DateTime(2100, 12, 31);

            // Act
            var communication = new Communication(content, startDate, endDate);

            // Assert
            Assert.NotNull(communication);
            Assert.Equal(startDate, communication.StartDate);
            Assert.Equal(endDate, communication.EndDate);
        }

        [Fact]
        public void Communication_ShouldCreate_WhenStartDateIsMinValue()
        {
            // Arrange
            var content = "Content";
            var startDate = DateTime.MinValue;
            var endDate = DateTime.MinValue.AddDays(1);

            // Act
            var communication = new Communication(content, startDate, endDate);

            // Assert
            Assert.NotNull(communication);
            Assert.Equal(startDate, communication.StartDate);
            Assert.Equal(endDate, communication.EndDate);
        }

        [Fact]
        public void Communication_ShouldCreate_WhenEndDateIsMaxValue()
        {
            // Arrange
            var content = "Content";
            var startDate = DateTime.MaxValue.AddDays(-1);
            var endDate = DateTime.MaxValue;

            // Act
            var communication = new Communication(content, startDate, endDate);

            // Assert
            Assert.NotNull(communication);
            Assert.Equal(startDate, communication.StartDate);
            Assert.Equal(endDate, communication.EndDate);
        }

        [Fact]
        public void Communication_ShouldCreate_WhenOnlyStartDateIsMinValue()
        {
            // Arrange
            var content = "Content";
            var startDate = DateTime.MinValue;
            DateTime? endDate = null;

            // Act
            var communication = new Communication(content, startDate, endDate);

            // Assert
            Assert.NotNull(communication);
            Assert.Equal(startDate, communication.StartDate);
            Assert.Null(communication.EndDate);
        }

        #endregion

        #endregion

        #region Intégration

        [Fact]
        public void Communication_ShouldMaintainState_AfterMultipleOperations()
        {
            // Arrange
            var id = Guid.NewGuid();
            var communication = new Communication(id, "Initial Content", DateTime.Now, DateTime.Now.AddDays(1));

            // Act
            communication.ChangeContent("Updated Content");
            var newStartDate = DateTime.Now.AddDays(5);
            var newEndDate = DateTime.Now.AddDays(10);
            communication.ChangeDates(newStartDate, newEndDate);
            communication.ChangeContent("Final Content");

            // Assert
            Assert.Equal(id, communication.Id);
            Assert.Equal("Final Content", communication.Content);
            Assert.Equal(newStartDate, communication.StartDate);
            Assert.Equal(newEndDate, communication.EndDate);
        }

        [Fact]
        public void Communication_ShouldNotChangeId_AfterConstruction()
        {
            // Arrange
            var id = Guid.NewGuid();
            var communication = new Communication(id, "Content", DateTime.Now, DateTime.Now.AddDays(1));

            // Act
            communication.ChangeContent("New Content");
            communication.ChangeDates(DateTime.Now.AddDays(2), DateTime.Now.AddDays(3));

            // Assert
            Assert.Equal(id, communication.Id);
        }

        [Fact]
        public void Communication_ShouldKeepEmptyId_WhenConstructedWithoutId()
        {
            // Arrange & Act
            var communication = new Communication("Content", DateTime.Now, DateTime.Now.AddDays(1));

            // Assert
            Assert.Equal(Guid.Empty, communication.Id);
        }

        #endregion
    }
}
