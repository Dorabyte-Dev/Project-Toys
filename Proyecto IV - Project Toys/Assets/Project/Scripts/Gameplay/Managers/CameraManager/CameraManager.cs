using Unity.Cinemachine;
using UnityEngine;
using DG.Tweening;

public class CameraManager : MonoBehaviour
{
    private CameraSwitch currentOffCombatCamera;
    private CameraSwitch currentOnCombatCamera;
    [SerializeField]private int offCombatPriority;
    [SerializeField]private int onCombatPriority;
    private CinemachineBrain cameraBrain;
    public CinemachineCamera activeCamera => cameraBrain.ActiveVirtualCamera as CinemachineCamera;
    private CinemachinePositionComposer positionComposer;
    [SerializeField]private float zoomAmount = 30f;
    private bool isZoomedIn = false;
    private float originalCameraDistance;
    private Vector3 originalCameraPosition;

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
    //Switch OnCombat Camera
    public void SwitchOnCombatCamera(CameraSwitch cam)
    {
        if (currentOnCombatCamera == cam) return;

        if(currentOnCombatCamera != null)
        {
            currentOnCombatCamera.LowerPriority(onCombatPriority);
        }
        currentOnCombatCamera = cam;
        currentOnCombatCamera.RaisePriority(onCombatPriority);
    }
    public void ToggleZoom()
    {
        bool isPositionComposer = activeCamera.TryGetComponent<CinemachinePositionComposer>(out positionComposer);

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
        bool isPositionComposer = activeCamera.TryGetComponent<CinemachinePositionComposer>(out positionComposer);

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
        Transform player = activeCamera.Target.TrackingTarget;
        // Guardar la distancia original
        originalCameraPosition = activeCamera.transform.position;
        
        Vector3 direction = player.position - originalCameraPosition;
        // Calcular la nueva distancia aplicando el porcentaje de zoom
        Vector3 targetPosition = originalCameraPosition + direction * zoomAmount / 100f;
            
        // Animar hacia la distancia con zoom
        DOTween.To(() => activeCamera.transform.position,
            x => activeCamera.transform.position = x,
            targetPosition,
            0.1f).SetEase(Ease.OutCirc);
    }
    private void UntoggleZoomRotationComposer()
    {
        DOTween.To(() => activeCamera.transform.position,
            x => activeCamera.transform.position = x,
            originalCameraPosition,
            0.1f).SetEase(Ease.OutCirc);
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
