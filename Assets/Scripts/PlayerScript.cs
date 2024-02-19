using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerScript : MonoBehaviour
{

    //Parameters
    //public float moveAccel;
    private float currentSpeed;
    private float walkTopSpeed;
    private float sprintSpeed;
    public float groundDrag;
    public float playerHeight;
    public LayerMask Ground;
    public Vector2 mouseSensitivity;


    //References
    public new Camera camera;


    //Data Stache
    [HideInInspector] public new Transform transform;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public new Collider collider;
    [HideInInspector] public Transform camTransform;
    private InputControls input;
    bool onGround;
    Vector2 inputDirection;
    Vector2 viewRotation;
    Vector2 viewInput;
    bool mouseActive = true;










    void Start()
    {
        input = new InputControls();
        input.Enable();
        transform = base.transform;
        rb = GetComponent<Rigidbody>();
        collider= GetComponent<Collider>();
        camTransform = camera.transform;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Set speed amounts for sprint and walk
        walkTopSpeed = 7;
        sprintSpeed = 14;

    }

    void Update()
    {
        //onGround = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, Ground);
        //if (onGround) rb.drag = groundDrag;
        //else rb.drag = 0;

        Sprint();

        OtherControls();

        inputDirection = input.Main.Movement.ReadValue<Vector2>();

        LookControls();
    }
    private void FixedUpdate()
    {
        Vector3 rotatedDirection = transform.forward * inputDirection.y + transform.right * inputDirection.x;

        //rb.AddForce(rotatedDirection.normalized * moveTopSpeed * 10f, ForceMode.Force); //Nick's OG solution

        //rb.velocity += rotatedDirection * moveAccel; //My First Solution
        //if (rb.velocity.magnitude >= moveTopSpeed) rb.velocity = rb.velocity.normalized * moveTopSpeed;

        rb.velocity = rotatedDirection * currentSpeed; //My Second Solution (Little to no Easing.)
    }

    void LookControls()
    {
        viewInput = input.Main.Looking.ReadValue<Vector2>();
        Vector2 viewInputAdj = viewInput * Time.deltaTime * mouseSensitivity;

        //Debug Function for Deactivating mouse movement when not needed.
        if (Input.GetKeyDown(KeyCode.L))
        {
            mouseActive = !mouseActive;
            Cursor.visible = !mouseActive;
            Cursor.lockState = mouseActive ? CursorLockMode.Locked : CursorLockMode.None;
        }

        if (mouseActive)
        {
            viewRotation.x += viewInputAdj.x;
            viewRotation.y -= viewInputAdj.y;
        }

        // Make it so you can't look up/down > 90 degrees
        viewRotation.y = Mathf.Clamp(viewRotation.y, -90f, 90f);

        transform.eulerAngles = Vector3.up * viewRotation.x;
        camTransform.eulerAngles = transform.eulerAngles + Vector3.right * viewRotation.y;
    }

    private void CapSpeed()
    {
        Vector3 baseVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // Cap the velocity
        if (baseVel.magnitude > currentSpeed)
        {
            Vector3 cappedVel = baseVel.normalized * currentSpeed;
            rb.velocity = new Vector3(cappedVel.x, rb.velocity.y, cappedVel.z);
        }
    }

    private void Sprint()
    {   
        // When the left shift key is pressed, update the speed.
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed = sprintSpeed;
        }
        else
        {
            currentSpeed = walkTopSpeed;
        }
    }


    void OtherControls()
    {
        //This is for later once we establish additional functionality.

        jumpPressed = input.Main.Jump.WasPressedThisFrame();
        blinkPressed = input.Main.Blink.WasPressedThisFrame();
        sprintHeld = input.Main.Sprint.IsPressed();
        pausePressed = input.Main.Pause.WasPressedThisFrame();

    }

    //Other Controls.
    bool sprintHeld;
    bool jumpPressed;
    bool blinkPressed;
    bool pausePressed;

}
