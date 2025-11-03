using UnityEngine;
using static ZoneCloser;

public class CameraCollider : MonoBehaviour
{
    public bool hasBeenActivated;
    public LayerMask playerMask;
    private CameraManager camManager;
    [SerializeField]private CameraSwitch cam;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camManager = FindAnyObjectByType<CameraManager>();
        if(cam == null)
        {
            cam = GetComponent<CameraSwitch>();
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
            camManager.SwitchOffCombatCamera(cam);
        }
    }
}
