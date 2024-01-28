using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PickableStand : MonoBehaviour, IInteractable
{
    [SerializeField] GameObject Missing;
    [SerializeField] float UpOffset;
    [SerializeField, Range(0, 0.5f)] float timeBetweenChecks = 0.1f;
    float checkTimer;
    bool showMissable;
    bool ObjectInStand;
    BoxCollider boxCollider;
    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }
    private void Update()
    {
        Missing.SetActive(showMissable);
        if(checkTimer <= 0)
        {
            checkTimer = timeBetweenChecks;
            Check();
        }
        else
        {
            checkTimer -= Time.deltaTime;
        }

    }
    void Check()
    {
        var hits = Physics.BoxCastAll(transform.position + (Vector3.up * UpOffset), (boxCollider.size / 2) * 1.5f, Vector3.up); ;
        showMissable = true;
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.TryGetComponent(out Pickable pickable))
            {
                showMissable = false;
            }
        }
    }


    public bool CanBeInteracted()
    {
        return showMissable;
    }

    public void Interact(PlayerController playerController)
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + (Vector3.up * UpOffset), boxCollider.size * 1.5f);
    }
}
