using UnityEngine.AI;

public class FollowPlayer : Action
{
    private bool followedPlayer;
    private Player player;
    
    public FollowPlayer()
    {
        AddPrecondition("followPlayer", false);
        AddEffect("followPlayer", true);
    }
    
    public override bool IsAchievable(NavMeshAgent _agent)
    {
        player = Player.instance;

        target = player.gameObject;
        return player != null;
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
