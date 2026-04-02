using Unity.VisualScripting;
using UnityEngine;

public class Enemy_DeadState : EnemyState
{

    public Enemy_DeadState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        /*Debug.Log("Entro en deadState");
        //anim.enabled = false;
        enemy.agent.enabled = false;
        PerfectDodgeManager.EndPerfectDodgeFlag(enemy.gameObject);
        if (enemy.spawner != null)
            enemy.spawner.EnemyDead(enemy.gameObject);
        //stateMachine.SwitchOffStateMachine();
        //enemy.EnemyDeathTest();*/
        enemy.Dead_Enter();
        enemy.SetEnemyDead();
        enemy._vfx.ResetPushFeedback();
        
    }


    public override void Update()
    {
        base.Update();
        enemy.Dead_Update();
    }
    public override void Exit()
    {
        base.Exit();
        enemy.Dead_Exit();
    }

    
}
