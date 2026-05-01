using UnityEngine;

public class Player_AnimationTriggers : Entity_AnimationTriggers
{
    private Player player;

    public override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
    }

    public override void AttackTrigger()
    {
        base.AttackTrigger();
        if ((player.currentAttack.extraEffects & AttackExtraEffect.SlamEffect) != 0)
        {
            player._vfx.SlamEffect();
        }

        if ((player.currentAttack.extraEffects & AttackExtraEffect.CameraShake) != 0)
        {
            player._vfx.AttackCameraShake();
        }
        if((player.currentAttack.extraEffects & AttackExtraEffect.ControllerShake) != 0)
        {
            player._vfx.ControllerShake();
        }
        
    }

    public override void HeavyTrigger()
    {
        base.HeavyTrigger();
        if ((player.currentAttack.extraEffects & AttackExtraEffect.SlamEffect) != 0)
        {
            player._vfx.SlamEffect();
        }

        if ((player.currentAttack.extraEffects & AttackExtraEffect.CameraShake) != 0)
        {
            player._vfx.AttackCameraShake();
        }
        if((player.currentAttack.extraEffects & AttackExtraEffect.ControllerShake) != 0)
        {
            player._vfx.ControllerShake();
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
