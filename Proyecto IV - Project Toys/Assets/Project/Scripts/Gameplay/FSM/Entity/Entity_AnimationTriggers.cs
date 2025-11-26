using UnityEngine;

public class Entity_AnimationTriggers : MonoBehaviour
{
    [SerializeField] private Entity entity;
    [SerializeField] private Entity_Combat entityCombat;

    public virtual void Awake()
    {
        entity = GetComponent<Entity>();
        entityCombat = GetComponent<Entity_Combat>();
    }
    public virtual void CurrentStateTrigger() 
    {
        entity.CurrentStateAnimationTrigger();
    }

    public virtual void AttackTrigger()
    {
        entityCombat.PerformAttack();
    }

    public virtual void HeavyTrigger()
    {
        entityCombat.PerformHeavyAttack();
    }
}
