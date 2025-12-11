using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{
    
    public PlayerInputSystem input { get; private set; }
    public Vector2 moveInput { get; private set; }
    
    [Header("Menu Navigation")]
    [SerializeField] private BlockButton[] menuElements; // Array de 3 elementos del menú
    [SerializeField] private float joystickThreshold = 0.5f; // Umbral para detectar movimiento del joystick
    [SerializeField] private float navigationCooldown = 0.3f; // Tiempo mínimo entre navegaciones
    [SerializeField] private Material highlightMaterial;
    
    
    private int currentIndex = 0;
    private float lastNavigationTime;
    private bool canNavigate = true;

    private void Awake()
    {
        input = new PlayerInputSystem();
    }
    private void Start()
    {
        if (menuElements.Length > 0)
        {
            UpdateMenuVisuals();
        }
    }
    
    private void OnEnable()
    {
        input.Enable();
        input.Menu.Navigate.performed += OnNavigate;
        input.Menu.Navigate.canceled += OnNavigate;
        input.Menu.Select.performed += OnSelect;

    }
    
    private void OnDisable()
    {
        // Quitar listeners para evitar referencias tras desactivar el objeto
        if (input != null)
        {
            input.Menu.Navigate.performed -= OnNavigate;
            input.Menu.Navigate.canceled -= OnNavigate;
            input.Menu.Select.performed -= OnSelect;
            input.Disable();
        }
    }
    private void Update()
        {
            // Si quedó consumido, reactivar cuando pase el cooldown
            if (!canNavigate && Time.time - lastNavigationTime >= navigationCooldown)
            {
                canNavigate = true;
            }
        }
    
    // Método para conectar con el Input System - llámalo desde tu PlayerInput o InputAction
    public void OnNavigate(InputAction.CallbackContext context)
    {
        if (!canNavigate || Time.time - lastNavigationTime < navigationCooldown)
            return;

        Vector2 navInput = context.ReadValue<Vector2>();
        
        if (Mathf.Abs(navInput.x) > joystickThreshold)
        {
            if (navInput.x > joystickThreshold) // Arriba
            {
                currentIndex--;
                if (currentIndex < 0)
                    currentIndex = menuElements.Length - 1;
            }
            else if (navInput.x < -joystickThreshold) // Abajo
            {
                currentIndex++;
                if (currentIndex >= menuElements.Length)
                    currentIndex = 0;
            }
            Debug.LogWarning("Current Button is: " + menuElements[currentIndex].name);
            UpdateMenuVisuals();
            lastNavigationTime = Time.time;
            canNavigate = false;
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            canNavigate = true;
        }
    }

    // Método para conectar con el Input System - llámalo cuando se presione el botón de selección
    public void OnSelect(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SelectCurrentElement();
        }
    }

    private void UpdateMenuVisuals()
    {
        // Aquí puedes agregar tu lógica de visualización (colores, escalas, etc.)
        for (int i = 0; i < menuElements.Length; i++)
        {
            if (menuElements[i] != null)
            {
                if (i == currentIndex)
                {
                    List<Material> materials = new  List<Material>();
                    menuElements[i].renderer.GetMaterials(materials);
                    materials.Add(highlightMaterial);
                    menuElements[i].renderer.SetMaterials(materials);
                }
                else{
                    List<Material> materials = new  List<Material>();
                    menuElements[i].renderer.GetMaterials(materials);
                    if (materials.Count > 1)
                    {
                        materials.RemoveAt(1);
                        menuElements[i].renderer.SetMaterials(materials);
                    }
                }
                // Por ahora solo activa/desactiva, puedes personalizar esto
                //menuElements[i].SetActive(i == currentIndex);
            }
        }
    }

    private void SelectCurrentElement()
    {
        if (menuElements[currentIndex] != null)
        {
            menuElements[currentIndex].BlockHit();
        }
    }

    public void PlayGame()
    {
        Fader.Instance.FadeToScene("Blocking_Prototype");
    }

    public void OpenOptions()
    {
        // Aquí abrirás tu menú de opciones
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
