using DG.Tweening;
using UnityEngine;

/// <summary>
/// Manages the player's dash state, handling both regular dashes and perfect dodge mechanics.
/// Includes slope transition handling and curved movement for perfect dodges.
/// </summary>
public class Player_DashState : Player_GroundedState
{
    // Physics and movement variables
    private Vector3 _forToApply;        // Force vector to apply during dash
    private float _dashSpeed;            // Calculated dash speed based on distance and duration
    
    // Slope handling
    private bool _enteredSlope;          // Tracks if player started dash on a slope
    private bool _switchSlope;           // Creo que no sirve pero me da miedo quitarla, lo miraré cuando no haya prisas
    
    // Perfect dodge system
    private bool _isPerfectDodge;        // Whether this dash is a perfect dodge
    
    public Player_DashState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    
        stateTimer = player.dashDuration;
        player.SetDashCooldown();
        
        Debug.Log("Entering Dash State: " + stateTimer);
    
        _isPerfectDodge = PerfectDodgeManager.IsPerfectDodge();
    
        if (_isPerfectDodge)
        {
            PerfectDodge(PerfectDodgeManager.GetPerfectDodgeEnemy());
        }
        else
        {
            Vector3 playerDirection = GetDashDirection();
            _dashSpeed = player.dashDistance / player.dashDuration;
            _forToApply = playerDirection * _dashSpeed;
        
            // Guardamos la dirección, el Move lo aplica SetVelocity en Update
            _enteredSlope = player.OnGround();
        }
        PerfectDodgeManager.WipePerfectDodgeFlags();
    }
    
    public override void Update()
    {
        base.Update();

        stateTimer -= Time.deltaTime;
        //Debug.Log("Dash State Timer: " + stateTimer);
        if (!_isPerfectDodge && player.CheckFallBreak(player.transform.position + _forToApply * Time.deltaTime))
        {
            player.ch.Move(_forToApply * Time.deltaTime);
        }

        if (stateTimer < 0f || !player.canMove)
        {
            stateMachine.ChangeState(player.idleState);
        }
    }
    
    public override void Exit()
    {
        base.Exit();
        player.SetVelocity(0f, 0f);
    }

    /// <summary>
    /// Calculates the dash direction based on camera-relative input.
    /// Returns forward direction if no input is detected.
    /// Adjusts direction for slopes to maintain ground-following movement.
    /// </summary>
    private Vector3 GetDashDirection()
    {
        Vector2 inputVector = player.cameraMoveInput;
        
        // If no input, dash forward in current facing direction
        if (inputVector.magnitude < 0.1f)
            return player.transform.forward;

        // Convert 2D input to 3D world direction
        Vector3 direction = new Vector3(inputVector.x, 0f, inputVector.y).normalized;
            
        return direction;
    }

    /// <summary>
    /// Executes a perfect dodge maneuver when dashing near an enemy.
    /// Creates a curved path around the enemy with slow-motion and visual effects.
    /// </summary>
    /// <param name="enemy">The enemy GameObject to dodge around</param>
    void PerfectDodge(GameObject enemy)
    {
        player.comboBarAmount += player.comboBarPerfectDodgeModifier;
        SoundManager.instance.Play("PerfectDodge");
        
        Vector3 direction = new Vector3(
            enemy.transform.position.x - player.transform.position.x,
            0f,
            enemy.transform.position.z - player.transform.position.z
        );

        Vector3 pDodgePosition = new Vector3(
            enemy.transform.position.x,
            player.transform.position.y,
            enemy.transform.position.z
        ) + direction.normalized * player.perfectDodgeEnemyDistance;

        Vector3 pDodgeCurvePoint = player.transform.position 
                                    + direction / 2 
                                    + Vector3.Cross(direction, Vector3.up).normalized * direction.magnitude / 1.25f;

        CameraManager.instance.ToggleZoom();
        Time.timeScale = 0.25f;
        player.SetInvincible((player.perfectDodgeDuration * 1.2f) / Time.timeScale);

        if (player.afterimageTrail != null)
            player.afterimageTrail.ToggleTrail();

        // Sustituimos rb.DOPath por mover el CharacterController desde la posición actual
        Vector3 startPos = player.transform.position;
        DOTween.To(
            () => 0f,
            t =>
            {
                // Interpolación cuadrática Bezier manual
                Vector3 targetPos = Mathf.Pow(1 - t, 2) * startPos
                                    + 2 * (1 - t) * t * pDodgeCurvePoint
                                    + Mathf.Pow(t, 2) * pDodgePosition;

                Vector3 delta = targetPos - player.transform.position;
                player.ch.Move(delta);
            },
            1f,
            player.perfectDodgeDuration
        ).OnComplete(() =>
        {
            Time.timeScale = 1f;
            CameraManager.instance.UntoggleZoom();
            player.afterimageTrail.UnToggleTrail();
        });
    }
}
