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
    public Entity_Health health;
    
    public int facingDirection = 1;
    
    public Transform player { get; private set; }

    [Header("Attack Cooldown System")]
    public bool isOnAttackCooldown;
    public float attackCooldown = 5f; // Tiempo en segundos antes de poder atacar de nuevo
    private float attackCooldownTimer;
    public float waitTimeToAttack;

    [Header("Wave Manager Control")] 
    public bool canAttackByManager;

    protected override void Awake()
    {
        base.Awake();
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<Enemy_Health>();
        agent.speed = moveSpeed;
        agent.acceleration = acceleration;
        agent.isStopped = false;
    }
    
    protected override void Update()
    {
        base.Update();

        // Cooldown
        if (isOnAttackCooldown)
        {
            attackCooldownTimer -= Time.deltaTime;
            if (attackCooldownTimer <= 0f)
            {
                isOnAttackCooldown = false;
                Debug.Log($"[{name}] Cooldown de ataque terminado, puede volver a atacar.");
            }
        }

        // FSM básica (solo idle/move/pursuit)
        UpdateFSMByNearness();
    }
    
    public override void DeadEntity()
    {
        base.DeadEntity();

        stateMachine.ChangeState(deadState);
    }
    protected override void Start()
    {
        base.Start();
        GetComponent<Entity_Combat>().targetHit.AddListener(OnPlayerDamaged);
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

    public void ResetStats()
    {
        stateMachine.SwitchOnStateMachine();
        agent.enabled = true;
        anim.enabled = true;
        agent.speed = moveSpeed;
        agent.acceleration = acceleration;
        agent.isStopped = false;
        health.ResetStats();
    }
    public void PlayerDeath()
    {
        stateMachine.ChangeState(idleState);
    }
    private void OnEnable()
    {
        Player.OnPlayerDeath += PlayerDeath;
        if (EnemyWaveManager.Instance != null)
            EnemyWaveManager.Instance.RegisterEnemy(this);
    }

    private void OnDisable()
    {
        Player.OnPlayerDeath -= PlayerDeath;
        if (EnemyWaveManager.Instance != null)
            EnemyWaveManager.Instance?.UnregisterEnemy(this);
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

    #region Player Detection + FSM básica

    public void UpdateFSMByNearness()
    {
        // No cambies nada si está muerto, flinch, o atacando
        if (stateMachine.currentState == deadState || 
            stateMachine.currentState == flinchState ||
            isAttacking)
            return;
        
        // Si está en lógica de ataque, NO tocar nada aquí
        if (stateMachine.currentState == waitAttackState ||
            stateMachine.currentState == attackState)
            return;

        switch (nearness)
        {
            case 0:
                // Sin jugador cerca: patrulla / idle
                if (stateMachine.currentState != moveState && stateMachine.currentState != idleState)
                {
                    stateMachine.ChangeState(moveState);
                }
                break;

            case 1:
            case 2:
                if (!canAttackByManager)
                {
                    stateMachine.ChangeState(pursuitState);
                }
                break;
        }
    }

    #endregion

    #region Attack Turn System

    public void StartAttackCooldown()
    {
        isOnAttackCooldown = true;
        attackCooldownTimer = attackCooldown;
        Debug.Log($"[{name}] Inicia cooldown de ataque ({attackCooldown}s).");
    }

    public void NotifyAttackStarted()
    {
        isAttacking = true;
        EnemyWaveManager.Instance.NotifyAttackStarted(this);
    }

    public void NotifyAttackFinished()
    {
        Debug.Log($"[Enemy] {name} notificó fin de ataque.");
    
        // 1) Avisar al WaveManager
        EnemyWaveManager.Instance.NotifyAttackEnded(this);
    
        // 2) Iniciar cooldown local
        StartAttackCooldown();
    
        isAttacking = false;
        canAttackByManager = false;
    }

    public bool CanStartAttackNow()
    {
        return canAttackByManager && !isAttacking && !isOnAttackCooldown;
    }
    public void AllowAttackFromManager()
    {
        canAttackByManager = true;
        Debug.Log($"[{name}] Recibe permiso del WaveManager para atacar.");
    }
    public void RevokeAttackPermission()
    {
        if (canAttackByManager)
        {
            canAttackByManager = false;
            EnemyWaveManager.Instance.CancelAttackRequest(this);
            Debug.Log($"[{name}] Permiso de ataque revocado.");
        }
    }

    #endregion
    
    #region Perfect Dodge
    void OnPlayerDamaged()
    {
        PerfectDodgeManager.EndPerfectDodgeFlag(this.gameObject);
    }
    #endregion
}