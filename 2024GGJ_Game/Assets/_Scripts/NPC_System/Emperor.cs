using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;

public class Emperor : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1.0f;
    [SerializeField] private float maxRotationAngle = 0f; // 180 degrees
    [SerializeField] private float minRotationAngle = -180.0f;
    private Transform playerTransform;
    private bool playerDetected = false;
    [SerializeField] private float currentRotationAngle = -90.0f;
    private int rotationDirection = 1; 
    private bool isWaiting = false;

    [SerializeField] private LayerMask treasureLayer; // Layer for treasures
    [SerializeField] private float rayLength = 10f; // Length of the detection ray
    private bool isDetectingTreasure = true;
    [SerializeField] private List<GameObject> treasures;
    [SerializeField] private Material missingMaterial;

    private void Start()
    {
    }

    void Update()
    {
        if (!playerDetected && !isWaiting && isDetectingTreasure)
        {
            RotateBackAndForth();
            DetectTreasure();
        }
        else if (playerDetected)
        {
            RotateTowardsPlayer();
        }
    }

    void RotateBackAndForth()
    {
        currentRotationAngle += rotationSpeed * Time.deltaTime * rotationDirection;

        if (currentRotationAngle > maxRotationAngle)
        {
            currentRotationAngle = maxRotationAngle;
            rotationDirection = -1;
            StartCoroutine(WaitRandomTime());
        }
        else if (currentRotationAngle < minRotationAngle)
        {
            currentRotationAngle = minRotationAngle;
            rotationDirection = 1;
            StartCoroutine(WaitRandomTime());
        }

        transform.rotation = Quaternion.Euler(transform.rotation.x, currentRotationAngle, transform.rotation.z);
    }

    private IEnumerator WaitRandomTime()
    {
        isWaiting = true;
        float waitTime = Random.Range(1f, 3f); 
        yield return new WaitForSeconds(waitTime);
        isWaiting = false;
    }

    void RotateTowardsPlayer()
    {
        if (playerTransform == null) return;

        Vector3 directionToPlayer = -(playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerDetected = true;
            playerTransform = other.transform;
            isDetectingTreasure = false; // Í£Ö¹ÉäÏß¼ì²â
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerDetected = false;
            playerTransform = null;
            isDetectingTreasure = true; // »Ö¸´ÉäÏß¼ì²â
        }
    }

    void DetectTreasure()
    {
        Vector3 direction = -transform.forward;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, direction, out hit, rayLength, treasureLayer))
        {
            Debug.Log("Get Treasure");
            GameObject hitObject = hit.collider.gameObject;
            if (treasures.Contains(hitObject)) 
            {
                Renderer renderer = hitObject.GetComponent<Renderer>();
                if (renderer != null && renderer.sharedMaterial == missingMaterial)
                {
                    Debug.Log("Danger");
                }
            }
        }

        Debug.DrawRay(transform.position, direction * rayLength, Color.red);
    }
}
