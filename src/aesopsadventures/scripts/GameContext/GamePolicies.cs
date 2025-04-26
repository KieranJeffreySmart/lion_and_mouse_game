using lion_and_mouse_game.Events;

namespace lion_and_mouse_game.GameContext
{
	public class GamePolicies
	{
		public static void IfMouseDied(IGameEngine gameEngine)
		{
			gameEngine.GameOver();
		}

		public static void IfMouseDayEnded(IGameEngine gameEngine, int currentDay, int foodStored)
		{
			if (currentDay >= 7)
			{
				gameEngine.WinGame(foodStored);
			}
		}
	}
}
