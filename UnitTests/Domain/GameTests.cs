using Domain.Enums;
using Domain.Party;
using Domain.User;

namespace UnitTests.Domain
{
    public class GameTests
    {
        #region CreateNew Tests - Cas Nominaux

        [Fact]
        public void CreateNew_ShouldCreateGameWithDefaultValues()
        {
            // Act
            var game = Game.CreateNew();

            // Assert
            Assert.Equal(GameStatus.Creating, game.Status);
            Assert.Equal(GameType.Private, game.Type);
            Assert.Equal(20, game.TotalQuestion);
            Assert.Equal(8, game.MaxPlayers);
            Assert.Equal(0, game.CurrentQuestion);
            Assert.Empty(game.Players);
        }

        [Fact]
        public void CreateNew_ShouldGenerateNonEmptyCode()
        {
            // Act
            var game = Game.CreateNew();

            // Assert
            Assert.NotNull(game.Code);
            Assert.False(string.IsNullOrWhiteSpace(game.Code.Value));
        }

        #endregion

        #region Constructor Tests - Cas Nominaux

        [Fact]
        public void Constructor_WithValidStringCode_ShouldCreateGame()
        {
            // Arrange
            var code = "ABCDEFGHIJ";

            // Act
            var game = new Game(code, GameStatus.Creating, GameType.Private, 10, 4);

            // Assert
            Assert.Equal(code, game.Code.Value);
            Assert.Equal(GameStatus.Creating, game.Status);
            Assert.Equal(GameType.Private, game.Type);
            Assert.Equal(10, game.TotalQuestion);
            Assert.Equal(4, game.MaxPlayers);
            Assert.Equal(0, game.CurrentQuestion);
        }

        [Fact]
        public void Constructor_WithId_ShouldSetId()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var game = new Game(id, "ABCDEFGHIJ", GameStatus.Playing, GameType.Public, 15, 6);

            // Assert
            Assert.Equal(id, game.Id);
        }

        [Fact]
        public void Constructor_WithEmptyId_ShouldKeepEmptyId()
        {
            // Act
            var game = new Game(Guid.Empty, "ABCDEFGHIJ", GameStatus.Creating, GameType.Private, 10, 4);

            // Assert
            Assert.Equal(Guid.Empty, game.Id);
        }

        #endregion

        #region NextQuestion Tests - Cas Nominaux

        [Fact]
        public void NextQuestion_ShouldIncrementCurrentQuestion()
        {
            // Arrange
            var game = new Game("ABCDEFGHIJ", GameStatus.Playing, GameType.Private, 10, 4);

            // Act
            game.NextQuestion();

            // Assert
            Assert.Equal(1, game.CurrentQuestion);
        }

        [Fact]
        public void NextQuestion_AtLastQuestion_ShouldReachTotal()
        {
            // Arrange
            var game = new Game("ABCDEFGHIJ", GameStatus.Playing, GameType.Private, 2, 4);
            game.NextQuestion();

            // Act
            game.NextQuestion();

            // Assert
            Assert.Equal(2, game.CurrentQuestion);
        }

        #endregion

        #region NextQuestion Tests - Cas Limites et Erreurs

        [Fact]
        public void NextQuestion_WhenExceedsTotalQuestion_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var game = new Game("ABCDEFGHIJ", GameStatus.Playing, GameType.Private, 1, 4);
            game.NextQuestion();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => game.NextQuestion());
        }

        #endregion

        #region ChangeTotalQuestion Tests - Cas Nominaux

        [Fact]
        public void ChangeTotalQuestion_WithValidValue_ShouldUpdate()
        {
            // Arrange
            var game = CreateValidGame();

            // Act
            game.ChangeTotalQuestion(30);

            // Assert
            Assert.Equal(30, game.TotalQuestion);
        }

        [Fact]
        public void ChangeTotalQuestion_WithZero_ShouldUpdate()
        {
            // Arrange
            var game = CreateValidGame();

            // Act
            game.ChangeTotalQuestion(0);

            // Assert
            Assert.Equal(0, game.TotalQuestion);
        }

        [Fact]
        public void ChangeTotalQuestion_WithMaxValue50_ShouldUpdate()
        {
            // Arrange
            var game = CreateValidGame();

            // Act
            game.ChangeTotalQuestion(50);

            // Assert
            Assert.Equal(50, game.TotalQuestion);
        }

        #endregion

        #region ChangeTotalQuestion Tests - Cas Limites et Erreurs

        [Fact]
        public void ChangeTotalQuestion_WithNegativeValue_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var game = CreateValidGame();

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => game.ChangeTotalQuestion(-1));
        }

        [Fact]
        public void ChangeTotalQuestion_WithValueOver50_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var game = CreateValidGame();

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => game.ChangeTotalQuestion(51));
        }

        #endregion

        #region ChangeMaxPlayers Tests - Cas Nominaux

        [Fact]
        public void ChangeMaxPlayers_WithValidValue_ShouldUpdate()
        {
            // Arrange
            var game = CreateValidGame();

            // Act
            game.ChangeMaxPlayers(6);

            // Assert
            Assert.Equal(6, game.MaxPlayers);
        }

        [Fact]
        public void ChangeMaxPlayers_WithMax10_ShouldUpdate()
        {
            // Arrange
            var game = CreateValidGame();

            // Act
            game.ChangeMaxPlayers(10);

            // Assert
            Assert.Equal(10, game.MaxPlayers);
        }

        #endregion

        #region ChangeMaxPlayers Tests - Cas Limites et Erreurs

        [Fact]
        public void ChangeMaxPlayers_WithValueOver10_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var game = CreateValidGame();

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => game.ChangeMaxPlayers(11));
        }

        #endregion

        #region IsJoinable Tests

        [Fact]
        public void IsJoinable_WhenNoPlayers_ShouldReturnTrue()
        {
            // Arrange
            var game = CreateValidGame();

            // Act & Assert
            Assert.True(game.IsJoinable());
        }

        [Fact]
        public void IsJoinable_WhenFull_ShouldReturnFalse()
        {
            // Arrange
            var game = new Game("ABCDEFGHIJ", GameStatus.Creating, GameType.Private, 10, 2);
            game.AddPlayer(CreateValidUser(), "conn-1");
            game.AddPlayer(CreateValidUser(), "conn-2");

            // Act & Assert
            Assert.False(game.IsJoinable());
        }

        [Fact]
        public void IsJoinable_WhenBelowMax_ShouldReturnTrue()
        {
            // Arrange
            var game = new Game("ABCDEFGHIJ", GameStatus.Creating, GameType.Private, 10, 4);
            game.AddPlayer(CreateValidUser(), "conn-1");

            // Act & Assert
            Assert.True(game.IsJoinable());
        }

        #endregion

        #region AddPlayer Tests - Cas Nominaux

        [Fact]
        public void AddPlayer_ShouldAddPlayerToCollection()
        {
            // Arrange
            var game = CreateValidGame();
            var user = CreateValidUser();

            // Act
            game.AddPlayer(user, "conn-1");

            // Assert
            Assert.Single(game.Players);
        }

        [Fact]
        public void AddPlayer_DefaultRole_ShouldBePlayer()
        {
            // Arrange
            var game = CreateValidGame();

            // Act
            game.AddPlayer(CreateValidUser(), "conn-1");

            // Assert
            Assert.Equal(RoleParty.Player, game.Players.First().Role);
        }

        [Fact]
        public void AddPlayer_WithOwnerRole_ShouldAddOwner()
        {
            // Arrange
            var game = CreateValidGame();

            // Act
            game.AddPlayer(CreateValidUser(), "conn-1", RoleParty.Owner);

            // Assert
            Assert.Equal(RoleParty.Owner, game.Players.First().Role);
        }

        [Fact]
        public void AddPlayer_ShouldAssociateUserToPlayer()
        {
            // Arrange
            var game = CreateValidGame();
            var user = CreateValidUser();

            // Act
            game.AddPlayer(user, "conn-1");

            // Assert
            Assert.Equal(user, game.Players.First().User);
        }

        [Fact]
        public void AddPlayer_MultipleTimesUpToMax_ShouldSucceed()
        {
            // Arrange
            var game = new Game("ABCDEFGHIJ", GameStatus.Creating, GameType.Private, 10, 3);

            // Act
            game.AddPlayer(CreateValidUser(), "conn-1");
            game.AddPlayer(CreateValidUser(), "conn-2");
            game.AddPlayer(CreateValidUser(), "conn-3");

            // Assert
            Assert.Equal(3, game.Players.Count);
        }

        #endregion

        #region AddPlayer Tests - Cas Limites et Erreurs

        [Fact]
        public void AddPlayer_WhenFull_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var game = new Game("ABCDEFGHIJ", GameStatus.Creating, GameType.Private, 10, 1);
            game.AddPlayer(CreateValidUser(), "conn-1");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => game.AddPlayer(CreateValidUser(), "conn-2"));
        }

        #endregion

        #region Helper Methods

        private static Game CreateValidGame() =>
            new Game("ABCDEFGHIJ", GameStatus.Creating, GameType.Private, 10, 4);

        private static GuestUser CreateValidUser() =>
            new GuestUser("TestUser", "avatar.png");

        #endregion
    }
}
