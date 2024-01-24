using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "pickable", menuName = "ScriptableObjects/PickableObject")]
public class PickableObject : ScriptableObject
{
    public int id;
    public string objectName;
    public Sprite sprite;
}
