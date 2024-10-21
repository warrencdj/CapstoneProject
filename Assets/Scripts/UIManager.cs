using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject joystickCanvas; // Assign in Inspector
    public GameObject dialogueCanvas; // Assign in Inspector

    void Start()
    {
        // Ensure the dialogue canvas is not visible at the start
        dialogueCanvas.SetActive(false);
    }

    public void ShowDialogueCanvas()
    {
        // Hide the joystick and show the dialogue canvas
        joystickCanvas.SetActive(false);
        dialogueCanvas.SetActive(true);
    }

    public void HideDialogueCanvas()
    {
        // Show the joystick and hide the dialogue canvas
        joystickCanvas.SetActive(true);
        dialogueCanvas.SetActive(false);
    }
}