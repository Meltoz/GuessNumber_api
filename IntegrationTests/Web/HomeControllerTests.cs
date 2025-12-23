using Infrastructure;
using Meltix.IntegrationTests;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text;
using System.Text.Json;
using Web.ViewModels;

namespace IntegrationTests.Web
{
    public class HomeControllerTests
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;
        private readonly GuessNumberContext _context;
        private readonly JsonSerializerOptions _jsonOptions;

        public HomeControllerTests()
        {
            _context = DbContextProvider.SetupContext();
            _factory = new CustomWebApplicationFactory(_context);
            _client = _factory.CreateClient();
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        #region AddProposal

        [Fact]
        public async Task AddProposal_WithValidData_ShouldReturnOk()
        {
            // Arrange
            var newProposal = new ProposalVM
            {
                Libelle = "Quelle est la population de Paris en 2024 ?",
                Response = "2161000",
                Author = "Wikipedia",
                Source = "https://fr.wikipedia.org/wiki/Paris"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newProposal),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/Home/AddProposal", content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task AddProposal_WithValidData_ShouldInsertProposalInDatabase()
        {
            // Arrange
            var newProposal = new ProposalVM
            {
                Libelle = "Combien de kilomètres mesure le tour de France ?",
                Response = "3500",
                Author = "Tour de France",
                Source = "https://www.letour.fr"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newProposal),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/Home/AddProposal", content);

            // Assert
            response.EnsureSuccessStatusCode();

            var proposalInDb = await _context.Proposals
                .FirstOrDefaultAsync(p => p.Libelle == newProposal.Libelle);

            Assert.NotNull(proposalInDb);
            Assert.Equal(newProposal.Libelle, proposalInDb.Libelle);
            Assert.Equal(newProposal.Response, proposalInDb.Response);
            Assert.Equal(newProposal.Author, proposalInDb.Author);
            Assert.Equal(newProposal.Source, proposalInDb.Source);
        }

        [Fact]
        public async Task AddProposal_WithMinimalFields_ShouldReturnOk()
        {
            // Arrange
            var newProposal = new ProposalVM
            {
                Libelle = "Quelle est la température d'ébullition de l'eau ?",
                Response = "100",
                Author = null,
                Source = null
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newProposal),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/Home/AddProposal", content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task AddProposal_WithMinimalFields_ShouldInsertProposalInDatabase()
        {
            // Arrange
            var newProposal = new ProposalVM
            {
                Libelle = "Combien y a-t-il de continents sur Terre ?",
                Response = "7",
                Author = null,
                Source = null
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newProposal),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/Home/AddProposal", content);

            // Assert
            response.EnsureSuccessStatusCode();

            var proposalInDb = await _context.Proposals
                .FirstOrDefaultAsync(p => p.Libelle == newProposal.Libelle);

            Assert.NotNull(proposalInDb);
            Assert.Equal(newProposal.Libelle, proposalInDb.Libelle);
            Assert.Equal(newProposal.Response, proposalInDb.Response);
            Assert.Null(proposalInDb.Author);
            Assert.Null(proposalInDb.Source);
        }

        [Fact]
        public async Task AddProposal_WithNullProposal_ShouldReturnBadRequest()
        {
            // Arrange
            var content = new StringContent(
                "null",
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/Home/AddProposal", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task AddProposal_WithInvalidLibelleTooShort_ShouldReturnInternalServerError()
        {
            // Arrange
            var newProposal = new ProposalVM
            {
                Libelle = "Ab",
                Response = "100",
                Author = "Test",
                Source = "https://test.com"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newProposal),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/Home/AddProposal", content);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task AddProposal_WithInvalidLibelleNotEnoughWords_ShouldReturnInternalServerError()
        {
            // Arrange
            var newProposal = new ProposalVM
            {
                Libelle = "Two words",
                Response = "100",
                Author = "Test",
                Source = "https://test.com"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newProposal),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/Home/AddProposal", content);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task AddProposal_WithInvalidResponseNotANumber_ShouldReturnInternalServerError()
        {
            // Arrange
            var newProposal = new ProposalVM
            {
                Libelle = "Quelle est la réponse à cette question ?",
                Response = "not a number",
                Author = "Test",
                Source = "https://test.com"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newProposal),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/Home/AddProposal", content);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task AddProposal_WithInvalidAuthorTooShort_ShouldReturnInternalServerError()
        {
            // Arrange
            var newProposal = new ProposalVM
            {
                Libelle = "Quelle est la réponse à cette question ?",
                Response = "42",
                Author = "AB",
                Source = "https://test.com"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newProposal),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/Home/AddProposal", content);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task AddProposal_WithInvalidSourceNotHttp_ShouldReturnInternalServerError()
        {
            // Arrange
            var newProposal = new ProposalVM
            {
                Libelle = "Quelle est la réponse à cette question ?",
                Response = "42",
                Author = "Test Author",
                Source = "not-a-url"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newProposal),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/Home/AddProposal", content);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task AddProposal_WithSpecialCharacters_ShouldReturnOk()
        {
            // Arrange
            var newProposal = new ProposalVM
            {
                Libelle = "Quelle est la température à l'équateur ?",
                Response = "30",
                Author = "Auteur Ünîçödé",
                Source = "https://météo-française.fr"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newProposal),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/Home/AddProposal", content);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task AddProposal_WithSpecialCharacters_ShouldInsertCorrectly()
        {
            // Arrange
            var newProposal = new ProposalVM
            {
                Libelle = "Combien de degrés Celsius fait-il à Montréal ?",
                Response = "25",
                Author = "Météo Québec",
                Source = "https://météo.ca"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newProposal),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/Home/AddProposal", content);

            // Assert
            response.EnsureSuccessStatusCode();

            var proposalInDb = await _context.Proposals
                .FirstOrDefaultAsync(p => p.Libelle == newProposal.Libelle);

            Assert.NotNull(proposalInDb);
            Assert.Equal(newProposal.Libelle, proposalInDb.Libelle);
            Assert.Equal(newProposal.Author, proposalInDb.Author);
        }

        [Theory]
        [InlineData("0")]
        [InlineData("1")]
        [InlineData("999999")]
        [InlineData("2147483647")]
        public async Task AddProposal_WithDifferentResponseValues_ShouldReturnOk(string response)
        {
            // Arrange
            var newProposal = new ProposalVM
            {
                Libelle = $"Quelle est la réponse numéro {response} ?",
                Response = response,
                Author = "Test Author",
                Source = "https://test.com"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newProposal),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var httpResponse = await _client.PostAsync("/api/Home/AddProposal", content);

            // Assert
            httpResponse.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("0")]
        [InlineData("1")]
        [InlineData("999999")]
        [InlineData("2147483647")]
        public async Task AddProposal_WithDifferentResponseValues_ShouldInsertCorrectly(string response)
        {
            // Arrange
            var newProposal = new ProposalVM
            {
                Libelle = $"Quel est le nombre exact {response} ?",
                Response = response,
                Author = null,
                Source = null
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newProposal),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var httpResponse = await _client.PostAsync("/api/Home/AddProposal", content);

            // Assert
            httpResponse.EnsureSuccessStatusCode();

            var proposalInDb = await _context.Proposals
                .FirstOrDefaultAsync(p => p.Response == response && p.Libelle.Contains(response));

            Assert.NotNull(proposalInDb);
            Assert.Equal(response, proposalInDb.Response);
        }

        [Fact]
        public async Task AddProposal_CalledMultipleTimes_ShouldInsertAllProposals()
        {
            // Arrange
            var proposal1 = new ProposalVM
            {
                Libelle = "Quelle est la hauteur de la Tour Eiffel ?",
                Response = "330",
                Author = "Test",
                Source = "https://test.com"
            };

            var proposal2 = new ProposalVM
            {
                Libelle = "Quelle est la population de Tokyo ?",
                Response = "14000000",
                Author = "Stats",
                Source = "https://stats.com"
            };

            var content1 = new StringContent(
                JsonSerializer.Serialize(proposal1),
                Encoding.UTF8,
                "application/json"
            );

            var content2 = new StringContent(
                JsonSerializer.Serialize(proposal2),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response1 = await _client.PostAsync("/api/Home/AddProposal", content1);
            var response2 = await _client.PostAsync("/api/Home/AddProposal", content2);

            // Assert
            response1.EnsureSuccessStatusCode();
            response2.EnsureSuccessStatusCode();

            var proposalsInDb = await _context.Proposals.ToListAsync();
            Assert.True(proposalsInDb.Count >= 2);

            var dbProposal1 = await _context.Proposals
                .FirstOrDefaultAsync(p => p.Libelle == proposal1.Libelle);
            var dbProposal2 = await _context.Proposals
                .FirstOrDefaultAsync(p => p.Libelle == proposal2.Libelle);

            Assert.NotNull(dbProposal1);
            Assert.NotNull(dbProposal2);
        }

        [Fact]
        public async Task AddProposal_ShouldGenerateUniqueId()
        {
            // Arrange
            var proposal1 = new ProposalVM
            {
                Libelle = "Première proposition unique test",
                Response = "100",
                Author = null,
                Source = null
            };

            var proposal2 = new ProposalVM
            {
                Libelle = "Deuxième proposition unique test",
                Response = "200",
                Author = null,
                Source = null
            };

            var content1 = new StringContent(
                JsonSerializer.Serialize(proposal1),
                Encoding.UTF8,
                "application/json"
            );

            var content2 = new StringContent(
                JsonSerializer.Serialize(proposal2),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            await _client.PostAsync("/api/Home/AddProposal", content1);
            await _client.PostAsync("/api/Home/AddProposal", content2);

            // Assert
            var dbProposal1 = await _context.Proposals
                .FirstOrDefaultAsync(p => p.Libelle == proposal1.Libelle);
            var dbProposal2 = await _context.Proposals
                .FirstOrDefaultAsync(p => p.Libelle == proposal2.Libelle);

            Assert.NotNull(dbProposal1);
            Assert.NotNull(dbProposal2);
            Assert.NotEqual(Guid.Empty, dbProposal1.Id);
            Assert.NotEqual(Guid.Empty, dbProposal2.Id);
            Assert.NotEqual(dbProposal1.Id, dbProposal2.Id);
        }

        [Fact]
        public async Task AddProposal_WithEmptyAuthor_ShouldStoreNullAuthor()
        {
            // Arrange
            var newProposal = new ProposalVM
            {
                Libelle = "Quel est le nombre de planètes dans le système solaire ?",
                Response = "8",
                Author = "",
                Source = "https://astronomy.com"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newProposal),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/Home/AddProposal", content);

            // Assert
            response.EnsureSuccessStatusCode();

            var proposalInDb = await _context.Proposals
                .FirstOrDefaultAsync(p => p.Libelle == newProposal.Libelle);

            Assert.NotNull(proposalInDb);
            Assert.Null(proposalInDb.Author);
        }

        [Fact]
        public async Task AddProposal_WithEmptySource_ShouldStoreNullSource()
        {
            // Arrange
            var newProposal = new ProposalVM
            {
                Libelle = "Quelle est la vitesse de la lumière ?",
                Response = "299792458",
                Author = "Physics Expert",
                Source = ""
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newProposal),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/Home/AddProposal", content);

            // Assert
            response.EnsureSuccessStatusCode();

            var proposalInDb = await _context.Proposals
                .FirstOrDefaultAsync(p => p.Libelle == newProposal.Libelle);

            Assert.NotNull(proposalInDb);
            Assert.Null(proposalInDb.Source);
        }

        #endregion
    }
}
