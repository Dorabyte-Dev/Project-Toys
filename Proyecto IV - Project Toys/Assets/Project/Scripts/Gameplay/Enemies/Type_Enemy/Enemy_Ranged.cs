using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy_Ranged : Enemy
{
    [Header("Ranged Enemy Prefabs and References")]
    public GameObject projectilePrefab;
    public Transform projectileSpawnPointCenter;
    
    [Header("Ranged Enemy Settings")]
    public float detectionRadius = 10f;
    public float fleeRadius = 5f;
    public float timeWaitToSendWaveManagerRequest;
    private float _currentWaitTime;
    
    [Header("Projectile Settings")]
    public int maxProjectiles = 5;
    public float projectileRotationSpeed = 10f;
    public float projectileRotationRadius = 3f;
    private List<GameObject> _projectiles;
    private Proyectil _projectile;
    public float projectileSpeed;
    
    
    [Header("States Timer Settings")]
    public float flinchTime;
    private float _stateTimer;
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
        stateMachine.Initialize(idleState);
    }
    protected override void Update()
    {
        base.Update();
        _stateTimer -= Time.deltaTime;
        RotateProjectilesAroundPivot(_projectiles);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.darkRed;
        Gizmos.DrawWireSphere(transform.position, fleeRadius);
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
        InvokeProjectiles();
    }
    public override void Idle_Update()
    {
        base.Idle_Update();
        GetDistanceToPlayer();
        if (distanceToPlayer <= fleeRadius)
        {
            stateMachine.ChangeState(moveState);
            Debug.Log("Change to moveState");
        }
        else if (distanceToPlayer <= detectionRadius)
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
    #region PursuitFunctions
    /* =======================================================================================
     * STATE: PURSUIT
     * ======================================================================================= */
    public override void Pursuit_Enter()
    {
        base.Pursuit_Enter();
    }
    public override void Pursuit_Update()
    {
        base.Pursuit_Update();
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
    public override void Attack_Enter()
    {
        base.Attack_Enter();
        InvokeRepeating(nameof(ThrowProjectile),0, 2);
    }
    public override void Attack_Update()
    {
        base.Attack_Update();
        LookToPlayer();
        
        //Logica de ataque a distancia (Lanza 1 de 5 proyectiles cada 2 segundos)
        
    }
    public override void Attack_Exit()
    {
        base.Attack_Exit();
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

        _projectile.direction = GetPlayerDirection().normalized;
        _projectile.speed = projectileSpeed;
        _projectile.transform.parent = null;
        _projectile.Release();
        _projectile = null;
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
            InvokeProjectiles();
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
        
        //Detectar si puede atacar (Si tiene proyectiles y si el manager le dio permiso)
        if (_currentWaitTime >= timeWaitToSendWaveManagerRequest)
        {
            if (canAttackByManager)
            {
                //Cambiar al estado de ataque
                
                stateMachine.ChangeState(attackState);
            }
            else
            {
                _currentWaitTime = 0;
            }
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