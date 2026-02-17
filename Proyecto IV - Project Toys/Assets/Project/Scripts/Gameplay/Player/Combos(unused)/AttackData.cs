using UnityEngine;

[CreateAssetMenu(fileName = "ComboAttack", menuName = "Scriptable Objects/ComboAttack")]
public class AttackData : ScriptableObject
{
    public string attackName;
    public float motionValue;
    public float attackVelocity;
}
