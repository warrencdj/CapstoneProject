using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerController : MonoBehaviour
{
    public void LoadNextScene(int sceneIndex)
    {
        // Load the next scene based on the sceneIndex
        SceneManager.LoadScene(sceneIndex);
    }

    public void LoadNextSceneByName(string sceneName)
    {
        // Load the next scene based on the scene name
        SceneManager.LoadScene(sceneName);
    }
}