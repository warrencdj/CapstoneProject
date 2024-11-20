using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public GameObject playerUI;          // Reference to the player UI GameObject
    private static int currentLessonIndex = 0; // Tracks the current lesson index
    public GameObject[] lessonPanels;    // Array of lesson panels to show in order
    public static int totalLessons;      // Total number of lessons (set from the LessonManager)

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
                    HandleNPCInteraction(hit.collider.gameObject);
                }
            }
        }
    }

    private void HandleNPCInteraction(GameObject npc)
    {
        if (currentLessonIndex < lessonPanels.Length)
        {
            // Show the current lesson panel and disable the player UI
            if (lessonPanels[currentLessonIndex] != null && playerUI != null)
            {
                lessonPanels[currentLessonIndex].SetActive(true);  // Show the lesson panel
                playerUI.SetActive(false);                        // Disable the player UI
                PauseGame();
            }

            // Increment the lesson index for the next interaction
            currentLessonIndex++;

            // Destroy the NPC after interaction
            Destroy(npc);
        }
        else
        {
            Debug.Log("All lessons completed. Ready to start the game!");
        }
    }

    public void CloseLessonPanel()
    {
        if (currentLessonIndex > 0 && currentLessonIndex <= lessonPanels.Length)
        {
            // Close the previous lesson panel and re-enable the player UI
            if (lessonPanels[currentLessonIndex - 1] != null && playerUI != null)
            {
                lessonPanels[currentLessonIndex - 1].SetActive(false); // Hide the lesson panel
                playerUI.SetActive(true);                            // Enable the player UI
                ResumeGame();
            }
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0; // Pause the game
    }

    private void ResumeGame()
    {
        Time.timeScale = 1; // Resume the game
    }
}
