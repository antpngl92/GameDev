using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Player Attributes")]
    public float movementSpeed = 10.0f;
    public float jumpHeight = 15.0f;
    public float fallSpeed = -10.0f;
    public CharacterController controller;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundmask;

    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundmask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float verticalMovement = Input.GetAxis("Vertical");
        float horizontalMovement = Input.GetAxis("Horizontal");

        Vector3 finalMovement = transform.right * horizontalMovement + transform.forward * verticalMovement;

        controller.Move(finalMovement * movementSpeed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * fallSpeed);
        }

        velocity.y += fallSpeed * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

    }
}
