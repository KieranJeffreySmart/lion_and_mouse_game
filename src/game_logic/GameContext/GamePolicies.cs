using lion_and_mouse_game.Events;

namespace lion_and_mouse_game.GameContext
{
    public class GamePolicies
    {
        public static void IfMouseDied(GameEngine gameEngine, MouseDiedEvent gameEvent)
        {
            gameEngine.GameOver();
        }

        public static void IfMouseDayEnded(GameEngine gameEngine, MouseDayEndedEvent gameEvent)
        {
            if (gameEvent.CurrentDay >= 7)
            {
                gameEngine.WinGame(gameEvent.FoodStored);
            }
        }
    }
}