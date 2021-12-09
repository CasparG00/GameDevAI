using UnityEngine;

public class PlayerFollowTarget : Singleton<PlayerFollowTarget>
{
    [SerializeField] private float distance = 5;
    private Transform player;

    private void Start()
    {
        player = Player.instance.transform;
    }

    private void Update()
    {
        transform.position = player.position - player.forward * distance;
    }
}
