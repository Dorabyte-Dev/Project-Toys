using UnityEngine;
using UnityEngine.InputSystem;

public class DashTest : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 dashVector;
    public float dashForce;
    private PlayerInputSystem inputActions;
    

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
        Vector2 inputVector = inputActions.Player.Movement.ReadValue<Vector2>();
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        Debug.Log("DASH");
        dashVector = transform.forward * dashForce;
        rb.AddForce(new Vector3(dashVector.x, 0, dashVector.z), ForceMode.Impulse);
    }
}
