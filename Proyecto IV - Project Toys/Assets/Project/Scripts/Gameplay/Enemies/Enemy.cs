using System;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : Entity, IEnemyStates
{
    public Enemy_IdleState idleState;   // ESTADO DE IDLE
    public Enemy_MoveState moveState;    // ESTADO DE MOVIMIENTO\ WANDER
    public Enemy_PursuitState pursuitState;  // ESTADO DE PERSECUCION
    public Enemy_WaitAttackState waitAttackState; // ESTADO DE ESPERA DEL ATAQUE
    public Enemy_AttackState attackState;    // ESTADO DE ATTACK
    public Enemy_DeadState deadState; // ESTADO DE MUERTE
    public Enemy_FlinchState flinchState; // ESTADO DE ATURDIMIENTO
    public Enemy_ExecutionState executionState; // ESTADO DE EJECUCION

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

    [Space]
    public NavMeshAgent agent;
    public EnemySpawner spawner; //Que spawner lo ha generado
    public GameObject originalPrefab; //De que prefab se ha generado (util para la factory)
    public Entity_Health health;
    
    public int facingDirection = 1;
    
    public Transform player { get; private set; }
    
    [Header("WaveManager Specs")]
    public bool canAttackByManager; // permiso del manager para atacar

    protected override void Awake()
    {
        base.Awake();
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

        stateMachine.ChangeState(deadState);
    }
    protected override void Start()
    {
        base.Start();
        _combat = GetComponent<Entity_Combat>();
        _animationTriggers = GetComponent<Entity_AnimationTriggers>();
        _health = GetComponent<Entity_Health>();
        _vfx = GetComponent<Entity_VFX>();
        _combat.targetHit.AddListener(OnPlayerDamaged);
        //stateMachine.Initialize(idleState);
        if (spawner == null)
            Debug.LogWarning("Spawner not assigned. Check GameObject to component of EnemySpawner.cs");
    }

    public void Flip()
    {
        facingDirection *= -1;
        transform.Rotate(0f, 180f, 0f);
    }

    public void ChangeFlintState()
    {
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
    public void StopAttacking()
    {
        stateMachine.ChangeState(pursuitState);
    }
    public void PlayerDeath()
    {
        stateMachine.ChangeState(idleState);
    }

    public void ChangeEnemyState(EnemyState newState)
    {
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
    #endregion

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

    public float CheckPlayerDistance()
    {
        float distance;
        distance = (transform.position - playerTransform.position).sqrMagnitude;
        return distance;
    }
    
    public void GetDistanceToPlayer()
    {
        distanceToPlayer = CheckPlayerDistance();
    }
    #endregion

    #region Wave Manager
    
    /*public void AllowAttackFromManager()
    {
        canAttackByManager = true;
        Debug.Log($"[Enemy] {name} recibió permiso para atacar.");
    }

    public void NotifyAttackFinished()
    {
        isAttacking = false;
        canAttackByManager = false;
    
        if (EnemyWaveManager.Instance != null)
            EnemyWaveManager.Instance.NotifyEnemyFinishedAttack(this);
    
        Debug.Log($"[Enemy] {name} notificó fin de ataque.");
    }*/

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

    #endregion
}