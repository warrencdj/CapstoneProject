using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTransitionManager : MonoBehaviour
{
    // References to the player and spawn point
    public GameObject player;             // Reference to the player GameObject
    public GameObject newSpawnPoint;      // New spawn point (the area where the player will be moved)
    public LetterLesson letterLesson;     // Reference to the LetterLesson script

    // Reference to the obstacle course (or obstacles) that need to be removed
    public GameObject obstacleCourse;    // Reference to the obstacle course to destroy

    // Game state variables
    private bool allLettersCollected = false;  // Flag to track if all letters have been collected

    void Start()
    {
        // Initialize game state
        allLettersCollected = false;
    }

    void Update()
    {
        // Check if all letters have been collected and transition to the new area
        if (letterLesson.collectedLetterCount == letterLesson.letterGameObjects.Length && !allLettersCollected)
        {
            allLettersCollected = true;  // Ensure transition happens only once
            MovePlayerToNewArea();       // Move the player to the new area
        }
    }

    // Move the player to the new area after collecting all letters
    void MovePlayerToNewArea()
    {
        // Ensure the new spawn point is set
        if (newSpawnPoint != null)
        {
            // Move the player to the new spawn point
            player.transform.position = newSpawnPoint.transform.position;
        }

        // Optionally, disable the LetterLesson script if it's no longer needed
        if (letterLesson != null)
        {
            letterLesson.gameObject.SetActive(false);  // Disable letter collection
        }

        // Ensure the player stays active
        if (player != null && !player.activeSelf)
        {
            player.SetActive(true);  // Make sure the player GameObject is active
        }

        // Destroy the obstacle course if it exists
        if (obstacleCourse != null)
        {
            Destroy(obstacleCourse);  // Remove the obstacle course from the scene
        }
    }
}
