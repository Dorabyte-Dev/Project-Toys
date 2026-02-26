using System;
using UnityEngine;

public class Player_Combat : Entity_Combat
{
    [SerializeField] private Player player;

    public override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
    }
    // Llamado por Player_ComboSystem antes de que se ejecute el ataque

    public override void PerformAttack()
    {
        if (player.currentAttack == null)
        {
            Debug.LogWarning("[Player_Combat] No AttackData assigned, using base damage.");
            base.PerformAttack();
            return;
        }

        finalDamage = baseDamage * player.currentAttack.motionValue;
        Debug.Log("Final Damage: " + finalDamage);

        foreach (var target in GetDetectedColliders())
        {
            Entity_Health targetHealth = target.GetComponent<Entity_Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(finalDamage, this.transform);
                if (targetHealth.invincibleMode) return;
                targetHit?.Invoke();
            }
            else if (target.CompareTag("dObject"))
            {
                Break_Object breakObject = target.GetComponent<Break_Object>();
                if (breakObject != null)
                    breakObject.ActivateDestruction();
            }
            else
            {
                Debug.LogWarning("Entity_Health not found on " + target.name);
            }
        }
    }
}