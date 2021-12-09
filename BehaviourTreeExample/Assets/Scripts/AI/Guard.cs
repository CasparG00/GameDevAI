using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Guard : MonoBehaviour, IGoap
{
    public Inventory inventory;
    private NavMeshAgent agent;

    private Player player;
    [SerializeField] private Transform viewTransform;
    [SerializeField] private float maxChaseRange = 5;
    [SerializeField] private float stunTime = 2;

    private bool isRunning;

    private void Start()
    {
        if (GetComponent<Inventory>() == null)
        {
            inventory = gameObject.AddComponent<Inventory>();
        }

        agent = GetComponent<NavMeshAgent>();
        player = Player.instance;
    }

    public Dictionary<string, object> GetWorldData()
    {
        var worldData = new Dictionary<string, object>();
        
        worldData.Add("hasWeapon", inventory.GetAmount("weapon") > 0);
        worldData.Add("playerFound", false);

        return worldData;
    }

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
        if (distance < 1 + agent.stoppingDistance)
        {
            _action.SetInRange(true);
            return MoveState.inRange;
        }

        if (distance > maxChaseRange && _action.target == player.gameObject)
        {
            player.Escape();
            return MoveState.unreachable;
        }

        return MoveState.moving;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Smoke Bomb"))
        {
            player.Escape();
            if (!isRunning)
            {
                StartCoroutine(Stun());
            }
        }
    }

    private IEnumerator Stun()
    {
        isRunning = true;
        
        var originalSpeed = agent.speed;

        agent.speed = 0;

        yield return new WaitForSeconds(stunTime);

        agent.speed = originalSpeed;

        isRunning = false;
    }
}
