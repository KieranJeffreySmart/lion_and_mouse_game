using lion_and_mouse_game.Events;

namespace lion_and_mouse_game.LionContext
{
    public class LionPolicies
    {
        private readonly ILionBehaviorCalculator lionBehaviorCalculator;
        private readonly LionEngine lionEngine;

        public LionPolicies(ILionBehaviorCalculator lionBehaviorCalculator, LionEngine lionEngine)
        {
            this.lionBehaviorCalculator = lionBehaviorCalculator;
            this.lionEngine = lionEngine;
        }

        public void IfNewStory(NewStoryEvent gameEvent)
        {
            lionEngine.NewLion(gameEvent.LionStartingState);
        }

        public void IfNewDay(NewDayEvent gameEvent)
        {
            LionBehaviours behavior = lionBehaviorCalculator.Calculate(lionEngine, gameEvent);
            switch (behavior)
            {
                case LionBehaviours.Hunt:
                    lionEngine.Hunt();
                    break;
                case LionBehaviours.Sleep:
                    lionEngine.Sleep();
                    break;
                case LionBehaviours.GoHome:
                    lionEngine.GoHome();
                    break;
            }
        }
    }

    public interface ILionBehaviorCalculator
    {
        LionBehaviours Calculate(LionEngine lionEngine, NewDayEvent gameEvent);
    }

    public class DefaultLionBehaviorCalculator : ILionBehaviorCalculator
    {
        public LionBehaviours Calculate(LionEngine lionEngine, NewDayEvent gameEvent)
        {
            // Default behaviour: Lion always stays at home
            if (lionEngine.IsAtHome) return LionBehaviours.Hunt;
            else if (lionEngine.IsHunting) return LionBehaviours.Sleep;
            else if (lionEngine.IsSleeping) return LionBehaviours.GoHome;
            else return LionBehaviours.GoHome;
        }
    }

    public enum LionBehaviours
    {
        Hunt,
        Sleep,
        GoHome
    }
}