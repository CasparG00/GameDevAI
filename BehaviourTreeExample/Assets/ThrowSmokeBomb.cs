using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ThrowSmokeBomb : Action
{
    [SerializeField] private GameObject smokeBomb;
    [SerializeField] private float throwSpeed = 1f;
    private bool thrownSmokeBomb;

    private bool isRunning;

    public ThrowSmokeBomb()
    {
        AddPrecondition("findCover", true);
        AddPrecondition("protectPlayer", false);
        AddEffect("protectPlayer", true);
    }

    public override bool IsAchievable(NavMeshAgent _agent)
    {
        target = FindObjectOfType<Guard>().gameObject;
        return target != null;
    }

    public override bool IsCompleted()
    {
        return thrownSmokeBomb;
    }

    public override bool PerformAction(NavMeshAgent _agent)
    {
        if (!isRunning)
        {
            StartCoroutine(Throw(_agent));
        }

        return true;
    }

    private IEnumerator Throw(NavMeshAgent _agent)
    {
        isRunning = true;
        var bomb = Instantiate(smokeBomb, _agent.transform.position, Quaternion.identity);
        var rb = bomb.GetComponent<Rigidbody>();

        var force = CalculateVelocity(target.transform.position + target.GetComponent<NavMeshAgent>().velocity, _agent.transform.position, throwSpeed);
        rb.AddForce(force, ForceMode.Impulse);

        yield return new WaitForSeconds(throwSpeed);

        thrownSmokeBomb = true;
        isRunning = false;
    }

    private Vector3 CalculateVelocity(Vector3 _target, Vector3 _origin, float _time)
    {
        Vector3 distance = _target - _origin;
        Vector3 distanceXZ = distance;
        distanceXZ.y = 0;

        var sy = distance.y;
        float sxz = distanceXZ.magnitude;

        var vxz = sxz / _time;
        var vy = sy / _time + 0.5f * Mathf.Abs(Physics.gravity.y) * _time;

        var result = distanceXZ.normalized;
        result *= vxz;
        result.y = vy;

        return result;
    }

    public override bool RequiresInRange()
    {
        return false;
    }

    public override void Reset()
    {
        thrownSmokeBomb = false;
    }
}
