using Application.Interfaces.Repository;
using Application.Services;
using Domain.Enums;
using Domain.Party;
using Moq;

namespace UnitTests.Application
{
    public class QuestionServiceTests
    {
        private readonly Mock<IQuestionRepository> _questionRepositoryMock;
        private readonly QuestionService _service;

        public QuestionServiceTests()
        {
            _questionRepositoryMock = new Mock<IQuestionRepository>();
            _service = new QuestionService(_questionRepositoryMock.Object);
        }

        [Fact]
        public async Task AddQuestion_ShouldCallRepositoryInsertAsync()
        {
            // Arrange
            var category = new Category("Test Category");
            var question = new Question(
                "Test Question",
                "42",
                category,
                VisibilityQuestion.Public,
                TypeQuestion.Standard,
                "Test Author",
                "km"
            );

            _questionRepositoryMock.Setup(r => r.InsertAsync(It.IsAny<Question>()))
                .ReturnsAsync(question);

            // Act
            await _service.AddQuestion(
                "Test Question",
                "42",
                category,
                VisibilityQuestion.Public,
                TypeQuestion.Standard,
                "Test Author",
                "km"
            );

            // Assert
            _questionRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<Question>()), Times.Once);
        }

        [Fact]
        public async Task AddQuestion_ShouldReturnInsertedQuestion()
        {
            // Arrange
            var category = new Category("Science");
            var expectedQuestion = new Question(
                "Quelle est la capitale de la France ?",
                "1236",
                category,
                VisibilityQuestion.Public,
                TypeQuestion.Standard,
                "Author",
                "unit"
            );

            _questionRepositoryMock.Setup(r => r.InsertAsync(It.IsAny<Question>()))
                .ReturnsAsync(expectedQuestion);

            // Act
            var result = await _service.AddQuestion(
                "Quelle est la capitale de la France ?",
                "1245",
                category,
                VisibilityQuestion.Public,
                TypeQuestion.Standard,
                "Author",
                "unit"
            );

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedQuestion.Libelle, result.Libelle);
            Assert.Equal(expectedQuestion.Response, result.Response);
            Assert.Equal(expectedQuestion.Category, result.Category);
            Assert.Equal(expectedQuestion.Visibility, result.Visibility);
            Assert.Equal(expectedQuestion.Type, result.Type);
            Assert.Equal(expectedQuestion.Author, result.Author);
            Assert.Equal(expectedQuestion.Unit, result.Unit);
        }

        [Fact]
        public async Task AddQuestion_ShouldPassCorrectParametersToRepository()
        {
            // Arrange
            var category = new Category("Geography");
            var libelle = "Test Libelle";
            var response = "100";
            var visibility = VisibilityQuestion.Minigame;
            var type = TypeQuestion.PileDansLeMille;
            var author = "Test Author";
            var unit = "m";

            Question capturedQuestion = null;

            _questionRepositoryMock.Setup(r => r.InsertAsync(It.IsAny<Question>()))
                .Callback<Question>(q => capturedQuestion = q)
                .ReturnsAsync((Question q) => q);

            // Act
            await _service.AddQuestion(libelle, response, category, visibility, type, author, unit);

            // Assert
            Assert.NotNull(capturedQuestion);
            Assert.Equal(libelle, capturedQuestion.Libelle);
            Assert.Equal(response, capturedQuestion.Response);
            Assert.Equal(category, capturedQuestion.Category);
            Assert.Equal(visibility, capturedQuestion.Visibility);
            Assert.Equal(type, capturedQuestion.Type);
            Assert.Equal(author, capturedQuestion.Author);
            Assert.Equal(unit, capturedQuestion.Unit);
        }

        [Fact]
        public async Task AddQuestion_WithMinimalFields_ShouldCallRepositoryInsertAsync()
        {
            // Arrange
            var category = new Category("Minimal");
            var question = new Question(
                "Minimal Question",
                "0",
                category,
                VisibilityQuestion.Public,
                TypeQuestion.Standard,
                null,
                null
            );

            _questionRepositoryMock.Setup(r => r.InsertAsync(It.IsAny<Question>()))
                .ReturnsAsync(question);

            // Act
            await _service.AddQuestion(
                "Minimal Question",
                "0",
                category,
                VisibilityQuestion.Public,
                TypeQuestion.Standard,
                null, // Author optionnel
                null  // Unit optionnel
            );

            // Assert
            _questionRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<Question>()), Times.Once);
        }

        [Fact]
        public async Task AddQuestion_WithNullOptionalFields_ShouldPassNullToRepository()
        {
            // Arrange
            var category = new Category("Null Fields");
            Question capturedQuestion = null;

            _questionRepositoryMock.Setup(r => r.InsertAsync(It.IsAny<Question>()))
                .Callback<Question>(q => capturedQuestion = q)
                .ReturnsAsync((Question q) => q);

            // Act
            await _service.AddQuestion(
                "Question without author/unit",
                "50",
                category,
                VisibilityQuestion.Public,
                TypeQuestion.Standard,
                null,
                null
            );

            // Assert
            Assert.NotNull(capturedQuestion);
            Assert.Null(capturedQuestion.Author);
            Assert.Null(capturedQuestion.Unit);
        }

        [Fact]
        public async Task AddQuestion_WithCombinedEnumFlags_ShouldCallRepositoryInsertAsync()
        {
            // Arrange
            var category = new Category("Flags");
            var visibility = VisibilityQuestion.Minigame | VisibilityQuestion.Public;
            var type = TypeQuestion.Standard | TypeQuestion.SurLaPiste;
            var question = new Question(
                "Question with flags",
                "75",
                category,
                visibility,
                type,
                "Author",
                "cm"
            );

            _questionRepositoryMock.Setup(r => r.InsertAsync(It.IsAny<Question>()))
                .ReturnsAsync(question);

            // Act
            await _service.AddQuestion(
                "Question with flags",
                "75",
                category,
                visibility,
                type,
                "Author",
                "cm"
            );

            // Assert
            _questionRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<Question>()), Times.Once);
        }

        [Fact]
        public async Task AddQuestion_WithCombinedFlags_ShouldPassCorrectFlagsToRepository()
        {
            // Arrange
            var category = new Category("Combined");
            var visibility = VisibilityQuestion.Minigame | VisibilityQuestion.Custom | VisibilityQuestion.Public;
            var type = TypeQuestion.PileDansLeMille | TypeQuestion.UnDernierCoup;
            Question capturedQuestion = null;

            _questionRepositoryMock.Setup(r => r.InsertAsync(It.IsAny<Question>()))
                .Callback<Question>(q => capturedQuestion = q)
                .ReturnsAsync((Question q) => q);

            // Act
            await _service.AddQuestion(
                "Complex flags question",
                "200",
                category,
                visibility,
                type,
                "Author",
                "kg"
            );

            // Assert
            Assert.NotNull(capturedQuestion);
            Assert.Equal(visibility, capturedQuestion.Visibility);
            Assert.Equal(type, capturedQuestion.Type);
            Assert.True(capturedQuestion.Visibility.HasFlag(VisibilityQuestion.Minigame));
            Assert.True(capturedQuestion.Visibility.HasFlag(VisibilityQuestion.Custom));
            Assert.True(capturedQuestion.Visibility.HasFlag(VisibilityQuestion.Public));
            Assert.True(capturedQuestion.Type.HasFlag(TypeQuestion.PileDansLeMille));
            Assert.True(capturedQuestion.Type.HasFlag(TypeQuestion.UnDernierCoup));
        }

        [Theory]
        [InlineData(VisibilityQuestion.Minigame, TypeQuestion.PileDansLeMille)]
        [InlineData(VisibilityQuestion.Custom, TypeQuestion.Standard)]
        [InlineData(VisibilityQuestion.Public, TypeQuestion.Standard)]
        [InlineData(VisibilityQuestion.Minigame | VisibilityQuestion.Public, TypeQuestion.UnDernierCoup |TypeQuestion.Standard)]
        public async Task AddQuestion_WithDifferentEnumCombinations_ShouldCallRepositoryInsertAsync(
            VisibilityQuestion visibility,
            TypeQuestion type)
        {
            // Arrange
            var category = new Category($"Category_{visibility}_{type}");
            var question = new Question(
                $"Question {visibility} {type}",
                "50",
                category,
                visibility,
                type,
                null,
                null
            );

            _questionRepositoryMock.Setup(r => r.InsertAsync(It.IsAny<Question>()))
                .ReturnsAsync(question);

            // Act
            await _service.AddQuestion(
                $"Question {visibility} {type}",
                "50",
                category,
                visibility,
                type,
                null,
                null
            );

            // Assert
            _questionRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<Question>()), Times.Once);
        }

        [Fact]
        public async Task AddQuestion_WithSpecialCharacters_ShouldCallRepositoryInsertAsync()
        {
            // Arrange
            var category = new Category("Catégorie spéciale");
            var libelle = "Quelle est la température à l'équateur ? 🌡️";
            var author = "Auteur Ünîçödé";
            var unit = "°C";
            var question = new Question(
                libelle,
                "30",
                category,
                VisibilityQuestion.Public,
                TypeQuestion.Standard,
                author,
                unit
            );

            _questionRepositoryMock.Setup(r => r.InsertAsync(It.IsAny<Question>()))
                .ReturnsAsync(question);

            // Act
            await _service.AddQuestion(libelle, "30", category, VisibilityQuestion.Public, TypeQuestion.Standard, author, unit);

            // Assert
            _questionRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<Question>()), Times.Once);
        }

        [Theory]
        [InlineData("0")]
        [InlineData("999999")]
        [InlineData("2147483647")] // int.MaxValue
        public async Task AddQuestion_WithDifferentResponseValues_ShouldCallRepositoryInsertAsync(string response)
        {
            // Arrange
            var category = new Category($"Category_{response}");
            var question = new Question(
                $"Question with response {response}",
                response,
                category,
                VisibilityQuestion.Public,
                TypeQuestion.Standard,
                null,
                null
            );

            _questionRepositoryMock.Setup(r => r.InsertAsync(It.IsAny<Question>()))
                .ReturnsAsync(question);

            // Act
            await _service.AddQuestion(
                $"Question with response {response}",
                response,
                category,
                VisibilityQuestion.Public,
                TypeQuestion.Standard,
                null,
                null
            );

            // Assert
            _questionRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<Question>()), Times.Once);
        }

        [Fact]
        public async Task AddQuestion_ShouldCreateNewQuestionInstanceEachTime()
        {
            // Arrange
            var category = new Category("Same Category");
            var capturedQuestions = new List<Question>();

            _questionRepositoryMock.Setup(r => r.InsertAsync(It.IsAny<Question>()))
                .Callback<Question>(q => capturedQuestions.Add(q))
                .ReturnsAsync((Question q) => q);

            // Act
            await _service.AddQuestion("Question 1", "10", category, VisibilityQuestion.Public, TypeQuestion.Standard, null, null);
            await _service.AddQuestion("Question 2", "20", category, VisibilityQuestion.Public, TypeQuestion.Standard, null, null);

            // Assert
            Assert.Equal(2, capturedQuestions.Count);
            Assert.NotSame(capturedQuestions[0], capturedQuestions[1]); // Différentes instances
            Assert.Equal("Question 1", capturedQuestions[0].Libelle);
            Assert.Equal("Question 2", capturedQuestions[1].Libelle);
            _questionRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<Question>()), Times.Exactly(2));
        }

        [Fact]
        public async Task AddQuestion_WithLongLibelle_ShouldCallRepositoryInsertAsync()
        {
            // Arrange
            var category = new Category("Long Text");
            var longLibelle = new string('A', 1000);
            var question = new Question(
                longLibelle,
                "42",
                category,
                VisibilityQuestion.Public,
                TypeQuestion.Standard,
                null,
                null
            );

            _questionRepositoryMock.Setup(r => r.InsertAsync(It.IsAny<Question>()))
                .ReturnsAsync(question);

            // Act
            await _service.AddQuestion(longLibelle, "42", category, VisibilityQuestion.Public, TypeQuestion.Standard, null, null);

            // Assert
            _questionRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<Question>()), Times.Once);
        }

        [Fact]
        public async Task AddQuestion_ShouldNotModifyCategoryParameter()
        {
            // Arrange
            var category = new Category("Immutable Category");
            var originalName = category.Name;

            _questionRepositoryMock.Setup(r => r.InsertAsync(It.IsAny<Question>()))
                .ReturnsAsync((Question q) => q);

            // Act
            await _service.AddQuestion(
                "Test Question",
                "42",
                category,
                VisibilityQuestion.Public,
                TypeQuestion.Standard,
                null,
                null
            );

            // Assert
            Assert.Equal(originalName, category.Name); // La catégorie n'a pas été modifiée
            _questionRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<Question>()), Times.Once);
        }
    }
}
