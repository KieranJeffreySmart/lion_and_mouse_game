using game_domain_api.Events;
using game_domain_api.Repository;

namespace game_domain_api.GameContext
{
    public class GameEngine(IEventPub eventBroker, IGameDataRepository gameDataRepository): IGameEngine
    {
        private Game? loadedGame = null;
        readonly IEventPub eventBroker = eventBroker;
        readonly IGameDataRepository gameDataRepository = gameDataRepository;

        public bool IsGameRunning => loadedGame?.GameState == GameStates.Playing;

        public Guid CurrentPlayerId => loadedGame?.PlayerId ?? Guid.Empty;

        public async Task New(Guid playerId)
        {
            loadedGame = new Game(new GameData { Id = Guid.NewGuid(), GameState = GameStates.Playing, PlayerId = playerId });
            await gameDataRepository.AddAsync(loadedGame.AsData());
            eventBroker.Publish(new NewGameStartedEvent(LionStates.Sleeping));
        }

        public void GameOver(int foodStored)
        {
            loadedGame?.LoseGame(foodStored);
            eventBroker.Publish(new GameLost());
        }

        public void WinGame(int foodStored)
        {
            loadedGame?.WinGame(foodStored, CalculateAccolade(foodStored));
            eventBroker.Publish(new GameWon(foodStored, CalculateAccolade(foodStored)));
        }

        private static Accolades CalculateAccolade(int foodStored)
        {
            if (foodStored < 5) return Accolades.Desperado;
            else if (foodStored >= 10) return Accolades.Hoarder;

            return Accolades.Survivor;
        }

        public GameData GetGameData()
        {
            return loadedGame?.AsData() ?? new GameData { };
        }

        public async Task LoadGameById(Guid gameId)
        {
            var gameData = await gameDataRepository.GetByIdAsync(gameId);
            if (gameData is null)
            {
                throw new Exception($"Game with Id {gameId} not found");
            }

            loadedGame = new Game(gameData);
        }
    }

    public interface IGameEngine
    {
        public bool IsGameRunning { get; }
        public Guid CurrentPlayerId { get; }
        Task New(Guid playerId);
        void GameOver(int foodStored);
        void WinGame(int foodStored);
        GameData GetGameData();
        Task LoadGameById(Guid gameId);
    }
}