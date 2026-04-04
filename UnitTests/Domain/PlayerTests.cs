using Domain.Enums;
using Domain.Exceptions;
using Domain.Party;

namespace UnitTests.Domain
{
    public class PlayerTests
    {
        #region Constructor Tests - Cas Nominaux

        [Fact]
        public void Constructor_WithPlayerRole_ShouldCreatePlayer()
        {
            // Arrange
            var (userId, pseudo, avatar) = CreateValidUserData();
            var role = RoleParty.Player;
            var connectionId = "conn-123";

            // Act
            var player = new Player(userId, pseudo, avatar, role, connectionId);

            // Assert
            Assert.Equal(role, player.Role);
            Assert.Equal(connectionId, player.ConnectionId);
            Assert.Equal(0, player.Score);
            Assert.Equal(userId, player.UserId);
            Assert.Equal(pseudo, player.Pseudo);
            Assert.Equal(avatar, player.Avatar);
        }

        [Fact]
        public void Constructor_WithOwnerRole_ShouldCreatePlayerWithOwnerRole()
        {
            // Arrange
            var (userId, pseudo, avatar) = CreateValidUserData();
            var role = RoleParty.Owner;
            var connectionId = "conn-owner";

            // Act
            var player = new Player(userId, pseudo, avatar, role, connectionId);

            // Assert
            Assert.Equal(RoleParty.Owner, player.Role);
            Assert.Equal(connectionId, player.ConnectionId);
        }

        [Fact]
        public void Constructor_ShouldInitializeScoreToZero()
        {
            // Arrange & Act
            var player = CreateValidPlayer();

            // Assert
            Assert.Equal(0, player.Score);
        }

        [Fact]
        public void Constructor_ShouldStoreUserData()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var pseudo = "PlayerOne";
            var avatar = "avatar.png";

            // Act
            var player = new Player(userId, pseudo, avatar, RoleParty.Player, "conn-123");

            // Assert
            Assert.Equal(userId, player.UserId);
            Assert.Equal(pseudo, player.Pseudo);
            Assert.Equal(avatar, player.Avatar);
        }

        #endregion

        #region Constructor Tests - Cas Limites et Erreurs

        [Fact]
        public void Constructor_WithNullConnectionId_ShouldThrowArgumentNullException()
        {
            // Arrange
            var (userId, pseudo, avatar) = CreateValidUserData();
            string connectionId = null;

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new Player(userId, pseudo, avatar, RoleParty.Player, connectionId));
        }

        [Fact]
        public void Constructor_WithEmptyConnectionId_ShouldThrowArgumentNullException()
        {
            // Arrange
            var (userId, pseudo, avatar) = CreateValidUserData();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new Player(userId, pseudo, avatar, RoleParty.Player, ""));
        }

        [Fact]
        public void Constructor_WithWhitespaceConnectionId_ShouldThrowArgumentNullException()
        {
            // Arrange
            var (userId, pseudo, avatar) = CreateValidUserData();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new Player(userId, pseudo, avatar, RoleParty.Player, "   "));
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

        private static (Guid userId, string pseudo, string avatar) CreateValidUserData() =>
            (Guid.NewGuid(), "TestUser", "avatar.png");

        private Player CreateValidPlayer()
        {
            var (userId, pseudo, avatar) = CreateValidUserData();
            return new Player(userId, pseudo, avatar, RoleParty.Player, "conn-123");
        }

        #endregion
    }
}
