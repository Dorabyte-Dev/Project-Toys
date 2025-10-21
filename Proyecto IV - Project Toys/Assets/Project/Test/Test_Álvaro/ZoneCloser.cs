using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class ZoneCloser : MonoBehaviour
{
    Rigidbody rb;
    public bool hasBeenActivated;
    public LayerMask playerMask;
    public ZoneEvent zoneEvent;
    public enum EventType
    {
        Combat,
        Puzzle
    }
    [System.Serializable]
    public struct ZoneEvent
    {
        public string name;
        public EventType eventType; 
        public UnityEvent uEvent;
        public EnemySpawner spawner;
    }

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
        if ((playerMask & (1 << other.gameObject.layer)) != 0 && !hasBeenActivated)
        {
            //Disable the Zone Closer
            hasBeenActivated = true;

            //Close zone
            CloseZone();

            //Trigger Zone Event
            zoneEvent.uEvent.Invoke();
            Debug.Log("Zone Event: " + zoneEvent.name + " activated");

            //Wait for Zone Unlocker
            zoneEvent.spawner.endCombat.AddListener(OpenZone);
        }
    }

    void CloseZone()
    {
        Debug.Log("Zone Closed");
    }

    void OpenZone()
    {
        Debug.Log("ZoneOpen");
    }
}
