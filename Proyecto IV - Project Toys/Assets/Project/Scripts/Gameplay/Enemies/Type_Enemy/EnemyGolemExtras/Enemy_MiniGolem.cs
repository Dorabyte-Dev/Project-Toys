using UnityEngine;

public class Enemy_MiniGolem : Enemy
{
    
    protected override void Awake() 
    {
        base.Awake();
        idleState = new Enemy_IdleState(this, stateMachine, "idle");
        pursuitState = new Enemy_PursuitState(this, stateMachine, "pursuit");
        //Attackstate??
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
        //_stateTimer -= Time.deltaTime;
    }
    
    #region Idle
    public override void Idle_Enter()
    {
        base.Idle_Enter();
    }

    public override void Idle_Update()
    {
        base.Idle_Update();
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
        agent.speed = moveSpeed;
        agent.acceleration = acceleration;
    }

    public override void Pursuit_Update()
    {
        base.Pursuit_Update();
        GetDistanceToPlayer();
        /*if (playerTransform != null)
        {
            agent.destination = playerTransform.position;
            if (distanceToPlayer < _attackPlayerRange)
            {
                ChangeEnemyState(waitAttackState);
            }
        }*/
    }

    public override void Pursuit_Exit()
    {
        base.Pursuit_Exit();
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
