using System.Net.Http.Json;
using game_domain_api.GameContext;
using game_domain_api.ApiDtos;
using game_domain_api;

namespace game_domain_api_tests;

public class GameApiEndToEndTests(CustomWebApplicationFactory<Program> factory) : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory = factory;

    [Fact]
    public async Task CreateANewGame()
    {  
        var client = _factory.CreateClient();

        // Given I have a player Id
        var playerId = Guid.NewGuid();

        // When I create a new game
        var response = await client.PostAsJsonAsync($"/game", new NewGameDto { PlayerId = playerId });
        response.EnsureSuccessStatusCode();

        // Then I receive a game Id and the game state
        var newGameResult = await response.Content.ReadFromJsonAsync<NewGameResultDto>();
        Assert.NotNull(newGameResult);
        Assert.NotEqual(Guid.Empty, newGameResult.GameId);
        Assert.Equal(GameStates.Playing, newGameResult.GameState);

        // Given I have a game Id
        var gameId = newGameResult.GameId;

        // When I get the game
        var gameResponse = await client.GetAsync($"/game/{gameId}");
        gameResponse.EnsureSuccessStatusCode();

        // Then the game is associated with the player Id and is in Playing state
        var gameData = await gameResponse.Content.ReadFromJsonAsync<GameData>();
        Assert.NotNull(gameData);
        Assert.Equal(gameId, gameData.Id);
        Assert.Equal(GameStates.Playing, gameData.GameState);
    }
}
