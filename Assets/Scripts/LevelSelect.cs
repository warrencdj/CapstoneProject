using SojaExiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelect : MonoBehaviour
{
    public void OpenLevel(int levelID)
    {
        // Load the scene by its build index
        SceneManager.LoadSceneAsync(levelID);
    }
}
