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
    public async Task SimpleWinningRunThroughAndRestart()
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
        Assert.Equal(GameRunTestData.STORY_TEXT_SIMPLE_WIN, finalStoryData.StoryText);
        
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


    [Theory]
    [InlineData("Bob", "SimpleWinScenario")]
    public async Task GameScenarioRunThroughs(string playerName, string scenario)
    {
        if (!GameRunTestData.Scenarios.ContainsKey(scenario)) throw new Exception($"Scenario {scenario} not found");
        var scenarioData = GameRunTestData.Scenarios[scenario];
        var client = _factory.CreateClient();
        _factory.SetLionBehavior(scenarioData.LionBehavior);
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
        Assert.NotEqual(Guid.Empty, newGameResult.PlayerId);
        
        foreach (var command in scenarioData.GetCommands(newGameResult.PlayerId))
        {
            var commandResponse = await client.PostAsJsonAsync("/command", command);
            Assert.NotNull(commandResponse);
            Assert.True(commandResponse.IsSuccessStatusCode);
        }
        
        var finalGameResponse = await client.GetAsync($"/game");
        Assert.NotNull(finalGameResponse);
        var finalGameData = (await finalGameResponse.Content.ReadFromJsonAsync<GameData>(options)) ?? throw new Exception("Failed to deserialize final game data");
        Assert.Equal(scenarioData.ExpectedGameDataAfterCommands.GameState, finalGameData.GameState);
        Assert.Equal(scenarioData.ExpectedGameDataAfterCommands.FinishingFood, finalGameData.FinishingFood);
        Assert.Equal(scenarioData.ExpectedGameDataAfterCommands.Accolade, finalGameData.Accolade);

        var finalStoryResponse = await client.GetAsync($"/story");
        Assert.NotNull(finalStoryResponse);
        var finalStoryData = (await finalStoryResponse.Content.ReadFromJsonAsync<StoryData>(options)) ?? throw new Exception("Failed to deserialize final story data");
        Assert.Equal(scenarioData.ExpectedStoryDataAfterCommands.CurrentDay, finalStoryData.CurrentDay);
        Assert.Equal(scenarioData.ExpectedStoryDataAfterCommands.StoryText, finalStoryData.StoryText);
    }
}
