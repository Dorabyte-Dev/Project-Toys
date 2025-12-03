using UnityEngine;

public class Enemy_PlayerDetection : MonoBehaviour
{
    private Enemy enemy;

    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            enemy.nearness++;
            enemy.playerTransform = other.transform;
            Debug.Log($"[Detection] {enemy.name} nearness = {enemy.nearness} (ENTER)");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            enemy.playerTransform = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            enemy.nearness--;
            Debug.Log($"[Detection] {enemy.name} nearness = {enemy.nearness} (EXIT)");

            if (enemy.nearness <= 0)
            {
                enemy.playerTransform = null;
            }
        }
    }
}