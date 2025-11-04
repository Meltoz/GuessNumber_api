using Domain.Enums;
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
                StartDate = startDate.ToString("o"),
                EndDate = endDate.ToString("o")
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

        #region UpdateCommunication Tests

        [Fact]
        public async Task UpdateCommunication_WithValidData_ShouldReturnOk()
        {
            // Arrange
            var existingCommunication = new CommunicationEntity
            {
                Id = Guid.NewGuid(),
                Content = "Original Content",
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddDays(5)
            };
            _context.Communications.Add(existingCommunication);
            await _context.SaveChangesAsync();

            var updateModel = new CommunicationAdminVM
            {
                Id = existingCommunication.Id,
                Content = "Updated Content",
                StartDate = DateTime.UtcNow.AddDays(1).ToString("o"),
                EndDate = DateTime.UtcNow.AddDays(6).ToString("o")
            };
            var content = new StringContent(
                JsonSerializer.Serialize(updateModel),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PatchAsync("/api/communicationAdmin/update", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<CommunicationAdminVM>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(result);
            Assert.Equal(updateModel.Content, result.Content);
            Assert.Equal(existingCommunication.Id, result.Id);
        }

        [Fact]
        public async Task UpdateCommunication_WithInvalidModel_ShouldReturnBadRequest()
        {
            // Arrange
            var updateModel = new CommunicationAdminVM
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
            var response = await _client.PatchAsync("/api/communicationAdmin/update", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateCommunication_WithNonExistentId_ShouldReturnError()
        {
            // Arrange
            var updateModel = new CommunicationAdminVM
            {
                Id = Guid.NewGuid(),
                Content = "Test Content",
                StartDate = DateTime.UtcNow.ToString("o"),
                EndDate = DateTime.UtcNow.AddDays(1).ToString("o")
            };
            var content = new StringContent(
                JsonSerializer.Serialize(updateModel),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PatchAsync("/api/communicationAdmin/update", content);

            // Assert
            Assert.False(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task UpdateCommunication_WithEmptyId_ShouldReturnError()
        {
            // Arrange
            var updateModel = new CommunicationAdminVM
            {
                Id = Guid.Empty,
                Content = "Test Content",
                StartDate = DateTime.UtcNow.ToString("o"),
                EndDate = DateTime.UtcNow.AddDays(1).ToString("o")
            };
            var content = new StringContent(
                JsonSerializer.Serialize(updateModel),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PatchAsync("/api/communicationAdmin/update", content);

            // Assert
            Assert.False(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task UpdateCommunication_WithOnlyContentChange_ShouldReturnOk()
        {
            // Arrange
            var startDate = DateTime.UtcNow;
            var endDate = DateTime.UtcNow.AddDays(5);
            var existingCommunication = new CommunicationEntity
            {
                Id = Guid.NewGuid(),
                Content = "Original Content",
                Start = startDate,
                End = endDate
            };
            _context.Communications.Add(existingCommunication);
            await _context.SaveChangesAsync();

            var updateModel = new CommunicationAdminVM
            {
                Id = existingCommunication.Id,
                Content = "Updated Content Only",
                StartDate = startDate.ToString("o"),
                EndDate = endDate.ToString("o")
            };
            var content = new StringContent(
                JsonSerializer.Serialize(updateModel),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PatchAsync("/api/communicationAdmin/update", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<CommunicationAdminVM>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(result);
            Assert.Equal("Updated Content Only", result.Content);
        }

        [Fact]
        public async Task UpdateCommunication_WithOnlyDatesChange_ShouldReturnOk()
        {
            // Arrange
            var existingCommunication = new CommunicationEntity
            {
                Id = Guid.NewGuid(),
                Content = "Same Content",
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddDays(5)
            };
            _context.Communications.Add(existingCommunication);
            await _context.SaveChangesAsync();

            var newStartDate = DateTime.UtcNow.AddDays(2);
            var newEndDate = DateTime.UtcNow.AddDays(7);
            var updateModel = new CommunicationAdminVM
            {
                Id = existingCommunication.Id,
                Content = "Same Content",
                StartDate = newStartDate.ToString("o"),
                EndDate = newEndDate.ToString("o")
            };
            var content = new StringContent(
                JsonSerializer.Serialize(updateModel),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PatchAsync("/api/communicationAdmin/update", content);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<CommunicationAdminVM>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(result);
            Assert.Equal("Same Content", result.Content);
        }

        #endregion

        #region DeleteCommunication Tests

        [Fact]
        public async Task DeleteCommunication_WithExistingId_ShouldReturnOk()
        {
            // Arrange
            var communication = new CommunicationEntity
            {
                Id = Guid.NewGuid(),
                Content = "To Delete",
                Start = DateTime.UtcNow,
                End = DateTime.UtcNow.AddDays(5)
            };
            _context.Communications.Add(communication);
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.DeleteAsync($"/api/communicationAdmin/delete?idCommunication={communication.Id}");

            // Assert
            response.EnsureSuccessStatusCode();

            // Verify deletion
            var deletedCommunication = await _context.Communications.FindAsync(communication.Id);
            Assert.Null(deletedCommunication);
        }

        [Fact]
        public async Task DeleteCommunication_WithNonExistentId_ShouldReturnError()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var response = await _client.DeleteAsync($"/api/communicationAdmin/delete?idCommunication={nonExistentId}");

            // Assert
            Assert.False(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task DeleteCommunication_WithInvalidGuid_ShouldReturnBadRequest()
        {
            // Act
            var response = await _client.DeleteAsync("/api/communicationAdmin/delete?idCommunication=invalid-guid");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DeleteCommunication_WithEmptyGuid_ShouldReturnError()
        {
            // Act
            var response = await _client.DeleteAsync($"/api/communicationAdmin/delete?idCommunication={Guid.Empty}");

            // Assert
            Assert.False(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task DeleteCommunication_WithMissingParameter_ShouldReturnBadRequest()
        {
            // Act
            var response = await _client.DeleteAsync("/api/communicationAdmin/delete");

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        #endregion

        #endregion

        #region Reports

        #region GetAll - Tests

        [Fact]
        public async Task GetReport_ShouldReturnOk()
        {
            // Arrange
            var explanation = "Bug on login page";
            var mail = "user@example.com";
            var type = TypeReport.Bug;
            var context = ContextReport.Site;

            _context.Reports.Add(new ReportEntity
            {
                Explanation = explanation,
                Mail = mail,
                Type = type,
                Context = context,
                Created = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/reportAdmin/search?pageIndex=0&pageSize=10");
            response.EnsureSuccessStatusCode();

            // Assert
            var reports = JsonSerializer.Deserialize<List<ReportVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            Assert.NotNull(reports);
            Assert.Single(reports);
            var report = reports.First();
            Assert.Equal(explanation, report.Explanation);
            Assert.Equal(mail, report.Mail);
            Assert.Equal(type.ToString(), report.Type);
            Assert.Equal(context.ToString(), report.Context);
        }

        [Fact]
        public async Task GetReport_ShouldReturnTotalCountInHeader()
        {
            // Arrange
            _context.Reports.AddRange(
                new ReportEntity
                {
                    Explanation = "Bug 1",
                    Mail = "user1@example.com",
                    Type = TypeReport.Bug,
                    Context = ContextReport.Site,
                    Created = DateTime.UtcNow
                },
                new ReportEntity
                {
                    Explanation = "Bug 2",
                    Mail = "user2@example.com",
                    Type = TypeReport.Bug,
                    Context = ContextReport.Question,
                    Created = DateTime.UtcNow
                },
                new ReportEntity
                {
                    Explanation = "Improvement 1",
                    Mail = "user3@example.com",
                    Type = TypeReport.Improuvment,
                    Context = ContextReport.Site,
                    Created = DateTime.UtcNow
                }
            );
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/reportAdmin/search?pageIndex=0&pageSize=10");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.True(response.Headers.Contains("X-Total-Count") || response.Headers.Contains("Total-Count"));
            var totalCount = response.Headers.GetValues("X-Total-Count").FirstOrDefault()
                          ?? response.Headers.GetValues("Total-Count").FirstOrDefault();
            Assert.Equal("3", totalCount);
        }

        [Fact]
        public async Task GetReport_WithSearch_ShouldReturnFilteredResults()
        {
            // Arrange
            _context.Reports.AddRange(
                new ReportEntity
                {
                    Explanation = "Login bug on homepage",
                    Mail = "user1@example.com",
                    Type = TypeReport.Bug,
                    Context = ContextReport.Site,
                    Created = DateTime.UtcNow
                },
                new ReportEntity
                {
                    Explanation = "Feature request for dark mode",
                    Mail = "user2@example.com",
                    Type = TypeReport.Improuvment,
                    Context = ContextReport.Question,
                    Created = DateTime.UtcNow
                }
            );
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/reportAdmin/search?pageIndex=0&pageSize=10&search=login bug on homepage");
            response.EnsureSuccessStatusCode();

            // Assert
            var reports = JsonSerializer.Deserialize<List<ReportVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            Assert.NotNull(reports);
            Assert.Single(reports);
            Assert.Contains("Login bug", reports.First().Explanation);
        }

        [Fact]
        public async Task GetReport_WithSearchByMail_ShouldReturnMatchingReports()
        {
            // Arrange
            var searchMail = "specific.user@example.com";
            _context.Reports.AddRange(
                new ReportEntity
                {
                    Explanation = "Bug report 1",
                    Mail = searchMail,
                    Type = TypeReport.Bug,
                    Context = ContextReport.Site,
                    Created = DateTime.UtcNow
                },
                new ReportEntity
                {
                    Explanation = "Bug report 2",
                    Mail = "other@example.com",
                    Type = TypeReport.Bug,
                    Context = ContextReport.Question,
                    Created = DateTime.UtcNow
                }
            );
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync($"/api/reportAdmin/search?pageIndex=0&pageSize=10&search={searchMail}");
            response.EnsureSuccessStatusCode();

            // Assert
            var reports = JsonSerializer.Deserialize<List<ReportVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            Assert.NotNull(reports);
            Assert.Single(reports);
            Assert.Equal(searchMail, reports.First().Mail);
        }

        [Fact]
        public async Task GetReport_WithPagination_ShouldReturnCorrectPage()
        {
            // Arrange
            for (int i = 0; i < 15; i++)
            {
                _context.Reports.Add(new ReportEntity
                {
                    Explanation = $"Bug report {i}",
                    Mail = $"user{i}@example.com",
                    Type = TypeReport.Bug,
                    Context = ContextReport.Site,
                    Created = DateTime.UtcNow.AddMinutes(-i)
                });
            }
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/reportAdmin/search?pageIndex=1&pageSize=10");
            response.EnsureSuccessStatusCode();

            // Assert
            var reports = JsonSerializer.Deserialize<List<ReportVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            Assert.NotNull(reports);
            Assert.Equal(5, reports.Count); // 15 total, page 1 (second page) should have 5 items

            var totalCount = response.Headers.GetValues("X-Total-Count").FirstOrDefault()
                          ?? response.Headers.GetValues("Total-Count").FirstOrDefault();
            Assert.Equal("15", totalCount);
        }

        [Fact]
        public async Task GetReport_WithNegativePageIndex_ShouldReturnBadRequest()
        {
            // Act
            var response = await _client.GetAsync("/api/reportAdmin/search?pageIndex=-1&pageSize=10");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetReport_WithZeroPageSize_ShouldReturnBadRequest()
        {
            // Act
            var response = await _client.GetAsync("/api/reportAdmin/search?pageIndex=0&pageSize=0");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetReport_WithNegativePageSize_ShouldReturnBadRequest()
        {
            // Act
            var response = await _client.GetAsync("/api/reportAdmin/search?pageIndex=0&pageSize=-5");

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetReport_WithNoResults_ShouldReturnEmptyList()
        {
            // Act
            var response = await _client.GetAsync("/api/reportAdmin/search?pageIndex=0&pageSize=10");
            response.EnsureSuccessStatusCode();

            // Assert
            var reports = JsonSerializer.Deserialize<List<ReportVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            Assert.NotNull(reports);
            Assert.Empty(reports);

            var totalCount = response.Headers.GetValues("X-Total-Count").FirstOrDefault()
                          ?? response.Headers.GetValues("Total-Count").FirstOrDefault();
            Assert.Equal("0", totalCount);
        }

        [Fact]
        public async Task GetReport_WithDifferentReportTypes_ShouldReturnAll()
        {
            // Arrange
            _context.Reports.AddRange(
                new ReportEntity
                {
                    Explanation = "Bug report",
                    Mail = "user1@example.com",
                    Type = TypeReport.Bug,
                    Context = ContextReport.Site,
                    Created = DateTime.UtcNow
                },
                new ReportEntity
                {
                    Explanation = "Improvement suggestion",
                    Mail = "user2@example.com",
                    Type = TypeReport.Improuvment,
                    Context = ContextReport.Question,
                    Created = DateTime.UtcNow
                }
            );
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/reportAdmin/search?pageIndex=0&pageSize=10");
            response.EnsureSuccessStatusCode();

            // Assert
            var reports = JsonSerializer.Deserialize<List<ReportVM>>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            Assert.NotNull(reports);
            Assert.Equal(2, reports.Count);
            Assert.Contains(reports, r => r.Type == TypeReport.Bug.ToString());
            Assert.Contains(reports, r => r.Type == TypeReport.Improuvment.ToString());
        }

        #endregion

        [Fact]
        public async Task GetReportById_WithValidId_ShouldReturnOk()
        {
            // Arrange
            var existingReport = new ReportEntity
            {
                Id = Guid.NewGuid(),
                Type = TypeReport.Bug,
                Context = ContextReport.Site,
                Explanation = "Test Explanation",
                Mail = "test@example.com"
            };
            _context.Reports.Add(existingReport);
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync($"/api/reportAdmin/getbyid?id={existingReport.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<ReportVM>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(result);
            Assert.Equal(existingReport.Id, result.Id);
            Assert.Equal(nameof(TypeReport.Bug), result.Type);
            Assert.Equal(nameof(ContextReport.Site), result.Context);
            Assert.Equal(existingReport.Explanation, result.Explanation);
            Assert.Equal(existingReport.Mail, result.Mail);
        }

        [Fact]
        public async Task GetReportById_WithImprovementType_ShouldReturnOk()
        {
            // Arrange
            var existingReport = new ReportEntity
            {
                Id = Guid.NewGuid(),
                Type = TypeReport.Improuvment,
                Context = ContextReport.Question,
                Explanation = "Suggestion for improvement",
                Mail = "improvement@example.com"
            };
            _context.Reports.Add(existingReport);
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync($"/api/reportAdmin/getbyid?id={existingReport.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<ReportVM>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(result);
            Assert.Equal(nameof(TypeReport.Improuvment), result.Type);
            Assert.Equal(nameof(ContextReport.Question), result.Context);
        }

        [Fact]
        public async Task GetReportById_WithNonExistentId_ShouldReturnError()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/api/reportAdmin/getbyid?id={nonExistentId}");

            // Assert
            Assert.False(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task GetReportById_WithInvalidGuid_ShouldReturnBadRequest()
        {
            // Arrange
            var invalidGuid = "invalid-guid";

            // Act
            var response = await _client.GetAsync($"/api/reportAdmin/getbyid?id={invalidGuid}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetReportById_WithNullMail_ShouldReturnOk()
        {
            // Arrange
            var existingReport = new ReportEntity
            {
                Id = Guid.NewGuid(),
                Type = TypeReport.Bug,
                Context = ContextReport.Site,
                Explanation = "Explanation without email",
                Mail = null
            };
            _context.Reports.Add(existingReport);
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync($"/api/reportAdmin/getbyid?id={existingReport.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            var result = JsonSerializer.Deserialize<ReportVM>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            Assert.NotNull(result);
            Assert.Equal(existingReport.Id, result.Id);
            Assert.Null(result.Mail);
        }

        [Fact]
        public async Task DeleteReport_WithValidId_ShouldReturnOk()
        {
            // Arrange
            var existingReport = new ReportEntity
            {
                Id = Guid.NewGuid(),
                Type = TypeReport.Bug,
                Context = ContextReport.Site,
                Explanation = "This report will be deleted",
                Mail = "delete@example.com"
            };
            _context.Reports.Add(existingReport);
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.DeleteAsync($"/api/reportAdmin/delete?id={existingReport.Id}");

            // Assert
            response.EnsureSuccessStatusCode();

            // Vérifier que le rapport a bien été supprimé
            var deletedReport = await _context.Reports.FindAsync(existingReport.Id);
            Assert.Null(deletedReport);
        }

        [Fact]
        public async Task DeleteReport_WithNonExistentId_ShouldReturnError()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var response = await _client.DeleteAsync($"/api/reportAdmin/delete?id={nonExistentId}");

            // Assert
            Assert.False(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task DeleteReport_WithInvalidGuid_ShouldReturnBadRequest()
        {
            // Arrange
            var invalidGuid = "invalid-guid";

            // Act
            var response = await _client.DeleteAsync($"/api/reportAdmin/delete?id={invalidGuid}");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DeleteReport_ShouldNotAffectOtherReports()
        {
            // Arrange
            var report1 = new ReportEntity
            {
                Id = Guid.NewGuid(),
                Type = TypeReport.Bug,
                Context = ContextReport.Site,
                Explanation = "First report",
                Mail = "report1@example.com"
            };
            var report2 = new ReportEntity
            {
                Id = Guid.NewGuid(),
                Type = TypeReport.Improuvment,
                Context = ContextReport.Question,
                Explanation = "Second report",
                Mail = "report2@example.com"
            };
            _context.Reports.AddRange(report1, report2);
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.DeleteAsync($"/api/reportAdmin/delete?id={report1.Id}");

            // Assert
            response.EnsureSuccessStatusCode();

            var deletedReport = await _context.Reports.FindAsync(report1.Id);
            var remainingReport = await _context.Reports.FindAsync(report2.Id);

            Assert.Null(deletedReport);
            Assert.NotNull(remainingReport);
            Assert.Equal(TypeReport.Improuvment, remainingReport.Type);
            Assert.Equal(ContextReport.Question, remainingReport.Context);
        }

        [Fact]
        public async Task GetReportById_WithAllEnumCombinations_ShouldReturnCorrectValues()
        {
            // Arrange - Tester toutes les combinaisons d'enums
            var reports = new[]
            {
        new ReportEntity
        {
            Id = Guid.NewGuid(),
            Type = TypeReport.Bug,
            Context = ContextReport.Site,
            Explanation = "Bug on Site",
            Mail = "test1@example.com"
        },
        new ReportEntity
        {
            Id = Guid.NewGuid(),
            Type = TypeReport.Bug,
            Context = ContextReport.Question,
            Explanation = "Bug on Question",
            Mail = "test2@example.com"
        },
        new ReportEntity
        {
            Id = Guid.NewGuid(),
            Type = TypeReport.Improuvment,
            Context = ContextReport.Site,
            Explanation = "Improvement for Site",
            Mail = "test3@example.com"
        },
        new ReportEntity
        {
            Id = Guid.NewGuid(),
            Type = TypeReport.Improuvment,
            Context = ContextReport.Question,
            Explanation = "Improvement for Question",
            Mail = "test4@example.com"
        }
    };
            _context.Reports.AddRange(reports);
            await _context.SaveChangesAsync();

            // Act & Assert
            foreach (var report in reports)
            {
                var response = await _client.GetAsync($"/api/reportAdmin/getbyid?id={report.Id}");
                response.EnsureSuccessStatusCode();

                var result = JsonSerializer.Deserialize<ReportVM>(
                    await response.Content.ReadAsStringAsync(),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                Assert.NotNull(result);
                Assert.Equal(report.Type.ToString(), result.Type);
                Assert.Equal(report.Context.ToString(), result.Context);
            }
        }

        #endregion

    }
}
