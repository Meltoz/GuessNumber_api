using Application.Interfaces.Repository;
using Application.Services;
using Domain;
using Domain.Enums;
using Moq;
using Shared;

namespace UnitTests.Application
{
    public class ReportServiceTests
    {
        #region GetAll
        [Fact]
        public async Task GetAll_ShouldCallSearchRepo()
        {
            // Arrange
            var reportId = Guid.NewGuid();
            var report = new Report(reportId, TypeReport.Bug, ContextReport.Site, "Test bug", "test@example.com");
            var pagedResult = new PagedResult<Report>
            {
                TotalCount = 1,
                Data = new List<Report>() { report }
            };

            var repoMock = new Mock<IReportRepository>();
            repoMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            var service = new ReportService(repoMock.Object);

            // Act
            var reports = await service.GetAll(0, 10, "");

            // Assert
            repoMock.Verify(r => r.Search(0, 10, ""), Times.Once);
        }

        [Fact]
        public async Task GetAll_ShouldReturnPagedResultFromRepository()
        {
            // Arrange
            var reportId = Guid.NewGuid();
            var report = new Report(reportId, TypeReport.Bug, ContextReport.Site, "Test bug", "test@example.com");
            var pagedResult = new PagedResult<Report>
            {
                TotalCount = 1,
                Data = new List<Report>() { report }
            };

            var repoMock = new Mock<IReportRepository>();
            repoMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            var service = new ReportService(repoMock.Object);

            // Act
            var reports = await service.GetAll(0, 10, "");

            // Assert
            Assert.Equal(pagedResult, reports);
        }

        [Fact]
        public async Task GetAll_ShouldThrow_WhenRepositoryThrows()
        {
            // Arrange
            var repoMock = new Mock<IReportRepository>();
            repoMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()
            )).ThrowsAsync(new Exception("Repository failed"));

            var service = new ReportService(repoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => service.GetAll(0, 10, ""));
        }

        [Fact]
        public async Task GetAll_ShouldCalculateSkipCorrectly_ForFirstPage()
        {
            // Arrange
            var pagedResult = new PagedResult<Report>
            {
                TotalCount = 0,
                Data = new List<Report>()
            };

            var repoMock = new Mock<IReportRepository>();
            repoMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            var service = new ReportService(repoMock.Object);

            // Act
            await service.GetAll(0, 10, "");

            // Assert
            repoMock.Verify(r => r.Search(0, 10, ""), Times.Once);
        }

        [Fact]
        public async Task GetAll_ShouldCalculateSkipCorrectly_ForSecondPage()
        {
            // Arrange
            var pagedResult = new PagedResult<Report>
            {
                TotalCount = 0,
                Data = new List<Report>()
            };

            var repoMock = new Mock<IReportRepository>();
            repoMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            var service = new ReportService(repoMock.Object);

            // Act
            await service.GetAll(1, 10, "");

            // Assert
            repoMock.Verify(r => r.Search(10, 10, ""), Times.Once);
        }

        [Fact]
        public async Task GetAll_ShouldPassSearchTermToRepository()
        {
            // Arrange
            var searchTerm = "bug report";
            var pagedResult = new PagedResult<Report>
            {
                TotalCount = 0,
                Data = new List<Report>()
            };

            var repoMock = new Mock<IReportRepository>();
            repoMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            var service = new ReportService(repoMock.Object);

            // Act
            await service.GetAll(0, 10, searchTerm);

            // Assert
            repoMock.Verify(r => r.Search(0, 10, searchTerm), Times.Once);
        }

        [Fact]
        public async Task GetAll_ShouldPassNullSearchTerm_WhenSearchIsNull()
        {
            // Arrange
            var pagedResult = new PagedResult<Report>
            {
                TotalCount = 0,
                Data = new List<Report>()
            };

            var repoMock = new Mock<IReportRepository>();
            repoMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            var service = new ReportService(repoMock.Object);

            // Act
            await service.GetAll(0, 10, null);

            // Assert
            repoMock.Verify(r => r.Search(0, 10, null), Times.Once);
        }

        [Fact]
        public async Task GetAll_ShouldHandleDifferentPageSizes()
        {
            // Arrange
            var pagedResult = new PagedResult<Report>
            {
                TotalCount = 0,
                Data = new List<Report>()
            };

            var repoMock = new Mock<IReportRepository>();
            repoMock.Setup(r => r.Search(
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<string>()
            )).ReturnsAsync(pagedResult);

            var service = new ReportService(repoMock.Object);

            // Act
            await service.GetAll(2, 25, "");

            // Assert
            repoMock.Verify(r => r.Search(50, 25, ""), Times.Once);
        }
        #endregion
    }
}
