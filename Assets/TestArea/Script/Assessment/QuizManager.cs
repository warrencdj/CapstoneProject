using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuizManager : MonoBehaviour
{
    [TextArea]
    public List<string> questions; // List of questions set in the Inspector

    public TextMeshPro titleText;    // Reference to the Title TextMeshPro
    public TextMeshPro questionText; // Reference to the Question TextMeshPro

    public List<Sprite> allAnswerSprites; // List of all available answer sprites (Korean vowels or letters)

    // References to the answer option colliders (assuming 3 options)
    public AnswerOption answerOption1;
    public AnswerOption answerOption2;
    public AnswerOption answerOption3;

    private int currentQuestionIndex = 0; // Tracks the current question
    private Sprite correctAnswer;         // Holds the correct answer for the current question
    private bool isQuizStarted = false;   // Tracks if the quiz has started

    void Start()
    {
        // Display the initial message
        questionText.text = "Are you ready to play? Step on the green button to start.";
    }

    public void StartQuiz()
    {
        if (questions.Count > 0 && allAnswerSprites.Count >= questions.Count)
        {
            ShuffleQuestionsAndAnswers(); // Shuffle both questions and answers together
            isQuizStarted = true;
            currentQuestionIndex = 0;
            ShowQuestion();
        }
        else
        {
            questionText.text = "No questions or answers available!";
        }
    }

    public void ShowQuestion()
    {
        if (currentQuestionIndex < questions.Count)
        {
            // Display the current question
            questionText.text = questions[currentQuestionIndex];

            // Generate and display answer choices
            List<Sprite> answerChoices = GenerateAnswerChoices();
            SetAnswerOptions(answerChoices); // Update the answer option sprites
        }
        else
        {
            questionText.text = "Quiz Complete!";
            isQuizStarted = false;
        }
    }

    private void ShuffleQuestionsAndAnswers()
    {
        // Shuffle questions and their corresponding answers together
        for (int i = questions.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            // Swap questions
            string tempQuestion = questions[i];
            questions[i] = questions[randomIndex];
            questions[randomIndex] = tempQuestion;

            // Swap corresponding answers
            Sprite tempAnswer = allAnswerSprites[i];
            allAnswerSprites[i] = allAnswerSprites[randomIndex];
            allAnswerSprites[randomIndex] = tempAnswer;
        }
    }

    private List<Sprite> GenerateAnswerChoices()
    {
        List<Sprite> answerChoices = new List<Sprite>();

        // Get the correct answer for the current question
        correctAnswer = allAnswerSprites[currentQuestionIndex];
        answerChoices.Add(correctAnswer); // Add the correct answer first

        // Add random incorrect answers to the list
        while (answerChoices.Count < 3) // Ensure we have 3 total choices
        {
            Sprite randomAnswer = allAnswerSprites[Random.Range(0, allAnswerSprites.Count)];

            // Avoid duplicate answers, including the correct one
            if (!answerChoices.Contains(randomAnswer))
            {
                answerChoices.Add(randomAnswer);
            }
        }

        // Shuffle the final choices to randomize the order
        ShuffleAnswers(answerChoices);

        return answerChoices;
    }

    private void ShuffleAnswers(List<Sprite> answerChoices)
    {
        for (int i = answerChoices.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            // Swap elements
            Sprite temp = answerChoices[i];
            answerChoices[i] = answerChoices[randomIndex];
            answerChoices[randomIndex] = temp;
        }
    }

    private void SetAnswerOptions(List<Sprite> answerChoices)
    {
        // Assign the answer sprites and correct status to each AnswerOption
        answerOption1.UpdateAnswer(answerChoices[0], answerChoices[0] == correctAnswer);
        answerOption2.UpdateAnswer(answerChoices[1], answerChoices[1] == correctAnswer);
        answerOption3.UpdateAnswer(answerChoices[2], answerChoices[2] == correctAnswer);
    }

    public void AdvanceQuestion(bool correct)
    {
        if (correct)
        {
            questionText.color = Color.green; // Feedback for correct answer
            currentQuestionIndex++;           // Move to the next question
        }
        else
        {
            questionText.color = Color.red;   // Feedback for wrong answer
        }

        StartCoroutine(WaitAndProceed(correct));
    }

    private IEnumerator WaitAndProceed(bool correct)
    {
        yield return new WaitForSeconds(1f); // Delay before showing the next question

        // Show the next question if there are more questions
        if (currentQuestionIndex < questions.Count)
        {
            questionText.color = Color.white; // Reset text color
            ShowQuestion();
        }
        else
        {
            questionText.text = "Quiz Complete!";
            isQuizStarted = false;
        }
    }
}
