using UnityEngine;

public class DetectarJugadorPerfectDodge : MonoBehaviour
{
    private EnemyPerfectDodgeTest enemy;
    private int nearness;
    void Start()
    {
        enemy = GetComponentInParent<EnemyPerfectDodgeTest>();
    }

    void Update()
    {
        switch (nearness)
        {
            case 0:
                if(enemy.isAttacking == false)
                {
                    enemy.SetState(EnemyPerfectDodgeTest.enemyStates.Walk);
                }
                break;
            case 1:
                if (enemy.isAttacking == false)
                {
                    enemy.SetState(EnemyPerfectDodgeTest.enemyStates.Pursuit);
                }
                break;
            case 2:
                enemy.SetState(EnemyPerfectDodgeTest.enemyStates.Attack);
                break;
            default:
                Debug.LogWarningFormat("Error with the detect player system");
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            nearness++;
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
            nearness--;
        }
    }
}
