using UnityEngine;

public class Player_AnimationTriggers : Entity_AnimationTriggers
{
    private Player player;

    public override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
    }

    public override void HeavyTrigger()
    {
        base.HeavyTrigger();
        if ((player.currentAttack.extraEffects & AttackExtraEffect.SlamEffect) != 0)
        {
            //player._vfx.SlamEffect();
        }

        if ((player.currentAttack.extraEffects & AttackExtraEffect.CameraShake) != 0)
        {
            //player._vfx.AttackCameraShake();
        }
    }

    public void TriggerAttackVFX()
    {
        player._vfx.Slash();
    }

    public void EndExecution()
    {
        player.ChangePlayerState(player.idleState);
    }
}
