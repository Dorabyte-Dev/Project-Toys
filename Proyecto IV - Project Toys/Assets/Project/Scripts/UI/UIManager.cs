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
    [SerializeField] private MultipleSpriteFillBar healthBar;
    private float _healthBarFillAmount;
    public float HealthBarFillAmount
    {
        get => _healthBarFillAmount;
        set 
        {
            _healthBarFillAmount = Mathf.Clamp01(value);
            healthBar.Value = _healthBarFillAmount;
        }
    }
    public float healthBlockAnimationDistance = 20f;
    public float healthBlockAnimationDuration = 0.5f;
    public Ease healthBlockAnimationEase = Ease.InOutElastic;
    
    [FormerlySerializedAs("comboBarLeft")] [SerializeField] private Image comboBar;
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
            comboBar.fillAmount = _comboBarFillAmount;
            //comboBarRight.fillAmount = _comboBarFillAmount;

            /*if(Mathf.Approximately(_comboBarFillAmount, 1))
            {
                comboBarLeft.color = Color.blue;
                //comboBarRight.color = Color.blue;
            }
            else
            {
                comboBarLeft.color = Color.white;
                //comboBarRight.color = Color.white;
            }*/
        }
    }

    [Header("Pause Menu")]
    [SerializeField] private GameObject pausePanel;
    public Button optionsbackButton;
    public Button controlsBackButton;
    public static bool gameIsPaused;
    private CanvasGroup pausePanelGroup;
    private RectTransform pausePanelRect;
    [SerializeField] private string mainMenuString = "MainMenu";
    
    [Header("Options Panel")]
    [SerializeField] private GameObject optionsPanel;
    private CanvasGroup optionsPanelGroup;
    private RectTransform optionsPanelRect;
    [Header("Controls Panel")]
    [SerializeField] private GameObject controlsPanel;
    private CanvasGroup controlsPanelGroup;
    private RectTransform controlsPanelRect;

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
        InitUI();
    }

    private void InitUI()
    {
        curtainInstance = curtain;
        curtainInstance.material.SetFloat("_MaskScale", 1f);
        InitPausePanel();
        InitOptionsPanel();
        InitControlsPanel();
        optionsbackButton.onClick.AddListener(CloseOptions);
        controlsBackButton.onClick.AddListener(CloseControls);
    }

    private void Update()
    {
        HealthBarFillAmount = player.GetCurrentHealth() / player.GetMaxHealth();
        if (player.input.Menu.ESC.WasPressedThisFrame())
        {
            if (gameIsPaused)
            {
                if (optionsPanel.gameObject.activeSelf)
                    CloseOptions();
                else
                    ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
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
    
    #region Pause Canvas
    
    private void InitPausePanel()
    {
        pausePanel.SetActive(false);
        pausePanelGroup = pausePanel.GetComponent<CanvasGroup>();
        pausePanelRect = pausePanel.GetComponent<RectTransform>();
    }
    private void PauseGame()
    {
        gameIsPaused = true;
        Time.timeScale = 0f;
        OpenPausePanel();
    }
    private void OpenPausePanel()
    {
        pausePanel.SetActive(true);
        pausePanelGroup.alpha = 0f;
        pausePanelRect.anchoredPosition = new Vector2(0, -50f);

        pausePanelGroup 
            .DOFade(1f, 0.3f)
            .SetUpdate(true);

        pausePanelRect
            .DOAnchorPos(Vector2.zero, 0.3f)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }
    public void ResumeGame()
    {
        gameIsPaused = false;
        Time.timeScale = 1f;
        ClosePausePanel();
    }
    private void ClosePausePanel()
    {
        pausePanelGroup
            .DOFade(0f, 0.2f)   
            .SetUpdate(true);

        pausePanelRect
            .DOAnchorPos(new Vector2(0, -50f), 0.2f)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                pausePanel.SetActive(false);
            });
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(mainMenuString);
    }
    
    #endregion
    #region Options Canvas
    
    private void InitOptionsPanel()
    {
        optionsPanel.SetActive(false);
        optionsPanelGroup = optionsPanel.GetComponent<CanvasGroup>();
        optionsPanelRect = optionsPanel.GetComponent<RectTransform>();
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
    
    #endregion

    #region Controls Canvas
    private void InitControlsPanel()
    {
        controlsPanel.SetActive(false);
        controlsPanelGroup = optionsPanel.GetComponent<CanvasGroup>();
        controlsPanelRect = optionsPanel.GetComponent<RectTransform>();
    }
    public void OpenControls()
    {
        controlsPanel.SetActive(true);
        pausePanel.SetActive(false);

        controlsPanelGroup.alpha = 0f;
        controlsPanelRect.anchoredPosition = new Vector2(0, -50f);

        controlsPanelGroup
            .DOFade(1f, 0.3f)
            .SetUpdate(true);

        controlsPanelRect
            .DOAnchorPos(Vector2.zero, 0.3f)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }
    
    private void CloseControls()
    {
        controlsPanelGroup
            .DOFade(0f, 0.2f)
            .SetUpdate(true);

        controlsPanelRect
            .DOAnchorPos(new Vector2(0, -50f), 0.2f)
            .SetUpdate(true)
            .OnComplete(() =>
            {
                controlsPanel.SetActive(false);
                pausePanel.SetActive(true);
            });
    }
    #endregion
}