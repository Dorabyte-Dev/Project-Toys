using System;
using System.Collections;
using UnityEngine;

public class Enemy_MiniGolem : Enemy
{
    [Header("Mini Golem Stats")]
    public float detectPlayerRange;
    private float _detectPlayerRange => detectPlayerRange * detectPlayerRange;
    public float attackPushBackStrength;
    private float _originalPushDuration;
    public float attackPushBackDuration;
    private float _originalPushStrength;

    [HideInInspector] public bool hasBorn;
    
    [Header("Test Config")]
    public bool testingMiniGolem;

    private bool isInRecoil;
    
    protected override void Awake() 
    {
        base.Awake();
        idleState = new Enemy_IdleState(this, stateMachine, "idle");
        pursuitState = new Enemy_PursuitState(this, stateMachine, "pursuit");
        deadState = new Enemy_DeadState(this, stateMachine, "dead");
        flinchState = new Enemy_FlinchState(this, stateMachine, "flinch");
        executionState = new Enemy_ExecutionState(this, stateMachine, "execution");
        extraState = new Enemy_ExtraState(this, stateMachine, "extra");
    }
    
    protected override void Start()
    {
        base.Start();
        enemyType = EnemyType.MiniGolem;
        agent.speed = moveSpeed;
        agent.acceleration = acceleration;
        _originalPushDuration = _vfx.pushWaitDuration;
        _originalPushStrength = _vfx.pushStrength;
        if (!testingMiniGolem)
        {
            stateMachine.Initialize(extraState);
        }
        else
        {
            stateMachine.Initialize(idleState);
        }
    }
    
    protected override void Update()
    {
        base.Update();
        
        //_stateTimer -= Time.deltaTime;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectPlayerRange);
    }
    
    #region Damage Dealing

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isAttacking && !isInRecoil)
        {
            Debug.Log("Enemy_MiniGolem hit the player");
            playerTransform.gameObject.GetComponent<Player_Health>().TakeDamage(combat.GetBaseDamage() ,this.transform);
            PushBack();
        }
    }

    private void PushBack()
    {
        Vector3 pushDirection = (transform.position - playerTransform.position).normalized;
        
        StartCoroutine(AttackPushBack(pushDirection));
    }

    private IEnumerator AttackPushBack(Vector3 pushDirection)
    {
        if (rb != null)
        {
            Debug.Log("PushFeedback started for " + this.gameObject.name);
            
            agent.ResetPath();
            agent.enabled = false;
            rb.isKinematic = false;
            rb.AddForce(pushDirection * attackPushBackStrength, ForceMode.VelocityChange);
            isInRecoil = true;
            yield return new WaitUntil(() => rb.linearVelocity.magnitude <= 0.001f);
            
            yield return new WaitForSeconds(attackPushBackDuration);
            
            Debug.Log("PushFeedback finished for " + this.gameObject.name);
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.linearVelocity = Vector3.zero;
                agent.Warp(transform.position);
                agent.enabled = true;
                isInRecoil = false;
            }
            else
            {
                Debug.LogError("Rigidbody is null in ResetPushFeedback");
            }
            
        }
        else
        {
            Debug.LogError("Rigidbody is null in PushFeedback");
        }
    }
    
    

    #endregion

    #region Extra

    public override void Extra_Enter()
    {
        base.Extra_Enter();
    }

    public override void Extra_Update()
    {
        base.Extra_Update();
        if (hasBorn)
        {
            ChangeEnemyState(idleState);
        }
    }
    
    public override void Extra_Exit()
    {
        base.Extra_Exit();
    }

    #endregion
    #region Idle
    public override void Idle_Enter()
    {
        base.Idle_Enter();
        if (agent.hasPath)
        {
            agent.ResetPath();
        }
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
    #region Pursuit
    public override void Pursuit_Enter()
    {
        base.Pursuit_Enter();
        isAttacking = true;
    }

    public override void Pursuit_Update()
    {
        base.Pursuit_Update();
        GetDistanceToPlayer();
        if (playerTransform != null && agent.isActiveAndEnabled)
        {
            agent.destination = playerTransform.position;
        }

        if (distanceToPlayer > _detectPlayerRange)
        {
            ChangeEnemyState(idleState);
        }
    }

    public override void Pursuit_Exit()
    {
        base.Pursuit_Exit();
        isAttacking = false;
    }
    #endregion
    #region Dead

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
    #region Flinch

    public override void Flinch_Enter()
    {
        base.Flinch_Enter();
    }

    public override void Flinch_Update()
    {
        base.Flinch_Update();
    }

    public override void Flinch_Exit()
    {
        base.Flinch_Exit();
        
    }
    
    public override void ChangeFlinchState()
    {
        
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
