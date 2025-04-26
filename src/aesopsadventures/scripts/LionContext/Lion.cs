namespace lion_and_mouse_game.LionContext
{
	public class Lion
	{
		public LionStates State { get; }

		public Lion(LionStates state)
		{
			State = state;
		}

		public Lion SetState(LionStates newState)
		{
			return new Lion(newState);
		}
	}
}
