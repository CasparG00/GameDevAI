using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Patrol : Action
{
    private Player player;
    private bool playerFound;

    private List<Vector3> wayPoints = new List<Vector3>();
    private int visited;
    
    public Patrol()
    {
        AddPrecondition("playerFound", false);
        AddEffect("playerFound", true);
    }
    
    public override bool IsAchievable(NavMeshAgent _agent)
    {
        player = Player.instance;

        var points = FindObjectsOfType<WayPointComponent>();

        points = points.OrderBy(_point => _point.order).ToArray();
        foreach (var point in points)
        {
            wayPoints.Add(point.transform.position);
        }
        
        return player != null;
    }

    public override bool PerformAction(NavMeshAgent _agent)
    {
        var tf = _agent.transform;
        var direction = player.transform.position - tf.position;
        var angle = Vector3.Angle(tf.forward, direction);
        if (angle < 30)
        {
            if (Physics.Raycast(tf.position, direction, out var hit, 5))
            {
                var damageable = hit.transform.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    playerFound = true;
                    player.GetAttacked();
                }
            }
        }
        
        if (visited < wayPoints.Count)
        {
            _agent.SetDestination(wayPoints[visited]);
            var distance = Vector3.Distance(_agent.transform.position, _agent.destination);
            if (distance < 1)
            {
                visited++;
            }
        }
        else
        {
            visited = 0;
        }
        
        return true;
    }

    public override bool IsCompleted()
    {
        return playerFound;
    }

    public override bool RequiresInRange()
    {
        return false;
    }

    public override void Reset()
    {
        playerFound = false;
        visited = 0;
        wayPoints.Clear();
    }
}
