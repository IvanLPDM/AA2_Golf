using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Win : MonoBehaviour
{

    public Transform ball;          // Referencia al objeto con tag "Ball"
    public float triggerRadius = 0.5f; // Radio de detección manual
    private AudioSource source;
    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
    }



    void Update()
    {
        if (ball != null && ball.CompareTag("Ball"))
        {
            float distance = Vector3.Distance(transform.position, ball.position);
            if (distance < triggerRadius)
            {
                Debug.Log("WIN");
                source.Play(); 
                // Ejecuta aquí la lógica que necesites
            }
        }
    }
}
