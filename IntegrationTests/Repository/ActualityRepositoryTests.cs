
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
    }
}
