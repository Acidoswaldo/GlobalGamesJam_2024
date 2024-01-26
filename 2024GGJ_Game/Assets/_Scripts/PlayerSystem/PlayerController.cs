using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using static UnityEngine.GraphicsBuffer;

public class PlayerController : MonoBehaviour
{
    PlayerController Instance;
    Rigidbody rb;
    Transform playerAvatar;
    bool initialized;

    [Header("Movement variables")]
    [SerializeField] float speed;
    [SerializeField] float acceleration;
    Vector2 moveDirection;
    Vector2 lookDirection;
    bool canMove = true;

    [Header("Slap Variables")]
    [SerializeField] float slapRange;
    [SerializeField] float slapOffset;
    [SerializeField] LayerMask slapLayers;

    [Header("Interact Variables")]
    [SerializeField, Range(0, 0.3f)] float interactableCheckTime = 0.15f;
    [SerializeField] float interactRange = 10;
    [SerializeField] LayerMask interactableLayers;
    List<IInteractable> activeInteractables = new List<IInteractable>();
    IInteractable closestInteractable;
    float InteractableCheckTimer;

    [Header("Pickable Variables")]
    [SerializeField] Pickable currentPickable;
    [SerializeField] Transform pickableTransform;
    [SerializeField] float throwForce;


    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else if (Instance != null) { Destroy(gameObject); }
    }

    void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        playerAvatar = GetComponent<Transform>();
        initialized = true;
    }

    void Update()
    {
        if (!initialized) Initialize();
        SetRigidbodyTransform();
        CheckClosestInteractableTimer();
        Slap();
        Interact();
    }

    private void FixedUpdate()
    {
        SetRigidbodyVelocity();
    }

    private void SetRigidbodyTransform()
    {
        moveDirection = InputReader.MoveDirection;
        if (moveDirection.x != 0)
        {
            playerAvatar.localScale = new Vector3(Mathf.Sign(moveDirection.x), 1, 1);
        }
    }

    private void SetRigidbodyVelocity()
    {
        if (InputReader.Instance == null) return;
        if (!canMove) return;
        moveDirection = InputReader.MoveDirection;
        if (moveDirection != Vector2.zero) lookDirection = moveDirection;
        rb.velocity = Vector2.MoveTowards(rb.velocity, moveDirection * speed, acceleration * Time.deltaTime);

    }

    void Slap()
    {
        if (!InputReader.Slap) return;
        InputReader.Slap = false;

        if (currentPickable != null)
        {
            DropObject();
        }
        Collider2D[] slapTargets = Physics2D.OverlapCircleAll((Vector2)transform.position + (InputReader.MoveDirection * slapOffset), slapRange, slapLayers);
        foreach (var target in slapTargets)
        {
            //if(target.TryGetComponent(out ISlapable slapable))
            //{
            //    slapable.Slap();
            //}
        }
    }

    void CheckClosestInteractableTimer()
    {
        if (InteractableCheckTimer < 0)
        {
            CheckClosestInteractable();
            InteractableCheckTimer = interactableCheckTime;
            Debug.Log("Checking Interactables");
        }
        else
        {
            InteractableCheckTimer -= Time.deltaTime;
        }
    }

    void CheckClosestInteractable()
    {
        activeInteractables.Clear();
        var interactables = Physics.OverlapSphere(transform.position, interactRange, interactableLayers);
        foreach (var target in interactables)
        {
            Debug.Log("Target = " + target.name);
            if (target.TryGetComponent(out IInteractable interactable))
            {
                if (interactable.CanBeInteracted()) activeInteractables.Add(interactable);
            }
        }
        closestInteractable = null;
        float distance = Mathf.Infinity;
        foreach (IInteractable activeInteractable in activeInteractables)
        {
            float targetDis = Vector2.Distance((Vector2)transform.position, (Vector2)activeInteractable.gameObject.transform.position);
            if (targetDis < distance)
            {
                closestInteractable = activeInteractable;
                distance = targetDis;
            }
        }
    }

    void Interact()
    {
        if (!InputReader.Interact) return;
        InputReader.Interact = false;
        if (closestInteractable == null) return;
        closestInteractable.Interact(this);
    }

    public void PickObject(Pickable objectToPick)
    {
        if (currentPickable != null) currentPickable.Drop(lookDirection);
        currentPickable = objectToPick;
        currentPickable.Pick(pickableTransform);
    }

    void DropObject()
    {
        currentPickable.Drop(lookDirection * throwForce);
        currentPickable = null;
        CheckClosestInteractable();
    }


}
