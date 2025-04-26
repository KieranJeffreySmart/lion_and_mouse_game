using lion_and_mouse_game.Events;

namespace lion_and_mouse_game.LionContext
{
	public class LionPolicies
	{
		public static void IfNewStory(ILionEngine lionEngine, LionStates lionStartingState)
		{
			lionEngine.NewLion(lionStartingState);
		}

		public static void IfNewDay(ILionEngine lionEngine)
		{
			if (lionEngine.IsAtHome) lionEngine.Hunt();
			else if (lionEngine.IsHunting) lionEngine.Sleep();
			else if (lionEngine.IsSleeping) lionEngine.GoHome();
		}
	}
}
