using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
   
    [SerializeField] PlayerController[] playerControllers;
    public PlayerController[] GetPlayers() { return playerControllers; }


    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else if (Instance != this) Destroy(gameObject);
    }
}
