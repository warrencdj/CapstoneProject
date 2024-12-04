using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerOption : MonoBehaviour
{
    public Sprite answerSprite;   // The sprite to display for the answer
    public bool isCorrect;        // Flag to check if this is the correct answer

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        // If SpriteRenderer is missing, add one
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
    }

    // Method to update the answer sprite and set the correct status
    public void UpdateAnswer(Sprite sprite, bool correctStatus)
    {
        spriteRenderer.sprite = sprite;
        isCorrect = correctStatus;
    }

    // Method to handle when the player touches the answer option
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object touching the answer option is the player
        if (other.CompareTag("Player"))
        {
            // Call the OnAnswerSelected method to check if the answer is correct
            OnAnswerSelected();
        }
    }

    // Method to check if this answer is selected by the player
    public void OnAnswerSelected()
    {
        if (isCorrect)
        {
            Debug.Log("Correct answer!");
            // Notify QuizManager to proceed to the next question
            FindObjectOfType<QuizManager>().AdvanceQuestion(true);
        }
        else
        {
            Debug.Log("Wrong answer. Try again.");
            // Notify QuizManager to retry the same question
            FindObjectOfType<QuizManager>().AdvanceQuestion(false);
        }
    }
}