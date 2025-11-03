using UnityEngine;

public class EnemyDamageTest : MonoBehaviour
{
    private EnemyDavidTest enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<EnemyDavidTest>();
    }

    private void OnTriggerEnter(Collider other)
    {
        enemy.DealDamage();
    }
}
