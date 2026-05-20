using System;
using UnityEngine;

public class Player_Combat : Entity_Combat
{
    [SerializeField] private Player player;
    private float _finalDamage;

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

        _finalDamage = baseDamage * player.currentAttack.motionValue;
        Debug.Log("Final Damage: " + _finalDamage);
        Collider[] detectedColliders = GetDetectedColliders();
        foreach (var target in detectedColliders)
        {
            Entity_Health targetHealth = target.GetComponent<Entity_Health>();
            if (targetHealth != null)
            {
                if(targetHealth.isDead) continue;
                    targetHealth.TakeDamage(_finalDamage, this.transform);
                if(targetHealth.invincibleMode) continue;
                targetHit?.Invoke();
                SoundManager.instance.PlayRandomInRange(new string[]{ "PlayerVoiceHit1", "PlayerVoiceHit2", "PlayerVoiceHit3", "PlayerVoiceHit4"});
                //if (targetHealth.invincibleMode) return;
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

    protected override Collider[] GetDetectedColliders()
    {
        BoxCollider colliderUsed = player.GetColliderUsed(player.currentAttack.colliderUsed);
        Debug.Log("Box used: " + colliderUsed.name);
        Vector3 centerPoint = colliderUsed.transform.TransformPoint(colliderUsed.center);
        
        Vector3 halfExtents = Vector3.Scale(colliderUsed.size, transform.lossyScale) * 0.5f;

        Quaternion rotation = transform.rotation;

        return Physics.OverlapBox(centerPoint, halfExtents, rotation, whatIsTarget);
    }
}