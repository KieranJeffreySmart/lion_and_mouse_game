namespace Lion_and_mouse.src.Events
{
    public interface IGameEvent
    {
        public Guid Id { get; }

        public string Name { get; }
    }

    public class NewGameStartedEvent(LionStates lionStartingState) : IGameEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name => nameof(NewGameStartedEvent);
        public LionStates LionStartingState { get; } = lionStartingState;
    }

    public class DayEndedEvent(int currentDay, string storyText) : IGameEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name => nameof(DayEndedEvent);
        public int CurrentDay { get; } = currentDay;
        public string StoryText { get; } = storyText;
    }
    
    public class MouseDayEndedEvent(int currentDay, int foodStored) : IGameEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name => nameof(MouseDayEndedEvent);
        public int CurrentDay { get; } = currentDay;        
        public int FoodStored { get; } = foodStored;

    }

    public class MouseDiedEvent : IGameEvent 
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name => nameof(MouseDiedEvent);
    }

    public class ActionTakenEvent(int actionType, GameCharacterTypes characterType) : IGameEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name => nameof(ActionTakenEvent);

        public int ActionType { get; } = actionType;

        public GameCharacterTypes CharacterType { get; } = characterType;
    }

    public class NewStoryEvent(LionStates lionStartingState) : IGameEvent 
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name => nameof(NewStoryEvent);
        public LionStates LionStartingState { get; } = lionStartingState;
    }

    public class MouseReturnedHomeEvent(int foodGathered) : IGameEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name => nameof(MouseReturnedHomeEvent);
        public int FoodGathered { get; } = foodGathered;
    }

    public class MouseEatenEvent : IGameEvent 
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name => nameof(MouseEatenEvent);
    }

    public class MouseStayedHomeEvent : IGameEvent 
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name => nameof(MouseStayedHomeEvent);
    }
    
    public class MouseFoodStoreChanged : IGameEvent 
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name => nameof(MouseFoodStoreChanged);
    }
    
    public class GameWon : IGameEvent 
    {
        public GameWon(int foodStored, Accolades accolade)
        {
            FoodStored = foodStored;
            Accolade = accolade;
        }

        public Guid Id { get; } = Guid.NewGuid();
        public string Name => nameof(GameWon);
        public int FoodStored { get; }
        public Accolades Accolade { get; }
    }
    
    public class GameLost : IGameEvent 
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name => nameof(GameLost);
    }
}