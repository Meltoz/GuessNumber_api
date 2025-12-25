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

        #region AddReport

        [Fact]
        public async Task AddReport_WithValidData_ShouldReturnOk()
        {
            // Arrange
            var newReport = new ReportVM
            {
                Type = "Bug",
                Context = "Site",
                Explanation = "Test bug explanation",
                Mail = "test@example.com"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newReport),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/Home/AddReport", content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task AddReport_WithValidData_ShouldInsertReportInDatabase()
        {
            // Arrange
            var newReport = new ReportVM
            {
                Type = "Bug",
                Context = "Site",
                Explanation = "There is a critical bug in the login system",
                Mail = "reporter@example.com"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newReport),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/Home/AddReport", content);

            // Assert
            response.EnsureSuccessStatusCode();

            var reportInDb = await _context.Reports
                .FirstOrDefaultAsync(r => r.Explanation == newReport.Explanation);

            Assert.NotNull(reportInDb);
            Assert.Equal(newReport.Explanation, reportInDb.Explanation);
            Assert.Equal(newReport.Mail, reportInDb.Mail);
        }

        [Fact]
        public async Task AddReport_WithNullMail_ShouldReturnOk()
        {
            // Arrange
            var newReport = new ReportVM
            {
                Type = "Improuvment",
                Context = "Question",
                Explanation = "Suggestion for improvement without email",
                Mail = null
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newReport),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/Home/AddReport", content);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task AddReport_WithNullMail_ShouldInsertReportInDatabase()
        {
            // Arrange
            var newReport = new ReportVM
            {
                Type = "Bug",
                Context = "Question",
                Explanation = "Report without email address provided",
                Mail = null
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newReport),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/Home/AddReport", content);

            // Assert
            response.EnsureSuccessStatusCode();

            var reportInDb = await _context.Reports
                .FirstOrDefaultAsync(r => r.Explanation == newReport.Explanation);

            Assert.NotNull(reportInDb);
            Assert.Equal(newReport.Explanation, reportInDb.Explanation);
            Assert.Null(reportInDb.Mail);
        }

        [Fact]
        public async Task AddReport_WithNullReport_ShouldReturnBadRequest()
        {
            // Arrange
            var content = new StringContent(
                "null",
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/Home/AddReport", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task AddReport_WithInvalidType_ShouldReturnInternalServerError()
        {
            // Arrange
            var newReport = new ReportVM
            {
                Type = "InvalidType",
                Context = "Site",
                Explanation = "Test explanation",
                Mail = "test@example.com"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newReport),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/Home/AddReport", content);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task AddReport_WithInvalidContext_ShouldReturnInternalServerError()
        {
            // Arrange
            var newReport = new ReportVM
            {
                Type = "Bug",
                Context = "InvalidContext",
                Explanation = "Test explanation",
                Mail = "test@example.com"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newReport),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/Home/AddReport", content);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task AddReport_WithInvalidExplanationTooShort_ShouldReturnInternalServerError()
        {
            // Arrange
            var newReport = new ReportVM
            {
                Type = "Bug",
                Context = "Site",
                Explanation = "Short",
                Mail = "test@example.com"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newReport),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/Home/AddReport", content);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task AddReport_WithInvalidMail_ShouldReturnInternalServerError()
        {
            // Arrange
            var newReport = new ReportVM
            {
                Type = "Bug",
                Context = "Site",
                Explanation = "Test explanation for invalid mail",
                Mail = "invalid-email"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newReport),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/Home/AddReport", content);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Theory]
        [InlineData("Bug", "Site")]
        [InlineData("Bug", "Question")]
        [InlineData("Improuvment", "Site")]
        [InlineData("Improuvment", "Question")]
        public async Task AddReport_WithDifferentTypesAndContexts_ShouldReturnOk(string type, string context)
        {
            // Arrange
            var newReport = new ReportVM
            {
                Type = type,
                Context = context,
                Explanation = $"Test explanation for {type} in {context}",
                Mail = "test@example.com"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newReport),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/Home/AddReport", content);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("Bug", "Site")]
        [InlineData("Bug", "Question")]
        [InlineData("Improuvment", "Site")]
        [InlineData("Improuvment", "Question")]
        public async Task AddReport_WithDifferentTypesAndContexts_ShouldInsertCorrectly(string type, string context)
        {
            // Arrange
            var explanation = $"Detailed report for {type} and {context} combination";
            var newReport = new ReportVM
            {
                Type = type,
                Context = context,
                Explanation = explanation,
                Mail = "combo@example.com"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newReport),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/Home/AddReport", content);

            // Assert
            response.EnsureSuccessStatusCode();

            var reportInDb = await _context.Reports
                .FirstOrDefaultAsync(r => r.Explanation == explanation);

            Assert.NotNull(reportInDb);
            Assert.Equal(explanation, reportInDb.Explanation);
        }

        [Fact]
        public async Task AddReport_WithCaseInsensitiveType_ShouldReturnOk()
        {
            // Arrange
            var newReport = new ReportVM
            {
                Type = "bug", // lowercase
                Context = "Site",
                Explanation = "Test with lowercase type",
                Mail = "test@example.com"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newReport),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/Home/AddReport", content);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task AddReport_WithCaseInsensitiveContext_ShouldReturnOk()
        {
            // Arrange
            var newReport = new ReportVM
            {
                Type = "Bug",
                Context = "site", // lowercase
                Explanation = "Test with lowercase context",
                Mail = "test@example.com"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newReport),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/Home/AddReport", content);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task AddReport_CalledMultipleTimes_ShouldInsertAllReports()
        {
            // Arrange
            var report1 = new ReportVM
            {
                Type = "Bug",
                Context = "Site",
                Explanation = "First report about login issue",
                Mail = "user1@example.com"
            };

            var report2 = new ReportVM
            {
                Type = "Improuvment",
                Context = "Question",
                Explanation = "Second report suggesting new feature",
                Mail = "user2@example.com"
            };

            var content1 = new StringContent(
                JsonSerializer.Serialize(report1),
                Encoding.UTF8,
                "application/json"
            );

            var content2 = new StringContent(
                JsonSerializer.Serialize(report2),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response1 = await _client.PostAsync("/api/Home/AddReport", content1);
            var response2 = await _client.PostAsync("/api/Home/AddReport", content2);

            // Assert
            response1.EnsureSuccessStatusCode();
            response2.EnsureSuccessStatusCode();

            var dbReport1 = await _context.Reports
                .FirstOrDefaultAsync(r => r.Explanation == report1.Explanation);
            var dbReport2 = await _context.Reports
                .FirstOrDefaultAsync(r => r.Explanation == report2.Explanation);

            Assert.NotNull(dbReport1);
            Assert.NotNull(dbReport2);
        }

        [Fact]
        public async Task AddReport_ShouldGenerateUniqueId()
        {
            // Arrange
            var report1 = new ReportVM
            {
                Type = "Bug",
                Context = "Site",
                Explanation = "First unique report test",
                Mail = null
            };

            var report2 = new ReportVM
            {
                Type = "Bug",
                Context = "Site",
                Explanation = "Second unique report test",
                Mail = null
            };

            var content1 = new StringContent(
                JsonSerializer.Serialize(report1),
                Encoding.UTF8,
                "application/json"
            );

            var content2 = new StringContent(
                JsonSerializer.Serialize(report2),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            await _client.PostAsync("/api/Home/AddReport", content1);
            await _client.PostAsync("/api/Home/AddReport", content2);

            // Assert
            var dbReport1 = await _context.Reports
                .FirstOrDefaultAsync(r => r.Explanation == report1.Explanation);
            var dbReport2 = await _context.Reports
                .FirstOrDefaultAsync(r => r.Explanation == report2.Explanation);

            Assert.NotNull(dbReport1);
            Assert.NotNull(dbReport2);
            Assert.NotEqual(Guid.Empty, dbReport1.Id);
            Assert.NotEqual(Guid.Empty, dbReport2.Id);
            Assert.NotEqual(dbReport1.Id, dbReport2.Id);
        }

        [Fact]
        public async Task AddReport_WithEmptyMail_ShouldStoreNullMail()
        {
            // Arrange
            var newReport = new ReportVM
            {
                Type = "Bug",
                Context = "Site",
                Explanation = "Report with empty mail string",
                Mail = ""
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newReport),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/Home/AddReport", content);

            // Assert
            response.EnsureSuccessStatusCode();

            var reportInDb = await _context.Reports
                .FirstOrDefaultAsync(r => r.Explanation == newReport.Explanation);

            Assert.NotNull(reportInDb);
            Assert.Null(reportInDb.Mail);
        }

        [Fact]
        public async Task AddReport_WithValidMail_ShouldStoreMailCorrectly()
        {
            // Arrange
            var email = "valid.email@example.com";
            var newReport = new ReportVM
            {
                Type = "Improuvment",
                Context = "Question",
                Explanation = "Report with valid email address",
                Mail = email
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newReport),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/Home/AddReport", content);

            // Assert
            response.EnsureSuccessStatusCode();

            var reportInDb = await _context.Reports
                .FirstOrDefaultAsync(r => r.Explanation == newReport.Explanation);

            Assert.NotNull(reportInDb);
            Assert.Equal(email, reportInDb.Mail);
        }

        [Fact]
        public async Task AddReport_WithLongExplanation_ShouldReturnOk()
        {
            // Arrange
            var longExplanation = "This is a very detailed explanation about the bug or improvement. " +
                                  "It contains multiple sentences and provides comprehensive information. " +
                                  "The user has taken the time to describe the issue thoroughly, which is very helpful.";
            var newReport = new ReportVM
            {
                Type = "Bug",
                Context = "Site",
                Explanation = longExplanation,
                Mail = "detailed@example.com"
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newReport),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/Home/AddReport", content);

            // Assert
            response.EnsureSuccessStatusCode();

            var reportInDb = await _context.Reports
                .FirstOrDefaultAsync(r => r.Explanation == longExplanation);

            Assert.NotNull(reportInDb);
            Assert.Equal(longExplanation, reportInDb.Explanation);
        }

        [Theory]
        [InlineData("test@example.com")]
        [InlineData("user.name@domain.co.uk")]
        [InlineData("firstname.lastname@company.org")]
        [InlineData("valid_email@test-domain.com")]
        public async Task AddReport_WithDifferentValidEmails_ShouldReturnOk(string email)
        {
            // Arrange
            var newReport = new ReportVM
            {
                Type = "Bug",
                Context = "Site",
                Explanation = $"Test report with email {email}",
                Mail = email
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newReport),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/Home/AddReport", content);

            // Assert
            response.EnsureSuccessStatusCode();
        }

        [Theory]
        [InlineData("invalid")]
        [InlineData("@example.com")]
        [InlineData("user@")]
        [InlineData("user space@example.com")]
        [InlineData("user@@example.com")]
        public async Task AddReport_WithInvalidEmails_ShouldReturnInternalServerError(string email)
        {
            // Arrange
            var newReport = new ReportVM
            {
                Type = "Bug",
                Context = "Site",
                Explanation = $"Test report with invalid email {email}",
                Mail = email
            };

            var content = new StringContent(
                JsonSerializer.Serialize(newReport),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/Home/AddReport", content);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        #endregion

        #region GetActualities

        [Fact]
        public async Task GetActualities_WithActiveActualities_ReturnsOkWithActualities()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var actuality1 = new Infrastructure.Entities.ActualityEntity
            {
                Title = "Active Actuality 1",
                Content = "Content of actuality 1",
                StartPublish = now.AddDays(-5),
                EndPublish = now.AddDays(5)
            };
            var actuality2 = new Infrastructure.Entities.ActualityEntity
            {
                Title = "Active Actuality 2",
                Content = "Content of actuality 2",
                StartPublish = now.AddDays(-2),
                EndPublish = null
            };

            _context.Actualities.AddRange(actuality1, actuality2);
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/Home/GetActualities");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
            var actualities = JsonSerializer.Deserialize<List<ActualityVM>>(responseContent, _jsonOptions);

            Assert.NotNull(actualities);
            Assert.Equal(2, actualities.Count);
        }

        [Fact]
        public async Task GetActualities_WithNoActiveActualities_ReturnsNotFound()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var futureActuality = new Infrastructure.Entities.ActualityEntity
            {
                Title = "Future Actuality",
                Content = "Content",
                StartPublish = now.AddDays(5),
                EndPublish = now.AddDays(10)
            };
            var expiredActuality = new Infrastructure.Entities.ActualityEntity
            {
                Title = "Expired Actuality",
                Content = "Content",
                StartPublish = now.AddDays(-10),
                EndPublish = now.AddDays(-1)
            };

            _context.Actualities.AddRange(futureActuality, expiredActuality);
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/Home/GetActualities");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetActualities_WithNoActualities_ReturnsNotFound()
        {
            // Arrange
            // No actualities added to context

            // Act
            var response = await _client.GetAsync("/api/Home/GetActualities");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetActualities_ReturnsCorrectlyFormattedDates()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var actuality = new Infrastructure.Entities.ActualityEntity
            {
                Title = "Test Actuality",
                Content = "Test Content",
                StartPublish = new DateTime(2025, 1, 15, 10, 30, 0, DateTimeKind.Utc),
                EndPublish = new DateTime(2025, 12, 31, 23, 59, 59, DateTimeKind.Utc)
            };

            _context.Actualities.Add(actuality);
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/Home/GetActualities");

            // Assert
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var actualities = JsonSerializer.Deserialize<List<ActualityVM>>(responseContent, _jsonOptions);

            Assert.NotNull(actualities);
            Assert.Single(actualities);
        }

        [Fact]
        public async Task GetActualities_WithActualityWithoutEndDate_ReturnsNullEndDate()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var actuality = new Infrastructure.Entities.ActualityEntity
            {
                Title = "No End Date",
                Content = "Content without end date",
                StartPublish = now.AddDays(-5),
                EndPublish = null
            };

            _context.Actualities.Add(actuality);
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/Home/GetActualities");

            // Assert
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var actualities = JsonSerializer.Deserialize<List<ActualityVM>>(responseContent, _jsonOptions);

            Assert.NotNull(actualities);
            Assert.Single(actualities);
            Assert.Equal("No End Date", actualities[0].Title);
        }

        [Fact]
        public async Task GetActualities_ReturnsOnlyActiveActualities_NotExpiredOrFuture()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var activeActuality = new Infrastructure.Entities.ActualityEntity
            {
                Title = "Active",
                Content = "Active content",
                StartPublish = now.AddDays(-5),
                EndPublish = now.AddDays(5)
            };
            var futureActuality = new Infrastructure.Entities.ActualityEntity
            {
                Title = "Future",
                Content = "Future content",
                StartPublish = now.AddDays(2),
                EndPublish = now.AddDays(10)
            };
            var expiredActuality = new Infrastructure.Entities.ActualityEntity
            {
                Title = "Expired",
                Content = "Expired content",
                StartPublish = now.AddDays(-10),
                EndPublish = now.AddDays(-1)
            };

            _context.Actualities.AddRange(activeActuality, futureActuality, expiredActuality);
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/Home/GetActualities");

            // Assert
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var actualities = JsonSerializer.Deserialize<List<ActualityVM>>(responseContent, _jsonOptions);

            Assert.NotNull(actualities);
            Assert.Single(actualities);
            Assert.Equal("Active", actualities[0].Title);
        }

        [Fact]
        public async Task GetActualities_WithMultipleActiveActualities_ReturnsAllActive()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var actuality1 = new Infrastructure.Entities.ActualityEntity
            {
                Title = "Active 1",
                Content = "Content 1",
                StartPublish = now.AddDays(-10),
                EndPublish = now.AddDays(5)
            };
            var actuality2 = new Infrastructure.Entities.ActualityEntity
            {
                Title = "Active 2",
                Content = "Content 2",
                StartPublish = now.AddDays(-5),
                EndPublish = now.AddDays(10)
            };
            var actuality3 = new Infrastructure.Entities.ActualityEntity
            {
                Title = "Active 3",
                Content = "Content 3",
                StartPublish = now.AddDays(-1),
                EndPublish = null
            };

            _context.Actualities.AddRange(actuality1, actuality2, actuality3);
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/Home/GetActualities");

            // Assert
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var actualities = JsonSerializer.Deserialize<List<ActualityVM>>(responseContent, _jsonOptions);

            Assert.NotNull(actualities);
            Assert.Equal(3, actualities.Count);

            var titles = actualities.Select(a => a.Title).ToList();
            Assert.Contains("Active 1", titles);
            Assert.Contains("Active 2", titles);
            Assert.Contains("Active 3", titles);
        }

        [Fact]
        public async Task GetActualities_ReturnsCorrectMappedProperties()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var actuality = new Infrastructure.Entities.ActualityEntity
            {
                Title = "Test Title",
                Content = "Test Content Value",
                StartPublish = now.AddDays(-5),
                EndPublish = now.AddDays(5)
            };

            _context.Actualities.Add(actuality);
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/Home/GetActualities");

            // Assert
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var actualities = JsonSerializer.Deserialize<List<ActualityVM>>(responseContent, _jsonOptions);

            Assert.NotNull(actualities);
            Assert.Single(actualities);
            Assert.Equal("Test Title", actualities[0].Title);
            Assert.Equal("Test Content Value", actualities[0].Content);
        }

        #endregion

        #region GetCommunications

        [Fact]
        public async Task GetCommunications_WithActiveCommunications_ReturnsOkWithCommunicationContents()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var comm1 = new Infrastructure.Entities.CommunicationEntity
            {
                Content = "First active communication",
                Start = now.AddDays(-5),
                End = now.AddDays(5)
            };
            var comm2 = new Infrastructure.Entities.CommunicationEntity
            {
                Content = "Second active communication",
                Start = now.AddDays(-2),
                End = null
            };

            _context.Communications.AddRange(comm1, comm2);
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/Home/GetCommunications");

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseContent = await response.Content.ReadAsStringAsync();
            var communications = JsonSerializer.Deserialize<List<string>>(responseContent, _jsonOptions);

            Assert.NotNull(communications);
            Assert.Equal(2, communications.Count);
            Assert.Contains("First active communication", communications);
            Assert.Contains("Second active communication", communications);
        }

        [Fact]
        public async Task GetCommunications_WithNoActiveCommunications_ReturnsNoContent()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var futureComm = new Infrastructure.Entities.CommunicationEntity
            {
                Content = "Future communication",
                Start = now.AddDays(5),
                End = now.AddDays(10)
            };
            var expiredComm = new Infrastructure.Entities.CommunicationEntity
            {
                Content = "Expired communication",
                Start = now.AddDays(-10),
                End = now.AddDays(-1)
            };

            _context.Communications.AddRange(futureComm, expiredComm);
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/Home/GetCommunications");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task GetCommunications_WithNoCommunications_ReturnsNoContent()
        {
            // Arrange
            // No communications added to context

            // Act
            var response = await _client.GetAsync("/api/Home/GetCommunications");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task GetCommunications_ReturnsOnlyContentStrings()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var comm = new Infrastructure.Entities.CommunicationEntity
            {
                Content = "Test communication content",
                Start = now.AddDays(-1),
                End = now.AddDays(1)
            };

            _context.Communications.Add(comm);
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/Home/GetCommunications");

            // Assert
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var communications = JsonSerializer.Deserialize<List<string>>(responseContent, _jsonOptions);

            Assert.NotNull(communications);
            Assert.Single(communications);
            Assert.Equal("Test communication content", communications[0]);
        }

        [Fact]
        public async Task GetCommunications_WithMultipleActiveCommunications_ReturnsAllContents()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var comm1 = new Infrastructure.Entities.CommunicationEntity
            {
                Content = "Communication 1",
                Start = now.AddDays(-10),
                End = now.AddDays(5)
            };
            var comm2 = new Infrastructure.Entities.CommunicationEntity
            {
                Content = "Communication 2",
                Start = now.AddDays(-5),
                End = now.AddDays(10)
            };
            var comm3 = new Infrastructure.Entities.CommunicationEntity
            {
                Content = "Communication 3",
                Start = now.AddDays(-1),
                End = null
            };

            _context.Communications.AddRange(comm1, comm2, comm3);
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/Home/GetCommunications");

            // Assert
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var communications = JsonSerializer.Deserialize<List<string>>(responseContent, _jsonOptions);

            Assert.NotNull(communications);
            Assert.Equal(3, communications.Count);

            Assert.Contains("Communication 1", communications);
            Assert.Contains("Communication 2", communications);
            Assert.Contains("Communication 3", communications);
        }

        [Fact]
        public async Task GetCommunications_ReturnsOnlyActiveCommunications_NotExpiredOrFuture()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var activeComm = new Infrastructure.Entities.CommunicationEntity
            {
                Content = "Active",
                Start = now.AddDays(-5),
                End = now.AddDays(5)
            };
            var futureComm = new Infrastructure.Entities.CommunicationEntity
            {
                Content = "Future",
                Start = now.AddDays(2),
                End = now.AddDays(10)
            };
            var expiredComm = new Infrastructure.Entities.CommunicationEntity
            {
                Content = "Expired",
                Start = now.AddDays(-10),
                End = now.AddDays(-1)
            };

            _context.Communications.AddRange(activeComm, futureComm, expiredComm);
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/Home/GetCommunications");

            // Assert
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var communications = JsonSerializer.Deserialize<List<string>>(responseContent, _jsonOptions);

            Assert.NotNull(communications);
            Assert.Single(communications);
            Assert.Equal("Active", communications[0]);
        }

        [Fact]
        public async Task GetCommunications_WithCommunicationWithNullEnd_ReturnsItIfStarted()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var comm = new Infrastructure.Entities.CommunicationEntity
            {
                Content = "No end date communication",
                Start = now.AddDays(-5),
                End = null
            };

            _context.Communications.Add(comm);
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/Home/GetCommunications");

            // Assert
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var communications = JsonSerializer.Deserialize<List<string>>(responseContent, _jsonOptions);

            Assert.NotNull(communications);
            Assert.Single(communications);
            Assert.Equal("No end date communication", communications[0]);
        }

        [Fact]
        public async Task GetCommunications_WithCommunicationStartingNow_ReturnsIt()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var comm = new Infrastructure.Entities.CommunicationEntity
            {
                Content = "Starting now",
                Start = now,
                End = now.AddDays(5)
            };

            _context.Communications.Add(comm);
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/Home/GetCommunications");

            // Assert
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var communications = JsonSerializer.Deserialize<List<string>>(responseContent, _jsonOptions);

            Assert.NotNull(communications);
            Assert.Single(communications);
            Assert.Equal("Starting now", communications[0]);
        }

        [Fact]
        public async Task GetCommunications_WithCommunicationEndingNow_DoesNotReturnIt()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var comm = new Infrastructure.Entities.CommunicationEntity
            {
                Content = "Ending now",
                Start = now.AddDays(-5),
                End = now
            };

            _context.Communications.Add(comm);
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/Home/GetCommunications");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task GetCommunications_WithMixedActiveFutureAndExpired_ReturnsOnlyActiveContents()
        {
            // Arrange
            var now = DateTime.UtcNow;

            var active1 = new Infrastructure.Entities.CommunicationEntity
            {
                Content = "Active 1",
                Start = now.AddDays(-5),
                End = now.AddDays(5)
            };

            var active2 = new Infrastructure.Entities.CommunicationEntity
            {
                Content = "Active 2",
                Start = now.AddDays(-1),
                End = null
            };

            var future = new Infrastructure.Entities.CommunicationEntity
            {
                Content = "Future",
                Start = now.AddDays(1),
                End = now.AddDays(10)
            };

            var expired = new Infrastructure.Entities.CommunicationEntity
            {
                Content = "Expired",
                Start = now.AddDays(-10),
                End = now.AddDays(-2)
            };

            _context.Communications.AddRange(active1, active2, future, expired);
            await _context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/Home/GetCommunications");

            // Assert
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            var communications = JsonSerializer.Deserialize<List<string>>(responseContent, _jsonOptions);

            Assert.NotNull(communications);
            Assert.Equal(2, communications.Count);
            Assert.Contains("Active 1", communications);
            Assert.Contains("Active 2", communications);
        }

        #endregion
    }
}
