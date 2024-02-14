using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMouvement : MonoBehaviour
{
    public float mouseSensitivity = 100f;

    float xRotation = 0f;
    float yRotation = 0f;

    public float topAngle = -90f;
    public float bottomAngle = 90f;

    // Start is called before the first frame update
    void Start()
    {
        // Locking the cursor to the center of the screen and hiding it
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Rotating around the X axis (up and down)
        xRotation -= mouseY;

        // Clamping the rotation so the player can't look behind them
        xRotation = Mathf.Clamp(xRotation, topAngle, bottomAngle);

        // Rotating around the Y axis (left and right)
        yRotation += mouseX;

        // Applying the rotation to the camera
        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
