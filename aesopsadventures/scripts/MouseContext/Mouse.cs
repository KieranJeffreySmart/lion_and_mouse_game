namespace lion_and_mouse_game.MouseContext
{
	public class Mouse
	{
		public MouseStates State { get; }

		public int FoodStoreCount { get; }

		public Mouse(MouseStates state, int foodStoreCount)
		{
			State = state;
			FoodStoreCount = foodStoreCount;
		}

		public Mouse SetState(MouseStates newState)
		{
			return new Mouse(newState, FoodStoreCount);
		}

		public Mouse UpdateFoodStore(int delta)
		{
			return new Mouse(State, FoodStoreCount + delta);
		}
	}
}
