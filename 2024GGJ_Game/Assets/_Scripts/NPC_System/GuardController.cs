using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardController : MonoBehaviour
{
    [SerializeField] private List<Transform> patrolPoints;
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private float rotationSpeed = 5.0f;
    [SerializeField] private float reachThreshold = 0.1f;
    [SerializeField] AnimationCurve AnimationCurve;

    public float T = 0;
    private Transform currentTarget;
    private int currentTargetIndex = -1;

    void Start()
    {
        ChooseNextPatrolPoint();
    }

    void Update()
    {
        if (currentTarget != null)
        {
            MoveTowardsTarget();
            RotateTowardsTarget();
        }
    }

    void ChooseNextPatrolPoint()
    {
        if (patrolPoints.Count == 0)
        {
            return;
        }

        int randomIndex = Random.Range(0, patrolPoints.Count);

        // Ensure not to pick the same point consecutively
        while (randomIndex == currentTargetIndex)
        {
            randomIndex = Random.Range(0, patrolPoints.Count);
        }

        currentTargetIndex = randomIndex;
        currentTarget = patrolPoints[currentTargetIndex];
    }

    void MoveTowardsTarget()
    {
        if (Vector3.Distance(transform.position, currentTarget.position) > reachThreshold)
        {
            T += moveSpeed * Time.deltaTime;
            T = Mathf.Clamp01(T);
            float t = AnimationCurve.Evaluate(T);
            transform.position = Vector3.Lerp(transform.position, currentTarget.position, t);
        }
        else
        {
            T = 0;
            ChooseNextPatrolPoint();
        }
    }

    void RotateTowardsTarget()
    {
        Vector3 targetDirection = currentTarget.position - transform.position;
        float angle = Mathf.Atan2(targetDirection.y, targetDirection.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}