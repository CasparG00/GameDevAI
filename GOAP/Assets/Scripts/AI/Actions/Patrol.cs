using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Patrol : Action
{
    private Player player;

    private List<Vector3> wayPoints = new List<Vector3>();
    private int visited;
    
    public Patrol()
    {
        AddPrecondition("playerFound", false);
        AddEffect("playerFound", true);
    }
    
    public override bool IsAchievable(GameObject _agent)
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

    public override bool PerformAction(GameObject _agent)
    {
        var agent = _agent.GetComponent<NavMeshAgent>();
        var view = _agent.GetComponentInChildren<ViewTransform>().transform;
        
        var direction = player.transform.position - view.position;
        var angle = Vector3.Angle(view.forward, direction);
        if (angle < 30)
        {
            if (Physics.Raycast(view.position, direction, out var hit, 5))
            {
                var damageable = hit.transform.GetComponent<IDamageable>();
                if (damageable != null)
                {
                    isCompleted = true;
                    player.GetAttacked();
                }
            }
        }
        
        if (visited < wayPoints.Count)
        {
            agent.SetDestination(wayPoints[visited]);
            var distance = Vector3.Distance(_agent.transform.position, agent.destination);
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

    public override bool RequiresInRange()
    {
        return false;
    }

    public override void Reset()
    {
        isCompleted = false;
        visited = 0;
        wayPoints.Clear();
    }
}
