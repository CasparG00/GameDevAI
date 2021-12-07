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
        var ray = new Ray(transform.position, transform.forward);
        if (Physics.SphereCast(ray, 2, out var hit, 1, attackLayer))
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
