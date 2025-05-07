using UnityEngine;

public class PhysicsManager : MonoBehaviour
{
    public float mass = 0.045f;
    public float radius = 0.021f;
    public float gravity = 9.81f;
    public float rollingFriction = 0.4f;
    public float restitution = 0.8f;
    public float minBounceSpeed = 0.1f;
    public AudioSource source;

    private Vector3 checkpoint;

    public Vector3 velocity;
    public Vector3 acceleration;

    public LayerMask groundLayer;

    public bool grounded = false;

    private Vector3 normal;

    public Vector3 directionHit;
    public bool hit_Ball;
    public float force_hit;
    private bool deslizar = false;

    public bool isOnIce = false;
    public float groundResitance;
    public float iceResistance;

    public GameObject canvas;

    private void Start()
    {
        checkpoint = transform.position;
    }

    private void Update()
    {
        //Reset
        if (Input.GetKeyDown("r"))
        {
            ResetBall();
        }

        if (transform.position.y <= -10)
        {
            ResetBall();
        }

        if(Input.GetKeyDown("escape"))
        {
            Canvas_Script canvas_ =canvas.GetComponent<Canvas_Script>();
            canvas_.Active();
        }
    }

    void FixedUpdate()
    {
        ApplyGravity();
        grounded = Physics.CheckSphere(transform.position, radius + 0.005f, groundLayer);

        velocity += acceleration * Time.fixedDeltaTime;
        Vector3 displacement = velocity * Time.fixedDeltaTime;

        RaycastHit hit;
        bool hitDetected = false;

        

        if (hit_Ball)
        {
            HitBall(directionHit, force_hit);
            hit_Ball = false;
        }

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
            normal = hit.normal;

            isOnIce = hit.collider.CompareTag("Ice");

            

            if (velocity.magnitude > minBounceSpeed)
            {
                velocity = Vector3.Reflect(velocity, normal) * restitution;
                source.Play();
            }
            else // la pelota no rebota y se queda en el suelo
            {
                deslizar = true;   
            }

            
        }
        else
        {
            transform.position += displacement;
        }

        if (grounded && deslizar)
        {

            // Eliminar componentes normales de velocidad y aceleración
            velocity -= Vector3.Project(velocity, normal);
            acceleration -= Vector3.Project(acceleration, normal);

            // Calcular componente tangencial de la gravedad
            Vector3 gravityForce = Vector3.down * gravity;
            Vector3 tangentGravity = gravityForce - Vector3.Dot(gravityForce, normal) * normal;

            // Calcular velocidad tangencial
            Vector3 tangentialVelocity = velocity - Vector3.Project(velocity, normal);

            // Aplicar fricción de rodadura solo si hay movimiento tangencial
            if (tangentialVelocity.magnitude > 0.01f)
            {
                if (isOnIce)
                {
                    rollingFriction = iceResistance;
                }
                else
                    rollingFriction = groundResitance;

                Vector3 frictionDir = -tangentialVelocity.normalized;
                float normalForce = mass * gravity * Vector3.Dot(normal, Vector3.up);
                Vector3 friction = frictionDir * rollingFriction * normalForce / mass;

                // Limitar la fricción para que no invierta dirección
                Vector3 maxFriction = -tangentialVelocity / Time.fixedDeltaTime;
                if (friction.magnitude > maxFriction.magnitude)
                {
                    friction = maxFriction;
                }

                tangentGravity += friction;
            }
            else
            {
                // Si se ha frenado casi por completo, detenla del todo
                velocity = Vector3.zero;
                acceleration = Vector3.zero;
                deslizar = false;
                checkpoint = transform.position;
                return;
            }

            acceleration += tangentGravity;
            velocity += acceleration * Time.fixedDeltaTime;
            displacement = velocity * Time.fixedDeltaTime;
            transform.position += displacement;
        }


        //actualizar gravedad
        //if(grounded && transform.position.y >= 0.001)
        //{
        //    grounded = false;
        //}


        acceleration = Vector3.zero;
    }

    void ApplyGravity()
    {
        if (!grounded)
            acceleration += Vector3.down * gravity;

            
    }

    public void HitBall(Vector3 direction, float force)
    {
        direction.Normalize();

        Vector3 impulseVelocity = direction * (force / mass);

        velocity += impulseVelocity;

    }
    public void ResetBall()
    {
        velocity = Vector3.zero;
        acceleration = Vector3.zero;
        deslizar = false;
        transform.position = checkpoint;
        grounded = true;
    }
}