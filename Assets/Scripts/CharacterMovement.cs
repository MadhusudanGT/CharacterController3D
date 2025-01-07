using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Attributes")]
    [SerializeField] private MovementConfig movementConfig;

    [Header("Platform Specific Settings")]
    [SerializeField] private bool isPC = true;

    [Header("References GameObjects")]
    [SerializeField] private GameObject mobileInputLeft;
    [SerializeField] private GameObject mobileInputRight;

    private CharacterController characterController;
    private float turnSmoothVelocity;

    // Input Values
    private Vector2 input;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        HandleInput();
        MoveCharacter();
        RotateCharacter();
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
            var mobileInput = mobileInputLeft.GetComponent<MobileInputController>();
            input = new Vector2(mobileInput.Horizontal, mobileInput.Vertical);
        }
    }

    /// <summary>
    /// Moves the character based on user input.
    /// </summary>
    private void MoveCharacter()
    {
        Vector3 moveDirection = new Vector3(input.x, 0f, input.y).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            Vector3 movement = moveDirection * movementConfig.speed * Time.deltaTime;
            characterController.Move(movement);
        }
    }

    /// <summary>
    /// Rotates the character based on user input.
    /// </summary>
    private void RotateCharacter()
    {
        if (isPC) return; // Skip rotation handling for PC input.

        var mobileInput = mobileInputRight.GetComponent<MobileInputController>();
        Vector3 targetDirection = new(mobileInput.Horizontal, 0f, mobileInput.Vertical);

        if (targetDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(targetDirection.x, targetDirection.z) * Mathf.Rad2Deg;
            float smoothedAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, movementConfig.turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, smoothedAngle, 0f);
        }
    }
}
