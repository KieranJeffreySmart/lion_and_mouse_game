using game_domain_api.ApiDtos;
using game_domain_api.GameContext;

public class Program
{
    private static void Main(string[] args)
    {
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

        app.MapPost("/game", async (NewGameDto newGameDto) =>
        {
            return new NewGameResultDto
            {
                GameId = Guid.NewGuid(),
                GameState = GameStates.Playing
            };
        });

        app.MapGet("/game/{gameId}", async (Guid gameId) =>
        {
            return new GameData();
        })
        .WithName("GetGame")
        .WithOpenApi();

        app.Run();
    }
}
