using AutoMapper;
using Domain;
using Infrastructure;
using Infrastructure.Entities;
using Infrastructure.Repositories;
using Meltix.IntegrationTests;
using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Repository
{
    public class ProposalRepositoryTests
    {
        #region InsertAsync

        [Fact]
        public async Task InsertAsync_ShouldReturnProposal()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ProposalRepository(context, mapper);

            var proposal = new Proposal(
                "Quelle est la population de Paris en 2024 ?",
                "2161000",
                "https://fr.wikipedia.org/wiki/Paris",
                "Wikipedia"
            );

            // Act
            var result = await repository.InsertAsync(proposal);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal(proposal.Libelle, result.Libelle);
            Assert.Equal(proposal.Response, result.Response);
            Assert.Equal(proposal.Source, result.Source);
            Assert.Equal(proposal.Author, result.Author);
        }

        [Fact]
        public async Task InsertAsync_WithMinimalFields_ShouldReturnProposal()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ProposalRepository(context, mapper);

            var proposal = new Proposal(
                "Minimal proposal question",
                "123",
                null, // No source
                null  // No author
            );

            // Act
            var result = await repository.InsertAsync(proposal);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal(proposal.Libelle, result.Libelle);
            Assert.Equal(proposal.Response, result.Response);
            Assert.Null(result.Source);
            Assert.Null(result.Author);
        }

        [Fact]
        public async Task InsertAsync_ShouldSetCreatedAndUpdatedDates()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ProposalRepository(context, mapper);

            var beforeInsert = DateTime.UtcNow;

            var proposal = new Proposal(
                "Test proposal testing",
                "100",
                "https://test.com",
                "Test Author"
            );

            // Act
            var result = await repository.InsertAsync(proposal);
            var afterInsert = DateTime.UtcNow;

            // Assert
            var proposalEntity = await context.Proposals.FindAsync(result.Id);
            Assert.NotNull(proposalEntity);
            Assert.True(proposalEntity.Created >= beforeInsert && proposalEntity.Created <= afterInsert);
            Assert.True(proposalEntity.Updated >= beforeInsert && proposalEntity.Updated <= afterInsert);
            Assert.Equal(proposalEntity.Created, proposalEntity.Updated);
        }

        [Fact]
        public async Task InsertAsync_ShouldPersistToDatabase()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ProposalRepository(context, mapper);

            var proposal = new Proposal(
                "Persisted testing proposal",
                "999",
                "https://persisted.com",
                "Persisted Author"
            );

            // Act
            var result = await repository.InsertAsync(proposal);

            // Assert
            var retrievedProposal = await context.Proposals.FindAsync(result.Id);
            Assert.NotNull(retrievedProposal);
            Assert.Equal(proposal.Libelle, retrievedProposal.Libelle);
            Assert.Equal(proposal.Response, retrievedProposal.Response);
            Assert.Equal(proposal.Source, retrievedProposal.Source);
            Assert.Equal(proposal.Author, retrievedProposal.Author);
        }

        [Fact]
        public async Task InsertAsync_MultipleProposals_ShouldCreateMultipleRecords()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ProposalRepository(context, mapper);

            var proposal1 = new Proposal("Proposal testing test 1", "10", null, null);
            var proposal2 = new Proposal("Proposal testing test 2", "20", null, null);
            var proposal3 = new Proposal("Proposal testing test 3", "30", null, null);

            // Act
            var result1 = await repository.InsertAsync(proposal1);
            var result2 = await repository.InsertAsync(proposal2);
            var result3 = await repository.InsertAsync(proposal3);

            // Assert
            Assert.NotEqual(result1.Id, result2.Id);
            Assert.NotEqual(result1.Id, result3.Id);
            Assert.NotEqual(result2.Id, result3.Id);

            var proposalsCount = await context.Proposals.CountAsync();
            Assert.Equal(3, proposalsCount);
        }

        [Fact]
        public async Task InsertAsync_WithSpecialCharacters_ShouldStoreCorrectly()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ProposalRepository(context, mapper);

            var proposal = new Proposal(
                "Quelle est la température à l'équateur ?",
                "30",
                "https://météo-française.fr",
                "Auteur Ünîçödé"
            );

            // Act
            var result = await repository.InsertAsync(proposal);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(proposal.Libelle, result.Libelle);
            Assert.Equal(proposal.Author, result.Author);
            Assert.Equal(proposal.Source, result.Source);
        }

        [Theory]
        [InlineData("0")]
        [InlineData("999999")]
        [InlineData("2147483647")] // int.MaxValue
        public async Task InsertAsync_WithDifferentResponseValues_ShouldInsertCorrectly(string response)
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ProposalRepository(context, mapper);

            var proposal = new Proposal(
                $"Proposal with response {response}",
                response,
                null,
                null
            );

            // Act
            var result = await repository.InsertAsync(proposal);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(response, result.Response);
        }

        #endregion

        #region GetNext

        [Fact]
        public async Task GetNext_WithSingleProposal_ShouldReturnThatProposal()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ProposalRepository(context, mapper);

            var proposal = new Proposal(
                "Single testing proposal",
                "42",
                "https://single.com",
                "Single Author"
            );

            await repository.InsertAsync(proposal);

            // Act
            var result = await repository.GetNext();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(proposal.Libelle, result.Libelle);
            Assert.Equal(proposal.Response, result.Response);
        }

        [Fact]
        public async Task GetNext_WithMultipleProposals_ShouldReturnOldestProposal()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ProposalRepository(context, mapper);

            // Insert proposals with explicit Created dates
            var oldestProposal = new ProposalEntity
            {
                Id = Guid.NewGuid(),
                Libelle = "Oldest testing proposal",
                Response = "1",
                Created = DateTime.UtcNow.AddDays(-3),
                Updated = DateTime.UtcNow.AddDays(-3)
            };

            var middleProposal = new ProposalEntity
            {
                Id = Guid.NewGuid(),
                Libelle = "Middle testing proposal",
                Response = "2",
                Created = DateTime.UtcNow.AddDays(-2),
                Updated = DateTime.UtcNow.AddDays(-2)
            };

            var newestProposal = new ProposalEntity
            {
                Id = Guid.NewGuid(),
                Libelle = "Newest testing proposal",
                Response = "3",
                Created = DateTime.UtcNow.AddDays(-1),
                Updated = DateTime.UtcNow.AddDays(-1)
            };

            context.Proposals.Add(middleProposal);
            context.Proposals.Add(newestProposal);
            context.Proposals.Add(oldestProposal);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetNext();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(oldestProposal.Libelle, result.Libelle);
            Assert.Equal(oldestProposal.Response, result.Response);
        }

        [Fact]
        public async Task GetNext_WithNoProposals_ShouldReturnNull()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ProposalRepository(context, mapper);

            // Act
            var result = await repository.GetNext();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetNext_ShouldNotTrackEntity()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ProposalRepository(context, mapper);

            var proposal = new Proposal("Test proposal testing", "42", null, null);
            await repository.InsertAsync(proposal);

            // Clear the ChangeTracker to ensure the entity from InsertAsync is not tracked
            context.ChangeTracker.Clear();

            // Act
            var result = await repository.GetNext();

            // Assert
            Assert.NotNull(result);
            var tracked = context.ChangeTracker.Entries<ProposalEntity>()
                .Any(e => e.Entity.Id == result.Id && e.State != EntityState.Detached);
            Assert.False(tracked, "Entity should not be tracked after GetNext");
        }

        [Fact]
        public async Task GetNext_CalledMultipleTimes_ShouldReturnSameProposal()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ProposalRepository(context, mapper);

            var proposal = new Proposal("Test proposal testing", "42", null, null);
            await repository.InsertAsync(proposal);

            // Act
            var result1 = await repository.GetNext();
            var result2 = await repository.GetNext();

            // Assert
            Assert.NotNull(result1);
            Assert.NotNull(result2);
            Assert.Equal(result1.Libelle, result2.Libelle);
            Assert.Equal(result1.Response, result2.Response);
        }

        [Fact]
        public async Task GetNext_WithMinimalFields_ShouldReturnProposal()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ProposalRepository(context, mapper);

            var proposal = new Proposal("Minimal testing proposal", "123", null, null);
            await repository.InsertAsync(proposal);

            // Act
            var result = await repository.GetNext();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(proposal.Libelle, result.Libelle);
            Assert.Null(result.Source);
            Assert.Null(result.Author);
        }

        [Fact]
        public async Task GetNext_AfterDelete_ShouldReturnNextOldestProposal()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ProposalRepository(context, mapper);

            var proposal1 = new ProposalEntity
            {
                Id = Guid.NewGuid(),
                Libelle = "First proposal testing",
                Response = "1",
                Created = DateTime.UtcNow.AddDays(-2),
                Updated = DateTime.UtcNow.AddDays(-2)
            };

            var proposal2 = new ProposalEntity
            {
                Id = Guid.NewGuid(),
                Libelle = "Second proposal testing",
                Response = "2",
                Created = DateTime.UtcNow.AddDays(-1),
                Updated = DateTime.UtcNow.AddDays(-1)
            };

            context.Proposals.Add(proposal1);
            context.Proposals.Add(proposal2);
            await context.SaveChangesAsync();

            // Delete first proposal
            repository.Delete(proposal1.Id);

            // Act
            var result = await repository.GetNext();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(proposal2.Libelle, result.Libelle);
        }

        #endregion

        #region GetByIdAsync

        [Fact]
        public async Task GetByIdAsync_WithValidId_ShouldReturnProposal()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ProposalRepository(context, mapper);

            var proposal = new Proposal(
                "Test proposal testing",
                "42",
                "https://test.com",
                "Test Author"
            );

            var inserted = await repository.InsertAsync(proposal);

            // Act
            var result = await repository.GetByIdAsync(inserted.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(inserted.Id, result.Id);
            Assert.Equal(inserted.Libelle, result.Libelle);
            Assert.Equal(inserted.Response, result.Response);
            Assert.Equal(inserted.Source, result.Source);
            Assert.Equal(inserted.Author, result.Author);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistentId_ShouldReturnNull()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ProposalRepository(context, mapper);

            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await repository.GetByIdAsync(nonExistentId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldNotTrackEntity()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ProposalRepository(context, mapper);

            var proposal = new Proposal("Test proposal testing", "42", null, null);
            var inserted = await repository.InsertAsync(proposal);

            // Clear the ChangeTracker to ensure the entity from InsertAsync is not tracked
            context.ChangeTracker.Clear();

            // Act
            var result = await repository.GetByIdAsync(inserted.Id);

            // Assert
            Assert.NotNull(result);
            var tracked = context.ChangeTracker.Entries<ProposalEntity>()
                .Any(e => e.Entity.Id == result.Id && e.State != EntityState.Detached);
            Assert.False(tracked, "Entity should not be tracked after GetByIdAsync");
        }

        #endregion

        #region Delete

        [Fact]
        public async Task Delete_WithValidId_ShouldRemoveProposal()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ProposalRepository(context, mapper);

            var proposal = new Proposal("To delete testing", "42", null, null);
            var inserted = await repository.InsertAsync(proposal);

            // Act
            repository.Delete(inserted.Id);

            // Assert
            var deletedProposal = await context.Proposals.FindAsync(inserted.Id);
            Assert.Null(deletedProposal);
        }

        [Fact]
        public async Task Delete_WithValidId_ShouldPersistDeletion()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ProposalRepository(context, mapper);

            var proposal = new Proposal("To delete testing", "42", null, null);
            var inserted = await repository.InsertAsync(proposal);
            var proposalId = inserted.Id;

            // Act
            repository.Delete(proposalId);

            // Assert
            var count = await context.Proposals.CountAsync();
            Assert.Equal(0, count);
        }

        [Fact]
        public async Task Delete_MultipleProposals_ShouldOnlyDeleteSpecified()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ProposalRepository(context, mapper);

            var proposal1 = new Proposal("Proposal testing test 1", "10", null, null);
            var proposal2 = new Proposal("Proposal testing test 2", "20", null, null);
            var proposal3 = new Proposal("Proposal testing test 3", "30", null, null);

            var inserted1 = await repository.InsertAsync(proposal1);
            var inserted2 = await repository.InsertAsync(proposal2);
            var inserted3 = await repository.InsertAsync(proposal3);

            // Act
            repository.Delete(inserted2.Id);

            // Assert
            var remainingProposals = await context.Proposals.ToListAsync();
            Assert.Equal(2, remainingProposals.Count);
            Assert.Contains(remainingProposals, p => p.Id == inserted1.Id);
            Assert.Contains(remainingProposals, p => p.Id == inserted3.Id);
            Assert.DoesNotContain(remainingProposals, p => p.Id == inserted2.Id);
        }

        #endregion

        #region GetAllAsync

        [Fact]
        public async Task GetAllAsync_WithNoProposals_ShouldReturnEmptyList()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ProposalRepository(context, mapper);

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAsync_WithMultipleProposals_ShouldReturnAll()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ProposalRepository(context, mapper);

            var proposal1 = new Proposal("Proposal testing test 1", "10", null, null);
            var proposal2 = new Proposal("Proposal testing test 2", "20", null, null);
            var proposal3 = new Proposal("Proposal testing test 3", "30", null, null);

            await repository.InsertAsync(proposal1);
            await repository.InsertAsync(proposal2);
            await repository.InsertAsync(proposal3);

            // Act
            var result = await repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        #endregion

        #region CountProposal

        [Fact]
        public async Task CountProposal_WithNoProposals_ShouldReturnZero()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ProposalRepository(context, mapper);

            // Act
            var result = await repository.CountProposal();

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task CountProposal_WithSingleProposal_ShouldReturnOne()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ProposalRepository(context, mapper);

            var proposal = new Proposal("Test proposal testing", "42", null, null);
            await repository.InsertAsync(proposal);

            // Act
            var result = await repository.CountProposal();

            // Assert
            Assert.Equal(1, result);
        }

        [Fact]
        public async Task CountProposal_WithMultipleProposals_ShouldReturnCorrectCount()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ProposalRepository(context, mapper);

            var proposal1 = new Proposal("Proposal number one test", "10", null, null);
            var proposal2 = new Proposal("Proposal number two test", "20", null, null);
            var proposal3 = new Proposal("Proposal number three test", "30", null, null);

            await repository.InsertAsync(proposal1);
            await repository.InsertAsync(proposal2);
            await repository.InsertAsync(proposal3);

            // Act
            var result = await repository.CountProposal();

            // Assert
            Assert.Equal(3, result);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(25)]
        public async Task CountProposal_WithDifferentCounts_ShouldReturnCorrectValue(int expectedCount)
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ProposalRepository(context, mapper);

            for (int i = 0; i < expectedCount; i++)
            {
                var proposal = new Proposal($"Proposal number testing {i}", $"{i}", null, null);
                await repository.InsertAsync(proposal);
            }

            // Act
            var result = await repository.CountProposal();

            // Assert
            Assert.Equal(expectedCount, result);
        }

        [Fact]
        public async Task CountProposal_AfterInsert_ShouldIncrement()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ProposalRepository(context, mapper);

            var initialCount = await repository.CountProposal();

            var proposal = new Proposal("New proposal test question", "100", null, null);
            await repository.InsertAsync(proposal);

            // Act
            var newCount = await repository.CountProposal();

            // Assert
            Assert.Equal(initialCount + 1, newCount);
        }

        [Fact]
        public async Task CountProposal_AfterDelete_ShouldDecrement()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ProposalRepository(context, mapper);

            var proposal1 = new Proposal("Proposal number one test", "10", null, null);
            var proposal2 = new Proposal("Proposal number two test", "20", null, null);
            var proposal3 = new Proposal("Proposal number three test", "30", null, null);

            var inserted1 = await repository.InsertAsync(proposal1);
            await repository.InsertAsync(proposal2);
            await repository.InsertAsync(proposal3);

            var countBefore = await repository.CountProposal();

            // Act
            repository.Delete(inserted1.Id);
            var countAfter = await repository.CountProposal();

            // Assert
            Assert.Equal(3, countBefore);
            Assert.Equal(2, countAfter);
        }

        [Fact]
        public async Task CountProposal_AfterMultipleDeletes_ShouldReturnCorrectCount()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ProposalRepository(context, mapper);

            var proposal1 = new Proposal("Proposal number one test", "10", null, null);
            var proposal2 = new Proposal("Proposal number two test", "20", null, null);
            var proposal3 = new Proposal("Proposal number three test", "30", null, null);
            var proposal4 = new Proposal("Proposal number four test", "40", null, null);
            var proposal5 = new Proposal("Proposal number five test", "50", null, null);

            var inserted1 = await repository.InsertAsync(proposal1);
            var inserted2 = await repository.InsertAsync(proposal2);
            await repository.InsertAsync(proposal3);
            await repository.InsertAsync(proposal4);
            await repository.InsertAsync(proposal5);

            // Delete 2 proposals
            repository.Delete(inserted1.Id);
            repository.Delete(inserted2.Id);

            // Act
            var result = await repository.CountProposal();

            // Assert
            Assert.Equal(3, result);
        }

        [Fact]
        public async Task CountProposal_WithMinimalFields_ShouldCountCorrectly()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ProposalRepository(context, mapper);

            var proposal1 = new Proposal("Minimal proposal one test", "10", null, null);
            var proposal2 = new Proposal("Minimal proposal two test", "20", null, null);

            await repository.InsertAsync(proposal1);
            await repository.InsertAsync(proposal2);

            // Act
            var result = await repository.CountProposal();

            // Assert
            Assert.Equal(2, result);
        }

        [Fact]
        public async Task CountProposal_WithAllFields_ShouldCountCorrectly()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ProposalRepository(context, mapper);

            var proposal1 = new Proposal("Complete proposal one test", "10", "https://source1.com", "Author One");
            var proposal2 = new Proposal("Complete proposal two test", "20", "https://source2.com", "Author Two");
            var proposal3 = new Proposal("Complete proposal three test", "30", "https://source3.com", "Author Three");

            await repository.InsertAsync(proposal1);
            await repository.InsertAsync(proposal2);
            await repository.InsertAsync(proposal3);

            // Act
            var result = await repository.CountProposal();

            // Assert
            Assert.Equal(3, result);
        }

        [Fact]
        public async Task CountProposal_CalledMultipleTimes_ShouldReturnConsistentResult()
        {
            // Arrange
            var context = DbContextProvider.SetupContext();
            var mapper = MapperProvider.SetupMapper();
            var repository = new ProposalRepository(context, mapper);

            var proposal1 = new Proposal("Proposal one test question", "10", null, null);
            var proposal2 = new Proposal("Proposal two test question", "20", null, null);

            await repository.InsertAsync(proposal1);
            await repository.InsertAsync(proposal2);

            // Act
            var result1 = await repository.CountProposal();
            var result2 = await repository.CountProposal();
            var result3 = await repository.CountProposal();

            // Assert
            Assert.Equal(2, result1);
            Assert.Equal(2, result2);
            Assert.Equal(2, result3);
        }

        #endregion
    }
}
