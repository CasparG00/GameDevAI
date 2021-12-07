using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Guard : MonoBehaviour, IGoap
{
    public Inventory inventory;
    private NavMeshAgent agent;

    private GuardState state = GuardState.Patrolling;

    private Transform player;
    [SerializeField]
    private Transform viewTransform;
    
    private readonly List<GameObject> wayPoints = new List<GameObject>();
    private int visited;

    private void Start()
    {
        if (inventory == null)
        {
            inventory = gameObject.AddComponent<Inventory>();
        }

        agent = GetComponent<NavMeshAgent>();
        player = Player.instance.transform;
        
        var components = FindObjectsOfType<WayPointComponent>();
        if (components != null)
        {
            components = components.OrderBy(_component => _component.order).ToArray();
            foreach (var component in components)
            {
                wayPoints.Add(component.gameObject);
            }
        }
    }

    private void Update()
    {
        switch (state)
        {
            case GuardState.Patrolling:
                break;
            case GuardState.Chasing:
                var distance = Vector3.Distance(transform.position, player.position);
                if (distance < 5f)
                {
                    state = GuardState.Patrolling;
                }
                break;
        }
    }

    public Dictionary<string, object> GetWorldData()
    {
        var worldData = new Dictionary<string, object>();
        
        worldData.Add("hasWeapon", inventory.GetAmount("weapon") > 0);

        return worldData;
    }

    private void Patrol()
    {
        if (visited < wayPoints.Count)
        {
            agent.SetDestination(wayPoints[visited].transform.position);
            var distance = Vector3.Distance(transform.position, agent.destination);
            if (distance < 1)
            {
                visited++;
            }
        }
        else
        {
            visited = 0;
        }
    }

    public Dictionary<string, object> CreateGoals()
    {
        var goal = new Dictionary<string, object>();
        
        goal.Add("attackedPlayer", true);

        return goal;
    }

    public bool MoveAgent(Action _action)
    {
        var position = _action.target.transform.position;
        agent.SetDestination(position);

        var distance = Vector3.Distance(agent.transform.position, position);
        if (distance < 1)
        {
            _action.SetInRange(true);
            return true;
        }
        return false;
    }

    private enum GuardState
    {
        Patrolling,
        Chasing,
    }
}
