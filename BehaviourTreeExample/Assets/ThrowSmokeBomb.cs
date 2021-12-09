using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ThrowSmokeBomb : Action
{
    [SerializeField] private GameObject smokeBomb;
    [SerializeField] private float throwSpeed = 1f;

    private bool isRunning;

    public ThrowSmokeBomb()
    {
        AddPrecondition("findCover", true);
        AddPrecondition("protectPlayer", false);
        AddEffect("protectPlayer", true);
    }

    public override bool IsAchievable(GameObject _agent)
    {
        target = FindObjectOfType<Guard>().gameObject;
        return target != null;
    }

    public override bool PerformAction(GameObject _agent)
    {
        if (!isRunning)
        {
            StartCoroutine(Throw());
        }

        return true;
    }

    private IEnumerator Throw()
    {
        isRunning = true;

        var view = GetComponentInChildren<ViewTransform>().transform;
        
        var bomb = Instantiate(smokeBomb, view.position, Quaternion.identity);
        var rb = bomb.GetComponent<Rigidbody>();

        var force = CalculateVelocity(target.transform.position + target.GetComponent<NavMeshAgent>().velocity, view.position, throwSpeed);
        rb.AddForce(force, ForceMode.Impulse);

        yield return new WaitForSeconds(throwSpeed);

        isCompleted = true;
        isRunning = false;
    }

    private Vector3 CalculateVelocity(Vector3 _target, Vector3 _origin, float _time)
    {
        var distance = _target - _origin;
        var distanceXZ = distance;
        distanceXZ.y = 0;

        var sy = distance.y;
        var sxz = distanceXZ.magnitude;

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
        isCompleted = false;
    }
}
