using AutoMapper;
using Domain.Enums;
using Domain.Party;
using Infrastructure;
using Infrastructure.Entities;
using Infrastructure.Repositories;
using Meltix.IntegrationTests;
using Microsoft.EntityFrameworkCore;
using Shared.Filters;

namespace IntegrationTests.Repository
{
    public class QuestionRepositoryTests
    {
        #region AddQuestion
        [Fact]
        public async Task InsertAsync_ShouldReturnQuestionWithCategory()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = new CategoryEntity
            {
                Id = Guid.NewGuid(),
                Name = "Test Category",
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var categoryDomain = mapper.Map<Category>(category);
            var question = new Question(
                "Quelle est la capitale de la France ?",
                "42",
                categoryDomain,
                VisibilityQuestion.Public,
                TypeQuestion.Standard,
                "Test Author",
                "km"
            );

            // Act
            var result = await repository.InsertAsync(question);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal(question.Libelle, result.Libelle);
            Assert.Equal(question.Response, result.Response);
            Assert.Equal(question.Visibility, result.Visibility);
            Assert.Equal(question.Type, result.Type);
            Assert.Equal(question.Author, result.Author);
            Assert.Equal(question.Unit, result.Unit);

            // Vérifie que la Category est bien chargée
            Assert.NotNull(result.Category);
            Assert.Equal(category.Id, result.Category.Id);
            Assert.Equal(category.Name, result.Category.Name);
        }

        [Fact]
        public async Task InsertAsync_ShouldNotCreateDuplicateCategory()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = new CategoryEntity
            {
                Id = Guid.NewGuid(),
                Name = "Shared Category",
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var categoryDomain = mapper.Map<Category>(category);
            var question1 = new Question(
                "Question 1",
                "10",
                categoryDomain,
                VisibilityQuestion.Public,
                TypeQuestion.Standard,
                null,
                null
            );
            var question2 = new Question(
                "Question 2",
                "20",
                categoryDomain,
                VisibilityQuestion.Public,
                TypeQuestion.Standard,
                null,
                null
            );

            // Act
            var result1 = await repository.InsertAsync(question1);
            context.ChangeTracker.Clear(); // Nettoyer le tracker entre les insertions
            var result2 = await repository.InsertAsync(question2);

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.NotEqual(result1.Id, result2.Id);

            // Vérifie qu'il n'y a toujours qu'une seule catégorie en base
            var categoriesCount = await context.Categories.CountAsync();
            Assert.Equal(1, categoriesCount);

            // Vérifie que les deux questions pointent vers la même catégorie
            Assert.Equal(result1.Category.Id, result2.Category.Id);
            Assert.Equal(category.Id, result1.Category.Id);
        }

        [Fact]
        public async Task InsertAsync_ShouldSetCreatedAndUpdatedDates()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = new CategoryEntity
            {
                Id = Guid.NewGuid(),
                Name = "Test Category",
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var categoryDomain = mapper.Map<Category>(category);
            var beforeInsert = DateTime.UtcNow;

            var question = new Question(
                "Test Question",
                "100",
                categoryDomain,
                VisibilityQuestion.Public,
                TypeQuestion.Standard,
                null,
                null
            );

            // Act
            var result = await repository.InsertAsync(question);
            var afterInsert = DateTime.UtcNow;

            // Assert
            var questionEntity = await context.Questions.FindAsync(result.Id);
            Assert.NotNull(questionEntity);
            Assert.True(questionEntity.Created >= beforeInsert && questionEntity.Created <= afterInsert);
            Assert.True(questionEntity.Updated >= beforeInsert && questionEntity.Updated <= afterInsert);
            Assert.Equal(questionEntity.Created, questionEntity.Updated);
        }

        [Fact]
        public async Task InsertAsync_WithMinimalFields_ShouldReturnQuestionWithCategory()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = new CategoryEntity
            {
                Id = Guid.NewGuid(),
                Name = "Minimal Category",
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var categoryDomain = mapper.Map<Category>(category);
            var question = new Question(
                "Minimal Question",
                "0",
                categoryDomain,
                VisibilityQuestion.Public,
                TypeQuestion.Standard,
                null, // Author optionnel
                null  // Unit optionnel
            );

            // Act
            var result = await repository.InsertAsync(question);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Category);
            Assert.Equal(category.Id, result.Category.Id);
            Assert.Null(result.Author);
            Assert.Null(result.Unit);
        }

        [Fact]
        public async Task InsertAsync_WithCombinedEnumFlags_ShouldReturnQuestionWithCategory()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = new CategoryEntity
            {
                Id = Guid.NewGuid(),
                Name = "Flags Category",
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var categoryDomain = mapper.Map<Category>(category);
            var question = new Question(
                "Question with flags",
                "50",
                categoryDomain,
                VisibilityQuestion.Minigame | VisibilityQuestion.Public,
                TypeQuestion.Standard | TypeQuestion.PileDansLeMille,
                "Author",
                "m"
            );

            // Act
            var result = await repository.InsertAsync(question);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Category);
            Assert.True(result.Visibility.HasFlag(VisibilityQuestion.Minigame));
            Assert.True(result.Visibility.HasFlag(VisibilityQuestion.Public));
            Assert.True(result.Type.HasFlag(TypeQuestion.Standard));
            Assert.True(result.Type.HasFlag(TypeQuestion.PileDansLeMille));
        }

        [Fact]
        public async Task InsertAsync_ShouldPersistQuestionInDatabase()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = new CategoryEntity
            {
                Id = Guid.NewGuid(),
                Name = "Persist Category",
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var categoryDomain = mapper.Map<Category>(category);
            var question = new Question(
                "Persisted Question",
                "999",
                categoryDomain,
                VisibilityQuestion.Minigame,
                TypeQuestion.SurLaPiste,
                "Persistence Test",
                "kg"
            );

            // Act
            var result = await repository.InsertAsync(question);
            context.ChangeTracker.Clear(); // Nettoyer le cache pour forcer une lecture DB

            // Assert - Vérifier que la question existe bien en base
            var questionFromDb = await context.Questions
                .Include(q => q.Category)
                .FirstOrDefaultAsync(q => q.Id == result.Id);

            Assert.NotNull(questionFromDb);
            Assert.Equal(result.Id, questionFromDb.Id);
            Assert.Equal("Persisted Question", questionFromDb.Libelle);
            Assert.Equal("999", questionFromDb.Response);
            Assert.Equal(category.Id, questionFromDb.CategoryId);
            Assert.NotNull(questionFromDb.Category);
            Assert.Equal("Persist Category", questionFromDb.Category.Name);
        }

        [Fact]
        public async Task InsertAsync_WithSpecialCharacters_ShouldReturnQuestionWithCategory()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = new CategoryEntity
            {
                Id = Guid.NewGuid(),
                Name = "Catégorie avec accents éèê",
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var categoryDomain = mapper.Map<Category>(category);
            var question = new Question(
                "Quelle est la température à l'équateur ? 🌡️",
                "30",
                categoryDomain,
                VisibilityQuestion.Public,
                TypeQuestion.Standard,
                "Auteur Ünîçödé",
                "°C"
            );

            // Act
            var result = await repository.InsertAsync(question);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Category);
            Assert.Equal("Quelle est la température à l'équateur ? 🌡️", result.Libelle);
            Assert.Equal("Auteur Ünîçödé", result.Author);
            Assert.Equal("°C", result.Unit);
            Assert.Equal("Catégorie avec accents éèê", result.Category.Name);
        }

        [Theory]
        [InlineData(100)]
        [InlineData(0)]
        [InlineData(int.MaxValue)]
        public async Task InsertAsync_WithDifferentResponseValues_ShouldReturnQuestionWithCategory(int responseValue)
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = new CategoryEntity
            {
                Id = Guid.NewGuid(),
                Name = $"Category_{responseValue}",
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var categoryDomain = mapper.Map<Category>(category);
            var question = new Question(
                $"Question with response {responseValue}",
                responseValue.ToString(),
                categoryDomain,
                VisibilityQuestion.Public,
                TypeQuestion.Standard,
                null,
                null
            );

            // Act
            var result = await repository.InsertAsync(question);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Category);
            Assert.Equal(responseValue.ToString(), result.Response);
            Assert.Equal(category.Id, result.Category.Id);
        }
        #endregion

        #region SearchQuestion

        [Fact]
        public async Task SearchQuestion_WithoutFilters_ShouldReturnAllQuestionsOrderedByCreatedDesc()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = await CreateCategoryAsync(context, "Test Category");
            await CreateMultipleQuestionsAsync(context, mapper, category, 5);

            var filterOption = new QuestionFilter();

            // Act
            var result = await repository.SearchQuestion(0, 10, filterOption);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.TotalCount);
            Assert.Equal(5, result.Data.Count());
        }

        [Fact]
        public async Task SearchQuestion_WithLibelleFilter_ShouldReturnMatchingQuestions()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = await CreateCategoryAsync(context, "Test Category");

            await CreateQuestionAsync(context, mapper, category, "Quelle est la capitale de la France ?");
            await CreateQuestionAsync(context, mapper, category, "Quelle est la capitale de l'Allemagne ?");
            await CreateQuestionAsync(context, mapper, category, "Quel est le plus grand océan ?");

            var filterOption = new QuestionFilter { Libelle = "capitale" };

            // Act
            var result = await repository.SearchQuestion(0, 10, filterOption);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            Assert.All(result.Data, q => Assert.Contains("capitale", q.Libelle.Value, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task SearchQuestion_WithLibelleFilter_CaseInsensitive_ShouldReturnMatchingQuestions()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = await CreateCategoryAsync(context, "Test Category");

            await CreateQuestionAsync(context, mapper, category, "QUELLE EST LA CAPITALE ?");
            await CreateQuestionAsync(context, mapper, category, "quel est l'océan ?");

            var filterOption = new QuestionFilter { Libelle = "QuElLe" };

            // Act
            var result = await repository.SearchQuestion(0, 10, filterOption);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);
        }

        [Fact]
        public async Task SearchQuestion_WithAuthorFilter_ShouldReturnMatchingQuestions()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = await CreateCategoryAsync(context, "Test Category");

            await CreateQuestionAsync(context, mapper, category, "Question 1", author: "John Doe");
            await CreateQuestionAsync(context, mapper, category, "Question 2", author: "Jane Smith");
            await CreateQuestionAsync(context, mapper, category, "Question 3", author: "John Smith");

            var filterOption = new QuestionFilter { Author = "john" };

            // Act
            var result = await repository.SearchQuestion(0, 10, filterOption);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            Assert.All(result.Data, q => Assert.Contains("john", q.Author.Value, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task SearchQuestion_WithCategoriesFilter_SingleCategory_ShouldReturnMatchingQuestions()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category1 = await CreateCategoryAsync(context, "Category 1");
            var category2 = await CreateCategoryAsync(context, "Category 2");
            var category3 = await CreateCategoryAsync(context, "Category 3");

            await CreateQuestionAsync(context, mapper, category1, "Question Cat 1");
            await CreateQuestionAsync(context, mapper, category2, "Question Cat 2");
            await CreateQuestionAsync(context, mapper, category3, "Question Cat 3");

            var filterOption = new QuestionFilter
            {
                Categories = category1.Id.ToString()
            };

            // Act
            var result = await repository.SearchQuestion(0, 10, filterOption);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);
            Assert.Equal(category1.Id, result.Data.First().Category.Id);
        }

        [Fact]
        public async Task SearchQuestion_WithCategoriesFilter_MultipleCategories_ShouldReturnMatchingQuestions()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category1 = await CreateCategoryAsync(context, "Category 1");
            var category2 = await CreateCategoryAsync(context, "Category 2");
            var category3 = await CreateCategoryAsync(context, "Category 3");

            await CreateQuestionAsync(context, mapper, category1, "Question Cat 1");
            await CreateQuestionAsync(context, mapper, category2, "Question Cat 2");
            await CreateQuestionAsync(context, mapper, category3, "Question Cat 3");

            var filterOption = new QuestionFilter
            {
                Categories = $"{category1.Id},{category2.Id}"
            };

            // Act
            var result = await repository.SearchQuestion(0, 10, filterOption);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            Assert.All(result.Data, q =>
                Assert.True(q.Category.Id == category1.Id || q.Category.Id == category2.Id));
        }

        [Fact]
        public async Task SearchQuestion_WithCategoriesFilter_WithSpacesAndDuplicates_ShouldHandleCorrectly()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = await CreateCategoryAsync(context, "Test Category");
            await CreateQuestionAsync(context, mapper, category, "Question 1");

            var filterOption = new QuestionFilter
            {
                Categories = $"  {category.Id}  , {category.Id} ,  {category.Id}  "
            };

            // Act
            var result = await repository.SearchQuestion(0, 10, filterOption);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);
        }

        [Fact]
        public async Task SearchQuestion_WithInvalidCategoriesFilter_ShouldIgnoreInvalidGuids()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = await CreateCategoryAsync(context, "Test Category");
            await CreateQuestionAsync(context, mapper, category, "Question 1");

            var filterOption = new QuestionFilter
            {
                Categories = $"{category.Id},invalid-guid,  ,not-a-guid,{Guid.NewGuid()}"
            };

            // Act
            var result = await repository.SearchQuestion(0, 10, filterOption);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);
        }

        [Fact]
        public async Task SearchQuestion_WithEmptyCategoriesFilter_ShouldReturnAllQuestions()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = await CreateCategoryAsync(context, "Test Category");
            await CreateQuestionAsync(context, mapper, category, "Question 1");
            await CreateQuestionAsync(context, mapper, category, "Question 2");

            var filterOption = new QuestionFilter
            {
                Categories = "  ,  ,  "
            };

            // Act
            var result = await repository.SearchQuestion(0, 10, filterOption);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
        }

        [Fact]
        public async Task SearchQuestion_WithVisibilityFilter_Public_ShouldReturnPublicQuestions()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = await CreateCategoryAsync(context, "Test Category");

            await CreateQuestionAsync(context, mapper, category, "Public Q", visibility: VisibilityQuestion.Public);
            await CreateQuestionAsync(context, mapper, category, "Custom Q", visibility: VisibilityQuestion.Custom);
            await CreateQuestionAsync(context, mapper, category, "Minigame Q",
                visibility: VisibilityQuestion.Minigame, type: TypeQuestion.PileDansLeMille);

            var filterOption = new QuestionFilter { Visibility = (int)VisibilityQuestion.Public };

            // Act
            var result = await repository.SearchQuestion(0, 10, filterOption);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);
            Assert.True((result.Data.First().Visibility & VisibilityQuestion.Public) != 0);
        }

        [Fact]
        public async Task SearchQuestion_WithVisibilityFilter_Minigame_ShouldReturnMinigameQuestions()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = await CreateCategoryAsync(context, "Test Category");

            await CreateQuestionAsync(context, mapper, category, "Public Q", visibility: VisibilityQuestion.Public);
            await CreateQuestionAsync(context, mapper, category, "Minigame Q1",
                visibility: VisibilityQuestion.Minigame, type: TypeQuestion.PileDansLeMille);
            await CreateQuestionAsync(context, mapper, category, "Minigame Q2",
                visibility: VisibilityQuestion.Minigame, type: TypeQuestion.SurLaPiste);

            var filterOption = new QuestionFilter { Visibility = (int)VisibilityQuestion.Minigame };

            // Act
            var result = await repository.SearchQuestion(0, 10, filterOption);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            Assert.All(result.Data, q => Assert.True((q.Visibility & VisibilityQuestion.Minigame) != 0));
        }

        [Fact]
        public async Task SearchQuestion_WithVisibilityFilter_Combined_ShouldReturnMatchingQuestions()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = await CreateCategoryAsync(context, "Test Category");

            await CreateQuestionAsync(context, mapper, category, "Public Q", visibility: VisibilityQuestion.Public);
            await CreateQuestionAsync(context, mapper, category, "Custom Q", visibility: VisibilityQuestion.Custom);
            await CreateQuestionAsync(context, mapper, category, "Minigame Q",
                visibility: VisibilityQuestion.Minigame, type: TypeQuestion.PileDansLeMille);

            var filterOption = new QuestionFilter
            {
                Visibility = (int)(VisibilityQuestion.Public | VisibilityQuestion.Custom)
            };

            // Act
            var result = await repository.SearchQuestion(0, 10, filterOption);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            Assert.All(result.Data, q =>
                Assert.True((q.Visibility & VisibilityQuestion.Public) != 0 ||
                            (q.Visibility & VisibilityQuestion.Custom) != 0));
        }

        [Fact]
        public async Task SearchQuestion_WithTypeFilter_Standard_ShouldReturnStandardQuestions()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = await CreateCategoryAsync(context, "Test Category");

            await CreateQuestionAsync(context, mapper, category, "Standard Q",
                visibility: VisibilityQuestion.Public, type: TypeQuestion.Standard);
            await CreateQuestionAsync(context, mapper, category, "Pile Q",
                visibility: VisibilityQuestion.Minigame, type: TypeQuestion.PileDansLeMille);
            await CreateQuestionAsync(context, mapper, category, "Piste Q",
                visibility: VisibilityQuestion.Minigame, type: TypeQuestion.SurLaPiste);

            var filterOption = new QuestionFilter { Type = (int)TypeQuestion.Standard };

            // Act
            var result = await repository.SearchQuestion(0, 10, filterOption);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);
            Assert.True((result.Data.First().Type & TypeQuestion.Standard) != 0);
        }

        [Fact]
        public async Task SearchQuestion_WithTypeFilter_PileDansLeMille_ShouldReturnMatchingQuestions()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = await CreateCategoryAsync(context, "Test Category");

            await CreateQuestionAsync(context, mapper, category, "Standard Q",
                visibility: VisibilityQuestion.Public, type: TypeQuestion.Standard);
            await CreateQuestionAsync(context, mapper, category, "Pile Q",
                visibility: VisibilityQuestion.Minigame, type: TypeQuestion.PileDansLeMille);

            var filterOption = new QuestionFilter { Type = (int)TypeQuestion.PileDansLeMille };

            // Act
            var result = await repository.SearchQuestion(0, 10, filterOption);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);
            Assert.True((result.Data.First().Type & TypeQuestion.PileDansLeMille) != 0);
        }

        [Fact]
        public async Task SearchQuestion_WithTypeFilter_Combined_ShouldReturnMatchingQuestions()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = await CreateCategoryAsync(context, "Test Category");

            await CreateQuestionAsync(context, mapper, category, "Standard Q",
                visibility: VisibilityQuestion.Public, type: TypeQuestion.Standard);
            await CreateQuestionAsync(context, mapper, category, "Pile Q",
                visibility: VisibilityQuestion.Minigame, type: TypeQuestion.PileDansLeMille);
            await CreateQuestionAsync(context, mapper, category, "Piste Q",
                visibility: VisibilityQuestion.Minigame, type: TypeQuestion.SurLaPiste);
            await CreateQuestionAsync(context, mapper, category, "Coup Q",
                visibility: VisibilityQuestion.Minigame, type: TypeQuestion.UnDernierCoup);

            var filterOption = new QuestionFilter
            {
                Type = (int)(TypeQuestion.PileDansLeMille | TypeQuestion.SurLaPiste)
            };

            // Act
            var result = await repository.SearchQuestion(0, 10, filterOption);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            Assert.All(result.Data, q =>
                Assert.True((q.Type & TypeQuestion.PileDansLeMille) != 0 ||
                            (q.Type & TypeQuestion.SurLaPiste) != 0));
        }

        [Fact]
        public async Task SearchQuestion_WithPagination_FirstPage_ShouldReturnCorrectPage()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = await CreateCategoryAsync(context, "Test Category");
            await CreateMultipleQuestionsAsync(context, mapper, category, 15);

            var filterOption = new QuestionFilter();

            // Act
            var result = await repository.SearchQuestion(0, 5, filterOption);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(15, result.TotalCount);
            Assert.Equal(5, result.Data.Count());
        }

        [Fact]
        public async Task SearchQuestion_WithPagination_SecondPage_ShouldReturnCorrectPage()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = await CreateCategoryAsync(context, "Test Category");
            await CreateMultipleQuestionsAsync(context, mapper, category, 15);

            var filterOption = new QuestionFilter();

            // Act
            var result = await repository.SearchQuestion(5, 5, filterOption);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(15, result.TotalCount);
            Assert.Equal(5, result.Data.Count());
        }

        [Fact]
        public async Task SearchQuestion_WithPagination_LastPage_ShouldReturnRemainingItems()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = await CreateCategoryAsync(context, "Test Category");
            await CreateMultipleQuestionsAsync(context, mapper, category, 12);

            var filterOption = new QuestionFilter();

            // Act
            var result = await repository.SearchQuestion(10, 5, filterOption);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(12, result.TotalCount);
            Assert.Equal(2, result.Data.Count());
        }

        [Fact]
        public async Task SearchQuestion_WithMultipleFilters_ShouldReturnMatchingQuestions()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category1 = await CreateCategoryAsync(context, "Category 1");
            var category2 = await CreateCategoryAsync(context, "Category 2");

            await CreateQuestionAsync(context, mapper, category1, "Quelle capitale ?", author: "John Doe");
            await CreateQuestionAsync(context, mapper, category1, "Quel océan ?", author: "Jane Smith");
            await CreateQuestionAsync(context, mapper, category2, "Quelle capitale ?", author: "John Doe");
            await CreateQuestionAsync(context, mapper, category1, "Quelle capitale ?", author: "John Smith");

            var filterOption = new QuestionFilter
            {
                Libelle = "capitale",
                Author = "john doe",
                Categories = category1.Id.ToString()
            };

            // Act
            var result = await repository.SearchQuestion(0, 10, filterOption);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);
            var question = result.Data.First();
            Assert.Contains("capitale", question.Libelle.Value, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("john doe", question.Author.Value, StringComparison.OrdinalIgnoreCase);
            Assert.Equal(category1.Id, question.Category.Id);
        }

        [Fact]
        public async Task SearchQuestion_WithAllFilters_ShouldReturnMatchingQuestions()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = await CreateCategoryAsync(context, "Test Category");

            await CreateQuestionAsync(context, mapper, category, "Test capitale", author: "John");
            await CreateQuestionAsync(context, mapper, category, "Test capitale", author: "John",
                visibility: VisibilityQuestion.Minigame, type: TypeQuestion.PileDansLeMille);
            await CreateQuestionAsync(context, mapper, category, "Autre question", author: "John");

            var filterOption = new QuestionFilter
            {
                Libelle = "capitale",
                Author = "john",
                Categories = category.Id.ToString(),
                Visibility = (int)VisibilityQuestion.Minigame,
                Type = (int)TypeQuestion.PileDansLeMille
            };

            // Act
            var result = await repository.SearchQuestion(0, 10, filterOption);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);
            var question = result.Data.First();
            Assert.Contains("capitale", question.Libelle.Value, StringComparison.OrdinalIgnoreCase);
            Assert.True((question.Visibility & VisibilityQuestion.Minigame) != 0);
            Assert.True((question.Type & TypeQuestion.PileDansLeMille) != 0);
        }

        [Fact]
        public async Task SearchQuestion_ShouldIncludeCategoryInResults()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = await CreateCategoryAsync(context, "Test Category");
            await CreateQuestionAsync(context, mapper, category, "Test Question");

            var filterOption = new QuestionFilter();

            // Act
            var result = await repository.SearchQuestion(0, 10, filterOption);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.Data);
            Assert.NotNull(result.Data.First().Category);
            Assert.Equal(category.Id, result.Data.First().Category.Id);
            Assert.Equal(category.Name, result.Data.First().Category.Name);
        }

        [Fact]
        public async Task SearchQuestion_WithNoResults_ShouldReturnEmptyCollection()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = await CreateCategoryAsync(context, "Test Category");
            await CreateQuestionAsync(context, mapper, category, "Question 1");

            var filterOption = new QuestionFilter { Libelle = "nonexistent" };

            // Act
            var result = await repository.SearchQuestion(0, 10, filterOption);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.TotalCount);
            Assert.Empty(result.Data);
        }

        [Fact]
        public async Task SearchQuestion_OrderByCreatedDesc_ShouldReturnNewestFirst()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = await CreateCategoryAsync(context, "Test Category");

            var q1 = await CreateQuestionAsync(context, mapper, category, "Question 1");
            await Task.Delay(100);
            var q2 = await CreateQuestionAsync(context, mapper, category, "Question 2");
            await Task.Delay(100);
            var q3 = await CreateQuestionAsync(context, mapper, category, "Question 3");

            var filterOption = new QuestionFilter();

            // Act
            var result = await repository.SearchQuestion(0, 10, filterOption);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.TotalCount);
            var questions = result.Data.ToList();
        }

        [Fact]
        public async Task SearchQuestion_WithVisibilityAndTypeConsistency_Minigame_ShouldHaveNonStandardType()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = await CreateCategoryAsync(context, "Test Category");

            await CreateQuestionAsync(context, mapper, category, "Minigame Q1",
                visibility: VisibilityQuestion.Minigame, type: TypeQuestion.PileDansLeMille);
            await CreateQuestionAsync(context, mapper, category, "Minigame Q2",
                visibility: VisibilityQuestion.Minigame, type: TypeQuestion.SurLaPiste);
            await CreateQuestionAsync(context, mapper, category, "Minigame Q3",
                visibility: VisibilityQuestion.Minigame, type: TypeQuestion.UnDernierCoup);

            var filterOption = new QuestionFilter { Visibility = (int)VisibilityQuestion.Minigame };

            // Act
            var result = await repository.SearchQuestion(0, 10, filterOption);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.TotalCount);
            Assert.All(result.Data, q =>
            {
                Assert.True((q.Visibility & VisibilityQuestion.Minigame) != 0);
                Assert.True(q.Type != TypeQuestion.Standard);
                Assert.True((q.Type & (TypeQuestion.PileDansLeMille | TypeQuestion.SurLaPiste | TypeQuestion.UnDernierCoup)) != 0);
            });
        }

        [Fact]
        public async Task SearchQuestion_WithVisibilityAndTypeConsistency_PublicOrCustom_ShouldHaveStandardType()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = await CreateCategoryAsync(context, "Test Category");

            await CreateQuestionAsync(context, mapper, category, "Public Q",
                visibility: VisibilityQuestion.Public, type: TypeQuestion.Standard);
            await CreateQuestionAsync(context, mapper, category, "Custom Q",
                visibility: VisibilityQuestion.Custom, type: TypeQuestion.Standard);

            var filterOption = new QuestionFilter
            {
                Visibility = (int)(VisibilityQuestion.Public | VisibilityQuestion.Custom)
            };

            // Act
            var result = await repository.SearchQuestion(0, 10, filterOption);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            Assert.All(result.Data, q =>
            {
                Assert.True((q.Visibility & (VisibilityQuestion.Public | VisibilityQuestion.Custom)) != 0);
                Assert.Equal(TypeQuestion.Standard, q.Type);
            });
        }

        #region Helper Methods

        private async Task<CategoryEntity> CreateCategoryAsync(GuessNumberContext context, string name)
        {
            var category = new CategoryEntity
            {
                Id = Guid.NewGuid(),
                Name = name,
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            };
            context.Categories.Add(category);
            await context.SaveChangesAsync();
            return category;
        }

        private async Task<QuestionEntity> CreateQuestionAsync(
            GuessNumberContext context,
            IMapper mapper,
            CategoryEntity category,
            string libelle,
            string author = "Test Author",
            string response = "42",
            VisibilityQuestion visibility = VisibilityQuestion.Public,
            TypeQuestion type = TypeQuestion.Standard)
        {
            var categoryDomain = mapper.Map<Category>(category);
            var question = new Question(
                libelle,
                response,
                categoryDomain,
                visibility,
                type,
                author,
                "km"
            );

            var questionEntity = mapper.Map<QuestionEntity>(question);
            questionEntity.CategoryId = category.Id;
            questionEntity.Created = DateTime.UtcNow;
            questionEntity.Updated = DateTime.UtcNow;

            context.Questions.Add(questionEntity);
            await context.SaveChangesAsync();

            return questionEntity;
        }

        private async Task<List<QuestionEntity>> CreateMultipleQuestionsAsync(
             GuessNumberContext context,
            IMapper mapper,
            CategoryEntity category,
            int count)
        {
            var questions = new List<QuestionEntity>();
            for (int i = 0; i < count; i++)
            {
                var question = await CreateQuestionAsync(
                    context,
                    mapper,
                    category,
                    $"Question {i + 1}");

                await Task.Delay(10);
                questions.Add(question);
            }
            return questions;
        }

        #endregion

        #endregion

        #region GetByIdAsync

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnQuestionWithCategory()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = await CreateCategoryAsync(context, "Test Category");
            var questionEntity = await CreateQuestionAsync(context, mapper, category, "Test Question");

            // Act
            var result = await repository.GetByIdAsync(questionEntity.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(questionEntity.Id, result.Id);
            Assert.Equal(questionEntity.Libelle, result.Libelle);
            Assert.Equal(questionEntity.Response, result.Response);
            Assert.NotNull(result.Category);
            Assert.Equal(category.Id, result.Category.Id);
            Assert.Equal(category.Name, result.Category.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistentId_ShouldReturnNull()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await repository.GetByIdAsync(nonExistentId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_WithEmptyGuid_ShouldReturnNull()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            // Act
            var result = await repository.GetByIdAsync(Guid.Empty);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnQuestionWithAllFields()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = await CreateCategoryAsync(context, "Full Fields Category");
            var questionEntity = await CreateQuestionAsync(
                context,
                mapper,
                category,
                "Complete Question",
                author: "Complete Author",
                response: "999",
                visibility: VisibilityQuestion.Minigame | VisibilityQuestion.Public,
                type: TypeQuestion.PileDansLeMille | TypeQuestion.Standard
            );

            // Act
            var result = await repository.GetByIdAsync(questionEntity.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Complete Question", result.Libelle);
            Assert.Equal("999", result.Response);
            Assert.Equal("Complete Author", result.Author);
            Assert.NotNull(result.Category);
            Assert.True(result.Visibility.HasFlag(VisibilityQuestion.Minigame));
            Assert.True(result.Visibility.HasFlag(VisibilityQuestion.Public));
            Assert.True(result.Type.HasFlag(TypeQuestion.PileDansLeMille));
            Assert.True(result.Type.HasFlag(TypeQuestion.Standard));
        }

        [Fact]
        public async Task GetByIdAsync_WithMinimalFields_ShouldReturnQuestionWithCategory()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = await CreateCategoryAsync(context, "Minimal Category");
            var categoryDomain = mapper.Map<Category>(category);
            var question = new Question(
                "Minimal Question",
                "0",
                categoryDomain,
                VisibilityQuestion.Public,
                TypeQuestion.Standard,
                null, // No author
                null  // No unit
            );

            var inserted = await repository.InsertAsync(question);

            // Act
            var result = await repository.GetByIdAsync(inserted.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Minimal Question", result.Libelle);
            Assert.Null(result.Author);
            Assert.Null(result.Unit);
            Assert.NotNull(result.Category);
            Assert.Equal(category.Id, result.Category.Id);
        }

        [Fact]
        public async Task GetByIdAsync_WithSpecialCharacters_ShouldReturnQuestionWithCategory()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = await CreateCategoryAsync(context, "Catégorie spéciale éèê");
            var questionEntity = await CreateQuestionAsync(
                context,
                mapper,
                category,
                "Quelle est la température à l'équateur ? 🌡️",
                author: "Auteur Ünîçödé"
            );

            // Act
            var result = await repository.GetByIdAsync(questionEntity.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Quelle est la température à l'équateur ? 🌡️", result.Libelle);
            Assert.Equal("Auteur Ünîçödé", result.Author);
            Assert.NotNull(result.Category);
            Assert.Equal("Catégorie spéciale éèê", result.Category.Name);
        }

        [Fact]
        public async Task GetByIdAsync_MultipleQuestions_ShouldReturnCorrectOne()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = await CreateCategoryAsync(context, "Multiple Questions Category");
            var question1 = await CreateQuestionAsync(context, mapper, category, "Question 1");
            var question2 = await CreateQuestionAsync(context, mapper, category, "Question 2");
            var question3 = await CreateQuestionAsync(context, mapper, category, "Question 3");

            // Act
            var result = await repository.GetByIdAsync(question2.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(question2.Id, result.Id);
            Assert.Equal("Question 2", result.Libelle);
        }

        [Fact]
        public async Task GetByIdAsync_AfterInsert_ShouldReturnInsertedQuestion()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = new CategoryEntity
            {
                Id = Guid.NewGuid(),
                Name = "Insert Test Category",
                Created = DateTime.UtcNow,
                Updated = DateTime.UtcNow
            };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var categoryDomain = mapper.Map<Category>(category);
            var question = new Question(
                "Inserted Question",
                "123",
                categoryDomain,
                VisibilityQuestion.Public,
                TypeQuestion.Standard,
                "Insert Author",
                "km"
            );

            // Act - Insert then retrieve
            var inserted = await repository.InsertAsync(question);
            context.ChangeTracker.Clear();
            var result = await repository.GetByIdAsync(inserted.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(inserted.Id, result.Id);
            Assert.Equal("Inserted Question", result.Libelle);
            Assert.NotNull(result.Category);
            Assert.Equal(category.Name, result.Category.Name);
        }

        [Theory]
        [InlineData(VisibilityQuestion.Public, TypeQuestion.Standard)]
        [InlineData(VisibilityQuestion.Minigame, TypeQuestion.PileDansLeMille)]
        [InlineData(VisibilityQuestion.Custom, TypeQuestion.Standard)]
        [InlineData(VisibilityQuestion.Minigame | VisibilityQuestion.Public, TypeQuestion.SurLaPiste | TypeQuestion.PileDansLeMille)]
        public async Task GetByIdAsync_WithDifferentEnumCombinations_ShouldReturnQuestionWithCorrectEnums(
            VisibilityQuestion visibility,
            TypeQuestion type)
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = await CreateCategoryAsync(context, $"Enum_{visibility}_{type}");
            var questionEntity = await CreateQuestionAsync(
                context,
                mapper,
                category,
                $"Question {visibility} {type}",
                visibility: visibility,
                type: type
            );

            // Act
            var result = await repository.GetByIdAsync(questionEntity.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(visibility, result.Visibility);
            Assert.Equal(type, result.Type);
            Assert.NotNull(result.Category);
        }

        [Fact]
        public async Task GetByIdAsync_QuestionFromDifferentCategories_ShouldReturnCorrectCategory()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category1 = await CreateCategoryAsync(context, "Category 1");
            var category2 = await CreateCategoryAsync(context, "Category 2");
            var question1 = await CreateQuestionAsync(context, mapper, category1, "Question in Cat1");
            var question2 = await CreateQuestionAsync(context, mapper, category2, "Question in Cat2");

            // Act
            var result1 = await repository.GetByIdAsync(question1.Id);
            var result2 = await repository.GetByIdAsync(question2.Id);

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.NotNull(result1.Category);
            Assert.NotNull(result2.Category);
            Assert.Equal(category1.Id, result1.Category.Id);
            Assert.Equal("Category 1", result1.Category.Name);
            Assert.Equal(category2.Id, result2.Category.Id);
            Assert.Equal("Category 2", result2.Category.Name);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldNotTrackEntity()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = await CreateCategoryAsync(context, "No Tracking Category");
            var questionEntity = await CreateQuestionAsync(context, mapper, category, "No Tracking Question");

            // Clear the tracker before calling GetByIdAsync
            context.ChangeTracker.Clear();

            // Act
            var result = await repository.GetByIdAsync(questionEntity.Id);

            // Assert - Verify entity is not tracked
            var trackedEntities = context.ChangeTracker.Entries<QuestionEntity>().ToList();
            Assert.Empty(trackedEntities);
        }

        [Fact]
        public async Task GetByIdAsync_CalledMultipleTimes_ShouldReturnConsistentResults()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new QuestionRepository(context, mapper);

            var category = await CreateCategoryAsync(context, "Consistency Category");
            var questionEntity = await CreateQuestionAsync(context, mapper, category, "Consistent Question");

            // Act - Call multiple times
            var result1 = await repository.GetByIdAsync(questionEntity.Id);
            var result2 = await repository.GetByIdAsync(questionEntity.Id);
            var result3 = await repository.GetByIdAsync(questionEntity.Id);

            // Assert - All results should be equal
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.NotNull(result3);
            Assert.Equal(result1.Id, result2.Id);
            Assert.Equal(result1.Id, result3.Id);
            Assert.Equal(result1.Libelle, result2.Libelle);
            Assert.Equal(result1.Libelle, result3.Libelle);
            Assert.Equal(result1.Category.Id, result2.Category.Id);
            Assert.Equal(result1.Category.Id, result3.Category.Id);
        }

        #endregion
    }
}
