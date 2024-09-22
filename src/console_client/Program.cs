using lion_and_mouse_game.Events;
using lion_and_mouse_game.GameContext;
using lion_and_mouse_game.LionContext;
using lion_and_mouse_game.MouseContext;
using lion_and_mouse_game.StoryContext;

GameEventMediator broker = new();
GameEngine gameEngine = new(broker);
StoryEngine storyEngine = new(broker);
MouseEngine mouseEngine = new(broker);
LionEngine lionEngine = new(broker);

broker.Subscribe(new GameEventHandler<MouseDayEndedEvent>((gameEvent) => GamePolicies.IfMouseDayEnded(gameEngine, gameEvent)));
broker.Subscribe(new GameEventHandler<MouseDiedEvent>((gameEvent) => GamePolicies.IfMouseDied(gameEngine, gameEvent)));
broker.Subscribe(new GameEventHandler<NewGameStartedEvent>((gameEvent) => StoryPolicies.IfNewGame(storyEngine, gameEvent)));
broker.Subscribe(new GameEventHandler<ActionTakenEvent>((gameEvent) => StoryPolicies.IfActionTaken(storyEngine, gameEvent)));
broker.Subscribe(new GameEventHandler<NewStoryEvent>((gameEvent) => MousePolicies.IfNewStory(mouseEngine, gameEvent)));
broker.Subscribe(new GameEventHandler<DayEndedEvent>((gameEvent) => MousePolicies.IfDayEnded(mouseEngine, gameEvent)));
broker.Subscribe(new GameEventHandler<MouseReturnedHomeEvent>((gameEvent) => MousePolicies.IfMouseReturned(mouseEngine, gameEvent)));
broker.Subscribe(new GameEventHandler<MouseEatenEvent>((gameEvent) => MousePolicies.IfEaten(mouseEngine, gameEvent)));
broker.Subscribe(new GameEventHandler<NewStoryEvent>((gameEvent) => LionPolicies.IfNewStory(lionEngine, gameEvent)));
broker.Subscribe(new GameEventHandler<NewDayEvent>((gameEvent) => LionPolicies.IfNewDay(lionEngine, gameEvent)));


Console.WriteLine("Welcome to The Lion and Mouse survival game\r\n");

while (true)
{
    if (!gameEngine.IsGameRunning) 
    {
        var game = gameEngine.GetGame();
        if (game.GameState == GameStates.Lost.ToString()) 
        {
            Console.WriteLine("Oh no the mouse died!\r\nGame Over");
        }
        if (game.GameState == GameStates.Won.ToString()) 
        {
            Console.WriteLine("Yay! The mouse survived!\r\nYou Win");   
            Console.WriteLine("Yay! The mouse survived!\r\nYou Win");          
        }
        Console.WriteLine("Would you like to play a new game?");        
        Console.WriteLine("[S]tart                  [Q]uit");
    }
    else
    {
        var story = storyEngine.GetStory();
        var mouse = mouseEngine.GetMouse();
        var lion = lionEngine.GetLion();
        Console.WriteLine(story.StoryText);
        Console.WriteLine($"Day: {story.CurrentDay}");
        Console.WriteLine($"Food: {mouse.Food}");  
        Console.WriteLine($"\r\n------------------------\r\n");  
        Console.WriteLine($"Lion: {lion.State}");    
        Console.WriteLine($"Mouse: {mouse.State}");    
        Console.WriteLine("[S]tay at home           [H]unt");
    }

    var input = Console.ReadKey();
    Console.WriteLine("\r\n");

    if (!gameEngine.IsGameRunning) 
    {
        if (input.KeyChar == 's') gameEngine.New(Guid.NewGuid());
        if (input.KeyChar == 'q') break;
    }
    else
    {
        if (input.KeyChar == 's') mouseEngine.StayAtHome();
        if (input.KeyChar == 'h') mouseEngine.Hunt();
    }
}
