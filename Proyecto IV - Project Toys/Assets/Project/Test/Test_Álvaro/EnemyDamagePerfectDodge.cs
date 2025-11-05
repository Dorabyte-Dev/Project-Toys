using UnityEngine;

public class EnemyDamagePerfectDodge : MonoBehaviour
{
    private EnemyPerfectDodgeTest enemy;
    [SerializeField]private int perfectDodgeDetector;
    private void Awake()
    {
        enemy = GetComponentInParent<EnemyPerfectDodgeTest>();
    }

    private void OnDisable()
    {
        perfectDodgeDetector = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("pDodge"))
        {
            enemy.SetPerfectDodgeFlag();
        }
        else
        {
            enemy.DealDamage();
            enemy.EndPerfectDodgeFlag();
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
        if (other.gameObject.CompareTag("pDodge"))
        {
            enemy.EndPerfectDodgeFlag();
        }
    }
}
