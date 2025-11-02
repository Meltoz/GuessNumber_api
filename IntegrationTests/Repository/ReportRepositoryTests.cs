using Domain.Enums;
using Infrastructure.Entities;
using Infrastructure.Repositories;
using Meltix.IntegrationTests;

namespace IntegrationTests.Repository
{
    public class ReportRepositoryTests
    {
        [Fact]
        public async Task Search_WithoutSearchTerm_ReturnsAllReports()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ReportRepository(context, mapper);

            var report1 = new ReportEntity
            {
                Type = TypeReport.Bug,
                Context = ContextReport.Site,
                Explanation = "First bug report",
                Mail = "user1@example.com",
                Created = DateTime.UtcNow.AddDays(-2)
            };

            var report2 = new ReportEntity
            {
                Type = TypeReport.Improuvment,
                Context = ContextReport.Question,
                Explanation = "Feature request",
                Mail = "user2@example.com",
                Created = DateTime.UtcNow.AddDays(-1)
            };

            context.Reports.AddRange(report1, report2);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.Search(0, 10, "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Data.Count());
        }

        [Fact]
        public async Task Search_WithSearchTermInExplanation_ReturnsMatchingReports()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ReportRepository(context, mapper);

            var report1 = new ReportEntity
            {
                Type = TypeReport.Bug,
                Context = ContextReport.Site,
                Explanation = "Login button not working",
                Mail = "user1@example.com",
                Created = DateTime.UtcNow.AddDays(-2)
            };

            var report2 = new ReportEntity
            {
                Type = TypeReport.Improuvment,
                Context = ContextReport.Question,
                Explanation = "Add dark mode feature",
                Mail = "user2@example.com",
                Created = DateTime.UtcNow.AddDays(-1)
            };

            context.Reports.AddRange(report1, report2);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.Search(0, 10, "login button not working");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Data);
            Assert.Contains("Login button", result.Data.First().Explanation);
        }

        [Fact]
        public async Task Search_WithSearchTermInMail_ReturnsMatchingReports()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ReportRepository(context, mapper);

            var report1 = new ReportEntity
            {
                Type = TypeReport.Bug,
                Context = ContextReport.Site,
                Explanation = "Bug report 1",
                Mail = "john.doe@example.com",
                Created = DateTime.UtcNow.AddDays(-2)
            };

            var report2 = new ReportEntity
            {
                Type = TypeReport.Improuvment,
                Context = ContextReport.Question,
                Explanation = "Improvement request",
                Mail = "jane.smith@example.com",
                Created = DateTime.UtcNow.AddDays(-1)
            };

            context.Reports.AddRange(report1, report2);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.Search(0, 10, "john.doe@example.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);
            Assert.Single(result.Data);
            Assert.Equal("john.doe@example.com", result.Data.First().Mail);
        }

        [Fact]
        public async Task Search_WithNullSearchTerm_ReturnsAllReports()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ReportRepository(context, mapper);

            var report1 = new ReportEntity
            {
                Type = TypeReport.Bug,
                Context = ContextReport.Site,
                Explanation = "Bug description",
                Mail = "user@example.com",
                Created = DateTime.UtcNow.AddDays(-1)
            };

            context.Reports.Add(report1);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.Search(0, 10, null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);
        }

        [Fact]
        public async Task Search_WithNonMatchingSearchTerm_ReturnsEmptyResult()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ReportRepository(context, mapper);

            var report1 = new ReportEntity
            {
                Type = TypeReport.Bug,
                Context = ContextReport.Site,
                Explanation = "Bug report",
                Mail = "user@example.com",
                Created = DateTime.UtcNow
            };

            context.Reports.Add(report1);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.Search(0, 10, "nonexistent");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.TotalCount);
            Assert.Empty(result.Data);
        }

        [Fact]
        public async Task Search_WithPagination_ReturnsCorrectPage()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ReportRepository(context, mapper);

            for (int i = 0; i < 15; i++)
            {
                var report = new ReportEntity
                {
                    Type = TypeReport.Bug,
                    Context = ContextReport.Site,
                    Explanation = $"Bug report {i}",
                    Mail = $"user{i}@example.com",
                    Created = DateTime.UtcNow.AddDays(-i)
                };
                context.Reports.Add(report);
            }
            await context.SaveChangesAsync();

            // Act
            var result = await repository.Search(5, 5, "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(15, result.TotalCount);
            Assert.Equal(5, result.Data.Count());
        }

        [Fact]
        public async Task Search_CaseInsensitiveSearch_ReturnsMatchingReports()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ReportRepository(context, mapper);

            var report1 = new ReportEntity
            {
                Type = TypeReport.Bug,
                Context = ContextReport.Site,
                Explanation = "URGENT Bug Report",
                Mail = "Test@Example.COM",
                Created = DateTime.UtcNow
            };

            context.Reports.Add(report1);
            await context.SaveChangesAsync();

            // Act
            var resultExplanation = await repository.Search(0, 10, "urgent bug report");
            var resultMail = await repository.Search(0, 10, "test@example.com");

            // Assert
            Assert.Equal(1, resultExplanation.TotalCount);
            Assert.Equal(1, resultMail.TotalCount);
        }
    }
}
