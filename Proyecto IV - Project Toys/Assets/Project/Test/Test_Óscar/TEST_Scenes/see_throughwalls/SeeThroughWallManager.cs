using System.Collections.Generic;
using UnityEngine;

public class SeeThroughWallManager : MonoBehaviour
{
    [SerializeField] 
    private Transform player;
    
    [SerializeField]
    private LayerMask wallMask;
    
    [SerializeField]
    private Camera mainCamera;

    [Header("Cutout Settings")]
    [SerializeField]
    private float maxCutoutSize = 0.1f;
    
    [SerializeField]
    private float falloffSize = 0.05f;

    [Header("Animation of Cutout")]
    [SerializeField]
    private float smoothSpeedIn = 8f;   // velocidad de agrandarse
    [SerializeField]
    private float smoothSpeedOut = 6f;  // velocidad de encogerse

    private Dictionary<Material, float> materialCutoutSizes = new Dictionary<Material, float>();
    private HashSet<Material> activeMaterials = new HashSet<Material>();

    private void Awake()
    {
        if (mainCamera == null)
            mainCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        activeMaterials.Clear();
        
        // Raycast entre cámara y player
        Vector3 offset = player.position - transform.position;
        RaycastHit[] hitObjects = Physics.RaycastAll(
            transform.position, 
            offset, 
            offset.magnitude, 
            wallMask
        );

        // Posición del círculo en pantalla
        Vector2 cutoutPos = mainCamera.WorldToViewportPoint(player.position);
        cutoutPos.y /= (Screen.width / Screen.height);

        // Marcar materiales activos este frame
        if (hitObjects.Length > 0)
        {
            for (int i = 0; i < hitObjects.Length; i++)
            {
                Material[] materials = hitObjects[i]
                    .transform
                    .GetComponent<Renderer>()
                    .materials;

                for (int j = 0; j < materials.Length; j++)
                {
                    Material mat = materials[j];
                    activeMaterials.Add(mat);

                    if (!materialCutoutSizes.ContainsKey(mat))
                    {
                        materialCutoutSizes[mat] = 0f;
                    }
                }
            }
        }
        var snapshot = new Dictionary<Material, float>(materialCutoutSizes);
        foreach (var kvp in snapshot)
        {
            Material mat = kvp.Key;
            float currentSize = kvp.Value;

            bool isActive = activeMaterials.Contains(mat);
            float targetSize = isActive ? maxCutoutSize : 0f;
            float speed = isActive ? smoothSpeedIn : smoothSpeedOut;

            // Lerp → animación suave (ease-out)
            float newSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * speed);
            materialCutoutSizes[mat] = newSize;

            // Actualizar shader
            mat.SetVector("_CutoutPosition", cutoutPos);
            mat.SetFloat("_CutoutSize", newSize);
            mat.SetFloat("_FalloutSize", falloffSize);

            if (!isActive && newSize <= 0.001f)
            {
                materialCutoutSizes.Remove(mat);
            }
        }

        Debug.DrawLine(transform.position, player.position, Color.red);
    }
}