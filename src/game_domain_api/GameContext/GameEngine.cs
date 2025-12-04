using game_domain_api.Events;

namespace game_domain_api.GameContext
{
    public class GameEngine(IEventPub eventBroker)
    {
        private Game? loadedGame = null;
        readonly IEventPub eventBroker = eventBroker;

        public bool IsGameRunning => loadedGame?.GameState == GameStates.Playing;

        public Guid CurrentPlayerId => loadedGame?.PlayerId ?? Guid.Empty;

        public void New(Guid playerId)
        {
            loadedGame = new Game(GameStates.Playing, playerId);
            eventBroker.Publish(new NewGameStartedEvent(LionStates.Sleeping));
        }

        public void GameOver()
        {
            loadedGame = loadedGame?.LoseGame();
            eventBroker.Publish(new GameLost());
        }

        public void WinGame(int foodStored)
        {
            loadedGame = loadedGame?.WinGame(foodStored, CalculateAccolade(foodStored));
            eventBroker.Publish(new GameWon(foodStored, CalculateAccolade(foodStored)));
        }

        private static Accolades CalculateAccolade(int foodStored)
        {
            if (foodStored < 5) return Accolades.Desperado;
            else if (foodStored >= 10) return Accolades.Hoarder;

            return Accolades.Survivor;
        }

        public GameData GetGame()
        {
            return loadedGame is null
                ? new GameData { }
                : new GameData
                {
                    Id = loadedGame.Id,
                    GameState = loadedGame.GameState,
                    FinishingFood = loadedGame.FinishingFood,
                    Accolade = loadedGame.Accolade
                };
        }
    }

    [Serializable]
    public class GameData
    {
        public Guid Id { get; set; } = Guid.Empty;
        public GameStates GameState { get; set; } = GameStates.Unknown;
        public int FinishingFood { get; set; } = -1;
        public Accolades Accolade { get; set; } = Accolades.Unknown;
    }
}