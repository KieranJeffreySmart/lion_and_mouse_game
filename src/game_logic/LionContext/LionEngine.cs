using lion_and_mouse_game.Events;

namespace lion_and_mouse_game.LionContext
{
    public class LionEngine(IEventPub eventBroker)
    {
        private readonly IEventPub eventBroker = eventBroker;
        private Lion lion = new(LionStates.AtHome);

        public bool IsAtHome => lion?.State == LionStates.AtHome;
        public bool IsHunting => lion?.State == LionStates.Hunting;
        public bool IsSleeping => lion?.State == LionStates.Sleeping;

        public void GoHome()
        {
            lion = lion.SetState(LionStates.AtHome);
            eventBroker.Publish(new ActionTakenEvent((int)LionActionTypes.StayAtHome, GameCharacterTypes.Lion));
        }

        public void Hunt()
        {
            lion = lion.SetState(LionStates.Hunting);
            eventBroker.Publish(new ActionTakenEvent((int)LionActionTypes.Hunt, GameCharacterTypes.Lion));
        }

        public void Sleep()
        {
            lion = lion.SetState(LionStates.Sleeping);
            eventBroker.Publish(new ActionTakenEvent((int)LionActionTypes.Sleep, GameCharacterTypes.Lion));
        }

        public void NewLion(LionStates lionStartingState)
        {
            lion = new(lionStartingState);
            if (IsAtHome) eventBroker.Publish(new ActionTakenEvent((int)LionActionTypes.StayAtHome, GameCharacterTypes.Lion));
            else if (IsHunting) eventBroker.Publish(new ActionTakenEvent((int)LionActionTypes.Hunt, GameCharacterTypes.Lion));
            else if (IsSleeping) eventBroker.Publish(new ActionTakenEvent((int)LionActionTypes.Sleep, GameCharacterTypes.Lion));
        }

        public LionData GetLion()
        {
            return new()
            {
                State = lion.State.ToString()
            };
        }
    }


    [Serializable]
    public class LionData
    {
        public string State { get; set; } = string.Empty;
    }
}