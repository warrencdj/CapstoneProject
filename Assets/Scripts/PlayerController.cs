using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // For referencing the joystick

public class PlayerController : MonoBehaviour
{
    // References
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private CharacterController characterController;

    // New joystick reference
    [SerializeField] private Joystick joystick; // Add your joystick prefab here

    // Player settings
    [SerializeField] private float cameraSensitivity;
    [SerializeField] private float moveSpeed;

    // Touch detection for camera control
    private int rightFingerId;
    private float halfScreenWidth;

    // Camera control
    private Vector2 lookInput;
    private float cameraPitch;

    // Start is called before the first frame update
    void Start()
    {
        // Right finger is for camera rotation, initialized as not being tracked
        rightFingerId = -1;

        // Calculate half the screen width
        halfScreenWidth = Screen.width / 2;
    }

    // Update is called once per frame
    void Update()
    {
        // Handle camera input
        GetTouchInput();

        if (rightFingerId != -1)
        {
            // Only look around if the right finger is being tracked
            LookAround();
        }

        // Use joystick input for movement
        Move();
    }

    void GetTouchInput()
    {
        // Iterate through all detected touches
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);

            // Check each touch's phase
            switch (t.phase)
            {
                case TouchPhase.Began:
                    if (t.position.x > halfScreenWidth && rightFingerId == -1)
                    {
                        // Start tracking the right finger for camera movement
                        rightFingerId = t.fingerId;
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (t.fingerId == rightFingerId)
                    {
                        // Stop tracking the right finger
                        rightFingerId = -1;
                    }
                    break;

                case TouchPhase.Moved:
                    if (t.fingerId == rightFingerId)
                    {
                        // Get input for looking around
                        lookInput = t.deltaPosition * cameraSensitivity * Time.deltaTime;
                    }
                    break;

                case TouchPhase.Stationary:
                    if (t.fingerId == rightFingerId)
                    {
                        // Set the look input to zero if the finger is still
                        lookInput = Vector2.zero;
                    }
                    break;
            }
        }
    }

    void LookAround()
    {
        // Vertical (pitch) rotation
        cameraPitch = Mathf.Clamp(cameraPitch - lookInput.y, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(cameraPitch, 0, 0);

        // Horizontal (yaw) rotation
        transform.Rotate(transform.up, lookInput.x);
    }

    void Move()
    {
        // Use the joystick for movement
        Vector2 movementDirection = new Vector2(joystick.Horizontal, joystick.Vertical);

        // Only move if there is input
        if (movementDirection.sqrMagnitude > 0.1f)
        {
            Vector3 move = transform.right * movementDirection.x + transform.forward * movementDirection.y;
            characterController.Move(move * moveSpeed * Time.deltaTime);
        }
    }
}
