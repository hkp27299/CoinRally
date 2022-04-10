using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMovement : MonoBehaviour
{
    public float forwardSpeed=1;
    void Update()
    {
        transform.position -= Vector3.forward * forwardSpeed * Time.deltaTime;
    }
}
