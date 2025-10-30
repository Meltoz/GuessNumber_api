using Infrastructure;
using Infrastructure.Entities;
using Meltix.IntegrationTests;
using System.Net;
using System.Text;
using System.Text.Json;
using Web.ViewModels;

namespace IntegrationTests.Web
{
    public class AdminParametersTests
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        private readonly GuessNumberContext _context;

        public AdminParametersTests()
        {
            _context = DbContextProvider.SetupContext();
            _factory = new CustomWebApplicationFactory(_context);
            _client = _factory.CreateClient();

        }

        #region Actualities 

        #region GetAll Tests

        [Fact]
        public async Task GetAll_ShouldReturnOk()
        {
            // Arrange
            var title = "Test";
            var content = "Content";
            var startDate = new DateTime(2025, 10, 28, 20, 0, 0, DateTimeKind.Utc);
            _context.Actualities.Add(new ActualityEntity { Title = title, Content = content, StartPublish = startDate });
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/actualityAdmin/search?pageIndex=0&pageSize=10");
            response.EnsureSuccessStatusCode();

            // Assert
            var actualities = JsonSerializer.Deserialize<List<ActualityAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(actualities);
            Assert.Single(actualities);
            var actuality = actualities.First();
            Assert.Equal(title, actuality.Title);
            Assert.Equal(content, actuality.Content);
            Assert.Equal(startDate.ToString("o"), actuality.StartDate);
        }

        [Fact]
        public async Task GetAll_ShouldReturnTotalCountInHeader()
        {
            // Arrange
            _context.Actualities.AddRange(
                new ActualityEntity { Title = "Test1", Content = "Content1", StartPublish = DateTime.UtcNow },
                new ActualityEntity { Title = "Test2", Content = "Content2", StartPublish = DateTime.UtcNow },
                new ActualityEntity { Title = "Test3", Content = "Content3", StartPublish = DateTime.UtcNow }
            );
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/actualityAdmin/search?pageIndex=0&pageSize=10");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.True(response.Headers.Contains("X-Total-Count") || response.Headers.Contains("Total-Count"));
            var totalCount = response.Headers.GetValues("X-Total-Count").FirstOrDefault()
                          ?? response.Headers.GetValues("Total-Count").FirstOrDefault();
            Assert.Equal("3", totalCount);
        }

        [Fact]
        public async Task GetAll_WithPagination_ShouldReturnCorrectPage()
        {
            // Arrange
            for (int i = 0; i < 15; i++)
            {
                _context.Actualities.Add(new ActualityEntity
                {
                    Title = $"Test{i}",
                    Content = $"Content{i}",
                    StartPublish = DateTime.UtcNow.AddDays(-i)
                });
            }
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/actualityAdmin/search?pageIndex=1&pageSize=5");

            // Assert
            response.EnsureSuccessStatusCode();
            var actualities = JsonSerializer.Deserialize<List<ActualityAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(actualities);
            Assert.Equal(5, actualities.Count);
        }

        [Fact]
        public async Task GetAll_WithEmptyDatabase_ShouldReturnEmptyList()
        {
            // Act
            var response = await _client.GetAsync("/api/actualityAdmin/search?pageIndex=0&pageSize=10");

            // Assert
            response.EnsureSuccessStatusCode();
            var actualities = JsonSerializer.Deserialize<List<ActualityAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(actualities);
            Assert.Empty(actualities);
        }

        #endregion

        #region AddActuality Tests

        [Fact]
        public async Task AddActuality_WithValidData_ShouldReturnOk()
        {
            // Arrange
            var model = new ActualityAdminVM
            {
                Title = "New Actuality",
                Content = "New Content",
                StartDate = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm")
            };
            var content = new StringContent(
                JsonSerializer.Serialize(model),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/actualityAdmin/add", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<ActualityAdminVM>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(result);
            Assert.Equal(model.Title, result.Title);
            Assert.Equal(model.Content, result.Content);
            Assert.NotEqual(Guid.Empty, result.Id);
        }

        [Fact]
        public async Task AddActuality_WithStartAndEndDate_ShouldReturnOk()
        {
            // Arrange
            var startDate = DateTime.UtcNow;
            var endDate = DateTime.UtcNow.AddDays(7);
            var model = new ActualityAdminVM
            {
                Title = "Temporary Actuality",
                Content = "Temporary Content",
                StartDate = startDate.ToString("yyyy-MM-ddTHH:mm"),
                EndDate = endDate.ToString("yyyy-MM-ddTHH:mm")
            };
            var content = new StringContent(
                JsonSerializer.Serialize(model),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/actualityAdmin/add", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<ActualityAdminVM>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(result);
            Assert.NotNull(result.EndDate);
        }

        [Fact]
        public async Task AddActuality_WithInvalidStartDate_ShouldReturnBadRequest()
        {
            // Arrange
            var model = new ActualityAdminVM
            {
                Title = "Test",
                Content = "Content",
                StartDate = "invalid-date"
            };
            var content = new StringContent(
                JsonSerializer.Serialize(model),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/actualityAdmin/add", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var errorMessage = await response.Content.ReadAsStringAsync();
            Assert.Contains("Invalid start date format", errorMessage);
        }

        [Fact]
        public async Task AddActuality_WithInvalidEndDate_ShouldReturnBadRequest()
        {
            // Arrange
            var model = new ActualityAdminVM
            {
                Title = "Test",
                Content = "Content",
                StartDate = "2025-10-28T13:02",
                EndDate = "invalid-date"
            };
            var content = new StringContent(
                JsonSerializer.Serialize(model),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/actualityAdmin/add", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var errorMessage = await response.Content.ReadAsStringAsync();
            Assert.Contains("Invalid end date format.", errorMessage);
        }

        [Fact]
        public async Task AddActuality_WithInvalidModel_ShouldReturnBadRequest()
        {
            // Arrange
            var model = new ActualityAdminVM
            {
                // Title is required but missing
                Content = "Content",
                StartDate = DateTime.UtcNow.ToString("o")
            };
            var content = new StringContent(
                JsonSerializer.Serialize(model),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/actualityAdmin/add", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        #endregion

        #region UpdateActuality Tests

        [Fact]
        public async Task UpdateActuality_WithValidData_ShouldReturnOk()
        {
            // Arrange
            var existingActuality = new ActualityEntity
            {
                Id = Guid.NewGuid(),
                Title = "Original Title",
                Content = "Original Content",
                StartPublish = DateTime.UtcNow
            };
            _context.Actualities.Add(existingActuality);
            await _context.SaveChangesAsync();

            var updateModel = new ActualityAdminVM
            {
                Id = existingActuality.Id,
                Title = "Updated Title",
                Content = "Updated Content",
                StartDate = DateTime.UtcNow.ToString("o")
            };
            var content = new StringContent(
                JsonSerializer.Serialize(updateModel),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PatchAsync("/api/actualityAdmin/update", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<ActualityAdminVM>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(result);
            Assert.Equal(updateModel.Title, result.Title);
            Assert.Equal(updateModel.Content, result.Content);
            Assert.Equal(existingActuality.Id, result.Id);
        }

        [Fact]
        public async Task UpdateActuality_WithInvalidModel_ShouldReturnBadRequest()
        {
            // Arrange
            var updateModel = new ActualityAdminVM
            {
                Id = Guid.NewGuid(),
                // Missing required fields
            };
            var content = new StringContent(
                JsonSerializer.Serialize(updateModel),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PatchAsync("/api/actualityAdmin/update", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateActuality_WithNonExistentId_ShouldReturnError()
        {
            // Arrange
            var updateModel = new ActualityAdminVM
            {
                Id = Guid.NewGuid(),
                Title = "Test",
                Content = "Content",
                StartDate = DateTime.UtcNow.ToString("o")
            };
            var content = new StringContent(
                JsonSerializer.Serialize(updateModel),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PatchAsync("/api/actualityAdmin/update", content);

            // Assert
            Assert.False(response.IsSuccessStatusCode);
        }

        #endregion

        #region DeleteActuality Tests

        [Fact]
        public async Task DeleteActuality_WithExistingId_ShouldReturnOk()
        {
            // Arrange
            var actuality = new ActualityEntity
            {
                Id = Guid.NewGuid(),
                Title = "To Delete",
                Content = "Content",
                StartPublish = DateTime.UtcNow
            };
            _context.Actualities.Add(actuality);
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.DeleteAsync($"/api/actualityAdmin/delete?id={actuality.Id}");

            // Assert
            response.EnsureSuccessStatusCode();

            // Verify deletion
            var deletedActuality = await _context.Actualities.FindAsync(actuality.Id);
            Assert.Null(deletedActuality);
        }

        [Fact]
        public async Task DeleteActuality_WithNonExistentId_ShouldReturnError()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var response = await _client.DeleteAsync($"/api/actualityAdmin/delete?id={nonExistentId}");

            // Assert
            Assert.False(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task DeleteActuality_WithInvalidGuid_ShouldReturnBadRequest()
        {
            // Act
            var response = await _client.DeleteAsync("/api/actualityAdmin/delete?id=invalid-guid");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        #endregion

        #endregion

        #region Communication

        #region Search

        [Fact]
        public async Task SearchCommunication_ShouldReturnOk()
        {
            // Arrange
            var content = "Important message";
            var startDate = new DateTime(2025, 10, 28, 20, 0, 0, DateTimeKind.Utc);
            var endDate = new DateTime(2025, 11, 28, 20, 0, 0, DateTimeKind.Utc);
            _context.Communications.Add(new CommunicationEntity
            {
                Content = content,
                Start = startDate,
                End = endDate,
            });
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/communicationAdmin/search?pageIndex=0&pageSize=10");
            response.EnsureSuccessStatusCode();

            // Assert
            var communications = JsonSerializer.Deserialize<List<CommunicationAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(communications);
            Assert.Single(communications);
            var communication = communications.First();
            Assert.Equal(content, communication.Content);
            Assert.Equal(startDate.ToString("o"), communication.StartDate);
            Assert.Equal(endDate.ToString("o"), communication.EndDate);
        }

        [Fact]
        public async Task SearchCommunication_ShouldReturnTotalCountInHeader()
        {
            // Arrange
            _context.Communications.AddRange(
                new CommunicationEntity { Content = "Message1", Start = DateTime.UtcNow},
                new CommunicationEntity { Content = "Message2", Start = DateTime.UtcNow},
                new CommunicationEntity { Content = "Message3", Start = DateTime.UtcNow}
            );
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/communicationAdmin/search?pageIndex=0&pageSize=10");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.True(response.Headers.Contains("X-Total-Count") || response.Headers.Contains("Total-Count"));
            var totalCount = response.Headers.GetValues("X-Total-Count").FirstOrDefault()
                          ?? response.Headers.GetValues("Total-Count").FirstOrDefault();
            Assert.Equal("3", totalCount);
        }

        [Fact]
        public async Task SearchCommunication_WithPagination_ShouldReturnCorrectPage()
        {
            // Arrange
            for (int i = 0; i < 15; i++)
            {
                _context.Communications.Add(new CommunicationEntity
                {
                    Content = $"Message{i}",
                    Start = DateTime.UtcNow.AddDays(-i),
                });
            }
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/communicationAdmin/search?pageIndex=1&pageSize=5");

            // Assert
            response.EnsureSuccessStatusCode();
            var communications = JsonSerializer.Deserialize<List<CommunicationAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(communications);
            Assert.Equal(5, communications.Count);
        }

        [Fact]
        public async Task SearchCommunication_WithEmptyDatabase_ShouldReturnEmptyList()
        {
            // Act
            var response = await _client.GetAsync("/api/communicationAdmin/search?pageIndex=0&pageSize=10");

            // Assert
            response.EnsureSuccessStatusCode();
            var communications = JsonSerializer.Deserialize<List<CommunicationAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(communications);
            Assert.Empty(communications);
        }

        [Fact]
        public async Task SearchCommunication_WithMessageFilter_ShouldReturnFilteredResults()
        {
            // Arrange
            _context.Communications.AddRange(
                new CommunicationEntity { Content = "Important announcement", Start = DateTime.UtcNow },
                new CommunicationEntity { Content = "Regular message", Start = DateTime.UtcNow},
                new CommunicationEntity { Content = "Important update", Start = DateTime.UtcNow}
            );
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/communicationAdmin/search?pageIndex=0&pageSize=10&message=Important");

            // Assert
            response.EnsureSuccessStatusCode();
            var communications = JsonSerializer.Deserialize<List<CommunicationAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(communications);
            Assert.Equal(2, communications.Count);
            Assert.All(communications, c => Assert.Contains("Important", c.Content, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task SearchCommunication_WithMessageFilter_ShouldReturnEmptyWhenNoMatch()
        {
            // Arrange
            _context.Communications.AddRange(
                new CommunicationEntity { Content = "Message1", Start = DateTime.UtcNow},
                new CommunicationEntity { Content = "Message2", Start = DateTime.UtcNow }
            );
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/communicationAdmin/search?pageIndex=0&pageSize=10&message=NonExistent");

            // Assert
            response.EnsureSuccessStatusCode();
            var communications = JsonSerializer.Deserialize<List<CommunicationAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(communications);
            Assert.Empty(communications);
        }

        [Fact]
        public async Task SearchCommunication_ShouldSortByActiveDescending()
        {
            // Arrange
            _context.Communications.AddRange(
                new CommunicationEntity { Content = "Message1", Start = DateTime.UtcNow },
                new CommunicationEntity { Content = "Message2", Start = DateTime.UtcNow },
                new CommunicationEntity { Content = "Message3", Start = DateTime.UtcNow },
                new CommunicationEntity { Content = "Message4", Start = DateTime.UtcNow }
            );
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/communicationAdmin/search?pageIndex=0&pageSize=10");

            // Assert
            response.EnsureSuccessStatusCode();
            var communications = JsonSerializer.Deserialize<List<CommunicationAdminVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(communications);
            Assert.Equal(4, communications.Count);

            // TO Do ajouté la vérification du tri
        }

        #endregion

        #endregion

    }
}
