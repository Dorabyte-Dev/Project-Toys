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
        if(nearness >= 2)
        {
            Debug.Log("ATACAAAAA");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            enemy.jugadorDetectado = true;
            nearness++;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            enemy.jugadorDetectado = false;
            nearness--;
        }
    }
}
