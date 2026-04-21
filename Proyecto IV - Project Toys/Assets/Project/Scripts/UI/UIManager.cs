using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField]private Player player;
    [SerializeField] private Image healthBar;
    private float _healthBarFillAmount;
    public float HealthBarFillAmount
    {
        get => _healthBarFillAmount;
        set 
        {
            _healthBarFillAmount = Mathf.Clamp01(value);
            healthBar.fillAmount = _healthBarFillAmount;
        }
    }
    
    [SerializeField] private Image comboBarLeft;
    [SerializeField] private Image comboBarRight;

    [SerializeField] private RawImage curtain;
    private static RawImage curtainInstance;
    [HideInInspector]public bool IsComboBarFull => ComboBarFillAmount >= 1f;
    private float _comboBarFillAmount;
    public float ComboBarFillAmount
    {
        get => _comboBarFillAmount;
        set 
        {
            _comboBarFillAmount = Mathf.Clamp01(value);
            comboBarLeft.fillAmount = _comboBarFillAmount;
            comboBarRight.fillAmount = _comboBarFillAmount;

            if(Mathf.Approximately(_comboBarFillAmount, 1))
            {
                comboBarLeft.color = Color.blue;
                comboBarRight.color = Color.blue;
            }
            else
            {
                comboBarLeft.color = Color.white;
                comboBarRight.color = Color.white;
            }
        }
    }

    [Header("Pause Menu")]
    public GameObject pausePanel;
    public Button optionsbackButton;
    public static bool gameIsPaused;
    [SerializeField] private string mainMenuString = "MainMenu";
    
    [Header("Options Panel")]
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private CanvasGroup optionsPanelGroup;
    [SerializeField] private RectTransform optionsPanelRect;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (player == null)
        {
            player = FindAnyObjectByType<Player>();
        }
        curtainInstance = curtain;
        curtainInstance.material.SetFloat("_MaskScale", 1f);
        pausePanel.SetActive(false);
        optionsPanel.SetActive(false);
        optionsbackButton.onClick.AddListener(CloseOptions);
        
    }

    private void Update()
    {
        HealthBarFillAmount = player.GetCurrentHealth() / player.GetMaxHealth();
        if (player.input.Menu.ESC.WasPressedThisFrame())
        {
            if (gameIsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    private void PauseGame()
    {
        gameIsPaused = true;
        Time.timeScale = 0f;
        pausePanel.SetActive(true);
    }

    public void ResumeGame()
    {
        gameIsPaused = false;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuString);
    }

    public static void CloseCurtain(Action onComplete = null)
    {
        curtainInstance.material.DOFloat(0,"_MaskScale", 1f).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }
    
    public static void OpenCurtain(float delay = 0, Action onComplete = null)
    {
        
        curtainInstance.material.DOFloat(1,"_MaskScale", 1f).SetEase(Ease.InOutQuad).SetDelay(delay).OnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }

    public void OnDisable()
    {
        curtainInstance.material.SetFloat("_MaskScale", 1f);
    }
    
    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
        pausePanel.SetActive(false);

        optionsPanelGroup.alpha = 0f;
        optionsPanelRect.anchoredPosition = new Vector2(0, -50f);

        optionsPanelGroup
            .DOFade(1f, 0.3f)
            .SetUpdate(true);

        optionsPanelRect
            .DOAnchorPos(Vector2.zero, 0.3f)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);

        if (OptionsManager.Instance != null)
            OptionsManager.Instance.Init();
    }

    private void CloseOptions()
    {
        optionsPanelGroup
            .DOFade(0f, 0.2f)
            .SetUpdate(true);

        optionsPanelRect
            .DOAnchorPos(new Vector2(0, -50f), 0.2f)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                optionsPanel.SetActive(false);
                pausePanel.SetActive(true);
            });
    }
}
