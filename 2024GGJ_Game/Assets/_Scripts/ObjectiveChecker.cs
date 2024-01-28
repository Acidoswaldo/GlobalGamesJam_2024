using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveChecker : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Pickable pickable))
        {
            if(pickable.type == Pickable.PickableType.Treasure)
            {
                GameManager.Instance.AddTreasure();
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Pickable pickable))
        {
            if (pickable.type == Pickable.PickableType.Treasure)
            {
                GameManager.Instance.RemoveTreasures();
            }
        }
    }
}
