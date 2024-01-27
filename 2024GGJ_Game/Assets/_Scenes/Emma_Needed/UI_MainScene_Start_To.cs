using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//Emma_Add the UI Scene 

public class UI_MainScene_Start_To : MonoBehaviour
{
    private List<GameObject> deactivatedUIObjects = new List<GameObject>();

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //Load Main Menu to Level Level Selection Page.
    public void UI_1_MainMenutoLevelSelect()
    {
        StartCoroutine(LoadSceneAfterDelay());
    }

    IEnumerator LoadSceneAfterDelay()
    {
        // Deactivate all objects in the "UI" layer
        int uiLayer = LayerMask.NameToLayer("UI");
        foreach (var go in FindObjectsOfType(typeof(GameObject)) as GameObject[])
        {
            if (go.layer == uiLayer)
            {
                go.SetActive(false);
                deactivatedUIObjects.Add(go);
            }
        }

        // Wait for 1 second
        yield return new WaitForSeconds(1);

        // Load the scene
        SceneManager.LoadSceneAsync("UI_LevelSelect");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Reactivate all objects in the "UI" layer
        foreach (var go in deactivatedUIObjects)
        {
            go.SetActive(true);
        }
    }
}