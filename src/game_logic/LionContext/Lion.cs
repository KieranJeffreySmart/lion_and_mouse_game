namespace lion_and_mouse_game.LionContext
{
    public class Lion(LionStates state)
    {
        public LionStates State { get; } = state;

        public Lion SetState(LionStates newState)
        {
            return new Lion(newState);
        }
    }
}