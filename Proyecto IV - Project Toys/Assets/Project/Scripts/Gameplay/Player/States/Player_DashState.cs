using UnityEngine;

public class Player_DashState : Player_GroundedState
{
    Vector3 forToApply;
    public Player_DashState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        stateTimer = player.dashDuration;

        Vector3 playerDirection = GetDashDirection();
        Debug.Log("Dash direction: " + playerDirection);
        forToApply = playerDirection * player.dashSpeed;
    }


    public override void Update()
    {
        base.Update();
        rb.AddForce(forToApply, ForceMode.VelocityChange);

        if (stateTimer < 0f)
        {
            Debug.Log("Dash applied: " + forToApply);

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
        return direction;
    }
}
