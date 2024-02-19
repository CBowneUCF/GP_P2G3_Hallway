using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerScript : MonoBehaviour
{

    //Parameters
    //public float moveAccel;
    public float moveTopSpeed;
    public float groundDrag;
    public float playerHeight;
    public LayerMask groundMask;
    public LayerMask interactableMask;
    public Vector2 mouseSensitivity;
    public Vector3 interactSize;


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

    }

    void Update()
    {
        //onGround = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, Ground);
        //if (onGround) rb.drag = groundDrag;
        //else rb.drag = 0;

        OtherControls();

        inputDirection = input.Main.Movement.ReadValue<Vector2>();

        LookControls();

        if (interactPressed) InteractAction();
    }
    private void FixedUpdate()
    {
        Vector3 rotatedDirection = transform.forward * inputDirection.y + transform.right * inputDirection.x;

        //rb.AddForce(rotatedDirection.normalized * moveTopSpeed * 10f, ForceMode.Force); //Nick's OG solution

        //rb.velocity += rotatedDirection * moveAccel; //My First Solution
        //if (rb.velocity.magnitude >= moveTopSpeed) rb.velocity = rb.velocity.normalized * moveTopSpeed;

        rb.velocity = rotatedDirection * moveTopSpeed; //My Second Solution (Little to no Easing.)
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
        if (baseVel.magnitude > moveTopSpeed)
        {
            Vector3 cappedVel = baseVel.normalized * moveTopSpeed;
            rb.velocity = new Vector3(cappedVel.x, rb.velocity.y, cappedVel.z);
        }
    }


    void InteractAction()
    {
        Collider[] results = Physics.OverlapBox(camTransform.position + (camTransform.forward * (interactSize.z/2)), interactSize/2, camTransform.rotation, interactableMask);
        results[0].GetComponent<InteractableScript>().Interact();
    }


    void OtherControls()
    {
        //This is for later once we establish additional functionality.

        jumpPressed = input.Main.Jump.WasPressedThisFrame();
        blinkPressed = input.Main.Blink.WasPressedThisFrame();
        sprintHeld = input.Main.Sprint.IsPressed();
        pausePressed = input.Main.Pause.WasPressedThisFrame();
        interactPressed = input.Main.Interact.WasPressedThisFrame();

    }

    //Other Controls.
    bool sprintHeld;
    bool jumpPressed;
    bool blinkPressed;
    bool pausePressed;
    bool interactPressed;

}
