using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

public class Enemy : Entity, IEnemyStates
{
    public Rigidbody rb { get; private set; }
    public Enemy_IdleState idleState;   // ESTADO DE IDLE
    public Enemy_MoveState moveState;    // ESTADO DE MOVIMIENTO\ WANDER
    public Enemy_PursuitState pursuitState;  // ESTADO DE PERSECUCION
    public Enemy_WaitAttackState waitAttackState; // ESTADO DE ESPERA DEL ATAQUE
    public Enemy_AttackState attackState;    // ESTADO DE ATTACK
    public Enemy_DeadState deadState; // ESTADO DE MUERTE
    public Enemy_FlinchState flinchState; // ESTADO DE ATURDIMIENTO
    public Enemy_ExecutionState executionState; // ESTADO DE EJECUCION
    public Enemy_ExtraState extraState; // ESTADO EXTRA PERSONALIZADO

    [Header("Enemy Agent Specs")]
    public float acceleration;
    
    [Header("Enemy Attack Specs")]
    public int damage;
    [HideInInspector]public bool isAttacking;
    [HideInInspector] public bool hasAttacked;

    [Header("Player Coords")] 
    [HideInInspector] public int nearness;
    [HideInInspector] public Transform playerTransform;
    [HideInInspector] public float distanceToPlayer;
    public Transform playerExecutionTransform;

    [Space] 
    [HideInInspector] public Renderer mesh;
    public NavMeshAgent agent;
    public EnemySpawner spawner; //Que spawner lo ha generado
    public GameObject originalPrefab; //De que prefab se ha generado (util para la factory)
    public Entity_Health health;
    
    public int facingDirection = 1;
    
    public Transform player { get; private set; }
    
    [HideInInspector]public Enemy_Combat combat;
    [HideInInspector]public Enemy_Health _health;
    [HideInInspector]public Enemy_AnimationTriggers _animationTriggers;
    [HideInInspector]public Enemy_VFX _vfx;
    [HideInInspector]public EnemyUI enemyUI;
    
    [Header("WaveManager Specs")]
    public bool canAttackByManager; // permiso del manager para atacar
    
    [Header("Common States Specs")]
    public bool isBeingExecuted; 

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
        mesh = GetComponentInChildren<Renderer>();
        agent = GetComponent<NavMeshAgent>();
        health = GetComponent<Enemy_Health>();
        agent.speed = moveSpeed;
        agent.acceleration = acceleration;
        agent.isStopped = false;
        playerTransform = GameObject.FindWithTag("Player").transform;
        if(playerTransform == null) Debug.LogError("No player found");
    }
    
    public override void DeadEntity()
    {
        base.DeadEntity();

        ChangeEnemyState(deadState);
        
        SetEnemyDead();
    }
    
    public virtual void SetEnemyDead()
    {
        agent.isStopped = true;
        PerfectDodgeManager.EndPerfectDodgeFlag(this.gameObject);
        if (spawner != null)
            spawner.EnemyDead(this.gameObject);
        stateMachine.SwitchOffStateMachine();
    }
    protected override void Start()
    {
        base.Start();
        enemyUI = GetComponent<EnemyUI>();
        combat = GetComponent<Enemy_Combat>();
        _animationTriggers = GetComponent<Enemy_AnimationTriggers>();
        _health = GetComponent<Enemy_Health>();
        _vfx = GetComponent<Enemy_VFX>();
        combat.targetHit.AddListener(OnPlayerDamaged);
        _vfx.OnDissolveComplete += _animationTriggers.DisableAndDestroyEnemy;
        //stateMachine.Initialize(idleState);
        if (spawner == null)
            Debug.LogWarning("Spawner not assigned. Check GameObject to component of EnemySpawner.cs");
    }

    protected override void Update()
    {
        base.Update();
        if (Input.GetKeyDown(KeyCode.P))
        {
            //_health.TakeDamage(0, this.transform);
            ChangeEnemyState(executionState);
        }
    }

    public void Flip()
    {
        facingDirection *= -1;
        transform.Rotate(0f, 180f, 0f);
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
    
    public void EnterExecution()
    {
        ChangeEnemyState(executionState);
    }
    
    public void StopAttacking()
    {
        ChangeEnemyState(pursuitState);
    }
    public void PlayerDeath()
    {
        ChangeEnemyState(idleState);
    }

    public void ChangeEnemyState(EnemyState newState)
    {
        if(isBeingExecuted || health.isDead) return;
        stateMachine.ChangeState(newState);
    }
    private void OnEnable()
    {
        Player.OnPlayerDeath += PlayerDeath;
        if (EnemyWaveManager.Instance != null)
            EnemyWaveManager.Instance?.RegisterEnemy(this);
    }

    private void OnDisable()
    {
        Player.OnPlayerDeath -= PlayerDeath;
        _vfx.OnDissolveComplete -= _animationTriggers.DisableAndDestroyEnemy;
        if (EnemyWaveManager.Instance != null)
            EnemyWaveManager.Instance?.UnregisterEnemy(this);
        PerfectDodgeManager.EndPerfectDodgeFlag(this.gameObject);
    }

    /*public void EnemyDeathTest()
    {
        Destroy(this.gameObject);
        //this.gameObject.SetActive(false);
    }*/

    #region Common Enemy Methods
    protected void LookToPlayer()
    {
        transform.LookAt(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z), Vector3.up);
    }
    public virtual void ChangeFlinchState()
    {
        
    }
    #endregion

    #region Player Detection

    public float CheckPlayerDistance()
    {
        float distance;
        distance = (transform.position - playerTransform.position).sqrMagnitude;
        return distance;
    }

    public Vector3 GetPlayerDirection()
    {
        return playerTransform.position - transform.position;
    }
    
    protected void GetDistanceToPlayer()
    {
        distanceToPlayer = CheckPlayerDistance();
    }
    #endregion

    #region Perfect Dodge
    void OnPlayerDamaged()
    {
        PerfectDodgeManager.EndPerfectDodgeFlag(this.gameObject);
        hasAttacked = true;
    }
    #endregion

    #region StatesFunctions

    public virtual void Idle_Enter(){}
    public virtual void Idle_Update(){}
    public virtual void Idle_Exit(){}
    public virtual void Move_Enter(){}
    public virtual void Move_Update(){}
    public virtual void Move_Exit(){}
    public virtual void Pursuit_Enter(){}
    public virtual void Pursuit_Update(){}
    public virtual void Pursuit_Exit(){}
    public virtual void Attack_Enter(){}
    public virtual void Attack_Update(){}
    public virtual void Attack_Exit(){}
    public virtual void WaitAttack_Enter(){}
    public virtual void WaitAttack_Update(){}
    public virtual void WaitAttack_Exit(){}
    public virtual void Dead_Enter(){}
    public virtual void Dead_Update(){}
    public virtual void Dead_Exit(){}
    public virtual void Flinch_Enter(){}
    public virtual void Flinch_Update(){}
    public virtual void Flinch_Exit(){}
    public virtual void Execution_Enter(){}
    public virtual void Execution_Update(){}
    public virtual void Execution_Exit(){}
    public virtual void Extra_Enter(){}
    public virtual void Extra_Update(){}
    public virtual void Extra_Exit(){}
    #endregion
}