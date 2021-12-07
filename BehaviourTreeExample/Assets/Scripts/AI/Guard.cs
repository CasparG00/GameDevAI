using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Guard : MonoBehaviour, IGoap
{
    public Inventory inventory;
    private NavMeshAgent agent;

    private Transform player;
    [SerializeField] private Transform viewTransform;
    [SerializeField] private float maxChaseRange = 5;

    private void Start()
    {
        if (GetComponent<Inventory>() == null)
        {
            inventory = gameObject.AddComponent<Inventory>();
        }

        agent = GetComponent<NavMeshAgent>();
        player = Player.instance.transform;
    }

    public Dictionary<string, object> GetWorldData()
    {
        var worldData = new Dictionary<string, object>();
        
        worldData.Add("hasWeapon", inventory.GetAmount("weapon") > 0);
        worldData.Add("playerFound", false);

        return worldData;
    }

    // private void Patrol()
    // {
    //     if (visited < wayPoints.Count)
    //     {
    //         agent.SetDestination(wayPoints[visited].transform.position);
    //         var distance = Vector3.Distance(transform.position, agent.destination);
    //         if (distance < 1)
    //         {
    //             visited++;
    //         }
    //     }
    //     else
    //     {
    //         visited = 0;
    //     }
    // }

    public Dictionary<string, object> CreateGoals()
    {
        var goal = new Dictionary<string, object>();
        
        goal.Add("attackedPlayer", true);

        return goal;
    }

    public MoveState MoveAgent(Action _action)
    {
        var position = _action.target.transform.position;
        agent.SetDestination(position);

        var distance = Vector3.Distance(agent.transform.position, position);
        if (distance < 1)
        {
            _action.SetInRange(true);
            return MoveState.inRange;
        }

        if (distance > maxChaseRange && _action.target == player.gameObject)
        {
            return MoveState.unreachable;
        }

        return MoveState.moving;
    }
}
