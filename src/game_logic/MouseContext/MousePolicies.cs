using Lion_and_mouse.src.Events;

namespace Lion_and_mouse.src.MouseContext
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
            if (mouseEngine.FoodStoreIsEmpty)
            {
                mouseEngine.Starve();
            }
            else
            {
                mouseEngine.EndDay(gameEvent.CurrentDay);
            }
        }
    }
}