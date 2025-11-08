using Infrastructure.Entities;
using Infrastructure.Repositories;
using Meltix.IntegrationTests;
using Shared;
using Shared.Enums.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests.Repository
{
    public  class CategoryRepositoryTests
    {
        [Fact]
        public async Task Search_WithoutSearchTerm_ReturnsAllCategories()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new CategoryRepository(context, mapper);

            var category1 = new CategoryEntity
            {
                Name = "Technology",
                Created = DateTime.UtcNow.AddDays(-2)
            };

            var category2 = new CategoryEntity
            {
                Name = "Sports",
                Created = DateTime.UtcNow.AddDays(-1)
            };

            context.Categories.AddRange(category1, category2);
            await context.SaveChangesAsync();

            var sortOption = new SortOption<SortCategory>
            {
                SortBy = SortCategory.Name,
                Direction = SortDirection.Ascending
            };

            // Act
            var result = await repository.Search(0, 10, sortOption, "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Data.Count());
        }

        [Fact]
        public async Task Search_WithSearchTermInName_ReturnsMatchingCategories()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new CategoryRepository(context, mapper);

            var category1 = new CategoryEntity
            {
                Name = "Technology",
                Created = DateTime.UtcNow.AddDays(-2)
            };

            var category2 = new CategoryEntity
            {
                Name = "Sports",
                Created = DateTime.UtcNow.AddDays(-1)
            };

            var category3 = new CategoryEntity
            {
                Name = "Tech News",
                Created = DateTime.UtcNow
            };

            context.Categories.AddRange(category1, category2, category3);
            await context.SaveChangesAsync();

            var sortOption = new SortOption<SortCategory>
            {
                SortBy = SortCategory.Name,
                Direction = SortDirection.Ascending
            };

            // Act
            var result = await repository.Search(0, 10, sortOption, "tech");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Data.Count());
            Assert.All(result.Data, c => Assert.Contains("tech", c.Name.ToLower()));
        }

        [Fact]
        public async Task Search_WithSearchTermCaseInsensitive_ReturnsMatchingCategories()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new CategoryRepository(context, mapper);

            var category1 = new CategoryEntity
            {
                Name = "Technology",
                Created = DateTime.UtcNow.AddDays(-2)
            };

            var category2 = new CategoryEntity
            {
                Name = "SPORTS",
                Created = DateTime.UtcNow.AddDays(-1)
            };

            context.Categories.AddRange(category1, category2);
            await context.SaveChangesAsync();

            var sortOption = new SortOption<SortCategory>
            {
                SortBy = SortCategory.Name,
                Direction = SortDirection.Ascending
            };

            // Act
            var result = await repository.Search(0, 10, sortOption, "TECHNOLOGY");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Data);
            Assert.Equal("Technology", result.Data.First().Name);
        }

        [Fact]
        public async Task Search_WithNoMatches_ReturnsEmptyResult()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new CategoryRepository(context, mapper);

            var category1 = new CategoryEntity
            {
                Name = "Technology",
                Created = DateTime.UtcNow.AddDays(-2)
            };

            context.Categories.Add(category1);
            await context.SaveChangesAsync();

            var sortOption = new SortOption<SortCategory>
            {
                SortBy = SortCategory.Name,
                Direction = SortDirection.Ascending
            };

            // Act
            var result = await repository.Search(0, 10, sortOption, "nonexistent");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.TotalCount);
            Assert.Empty(result.Data);
        }

        [Fact]
        public async Task Search_SortByNameAscending_ReturnsCategoriesInCorrectOrder()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new CategoryRepository(context, mapper);

            var category1 = new CategoryEntity
            {
                Name = "Zebra",
                Created = DateTime.UtcNow.AddDays(-2)
            };

            var category2 = new CategoryEntity
            {
                Name = "Apple",
                Created = DateTime.UtcNow.AddDays(-1)
            };

            var category3 = new CategoryEntity
            {
                Name = "Mango",
                Created = DateTime.UtcNow
            };

            context.Categories.AddRange(category1, category2, category3);
            await context.SaveChangesAsync();

            var sortOption = new SortOption<SortCategory>
            {
                SortBy = SortCategory.Name,
                Direction = SortDirection.Ascending
            };

            // Act
            var result = await repository.Search(0, 10, sortOption, "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.TotalCount);
            var categories = result.Data.ToList();
            Assert.Equal("Apple", categories[0].Name);
            Assert.Equal("Mango", categories[1].Name);
            Assert.Equal("Zebra", categories[2].Name);
        }

        [Fact]
        public async Task Search_SortByNameDescending_ReturnsCategoriesInCorrectOrder()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new CategoryRepository(context, mapper);

            var category1 = new CategoryEntity
            {
                Name = "Zebra",
                Created = DateTime.UtcNow.AddDays(-2)
            };

            var category2 = new CategoryEntity
            {
                Name = "Apple",
                Created = DateTime.UtcNow.AddDays(-1)
            };

            var category3 = new CategoryEntity
            {
                Name = "Mango",
                Created = DateTime.UtcNow
            };

            context.Categories.AddRange(category1, category2, category3);
            await context.SaveChangesAsync();

            var sortOption = new SortOption<SortCategory>
            {
                SortBy = SortCategory.Name,
                Direction = SortDirection.Descending
            };

            // Act
            var result = await repository.Search(0, 10, sortOption, "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.TotalCount);
            var categories = result.Data.ToList();
            Assert.Equal("Zebra", categories[0].Name);
            Assert.Equal("Mango", categories[1].Name);
            Assert.Equal("Apple", categories[2].Name);
        }

        [Fact]
        public async Task Search_SortByCreatedAscending_ReturnsCategoriesInCorrectOrder()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new CategoryRepository(context, mapper);

            var category1 = new CategoryEntity
            {
                Name = "Category 1",
                Created = DateTime.UtcNow.AddDays(-5)
            };

            var category2 = new CategoryEntity
            {
                Name = "Category 2",
                Created = DateTime.UtcNow.AddDays(-3)
            };

            var category3 = new CategoryEntity
            {
                Name = "Category 3",
                Created = DateTime.UtcNow.AddDays(-1)
            };

            context.Categories.AddRange(category1, category2, category3);
            await context.SaveChangesAsync();

            var sortOption = new SortOption<SortCategory>
            {
                SortBy = SortCategory.Created,
                Direction = SortDirection.Ascending
            };

            // Act
            var result = await repository.Search(0, 10, sortOption, "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.TotalCount);
            var categories = result.Data.ToList();
            Assert.Equal("Category 1", categories[0].Name);
            Assert.Equal("Category 2", categories[1].Name);
            Assert.Equal("Category 3", categories[2].Name);
        }

        [Fact]
        public async Task Search_SortByCreatedDescending_ReturnsCategoriesInCorrectOrder()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new CategoryRepository(context, mapper);

            var category1 = new CategoryEntity
            {
                Name = "Category 1",
                Created = DateTime.UtcNow.AddDays(-5)
            };

            var category2 = new CategoryEntity
            {
                Name = "Category 2",
                Created = DateTime.UtcNow.AddDays(-3)
            };

            var category3 = new CategoryEntity
            {
                Name = "Category 3",
                Created = DateTime.UtcNow.AddDays(-1)
            };

            context.Categories.AddRange(category1, category2, category3);
            await context.SaveChangesAsync();

            var sortOption = new SortOption<SortCategory>
            {
                SortBy = SortCategory.Created,
                Direction = SortDirection.Descending
            };

            // Act
            var result = await repository.Search(0, 10, sortOption, "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.TotalCount);
            var categories = result.Data.ToList();
            Assert.Equal("Category 3", categories[0].Name);
            Assert.Equal("Category 2", categories[1].Name);
            Assert.Equal("Category 1", categories[2].Name);
        }

        [Fact]
        public async Task Search_WithPagination_ReturnsCorrectPage()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new CategoryRepository(context, mapper);

            for (int i = 1; i <= 15; i++)
            {
                context.Categories.Add(new CategoryEntity
                {
                    Name = $"Category {i}",
                    Created = DateTime.UtcNow.AddDays(-i)
                });
            }
            await context.SaveChangesAsync();

            var sortOption = new SortOption<SortCategory>
            {
                SortBy = SortCategory.Name,
                Direction = SortDirection.Ascending
            };

            // Act
            var result = await repository.Search(5, 5, sortOption, "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(15, result.TotalCount);
            Assert.Equal(5, result.Data.Count());
        }

        [Fact]
        public async Task Search_WithSearchTermAndSorting_AppliesBothCorrectly()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new CategoryRepository(context, mapper);

            var category1 = new CategoryEntity
            {
                Name = "Tech News",
                Created = DateTime.UtcNow.AddDays(-2)
            };

            var category2 = new CategoryEntity
            {
                Name = "Technology",
                Created = DateTime.UtcNow.AddDays(-1)
            };

            var category3 = new CategoryEntity
            {
                Name = "Sports",
                Created = DateTime.UtcNow
            };

            context.Categories.AddRange(category1, category2, category3);
            await context.SaveChangesAsync();

            var sortOption = new SortOption<SortCategory>
            {
                SortBy = SortCategory.Name,
                Direction = SortDirection.Descending
            };

            // Act
            var result = await repository.Search(0, 10, sortOption, "tech");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            var categories = result.Data.ToList();
            Assert.Equal("Technology", categories[0].Name);
            Assert.Equal("Tech News", categories[1].Name);
        }
    }
}
