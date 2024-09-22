using System.Text.Json;
using Fleck;
using lion_and_mouse_game.Events;
using lion_and_mouse_game.GameContext;
using lion_and_mouse_game.LionContext;
using lion_and_mouse_game.MouseContext;
using lion_and_mouse_game.StoryContext;


internal class Program
{
    private static void Main(string[] args)
    {
        List<IWebSocketConnection> wsConnections = new();
        GameEventMediator broker = new();
        WebSocketGameEventBroadcaster broadcaster = new WebSocketGameEventBroadcaster(broker, (gameEvent) => BroadcastGameEvent(wsConnections, gameEvent));
        GameEngine gameEngine = new(broadcaster);
        StoryEngine storyEngine = new(broadcaster);
        MouseEngine mouseEngine = new(broadcaster);
        LionEngine lionEngine = new();

        broker.Subscribe(new GameEventHandler<MouseDayEndedEvent>((gameEvent) => GamePolicies.IfMouseDayEnded(gameEngine, gameEvent)));
        broker.Subscribe(new GameEventHandler<MouseDiedEvent>((gameEvent) => GamePolicies.IfMouseDied(gameEngine, gameEvent)));
        broker.Subscribe(new GameEventHandler<NewGameStartedEvent>((gameEvent) => StoryPolicies.IfNewGame(storyEngine, gameEvent)));
        broker.Subscribe(new GameEventHandler<ActionTakenEvent>((gameEvent) => StoryPolicies.IfActionTaken(storyEngine, gameEvent)));
        broker.Subscribe(new GameEventHandler<NewStoryEvent>((gameEvent) => MousePolicies.IfNewStory(mouseEngine, gameEvent)));
        broker.Subscribe(new GameEventHandler<DayEndedEvent>((gameEvent) => MousePolicies.IfDayEnded(mouseEngine, gameEvent)));
        broker.Subscribe(new GameEventHandler<MouseReturnedHomeEvent>((gameEvent) => MousePolicies.IfMouseReturned(mouseEngine, gameEvent)));
        broker.Subscribe(new GameEventHandler<MouseEatenEvent>((gameEvent) => MousePolicies.IfEaten(mouseEngine, gameEvent)));
        broker.Subscribe(new GameEventHandler<NewStoryEvent>((gameEvent) => LionPolicies.IfNewStory(lionEngine, gameEvent)));
        broker.Subscribe(new GameEventHandler<DayEndedEvent>((gameEvent) => LionPolicies.IfNewDay(lionEngine, gameEvent)));

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        var app = builder.Build();

                // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.MapGet("/game", () =>
        {
            return gameEngine.GetGame();
        })
        .WithName("GetGame")
        .WithOpenApi();
        
        app.MapGet("/story", () =>
        {
            return storyEngine.GetStory();
        })
        .WithName("GetStory")
        .WithOpenApi();
        
        
        app.MapGet("/mouse", () =>
        {
            return mouseEngine.GetMouse();
        })
        .WithName("GetMouse")
        .WithOpenApi();

        app.MapPost("/play", (Guid playerId) =>
        {
            var playerType = PlayerTypes.observer;
            if (!gameEngine.IsGameRunning) 
            {
                gameEngine.New(playerId);
                playerType = PlayerTypes.mouse;
            }

            return new { SocketAddress = "ws://0.0.0.0:8181", PlayerType = playerType.ToString() };
        })
        .WithName("NewGame")
        .WithOpenApi();

        WebSocketServer server = new("ws://0.0.0.0:8181");

        server.Start((connection) => ConfigWebsocketServer(gameEngine, mouseEngine, connection, (conn) => wsConnections.Add(conn)));
    }

    private static void BroadcastGameEvent(List<IWebSocketConnection> wsConnections, IGameEvent gameEvent)
    {
        foreach (var connection in wsConnections)
        {
            connection.Send(JsonSerializer.Serialize(gameEvent));
        }
    }

    private static void ConfigWebsocketServer(GameEngine gameEngine, MouseEngine mouseEngine, IWebSocketConnection connection, Action<IWebSocketConnection> addConnection)
    {
        connection.OnOpen = () => addConnection(connection);
        connection.OnMessage = (message) => HandleClientMessage(gameEngine, mouseEngine, message);
    }

    private static void HandleClientMessage(GameEngine gameEngine, MouseEngine mouseEngine, string message)
    {
        var command = JsonSerializer.Deserialize<ClientCommand>(message);

        if (command is null) return;

        if (command.CommandType == GameCommands.MouseHunt)
        {
            if (gameEngine.IsGameRunning && gameEngine.CurrentPlayerId == command.PlayerId) mouseEngine.Hunt();
        }
        
        if (command.CommandType == GameCommands.MouseStayAtHome)
        {
            if (gameEngine.IsGameRunning && gameEngine.CurrentPlayerId == command.PlayerId) mouseEngine.StayAtHome();
        }
    }

    private class ClientCommand
    {
        public GameCommands CommandType { get; internal set; }
        public Guid PlayerId { get; internal set; }
    }

    private enum GameCommands
    {
        MouseHunt,
        MouseStayAtHome
    }

    private enum PlayerTypes
    {
        mouse,
        observer
    }
}