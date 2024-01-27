using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerController Instance;
    Rigidbody rb;
    Transform playerAvatar;
    Animator animator;
    bool initialized;
    [SerializeField] int playerIndex;

    [Header("Movement variables")]
    [SerializeField] float speed;
    [SerializeField] float acceleration;
    [SerializeField] float rotateSpeed;
    [SerializeField] bool in3D;
    Vector2 moveDirection;
    Vector3 lookDirection;
    bool canMove = true;
    float _currentRotateVelocity;

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
        animator = GetComponent<Animator>();
        initialized = true;
    }

    void Update()
    {
        if (!initialized) Initialize();
        CheckClosestInteractableTimer();
    }

    private void FixedUpdate()
    {
        SetRigidbodyVelocity();
    }

    public void Move(InputAction.CallbackContext MoveDirection)
    {
        if (!initialized) Initialize();
        moveDirection = MoveDirection.ReadValue<Vector2>();
        if (Vector2.Distance(moveDirection, Vector2.zero) > 0.01f)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
       
    }

    private void SetRigidbodyVelocity()
    {
        if (!canMove) return;
        Vector3 MoveDirection_3D = new Vector3(moveDirection.x, 0, moveDirection.y);
        if (moveDirection != Vector2.zero) lookDirection = moveDirection;
        rb.velocity = Vector3.MoveTowards(rb.velocity, MoveDirection_3D * speed, acceleration * Time.deltaTime);

        var targetAngle = Mathf.Atan2(lookDirection.x, lookDirection.y) * Mathf.Rad2Deg;
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle,ref _currentRotateVelocity, rotateSpeed);
        transform.rotation = Quaternion.Euler(0, angle, 0);

    }

    public void Slap(InputAction.CallbackContext ctx)
    {
        if (!initialized) Initialize();
        if (ctx.performed)
        {
            Debug.Log("Slap");
            if (currentPickable != null)
            {
                DropObject();
            }
            Collider[] slapTargets = Physics.OverlapSphere(transform.position + (transform.forward * slapOffset), slapRange, slapLayers);
            foreach (var target in slapTargets)
            {
                //if(target.TryGetComponent(out ISlapable slapable))
                //{
                //    slapable.Slap();
                //}
            }
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

    public void Interact(InputAction.CallbackContext ctx)
    {
        if (!initialized) Initialize();
        if (ctx.performed)
        {
            if (closestInteractable == null) return;
            closestInteractable.Interact(this);
        }
    }

    public void PickObject(Pickable objectToPick)
    {
        if (currentPickable != null) DropObject();
        currentPickable = objectToPick;
        currentPickable.Pick(pickableTransform);
    }

    void DropObject()
    {
        currentPickable.Drop(moveDirection * throwForce);
        currentPickable = null;
        CheckClosestInteractable();
    }

    public void Pause(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {

        }
    }

    public int PlayerIndex() { return playerIndex; }


}
