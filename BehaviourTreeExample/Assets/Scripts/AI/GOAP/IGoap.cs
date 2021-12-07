using System.Collections.Generic;

public interface IGoap
{
    public Dictionary<string, object> GetWorldData();
    public Dictionary<string, object> CreateGoals();
    public MoveState MoveAgent(Action _action);
}

public enum MoveState
{
    moving,
    inRange,
    unreachable,
}