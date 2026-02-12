using UnityEngine;

public class SeeThroughWallManager : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private Camera mainCamera;

    [Header("Cutout Settings")]
    [SerializeField] private float maxCutoutSize = 0.15f;
    [SerializeField] private float falloffSize = 0.05f;

    [Header("Animation")]
    [SerializeField] private float smoothSpeedIn = 8f;
    [SerializeField] private float smoothSpeedOut = 6f;

    [Header("DEBUG: Ajuste Manual de Coordenadas")]
    [SerializeField] private bool invertX = false;
    [SerializeField] private bool invertY = false;
    [SerializeField] private bool useAlternativeAspect = false;
    [SerializeField] private Vector2 manualOffset = Vector2.zero;

    private float currentCutoutSize = 0f;
    private bool wasObstructedLastFrame = false;

    private void Awake()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        
        Shader.SetGlobalFloat("_CutoutSize", 0f);
        Shader.SetGlobalFloat("_FalloutSize", falloffSize);
    }

    private void Update()
    {
        if (player == null) return;

        // 1. Detectar obstrucción
        Vector3 direction = player.position - mainCamera.transform.position;
        bool isObstructed = Physics.Raycast(mainCamera.transform.position, direction, direction.magnitude, wallMask);

        // 2. Calcular posición en pantalla
        Vector3 screenPoint = mainCamera.WorldToScreenPoint(player.position);

        // 3. Normalizar a -0.5 a 0.5
        float screenX = (screenPoint.x / Screen.width) - 0.5f;
        float screenY = (screenPoint.y / Screen.height) - 0.5f;

        // 4. Aplicar inversiones si es necesario (DEBUG)
        if (invertX) screenX *= -1f;
        if (invertY) screenY *= -1f;

        // 5. Aplicar aspect ratio
        float aspect = (float)Screen.width / Screen.height;
        Vector2 cutoutPos;

        if (useAlternativeAspect)
        {
            // Método alternativo: dividir Y en lugar de multiplicar X
            cutoutPos = new Vector2(screenX, screenY / aspect);
        }
        else
        {
            // Método estándar: multiplicar X
            cutoutPos = new Vector2(screenX * aspect, screenY);
        }

        // 6. Aplicar offset manual (DEBUG)
        cutoutPos += manualOffset;

        // 7. Animar tamaño
        float targetSize = isObstructed ? maxCutoutSize : 0f;
        float speed = isObstructed ? smoothSpeedIn : smoothSpeedOut;
        currentCutoutSize = Mathf.Lerp(currentCutoutSize, targetSize, Time.deltaTime * speed);

        // 8. Enviar al shader
        Shader.SetGlobalVector("_CutoutPosition", cutoutPos);
        Shader.SetGlobalFloat("_CutoutSize", currentCutoutSize);
        Shader.SetGlobalFloat("_FalloutSize", falloffSize);

        // Debug visual
        Debug.DrawRay(mainCamera.transform.position, direction, isObstructed ? Color.red : Color.green);
        
        if (isObstructed != wasObstructedLastFrame)
        {
            Debug.Log($"Obstrucción: {(isObstructed ? "BLOQUEADO" : "LIBRE")} | Size: {currentCutoutSize:F3} | Pos: {cutoutPos}");
        }
        
        wasObstructedLastFrame = isObstructed;
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            Shader.SetGlobalFloat("_FalloutSize", falloffSize);
        }
    }
}