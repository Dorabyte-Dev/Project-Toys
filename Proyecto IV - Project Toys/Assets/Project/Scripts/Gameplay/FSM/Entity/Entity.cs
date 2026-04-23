using System;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class Entity : MonoBehaviour
{
    public Animator anim;
    protected StateMachine stateMachine;
    [Header("Shadow")]
    public GameObject shadowPrefab;
    public float shadowOffsetY = 0.01f;
    public float shadowScaleMultiplier = 1f;
    [HideInInspector] public GameObject shadowInstance;
    [Header("Collision detection")]
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private Transform groundCheck;
    [SerializeField] public Transform targetCheck;
    [SerializeField] public float targetCheckRadius = 1;
    public bool groundDetected { get; private set; }
    public Vector3 groundNormal => groundDetected ? _groundHit.normal : Vector3.up;

    public float moveSpeed;
    public float turnSmoothTime = 0.1f;
    public float turnSmoothVelocity;

    [Header("Slope Detection")]
    [SerializeField] private float maxSlopeAngle = 30f;
    [SerializeField] private RaycastHit _groundHit;
    

    protected virtual void Awake()
    {
        if (!anim)
        {
            anim = GetComponent<Animator>();
            if (anim == null)
            {
                Debug.LogWarning("Animator component not found on " + gameObject.name);
            }
        }
        stateMachine = new StateMachine();
    }

    protected virtual void Start()
    {
        CastShadow();
    }
    protected virtual void Update()
    {
        HandleCollisionDetected();
        FollowShadow();
        stateMachine.UpdateActiveState();
    }
    

    private void HandleCollisionDetected()
    {
        groundDetected = Physics.Raycast(groundCheck.position, Vector3.down, out _groundHit, groundCheckDistance, whatIsGround);
    }
    public virtual void DeadEntity() 
    {
        
    }

    /*private void OnDrawGizmos() // Visualizacion de rayos y deteccion de pendientes
    {
        float rayDistance = 1f * 0.5f + 0.3f;

        Gizmos.color = Color.red;
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + new Vector3(0, -groundCheckDistance));


        // Raycast para detectar el suelo
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, rayDistance))
        {
            float angle = Vector3.Angle(Vector3.up, hit.normal);

            // Cambiar color seg�n si es pendiente v�lida o no
            if (angle < maxSlopeAngle && angle != 0)
            {
                Gizmos.color = Color.green; // Pendiente v�lida
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
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);
    }*/

    internal void CurrentStateAnimationTrigger()
    {
        stateMachine.currentState.CallAnimationTrigger();
    }

    public bool OnGround()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out _groundHit, 1f * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, _groundHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    public Vector3 ProjectVectorOnSlope(Vector3 vector)
    {
        Vector3 slopeMoveDirection = Vector3.ProjectOnPlane(vector, _groundHit.normal).normalized;

        return slopeMoveDirection;
    }

    public Vector3 ProjectVectorOutOfSlope(Vector3 vector) 
    {
        Vector3 slopeMoveDirection = Vector3.ProjectOnPlane(vector, Vector3.up).normalized;

        return slopeMoveDirection;
    }

    private void CastShadow()
    {
        HandleCollisionDetected();
        if (shadowPrefab != null && groundDetected)
        {
            shadowInstance = Instantiate(shadowPrefab, _groundHit.transform.position, Quaternion.identity);
            shadowInstance.transform.SetParent(transform);
            shadowInstance.transform.localScale *= shadowScaleMultiplier;
        }
        else
        {
            Debug.Log("Shadow prefab not assigned or ground not detected for " + gameObject.name);
            Invoke(nameof(CastShadow), 1f);
        }
    }
    
    private void FollowShadow()
    {
        if (shadowInstance != null)
        {
            shadowInstance.transform.position = _groundHit.point + Vector3.up * shadowOffsetY; // Ajusta la altura del shadow si es necesario
            shadowInstance.transform.rotation = Quaternion.FromToRotation(Vector3.up, _groundHit.normal);
        }
    }

}
