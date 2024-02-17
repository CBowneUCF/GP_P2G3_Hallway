using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float xSens;
    public float ySens;

    public Transform ori;

    float yRotate;
    float xRotate;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Mouse input
        float xMouse = Input.GetAxisRaw("Mouse X") * Time.deltaTime * xSens;
        float yMouse = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * ySens;

        yRotate += xMouse;
        xRotate -= yMouse; 

        // Make it so you can't look up/down > 90 degrees
        xRotate = Mathf.Clamp(xRotate, -90f, 90f);

        // Rotate camera
        transform.rotation = Quaternion.Euler(xRotate, yRotate, 0);
        ori.rotation = Quaternion.Euler(0, yRotate, 0);
    }
}
