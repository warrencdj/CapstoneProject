using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerQuizStart : MonoBehaviour
{
    public QuizManager quizManager; // Reference to the QuizManager

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            quizManager.StartQuiz(); // Start the quiz when the player steps on the button

            Destroy(gameObject);
        }
    }
}