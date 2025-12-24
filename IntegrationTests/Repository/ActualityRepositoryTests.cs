
using Domain;
using Infrastructure.Entities;
using Infrastructure.Repositories;
using Meltix.IntegrationTests;
using Shared;
using Shared.Enums.Sorting;

namespace IntegrationTests.Repository
{
    public class ActualityRepositoryTests
    {
        [Fact]
        public async Task Search_WithoutSearchTerm_ReturnsAllActualities()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper(); // À adapter selon votre configuration
            var actualityRepository = new ActualityRepository(context, mapper);

            var actuality1 = new ActualityEntity { Title = "First News", Content = "Content 1", StartPublish = DateTime.Now };
            var actuality2 = new ActualityEntity { Title = "Second News", Content = "Content 2", StartPublish = DateTime.Now };
            context.Actualities.AddRange(actuality1, actuality2);
            await context.SaveChangesAsync();

            // Act
            var result = await actualityRepository.Search(0, 10, new SortOption<SortActuality>() { Direction = SortDirection.Descending, SortBy = SortActuality.Created }, "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
        }

        [Fact]
        public async Task Search_WithSearchTermInTitle_ReturnsMatchingActualities()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var actualityRepository = new ActualityRepository(context, mapper);

            var actuality1 = new ActualityEntity { Title = "Breaking News", Content = "Important update", StartPublish = DateTime.Now };
            var actuality2 = new ActualityEntity { Title = "Weather Report", Content = "Sunny day" , StartPublish = DateTime.Now };
            context.Actualities.AddRange(actuality1, actuality2);
            await context.SaveChangesAsync();

            // Act
            var result = await actualityRepository.Search(0, 10, new SortOption<SortActuality>() { Direction= SortDirection.Descending, SortBy=SortActuality.Created}, "Breaking");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);
            Assert.Equal("Breaking News", result.Data.First().Title);
        }

        [Fact]
        public async Task Search_WithSearchTermInContent_ReturnsMatchingActualities()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var actualityRepository = new ActualityRepository(context, mapper);

            var actuality1 = new ActualityEntity { Title = "News 1", Content = "Important announcement", StartPublish = DateTime.Now };
            var actuality2 = new ActualityEntity { Title = "News 2", Content = "Regular update", StartPublish = DateTime.Now };
            context.Actualities.AddRange(actuality1, actuality2);
            await context.SaveChangesAsync();

            // Act
            var result = await actualityRepository.Search(0, 10, new SortOption<SortActuality>() { Direction = SortDirection.Descending, SortBy = SortActuality.Created }, "announcement");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);
            Assert.Equal("News 1", result.Data.First().Title);
        }

        [Fact]
        public async Task Search_WithSearchTermInTitleOrContent_ReturnsAllMatches()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var actualityRepository = new ActualityRepository(context, mapper);

            var actuality1 = new ActualityEntity { Title = "Sports Update", Content = "Football results", StartPublish = DateTime.Now };
            var actuality2 = new ActualityEntity { Title = "News Report", Content = "Sports highlights", StartPublish = DateTime.Now };
            var actuality3 = new ActualityEntity { Title = "Weather", Content = "Temperature", StartPublish = DateTime.Now };
            context.Actualities.AddRange(actuality1, actuality2, actuality3);
            await context.SaveChangesAsync();

            // Act
            var result = await actualityRepository.Search(0, 
                10, 
                new SortOption<SortActuality>() { Direction = SortDirection.Descending, SortBy = SortActuality.Created }, 
                "Sports");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
        }

        [Fact]
        public async Task Search_WithPagination_ReturnsCorrectPage()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var actualityRepository = new ActualityRepository(context, mapper);

            for (int i = 1; i <= 5; i++)
            {
                var actuality = new ActualityEntity { Title = $"News {i}", Content = $"Content {i}", StartPublish = DateTime.Now };
                context.Actualities.Add(actuality);
            }
            await context.SaveChangesAsync();

            // Act
            var result = await actualityRepository.Search(2, 
                2, 
                new SortOption<SortActuality>() { Direction = SortDirection.Descending, SortBy = SortActuality.Created }, 
                "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.TotalCount);
            Assert.Equal(2, result.Data.Count());
        }

        [Fact]
        public async Task Search_WithNoMatches_ReturnsEmptyResult()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var actualityRepository = new ActualityRepository(context, mapper);

            var actuality = new ActualityEntity { Title = "News", Content = "Content", StartPublish = DateTime.Now };
            context.Actualities.Add(actuality);
            await context.SaveChangesAsync();

            // Act
            var result = await actualityRepository.Search(0, 
                10, 
                new SortOption<SortActuality>() { Direction = SortDirection.Descending, SortBy = SortActuality.Created }, 
                "NonExistent");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.TotalCount);
            Assert.Empty(result.Data);
        }

        [Fact]
        public async Task Search_WithSortOptions_ReturnsOrderedResults()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var actualityRepository = new ActualityRepository(context, mapper);

            var actuality1 = new ActualityEntity { Title = "Z News", Content = "Content", StartPublish = DateTime.Now };
            var actuality2 = new ActualityEntity { Title = "A News", Content = "Content", StartPublish = DateTime.Now };
            context.Actualities.AddRange(actuality1, actuality2);
            await context.SaveChangesAsync();

            var sortOptions = new SortOption<SortActuality>
            {
                SortBy = SortActuality.Created,
                Direction = SortDirection.Ascending
            };

            // Act
            var result = await actualityRepository.Search(0, 10, sortOptions, "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
        }

        [Fact]
        public async Task Search_WithWhitespaceSearch_ReturnsAllActualities()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var actualityRepository = new ActualityRepository(context, mapper);

            var actuality = new ActualityEntity { Title = "News", Content = "Content", StartPublish = DateTime.Now };
            context.Actualities.Add(actuality);
            await context.SaveChangesAsync();

            var sortOptions = new SortOption<SortActuality>
            {
                SortBy = SortActuality.Created,
                Direction = SortDirection.Ascending
            };

            // Act
            var result = await actualityRepository.Search(0, 10, sortOptions, "   ");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);
        }

        [Fact]
        public async Task GetActives_WithActiveActualities_ReturnsOnlyActiveOnes()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var actualityRepository = new ActualityRepository(context, mapper);

            var now = DateTime.UtcNow;
            var activeActuality1 = new ActualityEntity
            {
                Title = "Active News 1",
                Content = "Content 1",
                StartPublish = now.AddDays(-5),
                EndPublish = now.AddDays(5)
            };
            var activeActuality2 = new ActualityEntity
            {
                Title = "Active News 2",
                Content = "Content 2",
                StartPublish = now.AddDays(-2),
                EndPublish = null // No end date
            };
            var futureActuality = new ActualityEntity
            {
                Title = "Future News",
                Content = "Content 3",
                StartPublish = now.AddDays(2),
                EndPublish = now.AddDays(10)
            };
            var expiredActuality = new ActualityEntity
            {
                Title = "Expired News",
                Content = "Content 4",
                StartPublish = now.AddDays(-10),
                EndPublish = now.AddDays(-1)
            };

            context.Actualities.AddRange(activeActuality1, activeActuality2, futureActuality, expiredActuality);
            await context.SaveChangesAsync();

            // Act
            var result = await actualityRepository.GetActives();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, a => a.Title == "Active News 1");
            Assert.Contains(result, a => a.Title == "Active News 2");
            Assert.DoesNotContain(result, a => a.Title == "Future News");
            Assert.DoesNotContain(result, a => a.Title == "Expired News");
        }

        [Fact]
        public async Task GetActives_WithNoActiveActualities_ReturnsEmptyCollection()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var actualityRepository = new ActualityRepository(context, mapper);

            var now = DateTime.UtcNow;
            var futureActuality = new ActualityEntity
            {
                Title = "Future News",
                Content = "Content",
                StartPublish = now.AddDays(5),
                EndPublish = now.AddDays(10)
            };
            var expiredActuality = new ActualityEntity
            {
                Title = "Expired News",
                Content = "Content",
                StartPublish = now.AddDays(-10),
                EndPublish = now.AddDays(-1)
            };

            context.Actualities.AddRange(futureActuality, expiredActuality);
            await context.SaveChangesAsync();

            // Act
            var result = await actualityRepository.GetActives();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetActives_WithOnlyActiveActualities_ReturnsAll()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var actualityRepository = new ActualityRepository(context, mapper);

            var now = DateTime.UtcNow;
            var actuality1 = new ActualityEntity
            {
                Title = "Active 1",
                Content = "Content 1",
                StartPublish = now.AddDays(-5),
                EndPublish = now.AddDays(5)
            };
            var actuality2 = new ActualityEntity
            {
                Title = "Active 2",
                Content = "Content 2",
                StartPublish = now.AddDays(-3),
                EndPublish = null
            };
            var actuality3 = new ActualityEntity
            {
                Title = "Active 3",
                Content = "Content 3",
                StartPublish = now.AddDays(-1),
                EndPublish = now.AddDays(10)
            };

            context.Actualities.AddRange(actuality1, actuality2, actuality3);
            await context.SaveChangesAsync();

            // Act
            var result = await actualityRepository.GetActives();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetActives_WithActualityStartingToday_ReturnsIt()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var actualityRepository = new ActualityRepository(context, mapper);

            var now = DateTime.UtcNow;
            var actuality = new ActualityEntity
            {
                Title = "Starting Today",
                Content = "Content",
                StartPublish = now.AddMinutes(-1), // Started 1 minute ago
                EndPublish = now.AddDays(5)
            };

            context.Actualities.Add(actuality);
            await context.SaveChangesAsync();

            // Act
            var result = await actualityRepository.GetActives();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Starting Today", result.First().Title);
        }

        [Fact]
        public async Task GetActives_WithActualityEndingToday_DoesNotReturnIt()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var actualityRepository = new ActualityRepository(context, mapper);

            var now = DateTime.UtcNow;
            var actuality = new ActualityEntity
            {
                Title = "Ending Today",
                Content = "Content",
                StartPublish = now.AddDays(-5),
                EndPublish = now.AddMinutes(-1) // Ended 1 minute ago
            };

            context.Actualities.Add(actuality);
            await context.SaveChangesAsync();

            // Act
            var result = await actualityRepository.GetActives();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetActives_WithNoActualities_ReturnsEmptyCollection()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var actualityRepository = new ActualityRepository(context, mapper);

            // Act
            var result = await actualityRepository.GetActives();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetActives_WithActualityWithoutEndDate_ReturnsIt()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var actualityRepository = new ActualityRepository(context, mapper);

            var now = DateTime.UtcNow;
            var actuality = new ActualityEntity
            {
                Title = "No End Date",
                Content = "Content",
                StartPublish = now.AddDays(-5),
                EndPublish = null
            };

            context.Actualities.Add(actuality);
            await context.SaveChangesAsync();

            // Act
            var result = await actualityRepository.GetActives();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("No End Date", result.First().Title);
            Assert.Null(result.First().EndPublish);
        }
    }
}
