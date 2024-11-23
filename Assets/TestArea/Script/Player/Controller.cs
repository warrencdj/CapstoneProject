using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Controller : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpHeight = 2f;
    [SerializeField] float gravity = -9.81f;

    public LayerMask groundLayer;
    public Transform groundCheck;
    public Animator anim; // Animator reference

    [SerializeField] float groundCheckRadius = 0.4f;

    private CharacterController characterController;
    private Vector3 velocity;
    private bool isGrounded;
    private float xInput;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        // Check if grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Reset vertical velocity when grounded
        }

        // Apply gravity
        velocity.y += gravity * Time.deltaTime;

        // Move Character
        Vector3 move = new Vector3(xInput, 0f, 0f); // Constrain movement to X-axis
        characterController.Move(move * moveSpeed * Time.deltaTime + new Vector3(0f, velocity.y, 0f) * Time.deltaTime);

        // Rotate Player
        if (xInput != 0)
        {
            RotatePlayer();
        }

        // Update Animator parameters
        anim.SetFloat("InputX", Mathf.Abs(xInput)); // Horizontal movement
        anim.SetBool("isGrounded", isGrounded); // Grounded status
        anim.SetFloat("Blend", Mathf.Abs(xInput)); // Speed for blend tree
    }

    public void HorizontalInput(float value)
    {
        xInput = value; // Set the horizontal input value
    }

    public void JumpInput()
    {
        if (isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // Calculate jump velocity
        }
    }

    private void RotatePlayer()
    {
        if (xInput == 0)
        {
            // Smoothly rotate to face the camera when idle
            Quaternion neutralRotation = Quaternion.Euler(0f, 0f, 0f); // Facing the camera
            transform.rotation = Quaternion.Lerp(transform.rotation, neutralRotation, Time.deltaTime * 10f);
        }
        else
        {
            // Determine the correct Y-rotation based on movement direction
            float targetYRotation = xInput > 0 ? 90f : -90f; // Right = 90°, Left = -90°

            // Ensure the character always rotates "through the front"
            if (transform.rotation.eulerAngles.y > 180f && targetYRotation < 0)
            {
                targetYRotation += 360f; // Adjust to rotate through the front-facing direction
            }
            else if (transform.rotation.eulerAngles.y < 180f && targetYRotation > 0)
            {
                targetYRotation -= 360f; // Adjust to rotate through the front-facing direction
            }

            // Smoothly rotate the character to face the target direction
            Quaternion targetRotation = Quaternion.Euler(0f, targetYRotation, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

}
