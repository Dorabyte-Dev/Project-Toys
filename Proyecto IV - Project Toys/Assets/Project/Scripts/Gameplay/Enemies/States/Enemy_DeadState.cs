using UnityEngine;

public class Enemy_DeadState : EnemyState
{

    public Enemy_DeadState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        anim.enabled = false;

        Debug.Log("ENTRO EN ESTADO DE MUERTE ENEMY");
        enemy.agent.enabled = false;
        stateMachine.SwitchOffStateMachine();
        Debug.Log("IM DEAD.\n PD: Compra pan");
    }


    public override void Update()
    {
        base.Update();
    }
    public override void Exit()
    {
        base.Exit();
    }
}
