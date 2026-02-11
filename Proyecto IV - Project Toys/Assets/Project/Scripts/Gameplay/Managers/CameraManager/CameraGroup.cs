using UnityEngine;

public class CameraGroup : MonoBehaviour
{
    public CameraSwitch explorationCamera;
    public CameraSwitch combatCamera;

    private CameraSwitch _activeCamera;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(explorationCamera == null)
        {
            combatCamera = explorationCamera;
        }
        else if(combatCamera == null)
        {
            explorationCamera = combatCamera;
        }
        else if(explorationCamera == null && combatCamera == null)
        {
            Debug.LogError($"CameraGroup with name: {name} has not been assigned any cameras! Please assign at least one in the inspector.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SwitchOnGroup()
    {
        if (CameraManager.instance.IsOnCombat)
        {
            _activeCamera = combatCamera;
            _activeCamera.RaisePriority(CameraManager.instance.onCombatPriority);
        }
        else
        {
            _activeCamera = explorationCamera;
            _activeCamera.RaisePriority(CameraManager.instance.offCombatPriority);
        }
            
    }
    
    public void SwitchOffGroup()
    {
        if (_activeCamera == combatCamera)
        {
            
            _activeCamera.LowerPriority(CameraManager.instance.onCombatPriority);
        }
        else
        {
            _activeCamera.LowerPriority(CameraManager.instance.offCombatPriority);
        }

        _activeCamera = null;
    }
    
    public void SwitchToCombat()
    {
        if (_activeCamera == combatCamera) return;

        if(_activeCamera != null)
        {
            _activeCamera.LowerPriority(CameraManager.instance.offCombatPriority);
        }
        _activeCamera = combatCamera;
        _activeCamera.RaisePriority(CameraManager.instance.onCombatPriority);
    }
    
    public void SwitchToExploration()
    {
        if (_activeCamera == explorationCamera) return;

        if(_activeCamera != null)
        {
            _activeCamera.LowerPriority(CameraManager.instance.onCombatPriority);
        }
        _activeCamera = explorationCamera;
        _activeCamera.RaisePriority(CameraManager.instance.offCombatPriority);
    }
    
}
