using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    bool initialized;

    [Header("Movement variables")]
    Vector2 desiredSpeed;
    bool canMove = true;
    [SerializeField] float speed;
    [SerializeField] float acceleration;

    [Header("SLap Variables")]
    [SerializeField] float slapRange;
    [SerializeField] float slapOffset;
    [SerializeField] LayerMask slapLayers;

    [Header("Interact Variables")]
    float InteractableCheckTimer;
    [SerializeField] IInteractable closestInteractable;
    [SerializeField, Range(0, 0.3f)] float interactableCheckTime = 0.15f;
    [SerializeField] float interactRange = 10;
    [SerializeField] Collider2D[] interactables = new Collider2D[8];
    [SerializeField] LayerMask interactableLayers;


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
        Collider2D[] slapTargets = Physics2D.OverlapCircleAll((Vector2)transform.position + (InputReader.MoveDirection * slapOffset), slapRange, slapLayers);
        foreach (var target in slapTargets)
        {
            //if(target.TryGetComponent(out ISlapable slapable))
            //{
            //    slapable.Slap();
            //}
        }
    }

    void CheckClosestInteractable()
    {
        if (InteractableCheckTimer < 0)
        {
            InteractableCheckTimer = interactableCheckTime;
            Physics2D.OverlapCircleNonAlloc(transform.position, interactRange, interactables, interactableLayers);
            float distance = Mathf.Infinity;
            foreach (var target in interactables)
            {
                if (target.TryGetComponent(out IInteractable interactable))
                {
                    float targetDis = Vector2.Distance((Vector2)transform.position, (Vector2)target.transform.position);
                    if (targetDis < distance)
                    {
                        closestInteractable = interactable;
                        distance = targetDis;
                    }
                }
            }
        }
        else
        {
            InteractableCheckTimer -= Time.deltaTime;
        }
    }

    void Interact()
    {
        if (!InputReader.Interact) return;
        InputReader.Interact = false;
        if (closestInteractable == null) return;
        closestInteractable.Interact();

    }


}
