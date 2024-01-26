using System.Collections;
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerDetected = true;
            playerTransform = other.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerDetected = false;
            playerTransform = null;
        }
    }
}
