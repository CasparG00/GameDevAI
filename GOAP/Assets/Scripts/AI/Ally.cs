using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ally : MonoBehaviour, IGoap
{
    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public Dictionary<string, object> GetWorldData()
    {
        var worldData = new Dictionary<string, object>();
        
        worldData.Add("protectPlayer", false);
        worldData.Add("followPlayer", false);
        worldData.Add("findCover", false);

        return worldData;
    }

    public Dictionary<string, object> CreateGoals()
    {
        var goal = new Dictionary<string, object>();

        goal.Add(Player.instance.gettingAttacked ? "protectPlayer" : "followPlayer", true);

        return goal;
    }

    public MoveState MoveAgent(Action _action)
    {
        var position = _action.target.transform.position;
        agent.SetDestination(position);

        var distance = Vector3.Distance(agent.transform.position, position);
        if (!(distance < 1 + agent.stoppingDistance)) return MoveState.moving;
        
        _action.SetInRange(true);
        return MoveState.inRange;
    }
}
