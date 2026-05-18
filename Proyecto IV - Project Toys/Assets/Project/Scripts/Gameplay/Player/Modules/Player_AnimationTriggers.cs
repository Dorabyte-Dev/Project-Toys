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
    
    #region Sound Functions

    public void PlayLightAttack1SFX()
    {
        SoundManager.instance.Play("LightAttack1");
    }
    
    public void PlayLightAttack2SFX()
    {
        SoundManager.instance.Play("LightAttack2");
    }
    
    public void PlayLightAttack3SFX()
    {
        SoundManager.instance.Play("LightAttack3");
    }
    
    public void PlayHeavyAttack1SFX()
    {
        SoundManager.instance.Play("HeavyAttack1");
    }
    
    public void PlayHeavyAttack2SFX()
    {
        SoundManager.instance.Play("HeavyAttack2");
    }
    
    public void PlayHeavyAttack3SFX()
    {
        SoundManager.instance.Play("HeavyAttack3");
    }

    public void PlayStepSFX()
    {
        switch (player.GetGroundType())
        {
            case "Grass":
                SoundManager.instance.PlayRandomInRange(new string[]{"GrassWalk1", "GrassWalk2"});
                break;
            case "Wood":
                SoundManager.instance.Play("WoodWalk");
                break;
            case "Stone":
                SoundManager.instance.PlayRandomInRange(new string[]{"StoneWalk1", "StoneWalk2", "StoneWalk3", "StoneWalk4"});
                break;
            default:
                SoundManager.instance.PlayRandomInRange(new string[]{"StoneWalk1", "StoneWalk2", "StoneWalk3", "StoneWalk4"});
                break;
        }
    }
    #endregion
}
