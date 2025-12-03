using System.Collections.Concurrent;

namespace lion_and_mouse_game.PlayerContext
{
    public class PlayerStore: IPlayerStore
    {
        private readonly ConcurrentDictionary<Guid, Player> _players = new();

        public void AddPlayer(Player player)
        {
            if (player == null)
                throw new ArgumentNullException(nameof(player));

            _players[player.Id] = player;
        }

        public Player? GetPlayer(Guid playerId)
        {
            if (playerId == Guid.Empty)
                throw new ArgumentNullException(nameof(playerId));

            return _players.TryGetValue(playerId, out var player) ? player : null;
        }

        public IEnumerable<Player> GetAllPlayers()
        {
            return _players.Values.ToList();
        }

        public bool RemovePlayer(Guid playerId)
        {
            if (playerId == Guid.Empty)
                throw new ArgumentNullException(nameof(playerId));

            return _players.TryRemove(playerId, out _);
        }

        public void Clear()
        {
            _players.Clear();
        }

        public int Count => _players.Count;
    }
}