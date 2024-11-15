using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public float interactionRange = 3f;  // Range within which the player can interact with the NPC
    private Transform playerTransform;   // Reference to the player's position
    private Camera mainCamera;           // Reference to the main camera for detecting raycast hits

    private void Start()
    {
        // Find the player transform and camera if not assigned
        playerTransform = transform;
        mainCamera = Camera.main;   // Assuming the main camera is tagged correctly

        // Check if the main camera is assigned, and log an error if it's missing
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera is missing or not tagged correctly! Please ensure the camera is tagged as 'MainCamera'.");
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
                // Trigger interaction with the NPC (you can add more NPC-specific logic here if needed)
                Debug.Log("Player interacted with NPC!");

                // Log the NPC's name for debugging
                Debug.Log("Interacted with: " + hit.collider.name);
            }
        }
    }
}
