using UnityEngine;
using UnityEngine.AI;

public class PickupWeapon : Action
{
    private bool pickedUp;
    private WeaponComponent targetStick;

    public PickupWeapon()
    {
        AddPrecondition("playerFound", true);
        AddPrecondition("hasWeapon", false);
        AddEffect("hasWeapon", true);
    }

    public override bool IsAchievable(NavMeshAgent _agent)
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
            Debug.Log("Stick Component not found in scene");
            return false;
        }

        targetStick = closest;
        target = targetStick.gameObject;
        
        return closest != null;
    }

    public override bool PerformAction(NavMeshAgent _agent)
    {
        var inventory = _agent.GetComponent<Inventory>();
        inventory.Add("Weapon", 1);
        
        pickedUp = true;
        
        return true;
    }

    public override bool IsCompleted()
    {
        return pickedUp;
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
        pickedUp = false;
    }
}