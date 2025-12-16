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
        
        _isPerfectDodge = PerfectDodgeManager.IsPerfectDodge();
        
        if (_isPerfectDodge)
        {
            // Execute perfect dodge towards the last detected enemy
            PerfectDodge(PerfectDodgeManager.GetPerfectDodgeEnemy());
        }
        else
        {
            // Calculate normal dash direction based on input
            Vector3 playerDirection = GetDashDirection();

            _dashSpeed = player.dashDistance / player.dashDuration;
            _forToApply = playerDirection * _dashSpeed;
            
            rb.AddForce(_forToApply, ForceMode.VelocityChange);

            // Store initial slope state for transition handling
            _enteredSlope = player.OnSlope();
        }
         PerfectDodgeManager.WipePerfectDodgeFlags();
    }
    
    public override void Update()
    {
        base.Update();

        // Handle slope transition mid-dash to maintain smooth movement
        if(_enteredSlope != player.OnSlope() && !_switchSlope)
        {
            if (_enteredSlope)
            {
                // Transitioning from slope to flat ground
                // Project velocity out of slope to prevent sudden height changes
                float remainingDashSpeed = rb.linearVelocity.magnitude;
                Vector3 newDashDirection = player.ProjectVectorOutOfSlope(rb.linearVelocity).normalized;
                rb.linearVelocity = newDashDirection * remainingDashSpeed;
            }
            else 
            {
                // Transitioning from flat ground to slope
                // Project velocity onto slope to follow terrain
                float remainingDashSpeed = rb.linearVelocity.magnitude;
                Vector3 newDashDirection = player.ProjectVectorOnSlope(rb.linearVelocity).normalized;
                rb.linearVelocity = newDashDirection * remainingDashSpeed;
            }
        }
        
        // Check if dash duration has completed
        if (stateTimer < 0f)
        {
            if (player.groundDetected)
            {
                // Return to idle if grounded
                stateMachine.ChangeState(player.idleState);
            }
            else
            {
                // Start falling if airborne /* HAY QUE BORRAR AIR STATE */
                stateMachine.ChangeState(player.fallState);
            }
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
        
        // Project onto slope if on sloped terrain
        if(player.OnSlope()) 
            direction = player.ProjectVectorOnSlope(direction);
            
        return direction;
    }

    /// <summary>
    /// Executes a perfect dodge maneuver when dashing near an enemy.
    /// Creates a curved path around the enemy with slow-motion and visual effects.
    /// </summary>
    /// <param name="enemy">The enemy GameObject to dodge around</param>
    void PerfectDodge(GameObject enemy)
    {
        Debug.LogWarning("Perfect");

        // Calculate direction vector from player to enemy (horizontal only)
        Vector3 direction =  new Vector3(
            enemy.transform.position.x - player.transform.position.x, 
            0f, 
            enemy.transform.position.z - player.transform.position.z
        );
        
        // Calculate final position: behind enemy at set distance
        Vector3 pDodgePosition = new Vector3(
            enemy.transform.position.x, 
            player.transform.position.y, 
            enemy.transform.position.z
        ) + direction.normalized * player.perfectDodgeEnemyDistance;
        
        // Calculate curve control point: perpendicular to enemy direction for arc movement
        Vector3 pDodgeCurvePoint = player.transform.position 
            + direction / 2 
            + Vector3.Cross(direction, Vector3.up).normalized * direction.magnitude / 1.25f;

        // Create curved path using Bezier curve (start, control, end)
        Vector3[] curvePoints = new Vector3[] { player.transform.position, pDodgeCurvePoint, pDodgePosition };
        
        // Apply cinematic effects
        CameraManager.instance.ToggleZoom();
        Time.timeScale = 0.25f; // Slow-motion effect
        
        // Enable afterimage trail effect if available
        if(player.afterimageTrail != null)
        {
            player.afterimageTrail.toggleTrail = true;
        }
        
        // Execute curved movement with DOTween, restore normal time on completion
        rb.DOPath(curvePoints, player.dashDuration).OnComplete(() => 
        { 
            Time.timeScale = 1f;
            CameraManager.instance.UntoggleZoom(); 
        });
    }
}
