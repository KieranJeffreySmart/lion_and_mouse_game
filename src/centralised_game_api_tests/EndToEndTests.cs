using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using centralised_game_api;
using lion_and_mouse_game.GameContext;
using lion_and_mouse_game.LionContext;
using lion_and_mouse_game.StoryContext;
using Microsoft.AspNetCore.Mvc.Testing;

namespace centralised_game_api_tests;

public class EndToEndTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly CustomWebApplicationFactory<Program> _factory;

    private const string STORY_TEXT_SIMPLE_WIN = "\r\nOnce upon a time there was a little mouse\r\n\r\n\r\nOn day 1, while searching for something to eat the mouse found a sleeping lion and in her haste returned home with only 1 food\r\n\r\nOn day 2, after searching for something to eat the mouse returned home with 2 food\r\n\r\nOn day 3, after searching for something to eat the mouse returned home with 2 food\r\n\r\nOn day 4, after searching for something to eat the mouse returned home with 2 food\r\n\r\nOn day 5, after searching for something to eat the mouse returned home with 2 food\r\n\r\nOn day 6, after searching for something to eat the mouse returned home with 2 food\r\n\r\nOn day 7, after searching for something to eat the mouse returned home with 2 food";

    public EndToEndTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }
    
    [Fact]
    public async Task DefaultStartingData()
    {
        // Given I have a players name
        var playerName = "bob";

        // When I get the game data
        var client = _factory.CreateClient();
        var options = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IncludeFields = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
            Converters = { new JsonStringEnumConverter() }
        };

        var gameDataResponse = await client.GetAsync($"/game");
        Assert.NotNull(gameDataResponse);
        Assert.True(gameDataResponse.IsSuccessStatusCode);
        var gameData = (await gameDataResponse.Content.ReadFromJsonAsync<GameData>(options)) ?? throw new Exception("Failed to deserialize game data");

        // Then the game data should be set to unsatarted game data
        Assert.NotNull(gameData);
        Assert.Equal(string.Empty, gameData.GameState);
        Assert.Equal(-1, gameData.FinishingFood);
        Assert.Equal(string.Empty, gameData.Accolade);
        
        // When I get the story data
        var unstartedStoryDataResponse = await client.GetAsync($"/story");
        Assert.NotNull(unstartedStoryDataResponse);
        var unstartedStoryData = (await unstartedStoryDataResponse.Content.ReadFromJsonAsync<StoryData>(options)) ?? throw new Exception("Failed to deserialize story data");

        // Then the story data should be set to unsatarted story data
        Assert.Equal(string.Empty, unstartedStoryData.Id);
        Assert.Equal(0, unstartedStoryData.CurrentDay);
        Assert.Equal(string.Empty, unstartedStoryData.StoryText);

        // When I start a new game
        var newGameResponse = await client.PostAsync($"/play?playerName={playerName}", null);
        Assert.NotNull(newGameResponse);
        Assert.True(newGameResponse.IsSuccessStatusCode);
        var newGameResult = (await newGameResponse.Content.ReadFromJsonAsync<NewGameResult>(options)) ?? throw new Exception("Failed to deserialize response body");

        // And get the game data
        var startedGameDataResponse = await client.GetAsync($"/game");
        Assert.NotNull(startedGameDataResponse);
        var startedGameData = (await startedGameDataResponse.Content.ReadFromJsonAsync<GameData>(options)) ?? throw new Exception("Failed to deserialize game data");

        // And get the story data
        var startedStoryDataResponse = await client.GetAsync($"/story");
        Assert.NotNull(startedStoryDataResponse);
        var startedStoryData = (await startedStoryDataResponse.Content.ReadFromJsonAsync<StoryData>(options)) ?? throw new Exception("Failed to deserialize story data");

        // Then the game data should be set to starting values
        Assert.NotNull(newGameResult);
        Assert.NotEqual(Guid.Empty, newGameResult.PlayerId);
        Assert.NotNull(newGameResult.SocketAddress);

        Assert.NotNull(startedGameData);
        Assert.NotEqual(string.Empty, startedGameData.Id);
        Assert.Equal(GameStates.Playing.ToString(), startedGameData.GameState);
        Assert.Equal(-1, startedGameData.FinishingFood);
        Assert.Equal("None", startedGameData.Accolade);
        
        // And the story data should be set to starting values
        Assert.NotEqual(string.Empty, startedStoryData.Id);
        Assert.Equal(1, startedStoryData.CurrentDay);
        Assert.Equal("\r\nOnce upon a time there was a little mouse\r\n", startedStoryData.StoryText);
    }
    
    [Fact]
    public async Task SimpleWinningRunThrough()
    {
        // Given I have a players name
        var playerName = "bob";

        // When I start a game
        var client = _factory.CreateClient();
        var options = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            IncludeFields = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            TypeInfoResolver = new DefaultJsonTypeInfoResolver(),
            Converters = { new JsonStringEnumConverter() }
        };

        var newGameResponse = await client.PostAsync($"/play?playerName={playerName}", null);
        Assert.NotNull(newGameResponse);
        var newGameResult = (await newGameResponse.Content.ReadFromJsonAsync<NewGameResult>(options)) ?? throw new Exception("Failed to deserialize response body");

        // Then I expect to receive initial game data
        Assert.NotEqual(Guid.Empty, newGameResult.PlayerId);
        Assert.NotNull(newGameResult.SocketAddress);

        // Given I hunt every day for 5 days
        var commands = new List<ClientCommand>
        {
            new() { CommandType = GameCommands.MouseHunt, PlayerId = newGameResult.PlayerId },
            new() { CommandType = GameCommands.MouseHunt, PlayerId = newGameResult.PlayerId },
            new() { CommandType = GameCommands.MouseHunt, PlayerId = newGameResult.PlayerId },
            new() { CommandType = GameCommands.MouseHunt, PlayerId = newGameResult.PlayerId },
            new() { CommandType = GameCommands.MouseHunt, PlayerId = newGameResult.PlayerId },
            new() { CommandType = GameCommands.MouseHunt, PlayerId = newGameResult.PlayerId },
            new() { CommandType = GameCommands.MouseHunt, PlayerId = newGameResult.PlayerId }
        };

        // And a the Lion always stays at home
        _factory.SetLionBehavior((lionEngine, gameEvent) => LionBehaviours.GoHome);

        // When I execute the set of commands
        foreach (var command in commands)
        {
            var commandResponse = await client.PostAsJsonAsync("/command", command);
            Assert.NotNull(commandResponse);
            Assert.True(commandResponse.IsSuccessStatusCode);
        }
 
        // Then I should win the game
        var finalGameDataResponse = await client.GetAsync($"/game");
        Assert.NotNull(finalGameDataResponse);
        var finalGameData = (await finalGameDataResponse.Content.ReadFromJsonAsync<GameData>(options)) ?? throw new Exception("Failed to deserialize game data");
        Assert.Equal(GameStates.Won.ToString(), finalGameData.GameState);
        Assert.Equal(15, finalGameData.FinishingFood);
        Assert.Equal("Hoarder", finalGameData.Accolade);

        // And the story should be complete
        var finalStoryDataResponse = await client.GetAsync($"/story");
        Assert.NotNull(finalStoryDataResponse);
        var finalStoryData = (await finalStoryDataResponse.Content.ReadFromJsonAsync<StoryData>(options)) ?? throw new Exception("Failed to deserialize story data");
        Assert.Equal(8, finalStoryData.CurrentDay);
        Assert.Equal(STORY_TEXT_SIMPLE_WIN, finalStoryData.StoryText);
        
        //When I start a new game
        newGameResponse = await client.PostAsync($"/play?playerName={playerName}", null);
        Assert.NotNull(newGameResponse);
        
        // And get the game data
        var startedGameDataResponse = await client.GetAsync($"/game");
        Assert.NotNull(startedGameDataResponse);
        var startedGameData = (await startedGameDataResponse.Content.ReadFromJsonAsync<GameData>(options)) ?? throw new Exception("Failed to deserialize game data");

        // And get the story data
        var startedStoryDataResponse = await client.GetAsync($"/story");
        Assert.NotNull(startedStoryDataResponse);
        var startedStoryData = (await startedStoryDataResponse.Content.ReadFromJsonAsync<StoryData>(options)) ?? throw new Exception("Failed to deserialize story data");

        //Then the game data should be reset
        Assert.NotNull(newGameResult);
        Assert.NotEqual(Guid.Empty, newGameResult.PlayerId);
        Assert.NotNull(newGameResult.SocketAddress);

        Assert.NotNull(startedGameData);
        Assert.NotEqual(string.Empty, startedGameData.Id);
        Assert.Equal(GameStates.Playing.ToString(), startedGameData.GameState);
        Assert.Equal(-1, startedGameData.FinishingFood);
        Assert.Equal("None", startedGameData.Accolade);
        
        // And the story data should be set to starting values
        Assert.NotEqual(string.Empty, startedStoryData.Id);
        Assert.Equal(1, startedStoryData.CurrentDay);
        Assert.Equal("\r\nOnce upon a time there was a little mouse\r\n", startedStoryData.StoryText);
    }
}