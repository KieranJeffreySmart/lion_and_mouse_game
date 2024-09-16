

namespace Lion_and_mouse.src.MouseContext
{
    public class Mouse(MouseStates state, int foodStoreCount)
    {
        public MouseStates State { get; } = state;

        public int FoodStoreCount { get; } = foodStoreCount;

        public Mouse SetState(MouseStates newState)
        {
            return new Mouse(newState, this.FoodStoreCount);
        }

        public Mouse UpdateFoodStore(int delta)
        {
            return new Mouse(State, this.FoodStoreCount + delta);
        }
    }
}