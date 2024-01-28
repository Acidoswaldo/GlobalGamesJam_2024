using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ObjectiveUI : MonoBehaviour
{
    public TextMeshProUGUI currentObjective;
    public TextMeshProUGUI maxObjective;

    void Update()
    {
        currentObjective.text = GameManager.Instance.currentTreasures.ToString();
        maxObjective.text = GameManager.Instance.MaxTreasures.ToString();
        
    }
}
