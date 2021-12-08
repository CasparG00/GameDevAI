using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FindCover : Action
{
    [SerializeField] private int coverSamplePointCount = 6;
    [SerializeField] private int coverSampleRadius = 10;

    private Transform enemy;
    private List<Vector3> potentialCoverPoints = new List<Vector3>();
    private Vector3 chosenCoverPoint = Vector3.zero;
    private bool coverFound;

    public FindCover()
    {
        AddPrecondition("findCover", false);
        AddEffect("findCover", true);
    }

    public override bool IsAchievable(NavMeshAgent _agent)
    {
        enemy = FindObjectOfType<Guard>().transform;

        if (enemy == null) return false;
        
        for (var i = 0; i < coverSamplePointCount; i++)
        {
            var point = _agent.transform.position + (Vector3)Random.insideUnitCircle * coverSampleRadius;

            if (NavMesh.FindClosestEdge(point, out var hit, NavMesh.AllAreas))
            {
                if (Vector3.Dot(hit.normal, enemy.position - _agent.transform.position) > 0)
                {
                    potentialCoverPoints.Add(hit.position);
                    Debug.Log("added: " + hit.position);
                }
            }
        }

        if (potentialCoverPoints.Count == 0) return false;
        chosenCoverPoint = potentialCoverPoints[Random.Range(0, potentialCoverPoints.Count)];

        return true;
    }

    public override bool PerformAction(NavMeshAgent _agent)
    {
        if (_agent.destination != chosenCoverPoint)
        {
            _agent.SetDestination(chosenCoverPoint);
            Debug.Log("agent destination is: " + chosenCoverPoint);
        }

        var distance = Vector3.Distance(_agent.transform.position, _agent.destination);
        if (distance < 1)
        {
            Debug.Log("found Cover");
            coverFound = true;
        }
        return true;
    }

    public override bool IsCompleted()
    {
        return coverFound;
    }

    public override bool RequiresInRange()
    {
        return false;
    }

    public override void Reset()
    {
        potentialCoverPoints.Clear();
        coverFound = false;
    }
}
