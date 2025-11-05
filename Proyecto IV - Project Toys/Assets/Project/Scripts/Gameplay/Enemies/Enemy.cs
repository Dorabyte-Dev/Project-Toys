using UnityEngine;
using UnityEngine.AI;

public class Enemy : Entity
{
    public Enemy_IdleState idleState;   // ESTADO DE IDLE
    public Enemy_MoveState moveState;    // ESTADO DE MOVIMIENTO
    public Enemy_PursuitState pursuitState;  // ESTADO DE PERSECUCION
    public Enemy_WaitAttackState waitAttackState; // ESTADO DE ESPERA DEL ATAQUE
    public Enemy_AttackState attackState;    // ESTADO DE ATTACK

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
    public bool jugadorDetectado;
    public int nearness;

    public NavMeshAgent agent;

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
    public void UpdateStateBasedOnNearness()
    {
        switch (nearness)
        {
            case 0:
                if (!isAttacking)
                {
                    stateMachine.ChangeState(moveState);
                }
                break;
            case 1:
                if (!isAttacking)
                {
                    stateMachine.ChangeState(pursuitState);
                }
                break;
            case 2:
                stateMachine.ChangeState(waitAttackState);
                break;
            default:
                Debug.LogWarning("Error with the detect player system");
                break;
        }
        Debug.Log(nearness);
    }
    #endregion

    #region Damage
    public void DealDamage()
    {
        Debug.Log("Pum te pego: " + damage + " de daño");
    }
    #endregion
}