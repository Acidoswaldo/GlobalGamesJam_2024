using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Emperor : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1.0f;
    [SerializeField] private float maxRotationAngle = 0f; // 180 degrees
    [SerializeField] private float minRotationAngle = -180.0f;
    private Transform playerTransform;
    private bool playerDetected = false;
    private float currentRotationAngle = -90.0f;
    private int rotationDirection = 1; // 1 for clockwise, -1 for counterclockwise
    private bool isWaiting = false;

    [Header("Treasure Setting")]
    [SerializeField] private float visionRange = 10.0f;
    [SerializeField] private LayerMask treasureLayer;
    private List<Transform> treasures = new List<Transform>();

    [Header("Fan Shaped Area for Sight")]
    [SerializeField] private int numberOfRays = 10; 
    [SerializeField] private LayerMask obstacleLayer;


    private void Start()
    {
        // Initialize the list of treasures
        GameObject[] treasureObjects = GameObject.FindGameObjectsWithTag("Treasure");
        foreach (var treasure in treasureObjects)
        {
            treasures.Add(treasure.transform);
        }
    }

    void Update()
    {
        if (!playerDetected && !isWaiting)
        {
            RotateBackAndForth();
        }
        else if (playerDetected)
        {
            RotateTowardsPlayer();
        }

        CheckTreasures();
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

        transform.rotation = Quaternion.Euler(0, 0, currentRotationAngle);
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

        Vector3 directionToPlayer = playerTransform.position - transform.position;
        float angleToPlayer = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angleToPlayer), rotationSpeed * Time.deltaTime);
    }

    void CheckTreasures()
    {
        foreach (Transform treasure in treasures)
        {
            Vector3 directionToTreasure = treasure.position - transform.position;
            if (Vector3.Angle(transform.forward, directionToTreasure) < maxRotationAngle / 2) // Check if within field of view
            {
                RaycastHit hit;
                if (Physics.Raycast(transform.position, directionToTreasure, out hit, visionRange, treasureLayer))
                {
                    if (hit.transform != treasure) // If the ray did not hit the expected treasure
                    {
                        Debug.Log("Danger");
                        // You can break out of the loop if you only want to log once per frame
                        break;
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerDetected = true;
            playerTransform = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerDetected = false;
            playerTransform = null;
        }
    }
}
