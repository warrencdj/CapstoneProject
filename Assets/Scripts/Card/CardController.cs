using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    [SerializeField] private Sprite backCard; // The back card image
    [SerializeField] private Sprite[] cards; // Drag and drop your card sprites in the Inspector
    [SerializeField] private int cardsPerRound = 10; // Number of pairs (cards) per round
    [SerializeField] private int totalRounds = 3; // Total rounds in the game

    public List<Sprite> gameCards = new List<Sprite>();
    public List<Button> btns = new List<Button>();

    private bool firstGuess, secondGuess;
    private int countGuesses;
    private int countCorrectGuesses;
    private int gameGuesses;
    private int currentRound = 1;

    private int firstGuessIndex, secondGuessIndex;
    private string firstGuessCard, secondGuessCard;

    void Start()
    {
        GetButtons();
        AddCardListeners();
        StartGame();
    }

    void GetButtons()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("CardButton");

        for (int i = 0; i < objects.Length; i++)
        {
            btns.Add(objects[i].GetComponent<Button>());
            btns[i].image.sprite = backCard;
        }
    }

    void StartGame()
    {
        currentRound = 1; // Start from the first round
        StartRound();
    }

    void StartRound()
    {
        // Reset for a new round
        gameCards.Clear();
        ShuffleCardsForRound(); // Shuffle the new cards for the round
        Shuffle(gameCards); // Shuffle the list after adding pairs
        gameGuesses = gameCards.Count / 2;

        // Reset button images before showing new cards
        ResetButtonImages();

        // Update UI or notify the player about the round (optional)
        Debug.Log("Starting Round " + currentRound);
        AddGameCards(); // Display the new cards for this round
    }

    void ShuffleCardsForRound()
    {
        int startIndex = 0; // Start of the cards for the current round
        int endIndex = Mathf.Min(cards.Length, cardsPerRound); // Ensure no index goes out of bounds

        // Collect cards for the current round
        for (int i = startIndex; i < endIndex; i++)
        {
            gameCards.Add(cards[i]);
        }

        // Now, duplicate the cards (create pairs)
        List<Sprite> pairedCards = new List<Sprite>(gameCards);
        gameCards.AddRange(pairedCards); // Add pairs to the game cards list
    }

    void AddGameCards()
    {
        // Ensure there are enough buttons to display the cards
        for (int i = 0; i < btns.Count; i++)
        {
            if (i < gameCards.Count)
            {
                btns[i].image.sprite = backCard; // Reset to back card initially
                btns[i].interactable = true; // Make sure all buttons are interactable
                btns[i].image.color = Color.white; // Reset color
            }
            else
            {
                btns[i].image.sprite = backCard; // If there are fewer cards than buttons, set back card
                btns[i].interactable = false; // Disable extra buttons
            }
        }

        // Shuffle the cards and assign them to buttons
        for (int i = 0; i < gameCards.Count; i++)
        {
            btns[i].image.sprite = backCard; // Ensure back card is shown initially
        }
    }

    void ResetButtonImages()
    {
        foreach (Button btn in btns)
        {
            btn.image.sprite = backCard; // Reset all buttons to show the back card initially
            btn.interactable = true; // Make sure they are interactable
        }
    }

    void AddCardListeners()
    {
        foreach (Button btn in btns)
        {
            btn.onClick.AddListener(() => PickUpCard());
        }
    }

    public void PickUpCard()
    {
        if (!firstGuess)
        {
            firstGuess = true;

            firstGuessIndex = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);

            firstGuessCard = gameCards[firstGuessIndex].name;

            btns[firstGuessIndex].image.sprite = gameCards[firstGuessIndex];
        }
        else if (!secondGuess)
        {
            secondGuess = true;

            secondGuessIndex = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);

            secondGuessCard = gameCards[secondGuessIndex].name;

            btns[secondGuessIndex].image.sprite = gameCards[secondGuessIndex];

            countGuesses++;

            StartCoroutine(CheckIfTheCardsMatch());
        }
    }

    IEnumerator CheckIfTheCardsMatch()
    {
        yield return new WaitForSeconds(1f);

        if (firstGuessCard == secondGuessCard)
        {
            yield return new WaitForSeconds(.5f);

            btns[firstGuessIndex].interactable = false;
            btns[secondGuessIndex].interactable = false;

            btns[firstGuessIndex].image.color = new Color(0, 0, 0, 0);
            btns[secondGuessIndex].image.color = new Color(0, 0, 0, 0);

            countCorrectGuesses++; // Increase correct guesses when a match is found

            CheckIfTheGameIsFinished(); // Check if all pairs are matched
        }
        else
        {
            yield return new WaitForSeconds(.5f);

            btns[firstGuessIndex].image.sprite = backCard;
            btns[secondGuessIndex].image.sprite = backCard;
        }

        yield return new WaitForSeconds(.5f);

        firstGuess = secondGuess = false;
    }

    void CheckIfTheGameIsFinished()
    {
        if (countCorrectGuesses == gameGuesses) // All pairs have been found
        {
            if (currentRound < totalRounds)
            {
                // Move to the next round
                currentRound++;
                countCorrectGuesses = 0; // Reset correct guesses for the new round
                StartRound(); // Start the new round with shuffled cards
            }
            else
            {
                Debug.Log("Game Finished");
                Debug.Log("It took " + countGuesses + " guesses to finish");
            }
        }
    }

    void Shuffle(List<Sprite> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Sprite tmp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = tmp;
        }
    }
}
