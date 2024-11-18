using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameIntroduction : MonoBehaviour
{
    public GameObject introPanel; // Reference to the intro panel (parent of the pages)

    private void Start()
    {
        ShowIntroduction(); // Show the introduction on game start
    }

    public void ShowIntroduction()
    {
        if (introPanel != null)
        {
            introPanel.SetActive(true); // Show the intro panel
            Time.timeScale = 0f;        // Pause the game
        }
        else
        {
            Debug.LogWarning("Intro panel is not assigned!");
        }
    }

    public void CloseIntroduction()
    {
        if (introPanel != null)
        {
            introPanel.SetActive(false); // Hide the intro panel
            Time.timeScale = 1f;         // Resume the game
        }
    }
}