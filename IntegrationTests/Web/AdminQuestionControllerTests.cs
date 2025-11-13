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
    }
}
