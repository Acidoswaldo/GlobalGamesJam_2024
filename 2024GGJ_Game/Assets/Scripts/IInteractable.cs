using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable 
{
    public bool CanBeInteracted();
    public void Interact(PlayerController playerController);
    GameObject gameObject { get; }
}
