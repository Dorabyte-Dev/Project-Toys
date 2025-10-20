using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using static UnityEditor.Experimental.GraphView.GraphView;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class ZoneCloser : MonoBehaviour
{
    Rigidbody rb;
    public LayerMask playerMask;
    public UnityEvent zoneEvent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((playerMask & (1 << other.gameObject.layer)) != 0)
        {
            //Close zone
            //Trigger Zone Event
            //Wait for Zone Unlocker
        }
    }

    void CloseZone()
    {

    }

    void TriggerZoneEvent()
    {

    }

    void WaitForZoneUnlocker()
    {

    }
}
