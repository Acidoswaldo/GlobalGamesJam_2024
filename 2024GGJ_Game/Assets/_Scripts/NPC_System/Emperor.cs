using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Unity.VisualScripting;

public class Emperor : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 1.0f;
    [SerializeField] private float maxRotationAngle = 0f;
    [SerializeField] private float minRotationAngle = -180.0f;
    private Transform playerTransform;
    private bool playerDetected = false;
    [SerializeField] private float currentRotationAngle = -90.0f;
    private int rotationDirection = 1;
    private bool isWaiting = false;

    [Header("Detect Treasures")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private LayerMask treasureLayer;
    [SerializeField] private float rayLength = 10f;
    private bool isDetectingTreasure = true;
    private float lastReduceTime = 0.0f;
    [SerializeField] private float reduceCooldown = 5.0f;
    [SerializeField] private Material missingMaterial;

    [Header("Entertainment Variables")]
    public Slider entertainmentSlider;
    [SerializeField] List<EntretainmentObject> entretainingObjects = new List<EntretainmentObject>();
    Pickable entretainingPickable;
    public class EntretainmentObject
    {
        public int id;
        public float entertainmentDuration;
        public float currentEntertainmentTime;
    }

    bool newEntretainingStarted;

    [Header("Implement")]
    [SerializeField] private GameEventSystem gameEventSystem;

    private void Start()
    {
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.startWidth = 0.05f;
            lineRenderer.endWidth = 0.05f;
        }
        entertainmentSlider = entertainmentSlider.GetComponent<Slider>();
    }

    void Update()
    {

        if (!playerDetected && !isWaiting && isDetectingTreasure)
        {
            isDetectingTreasure = true;
            RotateBackAndForth();
            DetectTreasure();
        }
        else if (playerDetected && entretainingPickable != null)
        {
           // RotateTowardsPlayer();
        }


        if (playerDetected && entertainmentSlider != null && entretainingPickable != null)
        {
            bool EntretainingObjectFoundAndNot0 = false;
            for (int i = 0; i < entretainingObjects.Count; i++)
            {
                if (entretainingPickable.ID == entretainingObjects[i].id)
                {
                    if (entretainingObjects[i].entertainmentDuration > 0)
                    {
                        EntretainingObjectFoundAndNot0 = true;
                        entretainingObjects[i].currentEntertainmentTime -= Time.deltaTime;
                        entertainmentSlider.value = entretainingObjects[i].currentEntertainmentTime / entretainingObjects[i].entertainmentDuration;
                        Debug.Log($"currentEntertainmentTime: {entretainingObjects[i].currentEntertainmentTime}, Slider Value: {entertainmentSlider.value}");
                        isDetectingTreasure = false;
                    }

                }
            }

            if (!EntretainingObjectFoundAndNot0)
            {
                isDetectingTreasure = true;
                RotateBackAndForth();
                DetectTreasure();
            }

        }
        if (entertainmentSlider.value == 0)
        {
            isDetectingTreasure = true;
            RotateBackAndForth();
            DetectTreasure();
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
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerDetected = true;
            playerTransform = other.transform;
            isDetectingTreasure = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerDetected = false;
            playerTransform = null;
            isDetectingTreasure = true;

            if (lineRenderer != null)
            {
                lineRenderer.enabled = true;
            }
        }
    }

    void DetectTreasure()
    {
        if (!isDetectingTreasure) return;

        Vector3 direction = -transform.forward;
        RaycastHit hit;

        if (Physics.Raycast(transform.position, direction, out hit, rayLength, treasureLayer))
        {
            GameObject hitObject = hit.collider.gameObject;
            Renderer renderer = hitObject.GetComponent<Renderer>();
            if (renderer != null && renderer.sharedMaterial == missingMaterial)
            {
                if (gameEventSystem != null && Time.time - lastReduceTime >= reduceCooldown)
                {
                    gameEventSystem.ReduceTime(20.0f);
                    lastReduceTime = Time.time;
                }
            }
        }
        DrawLine(direction);
    }



    void DrawLine(Vector3 direction)
    {
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + direction * rayLength);
        }
    }

    public void Entretain(Pickable pickable)
    {
        if (pickable.type != Pickable.PickableType.Plaything) return;
        bool hasEntretainingObject = false;
        for (int i = 0; i < entretainingObjects.Count; i++)
        {
            if (entretainingObjects[i].id == pickable.ID) hasEntretainingObject = true;
        }
        if (!hasEntretainingObject)
        {
            var NewEntretianingObject = new EntretainmentObject
            {
                id = pickable.ID,
                entertainmentDuration = pickable.EntretainingDuration,
                currentEntertainmentTime = pickable.EntretainingDuration


            };
            entretainingObjects.Add(NewEntretianingObject);
        }
        entretainingPickable = pickable;
        entertainmentSlider.gameObject.SetActive(true);
    }

    public void StopEntertainment()
    {
        entretainingPickable = null;
        entertainmentSlider.gameObject.SetActive(false);
    }
}