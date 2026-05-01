using System;
using UnityEngine;

public class PerfectDodgeCollider : MonoBehaviour
{
    public LayerMask whatIsPerfectDodge;
    private void OnTriggerEnter(Collider other)
    {
        if ((whatIsPerfectDodge & (1 << other.gameObject.layer)) != 0)
        {
            PerfectDodgeManager.SetPerfectDodgeFlag(other.transform.root.gameObject); //Risky
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if ((whatIsPerfectDodge & (1 << other.gameObject.layer)) != 0)
        {
            PerfectDodgeManager.EndPerfectDodgeFlag(other.transform.root.gameObject); //Risky
        }
    }
}
