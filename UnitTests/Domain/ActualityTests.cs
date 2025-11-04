using Domain;

namespace UnitTests.Domain
{
    public class ActualityTests
    {
        #region Constructor

        [Fact]
        public void Constructor_WithValidParameters_ShouldCreateActuality()
        {
            // Arrange
            var title = "Test Title";
            var content = "Test Content";
            var start = DateTime.Now;
            var end = DateTime.Now.AddDays(7);

            // Act
            var actuality = new Actuality(title, content, start, end);

            // Assert
            Assert.Equal(title, actuality.Title);
            Assert.Equal(content, actuality.Content);
            Assert.Equal(start, actuality.StartPublish);
            Assert.Equal(end, actuality.EndPublish);
        }

        [Fact]
        public void Constructor_WithId_ShouldCreateActualityWithId()
        {
            // Arrange
            var id = Guid.NewGuid();
            var title = "Test Title";
            var content = "Test Content";
            var start = DateTime.Now;
            var end = DateTime.Now.AddDays(7);

            // Act
            var actuality = new Actuality(id, title, content, start, end);

            // Assert
            Assert.Equal(id, actuality.Id);
            Assert.Equal(title, actuality.Title);
            Assert.Equal(content, actuality.Content);
        }

        [Fact]
        public void Constructor_WithEmptyGuid_ShouldAssignProvidedId()
        {
            // Arrange
            var id = Guid.NewGuid();
            var title = "Test Title";
            var content = "Test Content";
            var start = DateTime.Now;

            // Act
            var actuality = new Actuality(id, title, content, start, null);

            // Assert
            Assert.Equal(id, actuality.Id);
        }

        [Fact]
        public void Constructor_WithOnlyStartDate_ShouldCreateActuality()
        {
            // Arrange
            var title = "Test Title";
            var content = "Test Content";
            var start = DateTime.Now;

            // Act
            var actuality = new Actuality(title, content, start, null);

            // Assert
            Assert.Equal(start, actuality.StartPublish);
            Assert.Null(actuality.EndPublish);
        }

        #endregion

        #region ChangeTitle Tests

        [Fact]
        public void ChangeTitle_WithValidTitle_ShouldUpdateTitle()
        {
            // Arrange
            var actuality = new Actuality("Initial Title", "Content", DateTime.Now, DateTime.Now.AddDays(1));
            var newTitle = "New Title";

            // Act
            actuality.ChangeTitle(newTitle);

            // Assert
            Assert.Equal(newTitle, actuality.Title);
        }

        [Fact]
        public void ChangeTitle_WithSingleCharacter_ShouldSucceed()
        {
            // Arrange
            var actuality = new Actuality("Initial Title", "Content", DateTime.Now, DateTime.Now.AddDays(1));
            var newTitle = "A";

            // Act
            actuality.ChangeTitle(newTitle);

            // Assert
            Assert.Equal(newTitle, actuality.Title);
        }

        [Fact]
        public void ChangeTitle_WithExactly100Characters_ShouldSucceed()
        {
            // Arrange
            var actuality = new Actuality("Initial Title", "Content", DateTime.Now, DateTime.Now.AddDays(1));
            var title100 = new string('A', 100);

            // Act
            actuality.ChangeTitle(title100);

            // Assert
            Assert.Equal(title100, actuality.Title);
            Assert.Equal(100, actuality.Title.Length);
        }

        [Fact]
        public void ChangeTitle_WithSpecialCharacters_ShouldSucceed()
        {
            // Arrange
            var actuality = new Actuality("Initial Title", "Content", DateTime.Now, DateTime.Now.AddDays(1));
            var newTitle = "Titre avec accents: éàü & spéciaux !@#";

            // Act
            actuality.ChangeTitle(newTitle);

            // Assert
            Assert.Equal(newTitle, actuality.Title);
        }

        [Fact]
        public void ChangeTitle_MultipleTimes_ShouldUpdateEachTime()
        {
            // Arrange
            var actuality = new Actuality("Initial Title", "Content", DateTime.Now, DateTime.Now.AddDays(1));

            // Act & Assert
            actuality.ChangeTitle("Second Title");
            Assert.Equal("Second Title", actuality.Title);

            actuality.ChangeTitle("Third Title");
            Assert.Equal("Third Title", actuality.Title);
        }

        #region Error Cases

        [Fact]
        public void ChangeTitle_WithNull_ShouldThrowArgumentException()
        {
            // Arrange
            var actuality = new Actuality("Initial Title", "Content", DateTime.Now, DateTime.Now.AddDays(1));

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => actuality.ChangeTitle(null));
            Assert.Contains("Title is not valid", exception.Message);
            Assert.Equal("Title", exception.ParamName);
        }

        [Fact]
        public void ChangeTitle_WithEmptyString_ShouldThrowArgumentException()
        {
            // Arrange
            var actuality = new Actuality("Initial Title", "Content", DateTime.Now, DateTime.Now.AddDays(1));

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => actuality.ChangeTitle(string.Empty));
            Assert.Contains("Title is not valid", exception.Message);
            Assert.Equal("Title", exception.ParamName);
        }

        [Fact]
        public void ChangeTitle_WithWhitespaceOnly_ShouldThrowArgumentException()
        {
            // Arrange
            var actuality = new Actuality("Initial Title", "Content", DateTime.Now, DateTime.Now.AddDays(1));

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => actuality.ChangeTitle("   "));
            Assert.Contains("Title is not valid", exception.Message);
            Assert.Equal("Title", exception.ParamName);
        }

        [Fact]
        public void ChangeTitle_WithTabsAndSpaces_ShouldThrowArgumentException()
        {
            // Arrange
            var actuality = new Actuality("Initial Title", "Content", DateTime.Now, DateTime.Now.AddDays(1));

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => actuality.ChangeTitle("\t\n  \r"));
            Assert.Contains("Title is not valid", exception.Message);
        }

        [Fact]
        public void ChangeTitle_With101Characters_ShouldThrowArgumentException()
        {
            // Arrange
            var actuality = new Actuality("Initial Title", "Content", DateTime.Now, DateTime.Now.AddDays(1));
            var longTitle = new string('A', 101);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => actuality.ChangeTitle(longTitle));
            Assert.Contains("Title must be max 100 length", exception.Message);
            Assert.Equal("Title", exception.ParamName);
        }

        [Fact]
        public void ChangeTitle_With200Characters_ShouldThrowArgumentException()
        {
            // Arrange
            var actuality = new Actuality("Initial Title", "Content", DateTime.Now, DateTime.Now.AddDays(1));
            var longTitle = new string('A', 200);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => actuality.ChangeTitle(longTitle));
            Assert.Contains("Title must be max 100 length", exception.Message);
        }

        [Fact]
        public void ChangeTitle_WithSameTitle_ShouldThrowArgumentException()
        {
            // Arrange
            var title = "Same Title";
            var actuality = new Actuality(title, "Content", DateTime.Now, DateTime.Now.AddDays(1));

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => actuality.ChangeTitle(title));
            Assert.Contains("Title is already define", exception.Message);
            Assert.Equal("Title", exception.ParamName);
        }

        [Fact]
        public void ChangeTitle_WithSameTitleDifferentCase_ShouldSucceed()
        {
            // Arrange
            var actuality = new Actuality("Initial Title", "Content", DateTime.Now, DateTime.Now.AddDays(1));

            // Act
            actuality.ChangeTitle("INITIAL TITLE");

            // Assert
            Assert.Equal("INITIAL TITLE", actuality.Title);
        }

        #endregion

        #endregion

        #region ChangeContent Tests

        [Fact]
        public void ChangeContent_WithValidContent_ShouldUpdateContent()
        {
            // Arrange
            var actuality = new Actuality("Title", "Initial Content", DateTime.Now, DateTime.Now.AddDays(1));
            var newContent = "New Content";

            // Act
            actuality.ChangeContent(newContent);

            // Assert
            Assert.Equal(newContent, actuality.Content);
        }

        [Fact]
        public void ChangeContent_WithSingleCharacter_ShouldSucceed()
        {
            // Arrange
            var actuality = new Actuality("Title", "Initial Content", DateTime.Now, DateTime.Now.AddDays(1));
            var newContent = "A";

            // Act
            actuality.ChangeContent(newContent);

            // Assert
            Assert.Equal(newContent, actuality.Content);
        }

        [Fact]
        public void ChangeContent_WithVeryLongContent_ShouldSucceed()
        {
            // Arrange
            var actuality = new Actuality("Title", "Initial Content", DateTime.Now, DateTime.Now.AddDays(1));
            var longContent = new string('A', 10000);

            // Act
            actuality.ChangeContent(longContent);

            // Assert
            Assert.Equal(longContent, actuality.Content);
        }

        [Fact]
        public void ChangeContent_WithMultilineContent_ShouldSucceed()
        {
            // Arrange
            var actuality = new Actuality("Title", "Initial Content", DateTime.Now, DateTime.Now.AddDays(1));
            var multilineContent = "Line 1\nLine 2\nLine 3";

            // Act
            actuality.ChangeContent(multilineContent);

            // Assert
            Assert.Equal(multilineContent, actuality.Content);
        }

        [Fact]
        public void ChangeContent_WithSpecialCharactersAndHtml_ShouldSucceed()
        {
            // Arrange
            var actuality = new Actuality("Title", "Initial Content", DateTime.Now, DateTime.Now.AddDays(1));
            var htmlContent = "<p>Contenu avec <strong>HTML</strong> & caractères spéciaux: €, ©</p>";

            // Act
            actuality.ChangeContent(htmlContent);

            // Assert
            Assert.Equal(htmlContent, actuality.Content);
        }

        [Fact]
        public void ChangeContent_MultipleTimes_ShouldUpdateEachTime()
        {
            // Arrange
            var actuality = new Actuality("Title", "Initial Content", DateTime.Now, DateTime.Now.AddDays(1));

            // Act & Assert
            actuality.ChangeContent("Second Content");
            Assert.Equal("Second Content", actuality.Content);

            actuality.ChangeContent("Third Content");
            Assert.Equal("Third Content", actuality.Content);
        }

        #region Error Cases

        [Fact]
        public void ChangeContent_WithNull_ShouldThrowArgumentException()
        {
            // Arrange
            var actuality = new Actuality("Title", "Initial Content", DateTime.Now, DateTime.Now.AddDays(1));

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => actuality.ChangeContent(null));
            Assert.Contains("Content must containt value", exception.Message);
        }

        [Fact]
        public void ChangeContent_WithEmptyString_ShouldThrowArgumentException()
        {
            // Arrange
            var actuality = new Actuality("Title", "Initial Content", DateTime.Now, DateTime.Now.AddDays(1));

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => actuality.ChangeContent(string.Empty));
            Assert.Contains("Content must containt value", exception.Message);
        }

        [Fact]
        public void ChangeContent_WithWhitespaceOnly_ShouldThrowArgumentException()
        {
            // Arrange
            var actuality = new Actuality("Title", "Initial Content", DateTime.Now, DateTime.Now.AddDays(1));

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => actuality.ChangeContent("   "));
            Assert.Contains("Content must containt value", exception.Message);
        }

        [Fact]
        public void ChangeContent_WithTabsAndNewlines_ShouldThrowArgumentException()
        {
            // Arrange
            var actuality = new Actuality("Title", "Initial Content", DateTime.Now, DateTime.Now.AddDays(1));

            // Act & Assert
            Assert.Throws<ArgumentException>(() => actuality.ChangeContent("\t\n\r  "));
        }

        [Fact]
        public void ChangeContent_WithSameContent_ShouldThrowArgumentException()
        {
            // Arrange
            var content = "Same Content";
            var actuality = new Actuality("Title", content, DateTime.Now, DateTime.Now.AddDays(1));

            // Act & Assert
            Assert.Throws<ArgumentException>(() => actuality.ChangeContent(content));
        }

        #endregion

        #endregion

        #region ChangePublish Tests

        [Fact]
        public void ChangePublish_WithValidStartAndEnd_ShouldUpdateDates()
        {
            // Arrange
            var actuality = new Actuality("Title", "Content", DateTime.Now, DateTime.Now.AddDays(1));
            var newStart = new DateTime(2025, 12, 1, 10, 0, 0);
            var newEnd = new DateTime(2025, 12, 31, 23, 59, 59);

            // Act
            actuality.ChangePublish(newStart, newEnd);

            // Assert
            Assert.Equal(newStart, actuality.StartPublish);
            Assert.Equal(newEnd, actuality.EndPublish);
        }

        [Fact]
        public void ChangePublish_WithOnlyStartDate_ShouldUpdateStartAndSetEndToNull()
        {
            // Arrange
            var actuality = new Actuality("Title", "Content", DateTime.Now, DateTime.Now.AddDays(1));
            var newStart = new DateTime(2025, 12, 1);

            // Act
            actuality.ChangePublish(newStart, null);

            // Assert
            Assert.Equal(newStart, actuality.StartPublish);
            Assert.Null(actuality.EndPublish);
        }

        [Fact]
        public void ChangePublish_WithStartOneSecondBeforeEnd_ShouldSucceed()
        {
            // Arrange
            var actuality = new Actuality("Title", "Content", DateTime.Now, DateTime.Now.AddDays(1));
            var start = new DateTime(2025, 12, 1, 10, 0, 0);
            var end = new DateTime(2025, 12, 1, 10, 0, 1);

            // Act
            actuality.ChangePublish(start, end);

            // Assert
            Assert.Equal(start, actuality.StartPublish);
            Assert.Equal(end, actuality.EndPublish);
        }

        [Fact]
        public void ChangePublish_WithStartInPast_ShouldSucceed()
        {
            // Arrange
            var actuality = new Actuality("Title", "Content", DateTime.Now, DateTime.Now.AddDays(1));
            var start = new DateTime(2020, 1, 1);
            var end = new DateTime(2020, 12, 31);

            // Act
            actuality.ChangePublish(start, end);

            // Assert
            Assert.Equal(start, actuality.StartPublish);
            Assert.Equal(end, actuality.EndPublish);
        }

        [Fact]
        public void ChangePublish_WithStartInFuture_ShouldSucceed()
        {
            // Arrange
            var actuality = new Actuality("Title", "Content", DateTime.Now, DateTime.Now.AddDays(1));
            var start = new DateTime(2030, 1, 1);
            var end = new DateTime(2030, 12, 31);

            // Act
            actuality.ChangePublish(start, end);

            // Assert
            Assert.Equal(start, actuality.StartPublish);
            Assert.Equal(end, actuality.EndPublish);
        }

        [Fact]
        public void ChangePublish_WithVeryLongPeriod_ShouldSucceed()
        {
            // Arrange
            var actuality = new Actuality("Title", "Content", DateTime.Now, DateTime.Now.AddDays(1));
            var start = new DateTime(2025, 1, 1);
            var end = new DateTime(2050, 12, 31);

            // Act
            actuality.ChangePublish(start, end);

            // Assert
            Assert.Equal(start, actuality.StartPublish);
            Assert.Equal(end, actuality.EndPublish);
        }

        [Fact]
        public void ChangePublish_MultipleTimes_ShouldUpdateEachTime()
        {
            // Arrange
            var actuality = new Actuality("Title", "Content", DateTime.Now, DateTime.Now.AddDays(1));
            var start1 = new DateTime(2025, 1, 1);
            var end1 = new DateTime(2025, 12, 31);
            var start2 = new DateTime(2026, 1, 1);
            var end2 = new DateTime(2026, 12, 31);

            // Act & Assert
            actuality.ChangePublish(start1, end1);
            Assert.Equal(start1, actuality.StartPublish);
            Assert.Equal(end1, actuality.EndPublish);

            actuality.ChangePublish(start2, end2);
            Assert.Equal(start2, actuality.StartPublish);
            Assert.Equal(end2, actuality.EndPublish);
        }

        #region Error Cases

        [Fact]
        public void ChangePublish_WithBothDatesNull_ShouldThrowArgumentException()
        {
            // Arrange
            var actuality = new Actuality("Title", "Content", DateTime.Now, DateTime.Now.AddDays(1));

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => actuality.ChangePublish(null, null));
            Assert.Contains("At least one of the two dates (start or end) must be defined", exception.Message);
        }

        [Fact]
        public void ChangePublish_WithOnlyEndDate_ShouldUpdateOnlyEndDate()
        {
            // Arrange
            var initialStart = new DateTime(2025, 1, 1);
            var actuality = new Actuality("Title", "Content", initialStart, DateTime.Now.AddDays(1));
            var newEnd = new DateTime(2025, 12, 31);

            // Act
            actuality.ChangePublish(null, newEnd);

            // Assert - StartPublish doit rester inchangée
            Assert.Equal(initialStart, actuality.StartPublish);
            Assert.Equal(newEnd, actuality.EndPublish);
        }

        [Fact]
        public void ChangePublish_WithStartAfterEnd_ShouldThrowArgumentException()
        {
            // Arrange
            var actuality = new Actuality("Title", "Content", DateTime.Now, DateTime.Now.AddDays(1));
            var start = new DateTime(2025, 12, 31);
            var end = new DateTime(2025, 1, 1);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => actuality.ChangePublish(start, end));
            Assert.Contains("The start date must be strictly earlier than the end date", exception.Message);
        }

        [Fact]
        public void ChangePublish_WithStartEqualToEnd_ShouldThrowArgumentException()
        {
            // Arrange
            var actuality = new Actuality("Title", "Content", DateTime.Now, DateTime.Now.AddDays(1));
            var date = new DateTime(2025, 12, 1, 10, 30, 0);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => actuality.ChangePublish(date, date));
            Assert.Contains("The start date must be strictly earlier than the end date", exception.Message);
        }

        [Fact]
        public void ChangePublish_WithStartOneMsAfterEnd_ShouldThrowArgumentException()
        {
            // Arrange
            var actuality = new Actuality("Title", "Content", DateTime.Now, DateTime.Now.AddDays(1));
            var end = new DateTime(2025, 12, 1, 10, 0, 0);
            var start = new DateTime(2025, 12, 1, 10, 0, 0).AddMilliseconds(1);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => actuality.ChangePublish(start, end));
            Assert.Contains("The start date must be strictly earlier than the end date", exception.Message);
        }

        [Fact]
        public void ChangePublish_WithOnlyEndDate_ShouldChangePublish()
        {
            // Arrange
            var initialStart = new DateTime(2025, 6, 1);
            var actuality = new Actuality("Title", "Content", initialStart, DateTime.Now.AddDays(1));
            var newEnd = new DateTime(2025, 1, 1);

            //Act
            actuality.ChangePublish(null, newEnd);

            // Assert
            Assert.NotNull(actuality.EndPublish);
            Assert.Equal(initialStart, actuality.StartPublish);
            Assert.Equal(newEnd, actuality.EndPublish);
        }

        [Fact]
        public void ChangePublish_CanSetEndToNull_IfStartHasValue()
        {
            // Arrange
            var initialStart = new DateTime(2025, 1, 1);
            var initialEnd = new DateTime(2025, 12, 31);
            var actuality = new Actuality("Title", "Content", initialStart, initialEnd);

            Assert.NotNull(actuality.EndPublish); // Vérifier qu'on a bien une end date

            // Act - Mettre end à null en gardant start
            actuality.ChangePublish(initialStart, null);

            // Assert
            Assert.Equal(initialStart, actuality.StartPublish);
            Assert.Null(actuality.EndPublish);
        }

        [Fact]
        public void ChangePublish_CanChangeOnlyEndDate_KeepingExistingStart()
        {
            // Arrange
            var initialStart = new DateTime(2025, 1, 1);
            var initialEnd = new DateTime(2025, 6, 30);
            var actuality = new Actuality("Title", "Content", initialStart, initialEnd);

            var newEnd = new DateTime(2025, 12, 31);

            // Act - Changer seulement end en passant null pour start
            actuality.ChangePublish(null, newEnd);

            // Assert
            Assert.Equal(initialStart, actuality.StartPublish); // Start inchangée
            Assert.Equal(newEnd, actuality.EndPublish);
        }

        #endregion

        #endregion

        #region Integration Tests - Combined Scenarios

        [Fact]
        public void Actuality_CompleteLifecycle_ShouldWorkCorrectly()
        {
            // Arrange
            var title1 = "Initial Title";
            var content1 = "Initial Content";
            var start1 = new DateTime(2025, 1, 1);
            var end1 = new DateTime(2025, 12, 31);

            // Act - Create
            var actuality = new Actuality(title1, content1, start1, end1);

            // Assert - Initial state
            Assert.Equal(title1, actuality.Title);
            Assert.Equal(content1, actuality.Content);
            Assert.Equal(start1, actuality.StartPublish);
            Assert.Equal(end1, actuality.EndPublish);

            // Act - Update all properties
            var title2 = "Updated Title";
            var content2 = "Updated Content";
            var start2 = new DateTime(2026, 1, 1);
            var end2 = new DateTime(2026, 12, 31);

            actuality.ChangeTitle(title2);
            actuality.ChangeContent(content2);
            actuality.ChangePublish(start2, end2);

            // Assert - Updated state
            Assert.Equal(title2, actuality.Title);
            Assert.Equal(content2, actuality.Content);
            Assert.Equal(start2, actuality.StartPublish);
            Assert.Equal(end2, actuality.EndPublish);
        }

        [Fact]
        public void Actuality_WithIdConstructor_ShouldMaintainIdThroughUpdates()
        {
            // Arrange
            var id = Guid.NewGuid();
            var actuality = new Actuality(id, "Title", "Content", DateTime.Now, DateTime.Now.AddDays(1));

            // Act
            actuality.ChangeTitle("New Title");
            actuality.ChangeContent("New Content");
            actuality.ChangePublish(DateTime.Now.AddDays(10), DateTime.Now.AddDays(20));

            // Assert
            Assert.Equal(id, actuality.Id);
        }

        [Fact]
        public void Actuality_ChangePublishFromWithEndToWithoutEnd_ShouldSucceed()
        {
            // Arrange
            var actuality = new Actuality("Title", "Content", DateTime.Now, DateTime.Now.AddDays(7));
            Assert.NotNull(actuality.EndPublish);

            // Act
            actuality.ChangePublish(DateTime.Now.AddDays(10), null);

            // Assert
            Assert.Null(actuality.EndPublish);
        }

        [Fact]
        public void Actuality_ChangePublishFromWithoutEndToWithEnd_ShouldSucceed()
        {
            // Arrange
            var actuality = new Actuality("Title", "Content", DateTime.Now, null);
            Assert.Null(actuality.EndPublish);

            // Act
            var newEnd = DateTime.Now.AddDays(10);
            actuality.ChangePublish(DateTime.Now.AddDays(1), newEnd);

            // Assert
            Assert.Equal(newEnd, actuality.EndPublish);
        }

        #endregion

        #region Edge Cases - Boundary Testing

        [Fact]
        public void ChangeTitle_WithTitle99Characters_ShouldSucceed()
        {
            // Arrange
            var actuality = new Actuality("Initial Title", "Content", DateTime.Now, DateTime.Now.AddDays(1));
            var title99 = new string('A', 99);

            // Act
            actuality.ChangeTitle(title99);

            // Assert
            Assert.Equal(99, actuality.Title.Length);
        }

        [Fact]
        public void ChangeTitle_WithUnicodeCharacters_ShouldSucceed()
        {
            // Arrange
            var actuality = new Actuality("Initial Title", "Content", DateTime.Now, DateTime.Now.AddDays(1));
            var unicodeTitle = "Titre avec émojis 🎉🎊 et caractères japonais 日本語";

            // Act
            actuality.ChangeTitle(unicodeTitle);

            // Assert
            Assert.Equal(unicodeTitle, actuality.Title);
        }

        [Fact]
        public void ChangeContent_WithOnlyNumbers_ShouldSucceed()
        {
            // Arrange
            var actuality = new Actuality("Title", "Initial Content", DateTime.Now, DateTime.Now.AddDays(1));

            // Act
            actuality.ChangeContent("123456789");

            // Assert
            Assert.Equal("123456789", actuality.Content);
        }

        [Fact]
        public void ChangePublish_WithSameStartMultipleCalls_ShouldSucceed()
        {
            // Arrange
            var actuality = new Actuality("Title", "Content", DateTime.Now, DateTime.Now.AddDays(1));
            var start = new DateTime(2025, 1, 1);

            // Act & Assert - Should succeed even with same start but different end
            actuality.ChangePublish(start, new DateTime(2025, 6, 1));
            actuality.ChangePublish(start, new DateTime(2025, 12, 31));

            Assert.Equal(start, actuality.StartPublish);
            Assert.Equal(new DateTime(2025, 12, 31), actuality.EndPublish);
        }

        #endregion

    }
}
