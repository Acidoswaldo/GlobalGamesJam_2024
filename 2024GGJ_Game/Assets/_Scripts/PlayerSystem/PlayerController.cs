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
    [SerializeField] ParticleSystem moveParticles;
    Vector3 moveDirection;
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

    [Header("Entertaining Variables")]
    public bool entertaining;
    [SerializeField] ParticleSystem entretainParticles;

    [Header("Emperor Variables")]
    [SerializeField] private Emperor emperor;


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
        UpdateAnimator();
        UpdateEntertaining();
    }

    void UpdateAnimator()
    {
        float target = 0;
        if (currentPickable != null) { target = 1; }

        Debug.Log("layer" + animator.GetLayerName(1));
        float layerWeight = animator.GetLayerWeight(1);
        layerWeight = Mathf.MoveTowards(layerWeight, target, 2 * Time.deltaTime);
        animator.SetLayerWeight(1, layerWeight);
    }

    void UpdateEntertaining()
    {
        if (entertaining && currentPickable != null)
        {
            canMove = false;
            Debug.Log("Entertaining");
            entretainParticles.Play();
            emperor.Entretain(currentPickable);
        }
        else
        {
            canMove = true;
            entretainParticles.Stop();
            emperor.StopEntertainment();
        }
    }

    private void FixedUpdate()
    {
        SetRigidbodyVelocity();
    }

    public void Move(InputAction.CallbackContext MoveDirection)
    {
        if (!initialized) Initialize();

        // Get movement direction relative to main camera transform
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        moveDirection = forward * MoveDirection.ReadValue<Vector2>().y + right * MoveDirection.ReadValue<Vector2>().x;

        if (MoveDirection.ReadValue<Vector2>() != Vector2.zero && canMove)
        {
            if (moveParticles != null) moveParticles.Play();
            animator.SetBool("isWalking", true);
        }
        else
        {
            if (moveParticles != null) moveParticles.Stop();
            animator.SetBool("isWalking", false);
        }

    }

    private void SetRigidbodyVelocity()
    {
        if (!canMove)
        {
            rb.velocity = Vector3.MoveTowards(rb.velocity, Vector3.zero, acceleration * 2 * Time.deltaTime);
            return;
        }
        float moveSpeed = speed;
        if (currentPickable != null)
        {
            if (currentPickable.type == Pickable.PickableType.HeavyTreasure) moveSpeed /= 4;
            else if(currentPickable.type == Pickable.PickableType.Treasure) moveSpeed /= 2;
        }
        Vector3 MoveDirection_3D = moveDirection * moveSpeed;
        if (moveDirection != Vector3.zero) lookDirection = moveDirection;

        rb.velocity = Vector3.MoveTowards(rb.velocity, MoveDirection_3D, acceleration * Time.deltaTime);

        var targetAngle = Mathf.Atan2(lookDirection.x, lookDirection.z) * Mathf.Rad2Deg;
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _currentRotateVelocity, rotateSpeed);
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
            if(currentPickable != null)
            {
                if (currentPickable.type == Pickable.PickableType.Plaything)
                {
                    Entertain();
                }
                else
                {
                    DropObject();
                }
                return;
            }
            if (closestInteractable == null)
            {
                return;
            }
            closestInteractable.Interact(this);
        }else if (ctx.canceled)
        {
            StopEntertain();
        }
    }

    void Entertain()
    {
        entertaining = true;
    }
    void StopEntertain()
    {
        entertaining = false;
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
