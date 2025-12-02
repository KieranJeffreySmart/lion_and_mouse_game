using lion_and_mouse_game.Events;

namespace lion_and_mouse_game.MouseContext
{
	public class MousePolicies
	{
		public static void IfNewStory(IMouseEngine mouseEngine)
		{
			mouseEngine.NewMouse();
		}

		public static void IfEaten(IMouseEngine mouseEngine)
		{
			mouseEngine.Eaten();
		}

		public static void IfMouseReturned(IMouseEngine mouseEngine, int foodGathered)
		{
			mouseEngine.IncrementFoodStore(foodGathered);
			mouseEngine.GoHome();
		}

		public static void IfDayEnded(IMouseEngine mouseEngine, int currenDay)
		{
			if (mouseEngine.FoodStoreIsEmpty) mouseEngine.Starve(); 
			else mouseEngine.EndDay(currenDay);
		}
	}
}
