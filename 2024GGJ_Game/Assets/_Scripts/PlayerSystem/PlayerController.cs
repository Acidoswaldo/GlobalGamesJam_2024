using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    Vector2 desiredSpeed;
    bool canMove = true;
    bool initialized;
    [Header("Movement variables")]
    [SerializeField] float speed;
    [SerializeField] float acceleration;
    [Header("SLap Variables")]
    [SerializeField] Transform slapTransform;
    [SerializeField] float slapRange;
    [SerializeField] LayerMask slapLayers;

    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        initialized = true;
    }

    void Update()
    {
        if (!initialized) Initialize();
        SetRigidbodyVelocity();
        Slap();
        Interact();
    }

    private void SetRigidbodyVelocity()
    {
        if (InputReader.Instance == null) return;
        if (!canMove) return;
        Vector2 moveDirection = InputReader.MoveDirection;
        rb.velocity = Vector2.MoveTowards(rb.velocity, moveDirection * speed, acceleration * Time.deltaTime);
    }

    void Slap()
    {
        if (!InputReader.Slap) return;
        InputReader.Slap = false;
       Collider2D[] slapTargets = Physics2D.OverlapCircleAll(slapTransform.position, slapRange, slapLayers);
        foreach (var target  in slapTargets)
        {
            //if(target.TryGetComponent(out ISlapable slapable))
            //{
            //    slapable.Slap();
            //}
        }
    }

    void Interact()
    {
        if (!InputReader.Interact) return;
        InputReader.Interact = false;
        Collider2D[] slapTargets = Physics2D.OverlapCircleAll(slapTransform.position, slapRange, slapLayers);
        foreach (var target in slapTargets)
        {
            if (target.TryGetComponent(out IInteractable interactable))
            {
                interactable.Interact();
            }
        }
    }
}
