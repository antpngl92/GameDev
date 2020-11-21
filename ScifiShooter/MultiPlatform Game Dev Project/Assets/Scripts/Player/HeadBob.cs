using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBob : MonoBehaviour
{
    [Header("Headbob Variables")]
    public float headbobSpeed = 10f;
    public float bobbingAmount = 0.5f;
    public CharacterController controller;

    private float timer;
    private Vector3 defaultPosition;

    // Start is called before the first frame update
    void Start()
    {
        defaultPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        // Get movement input
        float verticalMovement = Input.GetAxis("Vertical");
        float horizontalMovement = Input.GetAxis("Horizontal");

        // If player is currently moving
        if (Mathf.Abs(verticalMovement) > 0 || (Mathf.Abs(horizontalMovement) > 0))
        {
            // Apply a sine function that moves the player camera up and down
            timer += Time.deltaTime * headbobSpeed;
            transform.localPosition = new Vector3(transform.localPosition.x, defaultPosition.y + Mathf.Sin(timer) * bobbingAmount, transform.localPosition.z);
        }
        else
        {
            // if player is not moving, then move player camera back to default positions
            timer = 0;
            transform.localPosition = new Vector3(transform.localPosition.x, Mathf.Lerp(transform.localPosition.y, defaultPosition.y, Time.deltaTime * headbobSpeed), transform.localPosition.z);
        }
    }
}
