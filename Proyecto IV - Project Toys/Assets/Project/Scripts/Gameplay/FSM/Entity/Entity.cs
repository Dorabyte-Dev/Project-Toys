using System;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public Transform trEntity;
    public Animator anim { get; private set; }
    public Rigidbody rb { get; private set; }
    protected StateMachine stateMachine;

    [Header("Collision detection")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    public bool groundDetected { get; private set; }

    public float moveSpeed;

    [Header("Slope Detection")]
    [SerializeField] private float maxSlopeAngle;
    [SerializeField] private RaycastHit slopeHit;

    protected virtual void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        stateMachine = new StateMachine();
    }

    protected virtual void Start()
    {
        
    }
    private void Update()
    {
        HandleCollisionDetected();
        stateMachine.UpdateActiveState();
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        Vector3 inputDirection = new Vector3(xVelocity, 0f, yVelocity);

        //If this entity is on a slope, we project the movement direction to the slope normal
        if (OnSlope())
        {
            Vector3 slopeMoveDirection = Vector3.ProjectOnPlane(inputDirection, slopeHit.normal).normalized;
            
            rb.linearVelocity = slopeMoveDirection * moveSpeed;

            if (rb.linearVelocity.y > 0)
            {
                rb.linearVelocity += Vector3.down * 5f * Time.deltaTime;
            }
        }
        else 
        {
            // The Entity is on flat ground
            Vector3 velocity = rb.linearVelocity;
            velocity.x = xVelocity;
            velocity.z = yVelocity;

            // Normalizar solo si hay movimiento
            if (velocity.magnitude > 1f)
            {
                velocity = velocity.normalized * moveSpeed;
            }
            rb.linearVelocity = new Vector3(xVelocity, 0f, yVelocity);
        }

        //rb.linearVelocity = velocity;

        //rb.MovePosition(transform.position + velocity * moveSpeed * Time.deltaTime);
    }

    private void HandleCollisionDetected()
    {
        groundDetected = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, whatIsGround);
    }

    private void OnDrawGizmos()
    {
        float rayDistance = 1f * 0.5f + 0.3f;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -groundCheckDistance));


        // Raycast para detectar el suelo
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, rayDistance))
        {
            float angle = Vector3.Angle(Vector3.up, hit.normal);

            // Cambiar color según si es pendiente válida o no
            if (angle < maxSlopeAngle && angle != 0)
            {
                Gizmos.color = Color.green; // Pendiente válida
            }
            else if (angle == 0)
            {
                Gizmos.color = Color.white; // Superficie plana
            }
            else
            {
                Gizmos.color = Color.red; // Pendiente muy empinada
            }

            // Dibujar la normal de la superficie (perpendicular a la pendiente)
            Gizmos.DrawRay(hit.point, hit.normal * 2f);

            // Punto de impacto
            Gizmos.DrawSphere(hit.point, 0.1f);
        }
    }

    internal void CurrentStateAnimationTrigger()
    {
        throw new NotImplementedException();
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, 1f * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }
}
