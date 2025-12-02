using System.Collections.Generic;
using UnityEngine;

public class PerfectDodgeManager : MonoBehaviour
{
    //Singleton


    //Perfect Dodge
    public static List<GameObject> pDodgeEnemies = new List<GameObject>();

    public static void SetPerfectDodgeFlag(GameObject enemy)
    {
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
}
