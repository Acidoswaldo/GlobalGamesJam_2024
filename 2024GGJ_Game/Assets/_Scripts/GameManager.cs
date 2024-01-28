using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Text conversationText;
    public int MaxTreasures; public int currentTreasures;
    public bool GameStarted;
    private bool showedText;


    [SerializeField] PlayerController[] playerControllers;
    public PlayerController[] GetPlayers() { return playerControllers; }
    public int deviceIndex;
    public bool Player1Ready;
    public bool Player2Ready;

    internal void SetDeviceIndex(int Index)
    {
        deviceIndex = Index;
    }

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else if (Instance != this) Destroy(gameObject);
    }


    public void SetPlayerReady(bool player1)
    {
        if (player1)
        {
            Player1Ready = true;
        }else
        {
            Player2Ready = true;
        }
        if(Player1Ready && Player2Ready && showedText == false)
        {
            GameStarted = true;
            GameEventSystem.Instance.StartGame(); 
            UpdateConversationText("Game Start!!");
            showedText = true;
        }
    }


    private void Start()
    {
        var allTreasures = FindObjectsOfType<Pickable>();
        for (int i = 0; i < allTreasures.Length; i++)
        {
            if (allTreasures[i].type == Pickable.PickableType.Treasure || allTreasures[i].type == Pickable.PickableType.HeavyTreasure)
            {
                MaxTreasures += 1;
            }
        }
    }

    public void AddTreasure()
    {
        currentTreasures += 1;
        if(currentTreasures >= MaxTreasures)
        {
            EndGame();
        }
    }

    public void RemoveTreasures()
    {
        currentTreasures -= 1;
    }

    private void EndGame()
    {
        UpdateConversationText("Congraduations! You win!!");
    }

    public void UpdateConversationText(string newText)
    {
        CancelInvoke("ClearText");
        conversationText.text = newText;
        Invoke("ClearText", 3f);
    }

    void ClearText()
    {
        conversationText.text = "";
    }
}
