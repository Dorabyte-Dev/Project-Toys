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

    private void Update()
    {
        if (player == null || shaderTarget == null) return;

        Vector3 camPos = mainCamera.transform.position;

        // 1. Detección con múltiples rayos
        isObstructed = CheckObstruction(camPos, player.position);

        // 2. Posición en pantalla del shaderTarget
        Vector3 screenPoint = mainCamera.WorldToScreenPoint(shaderTarget.position);
        float aspect = (float)Screen.width / Screen.height;
        Vector2 cutoutPos = new Vector2(
            ((screenPoint.x / Screen.width) - 0.5f) * aspect,  // Corregir aspect ratio aquí
            (screenPoint.y / Screen.height) - 0.5f
        );

        // 3. Animación del tamaño
        float targetSize = isObstructed ? maxCutoutSize : 0f;
        float speed = isObstructed ? smoothSpeedIn : smoothSpeedOut;
        currentCutoutSize = Mathf.Lerp(currentCutoutSize, targetSize, Time.deltaTime * speed);

        if (!isObstructed && currentCutoutSize < 0.001f)
            currentCutoutSize = 0f;

        // 4. Enviar al Shader
        Shader.SetGlobalVector("_CutoutPosition", cutoutPos);
        Shader.SetGlobalFloat("_CutoutSize", currentCutoutSize);
        Shader.SetGlobalFloat("_FalloutSize", falloffSize);
        if (Input.GetKeyDown(KeyCode.M))
        {
            enableShader = !enableShader; // Toggle con M
            Debug.Log("_EnableShader: " + enableShader);
        }
        Shader.SetGlobalFloat("_EnableShader", enableShader ? 1f : 0f);
    }

    private bool CheckObstruction(Vector3 camPos, Vector3 playerPos)
    {
        Vector3[] checkPoints = new Vector3[]
        {
            playerPos,
            playerPos + Vector3.up * playerHeight,
            playerPos + Vector3.up * (playerHeight * 0.5f),
            playerPos + Vector3.right * playerWidth,
            playerPos + Vector3.left * playerWidth,
        };

        foreach (Vector3 point in checkPoints)
        {
            Vector3 dir = point - camPos;
            bool hit = Physics.Raycast(camPos, dir, out RaycastHit hitInfo, dir.magnitude - raycastEndOffset, wallMask, QueryTriggerInteraction.Ignore);
            Debug.DrawRay(camPos, dir.normalized * (dir.magnitude - raycastEndOffset),
                                hit ? Color.red : Color.green);
            if (hit) return true;
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
}