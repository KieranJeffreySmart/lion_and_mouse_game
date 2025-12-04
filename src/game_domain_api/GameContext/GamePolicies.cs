using game_domain_api.Events;
using game_domain_api.GameContext;

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