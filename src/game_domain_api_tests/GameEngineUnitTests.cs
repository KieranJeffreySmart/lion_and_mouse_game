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
            Assert.Equal(GameStates.Started, gameData.GameState);
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
        
        [Fact]
        public async Task LoseAGameWithNoFood()
        {
            // Arrange
            var playerId = Guid.NewGuid();
            var mockEventPub = new Mock<IEventPub>();
            var mockGameDataRepository = new Mock<IGameDataRepository>();
            IGameEngine sut = new GameEngine(mockEventPub.Object, mockGameDataRepository.Object);
            await sut.New(playerId);
        
            // Act
            sut.GameOver(0);
        
            // Assert
            var gameData = sut.GetGameData();
            Assert.Equal(playerId, sut.CurrentPlayerId);
            Assert.False(sut.IsGameRunning);
            Assert.NotEqual(Guid.Empty, gameData.Id);
            Assert.Equal(GameStates.Lost, gameData.GameState);
            Assert.Equal(0, gameData.FinishingFood);
            Assert.Equal(Accolades.None, gameData.Accolade);
            Assert.Equal(playerId, gameData.PlayerId);
        }
        
        [Fact]
        public async Task LoseAGameWithSomeFood()
        {
            // Arrange
            var playerId = Guid.NewGuid();
            var mockEventPub = new Mock<IEventPub>();
            var mockGameDataRepository = new Mock<IGameDataRepository>();
            IGameEngine sut = new GameEngine(mockEventPub.Object, mockGameDataRepository.Object);
            await sut.New(playerId);
        
            // Act
            sut.GameOver(2);
        
            // Assert
            var gameData = sut.GetGameData();
            Assert.Equal(playerId, sut.CurrentPlayerId);
            Assert.False(sut.IsGameRunning);
            Assert.NotEqual(Guid.Empty, gameData.Id);
            Assert.Equal(GameStates.Lost, gameData.GameState);
            Assert.Equal(2, gameData.FinishingFood);
            Assert.Equal(Accolades.None, gameData.Accolade);
            Assert.Equal(playerId, gameData.PlayerId);
        }

        [Theory]
        [InlineData(-1, Accolades.Desperado)]
        [InlineData(0, Accolades.Desperado)]
        [InlineData(4, Accolades.Desperado)]
        [InlineData(5, Accolades.Survivor)]
        [InlineData(9, Accolades.Survivor)]
        [InlineData(10, Accolades.Hoarder)]
        [InlineData(15, Accolades.Hoarder)]
        public async Task WinAGameWithDifferentFoodAmounts(int finishingFood, Accolades expectedAccolade)
        {
            // Arrange
            var playerId = Guid.NewGuid();
            var mockEventPub = new Mock<IEventPub>();
            var mockGameDataRepository = new Mock<IGameDataRepository>();
            IGameEngine sut = new GameEngine(mockEventPub.Object, mockGameDataRepository.Object);
            await sut.New(playerId);
        
            // Act
            sut.WinGame(finishingFood);
        
            // Assert
            var gameData = sut.GetGameData();
            Assert.Equal(playerId, sut.CurrentPlayerId);
            Assert.False(sut.IsGameRunning);
            Assert.NotEqual(Guid.Empty, gameData.Id);
            Assert.Equal(GameStates.Won, gameData.GameState);
            Assert.Equal(finishingFood, gameData.FinishingFood);
            Assert.Equal(expectedAccolade, gameData.Accolade);
            Assert.Equal(playerId, gameData.PlayerId);
        }
    }
}