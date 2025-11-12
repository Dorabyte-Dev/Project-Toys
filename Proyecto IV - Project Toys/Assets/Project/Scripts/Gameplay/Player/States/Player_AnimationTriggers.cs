using UnityEngine;

public class Player_AnimationTriggers : Entity_AnimationTriggers
{
    private Player player;

    public override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
    }

    public override void CurrentStateTrigger()
    {
        base.AttackTrigger();
        player.CallAnimationTrigger();
    }
    public override void AttackTrigger()
    {
        base.AttackTrigger();
    }
}
