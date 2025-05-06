using UnityEngine;

public class OrbitCamera : MonoBehaviour
{
    public Transform player;
    public Camera camera;
    public float rotationSpeed = 3f;
    public float verticalSpeed = 2f;
    public float minY = -30f;
    public float maxY = 60f;

    public float distance = 5f;
    public float zoomSpeed = 2f;
    public float minDistance = 2f;
    public float maxDistance = 10f;

    private float currentX = 0f;
    private float currentY = 0f;
    private Vector3 currentRotation;

    void Start()
    {
        currentRotation = transform.localEulerAngles;
        currentY = currentRotation.y;
        currentX = currentRotation.x;
    }

    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X");
            currentY += mouseX * rotationSpeed;
            float mouseY = -Input.GetAxis("Mouse Y");
            currentX = Mathf.Clamp(currentX + mouseY * verticalSpeed, minY, maxY);
            player.Rotate(Vector3.up * mouseX * rotationSpeed, Space.World);
            transform.localEulerAngles = new Vector3(currentX, currentY, 0);
        }

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        distance -= scroll * zoomSpeed;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

        Vector3 direction = (camera.transform.position - player.position).normalized;
        camera.transform.position = player.position + direction * distance;
    }
}