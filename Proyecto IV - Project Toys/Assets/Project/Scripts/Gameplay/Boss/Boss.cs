using System;
using System.Collections;
using System.Collections.Generic;
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
    [Header("References")]
    [Tooltip("Es OBLIGATORIO tener un spawner de enemigos para que pueda invocar enemigos el boss")]public EnemySpawner enemySpawner;
    [Tooltip("El centro de la arena donde se va a mover el boss tras recuperarse")]public Transform arenaCenterTransform;
    [Tooltip("Es necesario un Box Collider para poner un lugar de spawn de los lapices")]public BoxCollider pencilAttackSpawnArea;
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
    [HideInInspector]public float slamDamage => combat.slamDamage;
    
    public int numberOfPencilsToInvoke = 5;
    public float pencilSpeed = 15f;
    public float timeToInvokePencils = 2f;
    [HideInInspector]public float pencilDamage => combat.pencilDamage;
    [HideInInspector]public List<GameObject> _projectiles;
    [HideInInspector]public Proyectil _pencilProjectile;
    
    
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
    
    #region Attack Methods
    // ---- Slam ----
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
    
    // ---- Pencil ----
    public void InstantiatePencilAttack()
    {
        
    }
    
    public IEnumerator PencilAttackCoroutine()
    {
        float timeBetweenPencils = timeToInvokePencils / numberOfPencilsToInvoke;
        for (int i = 0; i < numberOfPencilsToInvoke; i++)
        {
            _pencilProjectile = null;
            GameObject pencilInstance = Instantiate(pencilObjPrefab, GetPencilPosition(), Quaternion.identity);
            _pencilProjectile = pencilInstance.GetComponent<Proyectil>();
            if (_pencilProjectile != null)
            {
                _pencilProjectile.Release();
                _pencilProjectile.direction = Vector3.down;
                _pencilProjectile.speed = pencilSpeed;
                _pencilProjectile.OnPlayerHit += (() => Debug.Log("Player hit by pencil!"));
            }
            else
            {
                Debug.LogError("The instantiated pencil attack does not have a Proyectil component.");
            }
            yield return new WaitForSeconds(timeBetweenPencils); 
        }
        _pencilProjectile.OnProjectileDestroyed += () => isAttacking = false;
    }

    private Vector3 GetPencilPosition()
    {
        if(!pencilAttackSpawnArea) return transform.position;
        Vector3 randomPoint = new Vector3(
            UnityEngine.Random.Range(pencilAttackSpawnArea.bounds.min.x, pencilAttackSpawnArea.bounds.max.x),
            pencilAttackSpawnArea.bounds.center.y,
            UnityEngine.Random.Range(pencilAttackSpawnArea.bounds.min.z, pencilAttackSpawnArea.bounds.max.z)
        );
        return randomPoint;
    }
    #endregion
    
}
