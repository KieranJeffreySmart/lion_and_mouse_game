using System;
using Godot;
using lion_and_mouse_game.Events;

namespace lion_and_mouse_game.MouseContext
{
	public class MouseEngine : IMouseEngine
	{

		public MouseEngine()
		{
		}

		private Mouse mouse = new(MouseStates.AtHome, 2);

		public bool FoodStoreIsEmpty => mouse?.FoodStoreCount < 1;

		public void GoHome()
		{
			mouse = mouse.SetState(MouseStates.AtHome);
		}

		public void Eaten()
		{
			mouse = mouse.SetState(MouseStates.Dead);
		}

		public int EndDay(int currentDay)
		{
			mouse = mouse.SetState(MouseStates.AtHome).UpdateFoodStore(-1);
			return mouse.FoodStoreCount;
		}

		public void IncrementFoodStore(int foodGathered)
		{
			mouse = mouse.UpdateFoodStore(foodGathered);
		}

		public void Starve()
		{
			mouse = mouse.SetState(MouseStates.Dead);
		}

		public void NewMouse()
		{
			mouse = new(MouseStates.AtHome, 2);
		}

		public void Hunt()
		{
			mouse = mouse.SetState(MouseStates.Hunting);
		}

		public void StayAtHome()
		{
			mouse = mouse.SetState(MouseStates.AtHome);
		}

		public MouseData GetMouse()
		{
			return new MouseData { State = mouse.State.ToString(), Food = mouse.FoodStoreCount };
		}
	}

	public interface IMouseEngine
	{
		public bool FoodStoreIsEmpty {get;}

		public void GoHome();

		public void Eaten();

		public int EndDay(int currentDay);

		public void IncrementFoodStore(int foodGathered);

		public void Starve();

		public void NewMouse();

		public void Hunt();

		public void StayAtHome();
		public MouseData GetMouse();
	}

	public class MouseEngineEventDecorator: IMouseEngine
	{
		private readonly IEventPub eventBroker;
		private readonly IMouseEngine component;

		public MouseEngineEventDecorator(IEventPub eventBroker, IMouseEngine component)
		{
			this.eventBroker = eventBroker;
			this.component = component;
		}

		public bool FoodStoreIsEmpty => component.FoodStoreIsEmpty;

		public void GoHome()
		{
			component.GoHome();
		}

		public void Eaten()
		{
			component.Eaten();
			eventBroker.Publish(new MouseDiedEvent());
		}

		public int EndDay(int currentDay)
		{
			var foodStoreCount = component.EndDay(currentDay);
			eventBroker.Publish(new MouseDayEndedEvent(currentDay, foodStoreCount));
			return foodStoreCount;
		}

		public void IncrementFoodStore(int foodGathered)
		{
			component.IncrementFoodStore(foodGathered);
		}

		public void Starve()
		{
			component.Starve();
			eventBroker.Publish(new MouseDiedEvent());
		}

		public void NewMouse()
		{
			component.NewMouse();
		}

		public void Hunt()
		{
			component.Hunt();
			eventBroker.Publish(new ActionTakenEvent((int)MouseActionTypes.Hunt, GameCharacterTypes.Mouse));
		}

		public void StayAtHome()
		{
			component.StayAtHome();
			eventBroker.Publish(new ActionTakenEvent((int)MouseActionTypes.StayAtHome, GameCharacterTypes.Mouse));
		}

		public MouseData GetMouse()
		{
			return component.GetMouse();
		}
	}


	[Serializable]
	public class MouseData
	{
		public string State { get; set; } = string.Empty;
		public int Food { get; set; } = -1;
	}

}
