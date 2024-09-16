namespace Lion_and_mouse.src.Events
{
    public interface IGameEvent
    {
        public Guid Id { get; }
    }

    public class NewGameStartedEvent(LionStates lionStartingState) : IGameEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public LionStates LionStartingState { get; } = lionStartingState;
    }

    public class DayEndedEvent(int currentDay) : IGameEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public int CurrentDay { get; } = currentDay;
    }
    
    public class MouseDayEndedEvent(int currentDay, int foodStored) : IGameEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public int CurrentDay { get; } = currentDay;        
        public int FoodStored { get; } = foodStored;

    }

    public class MouseDiedEvent : IGameEvent 
    {
        public Guid Id { get; } = Guid.NewGuid();
    }

    public class ActionTakenEvent(int actionType, GameCharacterTypes characterType) : IGameEvent
    {
        public Guid Id { get; } = Guid.NewGuid();

        public int ActionType { get; } = actionType;

        public GameCharacterTypes CharacterType { get; } = characterType;
    }

    public class NewStoryEvent(LionStates lionStartingState) : IGameEvent 
    {
        public Guid Id { get; } = Guid.NewGuid();
        public LionStates LionStartingState { get; } = lionStartingState;
    }

    public class MouseReturnedHomeEvent(int foodGathered) : IGameEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public int FoodGathered { get; } = foodGathered;
    }

    public class MouseEatenEvent : IGameEvent 
    {
        public Guid Id { get; } = Guid.NewGuid();
    }

    public class MouseStayedHomeEvent : IGameEvent 
    {
        public Guid Id { get; } = Guid.NewGuid();
    }
    
    public class MouseFoodStoreChanged : IGameEvent 
    {
        public Guid Id { get; } = Guid.NewGuid();
    }
}