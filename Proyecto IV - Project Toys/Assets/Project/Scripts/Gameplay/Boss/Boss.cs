using System;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Entity
{
    #region States
    public Boss_BaseState baseState;
    public Boss_PursuitState pursuitState;
    public Boss_WeakState weakState;
    public Boss_SpawnEnemiesState spawnEnemiesState;
    public Boss_AttackState attackState;
    #endregion

    #region Components
    [HideInInspector]public NavMeshAgent agent;
    [HideInInspector] public Boss_AnimationTriggers animationTriggers;
    [HideInInspector] public Boss_Health health;
    [HideInInspector] public Boss_Combat combat;
    #endregion

    #region Conditions
    [HideInInspector]public bool canBeDamaged;
    [HideInInspector]public bool canBeExecuted;
    #endregion

    #region Player Reference
    [HideInInspector]public Transform playerTransform;
    [HideInInspector]public Player player;
    #endregion
    
    #region References
    [Tooltip("Es OBLIGATORIO tener un spawner de enemigos para que pueda invocar enemigos el boss")]public EnemySpawner enemySpawner;
    #endregion
    
    #region Settings
    [Header("Boss Settings")]
    public int numberOfPhases = 3;
    public float bossCanBeExecutedHpThreshold = 20f;
    
    public float timeInWeakState = 10f;
    #endregion
    
    protected override void Awake()
    {
        base.Awake();
        baseState = new Boss_BaseState(this, stateMachine, "base");
        pursuitState = new Boss_PursuitState(this, stateMachine, "pursuit");
        weakState = new Boss_WeakState(this, stateMachine, "weak");
        spawnEnemiesState = new Boss_SpawnEnemiesState(this, stateMachine, "spawnEnemies");
        attackState = new Boss_AttackState(this, stateMachine, "attack");
        agent = GetComponent<NavMeshAgent>();
        animationTriggers = GetComponent<Boss_AnimationTriggers>();
        health = GetComponent<Boss_Health>();
        combat = GetComponent<Boss_Combat>();
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(baseState);
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
