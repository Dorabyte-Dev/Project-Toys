using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using DG.Tweening;

public class MenuManager : MonoBehaviour
{
    /*public BlockButton playBlock;
    public BlockButton optionsBlock;
    public BlockButton exitBlock;*/
    
    [Header("Menu Navigation")]
    [SerializeField] private GameObject[] menuElements; // Array de 3 elementos del menú
    [SerializeField] private float joystickThreshold = 0.5f; // Umbral para detectar movimiento del joystick
    [SerializeField] private float navigationCooldown = 0.3f; // Tiempo mínimo entre navegaciones
    [SerializeField] private float highlightScale = 1.2f; // Escala del elemento seleccionado
    
    private int currentIndex = 0;
    private float lastNavigationTime;
    private bool canNavigate = true;
    private Vector2 joystickInput;

    private void Start()
    {
        if (menuElements.Length > 0)
        {
            HighlightCurrentElement();
        }
    }

    private void Update()
    {
        // Leer input del joystick (stick izquierdo)
        var gamepad = Gamepad.current;
        if (gamepad != null)
        {
            joystickInput = gamepad.leftStick.ReadValue();
            HandleJoystickNavigation();
        }
    }

    private void HandleJoystickNavigation()
    {
        // Solo navegar si ha pasado el tiempo de cooldown
        if (!canNavigate && Time.time - lastNavigationTime < navigationCooldown)
            return;

        // Detectar movimiento vertical del joystick
        if (Mathf.Abs(joystickInput.y) > joystickThreshold)
        {
            if (canNavigate)
            {
                int previousIndex = currentIndex;

                if (joystickInput.y > joystickThreshold) // Arriba
                {
                    currentIndex--;
                    if (currentIndex < 0)
                        currentIndex = menuElements.Length - 1; // Loop al final
                }
                else if (joystickInput.y < -joystickThreshold) // Abajo
                {
                    currentIndex++;
                    if (currentIndex >= menuElements.Length)
                        currentIndex = 0; // Loop al inicio
                }

                if (previousIndex != currentIndex)
                {
                    UnhighlightElement(previousIndex);
                    HighlightCurrentElement();
                    lastNavigationTime = Time.time;
                    canNavigate = false;
                }
            }
        }
        else
        {
            // Resetear cuando el joystick vuelve al centro
            canNavigate = true;
        }
    }

    private void HighlightCurrentElement()
    {
        if (currentIndex >= 0 && currentIndex < menuElements.Length && menuElements[currentIndex] != null)
        {
            menuElements[currentIndex].transform.DOScale(highlightScale, 0.2f).SetEase(Ease.OutBack);
        }
    }

    private void UnhighlightElement(int index)
    {
        if (index >= 0 && index < menuElements.Length && menuElements[index] != null)
        {
            menuElements[index].transform.DOScale(1f, 0.2f).SetEase(Ease.InOutQuad);
        }
    }

    // Método público para saber qué elemento está seleccionado
    public int GetCurrentIndex()
    {
        return currentIndex;
    }

    // Método para seleccionar el elemento actual (llamar cuando se presione el botón A/Cross)
    public void SelectCurrentElement()
    {
        switch (currentIndex)
        {
            case 0:
                PlayGame();
                break;
            case 1:
                OpenOptions();
                break;
            case 2:
                ExitGame();
                break;
        }
    }

    public void PlayGame()
    {
        Fader.Instance.FadeToScene("Blocking_Prototype");
    }

    public void OpenOptions()
    {
        Debug.Log("OPTIONS pulsado");
        // Aqu� abrir�s tu men� de opciones
    }

    public void ExitGame()
    {
        Debug.Log("EXIT pulsado");
        Application.Quit();
    }
}
