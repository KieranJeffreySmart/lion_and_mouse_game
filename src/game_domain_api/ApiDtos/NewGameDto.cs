using game_domain_api.GameContext;

namespace game_domain_api.ApiDtos;

public class NewGameDto
{
    public Guid PlayerId { get; set; }
}

public class NewGameResultDto
{
    public Guid GameId { get; set; }
    public GameStates GameState { get; set; }
}