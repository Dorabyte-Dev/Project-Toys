using System.Collections;
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
    private Vector3 lastPlayerPosition;
    private Vector3 currentPosition;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        
        switch (state)
        {
            case enemyStates.Walk:
                Debug.Log("BUSCANDO");
                GoToRandomPoint();
                break;
            case enemyStates.Pursuit:
                Debug.Log("TE ENCONTRÉ");
                agent.destination = playerTransform.position;
                lastPlayerPosition = playerTransform.position;
                currentPosition = transform.position;
                break;
            case enemyStates.Attack:
                transform.LookAt(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z), Vector3.up);    //De esta manera el enemigo estará mirando todo el rato al enemigo sin necesidad de cambiar su rotacion en Y
                agent.destination = currentPosition + (lastPlayerPosition - currentPosition) * .25f;
                Debug.Log("ATACAAAA");
                break;
        }
    }
    public void SetState(enemyStates setState)
    {
        state = setState;
    }

    #region AttackFunctions
    public IEnumerator AttackSequence()
    {
        Vector3 attackPosition = Vector3.zero;
        yield return new WaitForSecondsRealtime(1.5f);
        attackPosition = playerTransform.position;
        yield return new WaitForSecondsRealtime(0.5f);
        //Attack
    }
    #endregion

    #region WalkFunctions
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
    #endregion
}
