using Domain.Enums;
using Domain.Exceptions;
using Domain.Party;
using Domain.User;

namespace UnitTests.Domain
{
    public class PlayerTests
    {
        #region Constructor Tests - Cas Nominaux

        [Fact]
        public void Constructor_WithPlayerRole_ShouldCreatePlayer()
        {
            // Arrange
            var user = CreateValidUser();
            var role = RoleParty.Player;
            var connectionId = "conn-123";

            // Act
            var player = new Player(user, role, connectionId);

            // Assert
            Assert.Equal(role, player.Role);
            Assert.Equal(connectionId, player.ConnectionId);
            Assert.Equal(0, player.Score);
            Assert.Equal(user, player.User);
        }

        [Fact]
        public void Constructor_WithOwnerRole_ShouldCreatePlayerWithOwnerRole()
        {
            // Arrange
            var user = CreateValidUser();
            var role = RoleParty.Owner;
            var connectionId = "conn-owner";

            // Act
            var player = new Player(user, role, connectionId);

            // Assert
            Assert.Equal(RoleParty.Owner, player.Role);
            Assert.Equal(connectionId, player.ConnectionId);
        }

        [Fact]
        public void Constructor_ShouldInitializeScoreToZero()
        {
            // Arrange & Act
            var player = new Player(CreateValidUser(), RoleParty.Player, "conn-abc");

            // Assert
            Assert.Equal(0, player.Score);
        }

        [Fact]
        public void Constructor_ShouldAssociateUser()
        {
            // Arrange
            var user = new GuestUser("PlayerOne", "avatar.png");

            // Act
            var player = new Player(user, RoleParty.Player, "conn-123");

            // Assert
            Assert.Equal(user, player.User);
        }

        #endregion

        #region Constructor Tests - Cas Limites et Erreurs

        [Fact]
        public void Constructor_WithNullConnectionId_ShouldThrowArgumentNullException()
        {
            // Arrange
            var role = RoleParty.Player;
            string connectionId = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new Player(CreateValidUser(), role, connectionId));
        }

        [Fact]
        public void Constructor_WithEmptyConnectionId_ShouldThrowArgumentNullException()
        {
            // Arrange
            var role = RoleParty.Player;
            var connectionId = "";

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new Player(CreateValidUser(), role, connectionId));
        }

        [Fact]
        public void Constructor_WithWhitespaceConnectionId_ShouldThrowArgumentNullException()
        {
            // Arrange
            var role = RoleParty.Player;
            var connectionId = "   ";

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new Player(CreateValidUser(), role, connectionId));
        }

        #endregion

        #region ChangeScore Tests - Cas Nominaux

        [Fact]
        public void ChangeScore_WithHigherScore_ShouldUpdateScore()
        {
            // Arrange
            var player = CreateValidPlayer();
            var newScore = 10;

            // Act
            player.ChangeScore(newScore);

            // Assert
            Assert.Equal(newScore, player.Score);
        }

        [Fact]
        public void ChangeScore_WithSameScore_ShouldKeepScore()
        {
            // Arrange
            var player = CreateValidPlayer();
            player.ChangeScore(5);

            // Act
            player.ChangeScore(5);

            // Assert
            Assert.Equal(5, player.Score);
        }

        [Fact]
        public void ChangeScore_MultipleIncreases_ShouldUpdateScoreEachTime()
        {
            // Arrange
            var player = CreateValidPlayer();

            // Act
            player.ChangeScore(10);
            player.ChangeScore(20);
            player.ChangeScore(50);

            // Assert
            Assert.Equal(50, player.Score);
        }

        #endregion

        #region ChangeScore Tests - Cas Limites et Erreurs

        [Fact]
        public void ChangeScore_WithLowerScore_ShouldThrowInvalidScoreUpdateException()
        {
            // Arrange
            var player = CreateValidPlayer();
            player.ChangeScore(10);

            // Act & Assert
            Assert.Throws<InvalidScoreUpdateException>(() => player.ChangeScore(5));
        }

        [Fact]
        public void ChangeScore_WithNegativeScore_FromZero_ShouldThrowInvalidScoreUpdateException()
        {
            // Arrange
            var player = CreateValidPlayer();

            // Act & Assert
            Assert.Throws<InvalidScoreUpdateException>(() => player.ChangeScore(-1));
        }

        [Fact]
        public void ChangeScore_WithLowerScore_ExceptionMessageShouldContainBothScores()
        {
            // Arrange
            var player = CreateValidPlayer();
            player.ChangeScore(10);

            // Act & Assert
            var exception = Assert.Throws<InvalidScoreUpdateException>(() => player.ChangeScore(3));
            Assert.Contains("3", exception.Message);
            Assert.Contains("10", exception.Message);
        }

        #endregion

        #region Helper Methods

        private static GuestUser CreateValidUser() => new GuestUser("TestUser", "avatar.png");

        private Player CreateValidPlayer() => new Player(CreateValidUser(), RoleParty.Player, "conn-123");

        #endregion
    }
}
