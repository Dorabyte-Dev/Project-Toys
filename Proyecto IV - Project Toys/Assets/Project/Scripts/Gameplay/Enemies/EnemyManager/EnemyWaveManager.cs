using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveManager : MonoBehaviour
{
    public static EnemyWaveManager Instance { get; private set; }

    [Header("Attack Control")]
    public int maxSimultaneousAttackers = 1;

    private List<Enemy> activeEnemies = new List<Enemy>();
    private List<Enemy> currentAttackers = new List<Enemy>();
    private List<Enemy> enemiesWaitingToAttack = new List<Enemy>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("[EnemyWaveManager] Awake");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterEnemy(Enemy enemy)
    {
        if (!activeEnemies.Contains(enemy))
        {
            activeEnemies.Add(enemy);
            Debug.Log($"[WaveManager] {enemy.name} registrado. Total: {activeEnemies.Count}");
        }
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        activeEnemies.Remove(enemy);
        currentAttackers.Remove(enemy);
        enemiesWaitingToAttack.Remove(enemy);
        Debug.Log($"[WaveManager] {enemy.name} desregistrado. Total: {activeEnemies.Count}");
    }

    public void RequestAttackPermission(Enemy enemy)
    {
        if (!enemy || enemiesWaitingToAttack.Contains(enemy))
            return;

        // NO añadir si está en cooldown
        if (enemy.isOnAttackCooldown)
        {
            Debug.Log($"[WaveManager] {enemy.name} en cooldown, no puede pedir turno.");
            return;
        }

        enemiesWaitingToAttack.Add(enemy);
        Debug.Log($"[WaveManager] {enemy.name} solicita turno. En espera: {enemiesWaitingToAttack.Count}");

        TryGrantNextAttack();
    }

    public void CancelAttackRequest(Enemy enemy)
    {
        if (enemiesWaitingToAttack.Contains(enemy))
        {
            enemiesWaitingToAttack.Remove(enemy);
            Debug.Log($"[WaveManager] {enemy.name} cancela solicitud.");
        }
    }

    public void NotifyAttackStarted(Enemy enemy)
    {
        if (!currentAttackers.Contains(enemy))
        {
            currentAttackers.Add(enemy);
        }
        Debug.Log($"[WaveManager] {enemy.name} START attack. Attacking: {currentAttackers.Count}/{maxSimultaneousAttackers}");
    }

    public void NotifyAttackEnded(Enemy enemy)
    {
        currentAttackers.Remove(enemy);
        Debug.Log($"[WaveManager] {enemy.name} END attack. Attacking: {currentAttackers.Count}/{maxSimultaneousAttackers}");

        TryGrantNextAttack();
    }

    private void TryGrantNextAttack()
    {
        if (currentAttackers.Count >= maxSimultaneousAttackers)
            return;

        for (int i = 0; i < enemiesWaitingToAttack.Count; i++)
        {
            Enemy e = enemiesWaitingToAttack[i];
            
            if (e == null)
            {
                enemiesWaitingToAttack.RemoveAt(i);
                i--;
                continue;
            }

            // NO dar turno si está en cooldown
            if (e.isOnAttackCooldown)
            {
                Debug.Log($"[WaveManager] {e.name} en cooldown, se salta.");
                continue;
            }

            // Dar permiso
            e.AllowAttackFromManager();
            enemiesWaitingToAttack.RemoveAt(i);
            Debug.Log($"[WaveManager] {e.name} recibe permiso para atacar.");
            return;
        }
    }

    public bool IsWaveCleared()
    {
        return activeEnemies.Count == 0;
    }
    
    
}