using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    public float rotateSpeed = 1.0f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0,0,rotateSpeed, Space.Self);
    }
}
