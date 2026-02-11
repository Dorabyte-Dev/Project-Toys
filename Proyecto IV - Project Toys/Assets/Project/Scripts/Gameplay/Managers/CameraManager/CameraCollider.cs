using UnityEngine;
using UnityEngine.Serialization;
using static ZoneCloser;

public class CameraCollider : MonoBehaviour
{
    public bool hasBeenActivated;
    public LayerMask playerMask;
    private CameraManager camManager;
    [FormerlySerializedAs("cam")] [SerializeField]private CameraGroup camGroup;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camManager = CameraManager.instance;
        if(camGroup == null)
        {
            Debug.LogError($"CameraCollider with name: {name} has not been assigned a CameraGroup! Please assign one in the inspector.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((playerMask & (1 << other.gameObject.layer)) != 0 && !hasBeenActivated)
        {
            camManager.SwitchCameraGroup(camGroup);
        }
    }
}
