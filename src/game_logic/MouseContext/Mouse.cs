

namespace Lion_and_mouse.src.MouseContext
{
    internal class Mouse(MouseStates state, int foodStoreCount)
    {
        public MouseStates State { get; } = state;

        public int FoodStoreCount { get; } = foodStoreCount;

        internal Mouse SetState(MouseStates newState)
        {
            return new Mouse(newState, this.FoodStoreCount);
        }

        internal Mouse UpdateFoodStore(int delta)
        {
            return new Mouse(State, this.FoodStoreCount + delta);
        }
    }
}