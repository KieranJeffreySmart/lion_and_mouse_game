using Lion_and_mouse.src.Events;

namespace Lion_and_mouse.src.GameContext
{
    public class GameEngine(IEventPub eventBroker)
    {
        private Game? loadedGame = null;
        readonly IEventPub eventBroker = eventBroker;

        public bool IsGameRunning => loadedGame?.GameState == GameStates.Playing;

        public void New()
        {
            loadedGame = new Game(GameStates.Playing);
            eventBroker.Publish(new NewGameStartedEvent(LionStates.Sleeping));
        }

        public void GameOver()
        {
            loadedGame = loadedGame?.LoseGame();            
        }

        internal void WinGame(int foodStored)
        {
            loadedGame = loadedGame?.WinGame(foodStored*2, CalculateAccolade(foodStored));  
        }

        private static Accolades CalculateAccolade(int foodStored)
        {
            if (foodStored < 5) return Accolades.Desperate;
            if (foodStored > 10) return Accolades.Hoarder;

            return Accolades.Survivor;
        }
    }
}