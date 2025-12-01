using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveManager : MonoBehaviour
{
    public static EnemyWaveManager Instance { get; private set; }

    [Header("Group AI Settings")]
    public int maxSimultaneousAttackers = 1;   // Solo 1 enemigo atacando a la vez
    public float minAttackInterval = 1.5f;     // Mínimo tiempo entre ataques de grupo
    public float maxAttackInterval = 3f;       // Máximo tiempo entre ataques de grupo

    [SerializeField] private List<Enemy> activeEnemies = new List<Enemy>();
    [SerializeField] private Queue<Enemy> attackQueue = new Queue<Enemy>();
    [SerializeField] private float groupAttackTimer;

    private void Awake()
    {
        Debug.Log("Wave Manager Awake");
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        HandleGroupAttackLogic();
    }

    public void RegisterEnemy(Enemy enemy)
    {
        if (!activeEnemies.Contains(enemy))
        {
            activeEnemies.Add(enemy);
            attackQueue.Enqueue(enemy);
        }
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        activeEnemies.Remove(enemy);

        // Reconstruimos la cola sin el enemigo muerto/despawned
        var newQueue = new Queue<Enemy>();
        foreach (var e in attackQueue)
        {
            if (e != enemy)
                newQueue.Enqueue(e);
        }
        attackQueue.Clear();
        foreach (var e in newQueue)
            attackQueue.Enqueue(e);
    }

    public void NotifyEnemyFinishedAttack(Enemy enemy)
    {
        // Cuando un enemigo acaba de atacar, lo mandamos al final de la cola
        Debug.Log("Notify Finished Attack");
        if (activeEnemies.Contains(enemy))
        {
            attackQueue.Enqueue(enemy);
        }
    }

    private void HandleGroupAttackLogic()
    {
        if (activeEnemies.Count == 0)
            return;

        groupAttackTimer -= Time.deltaTime;
        if (groupAttackTimer > 0)
            return;

        // ¿Cuántos están atacando ya?
        int currentlyAttacking = 0;
        foreach (var e in activeEnemies)
        {
            if (e.isAttacking)
                currentlyAttacking++;
        }

        if (currentlyAttacking >= maxSimultaneousAttackers)
            return;

        // Dar turno al siguiente de la cola
        if (attackQueue.Count > 0)
        {
            var next = attackQueue.Dequeue();
            if (next != null && !next.isAttacking)
            {
                next.AllowAttackFromManager();
                // Reiniciamos el temporizador de ritmo de ataques
                groupAttackTimer = Random.Range(minAttackInterval, maxAttackInterval);
            }
        }
    }
    
    public bool IsWaveCleared()
    {
        return activeEnemies.Count == 0;
    }
}

