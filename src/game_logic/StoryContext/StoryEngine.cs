
using System.Collections.Concurrent;
using Lion_and_mouse.src.Events;

namespace Lion_and_mouse.src.StoryContext
{
    public class StoryEngine(IEventPub eventBroker)
    {
        private readonly IEventPub eventBroker = eventBroker;
        Story? currentStory = null;
        private readonly List<PlayerAction> actions = [];
        
        public void NewStory(LionStates lionStartingState)
        {
            currentStory = new Story(1);
            eventBroker.Publish(new NewStoryEvent(lionStartingState));
        }

        public void EndDay()
        {
            var mouseAction = actions.FirstOrDefault(a => a.CharacterType == GameCharacterTypes.Mouse && a.Day == currentStory?.CurrentDay);
            var lionAction = actions.FirstOrDefault(a => a.CharacterType == GameCharacterTypes.Lion && a.Day == currentStory?.CurrentDay);

            if (mouseAction != null && lionAction != null && currentStory != null)
            {
                IGameEvent? gameEvent = null;
                var newParagraph = string.Empty;
                if (mouseAction.ActionType == (int)MouseActionTypes.Hunt)
                {
                    if (lionAction.ActionType == (int)LionActionTypes.Hunt)
                    {
                        if(FiftyFiftyTest())
                        {
                            gameEvent = new MouseEatenEvent();
                            newParagraph = $"while searching for something to eat the mouse was eaten by a lion";
                        }
                        else
                        {
                            gameEvent = new MouseReturnedHomeEvent(1);
                            newParagraph = $"while searching for something to eat the mouse avoided being eaten by a lion and returned home with 1 food";
                        }
                    }

                    if (lionAction.ActionType == (int)LionActionTypes.Sleep)
                    {
                        // TODO: Implement encounter logic
                        gameEvent = new MouseReturnedHomeEvent(1);
                        newParagraph = $"while searching for something to eat the mouse found a sleeping lion and in her haste returned home with only 1 food";
                    }

                    if (lionAction.ActionType == (int)LionActionTypes.StayAtHome)
                    {
                        gameEvent = new MouseReturnedHomeEvent(2);
                        newParagraph = $"after searching for something to eat the mouse returned home with 2 food";
                    }
                }
                else
                {
                    gameEvent = new MouseStayedHomeEvent();
                    newParagraph = $"the mouse stayed at home";
                }

                if (!(gameEvent is null))
                {
                    eventBroker.Publish(gameEvent);
                    currentStory.AddParagraph($"On day {currentStory.CurrentDay}, {newParagraph}");
                }

                currentStory = currentStory.IncrementDay();
                eventBroker.Publish(new DayEndedEvent(this.currentStory.CurrentDay, currentStory.Text));
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
                actions.Add(new PlayerAction(characterType, actionType, currentStory?.CurrentDay ?? 0, actions.Count+1));
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
                ? new StoryData {} 
                : new StoryData { 
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