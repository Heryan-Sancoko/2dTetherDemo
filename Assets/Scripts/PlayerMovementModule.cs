using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class PlayerMovementModule : PlayerModule
{
    [Header("Movement variables")]
    [SerializeField] private Vector2 inputVector;
    public Vector2 InputVector => inputVector;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float topSpeed;
    private Rigidbody rbody;


    [Header("Jump variables")]
    [SerializeField] private float jumpSpeed;
    [SerializeField, Tooltip("How quickly the jumpspeed decays to 0. Lerp.")] private float jumpSpeedDecayRate;
    [SerializeField] private float availableJumpTime;
    [SerializeField] private int availableJumps;
    [SerializeField] private float jumpCancelTime;
    [SerializeField] private float availableCoyoteTime = 0.2f;
    private float currentJumpSpeed;
    [SerializeField] private int jumpsUsed;
    private float usedJumpTime;
    private float coyoteTimeUsed;

    public enum MoveStatus{idle, jumping, moving, tethering, passive};
    [SerializeField] private MoveStatus currentMoveStatus;

    private InputManager inputmanager;
    private GroundedCheckModule groundedCheckModule;

    //cancel jump when hitting a roof or enemy contact
    public UnityAction JumpCancelled;

    private static float playerDiameter = 0.5f;


    private void Start()
    {
        rbody = playerController.Rbody;
        inputmanager = InputManager.Instance;
        inputmanager.Move.performed += OnMove;
        inputmanager.Move.canceled += OnMove;
        inputmanager.Jump.performed += OnJump;
        JumpCancelled += OnJumpCancelled;

        groundedCheckModule = playerController.GetModule<GroundedCheckModule>();
        groundedCheckModule.JustLanded += OnLand;

    }

    private void OnMove(InputAction.CallbackContext callback)
    {
        inputVector = callback.ReadValue<Vector2>();
    }

    private void OnJump(InputAction.CallbackContext callback)
    {
        if (jumpsUsed >= availableJumps)
            return;
        else if (jumpsUsed < availableJumps)
        {
            //move the player to the ground if we are grounded
            //so that all jumps have a consistent starting point
            if (groundedCheckModule.IsGrounded)
                transform.position = groundedCheckModule.HitPoint + (Vector3.up * playerDiameter);

            //don't consume a jump if we are grounded or have coyote time
            if (!groundedCheckModule.IsGrounded && coyoteTimeUsed >= availableCoyoteTime)
                jumpsUsed++;

            //consume coyote time when we jump
            //and begin jumping
            coyoteTimeUsed = availableCoyoteTime;
            usedJumpTime = 0;
            currentJumpSpeed = jumpSpeed;
            currentMoveStatus = MoveStatus.jumping;
        }
    }

    private void OnLand()
    {
        jumpsUsed = 0;
        coyoteTimeUsed = 0;
        currentMoveStatus = inputVector == Vector2.zero ?
            MoveStatus.idle : 
            MoveStatus.moving;
    }

    public override void UpdatePlayerModule()
    {
        base.UpdatePlayerModule();

        if (usedJumpTime < availableJumpTime)
        {
            //if jump is released early, cancel ascension and begin descent
            if (inputmanager.Jump.WasReleasedThisFrame() ||
            usedJumpTime == availableJumpTime)
            {
                //descend if player let go of jump or jump time depleted
                JumpCancelled?.Invoke();
            }
        }
    }

    public override void FixedUpdatePlayerModule()
    {
        base.FixedUpdatePlayerModule();

        if (currentMoveStatus == MoveStatus.tethering)
            return;

        if (groundedCheckModule.IsGrounded)
        {
            GroundedMovement();
        }
        else
        {
            AerialMovement();
        }
    }

    private void GroundedMovement()
    {
        ApplyStandardMovement();
        ApplyJump();
    }

    private void AerialMovement()
    {
        ResetPostSwingMomentum();

        if (currentMoveStatus != MoveStatus.passive)
        {
            ApplyStandardMovement();
        }

        ManageCoyoteTime();
        ApplyJump();


    }

    private void ResetPostSwingMomentum()
    {
        if (inputVector != Vector2.zero)
        {
            currentMoveStatus = MoveStatus.jumping;
        }
    }

    private void ApplyJump()
    {
        //ascend if jump time available
        if (usedJumpTime < availableJumpTime)
        {
            Vector3 jumpVelocity = new Vector3(rbody.velocity.x, currentJumpSpeed, 0);

            //apply jump speed
            ApplyNewVelocityToRigidbody(jumpVelocity);

            currentJumpSpeed = Mathf.Lerp(currentJumpSpeed, 0, jumpSpeedDecayRate);
            usedJumpTime += Time.deltaTime;

        }

    }

    private void ApplyStandardMovement()
    {
        Vector3 moveVector = new Vector3(inputVector.x * moveSpeed, rbody.velocity.y, 0);
        ApplyNewVelocityToRigidbody(moveVector);
    }

    private void ManageCoyoteTime()
    {
        if (coyoteTimeUsed < availableCoyoteTime)
        {
            coyoteTimeUsed += Time.deltaTime;
        }
    }

    //Function to keep debugging easy
    public void ApplyNewVelocityToRigidbody(Vector3 newVel)
    {
        rbody.velocity = newVel;
    }

    private void OnJumpCancelled()
    {
        usedJumpTime = availableJumpTime;
        Vector3 JumpStopVelocity = new Vector3(rbody.velocity.x, 0, 0);
        ApplyNewVelocityToRigidbody(JumpStopVelocity);
    }

    public void ChangeCurrentMoveStatus(MoveStatus newMoveStatus)
    {
        currentMoveStatus = newMoveStatus;
    }

    public void SetJumpAmount(int amountOfJumpsRemaining)
    {
        jumpsUsed = availableJumps-amountOfJumpsRemaining;
    }


}
