using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.VersionControl.Asset;

public class Enemy : Entity
{
    public Enemy_IdleState idleState;
    public Enemy_MoveState moveState;

    [Header("Enemy Specs")]
    public float range;
    public float attackRange;
    public float waitTime;
    public float acceleration;

    [Header("Enemy Attack Specs")]
    public float attackAcceleration;
    public int damage;
    public float attackSpeed;
    public GameObject damageCollider;
    public bool isAttacking;

    [Header("Player Coords")]
    [HideInInspector] public Transform playerTransform;
    private Vector3 lastPlayerPosition;
    private Vector3 currentPosition;
    private Vector3 attackPoint;
    public bool jugadorDetectado;
    private int nearness;

    public NavMeshAgent agent;
    private float currentTime;

    public int facingDirection = 1;

    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.acceleration = acceleration;
        damageCollider.SetActive(false);
    }
    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    public void Flip()
    {
        facingDirection *= -1;
        transform.Rotate(0f, 180f, 0f);
    }


    #region Player Detection
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            nearness++;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerTransform = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            nearness--;
        }
    }
    #endregion
}
