
using System.Collections.Concurrent;
using Lion_and_mouse.src.Events;

namespace Lion_and_mouse.src.StoryContext
{
    public class StoryEngine(IEventPub eventBroker)
    {
        private readonly IEventPub eventBroker = eventBroker;
        Story? currentStory = null;
        private readonly List<Action> actions = [];
        
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
                if (mouseAction.ActionType == (int)MouseActionTypes.Hunt)
                {
                    if (lionAction.ActionType == (int)LionActionTypes.Hunt)
                    {
                        if(FiftyFiftyTest())
                        {
                            eventBroker.Publish(new MouseEatenEvent());
                        }
                    }

                    if (lionAction.ActionType == (int)LionActionTypes.Sleep)
                    {
                        // TODO: Implement encounter logic
                        this.eventBroker.Publish(new MouseReturnedHomeEvent(1));
                    }

                    if (lionAction.ActionType == (int)LionActionTypes.StayAtHome)
                    {
                        this.eventBroker.Publish(new MouseReturnedHomeEvent(2));
                    }
                }
                else
                {
                    eventBroker.Publish(new MouseStayedHomeEvent());
                }

                currentStory = currentStory.IncrementDay();
                eventBroker.Publish(new DayEndedEvent(this.currentStory.CurrentDay));
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
                actions.Add(new Action(characterType, actionType, currentStory?.CurrentDay ?? 0, actions.Count+1));
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
    }

    public class Action(GameCharacterTypes characterType, int actionType, int day, int sequenceNumber)
    {
        public GameCharacterTypes CharacterType { get; } = characterType;

        public int ActionType { get; } = actionType;

        public int Day { get; } = day;

        public int SequenceNumber { get; } = sequenceNumber;
    }
}