using UnityEngine;

public class Boss_AnimationTriggers : Entity_AnimationTriggers
{
    private Boss boss;
    
    public override void Awake()
    {
        base.Awake();
        boss = GetComponent<Boss>();
    }

    public void TriggerSlamAttack()
    {
        boss.InstantiateSlamAttack();
    }
    
}
