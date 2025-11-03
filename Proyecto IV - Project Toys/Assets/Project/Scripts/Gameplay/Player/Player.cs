using UnityEngine;

public class Player : Entity
{
    public PlayerInputSystem input { get; private set; }
    public Player_IdleState idleState { get; private set; }
    public Player_MoveState moveState { get; private set; }
    public Player_JumpState jumpState { get; private set; }
    public Player_FallState fallState { get; private set; }
    public Player_DashState dashState { get; private set; }
    public Player_LightAttackState lightAttackState { get; private set; }

    [Header("Attack Details")]
    public Vector2 attackVelocity;
    public float attackVelocityDuration = .1f;

    [Header("Movement Specs")]
    public Vector2 moveInput { get; private set; }
    public float jumpForce = 5;

    [Header("Dash Specs")]
    public float dashDuration = .25f;
    public float dashDistance = 20f;

    protected override void Awake()
    {
        base.Awake();
        input = new PlayerInputSystem();

        idleState = new Player_IdleState(this, stateMachine, "Idle");
        moveState = new Player_MoveState(this, stateMachine, "Move");
        jumpState = new Player_JumpState(this, stateMachine, "jumpFall");
        fallState = new Player_FallState(this, stateMachine, "jumpFall");
        dashState = new Player_DashState(this, stateMachine, "Dash");
        lightAttackState = new Player_LightAttackState(this, stateMachine, "LightPressed");

    }
    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
    }

    private void OnEnable()
    {
        input.Enable();
        input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Movement.canceled += ctx => moveInput = Vector2.zero;

    }

    public void CallAnimationTrigger()
    {
        stateMachine.currentState.CallAnimationTrigger();
    }

    private void OnDisable()
    {
        input.Disable();
    }

   
}
