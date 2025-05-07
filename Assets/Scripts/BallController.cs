using UnityEngine;

public class BallController : MonoBehaviour
{
    public Transform ball;               // Referencia a la bola
    public GameObject stick;             // El palo
    public AudioSource audio;
    public Mesh red;
    public Mesh blue;
    public Mesh green;
    public Transform stickTransform;
    public float minRotation = 45f;
    public float maxRotation = 45f;
    public float sensitivity = 0.2f;     // Sensibilidad del arrastre

    private bool dragging = false;
    private float startMouseY;
    private float currentRotation = 0f;

    public float hitDistance = 0.1f;
    private Vector3 lastPosition;

    private Quaternion initialRotation;

    private MeshFilter meshFilter;

    private Force force;

    enum Force{
        RED,
        BLUE,
        GREEN
    }

    void Start()
    {
        stick.SetActive(false);
        meshFilter = stick.GetComponent<MeshFilter>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            meshFilter.mesh = red;
            force = Force.RED;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            meshFilter.mesh = blue;
            force = Force.BLUE;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            meshFilter.mesh = green;
            force = Force.GREEN;
        }

        PhysicsManager PM;
        PM = ball.GetComponent<PhysicsManager>();

        if (PM.velocity == Vector3.zero)
        {
            if (Input.GetMouseButtonDown(0))
            {
                dragging = true;
                stick.SetActive(true);
                startMouseY = Input.mousePosition.y;
                currentRotation = 0f;

                Vector3 directionFromCamera = (ball.position - Camera.main.transform.position).normalized;

                stick.transform.position = ball.position + directionFromCamera * 0.3f;
                stick.transform.position += new Vector3(0, 3.8f, 0);
                stick.transform.LookAt(ball.position);

                initialRotation = stick.transform.rotation;
                lastPosition = stickTransform.position; // inicializar al empezar
            }

            if (Input.GetMouseButton(0) && dragging)
            {
                float deltaY = Input.mousePosition.y - startMouseY;
                float rotationAmount = Mathf.Clamp(deltaY * sensitivity, minRotation, maxRotation);
                currentRotation = rotationAmount;

                stick.transform.rotation = initialRotation * Quaternion.Euler(rotationAmount, 0, 0);
            }

            if (Input.GetMouseButtonUp(0))
            {
                dragging = false;
                stick.SetActive(false);
            }
        }

        

        if (ball != null && dragging)
        {
            Vector3 currentPosition = stickTransform.position;
            Vector3 toBallNow = ball.position - currentPosition;
            Vector3 toBallLast = ball.position - lastPosition;

            // Si el palo atravesó la pelota, desactivarlo
            if (Vector3.Dot(toBallNow, toBallLast) < 0)
            {
                Debug.Log("El palo ha atravesado la pelota");
                dragging = false;
                stick.SetActive(false);
                return;
            }

            float distance = Vector3.Distance(currentPosition, ball.transform.position);
            if (distance < hitDistance)
            {
                Debug.Log("¡El palo ha golpeado la pelota!");
                dragging = false;
                stick.SetActive(false);

                Vector3 direction = (ball.transform.position - stickTransform.position).normalized;
                float velocity = (currentPosition - lastPosition).magnitude / Time.deltaTime;

                switch (force)
                {
                    case Force.RED:
                        direction.y *= 2;
                        break;

                    case Force.GREEN:
                        direction.y *= 0.4f;
                        break;

                    case Force.BLUE:
                        direction.y *= 1f;
                        break;

                    default:
                        break;
                }

                

                ball.GetComponent<PhysicsManager>()?.HitBall(direction, velocity * 2);
                audio.Play();
            }

            lastPosition = currentPosition;
        }
    }
}