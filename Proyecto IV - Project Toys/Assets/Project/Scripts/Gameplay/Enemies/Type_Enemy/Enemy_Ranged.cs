using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Ranged : Enemy
{
    [Header("Ranged Enemy Prefabs and References")]
    public GameObject projectilePrefab;
    public Transform projectileSpawnPointCenter;
    
    [Header("Ranged Enemy Settings")]
    public float detectionRadius = 10f;
    private float _detectionRadius => detectionRadius * detectionRadius;    //Se utiliza el cuadrado del radio porque la distancia se calcula con .sqrMagnitude, lo que mejora el rendimiento al evitar la raíz cuadrada.
    public float fleeRadius = 5f;
    private float _fleeRadius => fleeRadius * fleeRadius;
    public float stopFleeRadius = 7.5f;
    private float _stopFleeRadius => stopFleeRadius * stopFleeRadius;
    public float timeWaitToSendWaveManagerRequest;
    private float _currentWaitTime;
    
    
    [Header("Projectile Settings")]
    public int maxProjectiles = 5;
    public float projectileRotationSpeed = 10f;
    public int projectileDamage = 20;
    public float projectileRotationRadius = 3f;
    private List<GameObject> _projectiles;
    private Proyectil _projectile;
    public float projectileSpeed;
    public float invokeProjectileSpeed = 1f;
    public float timeBetweenThrows = 2f;
    public float projectileTargetHeightOffset = 1.5f;
    
    [Header("States Timer Settings")]
    public float flinchTime;
    private float _stateTimer;
    protected override void Awake() 
    {
        base.Awake();

        idleState = new Enemy_IdleState(this, stateMachine, "idle");
        moveState = new Enemy_MoveState(this, stateMachine, "move");
        extraState = new Enemy_ExtraState(this, stateMachine, "extra");
        attackState = new Enemy_AttackState(this, stateMachine, "attack");
        waitAttackState = new Enemy_WaitAttackState(this, stateMachine, "waitAttack");
        deadState = new Enemy_DeadState(this, stateMachine, "dead");
        flinchState = new Enemy_FlinchState(this, stateMachine, "flinch");
        executionState = new Enemy_ExecutionState(this, stateMachine, "execution");
    }

    protected override void Start()
    {
        base.Start();
        _projectiles ??= new List<GameObject>(maxProjectiles);
        anim.SetFloat("invokeSpeed", invokeProjectileSpeed);
        stateMachine.Initialize(idleState);
    }
    protected override void Update()
    {
        base.Update();
        _stateTimer -= Time.deltaTime;
        Debug.Log(_projectiles.Count + " projectiles");
        RotateProjectilesAroundPivot(_projectiles);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.greenYellow;
        Gizmos.DrawWireSphere(transform.position, fleeRadius);
        Gizmos.color = Color.darkGreen;
        Gizmos.DrawWireSphere(transform.position, stopFleeRadius);
        Gizmos.color = Color.darkRed;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
    #region ProjectileFuntions
    private void InvokeProjectiles()
    {
        if(_projectiles == null) _projectiles = new List<GameObject>(maxProjectiles);
        //Logica de invocacion de proyectiles
        /*if (_projectiles.Count <= 0)
        {
            _projectiles = new List<GameObject>(maxProjectiles);
        }*/
        float angleStep = 360f / maxProjectiles;
        
        int projectilePositionsCount = _projectiles != null ? _projectiles.Count : 0;
        while (_projectiles.Count < maxProjectiles)
        {
            if (projectilePositionsCount >= maxProjectiles)
            {
                Debug.LogWarning("Start of infinite loop prevention in InvokeProjectiles");
                break;
            }
            //Calcular posicion en circulo del proyectil
            Vector3 projectilePosition = GetProjectilePosition(projectilePositionsCount, angleStep);
            //Instanciar proyectil en la posicion calculada
            _projectiles.Add(Instantiate(projectilePrefab));
            _projectiles[projectilePositionsCount].transform.position = projectilePosition;
            _projectiles[projectilePositionsCount].transform.parent = projectileSpawnPointCenter;
            projectilePositionsCount++;
        }
    }

    private Vector3 GetProjectilePosition(int step, float angleStep)
    {
        float angle = step * angleStep;
        float projectileXPosition = projectileSpawnPointCenter.position.x + Mathf.Cos(angle * Mathf.Deg2Rad) * projectileRotationRadius;
        float projectileZPosition = projectileSpawnPointCenter.position.z + Mathf.Sin(angle * Mathf.Deg2Rad) * projectileRotationRadius;
        Vector3 projectilePosition = new Vector3(projectileXPosition, projectileSpawnPointCenter.position.y, projectileZPosition);
        return projectilePosition;
    }

    private void RotateProjectilesAroundPivot(List<GameObject> projectiles)
    {
        if(projectiles == null) return;
        foreach (var projectile in projectiles)
        {
            projectile.transform.RotateAround(projectileSpawnPointCenter.position, Vector3.up, projectileRotationSpeed * Time.deltaTime);
        }
    }
    
    #endregion
    #region IdleFunctions
    /* =======================================================================================
     * STATE: IDLE
     * ======================================================================================= */
    public override void Idle_Enter()
    {
        base.Idle_Enter();
        //InvokeProjectiles();
        if(_projectiles.Count < maxProjectiles)
        {
            stateMachine.ChangeState(extraState);
        }
    }
    public override void Idle_Update()
    {
        base.Idle_Update();
        GetDistanceToPlayer();
        if (distanceToPlayer <= _fleeRadius)
        {
            stateMachine.ChangeState(moveState);
            Debug.Log("Change to moveState");
        }
        if (distanceToPlayer <= _detectionRadius)
        {
            stateMachine.ChangeState(waitAttackState);
            Debug.Log("Change to WaitState");
        }
        
    }
    public override void Idle_Exit()
    {
        base.Idle_Exit();
    }
    #endregion
    #region MoveFunctions
    /* =======================================================================================
     * STATE: MOVE (En este caso, huir del jugador)
     * ======================================================================================= */
    public override void Move_Enter()
    {
        base.Move_Enter();
        agent.isStopped = false;
    }
    public override void Move_Update()
    {
        base.Move_Update();
        GetDistanceToPlayer();
        if(distanceToPlayer >= _stopFleeRadius)
        {
            stateMachine.ChangeState(idleState);
        }
        else
        {
            FleePlayer();
        }
    }
    public override void Move_Exit()
    {
        base.Move_Exit();
        agent.isStopped = true;
        agent.ResetPath();
    }
    
    private void FleePlayer()
    {
        Vector3 fleeDirection = (transform.position - playerTransform.position).normalized;
        Vector3 fleePoint = GetFleePoint(fleeDirection);
        //Debug.Log(Vector3.Distance(fleePoint, transform.position));
        if(Vector3.Distance(fleePoint, transform.position) < 1.0f)
        {
            Vector3 rightDirection = Vector3.Cross(Vector3.up, fleeDirection).normalized;
            Vector3 leftDirection = -rightDirection;
            
            RaycastHit rightHit, leftHit;
            Physics.Raycast(transform.position, rightDirection, out rightHit, rightDirection.magnitude);
            Physics.Raycast(transform.position, leftDirection, out leftHit, leftDirection.magnitude);
            
            float rightDistance = rightHit.collider != null ? rightHit.distance : Mathf.Infinity;
            float leftDistance = leftHit.collider != null ? leftHit.distance : Mathf.Infinity;
            
            if (rightDistance > leftDistance)
            {
                fleePoint = GetFleePoint(rightDirection);
            }
            else
            {
                fleePoint = GetFleePoint(leftDirection);
            }
        }
        agent.SetDestination(fleePoint);
    }
    
    private Vector3 GetFleePoint(Vector3 direction)
    {
        Vector3 targetPoint = transform.position + direction * fleeRadius;
        NavMeshHit hit;
        /*NavMesh.SamplePosition(targetPoint, out hit, 2.0f, NavMesh.AllAreas);
        return hit.position;*/
        if (NavMesh.SamplePosition(targetPoint, out hit, 2.0f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        else
        {
            return transform.position; 
        }
    }
    #endregion
    #region ExtraFunctions
    /* =======================================================================================
     * STATE: EXTRA (En este caso, invocar proyectiles)
     * ======================================================================================= */
    public override void Extra_Enter()
    {
        base.Extra_Enter();
    }

    public override void Extra_Update()
    {
        base.Extra_Update();
        GetDistanceToPlayer();
        if (distanceToPlayer <= _fleeRadius)
        {
            stateMachine.ChangeState(moveState);
        }
    }

    public override void Extra_Exit()
    {
        base.Extra_Exit();
        InvokeProjectiles();    //Cambia de estado por animation trigger.
    }
    #endregion
    #region AttackFunctions
    /* =======================================================================================
     * STATE: ATTACK
     * ======================================================================================= */
    public override void Attack_Enter()
    {
        base.Attack_Enter();
        InvokeRepeating(nameof(ThrowProjectile),0, timeBetweenThrows);
    }
    public override void Attack_Update()
    {
        base.Attack_Update();
        LookToPlayer();
        GetDistanceToPlayer();
        if (distanceToPlayer <= _fleeRadius)
        {
            stateMachine.ChangeState(moveState);
        }
    }
    public override void Attack_Exit()
    {
        base.Attack_Exit();
        EnemyWaveManager.Instance.NotifyEnemyFinishedAttack(this);
        CancelInvoke(nameof(ThrowProjectile));
    }

    public void ThrowProjectile()
    {
        if (_projectiles.Count <= 0)
        {
            Debug.LogError("Enemy Ranged: Trying to throw a projectile but no projectiles have been found");
            return;
        }
        _projectile = _projectiles[0].GetComponent<Proyectil>();
        _projectiles.RemoveAt(0);

        //_projectile.direction = GetPlayerDirection().normalized;
        Vector3 targetPosition = playerTransform.position + Vector3.up * projectileTargetHeightOffset;
        _projectile.direction = (targetPosition - _projectile.transform.position).normalized;
        _projectile.speed = projectileSpeed;
        _projectile.transform.parent = null;
        _projectile.Release();
        _projectile.OnPlayerHit += OnProjectileHitPlayer;
        _projectile = null;

        if (_projectiles.Count <= 0)
        {
            stateMachine.ChangeState(extraState);
        }
    }

    private void OnProjectileHitPlayer()
    {
        //Debug.Log("Projectile hit the player! <b><size=20>GILIPOLLAS</size></b> ");
        playerTransform.gameObject.GetComponent<Player_Health>().TakeDamage(projectileDamage, this.transform);
    }

    #endregion
    #region WaitAttackFunctions
    /* =======================================================================================
     * STATE: WAIT ATTACK
     * ======================================================================================= */
    void CheckAndReloadProjectiles()
    {
        if (_projectiles.Count < maxProjectiles)
        {
            //InvokeProjectiles();
            stateMachine.ChangeState(extraState);
        }
    }
    public override void WaitAttack_Enter()
    {
        base.WaitAttack_Enter();
        //Detectar si tiene todos los proyectiles. En caso de no tenerlos, recargarlos.
        CheckAndReloadProjectiles();
        
        _currentWaitTime = 0;
    }

    public override void WaitAttack_Update()
    {
        base.WaitAttack_Update();
        canAttackByManager = EnemyWaveManager.Instance.RequestAttackPermission(this);
        
        _currentWaitTime += Time.deltaTime;
        
        LookToPlayer();
        
        if (_currentWaitTime >= timeWaitToSendWaveManagerRequest)
        {
            if (canAttackByManager)
            {
                stateMachine.ChangeState(attackState);
            }
            else
            {
                _currentWaitTime = 0;
            }
        }
        
        GetDistanceToPlayer();
        
        if(distanceToPlayer <= _fleeRadius)
        {
            stateMachine.ChangeState(moveState);
        }
    }
    public override void WaitAttack_Exit()
    {
        base.WaitAttack_Exit();
    }
    #endregion
    #region DeadFunctions
    /* =======================================================================================
     * STATE: DEAD
     * ======================================================================================= */
    public override void Dead_Enter()
    {
        base.Dead_Enter();
        agent.enabled = false;
        PerfectDodgeManager.EndPerfectDodgeFlag(this.gameObject);
        if (spawner != null)
            spawner.EnemyDead(this.gameObject);
        if(_projectiles.Count <= 0) return;
        foreach (var projectile in _projectiles)
        {
            Proyectil proyectil = projectile.GetComponent<Proyectil>();
            if (proyectil != null)
            {
                proyectil.DestroyProjectile();
            }
        }
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
    #region FlinchFunctions
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
            stateMachine.ChangeState(idleState);
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