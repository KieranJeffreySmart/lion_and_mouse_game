using Lion_and_mouse.src.Events;

namespace Lion_and_mouse.src.GameContext
{
    public class GamePolicies
    {
        public static void IfMouseDied(GameEngine gameEngine, MouseDiedEvent gameEvent)
        {
            gameEngine.GameOver();
        }

        public static void IfMouseDayEnded(GameEngine gameEngine, MouseDayEndedEvent gameEvent)
        {
            if (gameEvent.CurrentDay > 7)
            {
                gameEngine.WinGame(gameEvent.FoodStored);
            }
        }
    }
}