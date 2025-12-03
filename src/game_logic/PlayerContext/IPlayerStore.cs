namespace lion_and_mouse_game.PlayerContext
{
    public interface IPlayerStore
    {
        void AddPlayer(Player player);
        Player? GetPlayer(Guid playerId);
        IEnumerable<Player> GetAllPlayers();
        bool RemovePlayer(Guid playerId);
        void Clear();
        int Count { get; }
    }
}