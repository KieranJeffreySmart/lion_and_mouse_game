using Lion_and_mouse.src.Events;

namespace Lion_and_mouse.src.LionContext
{
    public class LionPolicies
    {
        public static void IfNewStory(LionEngine lionEngine, NewStoryEvent gameEvent)
        {
            lionEngine.NewLion(gameEvent.LionStartingState);
        }

        public static void IfDayEnded(LionEngine lionEngine, DayEndedEvent gameEvent)
        {
            if (lionEngine.IsAtHome) lionEngine.Hunt();
            if (lionEngine.IsHunting) lionEngine.Sleep();
            if (lionEngine.IsSleeping) lionEngine.GoHome();
        }
    }
}