using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class QuizManager : MonoBehaviour
{
    [TextArea]
    public List<string> questions; // List of questions
    public List<Sprite> allAnswerSprites; // All possible answer sprites

    public TextMeshPro titleText;
    public TextMeshPro questionText;

    public AnswerOption answerOption1;
    public AnswerOption answerOption2;
    public AnswerOption answerOption3;

    public GameObject stageClearPanel; // Reference to the stage clear panel UI
    public GameObject playerUI; // Reference to the Player UI

    private Queue<QuestionData> questionQueue; // Queue to track questions
    private QuestionData currentQuestion; // Track the current question
    private bool isQuizStarted = false;

    private class QuestionData
    {
        public string QuestionText;
        public Sprite CorrectAnswer;
        public List<Sprite> AnswerChoices;
    }

    void Start()
    {
        questionText.text = "Are you ready to play? Step on the green button to start.";
        stageClearPanel.SetActive(false); // Initially hide the stage clear panel
        playerUI.SetActive(true); // Ensure Player UI is active initially
    }

    public void StartQuiz()
    {
        if (questions.Count > 0 && allAnswerSprites.Count >= 3)
        {
            isQuizStarted = true;
            questionQueue = GenerateQuestionQueue(); // Procedurally generate questions
            ShowNextQuestion();
        }
        else
        {
            questionText.text = "Not enough data to start the quiz!";
        }
    }

    private Queue<QuestionData> GenerateQuestionQueue()
    {
        List<QuestionData> questionList = new List<QuestionData>();

        for (int i = 0; i < questions.Count; i++)
        {
            string questionText = questions[i];
            Sprite correctAnswer = allAnswerSprites[i];

            // Generate answer choices
            List<Sprite> answerChoices = new List<Sprite> { correctAnswer };
            while (answerChoices.Count < 3)
            {
                Sprite randomAnswer = allAnswerSprites[Random.Range(0, allAnswerSprites.Count)];
                if (!answerChoices.Contains(randomAnswer))
                {
                    answerChoices.Add(randomAnswer);
                }
            }

            // Shuffle answer choices
            ShuffleList(answerChoices);

            // Create a new QuestionData instance
            QuestionData questionData = new QuestionData
            {
                QuestionText = questionText,
                CorrectAnswer = correctAnswer,
                AnswerChoices = answerChoices
            };

            questionList.Add(questionData);
        }

        // Shuffle the question list to randomize question order
        ShuffleList(questionList);

        // Convert the list to a queue for easy processing
        return new Queue<QuestionData>(questionList);
    }

    private void ShowNextQuestion()
    {
        if (questionQueue.Count > 0)
        {
            currentQuestion = questionQueue.Dequeue();

            // Update the question text
            questionText.text = currentQuestion.QuestionText;

            // Assign answer choices to the answer options
            answerOption1.UpdateAnswer(currentQuestion.AnswerChoices[0], currentQuestion.AnswerChoices[0] == currentQuestion.CorrectAnswer);
            answerOption2.UpdateAnswer(currentQuestion.AnswerChoices[1], currentQuestion.AnswerChoices[1] == currentQuestion.CorrectAnswer);
            answerOption3.UpdateAnswer(currentQuestion.AnswerChoices[2], currentQuestion.AnswerChoices[2] == currentQuestion.CorrectAnswer);
        }
        else
        {
            // When quiz is finished, show the stage clear panel with a delay
            StartCoroutine(ShowStageClear());
        }
    }

    public void AdvanceQuestion(bool correct)
    {
        StartCoroutine(HandleAnswerFeedback(correct));
    }

    private IEnumerator HandleAnswerFeedback(bool correct)
    {
        questionText.color = correct ? Color.green : Color.red; // Feedback color
        yield return new WaitForSeconds(1f); // Short delay for feedback
        questionText.color = Color.white; // Reset color

        if (correct)
        {
            ShowNextQuestion();
        }
        else
        {
            ReshuffleAnswers(); // Reshuffle the answers for the current question
        }
    }

    private void ReshuffleAnswers()
    {
        // Reshuffle the answer choices for the current question
        ShuffleList(currentQuestion.AnswerChoices);

        // Reassign the reshuffled answer choices to the answer options
        answerOption1.UpdateAnswer(currentQuestion.AnswerChoices[0], currentQuestion.AnswerChoices[0] == currentQuestion.CorrectAnswer);
        answerOption2.UpdateAnswer(currentQuestion.AnswerChoices[1], currentQuestion.AnswerChoices[1] == currentQuestion.CorrectAnswer);
        answerOption3.UpdateAnswer(currentQuestion.AnswerChoices[2], currentQuestion.AnswerChoices[2] == currentQuestion.CorrectAnswer);
    }

    private IEnumerator ShowStageClear()
    {
        // Hide the player UI when the stage clear screen is shown
        playerUI.SetActive(false);

        // Wait for a short delay before showing the stage clear screen
        yield return new WaitForSeconds(1f);

        // Display the stage clear panel
        stageClearPanel.SetActive(true);
    }

    private void ShuffleList<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
