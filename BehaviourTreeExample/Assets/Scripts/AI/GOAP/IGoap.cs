using System.Collections.Generic;
using UnityEngine.AI;

public interface IGoap
{
    public Dictionary<string, object> GetWorldData();
    public Dictionary<string, object> CreateGoals();
    public bool MoveAgent(Action _action);
}