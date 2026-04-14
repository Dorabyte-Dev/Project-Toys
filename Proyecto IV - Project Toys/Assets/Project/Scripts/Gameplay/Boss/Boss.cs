using System;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Entity
{
    #region States
    public Boss_IdleState idleState;
    public Boss_PursuitState pursuitState;
    public Boss_WeakState weakState;
    public Boss_SpawnEnemiesState spawnEnemiesState;
    public Boss_MeleeAttackState meleeAttackState;
    public Boss_RangedAttackState rangedAttackState;
    #endregion

    #region Components
    [HideInInspector]public NavMeshAgent agent;
    #endregion

    #region Conditions
    [HideInInspector]public bool canBeDamaged;
    #endregion

    #region Player Reference
    [HideInInspector]public Transform playerTransform;
    [HideInInspector]public Player player;
    #endregion
    
    protected override void Awake()
    {
        base.Awake();
        idleState = new Boss_IdleState(this, stateMachine, "idle");
        pursuitState = new Boss_PursuitState(this, stateMachine, "pursuit");
        weakState = new Boss_WeakState(this, stateMachine, "weak");
        spawnEnemiesState = new Boss_SpawnEnemiesState(this, stateMachine, "spawnEnemies");
        meleeAttackState = new Boss_MeleeAttackState(this, stateMachine, "meleeAttack");
        rangedAttackState = new Boss_RangedAttackState(this, stateMachine, "rangedAttack");
        agent = GetComponent<NavMeshAgent>();
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();
    }

    public override void DeadEntity()
    {
        base.DeadEntity();
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    public void ChangeBossState(BossState newState)
    {
        stateMachine.ChangeState(newState);
    }
    
    
}
