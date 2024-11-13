using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public float interactionRange = 3f;  // The range within which the player can interact with the NPC
    private Transform playerTransform;   // Reference to the player's position

    private void Start()
    {
        // Find the player's transform in the scene (assuming the player has a "Player" tag)
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        // Ensure playerTransform is assigned
        if (playerTransform != null)
        {
            // Calculate the distance between the NPC and the player
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            // If the player is within interaction range, listen for the player's input
            if (distanceToPlayer <= interactionRange)
            {
                // If the player taps (on mobile) or clicks (on desktop), interact with the NPC
                if (Input.GetMouseButtonDown(0)) // Left click or tap on mobile
                {
                    OnNPCInteract();
                }
            }
        }
    }

    private void OnNPCInteract()
    {
        // Debug message for interaction with the NPC (can be replaced with a sound effect or animation)
        Debug.Log("Player interacted with the NPC!");

        // Find the UI GameObject by tag (e.g., "UI") and activate it
        GameObject uiCanvas = GameObject.FindGameObjectWithTag("UI");

        if (uiCanvas != null)
        {
            uiCanvas.SetActive(true);  // Activate the UI
        }
        else
        {
            Debug.LogWarning("UI GameObject with 'UI' tag not found!");
        }
    }
}