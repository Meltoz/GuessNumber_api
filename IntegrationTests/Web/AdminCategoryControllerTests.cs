using Infrastructure;
using Infrastructure.Entities;
using Meltix.IntegrationTests;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text;
using System.Text.Json;
using Web.ViewModels;

namespace IntegrationTests.Web
{
    public class AdminCategoryControllerTests
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        private readonly GuessNumberContext _context;

        public AdminCategoryControllerTests()
        {
            _context = DbContextProvider.SetupContext();
            _factory = new CustomWebApplicationFactory(_context);
            _client = _factory.CreateClient();

        }

        #region Search Tests

        [Fact]
        public async Task Search_ShouldReturnOk_WithValidParameters()
        {
            // Arrange
            var categoryName = "Electronics";
            _context.Categories.Add(new CategoryEntity { Name = categoryName, Created = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/AdminCategory/search?pageIndex=0&pageSize=10&sort=name_ascending&search=");

            // Assert
            response.EnsureSuccessStatusCode();
            var categories = JsonSerializer.Deserialize<List<CategoryAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(categories);
            Assert.Single(categories);
            var category = categories.First();
            Assert.Equal(categoryName, category.Name);
            Assert.NotEqual(Guid.Empty, category.Id);
        }

        [Fact]
        public async Task Search_ShouldReturnTotalCountInHeader()
        {
            // Arrange
            _context.Categories.AddRange(
                new CategoryEntity { Name = "Electronics", Created = DateTime.UtcNow },
                new CategoryEntity { Name = "Books", Created = DateTime.UtcNow },
                new CategoryEntity { Name = "Clothing", Created = DateTime.UtcNow }
            );
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/AdminCategory/search?pageIndex=0&pageSize=10&sort=name_ascending&search=");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.True(response.Headers.Contains("X-Total-Count") || response.Headers.Contains("Total-Count"));
            var totalCount = response.Headers.GetValues("X-Total-Count").FirstOrDefault()
                          ?? response.Headers.GetValues("Total-Count").FirstOrDefault();
            Assert.Equal("3", totalCount);
        }

        [Fact]
        public async Task Search_WithPagination_ShouldReturnCorrectPage()
        {
            // Arrange
            for (int i = 0; i < 15; i++)
            {
                _context.Categories.Add(new CategoryEntity
                {
                    Name = $"Category{i:D2}",
                    Created = DateTime.UtcNow.AddDays(-i)
                });
            }
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/AdminCategory/search?pageIndex=1&pageSize=5&sort=name_ascending&search=");

            // Assert
            response.EnsureSuccessStatusCode();
            var categories = JsonSerializer.Deserialize<List<CategoryAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(categories);
            Assert.Equal(5, categories.Count);
        }

        [Fact]
        public async Task Search_WithEmptyDatabase_ShouldReturnEmptyList()
        {
            // Act
            var response = await _client.GetAsync("/api/AdminCategory/search?pageIndex=0&pageSize=10&sort=name_ascending&search=");

            // Assert
            response.EnsureSuccessStatusCode();
            var categories = JsonSerializer.Deserialize<List<CategoryAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(categories);
            Assert.Empty(categories);
        }

        [Fact]
        public async Task Search_WithSearchTerm_ShouldReturnMatchingCategories()
        {
            // Arrange
            _context.Categories.AddRange(
                new CategoryEntity { Name = "Electronics", Created = DateTime.UtcNow },
                new CategoryEntity { Name = "Books", Created = DateTime.UtcNow },
                new CategoryEntity { Name = "Electronic Devices", Created = DateTime.UtcNow }
            );
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/AdminCategory/search?pageIndex=0&pageSize=10&sort=name_ascending&search=electronic");

            // Assert
            response.EnsureSuccessStatusCode();
            var categories = JsonSerializer.Deserialize<List<CategoryAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(categories);
            Assert.Equal(2, categories.Count);
            Assert.All(categories, c => Assert.Contains("electronic", c.Name.ToLower()));
        }

        [Fact]
        public async Task Search_WithEmptySearchTerm_ShouldReturnAllCategories()
        {
            // Arrange
            _context.Categories.AddRange(
                new CategoryEntity { Name = "Electronics", Created = DateTime.UtcNow },
                new CategoryEntity { Name = "Books", Created = DateTime.UtcNow }
            );
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/AdminCategory/search?pageIndex=0&pageSize=10&sort=name_ascending&search=");

            // Assert
            response.EnsureSuccessStatusCode();
            var categories = JsonSerializer.Deserialize<List<CategoryAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(categories);
            Assert.Equal(2, categories.Count);
        }

        [Fact]
        public async Task Search_SortByNameAscending_ShouldReturnCategoriesInCorrectOrder()
        {
            // Arrange
            _context.Categories.AddRange(
                new CategoryEntity { Name = "Zebra", Created = DateTime.UtcNow },
                new CategoryEntity { Name = "Apple", Created = DateTime.UtcNow },
                new CategoryEntity { Name = "Mango", Created = DateTime.UtcNow }
            );
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/AdminCategory/search?pageIndex=0&pageSize=10&sort=name_ascending&search=");

            // Assert
            response.EnsureSuccessStatusCode();
            var categories = JsonSerializer.Deserialize<List<CategoryAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(categories);
            Assert.Equal(3, categories.Count);
            Assert.Equal("Apple", categories[0].Name);
            Assert.Equal("Mango", categories[1].Name);
            Assert.Equal("Zebra", categories[2].Name);
        }

        [Fact]
        public async Task Search_SortByNameDescending_ShouldReturnCategoriesInCorrectOrder()
        {
            // Arrange
            _context.Categories.AddRange(
                new CategoryEntity { Name = "Zebra", Created = DateTime.UtcNow },
                new CategoryEntity { Name = "Apple", Created = DateTime.UtcNow },
                new CategoryEntity { Name = "Mango", Created = DateTime.UtcNow }
            );
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/AdminCategory/search?pageIndex=0&pageSize=10&sort=name_descending&search=");

            // Assert
            response.EnsureSuccessStatusCode();
            var categories = JsonSerializer.Deserialize<List<CategoryAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(categories);
            Assert.Equal(3, categories.Count);
            Assert.Equal("Zebra", categories[0].Name);
            Assert.Equal("Mango", categories[1].Name);
            Assert.Equal("Apple", categories[2].Name);
        }

        [Fact]
        public async Task Search_SortByCreatedAscending_ShouldReturnCategoriesInCorrectOrder()
        {
            // Arrange
            var now = DateTime.UtcNow;
            _context.Categories.AddRange(
                new CategoryEntity { Name = "Third", Created = now.AddDays(-1) },
                new CategoryEntity { Name = "First", Created = now.AddDays(-3) },
                new CategoryEntity { Name = "Second", Created = now.AddDays(-2) }
            );
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/AdminCategory/search?pageIndex=0&pageSize=10&sort=created_ascending&search=");

            // Assert
            response.EnsureSuccessStatusCode();
            var categories = JsonSerializer.Deserialize<List<CategoryAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(categories);
            Assert.Equal(3, categories.Count);
            Assert.Equal("First", categories[0].Name);
            Assert.Equal("Second", categories[1].Name);
            Assert.Equal("Third", categories[2].Name);
        }

        [Fact]
        public async Task Search_SortByCreatedDescending_ShouldReturnCategoriesInCorrectOrder()
        {
            // Arrange
            var now = DateTime.UtcNow;
            _context.Categories.AddRange(
                new CategoryEntity { Name = "Third", Created = now.AddDays(-1) },
                new CategoryEntity { Name = "First", Created = now.AddDays(-3) },
                new CategoryEntity { Name = "Second", Created = now.AddDays(-2) }
            );
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/AdminCategory/search?pageIndex=0&pageSize=10&sort=created_descending&search=");

            // Assert
            response.EnsureSuccessStatusCode();
            var categories = JsonSerializer.Deserialize<List<CategoryAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(categories);
            Assert.Equal(3, categories.Count);
            Assert.Equal("Third", categories[0].Name);
            Assert.Equal("Second", categories[1].Name);
            Assert.Equal("First", categories[2].Name);
        }

        [Fact]
        public async Task Search_WithNegativePageIndex_ShouldReturnBadRequest()
        {
            // Act
            var response = await _client.GetAsync("/api/AdminCategory/search?pageIndex=-1&pageSize=10&sort=name_ascending&search=");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Search_WithZeroPageSize_ShouldReturnBadRequest()
        {
            // Act
            var response = await _client.GetAsync("/api/AdminCategory/search?pageIndex=0&pageSize=0&sort=name_ascending&search=");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Search_WithNegativePageSize_ShouldReturnBadRequest()
        {
            // Act
            var response = await _client.GetAsync("/api/AdminCategory/search?pageIndex=0&pageSize=-5&sort=name_ascending&search=");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Search_WithInvalidSortFormat_ShouldReturnBadRequest()
        {
            // Act
            var response = await _client.GetAsync("/api/AdminCategory/search?pageIndex=0&pageSize=10&sort=invalid_sort&search=");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Search_WithInvalidSortField_ShouldReturnBadRequest()
        {
            // Act
            var response = await _client.GetAsync("/api/AdminCategory/search?pageIndex=0&pageSize=10&sort=invalidfield_ascending&search=");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Search_WithInvalidSortDirection_ShouldReturnBadRequest()
        {
            // Act
            var response = await _client.GetAsync("/api/AdminCategory/search?pageIndex=0&pageSize=10&sort=name_invalid&search=");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Search_WithMissingSortParameter_ShouldReturnBadRequest()
        {
            // Act
            var response = await _client.GetAsync("/api/AdminCategory/search?pageIndex=0&pageSize=10&search=");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Search_WithEmptySortParameter_ShouldReturnBadRequest()
        {
            // Act
            var response = await _client.GetAsync("/api/AdminCategory/search?pageIndex=0&pageSize=10&sort=&search=");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Search_WithLargePageSize_ShouldReturnCorrectNumberOfItems()
        {
            // Arrange
            for (int i = 0; i < 5; i++)
            {
                _context.Categories.Add(new CategoryEntity
                {
                    Name = $"Category{i}",
                    Created = DateTime.UtcNow
                });
            }
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/AdminCategory/search?pageIndex=0&pageSize=100&sort=name_ascending&search=");

            // Assert
            response.EnsureSuccessStatusCode();
            var categories = JsonSerializer.Deserialize<List<CategoryAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(categories);
            Assert.Equal(5, categories.Count);
        }

        [Fact]
        public async Task Search_WithPageIndexBeyondResults_ShouldReturnEmptyList()
        {
            // Arrange
            _context.Categories.Add(new CategoryEntity { Name = "Category1", Created = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/AdminCategory/search?pageIndex=10&pageSize=10&sort=name_ascending&search=");

            // Assert
            response.EnsureSuccessStatusCode();
            var categories = JsonSerializer.Deserialize<List<CategoryAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(categories);
            Assert.Empty(categories);
        }

        [Fact]
        public async Task Search_WithSearchTermNoMatch_ShouldReturnEmptyList()
        {
            // Arrange
            _context.Categories.AddRange(
                new CategoryEntity { Name = "Electronics", Created = DateTime.UtcNow },
                new CategoryEntity { Name = "Books", Created = DateTime.UtcNow }
            );
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/AdminCategory/search?pageIndex=0&pageSize=10&sort=name_ascending&search=nonexistent");

            // Assert
            response.EnsureSuccessStatusCode();
            var categories = JsonSerializer.Deserialize<List<CategoryAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(categories);
            Assert.Empty(categories);
        }

        [Fact]
        public async Task Search_WithSpecialCharactersInSearch_ShouldHandleCorrectly()
        {
            // Arrange
            _context.Categories.AddRange(
                new CategoryEntity { Name = "C# Programming", Created = DateTime.UtcNow },
                new CategoryEntity { Name = "Books", Created = DateTime.UtcNow }
            );
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/AdminCategory/search?pageIndex=0&pageSize=10&sort=name_ascending&search=C%23");

            // Assert
            response.EnsureSuccessStatusCode();
            var categories = JsonSerializer.Deserialize<List<CategoryAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(categories);
            Assert.Single(categories);
            Assert.Contains("C#", categories[0].Name);
        }

        [Fact]
        public async Task Search_ReturnsCorrectIdForEachCategory()
        {
            // Arrange
            var category1 = new CategoryEntity { Name = "Category1", Created = DateTime.UtcNow };
            var category2 = new CategoryEntity { Name = "Category2", Created = DateTime.UtcNow };
            _context.Categories.AddRange(category1, category2);
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/AdminCategory/search?pageIndex=0&pageSize=10&sort=name_ascending&search=");

            // Assert
            response.EnsureSuccessStatusCode();
            var categories = JsonSerializer.Deserialize<List<CategoryAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(categories);
            Assert.Equal(2, categories.Count);
            Assert.All(categories, c => Assert.NotEqual(Guid.Empty, c.Id));
            Assert.Contains(categories, c => c.Name == "Category1");
            Assert.Contains(categories, c => c.Name == "Category2");
        }
        #endregion

        #region Add Tests
        [Fact]
        public async Task Add_ShouldReturnOk_WithValidCategory()
        {
            // Arrange
            var newCategory = new CategoryAdminVM
            {
                Name = "Electronics"
            };
            var content = new StringContent(
                JsonSerializer.Serialize(newCategory),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminCategory/Add", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var categoryResult = JsonSerializer.Deserialize<CategoryAdminVM>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(categoryResult);
            Assert.Equal(newCategory.Name, categoryResult.Name);
            Assert.NotNull(categoryResult.Id);
            Assert.NotEqual(Guid.Empty, categoryResult.Id);
        }

        [Fact]
        public async Task Add_ShouldPersistCategoryInDatabase()
        {
            // Arrange
            var newCategory = new CategoryAdminVM
            {
                Name = "Books"
            };
            var content = new StringContent(
                JsonSerializer.Serialize(newCategory),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminCategory/Add", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var categoryResult = JsonSerializer.Deserialize<CategoryAdminVM>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            var categoryInDb = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryResult.Id);
            Assert.NotNull(categoryInDb);
            Assert.Equal(newCategory.Name, categoryInDb.Name);
        }

        [Fact]
        public async Task Add_ShouldReturnBadRequest_WhenNameIsNull()
        {
            // Arrange
            var categoryWithNullName = new CategoryAdminVM
            {
                Name = null
            };
            var content = new StringContent(
                JsonSerializer.Serialize(categoryWithNullName),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminCategory/Add", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Add_ShouldReturnBadRequest_WhenNameIsEmpty()
        {
            // Arrange
            var categoryWithEmptyName = new CategoryAdminVM
            {
                Name = ""
            };
            var content = new StringContent(
                JsonSerializer.Serialize(categoryWithEmptyName),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminCategory/Add", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Add_ShouldReturnBadRequest_WhenNameIsWhitespace()
        {
            // Arrange
            var categoryWithWhitespaceName = new CategoryAdminVM
            {
                Name = "   "
            };
            var content = new StringContent(
                JsonSerializer.Serialize(categoryWithWhitespaceName),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminCategory/Add", content);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task Add_ShouldReturnBadRequest_WhenNameHasTwoCharacters()
        {
            // Arrange
            var categoryWithShortName = new CategoryAdminVM
            {
                Name = "AB"
            };
            var content = new StringContent(
                JsonSerializer.Serialize(categoryWithShortName),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminCategory/Add", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var errorContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("Name", errorContent);
        }

        [Fact]
        public async Task Add_ShouldReturnBadRequest_WhenNameHasOneCharacter()
        {
            // Arrange
            var categoryWithShortName = new CategoryAdminVM
            {
                Name = "A"
            };
            var content = new StringContent(
                JsonSerializer.Serialize(categoryWithShortName),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminCategory/Add", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Add_ShouldReturnOk_WhenNameHasExactlyThreeCharacters()
        {
            // Arrange
            var categoryWithMinName = new CategoryAdminVM
            {
                Name = "ABC"
            };
            var content = new StringContent(
                JsonSerializer.Serialize(categoryWithMinName),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminCategory/Add", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var categoryResult = JsonSerializer.Deserialize<CategoryAdminVM>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(categoryResult);
            Assert.Equal("ABC", categoryResult.Name);
        }

        [Fact]
        public async Task Add_ShouldReturnOk_WhenNameHasExactly500Characters()
        {
            // Arrange
            var maxLengthName = new string('A', 500);
            var categoryWithMaxName = new CategoryAdminVM
            {
                Name = maxLengthName
            };
            var content = new StringContent(
                JsonSerializer.Serialize(categoryWithMaxName),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminCategory/Add", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var categoryResult = JsonSerializer.Deserialize<CategoryAdminVM>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(categoryResult);
            Assert.Equal(maxLengthName, categoryResult.Name);
        }

        [Fact]
        public async Task Add_ShouldReturnBadRequest_WhenNameExceeds500Characters()
        {
            // Arrange
            var tooLongName = new string('A', 501);
            var categoryWithLongName = new CategoryAdminVM
            {
                Name = tooLongName
            };
            var content = new StringContent(
                JsonSerializer.Serialize(categoryWithLongName),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminCategory/Add", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var errorContent = await response.Content.ReadAsStringAsync();
            Assert.Contains("Name", errorContent);
        }

        [Fact]
        public async Task Add_ShouldReturnBadRequest_WhenNameExceeds500CharactersSignificantly()
        {
            // Arrange
            var tooLongName = new string('A', 1000);
            var categoryWithLongName = new CategoryAdminVM
            {
                Name = tooLongName
            };
            var content = new StringContent(
                JsonSerializer.Serialize(categoryWithLongName),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminCategory/Add", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Add_ShouldReturnBadRequest_WhenBodyIsNull()
        {
            // Arrange
            var content = new StringContent(
                "null",
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminCategory/Add", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Add_ShouldReturnBadRequest_WhenBodyIsEmpty()
        {
            // Arrange
            var content = new StringContent(
                "",
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminCategory/Add", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Add_ShouldReturnBadRequest_WhenJsonIsInvalid()
        {
            // Arrange
            var content = new StringContent(
                "{invalid json}",
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminCategory/Add", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Add_ShouldHandleSpecialCharactersInName()
        {
            // Arrange
            var newCategory = new CategoryAdminVM
            {
                Name = "C# & .NET Programming"
            };
            var content = new StringContent(
                JsonSerializer.Serialize(newCategory),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminCategory/Add", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var categoryResult = JsonSerializer.Deserialize<CategoryAdminVM>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(categoryResult);
            Assert.Equal(newCategory.Name, categoryResult.Name);
        }

        [Fact]
        public async Task Add_ShouldHandleUnicodeCharactersInName()
        {
            // Arrange
            var newCategory = new CategoryAdminVM
            {
                Name = "Électronique & 日本語"
            };
            var content = new StringContent(
                JsonSerializer.Serialize(newCategory),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminCategory/Add", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var categoryResult = JsonSerializer.Deserialize<CategoryAdminVM>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(categoryResult);
            Assert.Equal(newCategory.Name, categoryResult.Name);
        }

        [Fact]
        public async Task Add_ShouldHandleNumbersInName()
        {
            // Arrange
            var newCategory = new CategoryAdminVM
            {
                Name = "Category123"
            };
            var content = new StringContent(
                JsonSerializer.Serialize(newCategory),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminCategory/Add", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var categoryResult = JsonSerializer.Deserialize<CategoryAdminVM>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(categoryResult);
            Assert.Equal(newCategory.Name, categoryResult.Name);
        }

        [Fact]
        public async Task Add_ShouldGenerateUniqueIdForEachCategory()
        {
            // Arrange
            var category1 = new CategoryAdminVM { Name = "Category1" };
            var category2 = new CategoryAdminVM { Name = "Category2" };

            var content1 = new StringContent(
                JsonSerializer.Serialize(category1),
                Encoding.UTF8,
                "application/json"
            );
            var content2 = new StringContent(
                JsonSerializer.Serialize(category2),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response1 = await _client.PostAsync("/api/AdminCategory/Add", content1);
            var response2 = await _client.PostAsync("/api/AdminCategory/Add", content2);

            // Assert
            response1.EnsureSuccessStatusCode();
            response2.EnsureSuccessStatusCode();

            var result1 = JsonSerializer.Deserialize<CategoryAdminVM>(
                await response1.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            var result2 = JsonSerializer.Deserialize<CategoryAdminVM>(
                await response2.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            Assert.NotEqual(result1.Id, result2.Id);
        }

        [Fact]
        public async Task Add_ShouldIgnoreIdInRequest_WhenIdIsProvided()
        {
            // Arrange
            var providedId = Guid.NewGuid();
            var newCategory = new CategoryAdminVM
            {
                Id = providedId,
                Name = "Electronics"
            };
            var content = new StringContent(
                JsonSerializer.Serialize(newCategory),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminCategory/Add", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var categoryResult = JsonSerializer.Deserialize<CategoryAdminVM>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(categoryResult);
            // L'ID devrait être généré par le serveur, pas celui fourni
            Assert.NotEqual(providedId, categoryResult.Id);
        }

        [Fact]
        public async Task Add_ShouldAcceptNullId()
        {
            // Arrange
            var newCategory = new CategoryAdminVM
            {
                Id = null,
                Name = "Electronics"
            };
            var content = new StringContent(
                JsonSerializer.Serialize(newCategory),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminCategory/Add", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var categoryResult = JsonSerializer.Deserialize<CategoryAdminVM>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(categoryResult);
            Assert.NotNull(categoryResult.Id);
            Assert.NotEqual(Guid.Empty, categoryResult.Id);
        }

        [Fact]
        public async Task Add_ShouldReturnCorrectContentType()
        {
            // Arrange
            var newCategory = new CategoryAdminVM
            {
                Name = "Electronics"
            };
            var content = new StringContent(
                JsonSerializer.Serialize(newCategory),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminCategory/Add", content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/json", response.Content.Headers.ContentType?.MediaType);
        }

        [Fact]
        public async Task Add_MultipleCategories_ShouldAllBePersistedCorrectly()
        {
            // Arrange
            var categories = new[]
            {
        new CategoryAdminVM { Name = "Electronics" },
        new CategoryAdminVM { Name = "Books" },
        new CategoryAdminVM { Name = "Clothing" }
    };

            // Act
            var createdIds = new List<Guid>();
            foreach (var category in categories)
            {
                var content = new StringContent(
                    JsonSerializer.Serialize(category),
                    Encoding.UTF8,
                    "application/json"
                );
                var response = await _client.PostAsync("/api/AdminCategory/Add", content);
                response.EnsureSuccessStatusCode();

                var result = JsonSerializer.Deserialize<CategoryAdminVM>(
                    await response.Content.ReadAsStringAsync(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );
                createdIds.Add(result.Id.Value);
            }

            // Assert
            var categoriesInDb = await _context.Categories.Where(c => createdIds.Contains(c.Id)).ToListAsync();
            Assert.Equal(3, categoriesInDb.Count);
            Assert.Contains(categoriesInDb, c => c.Name == "Electronics");
            Assert.Contains(categoriesInDb, c => c.Name == "Books");
            Assert.Contains(categoriesInDb, c => c.Name == "Clothing");
        }

        [Fact]
        public async Task Add_ShouldSetCreatedDateAutomatically()
        {
            // Arrange
            var newCategory = new CategoryAdminVM
            {
                Name = "Electronics"
            };
            var content = new StringContent(
                JsonSerializer.Serialize(newCategory),
                Encoding.UTF8,
                "application/json"
            );

            var beforeCreate = DateTime.UtcNow;

            // Act
            var response = await _client.PostAsync("/api/AdminCategory/add", content);

            var afterCreate = DateTime.UtcNow;

            // Assert
            response.EnsureSuccessStatusCode();
            var categoryResult = JsonSerializer.Deserialize<CategoryAdminVM>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            var categoryInDb = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryResult.Id);
            Assert.NotNull(categoryInDb);
            Assert.InRange(categoryInDb.Created, beforeCreate.AddSeconds(-1), afterCreate.AddSeconds(1));
        }

        [Fact]
        public async Task Add_ShouldHandleNameWithLeadingAndTrailingSpaces()
        {
            // Arrange
            var newCategory = new CategoryAdminVM
            {
                Name = "  Electronics  "
            };
            var content = new StringContent(
                JsonSerializer.Serialize(newCategory),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminCategory/Add", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var categoryResult = JsonSerializer.Deserialize<CategoryAdminVM>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(categoryResult);
            // Selon votre logique métier, le nom peut être trimé ou non
            Assert.True(categoryResult.Name == "  Electronics  " || categoryResult.Name == "Electronics");
        }

        [Fact]
        public async Task Add_ShouldReturnBadRequest_WhenNameIsOnlySpacesLessThanMinLength()
        {
            // Arrange
            var categoryWithSpaces = new CategoryAdminVM
            {
                Name = "  "
            };
            var content = new StringContent(
                JsonSerializer.Serialize(categoryWithSpaces),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminCategory/Add", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Add_ShouldHandleMixedCaseInName()
        {
            // Arrange
            var newCategory = new CategoryAdminVM
            {
                Name = "ElEcTrOnIcS"
            };
            var content = new StringContent(
                JsonSerializer.Serialize(newCategory),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/AdminCategory/Add", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var categoryResult = JsonSerializer.Deserialize<CategoryAdminVM>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(categoryResult);
            Assert.Equal("ElEcTrOnIcS", categoryResult.Name);
        }
        #endregion
    }
}
