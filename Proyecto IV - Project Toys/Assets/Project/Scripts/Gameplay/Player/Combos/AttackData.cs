using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ComboAttack", menuName = "Scriptable Objects/ComboAttack")]
public class AttackData : ScriptableObject
{
    [Tooltip("Multi-selector for extra effects to apply during the attack (CameraShake, SlamEffect, etc...)")] 
    public AttackExtraEffect extraEffects;
    [Tooltip("Collider to use during this attack")]
    public AttackColliderType colliderUsed;
    [Tooltip("Value used to determine strength of the attack. It's multiplied by the player's base attack damage to calculate the final damage of the attack.")]
    public float motionValue;
    [Tooltip("Value used to determine player's movement during the attack. It's applied every frame during the attackVelocity duration.")]
    public float attackMoveDistance;
    [Tooltip("Value used to determine when attackVelocity starts being applied during the attack. It's a percentage of the attack animation duration.")]
    public float attackMoveDurationStart;
    [Tooltip("Value used to determine when attackVelocity stops being applied during the attack. It's a percentage of the attack animation duration.")]
    public float attackMoveDurationEnd;
    [Tooltip("Reference to the next attack in the combo if the player performs a light attack during the current attack.")]
    public AttackData nextLightAttack;
    [Tooltip("Reference to the next attack in the combo if the player performs a heavy attack during the current attack.")]
    public AttackData nextHeavyAttack;
}