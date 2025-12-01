using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveManager : MonoBehaviour
{
    public static EnemyWaveManager Instance { get; private set; }

    [Header("Group AI Settings")]
    public int maxSimultaneousAttackers = 1;   // Solo 1 enemigo atacando a la vez
    public float minAttackInterval = 1.5f;     // Tiempo mínimo entre ataques de grupo
    public float maxAttackInterval = 3f;       // Tiempo máximo entre ataques de grupo

    [SerializeField] private List<Enemy> activeEnemies = new List<Enemy>();
    [SerializeField] private List<Enemy> enemiesWaitingToAttack = new List<Enemy>(); // Enemigos que quieren atacar
    [SerializeField] private float groupAttackTimer;

    private void Awake()
    {
        Debug.Log("[EnemyWaveManager] Awake");
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        // Inicializar el timer para que empiece a contar desde el principio
        groupAttackTimer = Random.Range(minAttackInterval, maxAttackInterval);
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
            Debug.Log($"[EnemyWaveManager] Enemigo registrado: {enemy.name}. Total activos: {activeEnemies.Count}");
        }
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        activeEnemies.Remove(enemy);
        enemiesWaitingToAttack.Remove(enemy);
        Debug.Log($"[EnemyWaveManager] Enemigo desregistrado: {enemy.name}. Total activos: {activeEnemies.Count}");
    }

    // Llamado por el enemigo cuando QUIERE atacar (está en rango)
    public void RequestAttackPermission(Enemy enemy)
    {
        if (!enemiesWaitingToAttack.Contains(enemy) && !enemy.isAttacking)
        {
            enemiesWaitingToAttack.Add(enemy);
            Debug.Log($"[EnemyWaveManager] {enemy.name} solicita permiso de ataque. En cola: {enemiesWaitingToAttack.Count}");
        }
    }

    // Llamado por el enemigo cuando ya NO quiere atacar (salió de rango o cambió de estado)
    public void CancelAttackRequest(Enemy enemy)
    {
        if (enemiesWaitingToAttack.Contains(enemy))
        {
            enemiesWaitingToAttack.Remove(enemy);
            Debug.Log($"[EnemyWaveManager] {enemy.name} cancela solicitud de ataque.");
        }
    }

    // Llamado cuando el enemigo TERMINA su ataque
    public void NotifyEnemyFinishedAttack(Enemy enemy)
    {
        Debug.Log($"[EnemyWaveManager] {enemy.name} terminó su ataque.");
        enemy.canAttackByManager = false;
        
        // Reiniciar el timer para dar oportunidad al siguiente
        groupAttackTimer = Random.Range(minAttackInterval, maxAttackInterval);
    }

    private void HandleGroupAttackLogic()
    {
        if (activeEnemies.Count == 0 || enemiesWaitingToAttack.Count == 0)
            return;

        groupAttackTimer -= Time.deltaTime;
        if (groupAttackTimer > 0)
            return;

        // Contar cuántos están atacando actualmente
        int currentlyAttacking = 0;
        foreach (var e in activeEnemies)
        {
            if (e != null && e.isAttacking)
                currentlyAttacking++;
        }

        if (currentlyAttacking >= maxSimultaneousAttackers)
            return;

        // Dar permiso al primero de la lista de espera
        if (enemiesWaitingToAttack.Count > 0)
        {
            Enemy next = enemiesWaitingToAttack[0];
            enemiesWaitingToAttack.RemoveAt(0);

            if (next != null && !next.isAttacking)
            {
                next.AllowAttackFromManager();
                Debug.Log($"[EnemyWaveManager] Permiso concedido a {next.name}. Atacando ahora: {currentlyAttacking + 1}/{maxSimultaneousAttackers}");
                
                // Reiniciar timer
                groupAttackTimer = Random.Range(minAttackInterval, maxAttackInterval);
            }
        }
    }

    public bool IsWaveCleared()
    {
        return activeEnemies.Count == 0;
    }
}