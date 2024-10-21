using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChpapterMenu : MonoBehaviour
{
    public Button[] buttons;
    public GameObject chapterButtons;

    private void Awake()
    {
        //Locking Levels
        ButtonsToArray();
        int unlockedChapter = PlayerPrefs.GetInt("UnlockedChapter", 1);
        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }
        for (int i = 0; i < unlockedChapter; i++)
        {
            buttons[i].interactable = true;
        }
    }

    void ButtonsToArray()
    {
        int childCount = chapterButtons.transform.childCount;
        buttons = new Button[childCount];
        for (int i = 0; i < childCount; i++)
        {
            buttons[i] = chapterButtons.transform.GetChild(i).GetComponent<Button>();
        }
    }
}
