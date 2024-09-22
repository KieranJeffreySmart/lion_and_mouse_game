using lion_and_mouse_game.Events;

namespace lion_and_mouse_game.MouseContext
{
    public class MousePolicies
    {
        public static void IfNewStory(MouseEngine mouseEngine, NewStoryEvent gameEvent)
        {
            mouseEngine.NewMouse();
        }

        public static void IfEaten(MouseEngine mouseEngine, MouseEatenEvent gameEvent)
        {
            mouseEngine.Eaten();
        }

        public static void IfMouseReturned(MouseEngine mouseEngine, MouseReturnedHomeEvent gameEvent)
        {
            mouseEngine.IncrementFoodStore(gameEvent.FoodGathered);
            mouseEngine.GoHome();
        }

        public static void IfDayEnded(MouseEngine mouseEngine, DayEndedEvent gameEvent)
        {
            if (mouseEngine.FoodStoreIsEmpty) mouseEngine.Starve(); 
            else mouseEngine.EndDay(gameEvent.CurrentDay);
        }
    }
}