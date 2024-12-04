using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LetterLesson : MonoBehaviour
{
    // References
    public GameObject[] letterGameObjects;  // Letters placed on the map
    public GameObject[] lessonPanels;      // Corresponding lesson panels for each letter
    public GameObject playerUI;            // Player UI GameObject
    public TextMeshProUGUI letterCounterText;  // TextMeshPro for showing collected letter count

    // Player movement and state
    private CharacterController characterController;
    private bool canMove = true;
    public float moveSpeed = 5f;
    public int collectedLetterCount = 0;  // Track letters collected

    void Start()
    {
        // Initialize components and UI state
        InitializeReferences();
        InitializeUI();
    }

    void OnTriggerEnter(Collider other)
    {
        // Trigger letter collection when colliding with a letter
        if (other.CompareTag("Letter"))
        {
            HandleLetterCollection(other.gameObject);
        }
    }

    void Update()
    {
        // Handle player movement
        HandlePlayerMovement();
    }

    // Initialize and validate critical references
    void InitializeReferences()
    {
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
            Debug.LogError("CharacterController not found on the player GameObject!");
    }

    // Set initial UI states
    void InitializeUI()
    {
        UpdateLetterCounter();
        if (playerUI != null) playerUI.SetActive(true);
    }

    // Handle collection of a letter
    void HandleLetterCollection(GameObject collectedLetter)
    {
        StopPlayerMovement();

        collectedLetterCount++;
        UpdateLetterCounter();

        ActivateLessonPanelForLetter(collectedLetter);

        collectedLetter.SetActive(false);
    }

    // Activate the lesson panel for the collected letter
    void ActivateLessonPanelForLetter(GameObject collectedLetter)
    {
        for (int i = 0; i < letterGameObjects.Length; i++)
        {
            if (letterGameObjects[i] == collectedLetter)
            {
                if (lessonPanels[i] != null)
                {
                    lessonPanels[i].SetActive(true);
                    Time.timeScale = 0;  // Pause the game
                    if (playerUI != null) playerUI.SetActive(false);  // Hide Player UI
                }
                else
                {
                    Debug.LogError($"Lesson panel is not assigned for letter: {collectedLetter.name}");
                }
                return;
            }
        }
        Debug.LogError($"No matching lesson panel found for letter: {collectedLetter.name}");
    }

    // Close a lesson panel and resume the game
    public void ClosePanel(int index)
    {
        if (lessonPanels[index] != null)
        {
            lessonPanels[index].SetActive(false);  // Hide the panel
            Time.timeScale = 1;  // Resume the game
            if (playerUI != null) playerUI.SetActive(true);  // Show Player UI
            canMove = true;  // Allow player movement
        }
        else
        {
            Debug.LogError($"Lesson panel is not assigned for index: {index}");
        }
    }

    // Update the letter counter display
    void UpdateLetterCounter()
    {
        if (letterCounterText != null)
        {
            letterCounterText.text = $"Letters: {collectedLetterCount}/{letterGameObjects.Length}";
        }
    }

    // Stop player movement
    void StopPlayerMovement()
    {
        canMove = false;
    }

    // Handle player movement if allowed
    void HandlePlayerMovement()
    {
        if (canMove)
        {
            float moveDirectionY = 0;
            Vector3 move = new Vector3(Input.GetAxis("Horizontal"), moveDirectionY, Input.GetAxis("Vertical"));
            characterController.Move(move * moveSpeed * Time.deltaTime);
        }
        else
        {
            characterController.Move(Vector3.zero);  // Stop movement
        }
    }
}
