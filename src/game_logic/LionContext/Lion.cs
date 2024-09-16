

namespace Lion_and_mouse.src.LionContext
{
    public class Lion(LionStates state)
    {
        public LionStates State { get; } = state;

        public static Lion SetState(LionStates newState)
        {
            return new Lion(newState);
        }
    }
}