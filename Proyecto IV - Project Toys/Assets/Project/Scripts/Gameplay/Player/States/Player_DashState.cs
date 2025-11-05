using UnityEngine;

public class Player_DashState : Player_GroundedState
{
    Vector3 forToApply;
    float dashSpeed;
    bool enteredSlope;
    bool switchSlope;
    bool isPerfectDodge;
    public Player_DashState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = player.dashDuration;

        Vector3 playerDirection = GetDashDirection();
        Debug.Log("Dash direction: " + playerDirection);

        dashSpeed = player.dashDistance/player.dashDuration;
        forToApply = playerDirection * dashSpeed;
        rb.AddForce(forToApply, ForceMode.VelocityChange);
        enteredSlope = player.OnSlope();
        isPerfectDodge = Object.FindAnyObjectByType<GameManager>().perfectDodgeWindowActive;
        if (isPerfectDodge) Debug.LogWarning("Perfect!");

        Debug.Log("Dash applied: " + forToApply);
    }


    public override void Update()
    {
        base.Update();

        
        if(enteredSlope != player.OnSlope() && !switchSlope)
        {
            if (enteredSlope)
            {
                float remainingDashSpeed = rb.linearVelocity.magnitude;
                Vector3 newDashDirection = player.ProjectVectorOutOfSlope(rb.linearVelocity).normalized;
                rb.linearVelocity = newDashDirection * remainingDashSpeed;
            }
            else 
            {
                Debug.Log("My last Linear velocity is: " + rb.linearVelocity + ", with magnitude of " + rb.linearVelocity.magnitude);
                float remainingDashSpeed = rb.linearVelocity.magnitude;
                Vector3 newDashDirection = player.ProjectVectorOnSlope(rb.linearVelocity).normalized;
                rb.linearVelocity = newDashDirection * remainingDashSpeed;
                Debug.Log("My new Linear velocity is: " + rb.linearVelocity + ", with magnitude of " + rb.linearVelocity.magnitude);
            }
        }
        if (stateTimer < 0f)
        {
            if (player.groundDetected)
            {
                stateMachine.ChangeState(player.idleState);
            }
            else
            {
                stateMachine.ChangeState(player.fallState);
            }
        }
    }
    public override void Exit()
    {
        base.Exit();
        player.SetVelocity(0f, 0f);
    }

    private Vector3 GetDashDirection()
    {
        Vector2 inputVector = player.moveInput;
        if (inputVector.magnitude < 0.1f)
            return player.transform.forward;

        Vector3 direction = new Vector3(inputVector.x, 0f, inputVector.y).normalized;
        if(player.OnSlope()) 
            direction = player.ProjectVectorOnSlope(direction);
        return direction;
    }
}
