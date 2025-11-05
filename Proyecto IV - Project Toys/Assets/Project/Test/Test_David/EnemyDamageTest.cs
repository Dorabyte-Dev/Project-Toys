using UnityEngine;

public class EnemyDamageTest : MonoBehaviour
{
    private EnemyDavidTest enemy;
    [SerializeField]private int perfectDodgeDetector;
    private void Awake()
    {
        enemy = GetComponentInParent<EnemyDavidTest>();
    }

    private void OnDisable()
    {
        perfectDodgeDetector = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            perfectDodgeDetector++;
        }
        if(perfectDodgeDetector >= 2)
        {
            enemy.DealDamage();
        }
        
    }

    private void OnTriggerStay(Collider other)
    {
        if(perfectDodgeDetector == 1)
        {
            Debug.Log("HAZ ESQUIVA PERFECTA");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            perfectDodgeDetector--;
        }
    }
}
