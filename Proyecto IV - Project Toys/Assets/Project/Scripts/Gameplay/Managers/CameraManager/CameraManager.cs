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
        // TODO: Implementar zoom para RotationComposer
        Debug.LogWarning("ToggleZoom no está implementado para cámaras con RotationComposer");
    }
    private void UntoggleZoomRotationComposer()
    {
        // TODO: Implementar zoom para RotationComposer
        Debug.LogWarning("ToggleZoom no está implementado para cámaras con RotationComposer");
    }
}
