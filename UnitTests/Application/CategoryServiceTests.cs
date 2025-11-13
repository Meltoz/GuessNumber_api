using Application.Exceptions;
using Application.Interfaces.Repository;
using Application.Services;
using Domain.Party;
using Moq;
using Shared;
using Shared.Enums.Sorting;

namespace UnitTests.Application
{
    public class CategoryServiceTests
    {
        #region Search Tests
        [Fact]
        public async Task Search_ShouldCallRepositorySearch()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new Category(categoryId, "Test Category");
            var sortOption = new SortOption<SortCategory> 
            { 
                SortBy = SortCategory.Name, 
                Direction = SortDirection.Ascending 
            };
            var pagedResult = new PagedResult<Category>
            {
                TotalCount = 1,
                Data = new List<Category>() { category }
            };

            var repoMock = new Mock<ICategoryRepository>();
            repoMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortCategory>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            var service = new CategoryService(repoMock.Object);

            // Act
            var result = await service.Search(0, 10, sortOption, "");

            // Assert
            repoMock.Verify(r => r.Search(0, 10, sortOption, ""), Times.Once);
        }

        [Fact]
        public async Task Search_ShouldReturnPagedResultFromRepository()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new Category(categoryId, "Test Category");
            var sortOption = new SortOption<SortCategory> { SortBy = SortCategory.Name, Direction = SortDirection.Ascending };
            var pagedResult = new PagedResult<Category>
            {
                TotalCount = 1,
                Data = new List<Category>() { category }
            };

            var repoMock = new Mock<ICategoryRepository>();
            repoMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortCategory>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            var service = new CategoryService(repoMock.Object);

            // Act
            var result = await service.Search(0, 10, sortOption, "");

            // Assert
            Assert.Equal(pagedResult, result);
        }

        [Fact]
        public async Task Search_ShouldThrow_WhenRepositoryThrows()
        {
            // Arrange
            var sortOption = new SortOption<SortCategory>
            {
                SortBy = SortCategory.Name, 
                Direction = SortDirection.Ascending 
            };
            var repoMock = new Mock<ICategoryRepository>();
            repoMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortCategory>>(),
                It.IsAny<string>()
            )).ThrowsAsync(new Exception("Repository failed"));

            var service = new CategoryService(repoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.Search(0, 10, sortOption, ""));
        }

        [Fact]
        public async Task Search_ShouldCalculateSkipCorrectly_ForFirstPage()
        {
            // Arrange
            var sortOption = new SortOption<SortCategory>{
                SortBy = SortCategory.Name, 
                Direction = SortDirection.Ascending 
            };
            var pagedResult = new PagedResult<Category>
            {
                TotalCount = 0,
                Data = new List<Category>()
            };

            var repoMock = new Mock<ICategoryRepository>();
            repoMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortCategory>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            var service = new CategoryService(repoMock.Object);

            // Act
            await service.Search(0, 10, sortOption, "");

            // Assert
            repoMock.Verify(r => r.Search(0, 10, sortOption, ""), Times.Once);
        }

        [Fact]
        public async Task Search_ShouldCalculateSkipCorrectly_ForSecondPage()
        {
            // Arrange
            var sortOption = new SortOption<SortCategory>
            {
                SortBy = SortCategory.Name, 
                Direction = SortDirection.Ascending 
            };
            var pagedResult = new PagedResult<Category>
            {
                TotalCount = 0,
                Data = new List<Category>()
            };

            var repoMock = new Mock<ICategoryRepository>();
            repoMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortCategory>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            var service = new CategoryService(repoMock.Object);

            // Act
            await service.Search(1, 10, sortOption, "");

            // Assert
            repoMock.Verify(r => r.Search(10, 10, sortOption, ""), Times.Once);
        }

        [Fact]
        public async Task Search_ShouldPassSearchTermToRepository()
        {
            // Arrange
            var searchTerm = "electronics";
            var sortOption = new SortOption<SortCategory>
            {
                SortBy = SortCategory.Name, 
                Direction = SortDirection.Ascending
            };
            var pagedResult = new PagedResult<Category>
            {
                TotalCount = 0,
                Data = new List<Category>()
            };

            var repoMock = new Mock<ICategoryRepository>();
            repoMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortCategory>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            var service = new CategoryService(repoMock.Object);

            // Act
            await service.Search(0, 10, sortOption, searchTerm);

            // Assert
            repoMock.Verify(r => r.Search(0, 10, sortOption, searchTerm), Times.Once);
        }

        [Fact]
        public async Task Search_ShouldPassEmptySearchTerm_WhenSearchIsEmpty()
        {
            // Arrange
            var sortOption = new SortOption<SortCategory>
            {
                SortBy = SortCategory.Name, 
                Direction = SortDirection.Ascending 
            };
            var pagedResult = new PagedResult<Category>
            {
                TotalCount = 0,
                Data = new List<Category>()
            };

            var repoMock = new Mock<ICategoryRepository>();
            repoMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortCategory>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            var service = new CategoryService(repoMock.Object);

            // Act
            await service.Search(0, 10, sortOption, "");

            // Assert
            repoMock.Verify(r => r.Search(0, 10, sortOption, ""), Times.Once);
        }

        [Fact]
        public async Task Search_ShouldHandleDifferentPageSizes()
        {
            // Arrange
            var sortOption = new SortOption<SortCategory>
            {
                SortBy = SortCategory.Name, 
                Direction = SortDirection.Ascending 
            };
            var pagedResult = new PagedResult<Category>
            {
                TotalCount = 0,
                Data = new List<Category>()
            };

            var repoMock = new Mock<ICategoryRepository>();
            repoMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortCategory>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            var service = new CategoryService(repoMock.Object);

            // Act
            await service.Search(2, 25, sortOption, "");

            // Assert
            repoMock.Verify(r => r.Search(50, 25, sortOption, ""), Times.Once);
        }

        [Fact]
        public async Task Search_ShouldPassCorrectSortOption_WithDescendingOrder()
        {
            // Arrange
            var sortOption = new SortOption<SortCategory>
            {
                SortBy = SortCategory.Name,
                Direction = SortDirection.Descending
            };
            var pagedResult = new PagedResult<Category>
            {
                TotalCount = 0,
                Data = new List<Category>()
            };

            var repoMock = new Mock<ICategoryRepository>();
            repoMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortCategory>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            var service = new CategoryService(repoMock.Object);

            // Act
            await service.Search(0, 10, sortOption, "");

            // Assert
            repoMock.Verify(r => r.Search(0, 10, sortOption, ""), Times.Once);
        }
        #endregion

        #region CreateNew Tests
        [Fact]
        public async Task CreateNew_ShouldCallRepositoryInsertAsync()
        {
            // Arrange
            var categoryName = "Electronics";
            var category = new Category(categoryName);

            var repoMock = new Mock<ICategoryRepository>();
            repoMock.Setup(r => r.InsertAsync(It.IsAny<Category>()))
                .ReturnsAsync(category);

            var service = new CategoryService(repoMock.Object);

            // Act
            await service.CreateNewAsync(categoryName);

            // Assert
            repoMock.Verify(r => r.InsertAsync(It.IsAny<Category>()), Times.Once);
        }

        [Fact]
        public async Task CreateNew_ShouldPassCategoryWithCorrectNameToRepository()
        {
            // Arrange
            var categoryName = "Electronics";
            Category capturedCategory = null;

            var repoMock = new Mock<ICategoryRepository>();
            repoMock.Setup(r => r.InsertAsync(It.IsAny<Category>()))
                .Callback<Category>(c => capturedCategory = c)
                .ReturnsAsync((Category c) => c);

            var service = new CategoryService(repoMock.Object);

            // Act
            await service.CreateNewAsync(categoryName);

            // Assert
            Assert.NotNull(capturedCategory);
            Assert.Equal(categoryName, capturedCategory.Name);
        }

        [Fact]
        public async Task CreateNew_ShouldReturnCategoryFromRepository()
        {
            // Arrange
            var categoryName = "Electronics";
            var categoryId = Guid.NewGuid();
            var expectedCategory = new Category(categoryId, categoryName);

            var repoMock = new Mock<ICategoryRepository>();
            repoMock.Setup(r => r.InsertAsync(It.IsAny<Category>()))
                .ReturnsAsync(expectedCategory);

            var service = new CategoryService(repoMock.Object);

            // Act
            var result = await service.CreateNewAsync(categoryName);

            // Assert
            Assert.Equal(expectedCategory, result);
        }

        [Fact]
        public async Task CreateNew_ShouldReturnCategoryWithCorrectName()
        {
            // Arrange
            var categoryName = "Books";
            var repoMock = new Mock<ICategoryRepository>();
            repoMock.Setup(r => r.InsertAsync(It.IsAny<Category>()))
                .ReturnsAsync((Category c) => c);

            var service = new CategoryService(repoMock.Object);

            // Act
            var result = await service.CreateNewAsync(categoryName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(categoryName, result.Name);
        }

        [Fact]
        public async Task CreateNew_ShouldThrow_WhenRepositoryThrows()
        {
            // Arrange
            var categoryName = "Electronics";
            var repoMock = new Mock<ICategoryRepository>();
            repoMock.Setup(r => r.InsertAsync(It.IsAny<Category>()))
                .ThrowsAsync(new Exception("Database error"));

            var service = new CategoryService(repoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.CreateNewAsync(categoryName));
        }

        [Fact]
        public async Task CreateNew_ShouldCreateNewCategoryInstanceBeforeInserting()
        {
            // Arrange
            var categoryName = "Electronics";
            Category capturedCategory = null;

            var repoMock = new Mock<ICategoryRepository>();
            repoMock.Setup(r => r.InsertAsync(It.IsAny<Category>()))
                .Callback<Category>(c => capturedCategory = c)
                .ReturnsAsync((Category c) => c);

            var service = new CategoryService(repoMock.Object);

            // Act
            await service.CreateNewAsync(categoryName);

            // Assert
            Assert.NotNull(capturedCategory);
            Assert.IsType<Category>(capturedCategory);
        }

        [Fact]
        public async Task CreateNew_ShouldCallRepositoryOnlyOnce()
        {
            // Arrange
            var categoryName = "Electronics";
            var repoMock = new Mock<ICategoryRepository>();
            repoMock.Setup(r => r.InsertAsync(It.IsAny<Category>()))
                .ReturnsAsync((Category c) => c);

            var service = new CategoryService(repoMock.Object);

            // Act
            await service.CreateNewAsync(categoryName);

            // Assert
            repoMock.Verify(r => r.InsertAsync(It.IsAny<Category>()), Times.Once);
            repoMock.VerifyNoOtherCalls();
        }

        [Fact]
        public async Task CreateNew_ShouldNotModifyReturnedCategoryFromRepository()
        {
            // Arrange
            var categoryName = "Electronics";
            var categoryId = Guid.NewGuid();
            var repositoryCategory = new Category(categoryId, categoryName);

            var repoMock = new Mock<ICategoryRepository>();
            repoMock.Setup(r => r.InsertAsync(It.IsAny<Category>()))
                .ReturnsAsync(repositoryCategory);

            var service = new CategoryService(repoMock.Object);

            // Act
            var result = await service.CreateNewAsync(categoryName);

            // Assert
            Assert.Same(repositoryCategory, result);
            Assert.Equal(categoryId, result.Id);
            Assert.Equal(categoryName, result.Name);
        }

        [Fact]
        public async Task CreateNew_WithDifferentNames_ShouldCreateDifferentCategories()
        {
            // Arrange
            var categoryName1 = "Electronics";
            var categoryName2 = "Books";
            var categories = new List<Category>();

            var repoMock = new Mock<ICategoryRepository>();
            repoMock.Setup(r => r.InsertAsync(It.IsAny<Category>()))
                .Callback<Category>(c => categories.Add(c))
                .ReturnsAsync((Category c) => c);

            var service = new CategoryService(repoMock.Object);

            // Act
            await service.CreateNewAsync(categoryName1);
            await service.CreateNewAsync(categoryName2);

            // Assert
            Assert.Equal(2, categories.Count);
            Assert.Equal(categoryName1, categories[0].Name);
            Assert.Equal(categoryName2, categories[1].Name);
        }

        [Fact]
        public async Task CreateNew_ShouldReturnCategoryWithAllProperties()
        {
            // Arrange
            var categoryName = "Electronics";
            var repoMock = new Mock<ICategoryRepository>();
            repoMock.Setup(r => r.InsertAsync(It.IsAny<Category>()))
                .ReturnsAsync((Category c) => c);

            var service = new CategoryService(repoMock.Object);

            // Act
            var result = await service.CreateNewAsync(categoryName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(categoryName, result.Name);
        }
        #endregion

        #region Update Tests

        [Fact]
        public async Task UpdateAsync_WithNullCategory_ShouldThrowArgumentException()
        {
            // Arrange
            var repoMock = new Mock<ICategoryRepository>();
            var service = new CategoryService(repoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateAsync(null));
        }

        [Fact]
        public async Task UpdateAsync_WithEmptyId_ShouldThrowArgumentException()
        {
            // Arrange
            var category = new Category(Guid.Empty, "Test Category");
            var repoMock = new Mock<ICategoryRepository>();
            var service = new CategoryService(repoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.UpdateAsync(category));
        }

        [Fact]
        public async Task UpdateAsync_WithNonExistentCategory_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new Category(categoryId, "Test Category");
            var repoMock = new Mock<ICategoryRepository>();
            repoMock.Setup(r => r.GetByIdAsync(categoryId))
                .ReturnsAsync((Category)null);
            var service = new CategoryService(repoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => service.UpdateAsync(category));
        }

        [Fact]
        public async Task UpdateAsync_WithSameName_ShouldNotCallChangeName()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var existingCategory = new Category(categoryId, "Test Category");
            var categoryToUpdate = new Category(categoryId, "Test Category");
            var repoMock = new Mock<ICategoryRepository>();
            repoMock.Setup(r => r.GetByIdAsync(categoryId))
                .ReturnsAsync(existingCategory);
            repoMock.Setup(r => r.UpdateAsync(It.IsAny<Category>()))
                .ReturnsAsync(existingCategory);
            var service = new CategoryService(repoMock.Object);

            // Act
            var result = await service.UpdateAsync(categoryToUpdate);

            // Assert
            repoMock.Verify(r => r.UpdateAsync(It.IsAny<Category>()), Times.Once);
            Assert.Equal("Test Category", result.Name);
        }

        [Fact]
        public async Task UpdateAsync_WithDifferentName_ShouldCallChangeName()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var existingCategory = new Category(categoryId, "Old Name");
            var categoryToUpdate = new Category(categoryId, "New Name");
            var repoMock = new Mock<ICategoryRepository>();
            repoMock.Setup(r => r.GetByIdAsync(categoryId))
                .ReturnsAsync(existingCategory);
            repoMock.Setup(r => r.UpdateAsync(It.IsAny<Category>()))
                .ReturnsAsync((Category c) => c);
            var service = new CategoryService(repoMock.Object);

            // Act
            var result = await service.UpdateAsync(categoryToUpdate);

            // Assert
            repoMock.Verify(r => r.UpdateAsync(It.IsAny<Category>()), Times.Once);
            Assert.Equal("New Name", result.Name);
        }

        [Fact]
        public async Task UpdateAsync_ShouldCallRepositoryUpdate()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var existingCategory = new Category(categoryId, "Test Category");
            var categoryToUpdate = new Category(categoryId, "Updated Category");
            var repoMock = new Mock<ICategoryRepository>();
            repoMock.Setup(r => r.GetByIdAsync(categoryId))
                .ReturnsAsync(existingCategory);
            repoMock.Setup(r => r.UpdateAsync(It.IsAny<Category>()))
                .ReturnsAsync(existingCategory);
            var service = new CategoryService(repoMock.Object);

            // Act
            await service.UpdateAsync(categoryToUpdate);

            // Assert
            repoMock.Verify(r => r.UpdateAsync(It.Is<Category>(c => c.Id == categoryId)), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldReturnUpdatedCategory()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var existingCategory = new Category(categoryId, "Test Category");
            var categoryToUpdate = new Category(categoryId, "Updated Category");
            var updatedCategory = new Category(categoryId, "Updated Category");
            var repoMock = new Mock<ICategoryRepository>();
            repoMock.Setup(r => r.GetByIdAsync(categoryId))
                .ReturnsAsync(existingCategory);
            repoMock.Setup(r => r.UpdateAsync(It.IsAny<Category>()))
                .ReturnsAsync(updatedCategory);
            var service = new CategoryService(repoMock.Object);

            // Act
            var result = await service.UpdateAsync(categoryToUpdate);

            // Assert
            Assert.Equal(updatedCategory, result);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task DeleteAsync_WithEmptyId_ShouldThrowArgumentException()
        {
            // Arrange
            var repoMock = new Mock<ICategoryRepository>();
            var service = new CategoryService(repoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.DeleteAsync(Guid.Empty));
        }

        [Fact]
        public async Task DeleteAsync_WithNonExistentCategory_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var repoMock = new Mock<ICategoryRepository>();
            repoMock.Setup(r => r.GetByIdAsync(categoryId))
                .ReturnsAsync((Category)null);
            var service = new CategoryService(repoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => service.DeleteAsync(categoryId));
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldCallRepositoryDelete()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new Category(categoryId, "Test Category");
            var repoMock = new Mock<ICategoryRepository>();
            repoMock.Setup(r => r.GetByIdAsync(categoryId))
                .ReturnsAsync(category);
            var service = new CategoryService(repoMock.Object);

            // Act
            await service.DeleteAsync(categoryId);

            // Assert
            repoMock.Verify(r => r.Delete(categoryId), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldCallGetByIdBeforeDelete()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new Category(categoryId, "Test Category");
            var repoMock = new Mock<ICategoryRepository>();
            repoMock.Setup(r => r.GetByIdAsync(categoryId))
                .ReturnsAsync(category);
            var service = new CategoryService(repoMock.Object);

            // Act
            await service.DeleteAsync(categoryId);

            // Assert
            repoMock.Verify(r => r.GetByIdAsync(categoryId), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WithValidId_ShouldNotCallRepositoryIfGetByIdThrows()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var repoMock = new Mock<ICategoryRepository>();
            repoMock.Setup(r => r.GetByIdAsync(categoryId))
                .ReturnsAsync((Category)null);
            var service = new CategoryService(repoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => service.DeleteAsync(categoryId));
            repoMock.Verify(r => r.Delete(It.IsAny<Guid>()), Times.Never);
        }

        #endregion

        #region GetById 

        [Fact]
        public async Task GetByIdAsync_WithEmptyId_ShouldThrowArgumentException()
        {
            // Arrange
            var repoMock = new Mock<ICategoryRepository>();
            var service = new CategoryService(repoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => service.GetByIdAsync(Guid.Empty));
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistentCategory_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var repoMock = new Mock<ICategoryRepository>();
            repoMock.Setup(r => r.GetByIdAsync(categoryId))
                .ReturnsAsync((Category)null);
            var service = new CategoryService(repoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => service.GetByIdAsync(categoryId));
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnCategory()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var expectedCategory = new Category(categoryId, "Test Category");
            var repoMock = new Mock<ICategoryRepository>();
            repoMock.Setup(r => r.GetByIdAsync(categoryId))
                .ReturnsAsync(expectedCategory);
            var service = new CategoryService(repoMock.Object);

            // Act
            var result = await service.GetByIdAsync(categoryId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedCategory.Id, result.Id);
            Assert.Equal(expectedCategory.Name, result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldCallRepositoryOnce()
        {
            // Arrange
            var categoryId = Guid.NewGuid();
            var category = new Category(categoryId, "Test Category");
            var repoMock = new Mock<ICategoryRepository>();
            repoMock.Setup(r => r.GetByIdAsync(categoryId))
                .ReturnsAsync(category);
            var service = new CategoryService(repoMock.Object);

            // Act
            await service.GetByIdAsync(categoryId);

            // Assert
            repoMock.Verify(r => r.GetByIdAsync(categoryId), Times.Once);
        }
        #endregion
    }
}
