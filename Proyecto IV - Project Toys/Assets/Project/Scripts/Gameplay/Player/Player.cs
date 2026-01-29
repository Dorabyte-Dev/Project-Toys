using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : Entity
{
    public static event Action OnPlayerDeath;
    #region States
    public PlayerInputSystem input { get; private set; }
    public Player_IdleState idleState { get; private set; }
    public Player_MoveState moveState { get; private set; }
    public Player_JumpState jumpState { get; private set; }
    public Player_FallState fallState { get; private set; }
    public Player_DashState dashState { get; private set; }
    public Player_LightAttackState lightAttackState { get; private set; }
    public Player_DeathState deathState { get; private set; }
    public Player_HeavyAttackState heavyState { get; private set; }
    public Player_ExecutionState executionState { get; private set; }

    #endregion

    [Header("Attack Details")]
    public Vector2[] attackVelocity;
    public float attackVelocityDuration = .1f;
    public float comboResetTime = 1;
    private Coroutine queuedAttackCo;


    [Header("Combo Bar Properties")] 
    
    public float comboBarHitModifier;
    public float comboBarPerfectDodgeModifier;
    public float maxComboBarAmount;
    private float _comboBarAmount;
    public float comboBarAmount
    {
        get { return _comboBarAmount; }
        set
        {
            _comboBarAmount = Mathf.Clamp(value, 0, maxComboBarAmount);
            SetComboBar();
        }
    }
    private bool _isComboBarFull;
    
    [Header("Execution Properties")]
    public float executionRadius;
    [HideInInspector] public Transform executionTarget;
    [HideInInspector] public Enemy executionEnemy;
    [SerializeField] public LayerMask executionTargetLayer;
    
    
    [Header("Movement Specs")]
    public Vector2 moveInput { get; private set; }
    public Vector2 cameraMoveInput { get; private set; }
    public Camera cam;
    public float jumpForce = 5;

    [Header("Dash Specs")]
    public float dashDuration = .25f;
    public float dashDistance = 20f;
    public float dashCooldown = .5f;
    private float _dashCooldownTimer;
    
    public float perfectDodgeDuration = .25f;
    public float perfectDodgeEnemyDistance = 1f;
    public MeshTrail afterimageTrail;

    [Header("Death Specs")] [SerializeField]private Transform activeCheckpoint;


    public Player_Combat _combat;
    public Player_Health _health;
    public Player_AnimationTriggers _animationTriggers;
    public Player_VFX _vfx;
    
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
        heavyState = new Player_HeavyAttackState(this, stateMachine, "HeavyPressed");
        executionState = new Player_ExecutionState(this,  stateMachine, "kill");
        
        _combat = GetComponent<Player_Combat>();
        _health = GetComponent<Player_Health>();
        _vfx = GetComponent<Player_VFX>();
        _animationTriggers = GetComponent<Player_AnimationTriggers>();
        _combat.targetHit.AddListener(OnEnemyHit);
        

    }
    protected override void Start()
    {
        base.Start();
        stateMachine.Initialize(idleState);
        SetComboBar();
        if (cam == null)
        {
           cam = Camera.main;
        }
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
        cameraMoveInput = MovementDirectionToCamera(moveInput);

        if(cameraMoveInput.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(cameraMoveInput.x, cameraMoveInput.y) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        }

        if (_isComboBarFull)
        {
            executionTarget = GetExecutionEnemy();
            if(!executionTarget) return;
            executionEnemy = executionTarget.GetComponent<Enemy>();
        }

    }

    private Collider[] GetNearEnemiesCollider()
    {
        return Physics.OverlapSphere(transform.position, executionRadius, executionTargetLayer);
    }
    
    private Transform GetExecutionEnemy() 
    {
        Collider[] colliders = GetNearEnemiesCollider();
        if (colliders == null || colliders.Length == 0) return null;
        Transform nearestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (Collider collider in colliders)
        {
            float distance = Vector3.Distance(transform.position, collider.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestEnemy = collider.transform;
            }
        }

        return nearestEnemy;
    }

    public bool CanExecute() => executionTarget;

    void SetComboBar()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.FillAmount = _comboBarAmount/maxComboBarAmount;
            _isComboBarFull = UIManager.Instance.IsComboBarFull;
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
    
    public Vector2 MovementDirectionToCamera(Vector2 _moveInput)
    {
        if (cam == null)
        {
            Debug.LogWarning("No camera assigned!"); 
            return _moveInput;
        }
        Vector3 xVector = cam.transform.right;
        Vector3 zVector = Vector3.Cross(xVector, Vector3.up);
        Vector3 moveVector = _moveInput.x * xVector + _moveInput.y * zVector;
        Vector3 moveVector2 = new Vector2(moveVector.x, moveVector.z);
        return moveVector2;
    }

    private void OnEnemyHit()
    {
        comboBarAmount += comboBarHitModifier;
    }

    public void Respawn()
    {
        rb.position = activeCheckpoint.position;
        
        CameraManager.instance.UnToggleOnCombatCamera();
        CameraManager.instance.SwitchOffCombatCamera(activeCheckpoint.GetComponent<Checkpoint>().checkpointCamera);
        _health.ResetStats();
        comboBarAmount = 0;
        //stateMachine.ChangeState(idleState);
        //Optimize later: reset all spawners in the scene
        foreach(EnemySpawner spawner in FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None))
        {
            spawner.ResetCombat();
            spawner.GetComponent<ZoneCloser>().ResetZoneCloser();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            activeCheckpoint = other.transform;
        }
    }

    public void SetDashCooldown()
    {
        _dashCooldownTimer = dashCooldown;
        DOTween.To(() => _dashCooldownTimer, x => _dashCooldownTimer = x, 0, dashCooldown);
    }

    public bool CanDash()
    {
        return _dashCooldownTimer <= 0f;
    }
}
