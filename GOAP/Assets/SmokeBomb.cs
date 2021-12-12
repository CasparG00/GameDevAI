using UnityEngine;

public class SmokeBomb : MonoBehaviour
{
    [SerializeField] private float destroyDuration = 0.5f;

    private void OnCollisionStay()
    {
        Destroy(gameObject, destroyDuration);
    }
}
