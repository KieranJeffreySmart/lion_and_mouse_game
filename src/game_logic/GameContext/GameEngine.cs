using System.Text.Json.Serialization;
using Lion_and_mouse.src.Events;

namespace Lion_and_mouse.src.GameContext
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
            if (foodStored > 10) return Accolades.Hoarder;

            return Accolades.Survivor;
        }

        public GameData GetGame()
        {
            return this.loadedGame is null 
                ? new GameData {} 
                : new GameData { 
                    Id = this.loadedGame.Id.ToString(), 
                    GameState = this.loadedGame.GameState.ToString() 
                };
        }
    }

    [Serializable]
    public class GameData
    {
        public string Id { get; set; } = string.Empty;
        public string GameState { get; set; } = string.Empty;
    }
}