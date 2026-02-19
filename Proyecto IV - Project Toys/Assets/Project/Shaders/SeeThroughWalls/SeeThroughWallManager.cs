using UnityEngine;

public class SeeThroughWallManager : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform shaderTarget;
    [SerializeField] private LayerMask wallMask;
    [SerializeField] private Camera mainCamera;

    [Header("Cutout Settings")]
    [Range(0f, 1f)] [SerializeField] private float maxCutoutSize = 0.15f;
    [Range(0f, 0.5f)] [SerializeField] private float falloffSize = 0.05f;

    [Header("Animation")]
    [SerializeField] private float smoothSpeedIn = 8f;
    [SerializeField] private float smoothSpeedOut = 6f;

    private float currentCutoutSize = 0f;
    [SerializeField] private bool isObstructed;

    private void Awake()
    {
        if (mainCamera == null) mainCamera = Camera.main;
        Shader.SetGlobalFloat("_CutoutSize", 0f);
    }

    private void Update()
    {
        if (player == null || shaderTarget == null) return;

        // 1. Raycast: Solo detecta si hay algo ENTRE la cámara y el jugador
        Vector3 camPos = mainCamera.transform.position;
        Vector3 targetPos = player.position;
        Vector3 direction = targetPos - camPos;
        
        // Usamos direction.magnitude - 0.1f para no detectar el suelo bajo los pies del player
        isObstructed = Physics.Raycast(camPos, direction, direction.magnitude - 0.5f, wallMask);

        // 2. Posición en pantalla (Normalizada -0.5 a 0.5)
        Vector3 screenPoint = mainCamera.WorldToScreenPoint(shaderTarget.position);
        Vector2 cutoutPos = new Vector2(
            (screenPoint.x / Screen.width) - 0.5f,
            (screenPoint.y / Screen.height) - 0.5f
        );

        // 3. Animación del tamaño
        float targetSize = isObstructed ? maxCutoutSize : 0f;
        float speed = isObstructed ? smoothSpeedIn : smoothSpeedOut;

        currentCutoutSize = Mathf.Lerp(currentCutoutSize, targetSize, Time.deltaTime * speed);

        if (!isObstructed && currentCutoutSize < 0.001f)
        {
            currentCutoutSize = 0f;
        }

        // 4. Enviar al Shader
        Shader.SetGlobalVector("_CutoutPosition", cutoutPos);
        Shader.SetGlobalFloat("_CutoutSize", currentCutoutSize);
        Shader.SetGlobalFloat("_FalloutSize", falloffSize);

        // Debug
        Debug.DrawRay(camPos, direction.normalized * (direction.magnitude - 0.5f), isObstructed ? Color.red : Color.green);
        
        if (!isObstructed)
        {
            Shader.SetGlobalFloat("_CutoutSize", 0f);
        }
        else
        {
            Shader.SetGlobalFloat("_CutoutSize", currentCutoutSize);
        }
    }
}