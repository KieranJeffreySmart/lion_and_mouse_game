using System.Text.Json;
using centralised_game_api;
using Fleck;
using lion_and_mouse_game.Events;
using lion_and_mouse_game.GameContext;
using lion_and_mouse_game.LionContext;
using lion_and_mouse_game.MouseContext;
using lion_and_mouse_game.PlayerContext;
using lion_and_mouse_game.StoryContext;
using Microsoft.AspNetCore.Mvc;


public class Program
{
    public static void Main(string[] args)
    {
        var  LocalhostAllowSpecificOrigins = "_localhostAllowSpecificOrigins";
        var builder = WebApplication.CreateBuilder(args);
        
        List<IWebSocketConnection> wsConnections = new();

        GameEventMediator eventMediator = new();

        builder.Services.AddSingleton<IEventMediator>(eventMediator);
        builder.Services.AddSingleton<IEventSub>(eventMediator);

        builder.Services.AddSingleton<IEventPub>(sp =>
        {
            return new WebSocketGameEventBroadcaster(sp.GetRequiredService<IEventMediator>(), (gameEvent) => BroadcastGameEvent(wsConnections, gameEvent));
        });
        builder.Services.AddSingleton<EngineFactory>();
        builder.Services.AddSingleton<ILionBehaviorCalculator, DefaultLionBehaviorCalculator>();
        builder.Services.AddSingleton(sp =>
        {
            return new LionPolicies(sp.GetRequiredService<ILionBehaviorCalculator>(), sp.GetRequiredService<EngineFactory>().GetLionEngine());
        });
        builder.Services.AddSingleton(sp =>
        {
            var engineFactory = sp.GetRequiredService<EngineFactory>();
            return new CommandHandler(engineFactory.GetGameEngine(), engineFactory.GetMouseEngine());
        });

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(name: LocalhostAllowSpecificOrigins,
                            policy  =>
                            {
                                policy.WithOrigins("http://localhost:3000");
                            });
        });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.UseCors(LocalhostAllowSpecificOrigins);
        }

        app.UseHttpsRedirection();

        app.MapGet("/game", ([FromServices] EngineFactory engineFactory) =>
        {
            return engineFactory.GetGameEngine().GetGame();
        })
        .WithName("GetGame")
        .WithOpenApi();
        
        app.MapGet("/story", ([FromServices] EngineFactory engineFactory) =>
        {
            return engineFactory.GetStoryEngine().GetStory();
        })
        .WithName("GetStory")
        .WithOpenApi();
        
        
        app.MapGet("/mouse", ([FromServices] EngineFactory engineFactory) =>
        {
            return engineFactory.GetMouseEngine().GetMouse();
        })
        .WithName("GetMouse")
        .WithOpenApi();

        PlayerStore playerStore = new();
        var webSocketUri = new Uri("ws://127.0.0.1:8001");

        app.MapPost("/play", ([FromQuery] string playerName, [FromServices] EngineFactory engineFactory) =>
        {
            var player = playerStore.GetAllPlayers().FirstOrDefault(p => p.Name == playerName);

            if (player == null)
            {
                player = new Player(playerName);
                playerStore.AddPlayer(player);
            }            
            
            if (!engineFactory.GetGameEngine().IsGameRunning) 
            {
                engineFactory.GetGameEngine().New(player.Id);
            }

            return new NewGameResult(webSocketUri, player.Id);
        })
        .WithName("NewGame")
        .WithOpenApi();

        app.MapPost("/command", ([FromBody] ClientCommand command, [FromServices] CommandHandler commandHandler) =>
        {
            commandHandler.Handle(command);
        })
        .WithName("NewCommand")
        .WithOpenApi();

        WebSocketServer server = new(webSocketUri.ToString());
        var commandHandler = app.Services.GetRequiredService<CommandHandler>();
        server.Start((connection) => ConfigWebsocketServer(commandHandler, connection, (conn) => wsConnections.Add(conn)));

        SubscribeToEvents(eventMediator, app.Services.GetRequiredService<LionPolicies>(), app.Services.GetRequiredService<EngineFactory>());

        app.Run();
    }

    private static void SubscribeToEvents(GameEventMediator eventMediator, LionPolicies lionPolicies, EngineFactory engineFactory)
    {
        var gameEngine = engineFactory.GetGameEngine();
        var mouseEngine = engineFactory.GetMouseEngine();
        var storyEngine = engineFactory.GetStoryEngine();
        eventMediator.Subscribe(new GameEventHandler<MouseDayEndedEvent>((gameEvent) => GamePolicies.IfMouseDayEnded(gameEngine, gameEvent)));
        eventMediator.Subscribe(new GameEventHandler<MouseDiedEvent>((gameEvent) => GamePolicies.IfMouseDied(gameEngine, gameEvent)));
        eventMediator.Subscribe(new GameEventHandler<NewGameStartedEvent>((gameEvent) => StoryPolicies.IfNewGame(storyEngine, gameEvent)));
        eventMediator.Subscribe(new GameEventHandler<ActionTakenEvent>((gameEvent) => StoryPolicies.IfActionTaken(storyEngine, gameEvent)));
        eventMediator.Subscribe(new GameEventHandler<NewStoryEvent>((gameEvent) => MousePolicies.IfNewStory(mouseEngine, gameEvent)));
        eventMediator.Subscribe(new GameEventHandler<DayEndedEvent>((gameEvent) => MousePolicies.IfDayEnded(mouseEngine, gameEvent)));
        eventMediator.Subscribe(new GameEventHandler<MouseReturnedHomeEvent>((gameEvent) => MousePolicies.IfMouseReturned(mouseEngine, gameEvent)));
        eventMediator.Subscribe(new GameEventHandler<MouseEatenEvent>((gameEvent) => MousePolicies.IfEaten(mouseEngine, gameEvent)));
        eventMediator.Subscribe(new GameEventHandler<NewStoryEvent>(lionPolicies.IfNewStory));
        eventMediator.Subscribe(new GameEventHandler<NewDayEvent>(lionPolicies.IfNewDay));
    }

    private static void BroadcastGameEvent(List<IWebSocketConnection> wsConnections, IGameEvent gameEvent)
    {
        foreach (var connection in wsConnections)
        {
            connection.Send(JsonSerializer.Serialize(gameEvent));
        }
    }

    private static void ConfigWebsocketServer(CommandHandler commandHandler, IWebSocketConnection connection, Action<IWebSocketConnection> addConnection)
    {
        connection.OnOpen = () => addConnection(connection);
        connection.OnMessage = (message) => HandleClientMessage(commandHandler, message);
    }

    private static void HandleClientMessage(CommandHandler commandHandler, string message)
    {
        var command = JsonSerializer.Deserialize<ClientCommand>(message);

        if (command == null)
            return;

        commandHandler.Handle(command);
    }
}
