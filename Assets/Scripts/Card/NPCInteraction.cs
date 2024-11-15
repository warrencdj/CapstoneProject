using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public float interactionRange = 3f;  // Range within which the player can interact with the NPC
    private Transform playerTransform;   // Reference to the player's position
    private Camera mainCamera;           // Reference to the main camera for detecting raycast hits

    // References to the UI elements
    public GameObject testCanvas;        // The canvas to show when interacting with NPC
    public GameObject joystickCanvas;   // The joystick UI to deactivate during interaction

    private bool isInteracting = false; // To track whether the player is interacting with an NPC

    private void Start()
    {
        // Find the player transform and camera if not assigned
        playerTransform = transform;
        mainCamera = Camera.main;   // Assuming the main camera is tagged correctly

        // Ensure the test canvas is initially inactive
        if (testCanvas != null)
        {
            testCanvas.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Test Canvas reference is missing!");
        }

        // Ensure the joystick canvas is active initially
        if (joystickCanvas != null)
        {
            joystickCanvas.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Joystick Canvas reference is missing!");
        }
    }

    private void Update()
    {
        // Handle interaction when the player clicks or taps
        if (Input.GetMouseButtonDown(0)) // Detect mouse click or touch input
        {
            TryInteractWithNPC();
        }
    }

    private void TryInteractWithNPC()
    {
        // Check if the main camera is assigned before proceeding
        if (mainCamera == null)
        {
            return;  // If the main camera is missing, skip the interaction logic
        }

        // Cast a ray from the camera to where the player clicked or tapped
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Check if the ray hits an NPC within the interaction range
        if (Physics.Raycast(ray, out hit, interactionRange))
        {
            // Check if the hit object is an NPC
            if (hit.collider != null && hit.collider.CompareTag("NPC"))
            {
                // Debug message for interaction with the NPC
                Debug.Log("Player interacted with NPC!");

                // Log the NPC's name for debugging
                Debug.Log("Interacted with: " + hit.collider.name);

                // Activate the test canvas and deactivate the joystick UI
                if (testCanvas != null)
                {
                    testCanvas.SetActive(true);
                }
                if (joystickCanvas != null)
                {
                    joystickCanvas.SetActive(false);
                }

                // Pause the game by setting the time scale to 0
                Time.timeScale = 0f;

                // Mark as interacting
                isInteracting = true;
            }
        }
    }

    public void EndInteraction()
    {
        // This method can be called when the interaction is finished (e.g., the player closes the canvas)

        // Deactivate the test canvas and reactivate the joystick UI
        if (testCanvas != null)
        {
            testCanvas.SetActive(false);
        }
        if (joystickCanvas != null)
        {
            joystickCanvas.SetActive(true);
        }

        // Resume the game by setting the time scale back to 1
        Time.timeScale = 1f;

        // Mark as not interacting
        isInteracting = false;
    }
}
