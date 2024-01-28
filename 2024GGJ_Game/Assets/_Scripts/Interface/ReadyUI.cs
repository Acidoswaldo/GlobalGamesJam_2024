using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ReadyUI : MonoBehaviour
{
    public TextMeshProUGUI player1StatusText;
    public TextMeshProUGUI player2StatusText;
    // Start is called before the first frame update
    void Update()
    {
        if (GameManager.Instance.Player1Ready)
        {
            player1StatusText.text = "Ready";
            player1StatusText.color = Color.green;
        }
        if (GameManager.Instance.Player2Ready)
        {
            player2StatusText.text = "Ready";
            player2StatusText.color = Color.green;
        }

        if (GameManager.Instance.Player1Ready && GameManager.Instance.Player2Ready) gameObject.SetActive(false);
    }
}
