using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Melee : Enemy
{
    [Space(30)]
    private bool hasStartedAttack;
    [Header("Enemy Melee Settings")]
    public float range;
    public float attackRange;
    public float waitTime;
    public float attackAcceleration;
    public float attackSpeed;
    public float pursuitPlayerRange;
    public float attackPlayerRange;
    [Header("Orbit Settings")]
    public float orbitDistance;
    public float orbitSpeed;
    [HideInInspector]public float orbitAngle;
    [Header("States Timer Settings")]
    public float flinchTime;
    public float idleTime;
    private float _stateTimer;

    #region WaitAttackVariables
    private Vector3 _attackPoint;
    private Vector3 _lastPlayerPosition;
    private Vector3 _currentPosition;
    private Vector3 _directionToPlayer;
    private float _currentTime;
    #endregion
    protected override void Awake()
    {
        base.Awake();

        enemyType = EnemyTypes.Melee;   

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
        stateMachine.Initialize(idleState);
    }

    protected override void Update()
    {
        base.Update();
        _stateTimer -= Time.deltaTime;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);
    }

    #region IdleFunctions
    /* =======================================================================================
     * STATE: IDLE
     * ======================================================================================= */
    public override void Idle_Enter()
    {
        base.Idle_Enter();
        _stateTimer = idleTime;
    }

    public override void Idle_Update()
    {
        base.Idle_Update();
        if (_stateTimer <= 0f)
        {
            stateMachine.ChangeState(moveState);
        }
    }
    public override void Idle_Exit()
    {
        base.Idle_Exit();
    }

    #endregion
    #region MoveFunctions
    /* =======================================================================================
     * STATE: MOVE
     * ======================================================================================= */
    private void GoToRandomPoint()
    {
        if (agent.pathPending || !agent.isOnNavMesh)
            return;

        agent.destination = RandomNavSphere(transform.position, range, -1);
    }

    private bool HasReachedDestination()
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }
    public override void Move_Enter()
    {
        base.Move_Enter();
        GoToRandomPoint();
    }
    public override void Move_Update()
    {
        base.Move_Update();
        GetDistanceToPlayer();
        if (!groundDetected)
        {
            stateMachine.ChangeState(idleState);
            Flip();
            return;
        }

        if (distanceToPlayer < pursuitPlayerRange)
        {
            stateMachine.ChangeState(pursuitState);
        }
        // Verificar si ha llegado al destino
        if (HasReachedDestination())
        {
            stateMachine.ChangeState(idleState);
            //GoToRandomPoint();
        }
    }

    public override void Move_Exit()
    {
        base.Move_Exit();
    }
    #endregion
    #region PursuitFunctions
    /* =======================================================================================
     * STATE: PURSUIT
     * ======================================================================================= */
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
        // Perseguir al jugador
        if (playerTransform != null)
        {
            agent.destination = playerTransform.position;
            if (distanceToPlayer < attackPlayerRange)
            {
                stateMachine.ChangeState(waitAttackState);
            }
            else if (distanceToPlayer > pursuitPlayerRange)
            {
                stateMachine.ChangeState(moveState);
            }
        }
    }
    public override void Pursuit_Exit()
    {
        base.Pursuit_Exit();
    }

    #endregion
    #region AttackFunctions
    /* =======================================================================================
     * STATE: ATTACK
     * ======================================================================================= */
    public void SetParametersAttack(Vector3 currentP, Vector3 attackP)
    {
        _currentPosition = currentP;
        _attackPoint = attackP;
    }
    public override void Attack_Enter()
    {
        base.Attack_Enter();
        agent.speed = attackSpeed;
        agent.acceleration = attackAcceleration;
        agent.destination = _attackPoint;
    }
    public override void Attack_Update()
    {
        base.Attack_Update();

        // Verificar si llegó al punto de ataque
        if (HasReachedDestination())
        {
            StopAttacking();
        }
    }

    public override void Attack_Exit()
    {
        base.Attack_Exit();
        //Debug.Log($"[Enemy_AttackState] {enemy.name} sale del ataque.");
        
        // IMPORTANTE: Notificar al manager que terminó el ataque
        //enemy.NotifyAttackFinished();
        EnemyWaveManager.Instance.NotifyEnemyFinishedAttack(this);
        
        hasAttacked = false;
        
        // Restaurar velocidades normales
        agent.speed = moveSpeed;
        agent.acceleration = acceleration;
    }
    #endregion
    #region WaitAttackFunctions
    /* =======================================================================================
     * STATE: WAIT ATTACK
     * ======================================================================================= */
    
    private void StopSmooth()
    {
        agent.destination = _currentPosition + (_lastPlayerPosition - _currentPosition) * 0.25f;
    }

    private Vector3 AttackPointToPlayer()
    {
        _directionToPlayer = (playerTransform.position - transform.position).normalized;
        return transform.position + _directionToPlayer * attackRange;
    }

    private void OrbitAroundPlayer()
    {
        // 1. Aumentar el �ngulo de �rbita con el tiempo
        // El Time.deltaTime * OrbitSpeed hace que el punto rote.
        orbitAngle += Time.deltaTime * orbitSpeed;

        // Asegurar que el �ngulo no se desborde (opcional, por limpieza)
        if (orbitAngle > 360f)
        {
            orbitAngle -= 360f;
        }

        // 2. Convertir el �ngulo a radianes para las funciones trigonom�tricas
        // Los �ngulos en C# suelen ser en grados.
        float angleInRad = orbitAngle * Mathf.Deg2Rad;

        // 3. Calcular la nueva posici�n de destino (en un plano 2D, X y Z)
        Vector3 targetPosition;
        targetPosition.x = playerTransform.position.x + orbitDistance * Mathf.Cos(angleInRad);
        targetPosition.y = playerTransform.position.y; // Mantener la altura del suelo
        targetPosition.z = playerTransform.position.z + orbitDistance * Mathf.Sin(angleInRad);


        // 4. Mover el NavMeshAgent al nuevo destino
        agent.destination = targetPosition;
    }

    private float InitializeOrbitAngle()
    {
        Vector3 directionPlayerToEnemy = transform.position - playerTransform.position;
        float angleInRadians = Mathf.Atan2(directionPlayerToEnemy.z, directionPlayerToEnemy.x);
        return angleInRadians * Mathf.Rad2Deg;
    }
    public override void WaitAttack_Enter()
    {
        base.WaitAttack_Enter();
        //enemy.isAttacking = true;

        // Guardar posiciones
        _currentPosition = transform.position;
        _lastPlayerPosition = playerTransform != null ? playerTransform.position : transform.forward;

        // Calcular punto de ataque (hacia donde va a embestir)
        
        StopSmooth();
        //attackPoint = lastPlayerPosition;

        // DETENER al enemigo durante la carga del ataque
        //enemy.agent.isStopped = true;
        //enemy.agent.velocity = Vector3.zero;

        // Mirar hacia el objetivo
        LookToPlayer();

        // Animaci�n de carga
        //anim.Play("WaitAttack");

        _currentTime = 0;
        //enemy.hasStartedAttack = false;
        
        
        orbitAngle = InitializeOrbitAngle();
    }

    public override void WaitAttack_Update()
    {
        base.WaitAttack_Update();
        canAttackByManager = EnemyWaveManager.Instance.RequestAttackPermission(this);
        _currentTime += Time.deltaTime;
        
        OrbitAroundPlayer();
        LookToPlayer();
        if (_currentTime >= waitTime)
        {
            if (canAttackByManager)
            {
                //Debug.Log("Cambiar a estado de ataque");
                _attackPoint = AttackPointToPlayer();
                SetParametersAttack(_currentPosition, _attackPoint);
            
                stateMachine.ChangeState(attackState);
            }
            else
            {
                _currentTime = 0;
            }
            
        }
    }

    public override void WaitAttack_Exit()
    {
        base.WaitAttack_Exit();
        agent.isStopped = false;
    }

    #endregion
    #region DeadFunctions
    /* =======================================================================================
     * STATE: DEAD
     * ======================================================================================= */
    public override void Dead_Enter()
    {
        base.Dead_Enter();
        Debug.Log("Entro en deadState");
        //anim.enabled = false;
        agent.enabled = false;
        PerfectDodgeManager.EndPerfectDodgeFlag(this.gameObject);
        if (spawner != null)
            spawner.EnemyDead(this.gameObject);
        //stateMachine.SwitchOffStateMachine();
        //enemy.EnemyDeathTest();
    }
    public override void Dead_Update()
    {
        base.Dead_Update();
    }

    public override void Dead_Exit()
    {
        base.Dead_Exit();
        Debug.Log("Salgo de deadState");
    }
    #endregion
    #region FLinchFunctions
    /* =======================================================================================
     * STATE: FLINCH
     * ======================================================================================= */
    public override void Flinch_Enter()
    {
        base.Flinch_Enter();
        agent.isStopped = true;
        _stateTimer = flinchTime;
    }

    public override void Flinch_Update()
    {
        base.Flinch_Update();
        if (_stateTimer <= 0f)
        {
            GetDistanceToPlayer();
            if (health.currentHp <= 0)
            {
                DeadEntity();
                return;
            }
            stateMachine.ChangeState(moveState);
        }
    }
    public override void Flinch_Exit()
    {
        base.Flinch_Exit();
        agent.isStopped = false;
    }

    #endregion
    #region ExecutionFunctions
    /* =======================================================================================
     * STATE: EXECUTION
     * ======================================================================================= */
    public override void Execution_Enter()
    {
        base.Execution_Enter();
        Debug.Log("Entered Enemy_ExecutionState");
        agent.isStopped = true;
    }
    public override void Execution_Update()
    {
        base.Execution_Update();
        this.gameObject.transform.DOShakeScale(1f, 0.1f, 5).OnComplete(() =>
        {
            stateMachine.ChangeState(deadState);
        });
    }
    public override void Execution_Exit()
    {
        base.Execution_Exit();
    }

    #endregion
}