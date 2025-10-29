using UnityEngine;

public class DetectarJugadorTest : MonoBehaviour
{
    private EnemyDavidTest enemy;
    private int nearness;
    void Start()
    {
        enemy = GetComponentInParent<EnemyDavidTest>();
    }

    void Update()
    {
        switch (nearness)
        {
            case 0:
                enemy.SetState(EnemyDavidTest.enemyStates.Walk);
                break;
            case 1:
                enemy.SetState(EnemyDavidTest.enemyStates.Pursuit);
                break;
            case 2:
                enemy.SetState(EnemyDavidTest.enemyStates.Attack);
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
