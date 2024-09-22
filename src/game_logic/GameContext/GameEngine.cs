using System.Text.Json.Serialization;
using lion_and_mouse_game.Events;

namespace lion_and_mouse_game.GameContext
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
            else if (foodStored > 10) return Accolades.Hoarder;

            return Accolades.Survivor;
        }

        public GameData GetGame()
        {
            return loadedGame is null
                ? new GameData { }
                : new GameData
                {
                    Id = loadedGame.Id.ToString(),
                    GameState = loadedGame.GameState.ToString(),
                    FinishingFood = loadedGame.FinishingFood,
                    Accolade = loadedGame.Accolade.ToString()
                };
        }
    }

    [Serializable]
    public class GameData
    {
        public string Id { get; set; } = string.Empty;
        public string GameState { get; set; } = string.Empty;
        public int FinishingFood { get; set; } = -1;
        public string Accolade { get; set; } = string.Empty;
    }
}