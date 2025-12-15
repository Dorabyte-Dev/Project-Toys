using System;
using System.Collections.Generic;
using UnityEngine;

public class PerfectDodgeManager : MonoBehaviour
{
    //Perfect Dodge
    public static List<GameObject> pDodgeEnemies = new List<GameObject>();
    //Non-static list to debug in inspector
    public List<GameObject> debug_pDodgeEnemies = new List<GameObject>();


    public void Start()
    {
        //Invert the static list to the non-static one for debugging
        //pDodgeEnemies = debug_pDodgeEnemies;

    }

    private void Update()
    {
        debug_pDodgeEnemies = pDodgeEnemies;
    }

    public static void SetPerfectDodgeFlag(GameObject enemy)
    {
        if (pDodgeEnemies.Contains(enemy)) return;
        
        pDodgeEnemies.Add(enemy);
        Debug.Log("He puesto la Flag de perfect dodge");
    }

    public static void EndPerfectDodgeFlag(GameObject enemy)
    {
        if (pDodgeEnemies.Contains(enemy))
        {
            pDodgeEnemies.Remove(enemy);
        }
    }
    
    public static void WipePerfectDodgeFlags()
    {
        pDodgeEnemies.Clear();
    }
}
