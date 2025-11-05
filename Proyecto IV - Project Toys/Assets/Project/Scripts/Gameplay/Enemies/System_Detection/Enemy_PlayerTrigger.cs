using UnityEngine;

public class Enemy_PlayerTrigger : MonoBehaviour
{
    private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }
    private void OnTriggerEnter(Collider other)
    {
        enemy.DealDamage();
    }
}
