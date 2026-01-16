using UnityEngine;

public class Enemy_Ranged : Enemy
{
    private bool hasStartedAttack;
    
    [Header("States Timer Settings")]
    private float _stateTimer;
    protected override void Awake()
    {
        base.Awake();

        enemyType = EnemyTypes.Ranged;

        idleState = new Enemy_IdleState(this, stateMachine, "idle");
        moveState = new Enemy_MoveState(this, stateMachine, "move");
        pursuitState = new Enemy_PursuitState(this, stateMachine, "pursuit");
        attackState = new Enemy_AttackState(this, stateMachine, "attack");
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

    #region IdleFunctions
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
    #region MoveFunctions
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
    #region WaitAttackFunctions
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
    #region DeadFunctions

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
    #endregion
    #region ExecutionFunctions

    public override void Execution_Enter()
    {
        base.Execution_Enter();
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