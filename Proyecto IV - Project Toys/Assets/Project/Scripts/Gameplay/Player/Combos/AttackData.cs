using UnityEngine;

[CreateAssetMenu(fileName = "ComboAttack", menuName = "Scriptable Objects/ComboAttack")]
public class AttackData : ScriptableObject
{
    public float motionValue;
    public float attackVelocity;
    public AttackData nextLightAttack;
    public AttackData nextHeavyAttack;
}
