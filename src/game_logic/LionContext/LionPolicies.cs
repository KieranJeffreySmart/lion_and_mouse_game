using lion_and_mouse_game.Events;

namespace lion_and_mouse_game.LionContext
{
    public class LionPolicies
    {
        public static void IfNewStory(LionEngine lionEngine, NewStoryEvent gameEvent)
        {
            lionEngine.NewLion(gameEvent.LionStartingState);
        }

        public static void IfNewDay(LionEngine lionEngine, NewDayEvent gameEvent)
        {
            if (lionEngine.IsAtHome) lionEngine.Hunt();
            else if (lionEngine.IsHunting) lionEngine.Sleep();
            else if (lionEngine.IsSleeping) lionEngine.GoHome();
        }
    }
}