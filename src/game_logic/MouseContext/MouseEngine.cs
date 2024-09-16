


using Lion_and_mouse.src.Events;

namespace Lion_and_mouse.src.MouseContext
{
    public class MouseEngine(IEventPub eventBroker)
    {
        private readonly IEventPub eventBroker = eventBroker;
        private Mouse mouse = new(MouseStates.AtHome, 2);

        public bool FoodStoreIsEmpty => mouse?.FoodStoreCount < 1;

        internal void GoHome()
        {
            mouse = mouse.SetState(MouseStates.AtHome);
        }

        internal void Eaten()
        {
            mouse = mouse.SetState(MouseStates.Dead);
            eventBroker.Publish(new MouseDiedEvent());
        }

        internal void EndDay(int currentDay)
        {
            mouse = mouse.UpdateFoodStore(-1);
            eventBroker.Publish(new MouseDayEndedEvent(currentDay, mouse.FoodStoreCount));
        }

        internal void IncrementFoodStore(int foodGathered)
        {
            mouse = mouse.UpdateFoodStore(foodGathered);
        }

        internal void Starve()
        {
            mouse = mouse.SetState(MouseStates.Dead);
            eventBroker.Publish(new MouseDiedEvent());
        }

        internal void NewMouse()
        {
            mouse = new(MouseStates.AtHome, 2);
        }

        internal void Hunt()
        {
            mouse = mouse.SetState(MouseStates.Hunting);
            eventBroker.Publish(new ActionTakenEvent((int)MouseActionTypes.Hunt, GameCharacterTypes.Mouse));
        }

        internal void StayAtHome()
        {
            mouse = mouse.SetState(MouseStates.AtHome);
            eventBroker.Publish(new ActionTakenEvent((int)MouseActionTypes.StayAtHome, GameCharacterTypes.Mouse));
        }
    }
}