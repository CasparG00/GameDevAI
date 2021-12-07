using UnityEngine;
using UnityEngine.AI;

public class LookForPlayer : Action
{
    private Player player;
    private bool playerFound;
    
    public LookForPlayer()
    {
        AddPrecondition("playerFound", false);
        AddEffect("playerFound", true);
    }
    
    public override bool IsAchievable(NavMeshAgent _agent)
    {
        player = Player.instance;
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
                }
            }
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
    }
}
