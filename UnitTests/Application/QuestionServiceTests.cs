using Application.Exceptions;
using Application.Interfaces.Repository;
using Application.Services;
using Domain.Enums;
using Domain.Party;
using Moq;
using Shared;
using Shared.Filters;

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

        #region Add Question
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

        #endregion

        #region Search Questions

        [Fact]
        public async Task Search_ShouldCallRepositorySearchQuestion()
        {
            // Arrange
            int pageIndex = 1;
            int pageSize = 10;
            var filterOption = new QuestionFilter
            {
                Libelle = "Test",
                Visibility = 1,
                Type = 1,
                Categories = "Science",
                Author = "Test Author"
            };

            var expectedSkip = 10; // pageIndex 1 * pageSize 10 = 10
            var pagedResult = new PagedResult<Question>
            {
                Data = new List<Question>(),
                TotalCount = 0
            };

            _questionRepositoryMock.Setup(r => r.SearchQuestion(expectedSkip, pageSize, filterOption))
                .ReturnsAsync(pagedResult);

            // Act
            await _service.Search(pageIndex, pageSize, filterOption);

            // Assert
            _questionRepositoryMock.Verify(r => r.SearchQuestion(expectedSkip, pageSize, filterOption), Times.Once);
        }

        [Fact]
        public async Task Search_ShouldReturnPagedResultFromRepository()
        {
            // Arrange
            int pageIndex = 2;
            int pageSize = 5;
            var filterOption = new QuestionFilter
            {
                Libelle = "Capitale",
                Visibility = (int)VisibilityQuestion.Public,
                Type = (int)TypeQuestion.Standard,
                Categories = "Geography",
                Author = "John Doe"
            };

            var category = new Category("Geography");
            var expectedQuestions = new List<Question>
    {
        new Question("Question 1", "1", category, VisibilityQuestion.Public, TypeQuestion.Standard, "John Doe", "km"),
        new Question("Question 2", "2", category, VisibilityQuestion.Public, TypeQuestion.Standard, "John Doe", "km")
    };

            var expectedSkip = 10; // pageIndex 2 * pageSize 5 = 10
            var expectedPagedResult = new PagedResult<Question>
            {
                Data = expectedQuestions,
                TotalCount = 15
            };

            _questionRepositoryMock.Setup(r => r.SearchQuestion(expectedSkip, pageSize, filterOption))
                .ReturnsAsync(expectedPagedResult);

            // Act
            var result = await _service.Search(pageIndex, pageSize, filterOption);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedPagedResult.TotalCount, result.TotalCount);
            Assert.Equal(expectedPagedResult.Data.Count(), result.Data.Count());
            Assert.Equal(expectedQuestions[0].Libelle, result.Data.First().Libelle);
        }

        [Fact]
        public async Task Search_WithPageIndexZero_ShouldCalculateSkipAsZero()
        {
            // Arrange
            int pageIndex = 0;
            int pageSize = 10;
            var filterOption = new QuestionFilter();

            var expectedSkip = 0; // pageIndex 0 * pageSize 10 = 0
            var pagedResult = new PagedResult<Question>
            {
                Data = new List<Question>(),
                TotalCount = 0
            };

            _questionRepositoryMock.Setup(r => r.SearchQuestion(expectedSkip, pageSize, It.IsAny<QuestionFilter>()))
                .ReturnsAsync(pagedResult);

            // Act
            await _service.Search(pageIndex, pageSize, filterOption);

            // Assert
            _questionRepositoryMock.Verify(r => r.SearchQuestion(expectedSkip, pageSize, It.IsAny<QuestionFilter>()), Times.Once);
        }

        [Fact]
        public async Task Search_WithNegativePageIndex_ShouldCalculateSkipAsZero()
        {
            // Arrange
            int pageIndex = -5;
            int pageSize = 10;
            var filterOption = new QuestionFilter();

            var expectedSkip = 0; // pageIndex négatif -> skip = 0
            var pagedResult = new PagedResult<Question>
            {
                Data = new List<Question>(),
                TotalCount = 0
            };

            _questionRepositoryMock.Setup(r => r.SearchQuestion(expectedSkip, pageSize, It.IsAny<QuestionFilter>()))
                .ReturnsAsync(pagedResult);

            // Act
            await _service.Search(pageIndex, pageSize, filterOption);

            // Assert
            _questionRepositoryMock.Verify(r => r.SearchQuestion(expectedSkip, pageSize, It.IsAny<QuestionFilter>()), Times.Once);
        }

        [Fact]
        public async Task Search_ShouldCalculateCorrectSkipValue()
        {
            // Arrange
            int pageIndex = 3;
            int pageSize = 20;
            var filterOption = new QuestionFilter();

            var expectedSkip = 60; // pageIndex 3 * pageSize 20 = 60
            var pagedResult = new PagedResult<Question>
            {
                Data = new List<Question>(),
                TotalCount = 0
            };

            _questionRepositoryMock.Setup(r => r.SearchQuestion(expectedSkip, pageSize, It.IsAny<QuestionFilter>()))
                .ReturnsAsync(pagedResult);

            // Act
            await _service.Search(pageIndex, pageSize, filterOption);

            // Assert
            _questionRepositoryMock.Verify(r => r.SearchQuestion(expectedSkip, pageSize, It.IsAny<QuestionFilter>()), Times.Once);
        }

        [Fact]
        public async Task Search_WithNullFilters_ShouldCallRepository()
        {
            // Arrange
            int pageIndex = 1;
            int pageSize = 10;
            var filterOption = new QuestionFilter
            {
                Libelle = null,
                Visibility = null,
                Type = null,
                Categories = null,
                Author = null
            };

            var expectedSkip = 10; // pageIndex 1 * pageSize 10 = 10
            var pagedResult = new PagedResult<Question>
            {
                Data = new List<Question>(),
                TotalCount = 0
            };

            _questionRepositoryMock.Setup(r => r.SearchQuestion(expectedSkip, pageSize, filterOption))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _service.Search(pageIndex, pageSize, filterOption);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Data);
            _questionRepositoryMock.Verify(r => r.SearchQuestion(expectedSkip, pageSize, filterOption), Times.Once);
        }

        [Fact]
        public async Task Search_WithEmptyResult_ShouldReturnEmptyPagedResult()
        {
            // Arrange
            int pageIndex = 0;
            int pageSize = 10;
            var filterOption = new QuestionFilter { Libelle = "NonExistent" };

            var expectedSkip = 0; // pageIndex 0 * pageSize 10 = 0
            var emptyPagedResult = new PagedResult<Question>
            {
                Data = new List<Question>(),
                TotalCount = 0
            };

            _questionRepositoryMock.Setup(r => r.SearchQuestion(expectedSkip, pageSize, filterOption))
                .ReturnsAsync(emptyPagedResult);

            // Act
            var result = await _service.Search(pageIndex, pageSize, filterOption);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result.Data);
            Assert.Equal(0, result.TotalCount);
        }

        [Fact]
        public async Task Search_WithMultipleResults_ShouldReturnAllResults()
        {
            // Arrange
            int pageIndex = 0;
            int pageSize = 3;
            var filterOption = new QuestionFilter { Categories = "Science" };

            var category = new Category("Science");
            var questions = new List<Question>
            {
                new Question("Q1", "1", category, VisibilityQuestion.Public, TypeQuestion.Standard, "Author1", "km"),
                new Question("Q2", "2", category, VisibilityQuestion.Public, TypeQuestion.Standard, "Author2", "m"),
                new Question("Q3", "3", category, VisibilityQuestion.Minigame, TypeQuestion.PileDansLeMille, "Author3", "g")
            };

            var expectedSkip = 0;
            var pagedResult = new PagedResult<Question>
            {
                Data = questions,
                TotalCount = 10
            };

            _questionRepositoryMock.Setup(r => r.SearchQuestion(expectedSkip, pageSize, filterOption))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _service.Search(pageIndex, pageSize, filterOption);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Data.Count());
            Assert.Equal(10, result.TotalCount);
            Assert.Equal("Q1", result.Data.First().Libelle);
            Assert.Equal("Q3", result.Data.Last().Libelle);
        }

        [Fact]
        public async Task Search_WithAllFilterProperties_ShouldPassCorrectFilters()
        {
            // Arrange
            int pageIndex = 0;
            int pageSize = 5;
            var filterOption = new QuestionFilter
            {
                Libelle = "Test Question",
                Visibility = (int)VisibilityQuestion.Public,
                Type = (int)TypeQuestion.Standard,
                Categories = "History,Science",
                Author = "Jane Doe"
            };

            var expectedSkip = 0;
            var pagedResult = new PagedResult<Question>
            {
                Data = new List<Question>(),
                TotalCount = 0
            };

            _questionRepositoryMock.Setup(r => r.SearchQuestion(expectedSkip, pageSize, filterOption))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _service.Search(pageIndex, pageSize, filterOption);

            // Assert
            Assert.NotNull(result);
            _questionRepositoryMock.Verify(r => r.SearchQuestion(
                expectedSkip,
                pageSize,
                It.Is<QuestionFilter>(f =>
                    f.Libelle == "Test Question" &&
                    f.Visibility == (int)VisibilityQuestion.Public &&
                    f.Type == (int)TypeQuestion.Standard &&
                    f.Categories == "History,Science" &&
                    f.Author == "Jane Doe"
                )),
                Times.Once);
        }

        #endregion

        #region GetById Question

        [Fact]
        public async Task GetById_WithValidId_ShouldReturnQuestion()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            var category = new Category("Test Category");
            var expectedQuestion = new Question(
                "Test Question",
                "42",
                category,
                VisibilityQuestion.Public,
                TypeQuestion.Standard,
                "Test Author",
                "km"
            );

            _questionRepositoryMock.Setup(r => r.GetByIdAsync(questionId))
                .ReturnsAsync(expectedQuestion);

            // Act
            var result = await _service.GetById(questionId);

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
        public async Task GetById_ShouldCallRepositoryGetByIdAsync()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            var category = new Category("Test Category");
            var question = new Question(
                "Test Question",
                "100",
                category,
                VisibilityQuestion.Public,
                TypeQuestion.Standard,
                null,
                null
            );

            _questionRepositoryMock.Setup(r => r.GetByIdAsync(questionId))
                .ReturnsAsync(question);

            // Act
            await _service.GetById(questionId);

            // Assert
            _questionRepositoryMock.Verify(r => r.GetByIdAsync(questionId), Times.Once);
        }

        [Fact]
        public async Task GetById_ShouldPassCorrectIdToRepository()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            var category = new Category("Test Category");
            var question = new Question(
                "Test Question",
                "200",
                category,
                VisibilityQuestion.Minigame,
                TypeQuestion.PileDansLeMille,
                "Author",
                "m"
            );

            Guid capturedId = Guid.Empty;
            _questionRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .Callback<Guid>(id => capturedId = id)
                .ReturnsAsync(question);

            // Act
            await _service.GetById(questionId);

            // Assert
            Assert.Equal(questionId, capturedId);
        }

        [Fact]
        public async Task GetById_WhenQuestionNotFound_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            _questionRepositoryMock.Setup(r => r.GetByIdAsync(questionId))
                .ReturnsAsync((Question?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
                () => _service.GetById(questionId)
            );

            Assert.NotNull(exception);
            Assert.Contains(questionId.ToString(), exception.Message);
        }

        [Fact]
        public async Task GetById_WithMinimalFields_ShouldReturnQuestion()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            var category = new Category("Minimal Category");
            var expectedQuestion = new Question(
                "Minimal Question",
                "0",
                category,
                VisibilityQuestion.Public,
                TypeQuestion.Standard,
                null, // No author
                null  // No unit
            );

            _questionRepositoryMock.Setup(r => r.GetByIdAsync(questionId))
                .ReturnsAsync(expectedQuestion);

            // Act
            var result = await _service.GetById(questionId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedQuestion.Libelle, result.Libelle);
            Assert.Null(result.Author);
            Assert.Null(result.Unit);
        }

        [Fact]
        public async Task GetById_WithCombinedEnumFlags_ShouldReturnQuestionWithCorrectFlags()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            var category = new Category("Flags Category");
            var expectedQuestion = new Question(
                "Question with flags",
                "50",
                category,
                VisibilityQuestion.Minigame | VisibilityQuestion.Public,
                TypeQuestion.Standard | TypeQuestion.PileDansLeMille,
                "Author",
                "m"
            );

            _questionRepositoryMock.Setup(r => r.GetByIdAsync(questionId))
                .ReturnsAsync(expectedQuestion);

            // Act
            var result = await _service.GetById(questionId);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Visibility.HasFlag(VisibilityQuestion.Minigame));
            Assert.True(result.Visibility.HasFlag(VisibilityQuestion.Public));
            Assert.True(result.Type.HasFlag(TypeQuestion.Standard));
            Assert.True(result.Type.HasFlag(TypeQuestion.PileDansLeMille));
        }

        [Fact]
        public async Task GetById_ShouldReturnQuestionWithCategory()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            var category = new Category("Geography");
            var expectedQuestion = new Question(
                "What is the capital of France?",
                "2161000",
                category,
                VisibilityQuestion.Public,
                TypeQuestion.Standard,
                "Geography Expert",
                "population"
            );

            _questionRepositoryMock.Setup(r => r.GetByIdAsync(questionId))
                .ReturnsAsync(expectedQuestion);

            // Act
            var result = await _service.GetById(questionId);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Category);
            Assert.Equal(category.Name, result.Category.Name);
        }

        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000000")] // Guid.Empty
        [InlineData("12345678-1234-1234-1234-123456789012")]
        [InlineData("ffffffff-ffff-ffff-ffff-ffffffffffff")]
        public async Task GetById_WithDifferentGuidFormats_WhenNotFound_ShouldThrowEntityNotFoundException(string guidString)
        {
            // Arrange
            var questionId = Guid.Parse(guidString);
            _questionRepositoryMock.Setup(r => r.GetByIdAsync(questionId))
                .ReturnsAsync((Question?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
                () => _service.GetById(questionId)
            );

            Assert.NotNull(exception);
            Assert.Contains(questionId.ToString(), exception.Message);
        }

        [Fact]
        public async Task GetById_WithSpecialCharacters_ShouldReturnQuestion()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            var category = new Category("Catégorie spéciale");
            var expectedQuestion = new Question(
                "Quelle est la température à l'équateur ? 🌡️",
                "30",
                category,
                VisibilityQuestion.Public,
                TypeQuestion.Standard,
                "Auteur Ünîçödé",
                "°C"
            );

            _questionRepositoryMock.Setup(r => r.GetByIdAsync(questionId))
                .ReturnsAsync(expectedQuestion);

            // Act
            var result = await _service.GetById(questionId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedQuestion.Libelle, result.Libelle);
            Assert.Equal(expectedQuestion.Author, result.Author);
            Assert.Equal(expectedQuestion.Unit, result.Unit);
        }

        [Fact]
        public async Task GetById_MultipleCalls_ShouldCallRepositoryForEach()
        {
            // Arrange
            var questionId1 = Guid.NewGuid();
            var questionId2 = Guid.NewGuid();
            var category = new Category("Test Category");
            var question1 = new Question("Question 1", "10", category, VisibilityQuestion.Public, TypeQuestion.Standard, null, null);
            var question2 = new Question("Question 2", "20", category, VisibilityQuestion.Public, TypeQuestion.Standard, null, null);

            _questionRepositoryMock.Setup(r => r.GetByIdAsync(questionId1))
                .ReturnsAsync(question1);
            _questionRepositoryMock.Setup(r => r.GetByIdAsync(questionId2))
                .ReturnsAsync(question2);

            // Act
            await _service.GetById(questionId1);
            await _service.GetById(questionId2);

            // Assert
            _questionRepositoryMock.Verify(r => r.GetByIdAsync(questionId1), Times.Once);
            _questionRepositoryMock.Verify(r => r.GetByIdAsync(questionId2), Times.Once);
        }

        #endregion

        #region Delete Question

        [Fact]
        public async Task DeleteQuestion_WithValidId_ShouldCallRepositoryGetByIdAsync()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            var category = new Category("Test Category");
            var question = new Question(
                "Test Question",
                "42",
                category,
                VisibilityQuestion.Public,
                TypeQuestion.Standard,
                null,
                null
            );

            _questionRepositoryMock.Setup(r => r.GetByIdAsync(questionId))
                .ReturnsAsync(question);

            // Act
            await _service.DeleteQuestion(questionId);

            // Assert
            _questionRepositoryMock.Verify(r => r.GetByIdAsync(questionId), Times.Once);
        }

        [Fact]
        public async Task DeleteQuestion_WithValidId_ShouldCallRepositoryDelete()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            var category = new Category("Test Category");
            var question = new Question(
                "Test Question",
                "42",
                category,
                VisibilityQuestion.Public,
                TypeQuestion.Standard,
                null,
                null
            );

            _questionRepositoryMock.Setup(r => r.GetByIdAsync(questionId))
                .ReturnsAsync(question);

            // Act
            await _service.DeleteQuestion(questionId);

            // Assert
            _questionRepositoryMock.Verify(r => r.Delete(questionId), Times.Once);
        }

        [Fact]
        public async Task DeleteQuestion_WithValidId_ShouldPassCorrectIdToGetByIdAsync()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            var category = new Category("Test Category");
            var question = new Question(
                "Test Question",
                "100",
                category,
                VisibilityQuestion.Public,
                TypeQuestion.Standard,
                "Author",
                "km"
            );

            Guid capturedId = Guid.Empty;
            _questionRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .Callback<Guid>(id => capturedId = id)
                .ReturnsAsync(question);

            // Act
            await _service.DeleteQuestion(questionId);

            // Assert
            Assert.Equal(questionId, capturedId);
        }

        [Fact]
        public async Task DeleteQuestion_WithValidId_ShouldPassCorrectIdToDelete()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            var category = new Category("Test Category");
            var question = new Question(
                "Test Question",
                "200",
                category,
                VisibilityQuestion.Minigame,
                TypeQuestion.PileDansLeMille,
                "Test Author",
                "m"
            );

            Guid capturedDeleteId = Guid.Empty;
            _questionRepositoryMock.Setup(r => r.GetByIdAsync(questionId))
                .ReturnsAsync(question);
            _questionRepositoryMock.Setup(r => r.Delete(It.IsAny<Guid>()))
                .Callback<Guid>(id => capturedDeleteId = id);

            // Act
            await _service.DeleteQuestion(questionId);

            // Assert
            Assert.Equal(questionId, capturedDeleteId);
        }

        [Fact]
        public async Task DeleteQuestion_WhenQuestionNotFound_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            _questionRepositoryMock.Setup(r => r.GetByIdAsync(questionId))
                .ReturnsAsync((Question)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
                () => _service.DeleteQuestion(questionId)
            );

            Assert.NotNull(exception);
            Assert.Contains(questionId.ToString(), exception.Message);
        }

        [Fact]
        public async Task DeleteQuestion_WhenQuestionNotFound_ShouldNotCallRepositoryDelete()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            _questionRepositoryMock.Setup(r => r.GetByIdAsync(questionId))
                .ReturnsAsync((Question)null);

            // Act
            try
            {
                await _service.DeleteQuestion(questionId);
            }
            catch (EntityNotFoundException)
            {
                // Expected exception
            }

            // Assert
            _questionRepositoryMock.Verify(r => r.Delete(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task DeleteQuestion_WithDifferentIds_ShouldCallRepositoryForEachId()
        {
            // Arrange
            var questionId1 = Guid.NewGuid();
            var questionId2 = Guid.NewGuid();
            var category = new Category("Test Category");
            var question1 = new Question("Question 1", "10", category, VisibilityQuestion.Public, TypeQuestion.Standard, null, null);
            var question2 = new Question("Question 2", "20", category, VisibilityQuestion.Public, TypeQuestion.Standard, null, null);

            _questionRepositoryMock.Setup(r => r.GetByIdAsync(questionId1))
                .ReturnsAsync(question1);
            _questionRepositoryMock.Setup(r => r.GetByIdAsync(questionId2))
                .ReturnsAsync(question2);

            // Act
            await _service.DeleteQuestion(questionId1);
            await _service.DeleteQuestion(questionId2);

            // Assert
            _questionRepositoryMock.Verify(r => r.GetByIdAsync(questionId1), Times.Once);
            _questionRepositoryMock.Verify(r => r.GetByIdAsync(questionId2), Times.Once);
            _questionRepositoryMock.Verify(r => r.Delete(questionId1), Times.Once);
            _questionRepositoryMock.Verify(r => r.Delete(questionId2), Times.Once);
        }

        [Fact]
        public async Task DeleteQuestion_ShouldNotModifyQuestionBeforeDelete()
        {
            // Arrange
            var questionId = Guid.NewGuid();
            var category = new Category("Original Category");
            var question = new Question(
                "Original Question",
                "50",
                category,
                VisibilityQuestion.Public,
                TypeQuestion.Standard,
                "Original Author",
                "km"
            );

            var originalLibelle = question.Libelle;
            var originalResponse = question.Response;

            _questionRepositoryMock.Setup(r => r.GetByIdAsync(questionId))
                .ReturnsAsync(question);

            // Act
            await _service.DeleteQuestion(questionId);

            // Assert
            Assert.Equal(originalLibelle, question.Libelle);
            Assert.Equal(originalResponse, question.Response);
            _questionRepositoryMock.Verify(r => r.Delete(questionId), Times.Once);
        }

        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000000")] // Guid.Empty
        [InlineData("12345678-1234-1234-1234-123456789012")]
        [InlineData("ffffffff-ffff-ffff-ffff-ffffffffffff")]
        public async Task DeleteQuestion_WithDifferentGuidFormats_ShouldCallRepository(string guidString)
        {
            // Arrange
            var questionId = Guid.Parse(guidString);
            var category = new Category("Test");
            var question = new Question("Test", "10", category, VisibilityQuestion.Public, TypeQuestion.Standard, null, null);

            _questionRepositoryMock.Setup(r => r.GetByIdAsync(questionId))
                .ReturnsAsync(question);

            // Act
            await _service.DeleteQuestion(questionId);

            // Assert
            _questionRepositoryMock.Verify(r => r.GetByIdAsync(questionId), Times.Once);
            _questionRepositoryMock.Verify(r => r.Delete(questionId), Times.Once);
        }

        #endregion
    }
}
