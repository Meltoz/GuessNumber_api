using Application.Exceptions;
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

        #region GetById

        [Fact]
        public async Task GetById_ShouldCallGetByIdAsyncRepo()
        {
            // Arrange
            var reportId = Guid.NewGuid();
            var report = new Report(reportId, TypeReport.Bug, ContextReport.Site, "Test bug", "test@example.com");
            var repoMock = new Mock<IReportRepository>();
            repoMock.Setup(r => r.GetByIdAsync(reportId)).ReturnsAsync(report);
            var service = new ReportService(repoMock.Object);

            // Act
            await service.GetById(reportId);

            // Assert
            repoMock.Verify(r => r.GetByIdAsync(reportId), Times.Once);
        }

        [Fact]
        public async Task GetById_ShouldReturnReportFromRepository()
        {
            // Arrange
            var reportId = Guid.NewGuid();
            var report = new Report(reportId, TypeReport.Bug, ContextReport.Site, "Test bug", "test@example.com");
            var repoMock = new Mock<IReportRepository>();
            repoMock.Setup(r => r.GetByIdAsync(reportId)).ReturnsAsync(report);
            var service = new ReportService(repoMock.Object);

            // Act
            var result = await service.GetById(reportId);

            // Assert
            Assert.Equal(report, result);
            Assert.Equal(reportId, result.Id);
            Assert.Equal(TypeReport.Bug, result.Type);
            Assert.Equal(ContextReport.Site, result.Context);
        }

        [Fact]
        public async Task GetById_WhenReportNotFound_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var reportId = Guid.NewGuid();
            var repoMock = new Mock<IReportRepository>();
            repoMock.Setup(r => r.GetByIdAsync(reportId)).ReturnsAsync((Report?)null);
            var service = new ReportService(repoMock.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => service.GetById(reportId));
        }

        [Fact]
        public async Task GetById_WithDifferentReportTypes_ShouldReturnCorrectReport()
        {
            // Arrange
            var reportId = Guid.NewGuid();
            var report = new Report(reportId, TypeReport.Improuvment, ContextReport.Question, "Test improvement", null);
            var repoMock = new Mock<IReportRepository>();
            repoMock.Setup(r => r.GetByIdAsync(reportId)).ReturnsAsync(report);
            var service = new ReportService(repoMock.Object);

            // Act
            var result = await service.GetById(reportId);

            // Assert
            Assert.Equal(TypeReport.Improuvment, result.Type);
            Assert.Equal(ContextReport.Question, result.Context);
            Assert.Null(result.Mail);
        }
        #endregion

        #region Delete

        [Fact]
        public async Task Delete_ShouldCallGetByIdAsyncRepo()
        {
            // Arrange
            var reportId = Guid.NewGuid();
            var report = new Report(reportId, TypeReport.Bug, ContextReport.Site, "Test bug", "test@example.com");
            var repoMock = new Mock<IReportRepository>();
            repoMock.Setup(r => r.GetByIdAsync(reportId)).ReturnsAsync(report);
            var service = new ReportService(repoMock.Object);

            // Act
            await service.Delete(reportId);

            // Assert
            repoMock.Verify(r => r.GetByIdAsync(reportId), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldCallDeleteRepo()
        {
            // Arrange
            var reportId = Guid.NewGuid();
            var report = new Report(reportId, TypeReport.Bug, ContextReport.Site, "Test bug", "test@example.com");
            var repoMock = new Mock<IReportRepository>();
            repoMock.Setup(r => r.GetByIdAsync(reportId)).ReturnsAsync(report);
            var service = new ReportService(repoMock.Object);

            // Act
            await service.Delete(reportId);

            // Assert
            repoMock.Verify(r => r.Delete(reportId), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenReportNotFound_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var reportId = Guid.NewGuid();
            var repoMock = new Mock<IReportRepository>();
            repoMock.Setup(r => r.GetByIdAsync(reportId)).ReturnsAsync((Report?)null);
            var service = new ReportService(repoMock.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => service.Delete(reportId));
        }

        [Fact]
        public async Task Delete_WhenReportNotFound_ShouldNotCallDeleteRepo()
        {
            // Arrange
            var reportId = Guid.NewGuid();
            var repoMock = new Mock<IReportRepository>();
            repoMock.Setup(r => r.GetByIdAsync(reportId)).ReturnsAsync((Report?)null);
            var service = new ReportService(repoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<EntityNotFoundException>(() => service.Delete(reportId));
            repoMock.Verify(r => r.Delete(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task Delete_WithValidId_ShouldCallDeleteWithCorrectId()
        {
            // Arrange
            var reportId = Guid.NewGuid();
            var report = new Report(reportId, TypeReport.Improuvment, ContextReport.Question, "Test improvement", "delete@example.com");
            var repoMock = new Mock<IReportRepository>();
            repoMock.Setup(r => r.GetByIdAsync(reportId)).ReturnsAsync(report);
            var service = new ReportService(repoMock.Object);

            // Act
            await service.Delete(reportId);

            // Assert
            repoMock.Verify(r => r.Delete(reportId), Times.Once);
            repoMock.Verify(r => r.Delete(It.Is<Guid>(id => id == reportId)), Times.Once);
        }

        #endregion

        #region CreateReport

        [Fact]
        public async Task CreateReport_WithValidData_ShouldCallInsertAsync()
        {
            // Arrange
            var type = "Bug";
            var context = "Site";
            var explanation = "Test bug explanation";
            var mail = "test@example.com";

            var repoMock = new Mock<IReportRepository>();
            repoMock.Setup(r => r.InsertAsync(It.IsAny<Report>()))
                .ReturnsAsync((Report r) => r);

            var service = new ReportService(repoMock.Object);

            // Act
            await service.CreateReport(type, context, explanation, mail);

            // Assert
            repoMock.Verify(r => r.InsertAsync(It.IsAny<Report>()), Times.Once);
        }

        [Fact]
        public async Task CreateReport_WithValidData_ShouldReturnReport()
        {
            // Arrange
            var type = "Bug";
            var context = "Site";
            var explanation = "Test bug explanation";
            var mail = "test@example.com";

            var repoMock = new Mock<IReportRepository>();
            repoMock.Setup(r => r.InsertAsync(It.IsAny<Report>()))
                .ReturnsAsync((Report r) => r);

            var service = new ReportService(repoMock.Object);

            // Act
            var result = await service.CreateReport(type, context, explanation, mail);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(TypeReport.Bug, result.Type);
            Assert.Equal(ContextReport.Site, result.Context);
            Assert.Equal(explanation, result.Explanation);
            Assert.Equal(mail, result.Mail);
        }

        [Fact]
        public async Task CreateReport_WithNullMail_ShouldReturnReportWithNullMail()
        {
            // Arrange
            var type = "Improuvment";
            var context = "Question";
            var explanation = "Suggestion for improvement";
            string? mail = null;

            var repoMock = new Mock<IReportRepository>();
            repoMock.Setup(r => r.InsertAsync(It.IsAny<Report>()))
                .ReturnsAsync((Report r) => r);

            var service = new ReportService(repoMock.Object);

            // Act
            var result = await service.CreateReport(type, context, explanation, mail);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(TypeReport.Improuvment, result.Type);
            Assert.Equal(ContextReport.Question, result.Context);
            Assert.Equal(explanation, result.Explanation);
            Assert.Null(result.Mail);
        }

        [Fact]
        public async Task CreateReport_WithInvalidType_ShouldThrowArgumentException()
        {
            // Arrange
            var type = "InvalidType";
            var context = "Site";
            var explanation = "Test explanation";
            var mail = "test@example.com";

            var repoMock = new Mock<IReportRepository>();
            var service = new ReportService(repoMock.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => service.CreateReport(type, context, explanation, mail)
            );
            Assert.Contains("Error when converting type", exception.Message);
        }

        [Fact]
        public async Task CreateReport_WithInvalidContext_ShouldThrowArgumentException()
        {
            // Arrange
            var type = "Bug";
            var context = "InvalidContext";
            var explanation = "Test explanation";
            var mail = "test@example.com";

            var repoMock = new Mock<IReportRepository>();
            var service = new ReportService(repoMock.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => service.CreateReport(type, context, explanation, mail)
            );
            Assert.Contains("Error when converting context", exception.Message);
        }

        [Fact]
        public async Task CreateReport_WithEmptyType_ShouldThrowArgumentException()
        {
            // Arrange
            var type = "";
            var context = "Site";
            var explanation = "Test explanation";
            var mail = "test@example.com";

            var repoMock = new Mock<IReportRepository>();
            var service = new ReportService(repoMock.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => service.CreateReport(type, context, explanation, mail)
            );
        }

        [Fact]
        public async Task CreateReport_WithEmptyContext_ShouldThrowArgumentException()
        {
            // Arrange
            var type = "Bug";
            var context = "";
            var explanation = "Test explanation";
            var mail = "test@example.com";

            var repoMock = new Mock<IReportRepository>();
            var service = new ReportService(repoMock.Object);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(
                () => service.CreateReport(type, context, explanation, mail)
            );
        }

        [Fact]
        public async Task CreateReport_WithCaseInsensitiveType_ShouldReturnReport()
        {
            // Arrange
            var type = "bug"; // lowercase
            var context = "Site";
            var explanation = "Test bug explanation";
            var mail = "test@example.com";

            var repoMock = new Mock<IReportRepository>();
            repoMock.Setup(r => r.InsertAsync(It.IsAny<Report>()))
                .ReturnsAsync((Report r) => r);

            var service = new ReportService(repoMock.Object);

            // Act
            var result = await service.CreateReport(type, context, explanation, mail);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(TypeReport.Bug, result.Type);
        }

        [Fact]
        public async Task CreateReport_WithCaseInsensitiveContext_ShouldReturnReport()
        {
            // Arrange
            var type = "Bug";
            var context = "site"; // lowercase
            var explanation = "Test bug explanation";
            var mail = "test@example.com";

            var repoMock = new Mock<IReportRepository>();
            repoMock.Setup(r => r.InsertAsync(It.IsAny<Report>()))
                .ReturnsAsync((Report r) => r);

            var service = new ReportService(repoMock.Object);

            // Act
            var result = await service.CreateReport(type, context, explanation, mail);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ContextReport.Site, result.Context);
        }

        [Theory]
        [InlineData("Bug", "Site")]
        [InlineData("Bug", "Question")]
        [InlineData("Improuvment", "Site")]
        [InlineData("Improuvment", "Question")]
        public async Task CreateReport_WithDifferentTypesAndContexts_ShouldReturnReport(string type, string context)
        {
            // Arrange
            var explanation = "Test explanation for different combinations";
            var mail = "test@example.com";

            var repoMock = new Mock<IReportRepository>();
            repoMock.Setup(r => r.InsertAsync(It.IsAny<Report>()))
                .ReturnsAsync((Report r) => r);

            var service = new ReportService(repoMock.Object);

            // Act
            var result = await service.CreateReport(type, context, explanation, mail);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(explanation, result.Explanation);
            Assert.Equal(mail, result.Mail);
        }

        [Fact]
        public async Task CreateReport_ShouldCreateReportWithCorrectProperties()
        {
            // Arrange
            var type = "Bug";
            var context = "Question";
            var explanation = "Detailed bug explanation";
            var mail = "user@test.com";

            Report? capturedReport = null;
            var repoMock = new Mock<IReportRepository>();
            repoMock.Setup(r => r.InsertAsync(It.IsAny<Report>()))
                .Callback<Report>(r => capturedReport = r)
                .ReturnsAsync((Report r) => r);

            var service = new ReportService(repoMock.Object);

            // Act
            await service.CreateReport(type, context, explanation, mail);

            // Assert
            Assert.NotNull(capturedReport);
            Assert.Equal(TypeReport.Bug, capturedReport.Type);
            Assert.Equal(ContextReport.Question, capturedReport.Context);
            Assert.Equal(explanation, capturedReport.Explanation);
            Assert.Equal(mail, capturedReport.Mail);
        }

        [Fact]
        public async Task CreateReport_WithInvalidType_ShouldNotCallInsertAsync()
        {
            // Arrange
            var type = "InvalidType";
            var context = "Site";
            var explanation = "Test explanation";
            var mail = "test@example.com";

            var repoMock = new Mock<IReportRepository>();
            var service = new ReportService(repoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => service.CreateReport(type, context, explanation, mail)
            );
            repoMock.Verify(r => r.InsertAsync(It.IsAny<Report>()), Times.Never);
        }

        [Fact]
        public async Task CreateReport_WithInvalidContext_ShouldNotCallInsertAsync()
        {
            // Arrange
            var type = "Bug";
            var context = "InvalidContext";
            var explanation = "Test explanation";
            var mail = "test@example.com";

            var repoMock = new Mock<IReportRepository>();
            var service = new ReportService(repoMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => service.CreateReport(type, context, explanation, mail)
            );
            repoMock.Verify(r => r.InsertAsync(It.IsAny<Report>()), Times.Never);
        }

        [Fact]
        public async Task CreateReport_CalledMultipleTimes_ShouldCallRepositoryEachTime()
        {
            // Arrange
            var repoMock = new Mock<IReportRepository>();
            repoMock.Setup(r => r.InsertAsync(It.IsAny<Report>()))
                .ReturnsAsync((Report r) => r);

            var service = new ReportService(repoMock.Object);

            // Act
            await service.CreateReport("Bug", "Site", "First report", "test1@example.com");
            await service.CreateReport("Improuvment", "Question", "Second report", "test2@example.com");

            // Assert
            repoMock.Verify(r => r.InsertAsync(It.IsAny<Report>()), Times.Exactly(2));
        }

        #endregion
    }
}
