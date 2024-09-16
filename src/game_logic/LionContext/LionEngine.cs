
namespace Lion_and_mouse.src.LionContext
{
    public class LionEngine
    {
        private Lion lion = new(LionStates.AtHome);

        public bool IsAtHome => lion?.State == LionStates.AtHome;
        public bool IsHunting => lion?.State == LionStates.Hunting;
        public bool IsSleeping => lion?.State == LionStates.Sleeping;

        public void GoHome()
        {
            lion = Lion.SetState(LionStates.AtHome);
        }

        public void Hunt()
        {
            lion = Lion.SetState(LionStates.Hunting);
        }

        public void NewLion(LionStates lionStartingState)
        {
            lion = new(lionStartingState);
        }

        public void Sleep()
        {
            lion = Lion.SetState(LionStates.Sleeping);
        }
    }
}