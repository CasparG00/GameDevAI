using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private float rotXSpeed = 30f;
    [SerializeField] private float rotYSpeed = 30f;
    [SerializeField] private Transform followTarget;

    private float angleX = 0;
    private float angleY = 0;
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        angleX += mouseX * rotXSpeed;
        angleY += mouseY * rotYSpeed;
        angleY = Mathf.Clamp(angleY, -85f, 85f);
        transform.position = followTarget.position;// Vector3.Lerp(transform.position, followTarget.position, Time.fixedDeltaTime * smoothFollowFactor);
        transform.rotation = Quaternion.Euler(-angleY, angleX, 0);
    }

}
