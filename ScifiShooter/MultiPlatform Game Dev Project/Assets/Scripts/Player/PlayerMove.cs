using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    #region Variables
    [Header("Player Attributes")]
    public float movementSpeed = 10.0f;
    public float jumpHeight = 15.0f;
    public float fallSpeed = -10.0f;
    public CharacterController controller;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundmask;

    public AudioClip jumpSound;
    public AudioClip[] footstepSounds = new AudioClip[5];
    public float footstepSoundDelay = 0.1f;

    private Vector3 velocity;
    private bool isGrounded;
    private bool isMoving;
    public AudioSource audioSource;

    private bool isfootStepPlaying = false;
    #endregion

    void Start()
    {
        controller = GetComponent<CharacterController>();
   
    }

    // Update is called once per frame
    void Update()
    {
        // Calculates if player is on the ground using the groundCheck gameobject
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundmask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Get movement input
        float verticalMovement = Input.GetAxis("Vertical");
        float horizontalMovement = Input.GetAxis("Horizontal");

        isMoving = (Mathf.Abs(verticalMovement) > 0 || Mathf.Abs(horizontalMovement) > 0) ? true : false;

        // Calculate movement
        Vector3 finalMovement = transform.right * horizontalMovement + transform.forward * verticalMovement;

        controller.Move(finalMovement * movementSpeed * Time.deltaTime);

        // Jumping mechanic
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * fallSpeed);
 
            audioSource.clip = jumpSound;
            audioSource.Play();

        }

        // If player is in the air, bring them down every frame
        velocity.y += fallSpeed * Time.deltaTime;

        // Play footstep sound
        if (isMoving && isGrounded)
        {
            if (!isfootStepPlaying)
            {
                PlayFootstepSounds();
            }
        }
        
        controller.Move(velocity * Time.deltaTime);
    }

    void PlayFootstepSounds()
    {
        StartCoroutine(PlayFootsteps());
    }

    IEnumerator PlayFootsteps()
    {
        // Pick a random footstep sound
        audioSource.clip = footstepSounds[Random.Range(0, footstepSounds.Length)];
        audioSource.Play();

        isfootStepPlaying = true;

        yield return new WaitForSeconds(footstepSoundDelay);

        isfootStepPlaying = false;
    }
}
