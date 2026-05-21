using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class OptionsManager : MonoBehaviour
{
    public static OptionsManager Instance;
    public LayerMask enemyLayer;

    [Header("Sliders")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider brightnessSlider;

    [Header("Slider Labels")]
    [SerializeField] private TMP_Text volumeValueText;
    [SerializeField] private TMP_Text brightnessValueText;

    [Header("Dropdowns")]
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    [Header("Toggle")]
    [SerializeField] private float dropPunchDownScale = 0.1f;
    [SerializeField] private float dropdownAnimDuration = 0.2f;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Toggle enemyUIToggle;
    
    public static bool isEnabledUI = true;

    [Header("Post Processing")]
    [SerializeField] private Light directionalLight;
    private float baseIntensity = -1f;
    
    // CORRECCIÓN: Lista paralela para evitar el desfase de índices del Dropdown
    private List<Resolution> filteredResolutions = new List<Resolution>(); 

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Init()
    {
        // Busca el Directional Light en todas las escenas cargadas
        if (directionalLight == null)
        {
            Light[] lights = FindObjectsByType<Light>(FindObjectsSortMode.None);
            foreach (var l in lights)
            {
                if (l.type == LightType.Directional)
                {
                    directionalLight = l;
                    break;
                }
            }
        }

        InitVolume();
        InitBrightness();
        InitQuality();
        InitResolution(); // Ahora filtra correctamente
        InitFullscreen();
        InitEnemyUI();
    }

    private void InitVolume()
    {
        if (volumeSlider == null) return;
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);
        SetVolume(volumeSlider.value);
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    private void InitBrightness()
    {
        if (brightnessSlider == null || directionalLight == null) return;
        if (baseIntensity < 0f) baseIntensity = directionalLight.intensity;

        brightnessSlider.minValue = 0f;
        brightnessSlider.maxValue = 1f;
        brightnessSlider.value = PlayerPrefs.GetFloat("Brightness", 1f);
        SetBrightness(brightnessSlider.value);
        
        brightnessSlider.onValueChanged.RemoveAllListeners();
        brightnessSlider.onValueChanged.AddListener(SetBrightness);
    }

    private void InitQuality()
    {
        if (qualityDropdown == null) return;
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new List<string>(QualitySettings.names));
        
        int savedQuality = PlayerPrefs.GetInt("Quality", QualitySettings.GetQualityLevel());
        qualityDropdown.value = savedQuality;
        
        // CORRECCIÓN: Forzamos la aplicación de la calidad guardada al iniciar
        QualitySettings.SetQualityLevel(savedQuality); 
        
        qualityDropdown.onValueChanged.AddListener(SetQuality);
    }

    private void InitResolution()
    {
        if (resolutionDropdown == null) return;
        
        Resolution[] allResolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        filteredResolutions.Clear();

        List<string> options = new List<string>();
        int currentResIndex = 0;
        
        // Recuperamos la resolución guardada (o usamos la de la pantalla por defecto)
        int savedWidth = PlayerPrefs.GetInt("ResWidth", Screen.currentResolution.width);
        int savedHeight = PlayerPrefs.GetInt("ResHeight", Screen.currentResolution.height);

        for (int i = 0; i < allResolutions.Length; i++)
        {
            string option = allResolutions[i].width + " x " + allResolutions[i].height;
            
            // Si es una resolución única, la guardamos
            if (!options.Contains(option)) 
            {
                options.Add(option);
                filteredResolutions.Add(allResolutions[i]); // Guardamos la real

                if (allResolutions[i].width == savedWidth && allResolutions[i].height == savedHeight)
                {
                    currentResIndex = filteredResolutions.Count - 1;
                }
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResIndex;
        resolutionDropdown.RefreshShownValue();
        
        // CORRECCIÓN: Forzamos la aplicación para pisar el error de Windows Registry
        SetResolution(currentResIndex);
        
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    private void InitFullscreen()
    {
        if (fullscreenToggle == null) return;
        
        // CORRECCIÓN: Recuperamos si era pantalla completa
        bool isFullscreen = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0) == 1;
        fullscreenToggle.isOn = isFullscreen;
        Screen.fullScreen = isFullscreen;
        
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }

    private void InitEnemyUI()
    {
        if (enemyUIToggle == null) return;
        
        // CORRECCIÓN: Respetamos el guardado de la UI
        isEnabledUI = PlayerPrefs.GetInt("EnemyUI", 1) == 1; 
        enemyUIToggle.isOn = isEnabledUI;
        enemyUIToggle.onValueChanged.AddListener(SetEnemyUI);
    }

    // ==== SETTERS ====
    public void SetVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("Volume", value);
        if (volumeValueText != null)
            volumeValueText.text = Mathf.RoundToInt(value * 100f) + "%";
    }

    public void SetBrightness(float value)
    {
        if (directionalLight != null && baseIntensity >= 0f)
        {
            float minFactor = 0.1f;
            float actualIntensity = Mathf.Lerp(baseIntensity * minFactor, baseIntensity, value);
            directionalLight.intensity = actualIntensity;
        }

        PlayerPrefs.SetFloat("Brightness", value);

        if (brightnessValueText != null)
            brightnessValueText.text = Mathf.RoundToInt(value * 100f) + "%";
    }

    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
        PlayerPrefs.SetInt("Quality", index);
        AnimateDropdown(qualityDropdown);
    }

    public void SetResolution(int index)
    {
        // CORRECCIÓN: Usamos la lista filtrada para que coincida con el Dropdown
        Resolution res = filteredResolutions[index];
        
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        
        // Guardamos los datos para la próxima vez que se abra el juego
        PlayerPrefs.SetInt("ResWidth", res.width);
        PlayerPrefs.SetInt("ResHeight", res.height);
    }

    public void SetFullscreen(bool value)
    {
        Screen.fullScreen = value;
        PlayerPrefs.SetInt("Fullscreen", value ? 1 : 0);
    }

    private void SetEnemyUI(bool value)
    {
        isEnabledUI = value;
        PlayerPrefs.SetInt("EnemyUI", value ? 1 : 0);
    }

    private void AnimateDropdown(TMP_Dropdown dropdown)
    {
        if (dropdown == null) return;
        RectTransform rt = dropdown.GetComponent<RectTransform>();
        if (rt == null) return;

        rt.DOKill();
        rt.localScale = Vector3.one;
        rt.DOPunchScale(Vector3.one * dropPunchDownScale, dropdownAnimDuration, 5, .5f);
        
        TMP_Text label = dropdown.captionText;
        if (label != null)
        {
            Color original = label.color;
            label.DOKill();
            label.DOColor(Color.white, dropdownAnimDuration * 0.3f)
                .OnComplete(() => label.DOColor(original, dropdownAnimDuration * 0.7f));
        }
    }
}