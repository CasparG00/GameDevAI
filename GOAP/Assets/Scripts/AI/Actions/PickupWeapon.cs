using UnityEngine;
using UnityEngine.AI;

public class PickupWeapon : Action
{
    private WeaponComponent targetWeapon;

    public PickupWeapon()
    {
        AddPrecondition("playerFound", true);
        AddPrecondition("hasWeapon", false);
        AddEffect("hasWeapon", true);
    }

    public override bool IsAchievable(GameObject _agent)
    {
        var items = FindObjectsOfType<WeaponComponent>();
        WeaponComponent closest = null;
        var minDist = Mathf.Infinity;

        foreach (var item in items)
        {
            var dist = Vector3.Distance(item.transform.position, _agent.transform.position);
            if (dist < minDist)
            {
                closest = item;
                minDist = dist;
            }
        }

        if (closest == null)
        {
            return false;
        }

        targetWeapon = closest;
        target = targetWeapon.gameObject;
        
        return closest != null;
    }

    public override bool PerformAction(GameObject _agent)
    {
        var inventory = _agent.GetComponent<Inventory>();
        inventory.Add("Weapon", 1);
        
        isCompleted = true;
        
        return true;
    }

    public override bool RequiresInRange()
    {
        return true;
    }

    public override bool IsInRange()
    {
        return inRange;
    }

    public override void Reset()
    {
        isCompleted = false;
    }
}