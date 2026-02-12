using DG.Tweening;
using UnityEngine;

public class Enemy_Golem : Enemy
{
    
    [Header("States Timer Settings")]
    public float flinchTime;
    private float _stateTimer;
    
    protected override void Awake() 
    {
        base.Awake();

        idleState = new Enemy_IdleState(this, stateMachine, "idle");
        moveState = new Enemy_MoveState(this, stateMachine, "move");
        pursuitState = new Enemy_PursuitState(this, stateMachine, "pursuit");
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
        stateMachine.Initialize(idleState);
    }
    
    protected override void Update()
    {
        base.Update();
        _stateTimer -= Time.deltaTime;
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
    #region Attack
    public override void Attack_Enter()
    {
        base.Attack_Enter();
    }

    public override void Attack_Update()
    {
        base.Attack_Update();
    }

    public override void Attack_Exit()
    {
        base.Attack_Exit();
    }
    #endregion
    #region Wait Attack
    public override void WaitAttack_Enter()
    {
        base.WaitAttack_Enter();
    }

    public override void WaitAttack_Update()
    {
        base.WaitAttack_Update();
    }

    public override void WaitAttack_Exit()
    {
        base.WaitAttack_Exit();
    }
    #endregion
    #region Dead

    public override void Dead_Enter()
    {
        base.Dead_Enter();
        agent.enabled = false;
        PerfectDodgeManager.EndPerfectDodgeFlag(this.gameObject);
        if (spawner != null)
            spawner.EnemyDead(this.gameObject);
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
    #region Execution
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
    #region Extra

    public override void Extra_Enter()
    {
        base.Extra_Enter();
    }

    public override void Extra_Update()
    {
        base.Extra_Update();
    }

    public override void Extra_Exit()
    {
        base.Extra_Exit();
    }
    #endregion
}
