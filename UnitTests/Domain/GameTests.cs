using Domain.Enums;
using Domain.Party;

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
            Assert.Equal(20, game.Settings.TotalQuestion);
            Assert.Equal(8, game.Settings.MaxPlayers);
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
            Assert.Equal(10, game.Settings.TotalQuestion);
            Assert.Equal(4, game.Settings.MaxPlayers);
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

        #region UpdateSettings Tests - Cas Nominaux

        [Fact]
        public void UpdateSettings_ChangeTotalQuestion_WithValidValue_ShouldUpdate()
        {
            // Arrange
            var game = CreateValidGame();

            // Act
            game.UpdateSettings(s => s.ChangeTotalQuestion(30));

            // Assert
            Assert.Equal(30, game.Settings.TotalQuestion);
        }

        [Fact]
        public void UpdateSettings_ChangeTotalQuestion_WithZero_ShouldUpdate()
        {
            // Arrange
            var game = CreateValidGame();

            // Act
            game.UpdateSettings(s => s.ChangeTotalQuestion(0));

            // Assert
            Assert.Equal(0, game.Settings.TotalQuestion);
        }

        [Fact]
        public void UpdateSettings_ChangeTotalQuestion_WithMaxValue50_ShouldUpdate()
        {
            // Arrange
            var game = CreateValidGame();

            // Act
            game.UpdateSettings(s => s.ChangeTotalQuestion(50));

            // Assert
            Assert.Equal(50, game.Settings.TotalQuestion);
        }

        [Fact]
        public void UpdateSettings_ChangeMaxPlayers_WithValidValue_ShouldUpdate()
        {
            // Arrange
            var game = CreateValidGame();

            // Act
            game.UpdateSettings(s => s.ChangeMaxPlayers(6));

            // Assert
            Assert.Equal(6, game.Settings.MaxPlayers);
        }

        [Fact]
        public void UpdateSettings_ChangeMaxPlayers_WithMax10_ShouldUpdate()
        {
            // Arrange
            var game = CreateValidGame();

            // Act
            game.UpdateSettings(s => s.ChangeMaxPlayers(10));

            // Assert
            Assert.Equal(10, game.Settings.MaxPlayers);
        }

        [Fact]
        public void UpdateSettings_MultipleChanges_ShouldUpdateAll()
        {
            // Arrange
            var game = CreateValidGame();

            // Act
            game.UpdateSettings(s =>
            {
                s.ChangeMaxPlayers(6);
                s.ChangeTotalQuestion(30);
            });

            // Assert
            Assert.Equal(6, game.Settings.MaxPlayers);
            Assert.Equal(30, game.Settings.TotalQuestion);
        }

        #endregion

        #region UpdateSettings Tests - Cas Limites et Erreurs

        [Fact]
        public void UpdateSettings_ChangeTotalQuestion_WithNegativeValue_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var game = CreateValidGame();

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => game.UpdateSettings(s => s.ChangeTotalQuestion(-1)));
        }

        [Fact]
        public void UpdateSettings_ChangeTotalQuestion_WithValueOver50_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var game = CreateValidGame();

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => game.UpdateSettings(s => s.ChangeTotalQuestion(51)));
        }

        [Fact]
        public void UpdateSettings_ChangeMaxPlayers_WithValueOver10_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var game = CreateValidGame();

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => game.UpdateSettings(s => s.ChangeMaxPlayers(11)));
        }

        [Fact]
        public void UpdateSettings_WhenGameIsPlaying_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var game = new Game("ABCDEFGHIJ", GameStatus.Playing, GameType.Private, 10, 4);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => game.UpdateSettings(s => s.ChangeMaxPlayers(6)));
        }

        [Fact]
        public void UpdateSettings_WhenGameIsFinished_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var game = new Game("ABCDEFGHIJ", GameStatus.Finished, GameType.Private, 10, 4);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => game.UpdateSettings(s => s.ChangeMaxPlayers(6)));
        }

        [Fact]
        public void UpdateSettings_WhenGameIsCancelled_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var game = new Game("ABCDEFGHIJ", GameStatus.Cancelled, GameType.Private, 10, 4);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => game.UpdateSettings(s => s.ChangeMaxPlayers(6)));
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
            game.AddPlayer(Guid.NewGuid(), "User1", "avatar.png", "conn-1");
            game.AddPlayer(Guid.NewGuid(), "User2", "avatar.png", "conn-2");

            // Act & Assert
            Assert.False(game.IsJoinable());
        }

        [Fact]
        public void IsJoinable_WhenBelowMax_ShouldReturnTrue()
        {
            // Arrange
            var game = new Game("ABCDEFGHIJ", GameStatus.Creating, GameType.Private, 10, 4);
            game.AddPlayer(Guid.NewGuid(), "User1", "avatar.png", "conn-1");

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
            var (userId, pseudo, avatar) = CreateValidUserData();

            // Act
            game.AddPlayer(userId, pseudo, avatar, "conn-1");

            // Assert
            Assert.Single(game.Players);
        }

        [Fact]
        public void AddPlayer_DefaultRole_ShouldBePlayer()
        {
            // Arrange
            var game = CreateValidGame();

            // Act
            game.AddPlayer(Guid.NewGuid(), "TestUser", "avatar.png", "conn-1");

            // Assert
            Assert.Equal(RoleParty.Player, game.Players.First().Role);
        }

        [Fact]
        public void AddPlayer_WithOwnerRole_ShouldAddOwner()
        {
            // Arrange
            var game = CreateValidGame();

            // Act
            game.AddPlayer(Guid.NewGuid(), "TestUser", "avatar.png", "conn-1", RoleParty.Owner);

            // Assert
            Assert.Equal(RoleParty.Owner, game.Players.First().Role);
        }

        [Fact]
        public void AddPlayer_ShouldStoreUserData()
        {
            // Arrange
            var game = CreateValidGame();
            var userId = Guid.NewGuid();
            var pseudo = "TestUser";
            var avatar = "avatar.png";

            // Act
            game.AddPlayer(userId, pseudo, avatar, "conn-1");

            // Assert
            Assert.Equal(userId, game.Players.First().UserId);
            Assert.Equal(pseudo, game.Players.First().Pseudo);
            Assert.Equal(avatar, game.Players.First().Avatar);
        }

        [Fact]
        public void AddPlayer_MultipleTimesUpToMax_ShouldSucceed()
        {
            // Arrange
            var game = new Game("ABCDEFGHIJ", GameStatus.Creating, GameType.Private, 10, 3);

            // Act
            game.AddPlayer(Guid.NewGuid(), "User1", "avatar.png", "conn-1");
            game.AddPlayer(Guid.NewGuid(), "User2", "avatar.png", "conn-2");
            game.AddPlayer(Guid.NewGuid(), "User3", "avatar.png", "conn-3");

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
            game.AddPlayer(Guid.NewGuid(), "User1", "avatar.png", "conn-1");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => game.AddPlayer(Guid.NewGuid(), "User2", "avatar.png", "conn-2"));
        }

        #endregion

        #region Helper Methods

        private static Game CreateValidGame() =>
            new Game("ABCDEFGHIJ", GameStatus.Creating, GameType.Private, 10, 4);

        private static (Guid userId, string pseudo, string avatar) CreateValidUserData() =>
            (Guid.NewGuid(), "TestUser", "avatar.png");

        #endregion
    }
}
