using UnityEngine;

public class Boss_BaseState : BossState
{
    private int randomAttackIndex;
    private int currentAttacksCount;
    
    public Boss_BaseState(Boss boss, StateMachine stateMachine, string animBoolName) : base(boss, stateMachine, animBoolName)
    {
    }
    // Ataque 1: Carga.
    // Ataque 2: Lápices.
    // Ataque 3: Slam.

    public override void Enter()
    {
        base.Enter();
        //randomAttackIndex = GetRandomAttackIndex();
        randomAttackIndex = 2;
        stateTimer = boss.timeInIdle;
    }

    public override void Update()
    {
        base.Update();
        if(stateTimer > 0) return;
        switch (randomAttackIndex)
        {
            case 1:
                boss.ChangeBossState(boss.chargeAttackState);
                Debug.Log("Boss_BaseState: Changing to ChargeAttackState");
                break;
            case 2:
                boss.ChangeBossState(boss.pencilAttackState);
                Debug.Log("Boss_BaseState: Changing to PencilAttackState");
                break;
            case 3:
                boss.ChangeBossState(boss.slamAttackState);
                Debug.Log("Boss_BaseState: Changing to SlamAttackState");
                break;
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
    
    private int GetRandomAttackIndex()
    {
        int index;
        if (currentAttacksCount >= boss.maxAttacksBeforeChargeAttack)
        {
            index = 1;
            currentAttacksCount = 0;
            return index;
        }
        index = Random.Range(1, 4); // Seria de 1 al numero de ataques cuerpo a cuerpo que tendrá el boss + 1
        currentAttacksCount++;
        return index;
    }
}
