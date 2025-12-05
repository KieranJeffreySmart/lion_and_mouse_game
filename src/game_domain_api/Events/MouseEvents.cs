
using game_domain_api.Events;

[Serializable]
public class MouseDayEndedEvent : IGameEvent
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CurrentDay { get; set; }
    public int FoodStored { get; set; }

}

[Serializable]
public class MouseDiedEvent : IGameEvent
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int FoodStored { get; set; }
}