using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


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
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private Toggle enemyUIToggle;
    public static bool isEnabledUI = true;

    [Header("Post Processing")]
    [SerializeField] private Light directionalLight;
    private float baseIntensity = -1f;
    private Resolution[] resolutions;

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
        InitResolution();
        InitFullscreen();
        InitEnemyUI();
    }


    private void InitVolume()
    {
        if (volumeSlider == null) return;
        volumeSlider.value = PlayerPrefs.GetFloat("Volume", 1f);
        AudioListener.volume = volumeSlider.value;
        volumeSlider.onValueChanged.AddListener(SetVolume);
        SetVolume(volumeSlider.value);
    }

    private void InitBrightness()
    {
        if (brightnessSlider == null || directionalLight == null) return;
        if (baseIntensity < 0f)
            baseIntensity = directionalLight.intensity;

        brightnessSlider.minValue = 0f;
        brightnessSlider.maxValue = 1f;
        brightnessSlider.value = PlayerPrefs.GetFloat("Brightness", 1f);

        brightnessSlider.onValueChanged.RemoveAllListeners();
        brightnessSlider.onValueChanged.AddListener(SetBrightness);
        SetBrightness(brightnessSlider.value);
    }

    private void InitQuality()
    {
        if (qualityDropdown == null) return;
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new List<string>(QualitySettings.names));
        qualityDropdown.value = PlayerPrefs.GetInt("Quality", QualitySettings.GetQualityLevel());
        qualityDropdown.onValueChanged.AddListener(SetQuality);
    }

    private void InitResolution()
    {
        if (resolutionDropdown == null) return;
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            if (!options.Contains(option)) options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
                currentIndex = i;
        }


        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentIndex;
        resolutionDropdown.onValueChanged.AddListener(SetResolution);
    }

    private void InitFullscreen()
    {
        if (fullscreenToggle == null) return;
        fullscreenToggle.isOn = Screen.fullScreen;
        fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
    }
    private void InitEnemyUI()
    {
        if (enemyUIToggle == null) return;
        isEnabledUI = true;
        enemyUIToggle.isOn = isEnabledUI;
        enemyUIToggle.onValueChanged.AddListener(SetEnemyUI);
        //SetEnemyUI(isEnabledUI);
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
    }

    public void SetResolution(int index)
    {
        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
    }

    public void SetFullscreen(bool value)
    {
        Screen.fullScreen = value;
    }

    private void SetEnemyUI(bool value)
    {
        isEnabledUI = value;
        PlayerPrefs.SetInt("EnemyUI", value ? 1 : 0);
    }
}