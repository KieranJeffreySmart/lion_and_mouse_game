using lion_and_mouse_game.Events;
using lion_and_mouse_game.GameContext;
using lion_and_mouse_game.LionContext;
using lion_and_mouse_game.MouseContext;
using lion_and_mouse_game.StoryContext;

internal class rest_Program
{
    private static void Main(string[] args)
    {
        GameEventMediator broker = new();
        GameEngine gameEngine = new(broker);
        StoryEngine storyEngine = new(broker);
        MouseEngine mouseEngine = new(broker);
        LionEngine lionEngine = new();


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

        app.MapGet("/game/status", () =>
        {
            return gameEngine.IsGameRunning ? "Running" : "Stopped";
        })
        .WithName("GetGameStatus")
        .WithOpenApi();

        app.MapPost("/game/new", (Guid playerId) =>
        {
            gameEngine.New(playerId);
        })
        .WithName("NewGame")
        .WithOpenApi();

        app.MapPost("/mouse/stayathome", () =>
        {
            mouseEngine.StayAtHome();
        })
        .WithName("MouseStayAtHome")
        .WithOpenApi();
        

        app.MapPost("/mouse/hunt", () =>
        {
            mouseEngine.Hunt();
        })
        .WithName("MouseHunt")
        .WithOpenApi();



        app.Run();
    }
}