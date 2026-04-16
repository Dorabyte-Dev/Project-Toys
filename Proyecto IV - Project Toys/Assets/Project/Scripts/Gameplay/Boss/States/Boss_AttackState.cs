using UnityEngine;

public class Boss_AttackState : BossState
{
    private int randomAttack;
    
    private int numberOfAttacks = 3; // Cambiar por el numero de ataques cuerpo a cuerpo que tendrá el boss
    private int currentAttackNumber;
    public Boss_AttackState(Boss boss, StateMachine stateMachine, string animBoolName) : base(boss, stateMachine, animBoolName)
    {
    }
    // Ataque 1: Melee. (Carga => Barrido)
    // Ataque 2: Lápices.
    // Ataque 3: Slam.

    public override void Enter()
    {
        base.Enter();
        randomAttack = Random.Range(1, 4); // Seria de 1 al numero de ataques cuerpo a cuerpo que tendrá el boss + 1
        boss.anim.SetInteger("AttackType", randomAttack);
    }

    public override void Update()
    {
        base.Update();
        if(currentAttackNumber >= numberOfAttacks)
        {
            stateMachine.ChangeState(boss.weakState);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
