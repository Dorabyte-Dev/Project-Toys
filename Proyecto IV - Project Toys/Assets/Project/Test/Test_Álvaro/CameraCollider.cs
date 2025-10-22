using UnityEngine;
using DG.Tweening;
using Unity.Cinemachine;

public class CameraCollider : MonoBehaviour
{
    /*Camera Switching*/
    public CinemachineCamera cam;
    private Player player;
    private int priority;

    void Start()
    {
        player = FindAnyObjectByType<Player>();
    }

    void Update()
    {
        
    }

    public void RaisePriority()
    {
        cam.Priority += priority;
    }

    public void LowerPriority()
    {
        cam.Priority -= priority;
    }

    /*void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("I am " + gameObject.name + " and I have priority");
            cam.Priority += 3;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("I am " + gameObject.name + " and I no longer have priority");
            cam.Priority -= 3;
        }
    }*/
}
