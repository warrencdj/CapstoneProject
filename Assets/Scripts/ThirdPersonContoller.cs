using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThridPersonController : MonoBehaviour
{
    // References
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private CharacterController characterController;

    // New joystick reference
    [SerializeField] private Joystick joystick;

    // Player settings
    [SerializeField] private float cameraSensitivity;
    [SerializeField] private float moveSpeed;

    // Camera settings for third-person view
    [SerializeField] private float cameraDistance = 5f;
    [SerializeField] private float cameraHeight = 2f;
    [SerializeField] private float cameraCollisionBuffer = 0.2f; // Buffer to prevent clipping
    [SerializeField] private LayerMask collisionLayers; // Layers the camera can collide with

    // View settings
    [SerializeField] private bool isThirdPersonView = true;

    // Touch detection for camera control
    private int rightFingerId;
    private float halfScreenWidth;

    // Camera control
    private Vector2 lookInput;
    private float cameraPitch;
    private float cameraYaw;

    private Vector3 desiredCameraPosition;

    // Start is called before the first frame update
    void Start()
    {
        rightFingerId = -1;
        halfScreenWidth = Screen.width / 2;
    }

    // Update is called once per frame
    void Update()
    {
        GetTouchInput();
        UpdateCameraPosition();
        Move();
    }

    void GetTouchInput()
    {
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);

            switch (t.phase)
            {
                case TouchPhase.Began:
                    if (t.position.x > halfScreenWidth && rightFingerId == -1)
                    {
                        rightFingerId = t.fingerId;
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    if (t.fingerId == rightFingerId)
                    {
                        rightFingerId = -1;
                    }
                    break;

                case TouchPhase.Moved:
                    if (t.fingerId == rightFingerId)
                    {
                        lookInput = t.deltaPosition * cameraSensitivity * Time.deltaTime;
                        cameraYaw += lookInput.x;
                        cameraPitch = Mathf.Clamp(cameraPitch - lookInput.y, -90f, 90f);
                    }
                    break;

                case TouchPhase.Stationary:
                    if (t.fingerId == rightFingerId)
                    {
                        lookInput = Vector2.zero;
                    }
                    break;
            }
        }
    }

    void UpdateCameraPosition()
    {
        if (isThirdPersonView)
        {
            // Third-person view: calculate the desired camera position behind the player
            Vector3 direction = new Vector3(0, cameraHeight, -cameraDistance);
            Quaternion rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0);
            desiredCameraPosition = transform.position + rotation * direction;

            // Check for collisions using a raycast from the player to the desired camera position
            RaycastHit hit;
            Vector3 cameraDirection = desiredCameraPosition - transform.position;

            if (Physics.Raycast(transform.position, cameraDirection.normalized, out hit, cameraDirection.magnitude, collisionLayers))
            {
                // If we hit something, move the camera closer to avoid clipping
                desiredCameraPosition = hit.point + hit.normal * cameraCollisionBuffer;
            }

            // Apply the new camera position
            cameraTransform.position = desiredCameraPosition;

            // Look at the player
            cameraTransform.LookAt(transform.position + Vector3.up * cameraHeight);
        }
        else
        {
            // First-person view positioning
            cameraTransform.position = transform.position + Vector3.up * 1.5f;
            cameraTransform.rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0);
        }
    }

    void Move()
    {
        Vector2 movementDirection = new Vector2(joystick.Horizontal, joystick.Vertical);

        if (movementDirection.sqrMagnitude > 0.1f)
        {
            Vector3 moveDirection;
            if (isThirdPersonView)
            {
                Vector3 forward = cameraTransform.forward;
                forward.y = 0; // Ignore the camera's vertical angle
                forward.Normalize();
                Vector3 right = cameraTransform.right;
                right.y = 0;
                right.Normalize();

                moveDirection = (forward * movementDirection.y + right * movementDirection.x).normalized;
            }
            else
            {
                moveDirection = transform.right * movementDirection.x + transform.forward * movementDirection.y;
            }

            characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
        }
    }

    // Method to toggle the view
    public void ToggleView()
    {
        isThirdPersonView = !isThirdPersonView;
    }
}
