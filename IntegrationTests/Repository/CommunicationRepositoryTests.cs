using Infrastructure.Entities;
using Infrastructure.Repositories;
using Meltix.IntegrationTests;
using Shared;
using Shared.Enums.Sorting;

namespace IntegrationTests.Repository
{
    public class CommunicationRepositoryTests
    {
        #region Search Tests

        [Fact]
        public async Task Search_WithoutSearchTerm_ReturnsAllCommunications()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new CommunicationRepository(context, mapper);

            var comm1 = new CommunicationEntity
            {
                Content = "First Communication",
                Start = DateTime.UtcNow.AddDays(-1),
                Created = DateTime.UtcNow.AddDays(-2)
            };
            var comm2 = new CommunicationEntity
            {
                Content = "Second Communication",
                Start = DateTime.UtcNow,
                Created = DateTime.UtcNow.AddDays(-1)
            };

            context.Communications.AddRange(comm1, comm2);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.Search(
                0,
                10,
                new SortOption<SortCommunication> { Direction = SortDirection.Descending, SortBy = SortCommunication.Created },
                "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
        }

        [Fact]
        public async Task Search_WithSearchTermInContent_ReturnsMatchingCommunications()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new CommunicationRepository(context, mapper);

            var comm1 = new CommunicationEntity
            {
                Content = "Important announcement",
                Start = DateTime.UtcNow,
                Created = DateTime.UtcNow
            };
            var comm2 = new CommunicationEntity
            {
                Content = "Regular update",
                Start = DateTime.UtcNow,
                Created = DateTime.UtcNow
            };
            var comm3 = new CommunicationEntity
            {
                Content = "Another important message",
                Start = DateTime.UtcNow,
                Created = DateTime.UtcNow
            };

            context.Communications.AddRange(comm1, comm2, comm3);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.Search(
                0,
                10,
                new SortOption<SortCommunication> { Direction = SortDirection.Ascending, SortBy = SortCommunication.Created },
                "important");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            Assert.All(result.Data, item => Assert.Contains("important", item.Content, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task Search_WithSearchTermNotFound_ReturnsEmpty()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new CommunicationRepository(context, mapper);

            var comm = new CommunicationEntity
            {
                Content = "Sample content",
                Start = DateTime.UtcNow,
                Created = DateTime.UtcNow
            };

            context.Communications.Add(comm);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.Search(
                0,
                10,
                new SortOption<SortCommunication> { Direction = SortDirection.Ascending, SortBy = SortCommunication.Created },
                "nonexistent");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.TotalCount);
            Assert.Empty(result.Data);
        }

        [Fact]
        public async Task Search_WithNullSearchTerm_ReturnsAllCommunications()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new CommunicationRepository(context, mapper);

            var comm = new CommunicationEntity
            {
                Content = "Content",
                Start = DateTime.UtcNow,
                Created = DateTime.UtcNow
            };

            context.Communications.Add(comm);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.Search(
                0,
                10,
                new SortOption<SortCommunication> { Direction = SortDirection.Ascending, SortBy = SortCommunication.Created },
                null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);
        }

        [Fact]
        public async Task Search_WithWhitespaceSearchTerm_ReturnsAllCommunications()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new CommunicationRepository(context, mapper);

            var comm = new CommunicationEntity
            {
                Content = "Content",
                Start = DateTime.UtcNow,
                Created = DateTime.UtcNow
            };

            context.Communications.Add(comm);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.Search(
                0,
                10,
                new SortOption<SortCommunication> { Direction = SortDirection.Ascending, SortBy = SortCommunication.Created },
                "   ");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);
        }

        #endregion

        #region Pagination Tests

        [Fact]
        public async Task Search_WithPagination_ReturnsCorrectPage()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new CommunicationRepository(context, mapper);

            for (int i = 1; i <= 15; i++)
            {
                context.Communications.Add(new CommunicationEntity
                {
                    Content = $"Communication {i}",
                    Start = DateTime.UtcNow,
                    Created = DateTime.UtcNow.AddMinutes(-i)
                });
            }
            await context.SaveChangesAsync();

            // Act
            var result = await repository.Search(
                5,
                5,
                new SortOption<SortCommunication> { Direction = SortDirection.Descending, SortBy = SortCommunication.Created },
                "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(15, result.TotalCount);
            Assert.Equal(5, result.Data.Count());
        }

        [Fact]
        public async Task Search_WithSkipGreaterThanTotal_ReturnsEmpty()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new CommunicationRepository(context, mapper);

            context.Communications.Add(new CommunicationEntity
            {
                Content = "Communication",
                Start = DateTime.UtcNow,
                Created = DateTime.UtcNow
            });
            await context.SaveChangesAsync();

            // Act
            var result = await repository.Search(
                10,
                10,
                new SortOption<SortCommunication> { Direction = SortDirection.Ascending, SortBy = SortCommunication.Created },
                "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);
            Assert.Empty(result.Data);
        }

        [Fact]
        public async Task Search_WithZeroTake_ReturnsEmptyItems()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new CommunicationRepository(context, mapper);

            context.Communications.Add(new CommunicationEntity
            {
                Content = "Communication",
                Start = DateTime.UtcNow,
                Created = DateTime.UtcNow
            });
            await context.SaveChangesAsync();

            // Act
            var result = await repository.Search(
                0,
                0,
                new SortOption<SortCommunication> { Direction = SortDirection.Ascending, SortBy = SortCommunication.Created },
                "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);
            Assert.Empty(result.Data);
        }

        #endregion

        #region Sort by Created Tests

        [Fact]
        public async Task Search_SortByCreatedAscending_ReturnsOrderedResults()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new CommunicationRepository(context, mapper);

            var comm1 = new CommunicationEntity
            {
                Content = "Third",
                Start = DateTime.UtcNow,
                Created = DateTime.UtcNow.AddDays(-1)
            };
            var comm2 = new CommunicationEntity
            {
                Content = "First",
                Start = DateTime.UtcNow,
                Created = DateTime.UtcNow.AddDays(-3)
            };
            var comm3 = new CommunicationEntity
            {
                Content = "Second",
                Start = DateTime.UtcNow,
                Created = DateTime.UtcNow.AddDays(-2)
            };

            context.Communications.AddRange(comm1, comm2, comm3);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.Search(
                0,
                10,
                new SortOption<SortCommunication> { Direction = SortDirection.Ascending, SortBy = SortCommunication.Created },
                "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.TotalCount);
            var items = result.Data.ToList();
            Assert.Equal("First", items[0].Content);
            Assert.Equal("Second", items[1].Content);
            Assert.Equal("Third", items[2].Content);
        }

        [Fact]
        public async Task Search_SortByCreatedDescending_ReturnsOrderedResults()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new CommunicationRepository(context, mapper);

            var comm1 = new CommunicationEntity
            {
                Content = "Second",
                Start = DateTime.UtcNow,
                Created = DateTime.UtcNow.AddDays(-2)
            };
            var comm2 = new CommunicationEntity
            {
                Content = "First",
                Start = DateTime.UtcNow,
                Created = DateTime.UtcNow.AddDays(-1)
            };
            var comm3 = new CommunicationEntity
            {
                Content = "Third",
                Start = DateTime.UtcNow,
                Created = DateTime.UtcNow.AddDays(-3)
            };

            context.Communications.AddRange(comm1, comm2, comm3);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.Search(
                0,
                10,
                new SortOption<SortCommunication> { Direction = SortDirection.Descending, SortBy = SortCommunication.Created },
                "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.TotalCount);
            var items = result.Data.ToList();
            Assert.Equal("First", items[0].Content);
            Assert.Equal("Second", items[1].Content);
            Assert.Equal("Third", items[2].Content);
        }

        #endregion

        #region Sort by Active Tests

        [Fact]
        public async Task Search_SortByActiveAscending_InactiveCommunicationsFirst()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new CommunicationRepository(context, mapper);

            var now = DateTime.UtcNow;

            // Communication active (Start <= now, End >= now)
            var activeComm = new CommunicationEntity
            {
                Content = "Active",
                Start = now.AddDays(-1),
                End = now.AddDays(1),
                Created = now
            };

            // Communication inactive (future)
            var futureComm = new CommunicationEntity
            {
                Content = "Future",
                Start = now.AddDays(1),
                End = now.AddDays(2),
                Created = now
            };

            // Communication inactive (expired)
            var expiredComm = new CommunicationEntity
            {
                Content = "Expired",
                Start = now.AddDays(-3),
                End = now.AddDays(-1),
                Created = now
            };

            context.Communications.AddRange(activeComm, futureComm, expiredComm);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.Search(
                0,
                10,
                new SortOption<SortCommunication> { Direction = SortDirection.Descending, SortBy = SortCommunication.Active },
                "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.TotalCount);
            var items = result.Data.ToList();
            // Inactive communications should come first (Future and Expired)
            Assert.True(items[0].Content == "Future" || items[0].Content == "Expired");
            Assert.True(items[1].Content == "Future" || items[1].Content == "Expired");
            // Active communication should come last
            Assert.Equal("Active", items[2].Content);
        }

        [Fact]
        public async Task Search_SortByActiveDescending_ActiveCommunicationsFirst()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new CommunicationRepository(context, mapper);

            var now = DateTime.UtcNow;

            // Communication active
            var activeComm = new CommunicationEntity
            {
                Content = "Active",
                Start = now.AddDays(-1),
                End = now.AddDays(1),
                Created = now
            };

            // Communication inactive (future)
            var futureComm = new CommunicationEntity
            {
                Content = "Future",
                Start = now.AddDays(1),
                End = now.AddDays(2),
                Created = now
            };

            // Communication inactive (expired)
            var expiredComm = new CommunicationEntity
            {
                Content = "Expired",
                Start = now.AddDays(-3),
                End = now.AddDays(-1),
                Created = now
            };

            context.Communications.AddRange(activeComm, futureComm, expiredComm);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.Search(
                0,
                10,
                new SortOption<SortCommunication> { Direction = SortDirection.Ascending, SortBy = SortCommunication.Active },
                "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.TotalCount);
            var items = result.Data.ToList();
            // Active communication should come first
            Assert.Equal("Active", items[0].Content);
            // Inactive communications should come after (Future and Expired)
            Assert.True(items[1].Content == "Future" || items[1].Content == "Expired");
            Assert.True(items[2].Content == "Future" || items[2].Content == "Expired");
        }

        [Fact]
        public async Task Search_SortByActive_CommunicationWithNullEnd_ConsideredActive()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new CommunicationRepository(context, mapper);

            var now = DateTime.UtcNow;

            // Communication active with null End (no expiration)
            var activeNoEndComm = new CommunicationEntity
            {
                Content = "Active No End",
                Start = now.AddDays(-1),
                End = null,
                Created = now
            };

            // Communication inactive (future)
            var futureComm = new CommunicationEntity
            {
                Content = "Future",
                Start = now.AddDays(1),
                End = now.AddDays(2),
                Created = now
            };

            context.Communications.AddRange(activeNoEndComm, futureComm);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.Search(
                0,
                10,
                new SortOption<SortCommunication> { Direction = SortDirection.Ascending, SortBy = SortCommunication.Active },
                "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            var items = result.Data.ToList();
            // Communication with null End should be considered active and come first
            Assert.Equal("Active No End", items[0].Content);
            Assert.Equal("Future", items[1].Content);
        }

        [Fact]
        public async Task Search_SortByActive_CommunicationStartEqualsNow_ConsideredActive()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new CommunicationRepository(context, mapper);

            var now = DateTime.UtcNow;

            // Communication starting exactly now
            var startNowComm = new CommunicationEntity
            {
                Content = "Start Now",
                Start = now,
                End = now.AddDays(1),
                Created = now
            };

            // Communication inactive (future)
            var futureComm = new CommunicationEntity
            {
                Content = "Future",
                Start = now.AddSeconds(1),
                End = now.AddDays(2),
                Created = now
            };

            context.Communications.AddRange(startNowComm, futureComm);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.Search(
                0,
                10,
                new SortOption<SortCommunication> { Direction = SortDirection.Ascending, SortBy = SortCommunication.Active },
                "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            var items = result.Data.ToList();
            // Communication starting now should be considered active
            Assert.Equal("Start Now", items[0].Content);
            Assert.Equal("Future", items[1].Content);
        }

        [Fact]
        public async Task Search_SortByActive_CommunicationEndEqualsNow_ConsideredActive()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new CommunicationRepository(context, mapper);

            var now = DateTime.UtcNow;

            // Communication ending exactly now
            var endNowComm = new CommunicationEntity
            {
                Content = "End Now",
                Start = now.AddDays(-1),
                End = now,
                Created = now
            };

            // Communication inactive (expired)
            var expiredComm = new CommunicationEntity
            {
                Content = "Expired",
                Start = now.AddDays(-3),
                End = now.AddSeconds(-1),
                Created = now
            };

            context.Communications.AddRange(endNowComm, expiredComm);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.Search(
                0,
                10,
                new SortOption<SortCommunication> { Direction = SortDirection.Ascending, SortBy = SortCommunication.Active },
                "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            var items = result.Data.ToList();
            // Communication ending now should be considered active
            Assert.Equal("End Now", items[0].Content);
            Assert.Equal("Expired", items[1].Content);
        }

        [Fact]
        public async Task Search_SortByActive_FutureCommunicationWithNullEnd_ConsideredInactive()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new CommunicationRepository(context, mapper);

            var now = DateTime.UtcNow;

            // Communication future with null End
            var futureNoEndComm = new CommunicationEntity
            {
                Content = "Future No End",
                Start = now.AddDays(1),
                End = null,
                Created = now
            };

            // Communication active
            var activeComm = new CommunicationEntity
            {
                Content = "Active",
                Start = now.AddDays(-1),
                End = now.AddDays(1),
                Created = now
            };

            context.Communications.AddRange(futureNoEndComm, activeComm);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.Search(
                0,
                10,
                new SortOption<SortCommunication> { Direction = SortDirection.Ascending, SortBy = SortCommunication.Active },
                "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            var items = result.Data.ToList();
            // Active communication should come first
            Assert.Equal("Active", items[0].Content);
            // Future communication (even with null End) should be considered inactive
            Assert.Equal("Future No End", items[1].Content);
        }

        #endregion

        #region Edge Cases Tests

        [Fact]
        public async Task Search_EmptyDatabase_ReturnsEmptyResult()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new CommunicationRepository(context, mapper);

            // Act
            var result = await repository.Search(
                0,
                10,
                new SortOption<SortCommunication> { Direction = SortDirection.Ascending, SortBy = SortCommunication.Created },
                "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(0, result.TotalCount);
            Assert.Empty(result.Data);
        }

        [Fact]
        public async Task Search_CommunicationWithMaxLengthContent_ReturnsCorrectly()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new CommunicationRepository(context, mapper);

            var maxContent = new string('A', 200); // MaxLength = 200
            var comm = new CommunicationEntity
            {
                Content = maxContent,
                Start = DateTime.UtcNow,
                Created = DateTime.UtcNow
            };

            context.Communications.Add(comm);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.Search(
                0,
                10,
                new SortOption<SortCommunication> { Direction = SortDirection.Ascending, SortBy = SortCommunication.Created },
                "AAA");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.TotalCount);
            Assert.Equal(maxContent, result.Data.First().Content);
        }

        [Fact]
        public async Task Search_CombinedSearchAndSort_WorksCorrectly()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new CommunicationRepository(context, mapper);

            var comm1 = new CommunicationEntity
            {
                Content = "Important update first",
                Start = DateTime.UtcNow,
                Created = DateTime.UtcNow.AddDays(-3)
            };
            var comm2 = new CommunicationEntity
            {
                Content = "Important update second",
                Start = DateTime.UtcNow,
                Created = DateTime.UtcNow.AddDays(-1)
            };
            var comm3 = new CommunicationEntity
            {
                Content = "Regular message",
                Start = DateTime.UtcNow,
                Created = DateTime.UtcNow.AddDays(-2)
            };

            context.Communications.AddRange(comm1, comm2, comm3);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.Search(
                0,
                10,
                new SortOption<SortCommunication> { Direction = SortDirection.Ascending, SortBy = SortCommunication.Created },
                "Important");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            var items = result.Data.ToList();
            Assert.Equal("Important update first", items[0].Content);
            Assert.Equal("Important update second", items[1].Content);
        }

        [Fact]
        public async Task Search_CombinedSearchSortAndPagination_WorksCorrectly()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new CommunicationRepository(context, mapper);

            for (int i = 1; i <= 10; i++)
            {
                context.Communications.Add(new CommunicationEntity
                {
                    Content = $"Test communication {i}",
                    Start = DateTime.UtcNow,
                    Created = DateTime.UtcNow.AddMinutes(-i)
                });
            }
            await context.SaveChangesAsync();

            // Act
            var result = await repository.Search(
                2,
                3,
                new SortOption<SortCommunication> { Direction = SortDirection.Descending, SortBy = SortCommunication.Created },
                "Test");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result.TotalCount);
            Assert.Equal(3, result.Data.Count());
        }

        [Fact]
        public async Task Search_AllCommunicationsActive_SortByActiveReturnsAll()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new CommunicationRepository(context, mapper);

            var now = DateTime.UtcNow;

            for (int i = 1; i <= 5; i++)
            {
                context.Communications.Add(new CommunicationEntity
                {
                    Content = $"Active communication {i}",
                    Start = now.AddDays(-i),
                    End = now.AddDays(i),
                    Created = now
                });
            }
            await context.SaveChangesAsync();

            // Act
            var result = await repository.Search(
                0,
                10,
                new SortOption<SortCommunication> { Direction = SortDirection.Descending, SortBy = SortCommunication.Active },
                "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.TotalCount);
        }

        [Fact]
        public async Task Search_AllCommunicationsInactive_SortByActiveReturnsAll()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new CommunicationRepository(context, mapper);

            var now = DateTime.UtcNow;

            for (int i = 1; i <= 5; i++)
            {
                context.Communications.Add(new CommunicationEntity
                {
                    Content = $"Inactive communication {i}",
                    Start = now.AddDays(i),
                    End = now.AddDays(i + 1),
                    Created = now
                });
            }
            await context.SaveChangesAsync();

            // Act
            var result = await repository.Search(
                0,
                10,
                new SortOption<SortCommunication> { Direction = SortDirection.Ascending, SortBy = SortCommunication.Active },
                "");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5, result.TotalCount);
        }

        #endregion
    }
}
