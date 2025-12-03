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
            //enemy.UpdateStateBasedOnNearness();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            enemy.playerTransform = other.transform;
            //enemy.UpdateStateBasedOnNearness();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            enemy.nearness--;
            //enemy.UpdateStateBasedOnNearness();
        }
    }
}
