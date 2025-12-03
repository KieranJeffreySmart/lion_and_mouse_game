using System;

namespace lion_and_mouse_game.PlayerContext
{
    public class Player
    {
        public Guid Id { get; }
        public string Name { get; }

        public Player(string name) : this(Guid.NewGuid(), name) { }

        public Player(Guid id, string name)
        {
            if (id == Guid.Empty) throw new ArgumentException("Id cannot be an empty GUID.", nameof(id));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name cannot be null or whitespace.", nameof(name));

            Id = id;
            Name = name;
        }

        public override string ToString() => $"{Name} ({Id})";
    }
}