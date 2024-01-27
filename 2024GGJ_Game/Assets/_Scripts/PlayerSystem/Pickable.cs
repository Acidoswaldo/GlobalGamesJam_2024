using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickable : MonoBehaviour, IInteractable
{
    public enum PickableType { Treasure, HeavyTreasure, Plaything }
    public PickableType type;
    [SerializeField] float followSpeed;
    [SerializeField] float upForce = 20;
    bool initialized;
    public Rigidbody rb;
    bool interacEnabled = true;
    bool picked;
    Transform targetTransform;
    [SerializeField] LayerMask everythingMask;
    [SerializeField] LayerMask nothingMask;
    Vector3 originalScale;
    [SerializeField] Vector3 pickedScale;
    [SerializeField] float scaleSpeed = 5;

    [SerializeField] private GameObject missingPrefab;

    private void Start()
    {
        Initialize();
        originalScale = transform.localScale;
    }

    void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null) rb = gameObject.AddComponent<Rigidbody>();
        initialized = true;
    }
    private void Update()
    {
        Movement();
        if (!picked)
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, originalScale, scaleSpeed * Time.deltaTime);
        }
        else
        {
            transform.localScale = Vector3.MoveTowards(transform.localScale, pickedScale, scaleSpeed * Time.deltaTime);
        }
    }

    void Movement()
    {
        if (targetTransform == null) return;
        Vector3 dir = targetTransform.position - transform.position;
        rb.velocity = dir * followSpeed;
    }

    public void Interact(PlayerController player)
    {
        if (picked) return;
        if (!initialized) Initialize();
        player.PickObject(this);
        interacEnabled = false;
        //rb.isKinematic = true;
    }

    public void Pick(Transform playerTarget)
    {
        picked = true;
        targetTransform = playerTarget;
        rb.excludeLayers = everythingMask;
        transform.rotation = playerTarget.rotation;
        rb.angularVelocity = Vector3.zero;

        if (missingPrefab != null)
        {
            GameObject missingObject = Instantiate(missingPrefab, transform.position, Quaternion.identity);
        }
    }

    public void Drop(Vector3 force, bool applyUpForce = true)
    {
        rb.velocity /= 2;
        Vector3 throwForce = force;
        rb.AddForce(throwForce, ForceMode.Impulse);
        if (applyUpForce) rb.AddForce(Vector3.up * upForce, ForceMode.Impulse);
        interacEnabled = true;
        picked = false;
        targetTransform = null;
        rb.excludeLayers = nothingMask;
    }

    bool IInteractable.CanBeInteracted()
    {
        return interacEnabled;
    }
}
