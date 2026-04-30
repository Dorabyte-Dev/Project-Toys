using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Serialization;

public class CameraManager : MonoBehaviour
{
    private CameraSwitch currentOffCombatCamera;
    private CameraSwitch currentOnCombatCamera;
    private CameraGroup activeCameraGroup;
    public int offCombatPriority;
    public int onCombatPriority;
    private CinemachineBrain cameraBrain;
    public CinemachineCamera activeCamera => cameraBrain.ActiveVirtualCamera as CinemachineCamera;
    private CinemachinePositionComposer positionComposer;
    [SerializeField]private float zoomAmount = 30f;
    private bool isZoomedIn = false;
    private float originalCameraDistance;
    private Vector3 originalCameraPosition;
    private bool isOnCombat;

    public bool IsOnCombat
    {
        get => isOnCombat;
        set
        {
            isOnCombat = value;
                if (activeCameraGroup != null)
                {
                    if (isOnCombat)
                    {
                        activeCameraGroup.SwitchToCombat();
                    }
                    else
                    {
                        activeCameraGroup.SwitchToExploration();
                    }
                }
        }
    }


    private Queue<CinemachineCamera> _zoomedCameras = new Queue<CinemachineCamera>();
    private CinemachineCamera _currentZoomedCamera;
    private Tween _currentShakeTween;
    [SerializeField]private float shakeDuration = 0.5f;
    [SerializeField] private float shakeStrength = 1f;
    [SerializeField] private int shakeVibrato = 10;

    public static CameraManager instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraBrain = FindAnyObjectByType<CinemachineBrain>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    #region Old Camera System
    public void ToggleOnCombatCamera()
    {
        IsOnCombat = true;
    }

    public void UnToggleOnCombatCamera()
    {
        IsOnCombat = false;
    }

    /*public void SwitchOffCombatCamera(CameraSwitch cam)
    {
        if (currentOffCombatCamera == cam) return;

        if(currentOffCombatCamera != null)
        {
            currentOffCombatCamera.LowerPriority(offCombatPriority);
        }
        currentOffCombatCamera = cam;
        currentOffCombatCamera.RaisePriority(offCombatPriority);
    }*/
    //Switch OnCombat Camera
    /*public void SwitchOnCombatCamera(CameraSwitch cam)
    {
        if (currentOnCombatCamera == cam) return;

        if(currentOnCombatCamera != null)
        {
            currentOnCombatCamera.LowerPriority(onCombatPriority);
        }
        currentOnCombatCamera = cam;
        currentOnCombatCamera.RaisePriority(onCombatPriority);
    }*/
    #endregion
    
    
    public void SwitchCameraGroup(CameraGroup camGroup)
    {
        if (activeCameraGroup)
        {
            activeCameraGroup.SwitchOffGroup();
        }
        activeCameraGroup = camGroup;
        activeCameraGroup.SwitchOnGroup();
    }

    #region Zoom

    public void ToggleZoom()
    {
        _zoomedCameras.Enqueue(activeCamera);
        _currentZoomedCamera = activeCamera;
        
        bool isPositionComposer = _currentZoomedCamera.TryGetComponent<CinemachinePositionComposer>(out positionComposer);

        if (isPositionComposer)
        {
            ToggleZoomPositionComposer();
        }
        else
        {
            ToggleZoomRotationComposer();
        }
        
        

    }

    public void UntoggleZoom()
    {
        _currentZoomedCamera = _zoomedCameras.Dequeue();
        bool isPositionComposer = _currentZoomedCamera.TryGetComponent<CinemachinePositionComposer>(out positionComposer);

        if (isPositionComposer)
        {
            UntoggleZoomPositionComposer();
        }
        else
        {
            UntoggleZoomRotationComposer();
        }
    }

    private void ToggleZoomPositionComposer()
    {
        // Guardar la distancia original
        originalCameraDistance = positionComposer.CameraDistance;
        
        // Calcular la nueva distancia aplicando el porcentaje de zoom
        float targetDistance = originalCameraDistance * (1f - zoomAmount / 100f);
            
        // Animar hacia la distancia con zoom
        DOTween.To(() => positionComposer.CameraDistance,
            x => positionComposer.CameraDistance = x,
            targetDistance,
            0.1f).SetEase(Ease.OutCirc);
    }

    private void UntoggleZoomPositionComposer()
    {
        // Animar de vuelta a la distancia original
        DOTween.To(() => positionComposer.CameraDistance,
            x => positionComposer.CameraDistance = x,
            originalCameraDistance,
            0.5f).SetEase(Ease.OutSine);
    }

    private void ToggleZoomRotationComposer()
    {
        Transform player = _currentZoomedCamera.Target.TrackingTarget;
        // Guardar la distancia original
        originalCameraPosition = _currentZoomedCamera.transform.position;
        
        Vector3 direction = player.position - originalCameraPosition;
        // Calcular la nueva distancia aplicando el porcentaje de zoom
        Vector3 targetPosition = originalCameraPosition + direction * zoomAmount / 100f;
            
        // Animar hacia la distancia con zoom
        DOTween.To(() => _currentZoomedCamera.transform.position,
            x => _currentZoomedCamera.transform.position = x,
            targetPosition,
            0.1f).SetEase(Ease.OutCirc);
    }
    private void UntoggleZoomRotationComposer()
    {
        DOTween.To(() => _currentZoomedCamera.transform.position,
            x => _currentZoomedCamera.transform.position = x,
            originalCameraPosition,
            0.1f).SetEase(Ease.OutCirc);
    }

    #endregion

    public void CameraShake()
    {
        if (activeCamera == null) return;

        if (_currentShakeTween != null && _currentShakeTween.IsActive())
        {
            _currentShakeTween.Complete();
        }

        //Make it bulletproof 
        _currentShakeTween = activeCamera.transform.DOShakePosition(shakeDuration, new Vector3(shakeStrength, shakeStrength, 0f), shakeVibrato, 90f, false, true);
    }
    
    public void CameraShake(float duration, float strength, int vibrato)
    {
        if (activeCamera == null) return;

        if (_currentShakeTween != null && _currentShakeTween.IsActive())
        {
            _currentShakeTween.Complete();
        }

        //Make it bulletproof 
        _currentShakeTween = activeCamera.transform.DOShakePosition(duration, new Vector3(strength, strength, 0f), vibrato, 90f, false, true);
    }
    public void ResetColliders()
    {
        //Reset all camera collider scripts in the scene
        CameraCollider[] colliders = FindObjectsByType<CameraCollider>(FindObjectsSortMode.None);
        foreach (CameraCollider col in colliders)
        {            
            col.hasBeenActivated = false;
        }
    }
}
