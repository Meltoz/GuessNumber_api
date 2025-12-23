using Application.Exceptions;
using Application.Interfaces.Repository;
using Application.Services;
using Domain;
using Moq;

namespace UnitTests.Application
{
    public class ProposalServiceTests
    {
        private readonly Mock<IProposalRepository> _proposalRepositoryMock;
        private readonly ProposalService _service;

        public ProposalServiceTests()
        {
            _proposalRepositoryMock = new Mock<IProposalRepository>();
            _service = new ProposalService(_proposalRepositoryMock.Object);
        }

        #region GoToNext

        [Fact]
        public async Task GoToNext_WithNullId_ShouldNotCallDelete()
        {
            // Arrange
            var expectedProposal = new Proposal(
                "Quelle est la population de Paris en 2024 ?",
                "2161000",
                "https://fr.wikipedia.org/wiki/Paris",
                "Wikipedia"
            );

            _proposalRepositoryMock.Setup(r => r.GetNext())
                .ReturnsAsync(expectedProposal);

            // Act
            var result = await _service.GoToNext(null);

            // Assert
            _proposalRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
            _proposalRepositoryMock.Verify(r => r.Delete(It.IsAny<Guid>()), Times.Never);
            _proposalRepositoryMock.Verify(r => r.GetNext(), Times.Once);
        }

        [Fact]
        public async Task GoToNext_WithNullId_ShouldReturnNextProposal()
        {
            // Arrange
            var expectedProposal = new Proposal(
                "Combien de kilomètres mesure le tour de France ?",
                "3500",
                "https://www.letour.fr",
                "Tour de France"
            );

            _proposalRepositoryMock.Setup(r => r.GetNext())
                .ReturnsAsync(expectedProposal);

            // Act
            var result = await _service.GoToNext(null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedProposal.Libelle, result.Libelle);
            Assert.Equal(expectedProposal.Response, result.Response);
            Assert.Equal(expectedProposal.Source, result.Source);
            Assert.Equal(expectedProposal.Author, result.Author);
        }

        [Fact]
        public async Task GoToNext_WithValidId_ShouldReturnNextProposal()
        {
            // Arrange
            var idToDelete = Guid.NewGuid();
            var proposalToDelete = new Proposal(
                "Old proposal to delete",
                "100",
                null,
                null
            );
            var expectedProposal = new Proposal(
                "New proposal question",
                "200",
                "https://example.com",
                "Test Author"
            );

            _proposalRepositoryMock.Setup(r => r.GetByIdAsync(idToDelete))
                .ReturnsAsync(proposalToDelete);
            _proposalRepositoryMock.Setup(r => r.GetNext())
                .ReturnsAsync(expectedProposal);

            // Act
            var result = await _service.GoToNext(idToDelete);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedProposal.Libelle, result.Libelle);
            Assert.Equal(expectedProposal.Response, result.Response);
        }

        [Fact]
        public async Task GoToNext_ShouldCallGetNext()
        {
            // Arrange
            var expectedProposal = new Proposal(
                "Test question testing",
                "42",
                null,
                null
            );

            _proposalRepositoryMock.Setup(r => r.GetNext())
                .ReturnsAsync(expectedProposal);

            // Act
            await _service.GoToNext(null);

            // Assert
            _proposalRepositoryMock.Verify(r => r.GetNext(), Times.Once);
        }

        [Fact]
        public async Task GoToNext_WithMinimalFields_ShouldReturnProposal()
        {
            // Arrange
            var expectedProposal = new Proposal(
                "Minimal proposal question",
                "123",
                null, // No source
                null  // No author
            );

            _proposalRepositoryMock.Setup(r => r.GetNext())
                .ReturnsAsync(expectedProposal);

            // Act
            var result = await _service.GoToNext(null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedProposal.Libelle, result.Libelle);
            Assert.Equal(expectedProposal.Response, result.Response);
            Assert.Null(result.Source);
            Assert.Null(result.Author);
        }

        [Fact]
        public async Task GoToNext_WithAllFields_ShouldReturnProposal()
        {
            // Arrange
            var expectedProposal = new Proposal(
                "Complete proposal with all fields populated",
                "999",
                "https://complete-source.com",
                "Complete Author"
            );

            _proposalRepositoryMock.Setup(r => r.GetNext())
                .ReturnsAsync(expectedProposal);

            // Act
            var result = await _service.GoToNext(null);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedProposal.Libelle, result.Libelle);
            Assert.Equal(expectedProposal.Response, result.Response);
            Assert.Equal(expectedProposal.Source, result.Source);
            Assert.Equal(expectedProposal.Author, result.Author);
        }

        #endregion

        #region Delete

        [Fact]
        public async Task Delete_WithValidId_ShouldCallRepositoryGetByIdAsync()
        {
            // Arrange
            var proposalId = Guid.NewGuid();
            var proposal = new Proposal(
                "Proposal to delete",
                "42",
                null,
                null
            );

            _proposalRepositoryMock.Setup(r => r.GetByIdAsync(proposalId))
                .ReturnsAsync(proposal);

            // Act
            await _service.Delete(proposalId);

            // Assert
            _proposalRepositoryMock.Verify(r => r.GetByIdAsync(proposalId), Times.Once);
        }

        [Fact]
        public async Task Delete_WithValidId_ShouldCallRepositoryDelete()
        {
            // Arrange
            var proposalId = Guid.NewGuid();
            var proposal = new Proposal(
                "Proposal to delete",
                "100",
                "https://source.com",
                "Author"
            );

            _proposalRepositoryMock.Setup(r => r.GetByIdAsync(proposalId))
                .ReturnsAsync(proposal);

            // Act
            await _service.Delete(proposalId);

            // Assert
            _proposalRepositoryMock.Verify(r => r.Delete(proposalId), Times.Once);
        }

        [Fact]
        public async Task Delete_WithValidId_ShouldPassCorrectIdToRepository()
        {
            // Arrange
            var proposalId = Guid.NewGuid();
            var proposal = new Proposal(
                "Test proposal testing",
                "50",
                null,
                null
            );

            Guid capturedId = Guid.Empty;
            _proposalRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
                .Callback<Guid>(id => capturedId = id)
                .ReturnsAsync(proposal);

            // Act
            await _service.Delete(proposalId);

            // Assert
            Assert.Equal(proposalId, capturedId);
        }

        [Fact]
        public async Task Delete_WhenProposalNotFound_ShouldThrowEntityNotFoundException()
        {
            // Arrange
            var proposalId = Guid.NewGuid();
            _proposalRepositoryMock.Setup(r => r.GetByIdAsync(proposalId))
                .ReturnsAsync((Proposal?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
                () => _service.Delete(proposalId)
            );

            Assert.NotNull(exception);
            Assert.Contains(proposalId.ToString(), exception.Message);
        }

        [Fact]
        public async Task Delete_WhenProposalNotFound_ShouldNotCallRepositoryDelete()
        {
            // Arrange
            var proposalId = Guid.NewGuid();
            _proposalRepositoryMock.Setup(r => r.GetByIdAsync(proposalId))
                .ReturnsAsync((Proposal?)null);

            // Act
            try
            {
                await _service.Delete(proposalId);
            }
            catch (EntityNotFoundException)
            {
                // Expected exception
            }

            // Assert
            _proposalRepositoryMock.Verify(r => r.Delete(It.IsAny<Guid>()), Times.Never);
        }

        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000000")] // Guid.Empty
        [InlineData("12345678-1234-1234-1234-123456789012")]
        [InlineData("ffffffff-ffff-ffff-ffff-ffffffffffff")]
        public async Task Delete_WithDifferentGuidFormats_ShouldCallRepository(string guidString)
        {
            // Arrange
            var proposalId = Guid.Parse(guidString);
            var proposal = new Proposal("Test test test test", "10", null, null);

            _proposalRepositoryMock.Setup(r => r.GetByIdAsync(proposalId))
                .ReturnsAsync(proposal);

            // Act
            await _service.Delete(proposalId);

            // Assert
            _proposalRepositoryMock.Verify(r => r.GetByIdAsync(proposalId), Times.Once);
            _proposalRepositoryMock.Verify(r => r.Delete(proposalId), Times.Once);
        }

        [Fact]
        public async Task Delete_WithDifferentIds_ShouldCallRepositoryForEachId()
        {
            // Arrange
            var proposalId1 = Guid.NewGuid();
            var proposalId2 = Guid.NewGuid();
            var proposal1 = new Proposal("Proposal 1 testing test", "10", null, null);
            var proposal2 = new Proposal("Proposal 2 testing test", "20", null, null);

            _proposalRepositoryMock.Setup(r => r.GetByIdAsync(proposalId1))
                .ReturnsAsync(proposal1);
            _proposalRepositoryMock.Setup(r => r.GetByIdAsync(proposalId2))
                .ReturnsAsync(proposal2);

            // Act
            await _service.Delete(proposalId1);
            await _service.Delete(proposalId2);

            // Assert
            _proposalRepositoryMock.Verify(r => r.GetByIdAsync(proposalId1), Times.Once);
            _proposalRepositoryMock.Verify(r => r.GetByIdAsync(proposalId2), Times.Once);
            _proposalRepositoryMock.Verify(r => r.Delete(proposalId1), Times.Once);
            _proposalRepositoryMock.Verify(r => r.Delete(proposalId2), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldNotModifyProposalBeforeDelete()
        {
            // Arrange
            var proposalId = Guid.NewGuid();
            var proposal = new Proposal(
                "Original Proposal testing",
                "50",
                "https://original.com",
                "Original Author"
            );

            var originalLibelle = proposal.Libelle;
            var originalResponse = proposal.Response;

            _proposalRepositoryMock.Setup(r => r.GetByIdAsync(proposalId))
                .ReturnsAsync(proposal);

            // Act
            await _service.Delete(proposalId);

            // Assert
            Assert.Equal(originalLibelle, proposal.Libelle);
            Assert.Equal(originalResponse, proposal.Response);
            _proposalRepositoryMock.Verify(r => r.Delete(proposalId), Times.Once);
        }

        #endregion

        #region Count

        [Fact]
        public async Task Count_ShouldCallRepositoryCountProposal()
        {
            // Arrange
            _proposalRepositoryMock.Setup(r => r.CountProposal())
                .ReturnsAsync(5);

            // Act
            await _service.Count();

            // Assert
            _proposalRepositoryMock.Verify(r => r.CountProposal(), Times.Once);
        }

        [Fact]
        public async Task Count_ShouldReturnCorrectCount()
        {
            // Arrange
            var expectedCount = 10;
            _proposalRepositoryMock.Setup(r => r.CountProposal())
                .ReturnsAsync(expectedCount);

            // Act
            var result = await _service.Count();

            // Assert
            Assert.Equal(expectedCount, result);
        }

        [Fact]
        public async Task Count_WithNoProposals_ShouldReturnZero()
        {
            // Arrange
            _proposalRepositoryMock.Setup(r => r.CountProposal())
                .ReturnsAsync(0);

            // Act
            var result = await _service.Count();

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task Count_WithSingleProposal_ShouldReturnOne()
        {
            // Arrange
            _proposalRepositoryMock.Setup(r => r.CountProposal())
                .ReturnsAsync(1);

            // Act
            var result = await _service.Count();

            // Assert
            Assert.Equal(1, result);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(1000)]
        public async Task Count_WithDifferentCounts_ShouldReturnCorrectValue(int count)
        {
            // Arrange
            _proposalRepositoryMock.Setup(r => r.CountProposal())
                .ReturnsAsync(count);

            // Act
            var result = await _service.Count();

            // Assert
            Assert.Equal(count, result);
        }

        [Fact]
        public async Task Count_CalledMultipleTimes_ShouldCallRepositoryEachTime()
        {
            // Arrange
            _proposalRepositoryMock.Setup(r => r.CountProposal())
                .ReturnsAsync(5);

            // Act
            await _service.Count();
            await _service.Count();
            await _service.Count();

            // Assert
            _proposalRepositoryMock.Verify(r => r.CountProposal(), Times.Exactly(3));
        }

        [Fact]
        public async Task Count_CalledMultipleTimes_ShouldReturnConsistentResults()
        {
            // Arrange
            _proposalRepositoryMock.Setup(r => r.CountProposal())
                .ReturnsAsync(7);

            // Act
            var result1 = await _service.Count();
            var result2 = await _service.Count();
            var result3 = await _service.Count();

            // Assert
            Assert.Equal(7, result1);
            Assert.Equal(7, result2);
            Assert.Equal(7, result3);
        }

        #endregion

        #region AddProposal

        [Fact]
        public async Task AddProposal_WithValidData_ShouldCallRepositoryInsertAsync()
        {
            // Arrange
            var libelle = "Quelle est la population mondiale en 2024 ?";
            var response = "8000000000";
            var author = "World Bank";
            var source = "https://www.worldbank.org";

            // Act
            await _service.AddProposal(libelle, response, author, source);

            // Assert
            _proposalRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<Proposal>()), Times.Once);
        }

        [Fact]
        public async Task AddProposal_ShouldCreateProposalWithCorrectLibelle()
        {
            // Arrange
            var libelle = "Combien de jours compte une année bissextile ?";
            var response = "366";
            var author = "Test Author";
            var source = "https://test.com";

            Proposal? capturedProposal = null;
            _proposalRepositoryMock.Setup(r => r.InsertAsync(It.IsAny<Proposal>()))
                .Callback<Proposal>(p => capturedProposal = p)
                .ReturnsAsync((Proposal p) => p);

            // Act
            await _service.AddProposal(libelle, response, author, source);

            // Assert
            Assert.NotNull(capturedProposal);
            Assert.Equal(libelle, capturedProposal.Libelle);
        }

        [Fact]
        public async Task AddProposal_ShouldCreateProposalWithCorrectResponse()
        {
            // Arrange
            var libelle = "Quelle est la vitesse de la lumière en km/s ?";
            var response = "299792";
            var author = "Physics";
            var source = "https://physics.com";

            Proposal? capturedProposal = null;
            _proposalRepositoryMock.Setup(r => r.InsertAsync(It.IsAny<Proposal>()))
                .Callback<Proposal>(p => capturedProposal = p)
                .ReturnsAsync((Proposal p) => p);

            // Act
            await _service.AddProposal(libelle, response, author, source);

            // Assert
            Assert.NotNull(capturedProposal);
            Assert.Equal(response, capturedProposal.Response);
        }

        [Fact]
        public async Task AddProposal_WithNullAuthor_ShouldCreateProposal()
        {
            // Arrange
            var libelle = "Quelle est la température d'ébullition de l'eau ?";
            var response = "100";
            string? author = null;
            var source = "https://science.com";

            Proposal? capturedProposal = null;
            _proposalRepositoryMock.Setup(r => r.InsertAsync(It.IsAny<Proposal>()))
                .Callback<Proposal>(p => capturedProposal = p)
                .ReturnsAsync((Proposal p) => p);

            // Act
            await _service.AddProposal(libelle, response, author, source);

            // Assert
            Assert.NotNull(capturedProposal);
            Assert.Equal(libelle, capturedProposal.Libelle);
            Assert.Equal(response, capturedProposal.Response);
            Assert.Null(capturedProposal.Author);
        }

        [Fact]
        public async Task AddProposal_WithNullSource_ShouldCreateProposal()
        {
            // Arrange
            var libelle = "Combien y a-t-il de continents sur Terre ?";
            var response = "7";
            var author = "Geography Teacher";
            string? source = null;

            Proposal? capturedProposal = null;
            _proposalRepositoryMock.Setup(r => r.InsertAsync(It.IsAny<Proposal>()))
                .Callback<Proposal>(p => capturedProposal = p)
                .ReturnsAsync((Proposal p) => p);

            // Act
            await _service.AddProposal(libelle, response, author, source);

            // Assert
            Assert.NotNull(capturedProposal);
            Assert.Equal(libelle, capturedProposal.Libelle);
            Assert.Equal(response, capturedProposal.Response);
            Assert.Null(capturedProposal.Source);
        }

        [Fact]
        public async Task AddProposal_WithNullAuthorAndSource_ShouldCreateProposal()
        {
            // Arrange
            var libelle = "Quelle est la distance Terre-Lune en kilomètres ?";
            var response = "384400";
            string? author = null;
            string? source = null;

            Proposal? capturedProposal = null;
            _proposalRepositoryMock.Setup(r => r.InsertAsync(It.IsAny<Proposal>()))
                .Callback<Proposal>(p => capturedProposal = p)
                .ReturnsAsync((Proposal p) => p);

            // Act
            await _service.AddProposal(libelle, response, author, source);

            // Assert
            Assert.NotNull(capturedProposal);
            Assert.Equal(libelle, capturedProposal.Libelle);
            Assert.Equal(response, capturedProposal.Response);
            Assert.Null(capturedProposal.Author);
            Assert.Null(capturedProposal.Source);
        }

        [Fact]
        public async Task AddProposal_WithAllFields_ShouldCreateCompleteProposal()
        {
            // Arrange
            var libelle = "Quelle est la profondeur maximale de l'océan Pacifique ?";
            var response = "10911";
            var author = "Marine Research Institute";
            var source = "https://oceanography.com";

            Proposal? capturedProposal = null;
            _proposalRepositoryMock.Setup(r => r.InsertAsync(It.IsAny<Proposal>()))
                .Callback<Proposal>(p => capturedProposal = p)
                .ReturnsAsync((Proposal p) => p);

            // Act
            await _service.AddProposal(libelle, response, author, source);

            // Assert
            Assert.NotNull(capturedProposal);
            Assert.Equal(libelle, capturedProposal.Libelle);
            Assert.Equal(response, capturedProposal.Response);
            Assert.Equal(author, capturedProposal.Author);
            Assert.Equal(source, capturedProposal.Source);
        }

        [Fact]
        public async Task AddProposal_WithEmptyAuthor_ShouldCreateProposalWithNullAuthor()
        {
            // Arrange
            var libelle = "Quel est le nombre de planètes dans le système solaire ?";
            var response = "8";
            var author = "";
            var source = "https://astronomy.com";

            Proposal? capturedProposal = null;
            _proposalRepositoryMock.Setup(r => r.InsertAsync(It.IsAny<Proposal>()))
                .Callback<Proposal>(p => capturedProposal = p)
                .ReturnsAsync((Proposal p) => p);

            // Act
            await _service.AddProposal(libelle, response, author, source);

            // Assert
            Assert.NotNull(capturedProposal);
            Assert.Null(capturedProposal.Author);
        }

        [Fact]
        public async Task AddProposal_WithEmptySource_ShouldCreateProposalWithNullSource()
        {
            // Arrange
            var libelle = "Quelle est la capitale du Japon ?";
            var response = "13960000";
            var author = "Geography Expert";
            var source = "";

            Proposal? capturedProposal = null;
            _proposalRepositoryMock.Setup(r => r.InsertAsync(It.IsAny<Proposal>()))
                .Callback<Proposal>(p => capturedProposal = p)
                .ReturnsAsync((Proposal p) => p);

            // Act
            await _service.AddProposal(libelle, response, author, source);

            // Assert
            Assert.NotNull(capturedProposal);
            Assert.Null(capturedProposal.Source);
        }

        [Fact]
        public async Task AddProposal_CalledMultipleTimes_ShouldCallRepositoryEachTime()
        {
            // Arrange
            var libelle1 = "Quelle est la hauteur de la Tour Eiffel ?";
            var response1 = "330";
            var libelle2 = "Quelle est la population de Tokyo ?";
            var response2 = "14000000";

            // Act
            await _service.AddProposal(libelle1, response1, null, null);
            await _service.AddProposal(libelle2, response2, null, null);

            // Assert
            _proposalRepositoryMock.Verify(r => r.InsertAsync(It.IsAny<Proposal>()), Times.Exactly(2));
        }

        [Theory]
        [InlineData("0")]
        [InlineData("1")]
        [InlineData("999999")]
        [InlineData("2147483647")]
        public async Task AddProposal_WithDifferentResponseValues_ShouldCreateProposal(string response)
        {
            // Arrange
            var libelle = "Quelle est la réponse à cette question ?";
            var author = "Test";
            var source = "https://test.com";

            Proposal? capturedProposal = null;
            _proposalRepositoryMock.Setup(r => r.InsertAsync(It.IsAny<Proposal>()))
                .Callback<Proposal>(p => capturedProposal = p)
                .ReturnsAsync((Proposal p) => p);

            // Act
            await _service.AddProposal(libelle, response, author, source);

            // Assert
            Assert.NotNull(capturedProposal);
            Assert.Equal(response, capturedProposal.Response);
        }

        #endregion
    }
}
