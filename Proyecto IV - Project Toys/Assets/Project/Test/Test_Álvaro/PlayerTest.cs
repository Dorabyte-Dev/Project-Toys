using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTest : MonoBehaviour
{
    public float speed;
    Rigidbody rb;
    Vector2 moveInput;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame

    private void Update()
    {
        // Lee el movimiento desde el nuevo Input System directamente
        moveInput = Vector2.zero;

        // WASD o Flechas
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                moveInput.y += 1;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                moveInput.y -= 1;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                moveInput.x -= 1;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                moveInput.x += 1;
        }

        // Soporte para gamepad (si hay uno conectado)
        if (Gamepad.current != null)
        {
            moveInput += Gamepad.current.leftStick.ReadValue();
        }

        moveInput = Vector2.ClampMagnitude(moveInput, 1f); // evita moverse más rápido en diagonal
    }
    void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(moveInput.x * speed, rb.linearVelocity.y, moveInput.y * speed);
    }
}
