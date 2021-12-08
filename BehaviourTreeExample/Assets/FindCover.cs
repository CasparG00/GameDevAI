using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FindCover : Action
{
    private bool coverFound;
    
    public FindCover()
    {
        AddPrecondition("findCover", false);
        AddEffect("findCover", true);
    }

    public override bool IsAchievable(NavMeshAgent _agent)
    {
        return true;
    }

    public override bool PerformAction(NavMeshAgent _agent)
    {
        return true;
    }

    public override bool IsCompleted()
    {
        return coverFound;
    }

    public override bool RequiresInRange()
    {
        return true;
    }

    public override void Reset()
    {
        coverFound = false;
    }
}
