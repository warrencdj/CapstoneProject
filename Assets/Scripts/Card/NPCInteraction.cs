using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public GameObject lessonPanel; // Reference to the LessonPanel
    public GameObject playerUI;   // Reference to the player UI GameObject
    private static bool hasPlayerInteracted = false; // Tracks if the player has interacted before

    private void Update()
    {
        // Check for player interaction with the NPC (tap or click)
        if (Input.GetMouseButtonDown(0)) // Left mouse button or screen tap
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("NPC")) // Check if the player tapped on an NPC
                {
                    HandleNPCInteraction();
                }
            }
        }
    }

    private void HandleNPCInteraction()
    {
        // If this is the first interaction, show the lesson panel and disable the player UI
        if (!hasPlayerInteracted)
        {
            if (lessonPanel != null && playerUI != null)
            {
                lessonPanel.SetActive(true);        // Show the lesson panel
                playerUI.SetActive(false);          // Disable the player UI
                PauseGame();
            }
            else
            {
                Debug.LogWarning("LessonPanel or PlayerUI is not assigned in NPCInteraction!");
            }

            hasPlayerInteracted = true; // Mark that the player has interacted
        }
        else
        {
            // Debug message when the player tries to interact again
            Debug.Log("Lesson already shown. You can continue the game!");
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0; // Pause the game
    }

    public void CloseLessonPanel()
    {
        if (lessonPanel != null && playerUI != null)
        {
            lessonPanel.SetActive(false); // Hide the lesson panel
            playerUI.SetActive(true);  // Enable the player UI
            ResumeGame();
        }
    }

    private void ResumeGame()
    {
        Time.timeScale = 1; // Resume the game
    }
}