using UnityEngine;
using Unity.Cinemachine;

public class SeeThroughWallManager : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform shaderTarget;
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private CinemachineBrain cinemachineBrain;

    [Header("Cutout Settings")]
    [Range(0f, 1f)] [SerializeField] private float maxCutoutSize = 0.2f;
    [Range(0f, 0.5f)] [SerializeField] private float falloffSize = 0.05f;
    [SerializeField] private float aspectRatio;
    
    [SerializeField] private float screenAspect = 0.5f;

    [Header("Raycast Settings")]
    [SerializeField] private float raycastEndOffset = 0.3f;
    [SerializeField] private float playerHeight = 1.0f;
    [SerializeField] private float playerWidth = 0.3f;

    [Header("Animation")]
    [SerializeField] private float smoothSpeedIn = 8f;
    [SerializeField] private float smoothSpeedOut = 6f;

    [Header("Debug")]
    [SerializeField] private bool isObstructed;
    [SerializeField] private bool enableShader = true; // Añade esto
    public float debugCutoutSize = 0f; // Para mostrar el tamaño actual del cutout en el Inspector
    public Vector2 debugCutoutPosition = Vector2.zero;
    

    private float currentCutoutSize = 0f;

    private void Awake()
    {
        if (cinemachineBrain == null)
            cinemachineBrain = FindObjectOfType<CinemachineBrain>();
        if (mainCamera == null)
            mainCamera = cinemachineBrain?.GetComponent<Camera>() ?? Camera.main;

        Shader.SetGlobalFloat("_CutoutSize", 0f);
        Shader.SetGlobalFloat("_FalloutSize", falloffSize);
        Shader.SetGlobalFloat("_EnableShader", enableShader ? 1f : 0f);
        
    }

    private void LateUpdate()
    {
        if (player == null || shaderTarget == null || mainCamera == null) return;

        // 1. Detección
        Vector3 camPos = mainCamera.transform.position;
        isObstructed = CheckObstruction(camPos, player.position);

        // 2. Cálculo de posición (Sincronizado con tu Shader Graph)
        Vector3 screenPoint = mainCamera.WorldToScreenPoint(shaderTarget.position);
        
        float resX = screenPoint.x / Screen.width;
        float resY = screenPoint.y / Screen.height;

        float aspect = (float)Screen.width / (float)Screen.height;
        screenAspect = aspect; // Para mostrar el aspecto actual en el Inspector
        Vector2 cutoutPos = new Vector2(resX * aspect, resY);
        debugCutoutPosition = cutoutPos; // Para mostrar la posición actual del cutout en el Inspector

        // 3. Animación
        float targetSize = isObstructed ? maxCutoutSize : 0f;
        float speed = isObstructed ? smoothSpeedIn : smoothSpeedOut;
        currentCutoutSize = Mathf.Lerp(currentCutoutSize, targetSize, Time.deltaTime * speed);
        debugCutoutSize = currentCutoutSize;

        // 4. Envío de datos
        if ((!isObstructed && currentCutoutSize < 0.001f) || CutsceneManager.IsCutsceneActive)
        {
            currentCutoutSize = 0f;
            Shader.SetGlobalFloat("_CutoutSize", 0f);
            return;
        }
        Shader.SetGlobalVector("_CutoutPosition", cutoutPos);
        
        Shader.SetGlobalFloat("_CutoutSize", currentCutoutSize);
        Shader.SetGlobalFloat("_FalloutSize", falloffSize);
        Shader.SetGlobalFloat("_EnableShader", 1f); 
    }

    private bool CheckObstruction(Vector3 camPos, Vector3 playerPos)
    {
        Vector3[] checkPoints = {
            playerPos + Vector3.up * (playerHeight * 0.5f),
            playerPos + Vector3.up * playerHeight,
            playerPos + Vector3.right * playerWidth,
            playerPos + Vector3.left * playerWidth
        };

        foreach (Vector3 pt in checkPoints)
        {
            Vector3 dir = pt - camPos;
            
            if (Physics.Raycast(camPos, dir, dir.magnitude - raycastEndOffset, wallMask))
            {
                Debug.DrawRay(camPos, dir, Color.red);
                return true;
            }
            else
            {
                Debug.DrawRay(camPos, dir, Color.green);
            }
        }
        return false;
    }
    private void OnValidate()
    {
        Shader.SetGlobalFloat("_CutoutSize", 0f);
    }
    private void OnDisable()
    {
        Shader.SetGlobalFloat("_CutoutSize", 0f);
        Shader.SetGlobalFloat("_EnableShader", 0f);
    }
    
    private void OnDestroy()
    {
        Shader.SetGlobalFloat("_CutoutSize", 0f);
        Shader.SetGlobalFloat("_EnableShader", 0f);
    }
    
    /*private void OnGUI()
    {
        if (shaderTarget == null || mainCamera == null) return;
        Vector3 sp = mainCamera.WorldToScreenPoint(shaderTarget.position);
        // Convertir Y (Unity usa Y desde abajo, OnGUI desde arriba)
        float guiY = Screen.height - sp.y;
        // Dibuja un punto rojo exactamente donde el script cree que está el jugador
        GUI.color = Color.red;
        GUI.DrawTexture(new Rect(sp.x - 5, guiY - 5, 10, 10), Texture2D.whiteTexture);
    }*/
}