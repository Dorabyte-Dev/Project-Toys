using UnityEngine;

public class PerfectDodgeCollider : MonoBehaviour
{

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Damage"))
        {
            PerfectDodgeManager.SetPerfectDodgeFlag(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Damage"))
        {
            PerfectDodgeManager.EndPerfectDodgeFlag(other.gameObject);
        }
    }
}
