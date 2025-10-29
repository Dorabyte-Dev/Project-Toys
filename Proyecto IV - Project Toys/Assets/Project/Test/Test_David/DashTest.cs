using UnityEngine;
using UnityEngine.InputSystem;

public class DashTest : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 dashVector;
    public float dashForce;
    private PlayerInputSystem inputActions;
    private Vector2 inputVector;
    public float speed;
    private void Awake()
    {
        inputActions = new PlayerInputSystem();
        inputActions.Player.Enable();
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        inputActions.Player.Dash.performed += OnDash;
    }
    private void OnDisable()
    {
        inputActions.Player.Dash.performed -= OnDash;
    }



    void Update()
    {
        //Debug.Log(transform.forward);
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            dashVector = transform.forward * dashForce;
            rb.AddForce(new Vector3(dashVector.x, 0, dashVector.z), ForceMode.Impulse);
        }*/
        inputVector = inputActions.Player.Movement.ReadValue<Vector2>();
        inputVector = Vector2.ClampMagnitude(inputVector, 1);
    }

    void FixedUpdate()
    {
        //rb.linearVelocity = new Vector3(inputVector.x * speed, rb.linearVelocity.y, inputVector.y * speed);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        Debug.Log("DASH");
        rb.linearVelocity = Vector3.zero;
        dashVector = transform.forward * dashForce;
        rb.AddForce(new Vector3(dashVector.x, 0, dashVector.z), ForceMode.Impulse);
    }
}
