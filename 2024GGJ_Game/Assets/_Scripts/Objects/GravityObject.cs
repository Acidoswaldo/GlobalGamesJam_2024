using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class GravityObject : MonoBehaviour
{
    enum GravityDirection { forward, up, down, back, left, right}
    [SerializeField] GravityDirection gravityDirection;
    [SerializeField] float gravityScale = 1;
    Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Vector3 direction = Vector3.down;
        switch (gravityDirection)
        {
            case GravityDirection.forward:
                direction = Vector3.forward;
                break;
            case GravityDirection.up:
                direction = Vector3.up;
                break;
            case GravityDirection.down:
                direction = -Vector3.up;
                break;
            case GravityDirection.back:
                direction = -Vector3.forward;
                break;
            case GravityDirection.left:
                direction = -Vector3.right;
                break;
            case GravityDirection.right:
                direction = Vector3.right;
                break;
        }

        rb.AddForce(direction * gravityScale);
    }

}
