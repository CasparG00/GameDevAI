using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Ally : MonoBehaviour, IGoap
{
    public Inventory inventory;
    private NavMeshAgent agent;

    private Transform player;
    [SerializeField] private Transform viewTransform;
    [SerializeField] private float followDistance = 5f;
    
    private void Start()
    {
        if (GetComponent<Inventory>() == null)
        {
            inventory = gameObject.AddComponent<Inventory>();
        }

        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = followDistance;
        
        player = Player.instance.transform;
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
        if (!(distance < agent.stoppingDistance)) return MoveState.moving;
        
        _action.SetInRange(true);
        return MoveState.inRange;
    }
}
