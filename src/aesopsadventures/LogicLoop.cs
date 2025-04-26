

using System;
using System.Collections.Generic;
using lion_and_mouse_game.Events;
using lion_and_mouse_game.GameContext;
using lion_and_mouse_game.LionContext;
using lion_and_mouse_game.MouseContext;
using lion_and_mouse_game.StoryContext;

public class LogicLoop {

	IGameEngine gameEngine;
	IStoryEngine storyEngine;
	IMouseEngine mouseEngine;
	ILionEngine lionEngine;	


	public StoryData Story => storyEngine.GetStory();
	public MouseData Mouse => mouseEngine.GetMouse();
	public LionData Lion => lionEngine.GetLion();
	public GameData Game => gameEngine.GetGame();
	private bool eventDriven = true;


    public void Init()
    {
		gameEngine = new GameEngine();
		storyEngine = new StoryEngine();
		mouseEngine = new MouseEngine();
		lionEngine = new LionEngine();
    }
    
    public void InitEventDriven(GameEventMediator broker) 
    {
        this.Init();        

		if (eventDriven)
			broker.Subscribe(new GameEventHandler<MouseDayEndedEvent>((gameEvent) => GamePolicies.IfMouseDayEnded(gameEngine, gameEvent.CurrentDay, gameEvent.FoodStored)));
			broker.Subscribe(new GameEventHandler<MouseDiedEvent>((gameEvent) => GamePolicies.IfMouseDied(gameEngine)));
			broker.Subscribe(new GameEventHandler<NewGameStartedEvent>((gameEvent) => StoryPolicies.IfNewGame(storyEngine, gameEvent.LionStartingState)));
			broker.Subscribe(new GameEventHandler<ActionTakenEvent>((gameEvent) => StoryPolicies.IfActionTaken(storyEngine, gameEvent.CharacterType, gameEvent.ActionType)));
			broker.Subscribe(new GameEventHandler<NewStoryEvent>((gameEvent) => MousePolicies.IfNewStory(mouseEngine)));
			broker.Subscribe(new GameEventHandler<DayEndedEvent>((gameEvent) => MousePolicies.IfDayEnded(mouseEngine, gameEvent.CurrentDay)));
			broker.Subscribe(new GameEventHandler<MouseReturnedHomeEvent>((gameEvent) => MousePolicies.IfMouseReturned(mouseEngine, gameEvent.FoodGathered)));
			broker.Subscribe(new GameEventHandler<MouseEatenEvent>((gameEvent) => MousePolicies.IfEaten(mouseEngine)));
			broker.Subscribe(new GameEventHandler<NewStoryEvent>((gameEvent) => LionPolicies.IfNewStory(lionEngine, gameEvent.LionStartingState)));
			broker.Subscribe(new GameEventHandler<NewDayEvent>((gameEvent) => LionPolicies.IfNewDay(lionEngine)));
    }

    public GameData GetGame()
    {
        return gameEngine.GetGame();
    }

    internal void NewGame(Guid guid)
    {
        gameEngine.New(guid);

		if (!eventDriven)
		{
			StoryPolicies.IfNewGame(storyEngine, LionStates.Sleeping);
			MousePolicies.IfNewStory(mouseEngine);
			LionPolicies.IfNewStory(lionEngine, LionStates.Sleeping);
		}
    }

    internal void MouseStayAtHome()
    {
        mouseEngine.StayAtHome();

		if (!eventDriven)
		{
			StoryPolicies.IfActionTaken(storyEngine, GameCharacterTypes.Mouse, (int)MouseActionTypes.StayAtHome);
		}
		
    }

    internal void MouseHunt()
    {
        mouseEngine.Hunt();
		
		if (!eventDriven)
		{
			StoryPolicies.IfActionTaken(storyEngine, GameCharacterTypes.Mouse, (int)MouseActionTypes.Hunt);
		}
    }

    internal IReadOnlyDictionary<StoryOptions, StoryOption> GetAllStoryOptions()
    {
        return storyEngine.GetAllStoryOptions();
    }
}