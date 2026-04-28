using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : Entity
{
    #region Variables
    public static event Action OnPlayerDeath;
    #region States
    public PlayerInputSystem input { get; private set; }
    public Player_IdleState idleState { get; private set; }
    public Player_MoveState moveState { get; private set; }
    public Player_JumpState jumpState { get; private set; }
    public Player_FallState fallState { get; private set; }
    public Player_DashState dashState { get; private set; }
    //public Player_LightAttackState lightAttackState { get; private set; }
    public Player_ComboSystem comboSystemState { get; private set; }

    public Player_FlinchState flinchState { get; private set; }
    public Player_DeathState deathState { get; private set; }
    //public Player_HeavyAttackState heavyState { get; private set; }
    public Player_ExecutionState executionState { get; private set; }

    #endregion

    #region ComboSystem
    [Header("Attack and Combo Details")] 
    public AttackData currentAttack;
    public float comboResetTime = 1;
    [Range(0, 1)]public float comboBufferUnlockThreshold;
    [Range(0f, 180f)] public float comboRedirectionLimit = 45f;
    private IEnumerator _activeForgetCoroutine;
    #endregion
    
    [Header("Attack Colliders")]
        public List<AttackCollider> attackColliders;

    #region ComboBar
    [Header("Combo Bar Properties")] 
    public float comboBarHitModifier;
    public float comboBarPerfectDodgeModifier; //TODO: implement perfect dodge combo charge
    public float maxComboBarAmount;
    private bool _isComboBarFull;
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
  
    #endregion

    #region Execution
  
    [Header("Execution Properties")]
    public float executionRadius;
    private Transform _executionTarget;
    [HideInInspector] public Transform executionTarget
    {
        get => _executionTarget;
        set
        {
            if (_executionTarget == value) return;
            
            if (_executionTarget != null)
            {
                Enemy prevEnemy = _executionTarget.GetComponent<Enemy>();
                if (prevEnemy != null && prevEnemy.enemyUI != null)
                {
                    prevEnemy.OnEnemyDeath -= () => executionTarget = null;
                    prevEnemy.SetExecutionFeedback(false);
                }
            }
            
            _executionTarget = value;

            if (_executionTarget != null)
            {
                executionEnemy = _executionTarget.GetComponent<Enemy>();
                if (executionEnemy != null)
                {
                    SetExecutionEnemy(executionEnemy);
                }
            }
            else
            {
                executionEnemy = null;
            }
        }
    }
    [HideInInspector] public Enemy executionEnemy;
    [SerializeField] public LayerMask executionTargetLayer;
    public ExecutionCameraManager executionCameraManager;
    [HideInInspector] public Transform executionTransform;
    #endregion

    #region Movement

    [Header("Movement Specs")]
    public bool canMove;
    public Camera cam;
    public CharacterController ch {get; private set;}
    private float _verticalVelocity;
    private const float Gravity = -9.81f;
    public Vector2 moveInput { get; private set; }
    public Vector2 cameraMoveInput { get; private set; }
    public float jumpForce = 5;
    

    #endregion

    #region Dash & Perfect Dodge

    [Header("Dash Specs")]
    public float dashDuration = .25f;
    public float dashDistance = 20f;
    public float dashCooldown = .5f;
    private float _dashCooldownTimer;
  
    public float perfectDodgeDuration = .25f;
    public float perfectDodgeEnemyDistance = 1f;
    public MeshTrail afterimageTrail;

    #endregion

    #region Respawn

    [Header("Death Specs")] 
    [SerializeField]private Transform activeCheckpoint;

    #endregion

    #region Module References

    public Player_Combat _combat;
    public Player_Health _health;
    public Player_AnimationTriggers _animationTriggers;
    public Player_VFX _vfx;

    #endregion

    public Vector3 debug_Velocity;

    #endregion

    #region Unity Lifecycle
    protected override void Awake()
    {
        base.Awake();
        ch = GetComponent<CharacterController>();
        input = new PlayerInputSystem();
        idleState = new Player_IdleState(this, stateMachine, "Idle");
        moveState = new Player_MoveState(this, stateMachine, "Move");
        jumpState = new Player_JumpState(this, stateMachine, "jumpFall");
        fallState = new Player_FallState(this, stateMachine, "jumpFall");
        dashState = new Player_DashState(this, stateMachine, "Dash");
        flinchState = new Player_FlinchState(this, stateMachine, "Flinch");
        deathState = new Player_DeathState(this, stateMachine, "death");
        comboSystemState = new Player_ComboSystem(this, stateMachine, "AttackPressed");
        executionState = new Player_ExecutionState(this,  stateMachine, "kill");
      
        _combat = GetComponent<Player_Combat>();
        _health = GetComponent<Player_Health>();
        _vfx = GetComponent<Player_VFX>();
        _animationTriggers = GetComponent<Player_AnimationTriggers>();
        _combat.targetHit.AddListener(OnEnemyHit);
        _combat.targetHit.AddListener(_vfx.HitStop);
        
        if (executionCameraManager == null)
        {
            executionCameraManager = GetComponentInChildren<ExecutionCameraManager>();
        }
        
        PlayerReference.RegisterPlayer(this.transform);
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
        canMove = true;
    }

    protected override void Update()
    {
        base.Update();
      
        cameraMoveInput = MovementDirectionToCamera(moveInput);
      
        if (ch.isGrounded && _verticalVelocity < 0f)
            _verticalVelocity = -2f; // Pequeño valor negativo para mantenerlo pegado al suelo
        else
            _verticalVelocity += Gravity * Time.deltaTime;

        if (_isComboBarFull)
        {
            SearchForExecutionTarget();
        }
    }
  
    private void OnEnable()
    {
        input.Enable();
        input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Movement.canceled += ctx => moveInput = Vector2.zero;
    }
  
    private void OnDisable()
    {
        input.Disable();
    }
  
    #endregion
  
    #region Enemy Execution

    private void SearchForExecutionTarget()
    {
        if (executionTarget == null)
        {
            executionTarget = GetExecutionEnemy();
        }
            
        if(!executionTarget ) return;
        //executionEnemy = executionTarget.GetComponent<Enemy>();
        if(executionEnemy != null)
        {
            if (!executionEnemy.isBeingExecuted && !executionEnemy._health.isDead)
            {
                executionTarget = GetExecutionEnemy();
            }
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
                if(collider.GetComponent<Enemy>()._health.isDead || collider.GetComponent<Enemy>().isBeingExecuted) continue;
                minDistance = distance;
                nearestEnemy = collider.transform;
            }
        }
        return nearestEnemy;
    }
    
    private void SetExecutionEnemy(Enemy enemy)
    {
        executionEnemy.OnEnemyDeath += () => executionTarget = null;
        executionEnemy.SetExecutionFeedback(true);
        executionTransform = executionEnemy.playerExecutionTransform;
    }

    void SetComboBar()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ComboBarFillAmount = _comboBarAmount/maxComboBarAmount;
            _isComboBarFull = UIManager.Instance.IsComboBarFull;
        }
    }
    #endregion

    public override void DeadEntity()
    {
        base.DeadEntity();
        OnPlayerDeath?.Invoke();
        stateMachine.ChangeState(deathState);
    }
    
    public void ChangePlayerState(PlayerState newState)
    {
        stateMachine.ChangeState(newState);
    }
    #region Movement
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
    public void RotatePlayerToMatchInput()
    {
        if(cameraMoveInput.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(cameraMoveInput.x, cameraMoveInput.y) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        }
    }
    public void SetVelocity(float xVelocity, float yVelocity)
    {
        Vector3 inputDirection = new Vector3(xVelocity, _verticalVelocity, yVelocity);

        /*if (inputDirection.magnitude > 1f)
            inputDirection = inputDirection.normalized;*/

        //Vector3 moveVelocity = inputDirection * moveSpeed;

        ch.Move(inputDirection * Time.deltaTime);
    }

    public void GrantControl()
    {
        canMove = true;
    }
    
    public void RevokeControl()
    {
        canMove = false;
    }
    #endregion
    private void OnEnemyHit()
    {
        comboBarAmount += comboBarHitModifier;
    }

    #region Death&Respawn

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            activeCheckpoint = other.transform;
        }
        if (other.CompareTag("Heal"))
        {
            _health.Heal(); // Heal amount hardcoded for testing, can be changed to a variable in the future
            Destroy(other.gameObject);
        }
    }
    public void StartRespawn()
    {
        UIManager.CloseCurtain(Respawn);
    }
    public void Respawn()
    {
        ch.enabled = false; // Desactivar para poder teletransportar
        transform.position = activeCheckpoint.position;
        ch.enabled = true;
      
        CameraManager.instance.UnToggleOnCombatCamera();
        //CameraManager.instance.SwitchOffCombatCamera(activeCheckpoint.GetComponent<Checkpoint>().checkpointCamera);
        CameraManager.instance.SwitchCameraGroup(activeCheckpoint.GetComponent<Checkpoint>().checkpointCameraGroup);
        _health.ResetStats();
        comboBarAmount = 0;
        UIManager.OpenCurtain(1f);
        stateMachine.ChangeState(idleState);
        //Optimize later: reset all spawners in the scene
        foreach(EnemySpawner spawner in FindObjectsByType<EnemySpawner>(FindObjectsSortMode.None))
        {
            spawner.ResetCombat();
            spawner.GetComponent<ZoneCloser>().ResetZoneCloser();
        }
        
        foreach(EventTrigger trigger in FindObjectsByType<EventTrigger>(FindObjectsSortMode.None))
        {
            trigger.Reset();
        }
        
        CameraManager.instance.SwitchCameraGroup(activeCheckpoint.GetComponent<Checkpoint>().checkpointCameraGroup);
        
        comboBarAmount = 0;
    }
    #endregion
 
    #region ComboSystem 
    private IEnumerator ForgetPreviousAttack(float time)
    {
        float elapsedTime = 0;
        Debug.Log("Current Attack Start Forget");
        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Debug.Log("Current Attack Forgotten");
        _activeForgetCoroutine = null;
        currentAttack = null;
    }
  
    public void CheckAttackBuffer(bool isLightAttack)
    {
        if (currentAttack != null) 
        {
            AttackData nextAttack = isLightAttack ? currentAttack.nextLightAttack : currentAttack.nextHeavyAttack;
            if (nextAttack != null)
            {
                comboSystemState.isLightAttack = isLightAttack;
                Debug.Log("Checking Attack Buffer: AttackBuffer active");
            }
            else
            {
                anim.SetTrigger(isLightAttack ? "LightTrigger" : "HeavyTrigger"); //Change Later
                Debug.Log("Checking Attack Buffer: Starting Over");
            }
        }
        else
        {
            anim.SetTrigger(isLightAttack ? "LightTrigger" : "HeavyTrigger"); //Change Later
            Debug.Log("Checking Attack Buffer: Starting From Zero");
        }
    }
    #region Event Callbacks from StateMachineBehaviours
    public void OnComboStarted()
    {
        if (_activeForgetCoroutine != null)
        {
            Debug.LogWarning("Coroutine Stopped");
            StopCoroutine(_activeForgetCoroutine);
            _activeForgetCoroutine = null;
        }
    }

    public void OnComboInterrupted()
    {
        _vfx.InterruptSlash();
    }
    public void OnComboEnded()
    {
        if (stateMachine.currentState == comboSystemState) stateMachine.ChangeState(idleState); //No me gusta mucho esto, refactor prone
        _activeForgetCoroutine = ForgetPreviousAttack(comboResetTime);
        StartCoroutine(_activeForgetCoroutine);
        _vfx.InterruptSlash();
    }
    public void OnComboAttackStarted(AttackData attack)
    {
        currentAttack = attack;
        comboSystemState.attackInitialPlayerAngle = transform.eulerAngles.y;
    }
    public void OnComboAttackEnded()
    {
      
    }
    #endregion
    #endregion
    #region Attack Colliders
    public BoxCollider GetColliderUsed(AttackColliderType currentAttackColliderUsed) =>
            attackColliders.FirstOrDefault(x => x.colliderType == currentAttackColliderUsed).collider;
    #endregion
    #region Dash
    public void SetDashCooldown()
    {
        _dashCooldownTimer = dashCooldown;
        DOTween.To(() => _dashCooldownTimer, x => _dashCooldownTimer = x, 0, dashCooldown);
    }

    public bool CanDash()
    {
        return _dashCooldownTimer <= 0f;
    }
    #endregion
  
    #region GetSet
    #region Execution
    public bool CanExecute()
    {
        if (executionEnemy)
        {
            if(!executionEnemy._health.isDead) return true;
        }
        return false;
    }
    #endregion
    #region HealthUI
    public float GetCurrentHealth() => _health.currentHp;
    public float GetMaxHealth() => _health.maxHp;
    #endregion
    #endregion
}