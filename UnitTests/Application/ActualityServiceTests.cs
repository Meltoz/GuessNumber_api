using Application.Exceptions;
using Application.Interfaces.Repository;
using Application.Services;
using Domain;
using Moq;
using Shared;
using Shared.Enums.Sorting;
using System;

namespace UnitTests.Application
{
    public class ActualityServiceTests
    {
        private readonly Mock<IActualityRepository> _actualityRepositoryMock;
        private readonly ActualityService _service;

        public ActualityServiceTests()
        {
            _actualityRepositoryMock = new Mock<IActualityRepository>();
            _service = new ActualityService(_actualityRepositoryMock.Object);
        }

        [Fact]
        public async void Search_ShouldCallSearchRepo()
        {
            // Arrange
            var actualityId = Guid.NewGuid();
            var actuality = new Actuality(actualityId, "test", "testing", DateTime.Now, DateTime.Now.AddDays(1));
            var sort = new SortOption<SortActuality> { Direction = SortDirection.Descending, SortBy= SortActuality.Created};
            var pagedResult = new PagedResult<Actuality> { TotalCount = 1, Data = new List<Actuality>() { actuality } };

            var repoMock = new Mock<IActualityRepository>();
            repoMock.Setup(r => r.Search(
                0,
                10,
                It.IsAny<SortOption<SortActuality>>(),
                It.IsAny<string>()
                )).ReturnsAsync(pagedResult);

            var service = new ActualityService(repoMock.Object);

            // Act
            var actualities = await service.Search(0, 10, sort, "");

            // Assert
            repoMock.Verify(r => r.Search(0, 10, sort, ""), Times.Once);
        }

        [Fact]
        public async void Search_ShouldReturnPagedResultFromRepository()
        {
            // Arrange
            var actualityId = Guid.NewGuid();
            var actuality = new Actuality(actualityId, "test", "testing", DateTime.Now, DateTime.Now.AddDays(1));
            var sort = new SortOption<SortActuality> { Direction = SortDirection.Descending, SortBy = SortActuality.Created };
            var pagedResult = new PagedResult<Actuality> { TotalCount = 1, Data = new List<Actuality>() { actuality } };

            var repoMock = new Mock<IActualityRepository>();
            repoMock.Setup(r => r.Search(
                0,
                10,
                It.IsAny<SortOption<SortActuality>>(),
                It.IsAny<string>()
                )).ReturnsAsync(pagedResult);

            var service = new ActualityService(repoMock.Object);

            // Act
            var actualities = await service.Search(0, 10, sort, "");

            // Assert
            Assert.Equal(pagedResult, actualities);
        }

        [Fact]
        public async Task Search_ShouldThrow_WhenRepositoryThrows()
        {
            // Arrange
            var repoMock = new Mock<IActualityRepository>();
            repoMock.Setup(r => r.Search(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<SortOption<SortActuality>>(), It.IsAny<string>()))
                .ThrowsAsync(new Exception("Repository failed"));

            var service = new ActualityService(repoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.Search(0, 10, new SortOption<SortActuality>(), ""));
        }

        [Fact]
        public async Task CreateNew_ShouldInsertActuality_WithCorrectValues()
        {
            // Arrange
            var title = "Test Title";
            var content = "Test Content";
            var start = new DateTime(2025, 1, 1);
            DateTime? end = new DateTime(2025, 1, 10);

            var expectedActuality = new Actuality(title, content, start, end);
            _actualityRepositoryMock
                .Setup(r => r.InsertAsync(It.IsAny<Actuality>()))
                .ReturnsAsync((Actuality a) => a);

            // Act
            var result = await _service.CreateNew(title, content, start, end);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(title, result.Title);
            Assert.Equal(content, result.Content);
            Assert.Equal(start, result.StartPublish);
            Assert.Equal(end, result.EndPublish);

            _actualityRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<Actuality>()), Times.Once);
        }

        [Fact]
        public async Task CreateNew_ShouldAllowNullEndDate()
        {
            // Arrange
            var title = "Title without end date";
            var content = "Some content";
            var start = DateTime.UtcNow;
            DateTime? end = null;

            _actualityRepositoryMock
                .Setup(r => r.InsertAsync(It.IsAny<Actuality>()))
                .ReturnsAsync((Actuality a) => a);

            // Act
            var result = await _service.CreateNew(title, content, start, end);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(title, result.Title);
            Assert.Null(result.EndPublish);
            _actualityRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<Actuality>()), Times.Once);
        }

        [Fact]
        public async Task CreateNew_ShouldReturnInsertedEntity()
        {
            // Arrange
            var actuality = new Actuality("Title", "Content", DateTime.UtcNow, null);
            _actualityRepositoryMock
                .Setup(r => r.InsertAsync(It.IsAny<Actuality>()))
                .ReturnsAsync(actuality);

            // Act
            var result = await _service.CreateNew("Title", "Content", actuality.StartPublish, null);

            // Assert
            Assert.Same(actuality, result);
        }

        [Fact]
        public async Task CreateNew_ShouldCallRepositoryEvenWithExtremeDates()
        {
            // Arrange
            var start = DateTime.MinValue;
            var end = DateTime.MaxValue;

            _actualityRepositoryMock
                .Setup(r => r.InsertAsync(It.IsAny<Actuality>()))
                .ReturnsAsync((Actuality a) => a);

            // Act
            var result = await _service.CreateNew("Edge case", "content", start, end);

            // Assert
            Assert.Equal(DateTime.MinValue, result.StartPublish);
            Assert.Equal(DateTime.MaxValue, result.EndPublish);
            _actualityRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<Actuality>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrow_WhenEntityNotFound()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var actuality = new Actuality(guid, "Title", "Content", DateTime.Now, null);
            _actualityRepositoryMock
                .Setup(r => r.GetByIdAsync(actuality.Id))
                .ReturnsAsync((Actuality)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.UpdateAsync(actuality));
            _actualityRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Actuality>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateTitle_WhenDifferent()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var existing = new Actuality(guid, "Old Title", "Content", DateTime.Now, null);
            var updated = new Actuality(guid, "New Title", "Content", existing.StartPublish, existing.EndPublish);

            _actualityRepositoryMock.Setup(r => r.GetByIdAsync(existing.Id)).ReturnsAsync(existing);
            _actualityRepositoryMock.Setup(r => r.UpdateAsync(existing)).ReturnsAsync(existing);

            // Act
            var result = await _service.UpdateAsync(updated);

            // Assert
            Assert.Equal("New Title", result.Title);
            _actualityRepositoryMock.Verify(r => r.UpdateAsync(existing), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateContent_WhenDifferent()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var existing = new Actuality(guid, "Title", "Old Content", DateTime.Now, null);
            var updated = new Actuality(guid, "Title", "New Content", existing.StartPublish, existing.EndPublish);

            _actualityRepositoryMock.Setup(r => r.GetByIdAsync(existing.Id)).ReturnsAsync(existing);
            _actualityRepositoryMock.Setup(r => r.UpdateAsync(existing)).ReturnsAsync(existing);

            // Act
            var result = await _service.UpdateAsync(updated);

            // Assert
            Assert.Equal("New Content", result.Content);
            _actualityRepositoryMock.Verify(r => r.UpdateAsync(existing), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdatePublishDates_WhenDifferent()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var existing = new Actuality(guid, "Title", "Content", new DateTime(2025, 1, 1), new DateTime(2025, 1, 5));
            var updated = new Actuality(guid,"Title", "Content", new DateTime(2025, 2, 1), new DateTime(2025, 2, 10));

            _actualityRepositoryMock.Setup(r => r.GetByIdAsync(existing.Id)).ReturnsAsync(existing);
            _actualityRepositoryMock.Setup(r => r.UpdateAsync(existing)).ReturnsAsync(existing);

            // Act
            var result = await _service.UpdateAsync(updated);

            // Assert
            Assert.Equal(updated.StartPublish, result.StartPublish);
            Assert.Equal(updated.EndPublish, result.EndPublish);
            _actualityRepositoryMock.Verify(r => r.UpdateAsync(existing), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldNotChange_WhenNothingDiffers()
        {
            // Arrange
            var dateStart = DateTime.Now;
            var dateEnd = dateStart.AddDays(5);
            var guid = Guid.NewGuid();
            var existing = new Actuality(guid, "Title", "Content", dateStart, dateEnd);
            var updated = new Actuality(guid, "Title", "Content", dateStart, dateEnd);

            _actualityRepositoryMock.Setup(r => r.GetByIdAsync(existing.Id)).ReturnsAsync(existing);
            _actualityRepositoryMock.Setup(r => r.UpdateAsync(existing)).ReturnsAsync(existing);

            // Act
            var result = await _service.UpdateAsync(updated);

            // Assert
            Assert.Equal(existing.Title, result.Title);
            Assert.Equal(existing.Content, result.Content);
            Assert.Equal(existing.StartPublish, result.StartPublish);
            Assert.Equal(existing.EndPublish, result.EndPublish);

            _actualityRepositoryMock.Verify(r => r.UpdateAsync(existing), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdate_WhenNullEndPublish()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var existing = new Actuality(guid, "Title", "Content", new DateTime(2025, 1, 1), new DateTime(2025, 1, 10));
            var updated = new Actuality(guid, "Title", "Content", existing.StartPublish, null);

            _actualityRepositoryMock.Setup(r => r.GetByIdAsync(existing.Id)).ReturnsAsync(existing);
            _actualityRepositoryMock.Setup(r => r.UpdateAsync(existing)).ReturnsAsync(existing);

            // Act
            var result = await _service.UpdateAsync(updated);

            // Assert
            Assert.Null(result.EndPublish);
            _actualityRepositoryMock.Verify(r => r.UpdateAsync(existing), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDelete_WhenIdExist()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var actuality = new Actuality(guid, "Title", "Content", new DateTime(2025, 1, 1), new DateTime(2025, 1, 5));
            _actualityRepositoryMock.Setup(r => r.GetByIdAsync(actuality.Id)).ReturnsAsync(actuality);
            _actualityRepositoryMock.Setup(r => r.Delete(actuality.Id));

            // Act
            await _service.DeleteActualityAsync(actuality.Id);

            // Assert
            _actualityRepositoryMock.Verify(r => r.GetByIdAsync(actuality.Id), Times.Once);
            _actualityRepositoryMock.Verify(r => r.Delete(actuality.Id), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowException_WhenIdNotExist()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var guid2 = Guid.NewGuid();
            var actuality = new Actuality(guid, "Title", "Content", new DateTime(2025, 1, 1), new DateTime(2025, 1, 5));
            _actualityRepositoryMock
                .Setup(r => r.GetByIdAsync(guid2))
                .ReturnsAsync((Actuality)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.DeleteActualityAsync(guid2));
            _actualityRepositoryMock.Verify(r => r.Delete(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task GetActiveActuality_ShouldCallGetActivesOnRepository()
        {
            // Arrange
            var actualities = new List<Actuality>
            {
                new Actuality(Guid.NewGuid(), "Active 1", "Content 1", DateTime.UtcNow.AddDays(-5), DateTime.UtcNow.AddDays(5)),
                new Actuality(Guid.NewGuid(), "Active 2", "Content 2", DateTime.UtcNow.AddDays(-2), null)
            };

            _actualityRepositoryMock
                .Setup(r => r.GetActives())
                .ReturnsAsync(actualities);

            // Act
            var result = await _service.GetActiveActualities();

            // Assert
            _actualityRepositoryMock.Verify(r => r.GetActives(), Times.Once);
        }

        [Fact]
        public async Task GetActiveActuality_ShouldReturnActiveActualities()
        {
            // Arrange
            var actuality1 = new Actuality(Guid.NewGuid(), "Active 1", "Content 1", DateTime.UtcNow.AddDays(-5), DateTime.UtcNow.AddDays(5));
            var actuality2 = new Actuality(Guid.NewGuid(), "Active 2", "Content 2", DateTime.UtcNow.AddDays(-2), null);
            var actualities = new List<Actuality> { actuality1, actuality2 };

            _actualityRepositoryMock
                .Setup(r => r.GetActives())
                .ReturnsAsync(actualities);

            // Act
            var result = await _service.GetActiveActualities();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, a => a.Title == "Active 1");
            Assert.Contains(result, a => a.Title == "Active 2");
        }

        [Fact]
        public async Task GetActiveActuality_ShouldReturnEmptyCollection_WhenNoActiveActualities()
        {
            // Arrange
            var emptyList = new List<Actuality>();

            _actualityRepositoryMock
                .Setup(r => r.GetActives())
                .ReturnsAsync(emptyList);

            // Act
            var result = await _service.GetActiveActualities();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _actualityRepositoryMock.Verify(r => r.GetActives(), Times.Once);
        }

        [Fact]
        public async Task GetActiveActuality_ShouldReturnSameCollectionFromRepository()
        {
            // Arrange
            var actualities = new List<Actuality>
            {
                new Actuality(Guid.NewGuid(), "Test", "Content", DateTime.UtcNow.AddDays(-5), DateTime.UtcNow.AddDays(5))
            };

            _actualityRepositoryMock
                .Setup(r => r.GetActives())
                .ReturnsAsync(actualities);

            // Act
            var result = await _service.GetActiveActualities();

            // Assert
            Assert.Same(actualities, result);
        }

        [Fact]
        public async Task GetActiveActuality_ShouldThrow_WhenRepositoryThrows()
        {
            // Arrange
            _actualityRepositoryMock
                .Setup(r => r.GetActives())
                .ThrowsAsync(new Exception("Repository error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.GetActiveActualities());
            _actualityRepositoryMock.Verify(r => r.GetActives(), Times.Once);
        }

        [Fact]
        public async Task GetActiveActuality_ShouldReturnActualitiesWithCorrectProperties()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var startDate = DateTime.UtcNow.AddDays(-5);
            var endDate = DateTime.UtcNow.AddDays(5);
            var actuality = new Actuality(guid, "Test Title", "Test Content", startDate, endDate);
            var actualities = new List<Actuality> { actuality };

            _actualityRepositoryMock
                .Setup(r => r.GetActives())
                .ReturnsAsync(actualities);

            // Act
            var result = await _service.GetActiveActualities();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);

            var firstActuality = result.First();
            Assert.Equal("Test Title", firstActuality.Title);
            Assert.Equal("Test Content", firstActuality.Content);
            Assert.Equal(startDate, firstActuality.StartPublish);
            Assert.Equal(endDate, firstActuality.EndPublish);
        }

        [Fact]
        public async Task GetActiveActuality_ShouldHandleActualitiesWithNullEndDate()
        {
            // Arrange
            var actuality = new Actuality(Guid.NewGuid(), "No End", "Content", DateTime.UtcNow.AddDays(-5), null);
            var actualities = new List<Actuality> { actuality };

            _actualityRepositoryMock
                .Setup(r => r.GetActives())
                .ReturnsAsync(actualities);

            // Act
            var result = await _service.GetActiveActualities();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Null(result.First().EndPublish);
        }
    }
}
