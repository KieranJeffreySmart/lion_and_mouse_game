public enum LionActionTypes
{
    StayAtHome,
    Hunt,
    Sleep
}

public enum MouseActionTypes
{
    StayAtHome,
    Hunt
}

public enum GameCharacterTypes
{
    Lion,
    Mouse
}

public enum MouseStates
{
    AtHome = 0,
    Hunting,
    Dead
}

public enum LionStates
{
    AtHome,
    Hunting,
    Sleeping
}

public enum Accolades
{
    None,
    Survivor,
    Hoarder,
    Desperado
}

public enum GameStates
{
    Playing,
    Lost,
    Won
}