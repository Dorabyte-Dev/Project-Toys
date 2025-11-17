using System;
using System.Collections;
using UnityEngine;

public class Player : Entity
{
    public static event Action OnPlayerDeath;
    public PlayerInputSystem input { get; private set; }
    public Player_IdleState idleState { get; private set; }
    public Player_MoveState moveState { get; private set; }
    public Player_JumpState jumpState { get; private set; }
    public Player_FallState fallState { get; private set; }
    public Player_DashState dashState { get; private set; }
    public Player_LightAttackState lightAttackState { get; private set; }
    public Player_DeathState deathState { get; private set; }

    [Header("Attack Details")]
    public Vector2[] attackVelocity;
    public float attackVelocityDuration = .1f;
    public float comboResetTime = 1;
    private Coroutine queuedAttackCo;

    [Header("Movement Specs")]
    public Vector2 moveInput { get; private set; }
    public Camera cam;
    public float jumpForce = 5;

    [Header("Dash Specs")]
    public float dashDuration = .25f;
    public float dashDistance = 20f;

    public float perfectDodgeDuration = .25f;
    public float perfectDodgeEnemyDistance = 1f;
    public Collider perfectDodgeCollider;
    public MeshTrail afterimageTrail;

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
        deathState = new Player_DeathState(this, stateMachine, "death");

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

    protected override void Update()
    {
        base.Update();
        Vector2 dir = new Vector2(moveInput.x, moveInput.y);

        if(dir.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        }

    }
    

    //public void CallAnimationTrigger()
    //{
    //    Debug.Log("Entro en trigger animation");
    //    stateMachine.currentState.CallAnimationTrigger();
    //}

    private void OnDisable()
    {
        input.Disable();
    }

    public override void DeadEntity()
    {
        base.DeadEntity();
        OnPlayerDeath?.Invoke();
        stateMachine.ChangeState(deathState);
    }

    public void EnterAttackStateWithDelay()
    {
        if(queuedAttackCo != null)
            StopCoroutine(queuedAttackCo);

        queuedAttackCo = StartCoroutine(EnterAttackStateWithDelayCo());
    }

    private IEnumerator EnterAttackStateWithDelayCo()
    {
        yield return new WaitForEndOfFrame();
        stateMachine.ChangeState(lightAttackState);
    }
    
    public Vector2 MovementDirectionToCamera(Vector2 moveInput)
    {
        Vector3 camZ = (transform.position - cam.transform.position).normalized;
        Vector3 xVector = Vector3.Cross(Vector3.up, camZ);
        Vector3 zVector = Vector3.Cross(xVector, Vector3.up);
        Vector3 moveVector = moveInput.x * xVector + moveInput.y * zVector;
        Vector3 moveVector2 = new Vector2(moveVector.x, moveVector.z);
        return moveVector2;
    }
    
    

}
