using System;
using System.Collections.Generic;
using UnityEngine;

public class PerfectDodgeManager : MonoBehaviour
{
    //Perfect Dodge
    public static List<GameObject> pDodgeEnemies = new List<GameObject>();
    //Non-static list to debug in inspector
    public List<GameObject> debug_pDodgeEnemies = new List<GameObject>();
    
    public static event Action OnPerfectDodgeFlagSet;
    public static  event Action OnPerfectDodgeFlagRemoved;

    private void Update()
    {
        debug_pDodgeEnemies = pDodgeEnemies;
    }

    public static void SetPerfectDodgeFlag(GameObject enemy)
    {
        if (pDodgeEnemies.Contains(enemy)) return;
        
        pDodgeEnemies.Add(enemy);
        OnPerfectDodgeFlagSet?.Invoke();
    }

    public static void EndPerfectDodgeFlag(GameObject enemy)
    {
        if (pDodgeEnemies.Contains(enemy))
        {
            Debug.Log("Flag Removed");
            pDodgeEnemies.Remove(enemy);
            OnPerfectDodgeFlagRemoved?.Invoke();
        }
        else
        {
            Debug.LogWarning("List doesn't contain the enemy");
        }
    }

    public static bool IsPerfectDodge()
    {
        if (pDodgeEnemies.Count <= 0) return false;

        bool b = false;
        foreach (GameObject enemy in pDodgeEnemies)
        {
            if (enemy != null)
            {
                b = true;
                break;
            }
        }

        return b;
    }
    
    public static GameObject GetPerfectDodgeEnemy()
    {
        if (pDodgeEnemies.Count <= 0) return null;

        for (int i = pDodgeEnemies.Count - 1; i >= 0; i--)
        {
            if (pDodgeEnemies[i] != null)
            {
                return pDodgeEnemies[i];
            }
        }

        return null;
    }
    
    public static void WipePerfectDodgeFlags()
    {
        pDodgeEnemies.Clear();
        OnPerfectDodgeFlagRemoved?.Invoke();
    }
}
