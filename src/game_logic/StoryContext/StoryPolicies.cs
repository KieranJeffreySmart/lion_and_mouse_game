using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.PortableExecutable;
using Lion_and_mouse.src.Events;
using System.Linq;

namespace Lion_and_mouse.src.StoryContext
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