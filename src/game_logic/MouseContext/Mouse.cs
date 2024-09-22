namespace lion_and_mouse_game.MouseContext
{
    public class Mouse(MouseStates state, int foodStoreCount)
    {
        public MouseStates State { get; } = state;

        public int FoodStoreCount { get; } = foodStoreCount;

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