using Application.Exceptions;
using Application.Interfaces.Repository;
using Application.Services;
using Domain;
using Moq;
using Shared;
using Shared.Enums.Sorting;

namespace UnitTests.Application
{
    public class CommunicationServiceTests
    {
        private readonly Mock<ICommunicationRepository> _communicationRepositoryMock;
        private readonly CommunicationService _service;

        public CommunicationServiceTests()
        {
            _communicationRepositoryMock = new Mock<ICommunicationRepository>();
            _service = new CommunicationService(_communicationRepositoryMock.Object);
        }

        #region Search Tests - Basic Functionality

        [Fact]
        public async Task Search_ShouldCallSearchRepo()
        {
            // Arrange
            var communicationId = Guid.NewGuid();
            var communication = new Communication(
                communicationId,
                "test",
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(1)
            );
            var sort = new SortOption<SortCommunication>
            {
                Direction = SortDirection.Descending,
                SortBy = SortCommunication.Created
            };
            var pagedResult = new PagedResult<Communication>
            {
                TotalCount = 1,
                Data = new List<Communication> { communication }
            };

            var repoMock = new Mock<ICommunicationRepository>();
            repoMock.Setup(r => r.Search(
                0,
                10,
                It.IsAny<SortOption<SortCommunication>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            var service = new CommunicationService(repoMock.Object);

            // Act
            var communications = await service.Search(0, 10, sort, "");

            // Assert
            repoMock.Verify(r => r.Search(0, 10, sort, ""), Times.Once);
        }

        [Fact]
        public async Task Search_ShouldReturnPagedResultFromRepository()
        {
            // Arrange
            var communicationId = Guid.NewGuid();
            var communication = new Communication(
                communicationId,
                "test",
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(1)
            );
            var sort = new SortOption<SortCommunication>
            {
                Direction = SortDirection.Descending,
                SortBy = SortCommunication.Created
            };
            var pagedResult = new PagedResult<Communication>
            {
                TotalCount = 1,
                Data = new List<Communication> { communication }
            };

            var repoMock = new Mock<ICommunicationRepository>();
            repoMock.Setup(r => r.Search(
                0,
                10,
                It.IsAny<SortOption<SortCommunication>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            var service = new CommunicationService(repoMock.Object);

            // Act
            var communications = await service.Search(0, 10, sort, "");

            // Assert
            Assert.Equal(pagedResult, communications);
        }

        #endregion

        #region Search Tests - Skip Calculation

        [Fact]
        public async Task Search_WithPageIndex0_ShouldCallRepoWithSkip0()
        {
            // Arrange
            var sort = new SortOption<SortCommunication>
            {
                Direction = SortDirection.Ascending,
                SortBy = SortCommunication.Created
            };
            var pagedResult = new PagedResult<Communication>
            {
                TotalCount = 0,
                Data = new List<Communication>()
            };

            _communicationRepositoryMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortCommunication>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            // Act
            await _service.Search(0, 10, sort, "");

            // Assert
            _communicationRepositoryMock.Verify(r => r.Search(0, 10, sort, ""), Times.Once);
        }

        [Fact]
        public async Task Search_WithPageIndex1_ShouldCallRepoWithCorrectSkip()
        {
            // Arrange
            var sort = new SortOption<SortCommunication>
            {
                Direction = SortDirection.Ascending,
                SortBy = SortCommunication.Created
            };
            var pagedResult = new PagedResult<Communication>
            {
                TotalCount = 0,
                Data = new List<Communication>()
            };

            _communicationRepositoryMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortCommunication>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            // Act
            await _service.Search(1, 10, sort, "");

            // Assert
            // skip should be pageIndex * pageSize = 1 * 10 = 10
            _communicationRepositoryMock.Verify(r => r.Search(10, 10, sort, ""), Times.Once);
        }

        [Fact]
        public async Task Search_WithPageIndex2AndPageSize20_ShouldCallRepoWithCorrectSkip()
        {
            // Arrange
            var sort = new SortOption<SortCommunication>
            {
                Direction = SortDirection.Ascending,
                SortBy = SortCommunication.Created
            };
            var pagedResult = new PagedResult<Communication>
            {
                TotalCount = 0,
                Data = new List<Communication>()
            };

            _communicationRepositoryMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortCommunication>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            // Act
            await _service.Search(2, 20, sort, "");

            // Assert
            // skip should be pageIndex * pageSize = 2 * 20 = 40
            _communicationRepositoryMock.Verify(r => r.Search(40, 20, sort, ""), Times.Once);
        }

        [Fact]
        public async Task Search_WithPageIndex5AndPageSize5_ShouldCallRepoWithCorrectSkip()
        {
            // Arrange
            var sort = new SortOption<SortCommunication>
            {
                Direction = SortDirection.Ascending,
                SortBy = SortCommunication.Created
            };
            var pagedResult = new PagedResult<Communication>
            {
                TotalCount = 0,
                Data = new List<Communication>()
            };

            _communicationRepositoryMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortCommunication>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            // Act
            await _service.Search(5, 5, sort, "");

            // Assert
            // skip should be pageIndex * pageSize = 5 * 5 = 25
            _communicationRepositoryMock.Verify(r => r.Search(25, 5, sort, ""), Times.Once);
        }

        #endregion

        #region Search Tests - Sort Options

        [Fact]
        public async Task Search_WithSortByCreatedAscending_ShouldPassCorrectSortOption()
        {
            // Arrange
            var sort = new SortOption<SortCommunication>
            {
                Direction = SortDirection.Ascending,
                SortBy = SortCommunication.Created
            };
            var pagedResult = new PagedResult<Communication>
            {
                TotalCount = 0,
                Data = new List<Communication>()
            };

            _communicationRepositoryMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortCommunication>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            // Act
            await _service.Search(0, 10, sort, "");

            // Assert
            _communicationRepositoryMock.Verify(r => r.Search(
                0,
                10,
                It.Is<SortOption<SortCommunication>>(s =>
                    s.SortBy == SortCommunication.Created &&
                    s.Direction == SortDirection.Ascending
                ),
                ""
            ), Times.Once);
        }

        [Fact]
        public async Task Search_WithSortByCreatedDescending_ShouldPassCorrectSortOption()
        {
            // Arrange
            var sort = new SortOption<SortCommunication>
            {
                Direction = SortDirection.Descending,
                SortBy = SortCommunication.Created
            };
            var pagedResult = new PagedResult<Communication>
            {
                TotalCount = 0,
                Data = new List<Communication>()
            };

            _communicationRepositoryMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortCommunication>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            // Act
            await _service.Search(0, 10, sort, "");

            // Assert
            _communicationRepositoryMock.Verify(r => r.Search(
                0,
                10,
                It.Is<SortOption<SortCommunication>>(s =>
                    s.SortBy == SortCommunication.Created &&
                    s.Direction == SortDirection.Descending
                ),
                ""
            ), Times.Once);
        }

        [Fact]
        public async Task Search_WithSortByActiveAscending_ShouldPassCorrectSortOption()
        {
            // Arrange
            var sort = new SortOption<SortCommunication>
            {
                Direction = SortDirection.Ascending,
                SortBy = SortCommunication.Active
            };
            var pagedResult = new PagedResult<Communication>
            {
                TotalCount = 0,
                Data = new List<Communication>()
            };

            _communicationRepositoryMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortCommunication>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            // Act
            await _service.Search(0, 10, sort, "");

            // Assert
            _communicationRepositoryMock.Verify(r => r.Search(
                0,
                10,
                It.Is<SortOption<SortCommunication>>(s =>
                    s.SortBy == SortCommunication.Active &&
                    s.Direction == SortDirection.Ascending
                ),
                ""
            ), Times.Once);
        }

        [Fact]
        public async Task Search_WithSortByActiveDescending_ShouldPassCorrectSortOption()
        {
            // Arrange
            var sort = new SortOption<SortCommunication>
            {
                Direction = SortDirection.Descending,
                SortBy = SortCommunication.Active
            };
            var pagedResult = new PagedResult<Communication>
            {
                TotalCount = 0,
                Data = new List<Communication>()
            };

            _communicationRepositoryMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortCommunication>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            // Act
            await _service.Search(0, 10, sort, "");

            // Assert
            _communicationRepositoryMock.Verify(r => r.Search(
                0,
                10,
                It.Is<SortOption<SortCommunication>>(s =>
                    s.SortBy == SortCommunication.Active &&
                    s.Direction == SortDirection.Descending
                ),
                ""
            ), Times.Once);
        }

        #endregion

        #region Search Tests - Search Term

        [Fact]
        public async Task Search_WithSearchTerm_ShouldPassSearchTermToRepository()
        {
            // Arrange
            var sort = new SortOption<SortCommunication>
            {
                Direction = SortDirection.Ascending,
                SortBy = SortCommunication.Created
            };
            var pagedResult = new PagedResult<Communication>
            {
                TotalCount = 0,
                Data = new List<Communication>()
            };

            _communicationRepositoryMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortCommunication>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            // Act
            await _service.Search(0, 10, sort, "test search");

            // Assert
            _communicationRepositoryMock.Verify(r => r.Search(0, 10, sort, "test search"), Times.Once);
        }

        [Fact]
        public async Task Search_WithEmptySearchTerm_ShouldPassEmptyStringToRepository()
        {
            // Arrange
            var sort = new SortOption<SortCommunication>
            {
                Direction = SortDirection.Ascending,
                SortBy = SortCommunication.Created
            };
            var pagedResult = new PagedResult<Communication>
            {
                TotalCount = 0,
                Data = new List<Communication>()
            };

            _communicationRepositoryMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortCommunication>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            // Act
            await _service.Search(0, 10, sort, "");

            // Assert
            _communicationRepositoryMock.Verify(r => r.Search(0, 10, sort, ""), Times.Once);
        }

        [Fact]
        public async Task Search_WithNullSearchTerm_ShouldPassNullToRepository()
        {
            // Arrange
            var sort = new SortOption<SortCommunication>
            {
                Direction = SortDirection.Ascending,
                SortBy = SortCommunication.Created
            };
            var pagedResult = new PagedResult<Communication>
            {
                TotalCount = 0,
                Data = new List<Communication>()
            };

            _communicationRepositoryMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortCommunication>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            // Act
            await _service.Search(0, 10, sort, null);

            // Assert
            _communicationRepositoryMock.Verify(r => r.Search(0, 10, sort, null), Times.Once);
        }

        #endregion

        #region Search Tests - Return Values

        [Fact]
        public async Task Search_WhenRepositoryReturnsEmptyResult_ShouldReturnEmptyPagedResult()
        {
            // Arrange
            var sort = new SortOption<SortCommunication>
            {
                Direction = SortDirection.Ascending,
                SortBy = SortCommunication.Created
            };
            var pagedResult = new PagedResult<Communication>
            {
                TotalCount = 0,
                Data = new List<Communication>()
            };

            _communicationRepositoryMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortCommunication>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            // Act
            var result = await _service.Search(0, 10, sort, "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.TotalCount);
            Assert.Empty(result.Data);
        }

        [Fact]
        public async Task Search_WhenRepositoryReturnsMultipleResults_ShouldReturnAllResults()
        {
            // Arrange
            var communications = new List<Communication>
        {
            new Communication(Guid.NewGuid(), "Communication 1", DateTime.UtcNow, DateTime.UtcNow.AddDays(1)),
            new Communication(Guid.NewGuid(), "Communication 2", DateTime.UtcNow, DateTime.UtcNow.AddDays(2)),
            new Communication(Guid.NewGuid(), "Communication 3", DateTime.UtcNow, DateTime.UtcNow.AddDays(3))
        };
            var sort = new SortOption<SortCommunication>
            {
                Direction = SortDirection.Ascending,
                SortBy = SortCommunication.Created
            };
            var pagedResult = new PagedResult<Communication>
            {
                TotalCount = 3,
                Data = communications
            };

            _communicationRepositoryMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortCommunication>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            // Act
            var result = await _service.Search(0, 10, sort, "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.TotalCount);
            Assert.Equal(3, result.Data.Count());
            Assert.Equal(communications, result.Data);
        }

        [Fact]
        public async Task Search_WhenRepositoryReturnsSingleResult_ShouldReturnSingleResult()
        {
            // Arrange
            var communicationId = Guid.NewGuid();
            var communication = new Communication(
                communicationId,
                "Single Communication",
                DateTime.UtcNow,
                DateTime.UtcNow.AddDays(1)
            );
            var sort = new SortOption<SortCommunication>
            {
                Direction = SortDirection.Ascending,
                SortBy = SortCommunication.Created
            };
            var pagedResult = new PagedResult<Communication>
            {
                TotalCount = 1,
                Data = new List<Communication> { communication }
            };

            _communicationRepositoryMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortCommunication>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            // Act
            var result = await _service.Search(0, 10, sort, "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Data);
            Assert.Equal(communication.Id, result.Data.First().Id);
            Assert.Equal(communication.Content, result.Data.First().Content);
        }

        [Fact]
        public async Task Search_WhenRepositoryReturnsCommunicationWithNullEndDate_ShouldReturnCorrectly()
        {
            // Arrange
            var communicationId = Guid.NewGuid();
            var communication = new Communication(
                communicationId,
                "Communication without end date",
                DateTime.UtcNow,
                null  // No end date
            );
            var sort = new SortOption<SortCommunication>
            {
                Direction = SortDirection.Ascending,
                SortBy = SortCommunication.Created
            };
            var pagedResult = new PagedResult<Communication>
            {
                TotalCount = 1,
                Data = new List<Communication> { communication }
            };

            _communicationRepositoryMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortCommunication>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            // Act
            var result = await _service.Search(0, 10, sort, "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Data);
            Assert.Equal(communication.Id, result.Data.First().Id);
            Assert.Equal(communication.Content, result.Data.First().Content);
            Assert.Null(result.Data.First().EndDate);
        }

        [Fact]
        public async Task Search_WhenRepositoryReturnsCommunicationWithNullStartDate_ShouldReturnCorrectly()
        {
            // Arrange
            var communicationId = Guid.NewGuid();
            var communication = new Communication(
                communicationId,
                "Communication without start date",
                null,  // No start date
                DateTime.UtcNow.AddDays(1)
            );
            var sort = new SortOption<SortCommunication>
            {
                Direction = SortDirection.Ascending,
                SortBy = SortCommunication.Created
            };
            var pagedResult = new PagedResult<Communication>
            {
                TotalCount = 1,
                Data = new List<Communication> { communication }
            };

            _communicationRepositoryMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortCommunication>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            // Act
            var result = await _service.Search(0, 10, sort, "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Data);
            Assert.Equal(communication.Id, result.Data.First().Id);
            Assert.Equal(communication.Content, result.Data.First().Content);
            Assert.Null(result.Data.First().StartDate);
        }

        #endregion

        #region Search Tests - Edge Cases

        [Fact]
        public async Task Search_WithLargePageIndex_ShouldCalculateCorrectSkip()
        {
            // Arrange
            var sort = new SortOption<SortCommunication>
            {
                Direction = SortDirection.Ascending,
                SortBy = SortCommunication.Created
            };
            var pagedResult = new PagedResult<Communication>
            {
                TotalCount = 0,
                Data = new List<Communication>()
            };

            _communicationRepositoryMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortCommunication>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            // Act
            await _service.Search(100, 50, sort, "");

            // Assert
            _communicationRepositoryMock.Verify(r => r.Search(5000, 50, sort, ""), Times.Once);
        }

        [Fact]
        public async Task Search_WithPageSize1_ShouldPassCorrectPageSize()
        {
            // Arrange
            var sort = new SortOption<SortCommunication>
            {
                Direction = SortDirection.Ascending,
                SortBy = SortCommunication.Created
            };
            var pagedResult = new PagedResult<Communication>
            {
                TotalCount = 0,
                Data = new List<Communication>()
            };

            _communicationRepositoryMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortCommunication>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            // Act
            await _service.Search(0, 1, sort, "");

            // Assert
            _communicationRepositoryMock.Verify(r => r.Search(0, 1, sort, ""), Times.Once);
        }

        [Fact]
        public async Task Search_WithLargePageSize_ShouldPassCorrectPageSize()
        {
            // Arrange
            var sort = new SortOption<SortCommunication>
            {
                Direction = SortDirection.Ascending,
                SortBy = SortCommunication.Created
            };
            var pagedResult = new PagedResult<Communication>
            {
                TotalCount = 0,
                Data = new List<Communication>()
            };

            _communicationRepositoryMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortCommunication>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            // Act
            await _service.Search(0, 1000, sort, "");

            // Assert
            _communicationRepositoryMock.Verify(r => r.Search(0, 1000, sort, ""), Times.Once);
        }

        [Fact]
        public async Task Search_MultipleCallsWithDifferentParameters_ShouldCallRepositoryMultipleTimes()
        {
            // Arrange
            var sort1 = new SortOption<SortCommunication>
            {
                Direction = SortDirection.Ascending,
                SortBy = SortCommunication.Created
            };
            var sort2 = new SortOption<SortCommunication>
            {
                Direction = SortDirection.Descending,
                SortBy = SortCommunication.Active
            };
            var pagedResult = new PagedResult<Communication>
            {
                TotalCount = 0,
                Data = new List<Communication>()
            };

            _communicationRepositoryMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<SortOption<SortCommunication>>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            // Act
            await _service.Search(0, 10, sort1, "search1");
            await _service.Search(1, 20, sort2, "search2");
            await _service.Search(2, 30, sort1, "");

            // Assert
            _communicationRepositoryMock.Verify(r => r.Search(0, 10, sort1, "search1"), Times.Once);
            _communicationRepositoryMock.Verify(r => r.Search(20, 20, sort2, "search2"), Times.Once);
            _communicationRepositoryMock.Verify(r => r.Search(60, 30, sort1, ""), Times.Once);
        }

        #endregion

        #region Update Tests - Basic Functionality

        [Fact]
        public async Task UpdateAsync_ShouldUpdateCommunication_WhenAllPropertiesChange()
        {
            // Arrange
            var communicationId = Guid.NewGuid();
            var originalComm = new Communication(communicationId, "Original Content", DateTime.Now, DateTime.Now.AddDays(5));
            var updatedComm = new Communication(communicationId, "Updated Content", DateTime.Now.AddDays(1), DateTime.Now.AddDays(6));

            _communicationRepositoryMock
                .Setup(r => r.GetByIdAsync(communicationId))
                .ReturnsAsync(originalComm);
            _communicationRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Communication>()))
                .ReturnsAsync((Communication c) => c);

            // Act
            var result = await _service.UpdateAsync(updatedComm);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updatedComm.Content, result.Content);
            Assert.Equal(updatedComm.StartDate, result.StartDate);
            Assert.Equal(updatedComm.EndDate, result.EndDate);
            _communicationRepositoryMock.Verify(r => r.GetByIdAsync(communicationId), Times.Once);
            _communicationRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Communication>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateContent_WhenOnlyContentChanges()
        {
            // Arrange
            var communicationId = Guid.NewGuid();
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(5);
            var originalComm = new Communication(communicationId, "Original Content", startDate, endDate);
            var updatedComm = new Communication(communicationId, "Updated Content", startDate, endDate);

            _communicationRepositoryMock
                .Setup(r => r.GetByIdAsync(communicationId))
                .ReturnsAsync(originalComm);
            _communicationRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Communication>()))
                .ReturnsAsync((Communication c) => c);

            // Act
            var result = await _service.UpdateAsync(updatedComm);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Content", result.Content);
            _communicationRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Communication>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateDates_WhenOnlyDatesChange()
        {
            // Arrange
            var communicationId = Guid.NewGuid();
            var originalComm = new Communication(communicationId, "Content", DateTime.Now, DateTime.Now.AddDays(5));
            var newStartDate = DateTime.Now.AddDays(1);
            var newEndDate = DateTime.Now.AddDays(6);
            var updatedComm = new Communication(communicationId, "Content", newStartDate, newEndDate);

            _communicationRepositoryMock
                .Setup(r => r.GetByIdAsync(communicationId))
                .ReturnsAsync(originalComm);
            _communicationRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Communication>()))
                .ReturnsAsync((Communication c) => c);

            // Act
            var result = await _service.UpdateAsync(updatedComm);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newStartDate, result.StartDate);
            Assert.Equal(newEndDate, result.EndDate);
            _communicationRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Communication>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowArgumentNullException_WhenCommunicationIsNull()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateAsync(null));
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowArgumentNullException_WhenCommunicationIdIsEmpty()
        {
            // Arrange
            var communication = new Communication(Guid.Empty, "Content", DateTime.Now, DateTime.Now.AddDays(1));

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _service.UpdateAsync(communication));
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowEntityNotFoundException_WhenCommunicationNotFound()
        {
            // Arrange
            var communicationId = Guid.NewGuid();
            var communication = new Communication(communicationId, "Content", DateTime.Now, DateTime.Now.AddDays(1));

            _communicationRepositoryMock
                .Setup(r => r.GetByIdAsync(communicationId))
                .ReturnsAsync((Communication)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.UpdateAsync(communication));
        }

        [Fact]
        public async Task UpdateAsync_ShouldNotUpdate_WhenNoPropertiesChange()
        {
            // Arrange
            var communicationId = Guid.NewGuid();
            var startDate = DateTime.Now;
            var endDate = DateTime.Now.AddDays(5);
            var originalComm = new Communication(communicationId, "Same Content", startDate, endDate);
            var updatedComm = new Communication(communicationId, "Same Content", startDate, endDate);

            _communicationRepositoryMock
                .Setup(r => r.GetByIdAsync(communicationId))
                .ReturnsAsync(originalComm);
            _communicationRepositoryMock
                .Setup(r => r.UpdateAsync(It.IsAny<Communication>()))
                .ReturnsAsync((Communication c) => c);

            // Act
            var result = await _service.UpdateAsync(updatedComm);

            // Assert
            Assert.NotNull(result);
            _communicationRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Communication>()), Times.Once);
        }

        #endregion

        #region Delete Tests - Basic Functionality

        [Fact]
        public async Task DeleteAsync_ShouldDeleteCommunication_WhenExists()
        {
            // Arrange
            var communicationId = Guid.NewGuid();
            var communication = new Communication(communicationId, "Content", DateTime.Now, DateTime.Now.AddDays(1));

            _communicationRepositoryMock
                .Setup(r => r.GetByIdAsync(communicationId))
                .ReturnsAsync(communication);

            // Act
            await _service.DeleteAsync(communicationId);

            // Assert
            _communicationRepositoryMock.Verify(r => r.GetByIdAsync(communicationId), Times.Once);
            _communicationRepositoryMock.Verify(r => r.Delete(communicationId), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrowEntityNotFoundException_WhenCommunicationNotFound()
        {
            // Arrange
            var communicationId = Guid.NewGuid();

            _communicationRepositoryMock
                .Setup(r => r.GetByIdAsync(communicationId))
                .ReturnsAsync((Communication)null);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => _service.DeleteAsync(communicationId));
            _communicationRepositoryMock.Verify(r => r.Delete(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_ShouldThrow_WhenRepositoryThrows()
        {
            // Arrange
            var communicationId = Guid.NewGuid();

            _communicationRepositoryMock
                .Setup(r => r.GetByIdAsync(communicationId))
                .ThrowsAsync(new Exception("Repository failed"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _service.DeleteAsync(communicationId));
        }

        #endregion
    }
}
