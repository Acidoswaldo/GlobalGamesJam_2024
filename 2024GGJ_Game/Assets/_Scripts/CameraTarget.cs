using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraTarget : MonoBehaviour
{
    [SerializeField] PlayerController controller_1;
    [SerializeField] PlayerController controller_2;
    [SerializeField] Transform king;
    [SerializeField] CinemachineVirtualCamera vCamera;
    [SerializeField] float minClamp;
    [SerializeField] float maxClamp;
    [SerializeField, Range(0f, 10f)] float offset;
    [SerializeField, Range(0f, 10f)] float playerInfluence = 2.5f;
    CinemachineFramingTransposer transposer;
    bool initialized;
    void Start()
    {
        Initiialize();
    }

    void Initiialize()
    {

        if (controller_1 == null) controller_1 = GameManager.Instance.GetPlayers()[0];
        controller_2 = GameManager.Instance.GetPlayers()[1];
        transposer = vCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        initialized = true;
    }


    void Update()
    {
        if (initialized) Initiialize();
        Vector3 midPoint = ((controller_1.transform.position * playerInfluence) + (controller_2.transform.position * playerInfluence) + king.position) / (1 + (playerInfluence * 2));
        float playerDistance = Vector3.Distance(controller_1.transform.position, controller_2.transform.position) + offset;
        float King1Distance = Vector3.Distance(controller_1.transform.position, king.position) + offset;
        float king2Distance = Vector3.Distance(controller_2.transform.position, king.position) + offset;

        float distance = playerDistance;
        if (King1Distance > distance) distance = King1Distance;
        if (king2Distance > distance) distance = king2Distance;

        distance = Mathf.Clamp(distance, minClamp, maxClamp);
        transposer.m_CameraDistance = distance;
        transform.position = midPoint;
    }
}
