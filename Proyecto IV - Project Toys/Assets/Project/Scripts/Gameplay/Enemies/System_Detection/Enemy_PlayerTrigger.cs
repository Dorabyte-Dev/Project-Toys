using UnityEngine;

public class Enemy_PlayerTrigger : MonoBehaviour
{
    private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }
}
