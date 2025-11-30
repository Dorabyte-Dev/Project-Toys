using System;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Entity
{
    public Enemy_IdleState idleState;   // ESTADO DE IDLE
    public Enemy_MoveState moveState;    // ESTADO DE MOVIMIENTO
    public Enemy_PursuitState pursuitState;  // ESTADO DE PERSECUCION
    public Enemy_WaitAttackState waitAttackState; // ESTADO DE ESPERA DEL ATAQUE
    public Enemy_AttackState attackState;    // ESTADO DE ATTACK
    public Enemy_DeadState deadState; // ESTADO DE MUERTE
    public Enemy_FlinchState flinchState; // ESTADO DE ATURDIMIENTO


    [Header("Enemy Specs")]
    public float range;
    public float attackRange;
    public float waitTime;
    public float acceleration;
    public float orbitDistance;
    public float orbitSpeed;
    [HideInInspector]public float orbitAngle;

    [Header("Enemy Attack Specs")]
    public float attackAcceleration;
    public int damage;
    public float attackSpeed;
    public bool isAttacking;
    public float flinchTime;

    [Header("Player Coords")]
    public bool jugadorDetectado;
    public int nearness;
    [HideInInspector] public Transform playerTransform;

    [Space]
    public NavMeshAgent agent;
    public EnemySpawner spawner; //Que spawner lo ha generado
    public GameObject originalPrefab; //De que prefab se ha generado (util para la factory)
    
    public int facingDirection = 1;
    
    public Transform player { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.acceleration = acceleration;
        agent.isStopped = false;
    }

    
    public override void DeadEntity()
    {
        base.DeadEntity();

        stateMachine.ChangeState(deadState);
    }
    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
        if (spawner == null)
            Debug.LogWarning("Spawner not assigned. Check GameObject to component of EnemySpawner.cs");
    }

    public void Flip()
    {
        facingDirection *= -1;
        transform.Rotate(0f, 180f, 0f);
    }

    public override void ChangeFlintState()
    {
        base.ChangeFlintState();
        Debug.Log("Entro en flichState");
        stateMachine.ChangeState(flinchState);
    }

    
    public void PlayerDeath()
    {
        stateMachine.ChangeState(idleState);
    }
    private void OnEnable()
    {
        Player.OnPlayerDeath += PlayerDeath;
    }

    private void OnDisable()
    {
        Player.OnPlayerDeath -= PlayerDeath;
    }

    //public Transform GetPlayerReference()
    //{
    //    if (player == null)
    //        player == PlayerDetected().transform;
    //    return player;
    //}

    //private RaycastHit PlayerDetected()
    //{
    //    RaycastHit hit =
    //        Physics.Raycast(transform.position, transform.forward, out hit, range)
    //    if (Physics.Raycast(transform.position, directionToPlayer, out hit, range))
    //    {
    //        if (hit.transform.CompareTag("Player"))
    //        {
    //            return hit;
    //        }
    //    }
    //    return hit;
    //}

    #region Player Detection
    public void UpdateStateBasedOnNearness()
    {
        Debug.Log("Updating state based on nearness");
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
                if (!isAttacking)
                {
                    stateMachine.ChangeState(waitAttackState);
                }
                break;
            default:
                Debug.LogWarning("Error with the detect player system");
                break;
        }
    }

    public void CallUpdateStateDetection()
    {
        UpdateStateBasedOnNearness();
    }
    #endregion

}