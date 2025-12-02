using System;
using System.Collections.Generic;
using System.Linq;
using lion_and_mouse_game.Events;

namespace lion_and_mouse_game.StoryContext
{
	public class StoryEngine : IStoryEngine
	{
		Story currentStory = null;
		private readonly List<PlayerAction> actions = new();
		private readonly List<StoryOptions> availableOptions = new();

		private readonly Dictionary<StoryOptions, StoryOption> options = new();

		public StoryEngine()
		{
			options.Add(StoryOptions.MouseStayAtHome, new StoryOption(
				"I have enough berries, I think I'll stay at home today",
				() => { currentStory = currentStory.AddParagraph("I have enough berries, I think I'll stay at home today"); }));
			options.Add(StoryOptions.MouseHunt, new StoryOption(
				"I need more berries, maybe I should go foraging today",
				() => { currentStory = currentStory.AddParagraph("I need more berries, maybe I should go foraging today"); }));
			options.Add(StoryOptions.EndStory, new StoryOption(
				"End Story",
				() => { currentStory = currentStory.AddParagraph("The End"); }));
		}

		public void NewStory(LionStates lionStartingState)
		{
			actions.Clear();
			currentStory = new Story();
			StartNewDay("\r\nOnce upon a time there was a little mouse\r\n");
		}

		public EndOfDayResult EndDay()
		{
			var resultevent = EndOfDayEvent.None;
			var gatheredFood = 0;
			var mouseAction = actions.FirstOrDefault(a => a.CharacterType == GameCharacterTypes.Mouse && a.Day == currentStory?.CurrentDay);
			var lionAction = actions.FirstOrDefault(a => a.CharacterType == GameCharacterTypes.Lion && a.Day == currentStory?.CurrentDay);

			if (mouseAction != null && lionAction != null && currentStory != null)
			{
				var newParagraph = string.Empty;
				if (mouseAction.ActionType == (int)MouseActionTypes.Hunt)
				{
					newParagraph = "and so the mouse went foraging for food...";

					if (lionAction.ActionType == (int)LionActionTypes.Hunt)
					{
						if (FiftyFiftyTest())
						{
							newParagraph += $"\r\nwhile searching for something to eat the mouse was eaten by a lion";
							resultevent = EndOfDayEvent.MouseEaten;
						}
						else
						{
							newParagraph = $"\r\nwhile searching for something to eat the mouse avoided being eaten by a lion and returned home with 1 food";
							resultevent = EndOfDayEvent.ReturnedHome;
							gatheredFood = 1;
						}
					}
					else if (lionAction.ActionType == (int)LionActionTypes.Sleep)
					{
						// TODO: Implement encounter logic
						newParagraph = $"\r\nwhile searching for something to eat the mouse found a sleeping lion and in her haste returned home with only 1 food";
						resultevent = EndOfDayEvent.ReturnedHome;
						gatheredFood = 1;
					}
					else if (lionAction.ActionType == (int)LionActionTypes.StayAtHome)
					{
						newParagraph = $"\r\nafter searching for something to eat the mouse returned home with 2 food";
						resultevent = EndOfDayEvent.ReturnedHome;
						gatheredFood = 2;
					}
				}
				else
				{
					newParagraph = $"and so the mouse stayed at home";
					resultevent = EndOfDayEvent.StayedAtHome;
				}

				currentStory = currentStory.AddParagraph(newParagraph);

				StartNewDay("later that night the mouse ate a berry for dinner and went to bed");
			}

			return new EndOfDayResult(resultevent, gatheredFood, currentStory.CurrentDay, currentStory.Text);
		}

		private void StartNewDay(string beforeParagraph)
		{
			currentStory = currentStory.IncrementDay();
			currentStory = currentStory.AddParagraph(beforeParagraph)
			.AddParagraph($"{(currentStory.CurrentDay == 1 ? "One ": "The next")} morning the mouse woke up and looked at her pile of berries and thought...");
			availableOptions.Clear();
			availableOptions.Add(StoryOptions.MouseHunt);
			availableOptions.Add(StoryOptions.MouseStayAtHome);
		}

		private static bool FiftyFiftyTest()
		{
			Random rnd = new(DateTime.Now.Second);
			var result = rnd.Next(1, 101);

			return result >= 50;
		}

		public void TrackCharacterAction(GameCharacterTypes characterType, int actionType)
		{
			if (actions.FirstOrDefault(a => a.CharacterType == characterType && a.Day == currentStory?.CurrentDay) == null)
			{
				actions.Add(new PlayerAction(characterType, actionType, currentStory?.CurrentDay ?? 0, actions.Count + 1));
			}
		}

		public bool AllActionsMade()
		{
			var sortedActions = actions.Where(a => a.Day == currentStory?.CurrentDay).OrderBy(a => a.SequenceNumber);
			return sortedActions.Any(a => a.CharacterType == GameCharacterTypes.Lion) && sortedActions.Any(a => a.CharacterType == GameCharacterTypes.Mouse);
		}

		public GameCharacterTypes GetLastActionCharacterType()
		{
			var sortedActions = actions.OrderBy(a => a.SequenceNumber).ToList();

			return sortedActions.Last().CharacterType;
		}

		public StoryData GetStory()
		{
			return currentStory is null
				? new StoryData { }
				: new StoryData
				{
					Id = currentStory.Id.ToString(),
					CurrentDay = currentStory.CurrentDay,
					StoryText = currentStory.Text,
					Options = availableOptions.ToList()
				};
		}

		public IReadOnlyDictionary<StoryOptions, StoryOption> GetAllStoryOptions()
		{
			return options;
		}
	}

	public interface IStoryEngine
	{
		public void NewStory(LionStates lionStartingState);

		public EndOfDayResult EndDay();

		public void TrackCharacterAction(GameCharacterTypes characterType, int actionType);

		public bool AllActionsMade();

		public GameCharacterTypes GetLastActionCharacterType();

		public StoryData GetStory();

		public IReadOnlyDictionary<StoryOptions, StoryOption> GetAllStoryOptions();
	}

	public class StoryEngineEventDecorator: IStoryEngine
	{
		private readonly IEventPub eventBroker;
		private readonly IStoryEngine component;

		public StoryEngineEventDecorator(IEventPub eventBroker, IStoryEngine component)
		{
			this.eventBroker = eventBroker;
			this.component = component;
		}

		public void NewStory(LionStates lionStartingState)
		{
			eventBroker.Publish(new NewStoryEvent(lionStartingState));
		}

		public EndOfDayResult EndDay()
		{
			var result = component.EndDay();

			if (result.EndOfDayEvent == EndOfDayEvent.None)
				
			switch (result.EndOfDayEvent)
			{
				
				case EndOfDayEvent.MouseEaten: 
					eventBroker.Publish(new MouseEatenEvent()); 
				break;

				case EndOfDayEvent.ReturnedHome:
					eventBroker.Publish(new MouseReturnedHomeEvent(result.GatheredFood)); 
				break;

				case EndOfDayEvent.StayedAtHome:
					eventBroker.Publish(new MouseStayedHomeEvent()); 
				break;
				
				default: throw new Exception("Unknown EndOfDay event");

			}

			eventBroker.Publish(new DayEndedEvent(result.CurrentDay, result.Text));

			return result;
		}

		public void TrackCharacterAction(GameCharacterTypes characterType, int actionType)
		{
			component.TrackCharacterAction(characterType, actionType);
		}

		public bool AllActionsMade()
		{
			return component.AllActionsMade();
		}

		public GameCharacterTypes GetLastActionCharacterType()
		{
			return component.GetLastActionCharacterType();
		}

		public StoryData GetStory()
		{
			return component.GetStory();
		}

		public IReadOnlyDictionary<StoryOptions, StoryOption> GetAllStoryOptions()
		{
			return component.GetAllStoryOptions();
		}
	}


	[Serializable]
	public class StoryData
	{
		public string Id { get; set; } = string.Empty;
		public string StoryText { get; set; } = string.Empty;
		public int CurrentDay { get; set; } = 0;
		public IEnumerable<StoryOptions> Options { get; set; } = new List<StoryOptions>();

	}

	public class PlayerAction
	{
		public GameCharacterTypes CharacterType { get; }

		public int ActionType { get; }

		public int Day { get; }

		public int SequenceNumber { get; }

		public PlayerAction(GameCharacterTypes characterType, int actionType, int day, int sequenceNumber)
		{
			CharacterType = characterType;
			ActionType = actionType;
			Day = day;
 			SequenceNumber = sequenceNumber;
		}
	}

	public class StoryOption
	{
		public string OptionText { get; }
		public Action Select { get; }

		public StoryOption(string optionText, Action select)
		{
			OptionText = optionText;
			Select = select;
		}
	}

	public enum EndOfDayEvent
	{
		None,
		MouseEaten,
		ReturnedHome,
		StayedAtHome

	}

	public class EndOfDayResult
	{
		public EndOfDayEvent EndOfDayEvent { get; }

		public int GatheredFood { get; }
		public int CurrentDay { get; }
		public string Text { get; }

		public EndOfDayResult(EndOfDayEvent endOfDayEvent, int gatheredFood, int currentDay, string text)
		{
			EndOfDayEvent = endOfDayEvent;
			GatheredFood = gatheredFood;
			CurrentDay = currentDay;
			Text = text;
		}
	}
}
