using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Treasure
{
    public GameObject gameObject;
    private Vector3 originalPosition;

    public Treasure(GameObject gameObject)
    {
        this.gameObject = gameObject;
        originalPosition = gameObject.transform.position;
    }

    public bool IsAtOriginalPosition()
    {
        return gameObject.transform.position == originalPosition;
    }
}
