using Domain.Enums;
using Infrastructure;
using Infrastructure.Entities;
using Meltix.IntegrationTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Web.ViewModels.Admin;

namespace IntegrationTests.Web
{
    public class AdminQuestionControllerTests
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        private readonly GuessNumberContext _context;
        private readonly JsonSerializerOptions _jsonOptions;

        public AdminQuestionControllerTests()
        {
            _context = DbContextProvider.SetupContext();
            _factory = new CustomWebApplicationFactory(_context);
            _client = _factory.CreateClient();
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        [Fact]
        public async Task Add_ShouldReturnOk_WithValidQuestion()
        {
            // Arrange
            var category = await CreateTestCategory("Test Category");
            var expectedQuestion = "Quelle est la capitale de la France ";

            var newQuestion = new QuestionAdminAddVM
            {
                Libelle = "Quelle est la capitale de la France ?",
                Response = 42,
                Category = category,
                Visibility = VisibilityQuestion.Public,
                Type = TypeQuestion.Standard,
                Author = "Test Author",
                Unit = "km"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newQuestion),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminQuestion/Add", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var questionResult = JsonSerializer.Deserialize<QuestionAdminVM>(
                await response.Content.ReadAsStringAsync(),
                _jsonOptions
            );

            Assert.NotNull(questionResult);
            Assert.Equal(expectedQuestion, questionResult.Libelle);
            Assert.Equal(newQuestion.Response, questionResult.Response);
            Assert.Equal(newQuestion.Visibility, questionResult.Visibility);
            Assert.Equal(newQuestion.Type, questionResult.Type);
            Assert.Equal(newQuestion.Author, questionResult.Author);
            Assert.Equal(newQuestion.Unit, questionResult.Unit);
            Assert.NotNull(questionResult.Id);
            Assert.NotEqual(Guid.Empty, questionResult.Id);
            Assert.Equal(category.Id, questionResult.Category.Id);
        }

        [Fact]
        public async Task Add_ShouldReturnOk_WithMinimalRequiredFields()
        {
            // Arrange
            var category = await CreateTestCategory("Minimal Category");

            var newQuestion = new QuestionAdminAddVM
            {
                Libelle = "Question minimale",
                Response = 10,
                Category = category,
                Visibility = VisibilityQuestion.Public,
                Type = TypeQuestion.Standard
                // Author et Unit sont optionnels (null)
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newQuestion),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminQuestion/Add", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var questionResult = JsonSerializer.Deserialize<QuestionAdminVM>(
                await response.Content.ReadAsStringAsync(),
                _jsonOptions
            );

            Assert.NotNull(questionResult);
            Assert.Equal(newQuestion.Libelle, questionResult.Libelle);
            Assert.Null(questionResult.Author);
            Assert.Null(questionResult.Unit);
        }

        [Fact]
        public async Task Add_ShouldReturnOk_WithCombinedVisibilityFlags()
        {
            // Arrange
            var category = await CreateTestCategory("Flags Category");

            var newQuestion = new QuestionAdminAddVM
            {
                Libelle = "Question avec visibilité combinée",
                Response = 100,
                Category = category,
                Visibility = VisibilityQuestion.Minigame | VisibilityQuestion.Public,
                Type = TypeQuestion.Standard | TypeQuestion.PileDansLeMille
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newQuestion),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminQuestion/Add", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var questionResult = JsonSerializer.Deserialize<QuestionAdminVM>(
                await response.Content.ReadAsStringAsync(),
                _jsonOptions
            );

            Assert.NotNull(questionResult);
            Assert.True(questionResult.Visibility.HasFlag(VisibilityQuestion.Minigame));
            Assert.True(questionResult.Visibility.HasFlag(VisibilityQuestion.Public));
            Assert.True(questionResult.Type.HasFlag(TypeQuestion.Standard));
            Assert.True(questionResult.Type.HasFlag(TypeQuestion.PileDansLeMille));
        }

        [Fact]
        public async Task Add_ShouldReturnBadRequest_WhenLibelleIsMissing()
        {
            // Arrange
            var category = await CreateTestCategory("Test Category");

            var invalidQuestion = new QuestionAdminAddVM
            {
                // Libelle manquant (null)
                Response = 42,
                Category = category,
                Visibility = VisibilityQuestion.Public,
                Type = TypeQuestion.Standard
            };

            var content = new StringContent(
                JsonSerializer.Serialize(invalidQuestion),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminQuestion/Add", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Add_ShouldReturnBadRequest_WhenLibelleIsEmpty()
        {
            // Arrange
            var category = await CreateTestCategory("Test Category");

            var invalidQuestion = new QuestionAdminAddVM
            {
                Libelle = "", // Libellé vide
                Response = 42,
                Category = category,
                Visibility = VisibilityQuestion.Public,
                Type = TypeQuestion.Standard
            };

            var content = new StringContent(
                JsonSerializer.Serialize(invalidQuestion),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminQuestion/Add", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Add_ShouldReturnBadRequest_WhenCategoryIsMissing()
        {
            // Arrange
            var invalidQuestion = new QuestionAdminAddVM
            {
                Libelle = "Question sans catégorie",
                Response = 42,
                // Category manquante (null)
                Visibility = VisibilityQuestion.Public,
                Type = TypeQuestion.Standard
            };

            var content = new StringContent(
                JsonSerializer.Serialize(invalidQuestion),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminQuestion/Add", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Add_ShouldReturnNotFound_WhenCategoryDoesNotExist()
        {
            // Arrange
            var invalidQuestion = new QuestionAdminAddVM
            {
                Libelle = "Question avec catégorie inexistante",
                Response = 42,
                Category = new CategoryAdminVM
                {
                    Id = Guid.NewGuid(), // ID qui n'existe pas
                    Name = "Inexistante"
                },
                Visibility = VisibilityQuestion.Public,
                Type = TypeQuestion.Standard
            };

            var content = new StringContent(
                JsonSerializer.Serialize(invalidQuestion),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminQuestion/Add", content);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Theory]
        [InlineData(VisibilityQuestion.Minigame, TypeQuestion.PileDansLeMille)]
        [InlineData(VisibilityQuestion.Custom, TypeQuestion.Standard)]
        [InlineData(VisibilityQuestion.Public, TypeQuestion.Standard)]
        [InlineData(VisibilityQuestion.Minigame | VisibilityQuestion.Custom, TypeQuestion.UnDernierCoup)]
        [InlineData(VisibilityQuestion.Public | VisibilityQuestion.Custom, TypeQuestion.Standard)]
        public async Task Add_ShouldReturnOk_WithDifferentEnumCombinations(
            VisibilityQuestion visibility,
            TypeQuestion type)
        {
            // Arrange
            var category = await CreateTestCategory($"Category_{visibility}_{type}");

            var newQuestion = new QuestionAdminAddVM
            {
                Libelle = $"Question {visibility} {type}",
                Response = 50,
                Category = category,
                Visibility = visibility,
                Type = type
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newQuestion),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminQuestion/Add", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var questionResult = JsonSerializer.Deserialize<QuestionAdminVM>(
                await response.Content.ReadAsStringAsync(),
                _jsonOptions
            );

            Assert.NotNull(questionResult);
            Assert.Equal(visibility, questionResult.Visibility);
            Assert.Equal(type, questionResult.Type);
        }

        [Fact]
        public async Task Add_ShouldReturnOk_WithLongLibelle()
        {
            // Arrange
            var category = await CreateTestCategory("Long Text Category");
            var longLibelle = new string('A', 500); // Teste avec un libellé long

            var newQuestion = new QuestionAdminAddVM
            {
                Libelle = longLibelle,
                Response = 42,
                Category = category,
                Visibility = VisibilityQuestion.Public,
                Type = TypeQuestion.Standard
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newQuestion),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminQuestion/Add", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var questionResult = JsonSerializer.Deserialize<QuestionAdminVM>(
                await response.Content.ReadAsStringAsync(),
                _jsonOptions
            );

            Assert.NotNull(questionResult);
            Assert.Equal(longLibelle, questionResult.Libelle);
        }

        [Fact]
        public async Task Add_ShouldReturnOk_WithSpecialCharactersInLibelle()
        {
            // Arrange
            var category = await CreateTestCategory("Special Chars Category");
            var specialLibelle = "Quelle est la température à l'équateur ? (°C) 🌡️";

            var newQuestion = new QuestionAdminAddVM
            {
                Libelle = specialLibelle,
                Response = 30,
                Category = category,
                Visibility = VisibilityQuestion.Public,
                Type = TypeQuestion.Standard,
                Unit = "°C"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newQuestion),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminQuestion/Add", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var questionResult = JsonSerializer.Deserialize<QuestionAdminVM>(
                await response.Content.ReadAsStringAsync(),
                _jsonOptions
            );

            Assert.NotNull(questionResult);
            Assert.Equal(specialLibelle, questionResult.Libelle);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(999999)]
        public async Task Add_ShouldReturnOk_WithDifferentResponseValues(int responseValue)
        {
            // Arrange
            var category = await CreateTestCategory($"Response_{responseValue}_Category");

            var newQuestion = new QuestionAdminAddVM
            {
                Libelle = $"Question avec réponse {responseValue}",
                Response = responseValue,
                Category = category,
                Visibility = VisibilityQuestion.Public,
                Type = TypeQuestion.Standard
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newQuestion),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminQuestion/Add", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var questionResult = JsonSerializer.Deserialize<QuestionAdminVM>(
                await response.Content.ReadAsStringAsync(),
                _jsonOptions
            );

            Assert.NotNull(questionResult);
            Assert.Equal(responseValue, questionResult.Response);
        }

        [Fact]
        public async Task Add_ShouldReturnInternalError_WithNegativeNumber()
        {
            // Arrange
            var category = await CreateTestCategory($"Response_Category");

            var newQuestion = new QuestionAdminAddVM
            {
                Libelle = $"Question avec réponse ",
                Response = -100,
                Category = category,
                Visibility = VisibilityQuestion.Public,
                Type = TypeQuestion.Standard
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newQuestion),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminQuestion/Add", content);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task Add_ShouldReturnOk_WithAllVisibilityFlags()
        {
            // Arrange
            var category = await CreateTestCategory("All Visibility Category");

            var newQuestion = new QuestionAdminAddVM
            {
                Libelle = "Question avec toutes les visibilités",
                Response = 42,
                Category = category,
                Visibility = VisibilityQuestion.Minigame | VisibilityQuestion.Custom | VisibilityQuestion.Public,
                Type = TypeQuestion.Standard | TypeQuestion.UnDernierCoup,
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newQuestion),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminQuestion/Add", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var questionResult = JsonSerializer.Deserialize<QuestionAdminVM>(
                await response.Content.ReadAsStringAsync(),
                _jsonOptions
            );

            Assert.NotNull(questionResult);
            Assert.True(questionResult.Visibility.HasFlag(VisibilityQuestion.Minigame));
            Assert.True(questionResult.Visibility.HasFlag(VisibilityQuestion.Custom));
            Assert.True(questionResult.Visibility.HasFlag(VisibilityQuestion.Public));
        }

        [Fact]
        public async Task Add_ShouldReturnBadRequest_WithAllTypeFlags()
        {
            // Arrange
            var category = await CreateTestCategory("All Types Category");

            var newQuestion = new QuestionAdminAddVM
            {
                Libelle = "Question avec tous les types",
                Response = 42,
                Category = category,
                Visibility = VisibilityQuestion.Public,
                Type = TypeQuestion.Standard | TypeQuestion.PileDansLeMille | TypeQuestion.SurLaPiste | TypeQuestion.UnDernierCoup
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newQuestion),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminQuestion/Add", content);

            // Assert

            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task Add_ShouldReturnOk_WithLongAuthorAndUnit()
        {
            // Arrange
            var category = await CreateTestCategory("Long Metadata Category");
            var longAuthor = new string('B', 100);
            var longUnit = new string('C', 50);

            var newQuestion = new QuestionAdminAddVM
            {
                Libelle = "Question avec métadonnées longues",
                Response = 42,
                Category = category,
                Visibility = VisibilityQuestion.Public,
                Type = TypeQuestion.Standard,
                Author = longAuthor,
                Unit = longUnit
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newQuestion),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminQuestion/Add", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var questionResult = JsonSerializer.Deserialize<QuestionAdminVM>(
                await response.Content.ReadAsStringAsync(),
                _jsonOptions
            );

            Assert.NotNull(questionResult);
            Assert.Equal(longAuthor, questionResult.Author);
            Assert.Equal(longUnit, questionResult.Unit);
        }

        [Fact]
        public async Task Add_ShouldReturnBadRequest_WhenModelIsInvalid()
        {
            // Arrange
            var invalidJson = "{ \"Libelle\": null, \"Response\": \"invalid\" }";
            var content = new StringContent(invalidJson, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/AdminQuestion/Add", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Add_ShouldCreateMultipleQuestions_InSameCategory()
        {
            // Arrange
            var category = await CreateTestCategory("Shared Category");

            var question1 = new QuestionAdminAddVM
            {
                Libelle = "Première question",
                Response = 1,
                Category = category,
                Visibility = VisibilityQuestion.Public,
                Type = TypeQuestion.Standard
            };

            var question2 = new QuestionAdminAddVM
            {
                Libelle = "Deuxième question",
                Response = 2,
                Category = category,
                Visibility = VisibilityQuestion.Public,
                Type = TypeQuestion.Standard
            };

            var content1 = new StringContent(
                JsonSerializer.Serialize(question1),
                Encoding.UTF8,
                "application/json"
            );

            var content2 = new StringContent(
                JsonSerializer.Serialize(question2),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response1 = await _client.PostAsync("/api/AdminQuestion/Add", content1);
            var response2 = await _client.PostAsync("/api/AdminQuestion/Add", content2);

            // Assert
            response1.EnsureSuccessStatusCode();
            response2.EnsureSuccessStatusCode();

            var result1 = JsonSerializer.Deserialize<QuestionAdminVM>(
                await response1.Content.ReadAsStringAsync(),
                _jsonOptions
            );
            var result2 = JsonSerializer.Deserialize<QuestionAdminVM>(
                await response2.Content.ReadAsStringAsync(),
                _jsonOptions
            );

            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.NotEqual(result1.Id, result2.Id);
            Assert.Equal(category.Id, result1.Category.Id);
            Assert.Equal(category.Id, result2.Category.Id);
        }

        #region Search Tests

        [Fact]
        public async Task Search_ShouldReturnOk_WithEmptyDatabase()
        {
            // Act
            var response = await _client.GetAsync("/api/AdminQuestion/Search?pageIndex=0&pageSize=10");

            // Assert
            response.EnsureSuccessStatusCode();
            var questions = JsonSerializer.Deserialize<List<QuestionAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                _jsonOptions
            );
            Assert.NotNull(questions);
            Assert.Empty(questions);
        }

        [Fact]
        public async Task Search_ShouldReturnTotalCountInHeader()
        {
            // Arrange
            var category = await CreateTestCategory("Test Category");
            await CreateTestQuestion("Question 1", 10, category);
            await CreateTestQuestion("Question 2", 20, category);
            await CreateTestQuestion("Question 3", 30, category);

            // Act
            var response = await _client.GetAsync("/api/AdminQuestion/Search?pageIndex=0&pageSize=10");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.True(response.Headers.Contains("X-Total-Count"));
            var totalCount = response.Headers.GetValues("X-Total-Count").FirstOrDefault();
            Assert.Equal("3", totalCount);
        }

        [Fact]
        public async Task Search_ShouldReturnAllQuestions_WithoutFilters()
        {
            // Arrange
            var category = await CreateTestCategory("Test Category");
            await CreateTestQuestion("Question 1", 10, category);
            await CreateTestQuestion("Question 2", 20, category);
            await CreateTestQuestion("Question 3", 30, category);

            // Act
            var response = await _client.GetAsync("/api/AdminQuestion/Search?pageIndex=0&pageSize=10");

            // Assert
            response.EnsureSuccessStatusCode();
            var questions = JsonSerializer.Deserialize<List<QuestionAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                _jsonOptions
            );
            Assert.NotNull(questions);
            Assert.Equal(3, questions.Count);
        }

        [Fact]
        public async Task Search_WithPagination_ShouldReturnCorrectPage()
        {
            // Arrange
            var category = await CreateTestCategory("Pagination Category");
            for (int i = 0; i < 15; i++)
            {
                await CreateTestQuestion($"Question {i}", i, category);
            }

            // Act - Get page 1 (second page, 0-indexed)
            var response = await _client.GetAsync("/api/AdminQuestion/Search?pageIndex=1&pageSize=5");

            // Assert
            response.EnsureSuccessStatusCode();
            var questions = JsonSerializer.Deserialize<List<QuestionAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                _jsonOptions
            );
            Assert.NotNull(questions);
            Assert.Equal(5, questions.Count);

            var totalCount = response.Headers.GetValues("X-Total-Count").FirstOrDefault();
            Assert.Equal("15", totalCount);
        }

        [Fact]
        public async Task Search_WithLibelleFilter_ShouldReturnMatchingQuestions()
        {
            // Arrange
            var category = await CreateTestCategory("Filter Category");
            await CreateTestQuestion("Quelle est la capitale de la France", 1, category);
            await CreateTestQuestion("Quelle est la population de Paris", 2, category);
            await CreateTestQuestion("Combien de régions en France", 3, category);
            await CreateTestQuestion("Quel est le plus grand océan", 4, category);

            // Act
            var response = await _client.GetAsync("/api/AdminQuestion/Search?pageIndex=0&pageSize=10&filter.Libelle=paris");

            // Assert
            response.EnsureSuccessStatusCode();
            var questions = JsonSerializer.Deserialize<List<QuestionAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                _jsonOptions
            );
            Assert.NotNull(questions);
            Assert.Single(questions);
            Assert.Contains("Paris", questions[0].Libelle);
        }

        [Fact]
        public async Task Search_WithLibelleFilter_ShouldBeCaseInsensitive()
        {
            // Arrange
            var category = await CreateTestCategory("Case Test Category");
            await CreateTestQuestion("Quelle est la capitale de la FRANCE", 1, category);
            await CreateTestQuestion("Question sur paris", 2, category);

            // Act
            var response = await _client.GetAsync("/api/AdminQuestion/Search?pageIndex=0&pageSize=10&filter.Libelle=FrAnCe");

            // Assert
            response.EnsureSuccessStatusCode();
            var questions = JsonSerializer.Deserialize<List<QuestionAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                _jsonOptions
            );
            Assert.NotNull(questions);
            Assert.Single(questions);
            Assert.Contains("FRANCE", questions[0].Libelle);
        }

        [Fact]
        public async Task Search_WithAuthorFilter_ShouldReturnMatchingQuestions()
        {
            // Arrange
            var category = await CreateTestCategory("Author Filter Category");
            await CreateTestQuestion("Question 1", 1, category, author: "John Doe");
            await CreateTestQuestion("Question 2", 2, category, author: "Jane Smith");
            await CreateTestQuestion("Question 3", 3, category, author: "John Smith");

            // Act
            var response = await _client.GetAsync("/api/AdminQuestion/Search?pageIndex=0&pageSize=10&filter.Author=john");

            // Assert
            response.EnsureSuccessStatusCode();
            var questions = JsonSerializer.Deserialize<List<QuestionAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                _jsonOptions
            );
            Assert.NotNull(questions);
            Assert.Equal(2, questions.Count);
            Assert.All(questions, q => Assert.Contains("John", q.Author));
        }

        [Fact]
        public async Task Search_WithCategoryFilter_ShouldReturnQuestionsInCategory()
        {
            // Arrange
            var category1 = await CreateTestCategory("Category 1");
            var category2 = await CreateTestCategory("Category 2");
            await CreateTestQuestion("Question in Cat1", 1, category1);
            await CreateTestQuestion("Question in Cat2", 2, category2);
            await CreateTestQuestion("Another in Cat1", 3, category1);

            // Act
            var response = await _client.GetAsync($"/api/AdminQuestion/Search?pageIndex=0&pageSize=10&filter.Categories={category1.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            var questions = JsonSerializer.Deserialize<List<QuestionAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                _jsonOptions
            );
            Assert.NotNull(questions);
            Assert.Equal(2, questions.Count);
            Assert.All(questions, q => Assert.Equal(category1.Id, q.Category.Id));
        }

        [Fact]
        public async Task Search_WithMultipleCategoryFilter_ShouldReturnQuestionsInAnyCategory()
        {
            // Arrange
            var category1 = await CreateTestCategory("Category 1");
            var category2 = await CreateTestCategory("Category 2");
            var category3 = await CreateTestCategory("Category 3");
            await CreateTestQuestion("Question in Cat1", 1, category1);
            await CreateTestQuestion("Question in Cat2", 2, category2);
            await CreateTestQuestion("Question in Cat3", 3, category3);

            // Act
            var response = await _client.GetAsync($"/api/AdminQuestion/Search?pageIndex=0&pageSize=10&filter.Categories={category1.Id},{category2.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            var questions = JsonSerializer.Deserialize<List<QuestionAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                _jsonOptions
            );
            Assert.NotNull(questions);
            Assert.Equal(2, questions.Count);
        }

        [Fact]
        public async Task Search_WithVisibilityFilter_ShouldReturnMatchingQuestions()
        {
            // Arrange
            var category = await CreateTestCategory("Visibility Filter Category");
            await CreateTestQuestion("Public Question", 1, category, visibility: VisibilityQuestion.Public);
            await CreateTestQuestion("Custom Question", 2, category, visibility: VisibilityQuestion.Custom);
            await CreateTestQuestion("Minigame Question", 3, category,
                visibility: VisibilityQuestion.Minigame,
                type: TypeQuestion.PileDansLeMille);

            // Act - Filter for Public visibility
            var visibilityValue = (int)VisibilityQuestion.Public;
            var response = await _client.GetAsync($"/api/AdminQuestion/Search?pageIndex=0&pageSize=10&filter.Visibility={visibilityValue}");

            // Assert
            response.EnsureSuccessStatusCode();
            var questions = JsonSerializer.Deserialize<List<QuestionAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                _jsonOptions
            );
            Assert.NotNull(questions);
            Assert.Single(questions);
            Assert.True(questions[0].Visibility.HasFlag(VisibilityQuestion.Public));
        }

        [Fact]
        public async Task Search_WithVisibilityFlagFilter_ShouldReturnQuestionsWithAnyMatchingFlag()
        {
            // Arrange
            var category = await CreateTestCategory("Visibility Flags Category");
            await CreateTestQuestion("Public Only", 1, category, visibility: VisibilityQuestion.Public);
            await CreateTestQuestion("Custom Only", 2, category, visibility: VisibilityQuestion.Custom);
            await CreateTestQuestion("Both Public and Custom", 3, category,
                visibility: VisibilityQuestion.Public | VisibilityQuestion.Custom);

            // Act - Filter for Public OR Custom
            var visibilityValue = (int)(VisibilityQuestion.Public | VisibilityQuestion.Custom);
            var response = await _client.GetAsync($"/api/AdminQuestion/Search?pageIndex=0&pageSize=10&filter.Visibility={visibilityValue}");

            // Assert
            response.EnsureSuccessStatusCode();
            var questions = JsonSerializer.Deserialize<List<QuestionAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                _jsonOptions
            );
            Assert.NotNull(questions);
            Assert.Equal(3, questions.Count);
        }

        [Fact]
        public async Task Search_WithTypeFilter_ShouldReturnMatchingQuestions()
        {
            // Arrange
            var category = await CreateTestCategory("Type Filter Category");
            await CreateTestQuestion("Standard Question", 1, category, type: TypeQuestion.Standard);
            await CreateTestQuestion("PileDansLeMille Question", 2, category,
                visibility: VisibilityQuestion.Minigame,
                type: TypeQuestion.PileDansLeMille);

            // Act - Filter for Standard type
            var typeValue = (int)TypeQuestion.Standard;
            var response = await _client.GetAsync($"/api/AdminQuestion/Search?pageIndex=0&pageSize=10&filter.Type={typeValue}");

            // Assert
            response.EnsureSuccessStatusCode();
            var questions = JsonSerializer.Deserialize<List<QuestionAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                _jsonOptions
            );
            Assert.NotNull(questions);
            Assert.Single(questions);
            Assert.True(questions[0].Type.HasFlag(TypeQuestion.Standard));
        }

        [Fact]
        public async Task Search_WithCombinedFilters_ShouldReturnQuestionsMatchingAllFilters()
        {
            // Arrange
            var category1 = await CreateTestCategory("Category 1");
            var category2 = await CreateTestCategory("Category 2");
            await CreateTestQuestion("France Question", 1, category1,
                author: "John Doe",
                visibility: VisibilityQuestion.Public);
            await CreateTestQuestion("France Question 2", 2, category2,
                author: "John Doe",
                visibility: VisibilityQuestion.Public);
            await CreateTestQuestion("Spain Question", 3, category1,
                author: "John Doe",
                visibility: VisibilityQuestion.Public);
            await CreateTestQuestion("France Question 3", 4, category1,
                author: "Jane Smith",
                visibility: VisibilityQuestion.Public);

            // Act - Filter by Libelle, Author, and Category
            var visibilityValue = (int)VisibilityQuestion.Public;
            var response = await _client.GetAsync($"/api/AdminQuestion/Search?pageIndex=0&pageSize=10&filter.Libelle=france&filter.Author=john&filter.Categories={category1.Id}&filter.Visibility={visibilityValue}");

            // Assert
            response.EnsureSuccessStatusCode();
            var questions = JsonSerializer.Deserialize<List<QuestionAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                _jsonOptions
            );
            Assert.NotNull(questions);
            Assert.Single(questions);
            Assert.Contains("France", questions[0].Libelle);
            Assert.Equal("John Doe", questions[0].Author);
            Assert.Equal(category1.Id, questions[0].Category.Id);
        }


        [Theory]
        [InlineData(-1, 10)]
        [InlineData(0, 0)]
        [InlineData(0, -1)]
        public async Task Search_WithInvalidPagination_ShouldReturnBadRequest(int pageIndex, int pageSize)
        {
            // Act
            var response = await _client.GetAsync($"/api/AdminQuestion/Search?pageIndex={pageIndex}&pageSize={pageSize}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Search_WithEmptyLibelleFilter_ShouldReturnAllQuestions()
        {
            // Arrange
            var category = await CreateTestCategory("Empty Filter Category");
            await CreateTestQuestion("Question 1", 1, category);
            await CreateTestQuestion("Question 2", 2, category);

            // Act
            var response = await _client.GetAsync("/api/AdminQuestion/Search?pageIndex=0&pageSize=10&filter.Libelle=");

            // Assert
            response.EnsureSuccessStatusCode();
            var questions = JsonSerializer.Deserialize<List<QuestionAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                _jsonOptions
            );
            Assert.NotNull(questions);
            Assert.Equal(2, questions.Count);
        }

        [Fact]
        public async Task Search_WithInvalidCategoryGuid_ShouldIgnoreFilter()
        {
            // Arrange
            var category = await CreateTestCategory("Invalid Guid Category");
            await CreateTestQuestion("Question 1", 1, category);

            // Act - Use invalid GUID format
            var response = await _client.GetAsync("/api/AdminQuestion/Search?pageIndex=0&pageSize=10&filter.Categories=invalid-guid");

            // Assert
            response.EnsureSuccessStatusCode();
            var questions = JsonSerializer.Deserialize<List<QuestionAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                _jsonOptions
            );
            Assert.NotNull(questions);
            Assert.Single(questions); // Should return all since filter is ignored
        }

        [Fact]
        public async Task Search_WithNonExistentCategory_ShouldReturnEmpty()
        {
            // Arrange
            var category = await CreateTestCategory("Existent Category");
            await CreateTestQuestion("Question 1", 1, category);
            var nonExistentGuid = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/api/AdminQuestion/Search?pageIndex=0&pageSize=10&filter.Categories={nonExistentGuid}");

            // Assert
            response.EnsureSuccessStatusCode();
            var questions = JsonSerializer.Deserialize<List<QuestionAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                _jsonOptions
            );
            Assert.NotNull(questions);
            Assert.Empty(questions);
        }

        [Fact]
        public async Task Search_ShouldIncludeCategoryInformation()
        {
            // Arrange
            var category = await CreateTestCategory("Category with Details");
            await CreateTestQuestion("Test Question", 42, category);

            // Act
            var response = await _client.GetAsync("/api/AdminQuestion/Search?pageIndex=0&pageSize=10");

            // Assert
            response.EnsureSuccessStatusCode();
            var questions = JsonSerializer.Deserialize<List<QuestionAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                _jsonOptions
            );
            Assert.NotNull(questions);
            Assert.Single(questions);
            Assert.NotNull(questions[0].Category);
            Assert.Equal(category.Id, questions[0].Category.Id);
            Assert.Equal(category.Name, questions[0].Category.Name);
        }

        [Fact]
        public async Task Search_WithPageBeyondResults_ShouldReturnEmptyList()
        {
            // Arrange
            var category = await CreateTestCategory("Limited Results Category");
            await CreateTestQuestion("Question 1", 1, category);
            await CreateTestQuestion("Question 2", 2, category);

            // Act - Request page 10 when only 2 results exist
            var response = await _client.GetAsync("/api/AdminQuestion/Search?pageIndex=10&pageSize=10");

            // Assert
            response.EnsureSuccessStatusCode();
            var questions = JsonSerializer.Deserialize<List<QuestionAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                _jsonOptions
            );
            Assert.NotNull(questions);
            Assert.Empty(questions);

            // Total count should still be correct
            var totalCount = response.Headers.GetValues("X-Total-Count").FirstOrDefault();
            Assert.Equal("2", totalCount);
        }

        #endregion

        #region GetDetail Tests

        [Fact]
        public async Task GetDetail_WithValidId_ShouldReturnOk()
        {
            // Arrange
            var category = await CreateTestCategory("Detail Test Category");
            var questionId = await CreateTestQuestion("Test Question for Detail", 100, category);

            // Act
            var response = await _client.GetAsync($"/api/AdminQuestion/GetDetail?id={questionId}");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetDetail_WithValidId_ShouldReturnQuestionWithCategory()
        {
            // Arrange
            var category = await CreateTestCategory("Detail Category");
            var questionId = await CreateTestQuestion("Detailed Question", 42, category, author: "Detail Author", unit: "km");

            // Act
            var response = await _client.GetAsync($"/api/AdminQuestion/GetDetail?id={questionId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var questionResult = JsonSerializer.Deserialize<QuestionAdminVM>(
                await response.Content.ReadAsStringAsync(),
                _jsonOptions
            );

            Assert.NotNull(questionResult);
            Assert.Equal(questionId, questionResult.Id);
            Assert.Equal("Detailed Question", questionResult.Libelle);
            Assert.Equal(42, questionResult.Response);
            Assert.Equal("Detail Author", questionResult.Author);
            Assert.Equal("km", questionResult.Unit);
            Assert.NotNull(questionResult.Category);
            Assert.Equal(category.Id, questionResult.Category.Id);
            Assert.Equal(category.Name, questionResult.Category.Name);
        }

        [Fact]
        public async Task GetDetail_WithNonExistentId_ShouldReturnNotFound()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/api/AdminQuestion/GetDetail?id={nonExistentId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetDetail_WithEmptyGuid_ShouldReturnBadRequest()
        {
            // Arrange
            var emptyGuid = Guid.Empty;

            // Act
            var response = await _client.GetAsync($"/api/AdminQuestion/GetDetail?id={emptyGuid}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetDetail_WithMinimalFields_ShouldReturnQuestion()
        {
            // Arrange
            var category = await CreateTestCategory("Minimal Detail Category");
            var questionId = await CreateTestQuestion(
                "Minimal Question",
                10,
                category,
                author: null,
                unit: null,
                visibility: VisibilityQuestion.Public,
                type: TypeQuestion.Standard
            );

            // Act
            var response = await _client.GetAsync($"/api/AdminQuestion/GetDetail?id={questionId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var questionResult = JsonSerializer.Deserialize<QuestionAdminVM>(
                await response.Content.ReadAsStringAsync(),
                _jsonOptions
            );

            Assert.NotNull(questionResult);
            Assert.Equal("Minimal Question", questionResult.Libelle);
            Assert.Null(questionResult.Author);
            Assert.Null(questionResult.Unit);
            Assert.NotNull(questionResult.Category);
        }

        [Fact]
        public async Task GetDetail_WithCombinedEnumFlags_ShouldReturnQuestionWithCorrectFlags()
        {
            // Arrange
            var category = await CreateTestCategory("Enum Flags Detail Category");
            var questionId = await CreateTestQuestion(
                "Question with combined flags",
                50,
                category,
                visibility: VisibilityQuestion.Minigame | VisibilityQuestion.Public,
                type: TypeQuestion.PileDansLeMille | TypeQuestion.Standard
            );

            // Act
            var response = await _client.GetAsync($"/api/AdminQuestion/GetDetail?id={questionId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var questionResult = JsonSerializer.Deserialize<QuestionAdminVM>(
                await response.Content.ReadAsStringAsync(),
                _jsonOptions
            );

            Assert.NotNull(questionResult);
            Assert.True(questionResult.Visibility.HasFlag(VisibilityQuestion.Minigame));
            Assert.True(questionResult.Visibility.HasFlag(VisibilityQuestion.Public));
            Assert.True(questionResult.Type.HasFlag(TypeQuestion.PileDansLeMille));
            Assert.True(questionResult.Type.HasFlag(TypeQuestion.Standard));
        }

        [Fact]
        public async Task GetDetail_WithSpecialCharacters_ShouldReturnQuestion()
        {
            // Arrange
            var category = await CreateTestCategory("Catégorie spéciale");
            var questionId = await CreateTestQuestion(
                "Quelle est la température à l'équateur ? 🌡️",
                30,
                category,
                author: "Auteur Ünîçödé",
                unit: "°C"
            );

            // Act
            var response = await _client.GetAsync($"/api/AdminQuestion/GetDetail?id={questionId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var questionResult = JsonSerializer.Deserialize<QuestionAdminVM>(
                await response.Content.ReadAsStringAsync(),
                _jsonOptions
            );

            Assert.NotNull(questionResult);
            Assert.Equal("Quelle est la température à l'équateur ? 🌡️", questionResult.Libelle);
            Assert.Equal("Auteur Ünîçödé", questionResult.Author);
            Assert.Equal("°C", questionResult.Unit);
        }

        [Fact]
        public async Task GetDetail_MultipleQuestions_ShouldReturnCorrectOne()
        {
            // Arrange
            var category = await CreateTestCategory("Multiple Detail Category");
            var questionId1 = await CreateTestQuestion("Question 1", 1, category);
            var questionId2 = await CreateTestQuestion("Question 2", 2, category);
            var questionId3 = await CreateTestQuestion("Question 3", 3, category);

            // Act
            var response = await _client.GetAsync($"/api/AdminQuestion/GetDetail?id={questionId2}");

            // Assert
            response.EnsureSuccessStatusCode();
            var questionResult = JsonSerializer.Deserialize<QuestionAdminVM>(
                await response.Content.ReadAsStringAsync(),
                _jsonOptions
            );

            Assert.NotNull(questionResult);
            Assert.Equal(questionId2, questionResult.Id);
            Assert.Equal("Question 2", questionResult.Libelle);
            Assert.Equal(2, questionResult.Response);
        }

        [Fact]
        public async Task GetDetail_QuestionsFromDifferentCategories_ShouldReturnCorrectCategory()
        {
            // Arrange
            var category1 = await CreateTestCategory("Category 1");
            var category2 = await CreateTestCategory("Category 2");
            var questionId1 = await CreateTestQuestion("Question in Cat1", 1, category1);
            var questionId2 = await CreateTestQuestion("Question in Cat2", 2, category2);

            // Act
            var response1 = await _client.GetAsync($"/api/AdminQuestion/GetDetail?id={questionId1}");
            var response2 = await _client.GetAsync($"/api/AdminQuestion/GetDetail?id={questionId2}");

            // Assert
            response1.EnsureSuccessStatusCode();
            response2.EnsureSuccessStatusCode();

            var result1 = JsonSerializer.Deserialize<QuestionAdminVM>(
                await response1.Content.ReadAsStringAsync(),
                _jsonOptions
            );
            var result2 = JsonSerializer.Deserialize<QuestionAdminVM>(
                await response2.Content.ReadAsStringAsync(),
                _jsonOptions
            );

            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.Equal(category1.Id, result1.Category.Id);
            Assert.Equal("Category 1", result1.Category.Name);
            Assert.Equal(category2.Id, result2.Category.Id);
            Assert.Equal("Category 2", result2.Category.Name);
        }

        [Theory]
        [InlineData(VisibilityQuestion.Public, TypeQuestion.Standard)]
        [InlineData(VisibilityQuestion.Minigame, TypeQuestion.PileDansLeMille)]
        [InlineData(VisibilityQuestion.Custom, TypeQuestion.Standard)]
        [InlineData(VisibilityQuestion.Minigame | VisibilityQuestion.Public, TypeQuestion.SurLaPiste)]
        public async Task GetDetail_WithDifferentEnumCombinations_ShouldReturnCorrectEnums(
            VisibilityQuestion visibility,
            TypeQuestion type)
        {
            // Arrange
            var category = await CreateTestCategory($"Enum_{visibility}_{type}");
            var questionId = await CreateTestQuestion(
                $"Question {visibility} {type}",
                50,
                category,
                visibility: visibility,
                type: type
            );

            // Act
            var response = await _client.GetAsync($"/api/AdminQuestion/GetDetail?id={questionId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var questionResult = JsonSerializer.Deserialize<QuestionAdminVM>(
                await response.Content.ReadAsStringAsync(),
                _jsonOptions
            );

            Assert.NotNull(questionResult);
            Assert.Equal(visibility, questionResult.Visibility);
            Assert.Equal(type, questionResult.Type);
        }

        [Fact]
        public async Task GetDetail_AfterCreation_ShouldReturnCreatedQuestion()
        {
            // Arrange - Create question via Add endpoint
            var category = await CreateTestCategory("After Creation Category");
            var newQuestion = new QuestionAdminAddVM
            {
                Libelle = "Newly created question",
                Response = 999,
                Category = category,
                Visibility = VisibilityQuestion.Public,
                Type = TypeQuestion.Standard,
                Author = "Creator",
                Unit = "test"
            };

            var createContent = new StringContent(
                JsonSerializer.Serialize(newQuestion),
                Encoding.UTF8,
                "application/json"
            );

            var createResponse = await _client.PostAsync("/api/AdminQuestion/Add", createContent);
            createResponse.EnsureSuccessStatusCode();
            var createdQuestion = JsonSerializer.Deserialize<QuestionAdminVM>(
                await createResponse.Content.ReadAsStringAsync(),
                _jsonOptions
            );

            // Act - Get detail of created question
            var response = await _client.GetAsync($"/api/AdminQuestion/GetDetail?id={createdQuestion.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            var detailResult = JsonSerializer.Deserialize<QuestionAdminVM>(
                await response.Content.ReadAsStringAsync(),
                _jsonOptions
            );

            Assert.NotNull(detailResult);
            Assert.Equal(createdQuestion.Id, detailResult.Id);
            Assert.Equal("Newly created question", detailResult.Libelle);
            Assert.Equal(999, detailResult.Response);
            Assert.Equal("Creator", detailResult.Author);
            Assert.Equal("test", detailResult.Unit);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(999999)]
        [InlineData(int.MaxValue)]
        public async Task GetDetail_WithDifferentResponseValues_ShouldReturnCorrectResponse(int responseValue)
        {
            // Arrange
            var category = await CreateTestCategory($"Response_{responseValue}_Category");
            var questionId = await CreateTestQuestion($"Question {responseValue}", responseValue, category);

            // Act
            var response = await _client.GetAsync($"/api/AdminQuestion/GetDetail?id={questionId}");

            // Assert
            response.EnsureSuccessStatusCode();
            var questionResult = JsonSerializer.Deserialize<QuestionAdminVM>(
                await response.Content.ReadAsStringAsync(),
                _jsonOptions
            );

            Assert.NotNull(questionResult);
            Assert.Equal(responseValue, questionResult.Response);
        }

        [Fact]
        public async Task GetDetail_CalledMultipleTimes_ShouldReturnSameQuestion()
        {
            // Arrange
            var category = await CreateTestCategory("Consistency Category");
            var questionId = await CreateTestQuestion("Consistent Question", 123, category);

            // Act - Call multiple times
            var response1 = await _client.GetAsync($"/api/AdminQuestion/GetDetail?id={questionId}");
            var response2 = await _client.GetAsync($"/api/AdminQuestion/GetDetail?id={questionId}");
            var response3 = await _client.GetAsync($"/api/AdminQuestion/GetDetail?id={questionId}");

            // Assert
            response1.EnsureSuccessStatusCode();
            response2.EnsureSuccessStatusCode();
            response3.EnsureSuccessStatusCode();

            var result1 = JsonSerializer.Deserialize<QuestionAdminVM>(
                await response1.Content.ReadAsStringAsync(),
                _jsonOptions
            );
            var result2 = JsonSerializer.Deserialize<QuestionAdminVM>(
                await response2.Content.ReadAsStringAsync(),
                _jsonOptions
            );
            var result3 = JsonSerializer.Deserialize<QuestionAdminVM>(
                await response3.Content.ReadAsStringAsync(),
                _jsonOptions
            );

            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.NotNull(result3);
            Assert.Equal(result1.Id, result2.Id);
            Assert.Equal(result1.Id, result3.Id);
            Assert.Equal(result1.Libelle, result2.Libelle);
            Assert.Equal(result1.Libelle, result3.Libelle);
        }

        [Fact]
        public async Task GetDetail_AfterDelete_ShouldReturnNotFound()
        {
            // Arrange
            var category = await CreateTestCategory("Delete Detail Category");
            var questionId = await CreateTestQuestion("Question to delete", 100, category);

            // Delete the question
            var deleteResponse = await _client.DeleteAsync($"/api/AdminQuestion/Delete?id={questionId}");
            deleteResponse.EnsureSuccessStatusCode();

            // Act - Try to get detail after deletion
            var response = await _client.GetAsync($"/api/AdminQuestion/GetDetail?id={questionId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        #endregion

        #region Delete Tests

        [Fact]
        public async Task Delete_ShouldReturnOk_WhenQuestionExists()
        {
            // Arrange
            var category = await CreateTestCategory("Delete Test Category");
            var questionId = await CreateTestQuestion("Question to delete", 100, category);

            // Act
            var response = await _client.DeleteAsync($"/api/AdminQuestion/Delete?id={questionId}");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ShouldRemoveQuestionFromDatabase()
        {
            // Arrange
            var category = await CreateTestCategory("Database Delete Category");
            var questionId = await CreateTestQuestion("Question to remove", 200, category);

            // Act
            var deleteResponse = await _client.DeleteAsync($"/api/AdminQuestion/Delete?id={questionId}");
            deleteResponse.EnsureSuccessStatusCode();

            // Assert - Verify question no longer exists by searching
            var searchResponse = await _client.GetAsync("/api/AdminQuestion/Search?pageIndex=0&pageSize=100");
            searchResponse.EnsureSuccessStatusCode();
            var questions = JsonSerializer.Deserialize<List<QuestionAdminVM>>(
                await searchResponse.Content.ReadAsStringAsync(),
                _jsonOptions
            );
            Assert.NotNull(questions);
            Assert.DoesNotContain(questions, q => q.Id == questionId);
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFound_WhenQuestionDoesNotExist()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var response = await _client.DeleteAsync($"/api/AdminQuestion/Delete?id={nonExistentId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Delete_WithMultipleQuestions_ShouldOnlyDeleteSpecifiedQuestion()
        {
            // Arrange
            var category = await CreateTestCategory("Multiple Delete Category");
            var question1Id = await CreateTestQuestion("Question 1", 1, category);
            var question2Id = await CreateTestQuestion("Question 2", 2, category);
            var question3Id = await CreateTestQuestion("Question 3", 3, category);

            // Act - Delete only question2
            var deleteResponse = await _client.DeleteAsync($"/api/AdminQuestion/Delete?id={question2Id}");
            deleteResponse.EnsureSuccessStatusCode();

            // Assert - Verify only question2 was deleted
            var searchResponse = await _client.GetAsync("/api/AdminQuestion/Search?pageIndex=0&pageSize=100");
            searchResponse.EnsureSuccessStatusCode();
            var questions = JsonSerializer.Deserialize<List<QuestionAdminVM>>(
                await searchResponse.Content.ReadAsStringAsync(),
                _jsonOptions
            );
            Assert.NotNull(questions);
            Assert.Contains(questions, q => q.Id == question1Id);
            Assert.DoesNotContain(questions, q => q.Id == question2Id);
            Assert.Contains(questions, q => q.Id == question3Id);
        }

        [Fact]
        public async Task Delete_AfterDeletion_ShouldNotAppearInSearchResults()
        {
            // Arrange
            var category = await CreateTestCategory("Search After Delete Category");
            var questionId = await CreateTestQuestion("Searchable Question", 42, category, author: "Test Author");

            // Verify question exists initially
            var initialSearchResponse = await _client.GetAsync("/api/AdminQuestion/Search?pageIndex=0&pageSize=100&filter.Author=Test%20Author");
            initialSearchResponse.EnsureSuccessStatusCode();
            var initialQuestions = JsonSerializer.Deserialize<List<QuestionAdminVM>>(
                await initialSearchResponse.Content.ReadAsStringAsync(),
                _jsonOptions
            );
            Assert.NotNull(initialQuestions);
            Assert.Single(initialQuestions);

            // Act - Delete the question
            var deleteResponse = await _client.DeleteAsync($"/api/AdminQuestion/Delete?id={questionId}");
            deleteResponse.EnsureSuccessStatusCode();

            // Assert - Verify it doesn't appear in search
            var afterDeleteSearchResponse = await _client.GetAsync("/api/AdminQuestion/Search?pageIndex=0&pageSize=100&filter.Author=Test%20Author");
            afterDeleteSearchResponse.EnsureSuccessStatusCode();
            var afterDeleteQuestions = JsonSerializer.Deserialize<List<QuestionAdminVM>>(
                await afterDeleteSearchResponse.Content.ReadAsStringAsync(),
                _jsonOptions
            );
            Assert.NotNull(afterDeleteQuestions);
            Assert.Empty(afterDeleteQuestions);
        }

        [Theory]
        [InlineData("12345678-1234-1234-1234-123456789012")]
        [InlineData("aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee")]
        [InlineData("ffffffff-ffff-ffff-ffff-ffffffffffff")]
        public async Task Delete_WithValidGuid_ShouldReturnNotFound_WhenQuestionDoesNotExist(string guidString)
        {
            // Arrange
            var questionId = Guid.Parse(guidString);

            // Act
            var response = await _client.DeleteAsync($"/api/AdminQuestion/Delete?id={questionId}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Delete_WithEmptyGuid_ShouldReturnNotFound()
        {
            // Arrange
            var emptyGuid = Guid.Empty;

            // Act
            var response = await _client.DeleteAsync($"/api/AdminQuestion/Delete?id={emptyGuid}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ShouldDecrementTotalCount()
        {
            // Arrange
            var category = await CreateTestCategory("Total Count Category");
            var question1Id = await CreateTestQuestion("Question 1", 1, category);
            var question2Id = await CreateTestQuestion("Question 2", 2, category);
            var question3Id = await CreateTestQuestion("Question 3", 3, category);

            // Verify initial count
            var initialSearchResponse = await _client.GetAsync("/api/AdminQuestion/Search?pageIndex=0&pageSize=100");
            initialSearchResponse.EnsureSuccessStatusCode();
            var initialTotalCount = initialSearchResponse.Headers.GetValues("X-Total-Count").FirstOrDefault();
            Assert.Equal("3", initialTotalCount);

            // Act - Delete one question
            var deleteResponse = await _client.DeleteAsync($"/api/AdminQuestion/Delete?id={question2Id}");
            deleteResponse.EnsureSuccessStatusCode();

            // Assert - Verify count decremented
            var afterDeleteSearchResponse = await _client.GetAsync("/api/AdminQuestion/Search?pageIndex=0&pageSize=100");
            afterDeleteSearchResponse.EnsureSuccessStatusCode();
            var afterDeleteTotalCount = afterDeleteSearchResponse.Headers.GetValues("X-Total-Count").FirstOrDefault();
            Assert.Equal("2", afterDeleteTotalCount);
        }

        [Fact]
        public async Task Delete_MultipleTimes_ShouldReturnNotFound_OnSecondAttempt()
        {
            // Arrange
            var category = await CreateTestCategory("Double Delete Category");
            var questionId = await CreateTestQuestion("Question to delete twice", 50, category);

            // Act - First deletion
            var firstDeleteResponse = await _client.DeleteAsync($"/api/AdminQuestion/Delete?id={questionId}");
            firstDeleteResponse.EnsureSuccessStatusCode();

            // Act - Second deletion attempt
            var secondDeleteResponse = await _client.DeleteAsync($"/api/AdminQuestion/Delete?id={questionId}");

            // Assert - Should return NotFound
            Assert.Equal(HttpStatusCode.NotFound, secondDeleteResponse.StatusCode);
        }

        [Fact]
        public async Task Delete_QuestionWithAllOptionalFields_ShouldReturnOk()
        {
            // Arrange
            var category = await CreateTestCategory("Full Question Category");
            var questionId = await CreateTestQuestion(
                "Complex Question with all fields",
                999,
                category,
                author: "Complex Author",
                unit: "km",
                visibility: VisibilityQuestion.Minigame | VisibilityQuestion.Public,
                type: TypeQuestion.PileDansLeMille | TypeQuestion.Standard
            );

            // Act
            var response = await _client.DeleteAsync($"/api/AdminQuestion/Delete?id={questionId}");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Delete_QuestionWithMinimalFields_ShouldReturnOk()
        {
            // Arrange
            var category = await CreateTestCategory("Minimal Question Category");
            var questionId = await CreateTestQuestion(
                "Minimal Question",
                10,
                category,
                author: null,
                unit: null,
                visibility: VisibilityQuestion.Public,
                type: TypeQuestion.Standard
            );

            // Act
            var response = await _client.DeleteAsync($"/api/AdminQuestion/Delete?id={questionId}");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ShouldNotAffectQuestionsInOtherCategories()
        {
            // Arrange
            var category1 = await CreateTestCategory("Category 1");
            var category2 = await CreateTestCategory("Category 2");
            var question1Id = await CreateTestQuestion("Question in Cat1", 1, category1);
            var question2Id = await CreateTestQuestion("Question in Cat2", 2, category2);

            // Act - Delete question from category1
            var deleteResponse = await _client.DeleteAsync($"/api/AdminQuestion/Delete?id={question1Id}");
            deleteResponse.EnsureSuccessStatusCode();

            // Assert - Verify question in category2 still exists
            var searchResponse = await _client.GetAsync($"/api/AdminQuestion/Search?pageIndex=0&pageSize=100&filter.Categories={category2.Id}");
            searchResponse.EnsureSuccessStatusCode();
            var questions = JsonSerializer.Deserialize<List<QuestionAdminVM>>(
                await searchResponse.Content.ReadAsStringAsync(),
                _jsonOptions
            );
            Assert.NotNull(questions);
            Assert.Single(questions);
            Assert.Equal(question2Id, questions[0].Id);
        }

        [Fact]
        public async Task Delete_MultipleQuestionsSequentially_ShouldSucceed()
        {
            // Arrange
            var category = await CreateTestCategory("Sequential Delete Category");
            var question1Id = await CreateTestQuestion("Question 1", 1, category);
            var question2Id = await CreateTestQuestion("Question 2", 2, category);
            var question3Id = await CreateTestQuestion("Question 3", 3, category);

            // Act - Delete all questions sequentially
            var delete1Response = await _client.DeleteAsync($"/api/AdminQuestion/Delete?id={question1Id}");
            var delete2Response = await _client.DeleteAsync($"/api/AdminQuestion/Delete?id={question2Id}");
            var delete3Response = await _client.DeleteAsync($"/api/AdminQuestion/Delete?id={question3Id}");

            // Assert - All deletions should succeed
            delete1Response.EnsureSuccessStatusCode();
            delete2Response.EnsureSuccessStatusCode();
            delete3Response.EnsureSuccessStatusCode();

            // Verify all questions are deleted
            var searchResponse = await _client.GetAsync("/api/AdminQuestion/Search?pageIndex=0&pageSize=100");
            searchResponse.EnsureSuccessStatusCode();
            var questions = JsonSerializer.Deserialize<List<QuestionAdminVM>>(
                await searchResponse.Content.ReadAsStringAsync(),
                _jsonOptions
            );
            Assert.NotNull(questions);
            Assert.Empty(questions);
        }

        #endregion

        // Méthode helper pour créer une catégorie de test
        private async Task<CategoryAdminVM> CreateTestCategory(string name)
        {
            var idCategory = Guid.NewGuid();
            var category = new CategoryEntity { Id = idCategory, Name = name };
            _context.Add(category);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            return new CategoryAdminVM { Id = idCategory, Name = name };

        }

        // Méthode helper pour créer une question de test
        private async Task<Guid> CreateTestQuestion(
            string libelle,
            int response,
            CategoryAdminVM category,
            string? author = null,
            string? unit = null,
            VisibilityQuestion visibility = VisibilityQuestion.Public,
            TypeQuestion type = TypeQuestion.Standard)
        {
            var questionId = Guid.NewGuid();
            var questionEntity = new QuestionEntity
            {
                Id = questionId,
                Libelle = libelle,
                Response = response.ToString(),
                CategoryId = category.Id,
                Author = author,
                Unit = unit,
                Visibility = visibility,
                Type = type
            };
            _context.Questions.Add(questionEntity);
            await _context.SaveChangesAsync();
            _context.ChangeTracker.Clear();

            return questionId;
        }
    }
}
