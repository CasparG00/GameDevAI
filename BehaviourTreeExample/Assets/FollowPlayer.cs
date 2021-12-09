using UnityEngine;
using UnityEngine.AI;

public class FollowPlayer : Action
{
    [SerializeField] private float playerFollowDistance = 5;
    private bool followedPlayer;
    private PlayerFollowTarget playerTarget;
    
    public FollowPlayer()
    {
        AddPrecondition("followPlayer", false);
        AddEffect("followPlayer", true);
    }
    
    public override bool IsAchievable(NavMeshAgent _agent)
    {
        playerTarget = PlayerFollowTarget.instance;
        target = playerTarget.gameObject;
        
        return playerTarget != null;
    }

    public override bool PerformAction(NavMeshAgent _agent)
    {
        followedPlayer = true;
        return true;
    }

    public override bool IsCompleted()
    {
        return followedPlayer;
    }

    public override bool RequiresInRange()
    {
        return true;
    }

    public override void Reset()
    {
        followedPlayer = false;
    }
}
