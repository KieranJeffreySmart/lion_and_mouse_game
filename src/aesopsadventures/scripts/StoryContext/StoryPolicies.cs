using lion_and_mouse_game.Events;

namespace lion_and_mouse_game.StoryContext
{
	public class StoryPolicies
	{
		public static void IfActionTaken(IStoryEngine storyEngine, GameCharacterTypes character, int action)
		{
			storyEngine.TrackCharacterAction(character, action);

			if (storyEngine.AllActionsMade() && storyEngine.GetLastActionCharacterType() == character)
				storyEngine.EndDay();
		}

		public static void IfNewGame(IStoryEngine storyEngine, LionStates LionStartingState)
		{
			storyEngine.NewStory(LionStartingState);
		}
	}
}
