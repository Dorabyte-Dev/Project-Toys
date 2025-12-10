using Unity.VisualScripting;
using UnityEngine;

public class Enemy_DeadState : EnemyState
{

    public Enemy_DeadState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    /*Aqui deberia de estar en la funcion Enter o Update el devolver el enemigo a la pool, de momento voy a poner una función temporal en el
     * script de Enemy que destruye al enemigo para morir y que no se queden en medio. No lo puedo hacer aqui ya que tiene que deribar de MonoBehaviour.
     */
    public override void Enter()
    {
        base.Enter();
        Debug.Log("Entro en deadState");
        //anim.enabled = false;
        enemy.agent.enabled = false;
        if (enemy.spawner != null)
            enemy.spawner.EnemyDead(enemy.gameObject);
        stateMachine.SwitchOffStateMachine();
        //enemy.EnemyDeathTest();
    }


    public override void Update()
    {
        base.Update();
    }
    public override void Exit()
    {
        Debug.Log("Salgo de deadState");
        base.Exit();
    }

    
}
