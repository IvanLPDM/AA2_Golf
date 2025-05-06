using UnityEngine;

public class BallController : MonoBehaviour
{
    public Transform ball;               // Referencia a la bola
    public GameObject stick;             // El palo
    public float maxPullBack = 0.5f;     // Máxima distancia hacia atrás
    public float sensitivity = 0.01f;    // Sensibilidad del arrastre

    private float currentPull = 0f;      // Posición actual del palo
    private bool dragging = false;
    private Vector3 initialLocalPosition;
    private float startMouseY;

    void Start()
    {
        stick.SetActive(false);
        initialLocalPosition = stick.transform.localPosition;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragging = true;
            stick.SetActive(true);
            startMouseY = Input.mousePosition.y;
            currentPull = 0f;

        }

        if (Input.GetMouseButton(0) && dragging)
        {
            float deltaY = Input.mousePosition.y - startMouseY;
            currentPull = Mathf.Clamp(deltaY * sensitivity, -maxPullBack, 0);

            Vector3 pullPosition = initialLocalPosition + new Vector3(0, 0, currentPull);
            stick.transform.localPosition = pullPosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            dragging = false;
            stick.SetActive(false);
            
        }
    }
}