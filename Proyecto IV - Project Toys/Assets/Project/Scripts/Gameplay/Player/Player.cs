using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
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
    public Player_DeathState deathState { get; private set; }
    //public Player_HeavyAttackState heavyState { get; private set; }
    public Player_ExecutionState executionState { get; private set; }

    #endregion

    #region ComboSystem
    [Header("Attack and Combo Details")] 
    [Tooltip("Reference for the current attack played by the ComboSystemState. It is remembered by the player for a bit after exiting combo statem.")]
    public AttackData currentAttack;
    
    [Tooltip("Time that player remembers last combo attack.")]
    public float comboResetTime = 1;
    
    [Tooltip("Normalized time to unlock player input in combos. Input is ignored before this threshold.")]
    [Range(0, 1)]public float comboBufferUnlockThreshold;
    
    [Tooltip("Max angle player can rotate during each attack of the combo.")]
    public float comboRedirectionLimit;
    private IEnumerator _activeForgetCoroutine; //Change to Tween when possible

    [Header("Attack Colliders")]
    //public Dictionary<AttackCollider, Collider> attackColliders;
    public List<AttackCollider> attackColliders;
    
    #endregion

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

    [HideInInspector]
    public Transform executionTarget
    {
        get
        {
            return _executionTarget;
        }
        set
        {
            if (_executionTarget == value) return;
            
            if (_executionTarget != null)
            {
                Enemy prevEnemy = _executionTarget.GetComponent<Enemy>();
                if (prevEnemy != null && prevEnemy.enemyUI != null)
                {
                    prevEnemy.enemyUI.HideExecutionUI();
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
    private Enemy _lastExecutionEnemy;
    [SerializeField] public LayerMask executionTargetLayer;
    public ExecutionCameraManager executionCameraManager; 
    [HideInInspector] public Transform executionTransform;
    
    #endregion

    #region Movement

    [Header("Movement Specs")]
    public Vector2 moveInput { get; private set; }
    public Vector2 cameraMoveInput { get; private set; }
    public Camera cam;
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

    #region Unity Lifecycle
    protected override void Awake()
    {
        base.Awake();
        input = new PlayerInputSystem();
        idleState = new Player_IdleState(this, stateMachine, "Idle");
        moveState = new Player_MoveState(this, stateMachine, "Move");
        jumpState = new Player_JumpState(this, stateMachine, "jumpFall");
        fallState = new Player_FallState(this, stateMachine, "jumpFall");
        dashState = new Player_DashState(this, stateMachine, "Dash");
        deathState = new Player_DeathState(this, stateMachine, "death");
        comboSystemState = new Player_ComboSystem(this, stateMachine, "AttackPressed");
        executionState = new Player_ExecutionState(this,  stateMachine, "kill");
        
        _combat = GetComponent<Player_Combat>();
        _health = GetComponent<Player_Health>();
        _vfx = GetComponent<Player_VFX>();
        _animationTriggers = GetComponent<Player_AnimationTriggers>();
        _combat.targetHit.AddListener(OnEnemyHit);
        if (executionCameraManager == null)
        {
            executionCameraManager = GetComponentInChildren<ExecutionCameraManager>();
        }

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

    protected override void Update()
    {
        base.Update();
        
        cameraMoveInput = MovementDirectionToCamera(moveInput);

        if (_isComboBarFull)
        {
            executionTarget = GetExecutionEnemy();
            /*if(!executionTarget) return;
            executionEnemy = executionTarget.GetComponent<Enemy>();*/
            /*if(executionTarget == _lastExecutionTarget) return;
            
            _lastExecutionTarget = executionTarget;*/
            
        }
        else if (executionTarget != null)
        {
            executionTarget = null;
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

    private void SetExecutionEnemy(Enemy enemy)
    {
        executionEnemy.enemyUI.ShowExecutionUI();
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
            //float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        
            transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        }
    }
    

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
    }
    
    public void Respawn()
    {
        rb.position = activeCheckpoint.position;
        
        CameraManager.instance.UnToggleOnCombatCamera();
        //CameraManager.instance.SwitchOffCombatCamera(activeCheckpoint.GetComponent<Checkpoint>().checkpointCamera);
        CameraManager.instance.SwitchCameraGroup(activeCheckpoint.GetComponent<Checkpoint>().checkpointCameraGroup);
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
                //Debug.Log("Checking Attack Buffer: AttackBuffer active");
            }
            else
            {
                anim.SetTrigger(isLightAttack ? "LightTrigger" : "HeavyTrigger"); //Change Later
                //Debug.Log("Checking Attack Buffer: Starting Over");
            }
        }
        else
        {
            anim.SetTrigger(isLightAttack ? "LightTrigger" : "HeavyTrigger"); //Change Later
            //Debug.Log("Checking Attack Buffer: Starting From Zero");
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
        stateMachine.ChangeState(idleState);
        _activeForgetCoroutine = ForgetPreviousAttack(comboResetTime);
        StartCoroutine(_activeForgetCoroutine);
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

    #region Debug

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.orangeRed;
        Gizmos.DrawWireSphere(transform.position, executionRadius);
    }

    #endregion
    #region GetSet
    #region Execution
    public bool CanExecute() => executionTarget;
    #endregion
    #region HealthUI
    public float GetCurrentHealth() => _health.currentHp;
    public float GetMaxHealth() => _health.maxHp;
    #endregion
    #endregion

    public BoxCollider GetColliderUsed(AttackColliderType currentAttackColliderUsed)
    {
        return attackColliders.FirstOrDefault(x => x.colliderType == currentAttackColliderUsed).collider;
    }
}
