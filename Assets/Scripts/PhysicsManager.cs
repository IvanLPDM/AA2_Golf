using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsManager: MonoBehaviour
{
    public float mass = 0.045f;
    public float radius = 0.021f;
    public float gravity = 9.81f;
    public float rollingFriction = 0.4f;
    public float restitution = 0.8f;
    public float minBounceSpeed;

    public Vector3 velocity;
    public Vector3 acceleration;

    public LayerMask groundLayer;

    private bool wasGrounded = false;

    void Update()
    {
        // Aplicar fuerzas
        ApplyGravity();
        ApplyRollingResistance();

        // Integrar movimiento
        velocity += acceleration * Time.deltaTime;
        transform.position += velocity * Time.deltaTime;

        // Detectar colisión
        bool groundedNow = IsGrounded();

        if (groundedNow)
        {
            // Siempre recolocar encima del suelo
            transform.position = new Vector3(transform.position.x, GetGroundY() + radius, transform.position.z);

            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f, groundLayer))
            {
                Vector3 normal = hit.normal;

                if (!wasGrounded && velocity.magnitude > minBounceSpeed)
                {
                    velocity = Vector3.Reflect(velocity, normal) * restitution;
                }
                else
                {
                    velocity = Vector3.zero;
                    acceleration = Vector3.zero;
                }
            }
            else
            {
                // Parar si el rebote es muy débil
                velocity.y = 0f;
                acceleration.y = 0f;
            }
        }

        wasGrounded = groundedNow;

        // Resetear aceleración
        acceleration = Vector3.zero;
    }

    void ApplyGravity()
    {
        acceleration += Vector3.down * gravity;
    }

    void ApplyRollingResistance()
    {
        if (velocity.magnitude > 0.01f)
        {
            Vector3 friction = -velocity.normalized * rollingFriction * gravity;
            acceleration += friction;
        }
    }

    bool IsGrounded()
    {
        RaycastHit hit;
        return Physics.Raycast(transform.position, Vector3.down, out hit, radius + 0.05f, groundLayer);
    }

    float GetGroundY()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 2f, groundLayer))
        {
            return hit.point.y;
        }
        return transform.position.y;
    }
}