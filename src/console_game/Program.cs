// See https://aka.ms/new-console-template for more information
using Lion_and_mouse.src.Events;
using Lion_and_mouse.src.GameContext;
using Lion_and_mouse.src.LionContext;
using Lion_and_mouse.src.MouseContext;
using Lion_and_mouse.src.StoryContext;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;


GameEventMediator broker = new();
GameEngine gameEngine = new(broker);
StoryEngine storyEngine = new(broker);
MouseEngine mouseEngine = new(broker);
LionEngine lionEngine = new();


WindowOptions options = WindowOptions.Default with
{
    Size = new Vector2D<int>(800, 600),
    Title = "The Lion and The Mouse Survival"
};
IWindow window = Window.Create(options);

window.Load += OnLoad;
window.Render += OnRender;

void OnRender(double timeDelta)
{
    //
}

void OnLoad()
{

    IInputContext input = window.CreateInput();
    for (int i = 0; i < input.Keyboards.Count; i++)
        input.Keyboards[i].KeyDown += KeyDown;

    broker.Subscribe(new GameEventHandler<MouseDayEndedEvent>((gameEvent) => GamePolicies.IfMouseDayEnded(gameEngine, gameEvent)));
    broker.Subscribe(new GameEventHandler<MouseDiedEvent>((gameEvent) => GamePolicies.IfMouseDied(gameEngine, gameEvent)));
    broker.Subscribe(new GameEventHandler<NewGameStartedEvent>((gameEvent) => StoryPolicies.IfNewGame(storyEngine, gameEvent)));
    broker.Subscribe(new GameEventHandler<ActionTakenEvent>((gameEvent) => StoryPolicies.IfActionTaken(storyEngine, gameEvent)));
    broker.Subscribe(new GameEventHandler<NewStoryEvent>((gameEvent) => MousePolicies.IfNewStory(mouseEngine, gameEvent)));
    broker.Subscribe(new GameEventHandler<DayEndedEvent>((gameEvent) => MousePolicies.IfDayEnded(mouseEngine, gameEvent)));
    broker.Subscribe(new GameEventHandler<MouseReturnedHomeEvent>((gameEvent) => MousePolicies.IfMouseReturned(mouseEngine, gameEvent)));
    broker.Subscribe(new GameEventHandler<MouseEatenEvent>((gameEvent) => MousePolicies.IfEaten(mouseEngine, gameEvent)));
    broker.Subscribe(new GameEventHandler<NewStoryEvent>((gameEvent) => LionPolicies.IfNewStory(lionEngine, gameEvent)));
    broker.Subscribe(new GameEventHandler<DayEndedEvent>((gameEvent) => LionPolicies.IfDayEnded(lionEngine, gameEvent)));
}

void KeyDown(IKeyboard keyboard, Key key, int arg3)
{
    if (key == Key.Q) window.Close();    
    if (!gameEngine.IsGameRunning && key == Key.N) { gameEngine.New(Guid.NewGuid()); }
    if (gameEngine.IsGameRunning && key == Key.H) { mouseEngine.Hunt(); }
    if (gameEngine.IsGameRunning && key == Key.S) { mouseEngine.StayAtHome(); }
}

window.Closing += OnClosing;

void OnClosing()
{
    Console.WriteLine("Thank you for playing. Goodbye!");
}

window.Run();
