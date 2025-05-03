using UnityEngine;

public class PhysicsManager : MonoBehaviour
{
    public float mass = 0.045f;
    public float radius = 0.021f;
    public float gravity = 9.81f;
    public float rollingFriction = 0.4f;
    public float restitution = 0.8f;
    public float minBounceSpeed = 0.1f;

    public Vector3 velocity;
    public Vector3 acceleration;

    public LayerMask groundLayer;

    public bool grounded = false;

    void FixedUpdate()
    {
        ApplyGravity();
        ApplyRollingResistance();

        velocity += acceleration * Time.fixedDeltaTime;
        Vector3 displacement = velocity * Time.fixedDeltaTime;

        RaycastHit hit;
        bool hitDetected = false;

        if (displacement.magnitude > 0.001f)
        {
            hitDetected = Physics.SphereCast(transform.position, radius, displacement.normalized, out hit, displacement.magnitude, groundLayer);
        }
        else
        {
            // Verificar colisión incluso si la pelota no se mueve
            hitDetected = Physics.SphereCast(transform.position, radius, Vector3.down, out hit, 0.05f, groundLayer);
        }

        if (hitDetected)
        {
            transform.position = hit.point + hit.normal * radius;
            Vector3 normal = hit.normal;

            if (velocity.magnitude > minBounceSpeed)
            {
                velocity = Vector3.Reflect(velocity, normal) * restitution;
            }
            else
            {
                velocity -= Vector3.Project(velocity, normal);
                acceleration -= Vector3.Project(acceleration, normal);

                Vector3 gravityForce = Vector3.down * gravity;
                Vector3 slopeForce = gravityForce - Vector3.Dot(gravityForce, normal) * normal;

                if (slopeForce.magnitude < 0.01f)
                {
                    velocity = Vector3.zero;
                    acceleration = Vector3.zero;
                    grounded = true;
                }
                else
                {
                    acceleration += slopeForce;
                }
            }
        }
        else if(!grounded)
        {
            transform.position += displacement;
        }


        acceleration = Vector3.zero;
    }

    void ApplyGravity()
    {
        if (!grounded)
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
}