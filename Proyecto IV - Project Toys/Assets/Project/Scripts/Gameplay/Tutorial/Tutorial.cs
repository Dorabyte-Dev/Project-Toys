using System;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    #region Static Events
    public static event Action OnTutorialStarted;
    public static event Action OnTutorialEnded;
    #endregion
    
    #region PlayerReference
    private PlayerInputSystem playerInput;
    private Player player;
    #endregion
    
    private List<TutorialPhase> tutorialsPhases = new List<TutorialPhase>();
    private int currentPhaseIndex = 0;
    private bool tutorialCompleted = false;
    
    #region Tutorial Phase Variables
    private int lightAttackCount = 0;
    private int heavyAttackCount = 0;
    [Header("Tutorial Variables & References")]
    [SerializeField] private int requiredLightAttacks = 3;
    [SerializeField] private GameObject pressLightAttackButton;
    [SerializeField] private GameObject lightHorizontalLayoutParent;
    private RectTransform lightAttackCanvasParent;
    [SerializeField] private int requiredHeavyAttacks = 3;
    [SerializeField] private GameObject pressHeavyAttackButton;
    [SerializeField] private GameObject heavyHorizontalLayoutParent;
    private RectTransform heavyAttackCanvasParent;
    private Image[] attackLightImages;
    private Image[] attackHeavyImages;
    
    [HideInInspector] public bool canContinueToNextPhase = false; // La idea de esta variable es que se pueda modificar a través de terminar dialogos o cinemáticas para que continue el tutorial.
    #endregion
    
    void Start()
    {
        GetPlayerInput();
        SetTutorialPhases();
        SetTutorialUI();
    }

    private void OnEnable()
    {
        BeginTutorial();
    }

    void Update()
    {
        CheckTutorialPhase();
    }
    
    private void GetPlayerInput()
    {
        player = PlayerReference.playerTransform.GetComponent<Player>();
        if (player == null)
        {
            Debug.LogError("[Tutorial] Could not find Player component on playerTransform.");
            return;
        }
        playerInput = player.input;
    }

    private void SetTutorialPhases()
    {
        canContinueToNextPhase = true; // De momento siempre emprieza en true ya que no está puesto el dialogo del principio.
        
        tutorialsPhases.Add(new TutorialPhase((() =>
        {
            if(!canContinueToNextPhase) return false;
           lightAttackCanvasParent.gameObject.SetActive(true);
           lightAttackCanvasParent.DOAnchorPosX(-375, 0.5f).SetEase(Ease.OutBack);
            canContinueToNextPhase = false;
            return true;
        })));
        
        tutorialsPhases.Add(AttackLightThreeTimes());
        
        tutorialsPhases.Add(new TutorialPhase((() =>
        {
            lightAttackCanvasParent.DOAnchorPosX(400, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                lightAttackCanvasParent.gameObject.SetActive(false);
                canContinueToNextPhase = true;
            });
            return true;
        })));
        
        tutorialsPhases.Add(new TutorialPhase((() =>
        {
            if(!canContinueToNextPhase) return false;
            heavyAttackCanvasParent.gameObject.SetActive(true);
            heavyAttackCanvasParent.DOAnchorPosX(-375, 0.5f).SetEase(Ease.OutBack);
            canContinueToNextPhase = false;
            return true;
        })));
        
        tutorialsPhases.Add(AttackHeavyThreeTimes());
        
        tutorialsPhases.Add(new TutorialPhase((() =>
        {
            heavyAttackCanvasParent.DOAnchorPosX(400, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
            {
                heavyAttackCanvasParent.gameObject.SetActive(false);
            });
            return true;
        })));
    }
    
    private void SetTutorialUI()
    {
        attackLightImages = new Image[requiredLightAttacks];
        attackHeavyImages = new Image[requiredHeavyAttacks];
        
        for (int i = 0; i < requiredLightAttacks; i++)
        {
            GameObject lightAttackButtonInstance = Instantiate(pressLightAttackButton, lightHorizontalLayoutParent.transform);
            attackLightImages[i] = lightAttackButtonInstance.GetComponent<Image>();
        }
        
        for (int i = 0; i < requiredHeavyAttacks; i++)
        {
            GameObject heavyAttackButtonInstance = Instantiate(pressHeavyAttackButton, heavyHorizontalLayoutParent.transform);
            attackHeavyImages[i] = heavyAttackButtonInstance.GetComponent<Image>();
        }
        
        lightAttackCanvasParent = lightHorizontalLayoutParent.transform.parent.GetComponent<RectTransform>();
        heavyAttackCanvasParent = heavyHorizontalLayoutParent.transform.parent.GetComponent<RectTransform>();
    }

    private void CheckTutorialPhase()
    {
        if(tutorialCompleted) return;
        
        if(currentPhaseIndex < tutorialsPhases.Count)
        {
            TutorialPhase currentPhase = tutorialsPhases[currentPhaseIndex];

            if (currentPhase.status == TutorialPhase.Status.NotStarted)
            {
                currentPhase.StartPhase();
            }
            
            currentPhase.UpdatePhase();

            if (currentPhase.isCompleted)
            {
                currentPhaseIndex++;
            }
        }
        else
        {
            tutorialCompleted = true;
            EndTutorial();
        }
    }
    

    private void BeginTutorial()
    {
        OnTutorialStarted?.Invoke();
        Debug.Log("[Tutorial] Tutorial Started!");
    }

    private void EndTutorial()
    {
        OnTutorialEnded?.Invoke();
        Debug.Log("[Tutorial] Tutorial Completed!");
    }
    
    #region Input Callbacks

    private void OnLightAttack(InputAction.CallbackContext context)
    {
        lightAttackCount++;
        attackLightImages[lightAttackCount - 1].color = Color.green; // Cambia el color del botón para indicar que se ha presionado
    }
    
    private void OnHeavyAttack(InputAction.CallbackContext context)
    {
        heavyAttackCount++;
        attackHeavyImages[heavyAttackCount - 1].color = Color.green; // Cambia el color del botón para indicar que se ha presionado
    }
    #endregion
    
    #region Tutorial Phases

    private TutorialPhase AttackLightThreeTimes()
    {
        return new TutorialPhase((() =>
        {
            bool result;
            playerInput.Player.LightAttack.performed += OnLightAttack;
            if (lightAttackCount >= 3)
            {
                result = true;
                playerInput.Player.LightAttack.performed -= OnLightAttack;
                // Funciones que indican que se ha completado la fase 
                // Siguiente dialogo o lo que sea
            }
            else
            {
                result = false;
            }
            //Debug.Log("[Tutorial]Light Attack Count: " + lightAttackCount);

            return result;
        }));    
    }
    
    private TutorialPhase AttackHeavyThreeTimes()
    {
        return new TutorialPhase((() =>
        {
            bool result;
            playerInput.Player.HeavyAttack.performed += OnHeavyAttack;
            if (heavyAttackCount >= 3)
            {
                result = true;
                playerInput.Player.HeavyAttack.performed -= OnHeavyAttack;
                // Funciones que indican que se ha completado la fase 
                // Siguiente dialogo o lo que sea
            }
            else
            {
                result = false;
            }
            //Debug.Log("[Tutorial]Heavy Attack Count: " + heavyAttackCount);

            return result;
        }));    
    }
    #endregion
}
