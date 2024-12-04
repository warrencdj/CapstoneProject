using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerPlayer : MonoBehaviour
{
    [SerializeField] LayerMask groundLayer;
    [SerializeField] private float runSpeed;
    [SerializeField] private int jumpHeight;

    private float gravity = -25f;
    private CharacterController characterController;
    private Vector3 velocity;
    private bool isGrounded;
    private float horizontalInput;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = 1;

        // Keep the player facing forward
        transform.forward = new Vector3(horizontalInput, 0, Mathf.Abs(horizontalInput) - 1);

        // Check if the player is grounded
        isGrounded = Physics.CheckSphere(transform.position, 0.1f, groundLayer, QueryTriggerInteraction.Ignore);

        // Reset vertical velocity if grounded
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = 0;
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;
        }

        // Move the player forward
        characterController.Move(new Vector3(horizontalInput * runSpeed, 0, 0) * Time.deltaTime);

        // Jump on touch
        if (isGrounded && IsJumpTouchDetected())
        {
            velocity.y += Mathf.Sqrt(jumpHeight * -2 * gravity);
        }

        // Apply vertical movement
        characterController.Move(velocity * Time.deltaTime);
    }

    // Detect touch input for jumping
    private bool IsJumpTouchDetected()
    {
        if (Input.touchCount > 0) // Check if there is any touch on the screen
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began) // Check if the touch just started
            {
                return true;
            }
        }

        return false;
    }
}
