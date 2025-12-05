using game_domain_api.Events;
using game_domain_api.GameContext;
using game_domain_api.Repository;
using Moq;

namespace game_domain_api_tests
{
    public class GameEngineUnitTests
    {
        [Fact]
        public async Task CreateANewGame()
        {
            // Arrange
            var mockEventPub = new Mock<IEventPub>();
            var mockGameDataRepository = new Mock<IGameDataRepository>();
            IGameEngine sut = new GameEngine(mockEventPub.Object, mockGameDataRepository.Object);

            var playerId = Guid.NewGuid();
            
            // Act
            await sut.New(playerId);
            var gameData = sut.GetGameData();
            
            // Assert
            Assert.Equal(playerId, sut.CurrentPlayerId);
            Assert.True(sut.IsGameRunning);
            Assert.NotEqual(Guid.Empty, gameData.Id);
            Assert.Equal(GameStates.Playing, gameData.GameState);
            Assert.Equal(-1, gameData.FinishingFood);
            Assert.Equal(Accolades.Unknown, gameData.Accolade);
            Assert.Equal(playerId, gameData.PlayerId);
        }

        [Fact]
        public async Task LoadGame()
        {
            // Arrange
            var playerId = Guid.NewGuid();
            var expectedGameData = new GameData
            {
                Id = Guid.NewGuid(),
                GameState = GameStates.Won,
                FinishingFood = 2,
                Accolade = Accolades.Survivor,
                PlayerId = playerId
            };
            var mockEventPub = new Mock<IEventPub>();
            var mockGameDataRepository = new Mock<IGameDataRepository>();
            mockGameDataRepository.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedGameData);
            IGameEngine sut = new GameEngine(mockEventPub.Object, mockGameDataRepository.Object);
            
            // Act
            await sut.LoadGameById(playerId);
            var gameData = sut.GetGameData();
            
            // Assert
            Assert.Equal(playerId, sut.CurrentPlayerId);
            Assert.False(sut.IsGameRunning);
            Assert.Equal(expectedGameData.Id, gameData.Id);
            Assert.Equal(expectedGameData.GameState, gameData.GameState);
            Assert.Equal(expectedGameData.FinishingFood, gameData.FinishingFood);
            Assert.Equal(expectedGameData.Accolade, gameData.Accolade);
            Assert.Equal(expectedGameData.PlayerId, gameData.PlayerId);
        }


    }
}