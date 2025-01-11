using System.Collections;
using System.Collections.Generic;
using DEPT.Unity;
using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Attributes")]
    [SerializeField] private MovementConfig movementConfig;

    [Header("Platform Specific Settings")]
    [SerializeField] private bool isPC = true;

    [Header("References GameObjects")]
    [SerializeField] private GameObject joystickInput;
    [SerializeField] private LocomotionAnimator animator;

    private CharacterController characterController;
    private MobileInputController joystickController;

    private float turnSmoothVelocity;
    private float gravity = -9.81f;
    private float verticalVelocity = 0f;
    private Vector2 input;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        joystickController = joystickInput.GetComponent<MobileInputController>();
    }

    private void Update()
    {
        HandleInput();
        MoveAndRotateCharacter();
        TriggerAnimations();
    }

    /// <summary>
    /// Handles user input based on the platform.
    /// </summary>
    private void HandleInput()
    {
        if (isPC)
        {
            input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        }
        else
        {
            input = new Vector2(joystickController.Horizontal, joystickController.Vertical);
        }
    }

    /// <summary>
    /// Moves and rotates the character based on user input.
    /// </summary>
    private void MoveAndRotateCharacter()
    {
        Vector3 moveDirection = new Vector3(input.x, 0f, input.y).normalized;

        if (moveDirection.magnitude <= 0.1f)
        {
            return; 
        }

        if (joystickController.magnitude >= 0.2)
        {
            if (joystickController.magnitude <= 0.3f)
            {
                movementConfig.speed = 0.5f;
            }
            else if (joystickController.magnitude <= 0.6f)
            {
                movementConfig.speed = 3f;
            }
            else
            {
                movementConfig.speed = 5f;
            }
        }
        else
        {
            movementConfig.speed = 0f;
        }

        float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
        float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, movementConfig.rotationSmoothTime);
        transform.rotation = Quaternion.Euler(0f, smoothAngle, 0f);

        Vector3 horizontalMovement = moveDirection * movementConfig.speed;

        if (characterController.isGrounded)
        {
            verticalVelocity = 0f;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 movement = new Vector3(horizontalMovement.x, verticalVelocity, horizontalMovement.z) * Time.deltaTime;
        characterController.Move(movement);
    }


    /// <summary>
    /// Triggers the appropriate animation based on movement magnitude.
    /// </summary>
    private void TriggerAnimations()
    {
        float movementMagnitude = input.magnitude;
        if (animator != null)
        {
            animator.SetSpeedXZ(movementMagnitude, Time.deltaTime);
        }
    }
    [Button("RESET POSITION")]
    public void ResetPos()
    {
        transform.position = movementConfig.initPos;
    }
}
