using System;
using UnityEngine;
using DG.Tweening;

public class TVScreenManager : MonoBehaviour
{
    [SerializeField] private MenuManager mainMenuManager;

    [Header("Camera Travelling")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform tvFocusPoint;
    [SerializeField] private float travelDuration = 1.2f;
    [SerializeField] private Ease travelEase = Ease.InOutSine;

    [Header("UI")]
    [SerializeField] private Canvas optionsCanvas;
    [SerializeField] private CanvasGroup optionsCanvasGroup;
    [SerializeField] private Transform optionsRoot;

    [Header("TV Screen Material")]
    [SerializeField] private Renderer tvScreenRenderer;
    [SerializeField] private BlockButton [] selectionButtons;

    private float flickerTime;
    [SerializeField] private Material tvScreenMaterial;
    [SerializeField] private Color tvOnEmission = Color.white;
    [SerializeField] private Color tvOffEmission = Color.black;

    // Guardamos posición/rotación inicial en variables, no en Transform
    private Vector3 startPosition;
    private Quaternion startRotation;
    private bool isOpen = false;

    private void Awake()
    {
        tvScreenMaterial = tvScreenRenderer.material;
    }

    private void Start()
    {
        // Guardamos la posición inicial de la cámara al arrancar
        startPosition = mainCamera.transform.position;
        startRotation = mainCamera.transform.rotation;

        if (optionsCanvas != null) optionsCanvas.gameObject.SetActive(false);
        SetTVEmission(false);
    }

    public void OpenOptionsTV()
    {
        if (isOpen) return;
        isOpen = true;

        if (mainMenuManager != null) mainMenuManager.enabled = false;

        mainCamera.transform.DOMove(tvFocusPoint.position, travelDuration).SetEase(travelEase);
        mainCamera.transform.DORotateQuaternion(tvFocusPoint.rotation, travelDuration).SetEase(travelEase)
            .OnComplete(() =>
            {
                SetTVEmission(true);

                // ===== [MEJORA UI] Animación de entrada =====
                if (optionsCanvas != null) optionsCanvas.gameObject.SetActive(true);

                if (optionsCanvasGroup != null && optionsRoot != null)
                {
                    optionsCanvasGroup.alpha = 0f;
                    optionsRoot.localScale = Vector3.one * 0.8f;

                    optionsCanvasGroup.DOFade(1f, 0.3f);
                    optionsRoot.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
                }
            });
    }

    public void CloseOptionsTV()
    {
        if (!isOpen) return;
        isOpen = false;

        if (optionsCanvas != null) optionsCanvas.gameObject.SetActive(false);
        SetTVEmission(false);
        if (selectionButtons.Length > 0)
        {
            foreach (var button in selectionButtons)
            {
                if (button != null) button.ResetBlock();
            }
        }


        // Volver a la posición guardada al inicio
        mainCamera.transform.DOMove(startPosition, travelDuration).SetEase(travelEase);
        mainCamera.transform.DORotateQuaternion(startRotation, travelDuration).SetEase(travelEase)
            .OnComplete(() =>
            {
                if (mainMenuManager != null) mainMenuManager.enabled = true;
            });
    }

    private void SetTVEmission(bool on)
    {
        if (tvScreenMaterial == null) return;

        if (on)
        {
            tvScreenMaterial.SetColor("_EmissionColor", Color.white);
            tvScreenMaterial.SetFloat("_UseEmissiveIntensity", 1f);
            tvScreenMaterial.SetFloat("_EmissiveIntensity", 2f);
            tvScreenMaterial.EnableKeyword("_EMISSION");
            tvScreenMaterial.DOFloat(0.5f, "_EmissiveExposureWeight", 1f);
            tvScreenMaterial.DOFloat(2f, "_EmissiveIntensity", 1f).SetEase(Ease.OutExpo);
            
        }
        else
        {
            tvScreenMaterial.DOFloat(1f, "_EmissiveExposureWeight", 0.5f);
            tvScreenMaterial.DOFloat(0f, "_EmissiveIntensity", 0.5f).OnComplete(() => {
                tvScreenMaterial.SetColor("_EmissiveColor", Color.black);
                tvScreenMaterial.DisableKeyword("_EMISSION");
            });
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O)) OpenOptionsTV();
        if (Input.GetKeyDown(KeyCode.P)) CloseOptionsTV();

        // ===== Flicker =====
        if (isOpen && tvScreenMaterial != null)
        {
            flickerTime += Time.deltaTime * 5f;
            
            float flicker = Mathf.PerlinNoise(flickerTime, 0f);
            float intensity = 1.5f + flicker * 1.5f;
    
            tvScreenMaterial.EnableKeyword("_EMISSION");
            tvScreenMaterial.SetFloat("_EmissiveIntensity", intensity);
    
            Color finalColor = tvOnEmission * (0.85f + flicker * 0.2f);
            tvScreenMaterial.SetColor("_EmissiveColor", finalColor);
        }
    }
}