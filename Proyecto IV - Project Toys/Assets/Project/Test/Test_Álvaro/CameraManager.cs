using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private CameraSwitch currentOffCombatCamera;
    private CameraSwitch currentOnCombatCamera;
    [SerializeField]private int offCombatPriority;
    [SerializeField]private int onCombatPriority;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToggleOnCombatCamera(CameraSwitch cam)
    {
        currentOnCombatCamera = cam;
        currentOnCombatCamera.RaisePriority(onCombatPriority);
    }

    public void UnToggleOnCombatCamera()
    {
        currentOnCombatCamera.LowerPriority(onCombatPriority);
    }

    public void SwitchOffCombatCamera(CameraSwitch cam)
    {
        if (currentOffCombatCamera == cam) return;

        if(currentOffCombatCamera != null)
        {
            currentOffCombatCamera.LowerPriority(offCombatPriority);
        }
        currentOffCombatCamera = cam;
        currentOffCombatCamera.RaisePriority(offCombatPriority);
    }
}
