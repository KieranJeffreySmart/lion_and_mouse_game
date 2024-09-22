using lion_and_mouse_game.Events;

namespace lion_and_mouse_game.MouseContext
{
    public class MouseEngine(IEventPub eventBroker)
    {
        private readonly IEventPub eventBroker = eventBroker;
        private Mouse mouse = new(MouseStates.AtHome, 2);

        public bool FoodStoreIsEmpty => mouse?.FoodStoreCount < 1;

        public void GoHome()
        {
            mouse = mouse.SetState(MouseStates.AtHome);
        }

        public void Eaten()
        {
            mouse = mouse.SetState(MouseStates.Dead);
            eventBroker.Publish(new MouseDiedEvent());
        }

        public void EndDay(int currentDay)
        {
            mouse = mouse.SetState(MouseStates.AtHome).UpdateFoodStore(-1);
            eventBroker.Publish(new MouseDayEndedEvent(currentDay, mouse.FoodStoreCount));
        }

        public void IncrementFoodStore(int foodGathered)
        {
            Console.WriteLine($"Gathered {foodGathered} food");
            mouse = mouse.UpdateFoodStore(foodGathered);
        }

        public void Starve()
        {
            mouse = mouse.SetState(MouseStates.Dead);
            eventBroker.Publish(new MouseDiedEvent());
        }

        public void NewMouse()
        {
            mouse = new(MouseStates.AtHome, 2);
        }

        public void Hunt()
        {
            mouse = mouse.SetState(MouseStates.Hunting);
            eventBroker.Publish(new ActionTakenEvent((int)MouseActionTypes.Hunt, GameCharacterTypes.Mouse));
        }

        public void StayAtHome()
        {
            mouse = mouse.SetState(MouseStates.AtHome);
            eventBroker.Publish(new ActionTakenEvent((int)MouseActionTypes.StayAtHome, GameCharacterTypes.Mouse));
        }

        public MouseData GetMouse()
        {
            return new MouseData { State = mouse.State.ToString(), Food = mouse.FoodStoreCount };
        }
    }


    [Serializable]
    public class MouseData
    {
        public string State { get; set; } = string.Empty;
        public int Food { get; set; } = -1;
    }

}