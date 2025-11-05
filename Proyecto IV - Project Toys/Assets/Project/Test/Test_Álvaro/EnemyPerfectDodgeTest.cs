using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.VersionControl.Asset;

public class EnemyPerfectDodgeTest : MonoBehaviour
{
    public bool jugadorDetectado;
    public bool isAttacking;
    public float range;
    public float attackRange;
    public float waitTime;
    public int damage;
    public float speed;
    public float acceleration;
    public float attackAcceleration;
    public float attackSpeed;
    public GameObject damageCollider;
    private NavMeshAgent agent;
    private Animator anim;
    [HideInInspector] public Transform playerTransform; 
    public enum enemyStates { Walk, Pursuit, Attack};
    private enemyStates state;
    private Vector3 lastPlayerPosition;
    private Vector3 currentPosition;
    private Vector3 attackPoint;
    private float currentTime;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        agent.speed = speed;
        agent.acceleration = acceleration;
        damageCollider.SetActive(false);
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
                currentTime = 0;
                agent.destination = playerTransform.position;
                lastPlayerPosition = playerTransform.position;
                currentPosition = transform.position;
                break;
            case enemyStates.Attack:
                if (!isAttacking)   //Se ejecuta solo al entrar al estado de ataque
                {
                    Debug.Log("ATACAAAA");
                    isAttacking = true;
                    damageCollider.SetActive(true);
                    agent.destination = currentPosition + (lastPlayerPosition - currentPosition) * .25f;
                    attackPoint = playerTransform.position;
                    lastPlayerPosition = attackPoint;
                    currentPosition = transform.position;
                    transform.LookAt(new Vector3(attackPoint.x, transform.position.y, attackPoint.z), Vector3.up);
                    anim.Play("WaitAttack");
                }

                currentTime += Time.deltaTime;
                if (currentTime >= waitTime)    //Se ejecuta cuando haya terminado de cargar el ataque.
                {
                    Vector3 attackDirection = (attackPoint - currentPosition).normalized;
                    attackPoint = currentPosition + attackDirection * attackRange;
                    anim.Play("Attack");
                    agent.speed = attackSpeed;
                    agent.acceleration = attackAcceleration;
                    agent.destination = attackPoint;
                    if (HasReachDestination())
                    {
                        FinishAttack();
                    }
                }

                
                
                break;
        }
    }
    public void SetState(enemyStates setState)
    {
        state = setState;
    }

    

    public bool HasReachDestination()
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }
        return false;
    }

    #region AttackFunctions
    public void FinishAttack()
    {
        isAttacking = false;
        damageCollider.SetActive(false);
        currentTime = 0f;
        currentPosition = transform.position;
        agent.speed = speed;
        agent.acceleration = acceleration;
        SetState(enemyStates.Pursuit);
    }
    public void DealDamage()
    {
        Debug.Log("Pum te pego: " + damage + " de daño");
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

    #region PerfectDodge
    public void SetPerfectDodgeFlag()
    {
        FindAnyObjectByType<GameManager>().perfectDodgeWindowActive = true;
    }

    public void EndPerfectDodgeFlag()
    {
        FindAnyObjectByType<GameManager>().perfectDodgeWindowActive = false;
    }
    #endregion
}
