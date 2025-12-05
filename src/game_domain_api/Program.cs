using game_domain_api.ApiDtos;
using game_domain_api.Events;
using game_domain_api.GameContext;
using game_domain_api.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace game_domain_api
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var inMemoryEventMediator = new GameEventMediator();
            builder.Services.AddSingleton<IEventPub>(inMemoryEventMediator);

            var connectionType = Environment.GetEnvironmentVariable("DB_CONNECTION_TYPE") ?? string.Empty;
            var connectionName = Environment.GetEnvironmentVariable("DB_CONNECTION_NAME") ?? "gameDatadb";
            if (connectionType == "postgresdb")
            {
                builder.AddNpgsqlDbContext<GameDbContext>(connectionName: connectionName);
            }
            else
            {
                builder.Services.AddDbContext<GameDbContext>(options => options.UseInMemoryDatabase(connectionName));
            }

            builder.Services.AddScoped<IGameDataRepository, GameDataRepository>();
            builder.Services.AddScoped<IGameEngine, GameEngine>();
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.MapPost("/game", async ([FromBody] NewGameDto newGameDto, [FromServices] IGameEngine gameEngine) =>
            {
                await gameEngine.New(newGameDto.PlayerId);
                var gameData = gameEngine.GetGame();
                return new NewGameResultDto
                {
                    GameId = gameData.Id,
                    GameState = gameData.GameState
                };
            });

            app.MapGet("/game/{gameId}", async (Guid gameId, [FromServices] IGameEngine gameEngine) =>
            {
                await gameEngine.LoadGameById(gameId);
                return gameEngine.GetGame();
            })
            .WithName("GetGame")
            .WithOpenApi();

            app.Run();
        }
    }
}