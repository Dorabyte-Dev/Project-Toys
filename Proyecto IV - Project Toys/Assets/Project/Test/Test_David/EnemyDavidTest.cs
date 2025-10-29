using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.VersionControl.Asset;

public class EnemyDavidTest : MonoBehaviour
{
    public bool jugadorDetectado;
    public float range;
    private NavMeshAgent agent;
    [HideInInspector] public Transform playerTransform; 
    public enum enemyStates { Walk, Pursuit, Attack};
    private enemyStates state;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (jugadorDetectado)
        {
            Debug.Log("TE ENCONTRÉ");
            agent.isStopped = false;
            agent.destination = playerTransform.position;
        }
        else
        {
            agent.isStopped = true;
        }*/
        
        switch (state)
        {
            case enemyStates.Walk:
                Debug.Log("BUSCANDO");
                GoToRandomPoint();
                break;
            case enemyStates.Pursuit:
                Debug.Log("TE ENCONTRÉ");
                agent.destination = playerTransform.position;
                break;
            case enemyStates.Attack:
                Debug.Log("ATACAAAA");
                break;
        }
    }
    public void SetState(enemyStates setState)
    {
        state = setState;
    }

    public void GoToRandomPoint()
    {
        if (agent.pathPending || !agent.isOnNavMesh || agent.remainingDistance > 0.1f)
            return;

        agent.destination = RandomNavSphere(transform.position, range, -1);
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist, int layermask)
    {
        Vector3 randDirection = Random.insideUnitSphere * dist;

        randDirection += origin;

        NavMeshHit navHit;

        NavMesh.SamplePosition(randDirection, out navHit, dist, layermask);

        return navHit.position;
    }
}
