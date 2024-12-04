using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerOption : MonoBehaviour
{
    public Sprite answerSprite;   // The sprite to display for the answer
    public bool isCorrect;        // Flag to check if this is the correct answer

    private SpriteRenderer spriteRenderer;
    private Collider answerCollider; // Reference to the collider
    private bool isInteractable = true; // Tracks if the answer can be interacted with

    [SerializeField]
    private float cooldownDuration = 1f; // Cooldown duration (in seconds)

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        answerCollider = GetComponent<Collider>();

        // If SpriteRenderer or Collider is missing, add one
        if (spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        if (answerCollider == null)
        {
            answerCollider = gameObject.AddComponent<BoxCollider>();
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
        // Check if the object touching the answer option is the player and it's interactable
        if (isInteractable && other.CompareTag("Player"))
        {
            // Call the OnAnswerSelected method to check if the answer is correct
            OnAnswerSelected();

            // Start the cooldown to prevent immediate re-interaction
            StartCoroutine(AnswerCooldown());
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

    // Cooldown to make the answer temporarily unselectable
    private IEnumerator AnswerCooldown()
    {
        isInteractable = false; // Disable interaction
        answerCollider.enabled = false; // Disable the collider

        yield return new WaitForSeconds(cooldownDuration); // Wait for the cooldown duration

        isInteractable = true; // Enable interaction
        answerCollider.enabled = true; // Enable the collider
    }
}
