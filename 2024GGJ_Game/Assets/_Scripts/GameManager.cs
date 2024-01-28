using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int MaxTreasures; public int currentTreasures;


    [SerializeField] PlayerController[] playerControllers;
    public PlayerController[] GetPlayers() { return playerControllers; }
    public int deviceIndex;

    internal void SetDeviceIndex(int Index)
    {
        deviceIndex = Index;
    }

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else if (Instance != this) Destroy(gameObject);
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
        Debug.Log("Won Game");
    }
}
