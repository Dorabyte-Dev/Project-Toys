using UnityEngine;

public class Player_Health : Entity_Health
{
    private Player player;
    
    public override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
    }
    public virtual void Heal(float healAmount)
    {
        currentHp += healAmount;
        if (currentHp > maxHp)
            currentHp = maxHp;
        //Play heal VFX here:
        player._vfx.HealingEffect();
        SoundManager.instance.Play("Heal");
    }

    public override void TakeDamage(float takeDamage, Transform damageDealer)
    {
        if(invincibleMode) return;
        base.TakeDamage(takeDamage, damageDealer);
        vfx.DamageFeedback(damageDealer);
        //VFX Feedback here
        
    }
}