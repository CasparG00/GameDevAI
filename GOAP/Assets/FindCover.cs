using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class FindCover : Action
{
    [SerializeField] private int coverSamplePointCount = 6;

    private Transform enemy;

    private NavMeshAgent agent;
    private Vector3 destination;
    
    private bool isRunning;
    
    public FindCover()
    {
        AddPrecondition("findCover", false);
        AddEffect("findCover", true);
    }

    public override bool IsAchievable(GameObject _agent)
    {
        agent = _agent.GetComponent<NavMeshAgent>();
        enemy = FindObjectOfType<Guard>().transform;

        if (enemy == null) return false;
        return true;
    }

    public override bool PerformAction(GameObject _agent)
    {
        if (!isRunning)
        {
            StartCoroutine(TakeCover(agent));
        }

        agent.SetDestination(destination);
        
        if (agent.isActiveAndEnabled)
        {
            if (Vector3.Distance(agent.destination, _agent.transform.position) < 1)
            {
                Debug.Log("found Cover");
                isCompleted = true;
                StopCoroutine(TakeCover(agent));
            }
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
        isRunning = false;
    }

    private IEnumerator TakeCover(NavMeshAgent _agent)
    {
        isRunning = true;
        yield return new WaitForSeconds(1);

        var hitList = new List<NavMeshHit>();

        for (var i = 0; i < coverSamplePointCount; i++) 
        {
            var spawnPoint = transform.position;
            var offset = Random.insideUnitCircle * i;
            spawnPoint.x += offset.x;
            spawnPoint.z += offset.y;

            NavMesh.FindClosestEdge(spawnPoint, out var navHit, NavMesh.AllAreas);
 
            if (Vector3.Dot(navHit.normal, enemy.position - _agent.transform.position) < 0)
            {
                hitList.Add(navHit);
                break;
            }
        }
        
        var sortedList = hitList.OrderBy(_hit => _hit.distance);

        destination = sortedList.FirstOrDefault().position;
        hitList.Clear();

        isRunning = false;
    }
}
