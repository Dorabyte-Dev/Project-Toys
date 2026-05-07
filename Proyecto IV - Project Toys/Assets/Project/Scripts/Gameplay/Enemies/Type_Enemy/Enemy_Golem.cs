using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Golem : Enemy
{
    [Header("Golem Settings")]
    public float flinchDamageThreshold;
    public float minimizeScaleMultiplier = 0.8f;
    [Header("Mini Golem Spawn Settings")]
    public float miniGolemSpawnRadius;
    public float miniGolemJumpPower;
    public float miniGolemJumpDuration;
    
    [Header("Detect Player Range Settings")]
    public float detectPlayerRange;
    private float _detectPlayerRange => detectPlayerRange * detectPlayerRange;
    public float attackPlayerRange;
    private float _attackPlayerRange => attackPlayerRange * attackPlayerRange;
    [Header("States Timer Settings")]
    public float flinchTime;
    private float _stateTimer;
    
    [Header("Prefabs References")]
    public GameObject miniClonPrefab;
    
    
    protected override void Awake() 
    {
        base.Awake();

        idleState = new Enemy_IdleState(this, stateMachine, "idle");
        moveState = new Enemy_MoveState(this, stateMachine, "move");
        pursuitState = new Enemy_PursuitState(this, stateMachine, "pursuit");
        attackState = new Enemy_AttackState(this, stateMachine, "attack");
        waitAttackState = new Enemy_WaitAttackState(this, stateMachine, "waitAttack");
        deadState = new Enemy_DeadState(this, stateMachine, "dead");
        flinchState = new Enemy_FlinchState(this, stateMachine, "flinch");
        executionState = new Enemy_ExecutionState(this, stateMachine, "execution");
    }
    
    protected override void Start()
    {
        base.Start();
        enemyType = EnemyType.Golem;
        stateMachine.Initialize(idleState);
    }
    
    protected override void Update()
    {
        base.Update();
        _stateTimer -= Time.deltaTime;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectPlayerRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackPlayerRange);
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);
    }
    
    #region Idle
    public override void Idle_Enter()
    {
        base.Idle_Enter();
    }

    public override void Idle_Update()
    {
        base.Idle_Update();
        GetDistanceToPlayer();
        if (distanceToPlayer <= _detectPlayerRange)
        {
            ChangeEnemyState(pursuitState);
        }
    }

    public override void Idle_Exit()
    {
        base.Idle_Exit();
    }
    #endregion
    #region Move

    public override void Move_Enter()
    {
        base.Move_Enter();
    }

    public override void Move_Update()
    {
        base.Move_Update();
    }

    public override void Move_Exit()
    {
        base.Move_Exit();
    }
    #endregion
    #region Pursuit
    public override void Pursuit_Enter()
    {
        base.Pursuit_Enter();
        agent.speed = moveSpeed;
        agent.acceleration = acceleration;
    }

    public override void Pursuit_Update()
    {
        base.Pursuit_Update();
        GetDistanceToPlayer();
        if (playerTransform != null)
        {
            agent.destination = playerTransform.position;
            if (distanceToPlayer < _attackPlayerRange)
            {
                ChangeEnemyState(waitAttackState);
            }
        }
    }

    public override void Pursuit_Exit()
    {
        base.Pursuit_Exit();
    }
    #endregion
    #region Attack
    public override void Attack_Enter()
    {
        base.Attack_Enter();
        agent.isStopped = true;
        transform.LookAt(new  Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z));
    }

    public override void Attack_Update()
    {
        base.Attack_Update();
    }

    public override void Attack_Exit()
    {
        base.Attack_Exit();
        agent.isStopped = false;
        agent.ResetPath();
    }
    #endregion
    #region Wait Attack
    public override void WaitAttack_Enter()
    {
        base.WaitAttack_Enter();
        agent.isStopped = true;
    }

    public override void WaitAttack_Update()
    {
        base.WaitAttack_Update();
        GetDistanceToPlayer();
        if(distanceToPlayer > _attackPlayerRange)
        {
            ChangeEnemyState(pursuitState);
        }
        
        canAttackByManager = EnemyWaveManager.Instance.RequestAttackPermission(this);
        if (canAttackByManager)
        {
            ChangeEnemyState(attackState);
        }
    }

    public override void WaitAttack_Exit()
    {
        base.WaitAttack_Exit();
        agent.isStopped = false;
        agent.ResetPath();
            
    }
    #endregion
    #region Dead

    public override void Dead_Enter()
    {
        base.Dead_Enter();
        /*agent.enabled = false;
        PerfectDodgeManager.EndPerfectDodgeFlag(this.gameObject);
        if (spawner != null)
            spawner.EnemyDead(this.gameObject);
        stateMachine.SwitchOffStateMachine();*/
    }

    public override void Dead_Update()
    {
        base.Dead_Update();
    }

    public override void Dead_Exit()
    {
        base.Dead_Exit();
    }
    #endregion
    #region Flinch

    public override void Flinch_Enter()
    {
        base.Flinch_Enter();
        agent.isStopped = true;
        _stateTimer = flinchTime;
        SpawnMiniGolem(GetRandomSpawnPosition(transform.position, miniGolemSpawnRadius));
        transform.localScale *= minimizeScaleMultiplier;
    }

    public override void Flinch_Update()
    {
        base.Flinch_Update();
        
        if (_stateTimer <= 0f)
        {
            ChangeEnemyState(pursuitState);
        }
    }

    public override void Flinch_Exit()
    {
        base.Flinch_Exit();
        agent.isStopped = false;
    }
    
    public override void ChangeFlinchState()
    {
        if (_health.damageReceived >= flinchDamageThreshold)
        {
            _health.damageReceived = 0f;
            ChangeEnemyState(flinchState);
        }
    }

    public void SpawnMiniGolem(Vector3 spawnPosition)
    {
        GameObject miniGolem = Instantiate(miniClonPrefab, transform.position, Quaternion.identity);
        miniGolem.transform.DOJump(spawnPosition, miniGolemJumpPower, 1, miniGolemJumpDuration).SetEase(Ease.OutQuad).OnComplete((
            () =>
            {
                miniGolem.GetComponent<Enemy_MiniGolem>().hasBorn = true;
            }));
    }

    private Vector3 GetRandomSpawnPosition(Vector3 origin, float dist)
    {
        Vector3 randomSpawnPosition = Vector3.zero;
        
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, -1);
        randomSpawnPosition = navHit.position;
        return randomSpawnPosition;
    }
    #endregion
    #region Execution
    public override void Execution_Enter()
    {
        base.Execution_Enter();
        agent.isStopped = true;
    }

    public override void Execution_Update()
    {
        base.Execution_Update();
    }

    public override void Execution_Exit()
    {
        base.Execution_Exit();
    }
    #endregion
}
/*⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀             ⡤⣖⢶⢲⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⢄⣀⣀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            ⠀⠀⠀⠀⠀⣀⣀⣠⡤⠶⠞⠛⢻⠭⠥⠷⣼⣼⢮⠿⠿⠛⠓⠒⢛⣓⣒⠋⠛⠁⠚⠒⠶⠶⠶⣶⣶⣶⣶⣶⣶⣶⣶⣶⡶⣞⣠⢏⡼⢡⠷⡶⡤⢤⡤⠴⣶⠶⠶⠶⠶⣦⠀
            ⢠⣤⣤⣶⣿⠋⠉⠀⠀⠀⠀⠀⠈⢦⠤⠤⠤⠤⠤⠤⠤⠤⠤⠤⠤⠤⠤⠤⠤⠤⠤⠤⠤⠤⠤⠤⠀⢀⠀⠀⠀⠀⠀⠀⠀⠈⣈⣉⣙⣉⣈⡇⡇⠈⣿⠀⣿⠀⠀⠀⠀⢹⡧
            ⠈⠉⠛⠻⢿⣤⣄⣀⠀⠀⠀⠀⢀⠾⠤⠤⠤⠀⠀⣀⣀⣀⣀⠀⠀⠀⠀⠀⠀⠀⢀⣀⣀⣀⣀⣀⣀⣀⣀⣀⣀⣀⣀⣀⣀⣀⣀⣀⠀⠀⠀⡇⡇⠀⣿⠀⣿⠀⠀⠀⠀⢸⡧
            ⠀⠀⠀⠀⠀⠀⠈⠙⠛⠣⢶⣤⣾⣤⣤⣤⣤⣤⣤⣤⣤⣤⣤⣤⣤⣤⣤⣤⣤⣤⣤⣤⣤⣤⣤⡠⣀⡀⠤⠤⠤⢤⣀⣀⣀⣀⡀⠀⣀⣀⣀⣇⣇⣰⣏⣸⣻⣀⣀⣀⣀⣼⠇
            ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡟⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠉⠉⠉⠉⠉⠁⠀⠀⢹⠈⠉⠀⠀⠀⠉⠉⠉⠉⠉⠉⠉⠀
            ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⡆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⣇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡼⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⢷⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⡞⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠳⣄⠀⣤⣤⣤⣤⣤⣤⣤⣤⡴⠶⠶⢦⣤⣤⣀⣀⣠⣤⣤⣤⣤⣄⣀⣀⣀⡀⣠⠏⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⢦⣿⠀⢀⠀⢀⣧⣀⡆⠀⠀⠀⢰⠀⠀⠉⠉⢀⣆⣀⣰⡈⢉⠉⠉⢹⣷⠋⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⣿⠀⢀⡟⠃⠀⣀⠈⠳⡀⠙⠀⠀⠀⢀⠖⠉⠀⣀⠀⠙⠺⡅⠀⣾⠃⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⠀⢸⡀⠀⢺⣫⠗⢀⠇⠀⠀⠀⠀⣇⠀⠀⡞⠋⢹⡀⠀⣇⠀⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⠀⠀⠉⠒⠢⠔⠒⠋⠀⠀⣤⡖⣤⠘⠦⣀⡉⠒⠋⣠⠜⠃⢠⡿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⠀⠀⠀⠀⠀⠀⠀⠀⢀⣀⣈⣉⣀⠀⠀⠀⠉⠉⠉⠀⠀⠀⢸⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⠀⠀⠀⠀⠀⠀⣠⠞⠉⢸⠀⢀⡏⠙⠢⡀⠀⠀⠀⠀⠀⠀⢸⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⠀⠀⠀⠀⢀⡞⢹⣂⣀⣸⠀⢸⣁⣀⣀⡟⣆⠀⠀⠀⠀⠀⢸⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡟⠀⠀⠀⢠⠏⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⡄⠀⠀⠀⠀⡿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⡗⠰⠄⠀⡜⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢹⠀⠀⠀⠀⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⣷⢠⡀⠀⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⠀⠀⠀⢀⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢹⠀⠄⠀⠧⢄⣀⠤⠖⠒⠋⡍⠉⠒⠒⠦⠤⠖⠋⠀⠀⡄⢸⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⠉⠉⢉⠙⠛⠛⠛⠛⠛⢻⡒⣲⠒⠒⠒⠒⠒⠒⠶⠶⠶⣾⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⠀⠀⢈⠀⠀⠀⠀⠀⠀⣰⠋⠹⡄⠀⠀⠀⠀⠀⠛⠀⠀⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠳⣄⡰⠋⠀⠀⠀⠀⠀⠀⠀⠀⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠛⠛⠛⠛⠛⢻⡟⠛⠛⠲⠶⠾⠶⠦⣤⡴⣶⠶⠶⠶⠶⠶⠇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢸⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡏⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
            ⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠠⠤⠤⠼⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠧⠤⠤⠄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀*/