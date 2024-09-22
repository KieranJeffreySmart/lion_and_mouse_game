using lion_and_mouse_game.Events;

namespace lion_and_mouse_game.StoryContext
{
    public class StoryEngine(IEventPub eventBroker)
    {
        private readonly IEventPub eventBroker = eventBroker;
        Story? currentStory = null;
        private readonly List<PlayerAction> actions = [];

        public void NewStory(LionStates lionStartingState)
        {
            currentStory = new Story(1);
            currentStory = currentStory.AddParagraph("\r\nOnce upon a time there was a little mouse\r\n");
            eventBroker.Publish(new NewStoryEvent(lionStartingState));
        }

        public void EndDay()
        {
            var mouseAction = actions.FirstOrDefault(a => a.CharacterType == GameCharacterTypes.Mouse && a.Day == currentStory?.CurrentDay);
            var lionAction = actions.FirstOrDefault(a => a.CharacterType == GameCharacterTypes.Lion && a.Day == currentStory?.CurrentDay);

            if (mouseAction != null && lionAction != null && currentStory != null)
            {
                var newParagraph = string.Empty;
                if (mouseAction.ActionType == (int)MouseActionTypes.Hunt)
                {
                    if (lionAction.ActionType == (int)LionActionTypes.Hunt)
                    {
                        if (FiftyFiftyTest())
                        {
                            eventBroker.Publish(new MouseEatenEvent());
                            newParagraph = $"while searching for something to eat the mouse was eaten by a lion";
                        }
                        else
                        {
                            eventBroker.Publish(new MouseReturnedHomeEvent(1));
                            newParagraph = $"while searching for something to eat the mouse avoided being eaten by a lion and returned home with 1 food";
                        }
                    }
                    else if (lionAction.ActionType == (int)LionActionTypes.Sleep)
                    {
                        // TODO: Implement encounter logic
                        eventBroker.Publish(new MouseReturnedHomeEvent(2));
                        newParagraph = $"while searching for something to eat the mouse found a sleeping lion and in her haste returned home with only 1 food";
                    }
                    else if (lionAction.ActionType == (int)LionActionTypes.StayAtHome)
                    {
                        eventBroker.Publish(new MouseReturnedHomeEvent(3));
                        newParagraph = $"after searching for something to eat the mouse returned home with 2 food";
                    }
                }
                else
                {
                    eventBroker.Publish(new MouseStayedHomeEvent());
                    newParagraph = $"the mouse stayed at home";
                }

                currentStory = currentStory.AddParagraph($"On day {currentStory.CurrentDay}, {newParagraph}");
                eventBroker.Publish(new DayEndedEvent(currentStory.CurrentDay, currentStory.Text));
                currentStory = currentStory.IncrementDay();
                eventBroker.Publish(new NewDayEvent(currentStory.CurrentDay));
            }
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
                    StoryText = currentStory.Text
                };
        }
    }


    [Serializable]
    public class StoryData
    {
        public string Id { get; set; } = string.Empty;
        public string StoryText { get; set; } = string.Empty;
        public int CurrentDay { get; set; } = 0;
    }

    public class PlayerAction(GameCharacterTypes characterType, int actionType, int day, int sequenceNumber)
    {
        public GameCharacterTypes CharacterType { get; } = characterType;

        public int ActionType { get; } = actionType;

        public int Day { get; } = day;

        public int SequenceNumber { get; } = sequenceNumber;
    }
}