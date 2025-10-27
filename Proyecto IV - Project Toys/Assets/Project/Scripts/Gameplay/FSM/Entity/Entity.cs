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
        Vector3 velocity = rb.linearVelocity;
        velocity.x = xVelocity;
        velocity.z = yVelocity;

        // Normalizar solo si hay movimiento
        if (velocity.magnitude > 1f)
        {
            velocity = velocity.normalized * moveSpeed;
        }

        rb.linearVelocity = velocity;

        //rb.linearVelocity = new Vector3(xVelocity, 0f, yVelocity);
    }

    private void HandleCollisionDetected()
    {
        groundDetected = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, whatIsGround);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, -groundCheckDistance));
    }

    internal void CurrentStateAnimationTrigger()
    {
        throw new NotImplementedException();
    }
}
