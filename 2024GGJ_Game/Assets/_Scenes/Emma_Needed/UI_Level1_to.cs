using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//Emma_Add the UI Scene 

public class UI_Level1_to : MonoBehaviour
{

    //Load LevelSelect Menu to Level_1
    public void UI_2_LevelSelectto_1()
    {
        SceneManager.LoadSceneAsync("Level1");    
    }

}
