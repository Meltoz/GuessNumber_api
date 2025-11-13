using Domain.Enums;
using Domain.Party;
using Infrastructure.Entities;
using Infrastructure.Repositories;
using Meltix.IntegrationTests;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
