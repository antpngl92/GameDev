using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    
    void Update()
    {
        // Simply keep rotating the game object every frame.
        transform.Rotate(new Vector3(0, 30, 0) * Time.deltaTime);
    }
}
