using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//Emma_Add the UI Scene 

public class UI_MainScene_Start_To : MonoBehaviour
{
    //Load Main Menu to Level Level Selection Page.
    public void UI_1_MainMenutoLevelSelect()
    {
        SceneManager.LoadSceneAsync("UI_LevelSelect");    
    }

}
