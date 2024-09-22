using lion_and_mouse_game.Events;

namespace lion_and_mouse_game.StoryContext
{
    public class StoryPolicies
    {
        public static void IfActionTaken(StoryEngine storyEngine, ActionTakenEvent gameEvent)
        {
            storyEngine.TrackCharacterAction(gameEvent.CharacterType, gameEvent.ActionType);

            if (storyEngine.AllActionsMade() && storyEngine.GetLastActionCharacterType() == gameEvent.CharacterType)
                storyEngine.EndDay();
        }

        public static void IfNewGame(StoryEngine storyEngine, NewGameStartedEvent gameEvent)
        {
            storyEngine.NewStory(gameEvent.LionStartingState);
        }
    }
}