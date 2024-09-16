
namespace Lion_and_mouse.src.LionContext
{
    public class LionEngine
    {
        private Lion lion = new(LionStates.AtHome);

        public bool IsAtHome => lion?.State == LionStates.AtHome;
        public bool IsHunting => lion?.State == LionStates.Hunting;
        public bool IsSleeping => lion?.State == LionStates.Sleeping;

        internal void GoHome()
        {
            lion = Lion.SetState(LionStates.AtHome);
        }

        internal void Hunt()
        {
            lion = Lion.SetState(LionStates.Hunting);
        }

        internal void NewLion(LionStates lionStartingState)
        {
            lion = new(lionStartingState);
        }

        internal void Sleep()
        {
            lion = Lion.SetState(LionStates.Sleeping);
        }
    }
}