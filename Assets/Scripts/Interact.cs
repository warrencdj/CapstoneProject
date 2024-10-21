using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interact : MonoBehaviour
{
    // Drag the prefab or object you want to interact with into this field
    public GameObject interactableObject;

    // Reference to the main camera (specifically named "Camera")
    private Camera mainCamera;

    // Maximum distance for interaction
    public float interactionDistance = 5f;

    // Reference to the UI Manager
    private UIManager uiManager;

    void Start()
    {
        // Find the camera by name ("Camera")
        mainCamera = GameObject.Find("Camera").GetComponent<Camera>();

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera (named 'Camera') not found!");
        }

        if (interactableObject == null)
        {
            Debug.LogError("interactableObject is not assigned in the Inspector!");
        }

        // Get the UIManager component
        uiManager = FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("UIManager not found in the scene!");
        }
    }

    void Update()
    {
        // Check for touches
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.GetTouch(0).position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Check if the object hit is the interactableObject
                if (hit.transform.gameObject == interactableObject)
                {
                    // Check distance between the player and the interactable object
                    float distance = Vector3.Distance(mainCamera.transform.position, interactableObject.transform.position);

                    if (distance <= interactionDistance)
                    {
                        Debug.Log(interactableObject.name + " was tapped within range!");

                        // Try to get the DialogueTrigger component from the NPC
                        DialogueTrigger dialogueTrigger = hit.transform.GetComponent<DialogueTrigger>();
                        if (dialogueTrigger != null)
                        {
                            dialogueTrigger.TriggerDialogue();
                            uiManager.ShowDialogueCanvas(); // Hide joystick and show dialogue
                        }
                    }
                    else
                    {
                        Debug.Log(interactableObject.name + " is too far to interact with.");
                    }
                }
            }
        }
    }
}
