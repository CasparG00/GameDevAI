using UnityEngine;
using UnityEngine.AI;

public class FollowPlayer : Action
{
    private PlayerFollowTarget playerTarget;
    
    public FollowPlayer()
    {
        AddPrecondition("followPlayer", false);
        AddEffect("followPlayer", true);
    }
    
    public override bool IsAchievable(GameObject _agent)
    {
        playerTarget = PlayerFollowTarget.instance;
        target = playerTarget.gameObject;
        
        return playerTarget != null;
    }

    public override bool PerformAction(GameObject _agent)
    {
        isCompleted = true;
        return true;
    }

    public override bool RequiresInRange()
    {
        return true;
    }

    public override void Reset()
    {
        isCompleted = false;
    }
}
