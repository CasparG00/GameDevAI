using UnityEngine;
using UnityEngine.AI;

public class Attack : Action
{
    [SerializeField] private float damage;
    [SerializeField] private LayerMask attackLayer;
    private bool attacked;
    
    public Attack()
    {
        AddPrecondition("playerFound", true);
        AddPrecondition("hasWeapon", true);
        AddEffect("attackedPlayer", true);
    }
    
    public override bool IsAchievable(NavMeshAgent _agent)
    {
        target = Player.instance.gameObject;
        return target != null;
    }

    public override bool PerformAction(NavMeshAgent _agent)
    {
        var distance = Vector3.Distance(_agent.transform.position, target.transform.position);
        if (distance > 5)
        {
            return false;
        }

        var agentTransform = _agent.transform;
        var hits = Physics.OverlapSphere(agentTransform.position, 2, attackLayer);
        foreach (var hit in hits)
        {
            var damageable = hit.transform.GetComponent<IDamageable>();
            damageable?.TakeDamage(gameObject, damage);
            attacked = true;
        }

        return true;
    }

    public override bool IsCompleted()
    {
        return attacked;
    }

    public override bool RequiresInRange()
    {
        return true;
    }

    public override void Reset()
    {
        attacked = false;
    }
}
