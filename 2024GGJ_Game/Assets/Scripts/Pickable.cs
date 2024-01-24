using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickable : MonoBehaviour, IInteractable
{
    [SerializeField] PickableObject pickableObject;
    bool initialized;
    public Rigidbody2D rb;
    bool interacEnabled = true;

    private void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        initialized = true;
    }

    public void Interact(PlayerController player)
    {
        if (!initialized) Initialize();
        player.PickObject(this);
        interacEnabled = false;
        //rb.isKinematic = true;
    }

    public void Drop(Vector2 force)
    {
        //rb.isKinematic = false;
        rb.AddForce(force);
        interacEnabled = true;
       
    }

    bool IInteractable.CanBeInteracted()
    {
        return interacEnabled;
    }
}
