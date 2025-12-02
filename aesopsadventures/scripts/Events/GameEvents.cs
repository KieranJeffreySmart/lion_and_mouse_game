using System;

namespace lion_and_mouse_game.Events
{
	public interface IGameEvent
	{
		public Guid Id { get; }

		public string Name { get; }
	}

	public class NewGameStartedEvent : IGameEvent
	{
		public Guid Id { get; } = Guid.NewGuid();
		public string Name => nameof(NewGameStartedEvent);
		public LionStates LionStartingState { get; }

		public NewGameStartedEvent(LionStates lionStartingState)
		{
			LionStartingState = lionStartingState;
		}
	}

	public class DayEndedEvent : IGameEvent
	{
		public Guid Id { get; } = Guid.NewGuid();
		public string Name => nameof(DayEndedEvent);
		public int CurrentDay { get; }
		public string StoryText { get; }

		public DayEndedEvent(int currentDay, string storyText)
		{
			CurrentDay = currentDay;
			StoryText = storyText;
		}
	}

	public class MouseDayEndedEvent : IGameEvent
	{
		public Guid Id { get; } = Guid.NewGuid();
		public string Name => nameof(MouseDayEndedEvent);
		public int CurrentDay { get; }
		public int FoodStored { get; }

		public MouseDayEndedEvent(int currentDay, int foodStored)
		{
			CurrentDay = currentDay;
			FoodStored = foodStored;
		}
	}

	public class NewDayEvent : IGameEvent
	{
		public Guid Id { get; } = Guid.NewGuid();
		public string Name => nameof(DayEndedEvent);
		public int CurrentDay { get; }

		public NewDayEvent(int currentDay)
		{
			CurrentDay = currentDay;
		}
	}

	public class MouseDiedEvent : IGameEvent
	{
		public Guid Id { get; } = Guid.NewGuid();
		public string Name => nameof(MouseDiedEvent);
	}

	public class ActionTakenEvent : IGameEvent
	{
		public Guid Id { get; } = Guid.NewGuid();
		public string Name => nameof(ActionTakenEvent);

		public int ActionType { get; }

		public GameCharacterTypes CharacterType { get; }

		public ActionTakenEvent(int actionType, GameCharacterTypes characterType)
		{
			ActionType = actionType;
			CharacterType = characterType;
		}
	}

	public class NewStoryEvent : IGameEvent
	{
		public Guid Id { get; } = Guid.NewGuid();
		public string Name => nameof(NewStoryEvent);
		public LionStates LionStartingState { get; }

		public NewStoryEvent(LionStates lionStartingState)
		{
			LionStartingState = lionStartingState;
		}
	}

	public class MouseReturnedHomeEvent : IGameEvent
	{
		public Guid Id { get; } = Guid.NewGuid();
		public string Name => nameof(MouseReturnedHomeEvent);
		public int FoodGathered { get; }

		public MouseReturnedHomeEvent(int foodGathered)
		{
			FoodGathered = foodGathered;
		}
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
