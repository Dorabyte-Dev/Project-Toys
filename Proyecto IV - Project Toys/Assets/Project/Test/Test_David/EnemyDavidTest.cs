using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.VersionControl.Asset;

public class EnemyDavidTest : MonoBehaviour
{
    public bool jugadorDetectado;
    public bool isAttacking;
    public float range;
    private NavMeshAgent agent;
    [HideInInspector] public Transform playerTransform; 
    public enum enemyStates { Walk, Pursuit, Attack};
    private enemyStates state;
    private Vector3 lastPlayerPosition;
    private Vector3 currentPosition;
    private Animator anim;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
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
                /*
                 * Nota para los proximos cambios:
                 * El objetivo es que el ataque sea una sequencia de una que ignore todo lo demás, es decir, cuando entra al estado de ataque
                 * no sale hasta que termina el ataque. Para conseguir este Ienumerator no es del todo una buena idea ya que se ejecuta de manera paralela 
                 * al codigo del update; lo ideal es crear una sequencia de animaciones las cuales realizan el ataque, o; sin animaciones;
                 * activar un trigger de ataque cuando pase el tiempo de espera del ataque.
                 */
                transform.LookAt(new Vector3(playerTransform.position.x, transform.position.y, playerTransform.position.z), Vector3.up);    //De esta manera el enemigo estará mirando todo el rato al enemigo sin necesidad de cambiar su rotacion en Y
                agent.destination = currentPosition + (lastPlayerPosition - currentPosition) * .25f;
                if (!agent.pathPending)
                {
                    if (agent.remainingDistance <= agent.stoppingDistance)
                    {
                        if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                        {

                            if (!isAttacking)
                            {
                                StartCoroutine(AttackSequence());
                            }
                            Debug.Log("ATACAAAA");
                            
                        }
                    }
                }
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
        Debug.Log("Attacking");
        isAttacking = true;
        agent.isStopped = true;
        Vector3 attackPosition = Vector3.zero;
        yield return new WaitForSecondsRealtime(1.5f);
        attackPosition = playerTransform.position;
        yield return new WaitForSecondsRealtime(0.5f);
        //Attack
        agent.isStopped = false;
        /*agent.speed *= 2;
        agent.acceleration *= 2;*/
        anim.Play("Foxy");
        agent.destination = attackPosition;
        isAttacking = false;
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
