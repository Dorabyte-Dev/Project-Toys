public class Enemy_Ranged : Enemy
{
    private bool hasStartedAttack;
    protected override void Awake()
    {
        base.Awake();

        idleState = new Enemy_IdleState(this, stateMachine, "idle");
        moveState = new Enemy_MoveState(this, stateMachine, "move");
        pursuitState = new Enemy_PursuitState(this, stateMachine, "pursuit");
        attackState = new Enemy_AttackState(this, stateMachine, "attack");
        deadState = new Enemy_DeadState(this, stateMachine, "idle");
        flinchState = new Enemy_FlinchState(this, stateMachine, "flinch");
    }

    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }
}