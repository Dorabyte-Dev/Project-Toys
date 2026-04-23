using System;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(NavMeshAgent),typeof(Boss_AnimationTriggers), typeof(Boss_Health))]
[RequireComponent(typeof(Boss_Combat))]
public class Boss : Entity
{
    #region States
    public Boss_BaseState baseState;
    public Boss_PursuitState pursuitState;
    public Boss_WeakState weakState;
    public Boss_SpawnEnemiesState spawnEnemiesState;
    public Boss_ChargeAttackState chargeAttackState;
    public Boss_SlamAttackState slamAttackState;
    public Boss_PencilAttackState pencilAttackState;
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
    [HideInInspector]public bool isAttacking;
    #endregion

    #region Player Reference
    [HideInInspector]public Transform playerTransform;
    [HideInInspector]public Player player;
    #endregion
    
    #region References
    [Tooltip("Es OBLIGATORIO tener un spawner de enemigos para que pueda invocar enemigos el boss")]public EnemySpawner enemySpawner;
    [Tooltip("El centro de la arena donde se va a mover el boss tras recuperarse")]public Transform arenaCenterTransform;
    #endregion

    #region Prefabs
    [Header("Prefabs")] 
    public GameObject slamObjPrefab;
    public GameObject pencilObjPrefab;
    #endregion
    
    #region Settings
    [Header("Boss Settings")]
    public int maxAttacksBeforeChargeAttack = 3;
    public float bossCanBeExecutedHpThreshold = 20f;
    
    public float slamSpeed = 10f;
    
    public float timeInWeakState = 10f;
    public float timeInIdle = 4f;
    #endregion
    
    protected override void Awake()
    {
        base.Awake();
        baseState = new Boss_BaseState(this, stateMachine, "base");
        pursuitState = new Boss_PursuitState(this, stateMachine, "pursuit");
        weakState = new Boss_WeakState(this, stateMachine, "weak");
        spawnEnemiesState = new Boss_SpawnEnemiesState(this, stateMachine, "spawnEnemies");
        chargeAttackState = new Boss_ChargeAttackState(this, stateMachine, "chargeAttack");
        slamAttackState = new Boss_SlamAttackState(this, stateMachine, "slamAttack");
        pencilAttackState = new Boss_PencilAttackState(this, stateMachine, "pencilAttack");
        
        agent = GetComponent<NavMeshAgent>();
        animationTriggers = GetComponent<Boss_AnimationTriggers>();
        health = GetComponent<Boss_Health>();
        combat = GetComponent<Boss_Combat>();
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(baseState);
        playerTransform = PlayerReference.playerTransform;
        if (playerTransform == null)
        {
            Debug.LogError("No player found");
            return;
        }
        player = playerTransform.GetComponent<Player>();
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
        enemySpawner.endCombat.AddListener(() => stateMachine.ChangeState(baseState));
    }

    private void OnDisable()
    {
        enemySpawner.endCombat.RemoveListener(() => stateMachine.ChangeState(baseState));
    }

    public void ChangeBossState(BossState newState)
    {
        stateMachine.ChangeState(newState);
    }
    
    #region Various Methods
    public void InstantiateSlamAttack()
    {
        GameObject slamInstance = Instantiate(slamObjPrefab, transform.position, Quaternion.identity);
        Proyectil slamProyectil = slamInstance.GetComponent<Proyectil>();
        if (slamProyectil != null)
        {
            isAttacking = false;
            slamProyectil.Release();
            slamProyectil.direction = (playerTransform.position - transform.position).normalized;
            slamProyectil.speed = slamSpeed;
        }
        else
        {
            Debug.LogError("The instantiated slam attack does not have a Proyectil component.");
        }
    }
    #endregion
    
}
