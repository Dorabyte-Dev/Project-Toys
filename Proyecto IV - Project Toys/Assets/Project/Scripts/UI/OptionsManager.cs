using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using TMPro;

public class OptionsManager : MonoBehaviour
{
    public static OptionsManager Instance;

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

    [Header("Post Processing")]
    [SerializeField] private Volume globalVolume;
    private Exposure exposure;
    private Resolution[] resolutions;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Init()
    {
        // Exposure
        if (globalVolume != null)
            globalVolume.profile.TryGet(out exposure);

        InitVolume();
        InitBrightness();
        InitQuality();
        InitResolution();
        InitFullscreen();
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
        if (brightnessSlider == null || exposure == null) return;
        brightnessSlider.minValue = -3f;
        brightnessSlider.maxValue = 3f;
        brightnessSlider.value = PlayerPrefs.GetFloat("Brightness", 0f);
        exposure.fixedExposure.value = brightnessSlider.value;
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

    // ===== SETTERS =====
    public void SetVolume(float value)
    {
        AudioListener.volume = value;
        PlayerPrefs.SetFloat("Volume", value);
        if (volumeValueText != null)
            volumeValueText.text = Mathf.RoundToInt(value * 100f) + "%";
    }

    public void SetBrightness(float value)
    {
        if (exposure != null)
        {
            exposure.mode.value = ExposureMode.Fixed;
            exposure.fixedExposure.value = value;
        }
        PlayerPrefs.SetFloat("Brightness", value);
        if (brightnessValueText != null)
            brightnessValueText.text = Mathf.RoundToInt((value + 3f) / 6f * 100f) + "%";
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
}