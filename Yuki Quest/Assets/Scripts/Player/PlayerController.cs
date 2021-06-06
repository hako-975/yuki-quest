using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    CharacterController characterController;

    GameObject cam;
    GameObject groundCheck;
    GameObject spawnPoint;

    bool isGrounded;
    bool isRunning;

    Vector3 velocity;
    Vector3 move;

    Animator animator;

    float turnSmoothVelocity;
    float canJump = 0f;

    public float gravity = -9.81f;
    public float groundDistance = 0.25f;
    public float jumpHeight = 2f;
    public float turnSmoothTime = 0.1f;
    public float movementSpeed = 4f;

    public Joystick joystick;
    public JoyButton jumpButton;

    public LayerMask groundMask;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        groundCheck = GameObject.FindGameObjectWithTag("GroundCheck");
        cam = GameObject.FindGameObjectWithTag("MainCamera");
        spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint");

        StartCoroutine(WaitPosition());

    }

    // Update is called once per frame
    void Update()
    {
        // if fall
        if (characterController.transform.position.y < -200f)
        {
            characterController.enabled = false;
            characterController.transform.position = spawnPoint.transform.position;
            characterController.enabled = true;
        }

        isGrounded = Physics.CheckSphere(groundCheck.transform.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float horizontal = Input.GetAxisRaw("Horizontal") + joystick.Horizontal;
        float vertical = Input.GetAxisRaw("Vertical") + joystick.Vertical;

        move = new Vector3(horizontal, 0f, vertical).normalized;

        if (move.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            characterController.Move(moveDirection.normalized * movementSpeed * Time.deltaTime);
        }

        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        isRunning = hasHorizontalInput || hasVerticalInput;

        if ((Input.GetKey(KeyCode.Space) || jumpButton.pressed) && isGrounded && Time.time > canJump)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            canJump = Time.time + 1f;
        }

        velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);

        // Animator
        animator.SetBool("IsRunning", isRunning);
        animator.SetBool("IsGrounded", isGrounded);
    }

    IEnumerator WaitPosition()
    {
        yield return new WaitForEndOfFrame();
        
        spawnPoint.transform.position = new Vector3(0f, 0.05f, 0f);

        characterController.enabled = false;
        characterController.transform.position = spawnPoint.transform.position;
        characterController.enabled = true;
    }
}
