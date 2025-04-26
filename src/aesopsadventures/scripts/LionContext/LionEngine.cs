using System;
using lion_and_mouse_game.Events;

namespace lion_and_mouse_game.LionContext
{
	public class LionEngine : ILionEngine
	{
		private Lion lion = new(LionStates.AtHome);

		public LionEngine()
		{
		}

		public bool IsAtHome => lion?.State == LionStates.AtHome;
		public bool IsHunting => lion?.State == LionStates.Hunting;
		public bool IsSleeping => lion?.State == LionStates.Sleeping;

		public void GoHome()
		{
			lion = lion.SetState(LionStates.AtHome);
		}

		public void Hunt()
		{
			lion = lion.SetState(LionStates.Hunting);
		}

		public void Sleep()
		{
			lion = lion.SetState(LionStates.Sleeping);
		}

		public void NewLion(LionStates lionStartingState)
		{
			lion = new(lionStartingState);
		}

		public LionData GetLion()
		{
			return new()
			{
				State = lion.State.ToString()
			};
		}
	}

	public interface ILionEngine
	{
		public bool IsAtHome {get;}
		public bool IsHunting {get;}
		public bool IsSleeping {get;}

		public void GoHome();

		public void Hunt();

		public void Sleep();

		public void NewLion(LionStates lionStartingState);

		public LionData GetLion();
	}

	public class LionEngineEventDecorator: ILionEngine
	{
		private readonly IEventPub eventBroker;
		private readonly ILionEngine component;

		public LionEngineEventDecorator(IEventPub eventBroker, ILionEngine component)
		{
			this.eventBroker = eventBroker;
			this.component = component;
		}

		public bool IsAtHome => component.IsAtHome;
		public bool IsHunting => component.IsHunting;
		public bool IsSleeping => component.IsSleeping;

		public void GoHome()
		{
			component.GoHome();
			eventBroker.Publish(new ActionTakenEvent((int)LionActionTypes.StayAtHome, GameCharacterTypes.Lion));
		}

		public void Hunt()
		{
			component.Hunt();
			eventBroker.Publish(new ActionTakenEvent((int)LionActionTypes.Hunt, GameCharacterTypes.Lion));
		}

		public void Sleep()
		{
			component.Sleep();
			eventBroker.Publish(new ActionTakenEvent((int)LionActionTypes.Sleep, GameCharacterTypes.Lion));
		}

		public void NewLion(LionStates lionStartingState)
		{
			component.NewLion(lionStartingState);
			if (IsAtHome) eventBroker.Publish(new ActionTakenEvent((int)LionActionTypes.StayAtHome, GameCharacterTypes.Lion));
			else if (IsHunting) eventBroker.Publish(new ActionTakenEvent((int)LionActionTypes.Hunt, GameCharacterTypes.Lion));
			else if (IsSleeping) eventBroker.Publish(new ActionTakenEvent((int)LionActionTypes.Sleep, GameCharacterTypes.Lion));
		}

		public LionData GetLion()
		{
			return component.GetLion();
		}
	}


	[Serializable]
	public class LionData
	{
		public string State { get; set; } = string.Empty;
	}
}
