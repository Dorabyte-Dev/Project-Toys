using DG.Tweening;
using Unity.Cinemachine;
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
    public ZoneDoor[] doors;
    [SerializeField]private CinemachineCamera openZoneCamera;

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
        //Debug.Log("I detected " +  other.gameObject.name + "with layer being " + playerMask.ToString() + "and condition is " + (playerMask & (1 << other.gameObject.layer)));
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
        foreach (var door in doors)
        {
            //Animation? Maybe?
            door.gameObject.SetActive(true);
            door.Close();
        }
    }

    void OpenZone()
    {
        //Cinemachine Change to other camera
        if (openZoneCamera != null)
        {
            //Camera looking at door
            openZoneCamera.Priority = 11;
            DOVirtual.DelayedCall(1f, () =>
            {
                //Doors open
                foreach (var door in doors)
                {
                    door.Open();
                }
            });
            DOVirtual.DelayedCall(2f, () =>
            {
                //Camera back to player
                openZoneCamera.Priority = 0;
                DOVirtual.DelayedCall(2f, () =>
                {
                    //Player can move again
                    FindFirstObjectByType<Player>().GrantControl();
                });
            });
        }
        else
        {
            foreach (var door in doors)
            {
                door.Open();
            }
            FindFirstObjectByType<Player>().GrantControl();
            Debug.LogWarning("Open Zone Camera not assigned!");
        }
    }
    
    public void ResetZoneCloser()
    {
        hasBeenActivated = false;
        foreach(ZoneDoor door in doors)
        {
            if (door.startClosed)
            {
                door.Close();
            }
            else
            {
                door.Open();
            }
        }
    }
    
    public void DebugReset()
    {
    }
}
