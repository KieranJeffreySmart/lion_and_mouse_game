public class NewGameResult
{
    public Uri SocketAddress { get; }
    public Guid PlayerId { get; }

    public NewGameResult(Uri socketAddress, Guid playerId)
    {
        SocketAddress = socketAddress;
        PlayerId = playerId;
    }

    public override bool Equals(object? obj)
    {
        return obj is NewGameResult other &&
               SocketAddress == other.SocketAddress &&
               PlayerId == other.PlayerId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(SocketAddress, PlayerId);
    }
}