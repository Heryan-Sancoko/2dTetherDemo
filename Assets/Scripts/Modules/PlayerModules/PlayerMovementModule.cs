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
    public float TopSpeed => topSpeed;
    private Rigidbody rbody;


    [Header("Jump variables")]
    [SerializeField] private float jumpSpeed;
    [SerializeField, Tooltip("How quickly the jumpspeed decays to 0. Lerp.")] private float jumpSpeedDecayRate;
    [SerializeField] private float availableJumpTime;
    [SerializeField] private int availableJumps;
    [SerializeField] private float jumpCancelTime;
    [SerializeField] private float availableCoyoteTime = 0.2f;
    [SerializeField] private int jumpsUsed;
    [SerializeField] private float SlowFallSpeed;
    [SerializeField] private float FastFallSpeed;
    [SerializeField] private float airDeceleration;

    private float currentJumpSpeed;
    private float usedJumpTime;
    private float coyoteTimeUsed;

    [SerializeField] private float forcedMovementTimer;
    private Vector3 forcedVelocity;
    private bool allowHorizontalMovement;


    private InputManager inputmanager;
    private GroundedCheckModule groundedCheckModule;

    //cancel jump when hitting a roof or enemy contact
    public UnityAction JumpCancelled;

    private const float maxFastFallValue = -0.7f;
    private const float minSlowFallValue = 0.7f;



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

        usedJumpTime = availableJumpTime;
    }

    private void LateUpdate()
    {
        //ClampVelocity();
    }

    private void ClampVelocity()
    {
        rbody.velocity = Vector3.ClampMagnitude(rbody.velocity, topSpeed);
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
            //if (groundedCheckModule.IsGrounded)
            //    transform.position = groundedCheckModule.HitPoint + (Vector3.up * playerDiameter);

            //don't consume a jump if we are grounded or have coyote time
            if (!groundedCheckModule.IsGrounded && coyoteTimeUsed >= availableCoyoteTime)
                jumpsUsed++;

            //consume coyote time when we jump
            //and begin jumping
            coyoteTimeUsed = availableCoyoteTime;
            usedJumpTime = 0;
            currentJumpSpeed = jumpSpeed;

            if (playerController.CurrentMoveStatus == MoveStatus.boosting)
            {
                forcedMovementTimer = 0;
            }

            playerController.SetCurrentMoveStatus(MoveStatus.jumping);
        }
    }

    private void OnLand()
    {
        jumpsUsed = 0;
        coyoteTimeUsed = 0;

        if (playerController.CurrentMoveStatus != MoveStatus.dashing)
        SelectBestMoveStatusFromContext();
    }

    public void SelectBestMoveStatusFromContext()
    {
        if (!groundedCheckModule.IsGrounded)
        {
            playerController.SetCurrentMoveStatus(MoveStatus.jumping);
            return;
        }

        MoveStatus newMoveStatus = inputVector == Vector2.zero ?
            MoveStatus.idle :
            MoveStatus.moving;

        playerController.SetCurrentMoveStatus(newMoveStatus);
    }

    public override void UpdatePlayerModule()
    {
        base.UpdatePlayerModule();

        if (usedJumpTime < availableJumpTime)
        {
            if (InputVector.y < maxFastFallValue)
            {
                FastFall();
                return;
            }


            //if jump is released early, cancel ascension and begin descent
            if (inputmanager.Jump.WasReleasedThisFrame() ||
            usedJumpTime >= availableJumpTime ||
            playerController.CurrentMoveStatus != MoveStatus.jumping)
            {
                //descend if player let go of jump or jump time depleted
                JumpCancelled?.Invoke();
            }
        }

        if (playerController.CurrentMoveStatus == MoveStatus.dashing)
        {
            FastFall();
        }

        if (forcedMovementTimer >= 0)
        {
            forcedMovementTimer -= Time.deltaTime;
            {
                if (playerController.CurrentMoveStatus == MoveStatus.attacking)
                {
                    ApplyNewVelocityToRigidbody(Vector3.zero);
                }

                //if (playerController.CurrentMoveStatus == MoveStatus.boosting)
                //{
                //    if (forcedMovementTimer<= 0 && inputVector.x != 0)
                //    SelectBestMoveStatusFromContext();
                //}
            }
        }
        else
        {
            if (playerController.CurrentMoveStatus == MoveStatus.boosting)
            {
                if (inputVector.x != 0)
                    SelectBestMoveStatusFromContext();
            }
        }
    }

    public override void FixedUpdatePlayerModule()
    {
        base.FixedUpdatePlayerModule();

        switch (playerController.CurrentMoveStatus)
        {
            case MoveStatus.airHop:
                //do JUMP things if in the air
                //do MOVE things if on the ground
                //nothing changes, but you are in the air accelerating upwards slightly?
                //once AIRHOP is over, go back to MOVE or JUMP
                break;
            case MoveStatus.attacking:
                //do JUMP things if in the air
                //do MOVE things if on the ground
                //nothing changes, but your status is now attacking?
                //once ATTACKING is over, go back to MOVE or JUMP
                break;
            case MoveStatus.boosting:
                //if we are told to jump, JUMP <===Put in update
                //dash in the direction of the boost area
                //check distance traveled
                //count down boost timer
                break;
            case MoveStatus.dashing:
                //dash in the preserved input direction
                //become intangible
                //do other dash things
                //count down dash timer
                break;
            case MoveStatus.floating:
                //does this need to be here?
                break;
            case MoveStatus.idle:
                //MOVE if input and grounded <===Put in update
                //jump if JUMPing or in the air <===Put in update
                break;
            case MoveStatus.jumping:
                //if we are on the ground, MOVE <===Put in update
                //if we input a dash, DASH <===Put in update
                //handle aerial movement
                break;
            case MoveStatus.moving:
                //if we are told to jump, JUMP <===Put in update
                //if we input a dash, DASH <===Put in update
                //handle grounded movements
                break;
            case MoveStatus.passive:
                //why is this here again?
                break;
            case MoveStatus.stunned:
                //block inputs
                //play stun animation
                //apply potential knockback
                //countdown the stun timer
                break;
            case MoveStatus.tethering:
                //refer to tether script
                return;
            default:
                break;
        }

        StandartMovement();

    }

    private void StandartMovement()
    {

        if (groundedCheckModule.IsGrounded)
        {
            GroundedMovement();
        }
        else
        {
            AerialMovement();
        }

        if (playerController.CurrentMoveStatus != MoveStatus.boosting)
        ClampVelocity();
    }

    private void BoostMovement()
    {
        
    }

    private void GroundedMovement()
    {
        ApplyStandardMovement();
        if (playerController.CurrentMoveStatus== MoveStatus.jumping)
        ApplyJump();
    }

    private void AerialMovement()
    {
        ResetPostSwingMomentum();

        if (playerController.CurrentMoveStatus != MoveStatus.passive)
        {
            InputBasedAirMovement();
        }

        ManageCoyoteTime();
        ApplyJump();


    }

    private void InputBasedAirMovement()
    {
        if (playerController.CurrentMoveStatus == MoveStatus.boosting)
        {
            ApplyAirMovement();
            return;
        }


        switch (inputVector.y)
        {
            case >= minSlowFallValue:
                SlowFall();
                break;
            case <= maxFastFallValue:
                FastFall();
                    break;
            default:
                ApplyAirMovement();
                break;
        }
    }

    private void SlowFall()
    {
        Vector3 moveVector = new Vector3(inputVector.x * AdjustedMovespeed(), Mathf.Clamp(rbody.velocity.y, -SlowFallSpeed, rbody.velocity.y), 0);
        ApplyNewVelocityToRigidbody(moveVector);
    }

    private void FastFall()
    {
        if (InputVector.y < maxFastFallValue)
        {
            if (playerController.CurrentMoveStatus == MoveStatus.jumping)
            {
                JumpCancelled?.Invoke();
            }
            else
                SelectBestMoveStatusFromContext();

            float fallspeed = FastFallSpeed;
            if (inputmanager.Jump.phase == InputActionPhase.Performed)
            {
                fallspeed = 0;
            }

            Vector3 moveVector = new Vector3(inputVector.x * AdjustedMovespeed(), fallspeed, 0);
            ApplyNewVelocityToRigidbody(moveVector);

        }
    }

    private void ResetPostSwingMomentum()
    {
        if (playerController.CurrentMoveStatus == MoveStatus.passive && inputVector != Vector2.zero)
        {
            playerController.SetCurrentMoveStatus(MoveStatus.jumping);
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
        Vector3 moveVector = new Vector3(inputVector.x * AdjustedMovespeed(), rbody.velocity.y, 0);
        //moveVector = ForceAirMovespeed(moveVector);
        ApplyNewVelocityToRigidbody(moveVector);
    }

    private float AdjustedMovespeed()
    {
        if (Vector3.Dot(rbody.velocity.normalized, InputVector) > 0f &&
            Mathf.Abs(rbody.velocity.x) > moveSpeed)
        {
            return Mathf.Abs(rbody.velocity.x);
        }

        return moveSpeed;
    }

    private void ApplyAirMovement()
    {
        float AdjustedInputVectorX = inputVector.x;

        if (playerController.CurrentMoveStatus == MoveStatus.boosting)
        {
            AdjustedInputVectorX = rbody.velocity.x;
        }
        else
        {
            AdjustedInputVectorX = inputVector.x * AdjustedMovespeed();
        }

        Vector3 moveVector = new Vector3(AdjustedInputVectorX, rbody.velocity.y, 0);

        //moveVector = ForceAirMovespeed(moveVector);

        ApplyNewVelocityToRigidbody(moveVector);

        if (inputmanager.Jump.phase == InputActionPhase.Performed && playerController.CurrentMoveStatus != MoveStatus.boosting)
            SlowFall();
    }

    public void JumpAfterHittingEnemy(Vector3 newVelocity, float duration, bool enableHorizontalMovement)
    {
        ForceVelocityInDirectionOverDuration(newVelocity, duration, enableHorizontalMovement, MoveStatus.airHop);
        SetJumpAmount(1);
    }

    private Vector3 ForceAirMovespeed(Vector3 oldMovespeed)
    {
        if (forcedMovementTimer >= 0)
        {
            oldMovespeed = forcedVelocity;
            if (allowHorizontalMovement)
            {
                if (inputVector.x!=0)
                oldMovespeed = new Vector3(inputVector.x * AdjustedMovespeed(), forcedVelocity.y, 0);
                else
                    oldMovespeed = forcedVelocity;

                //if (Vector3.Dot(rbody.velocity.normalized, InputVector) > 0.5f)
                //{
                //    oldMovespeed.x = InputVector.x * rbody.velocity.x;
                //}
                //else
                //    oldMovespeed.x = inputVector.x * moveSpeed;
            }

            //if (forcedMovementTimer <= 0)
            //{
            //    Debug.LogError("does this even proc?");
            //    //when we implement knockback on the player
            //    //check if we need to reset speed to zero here
            //    //and apply appropriate speed
            //    SelectBestMoveStatusFromContext();
            //    playerController.BecomeTangible();
            //}

            return oldMovespeed;
        }

        return oldMovespeed;
    }

    private void ManageCoyoteTime()
    {
        if (coyoteTimeUsed < availableCoyoteTime)
        {
            coyoteTimeUsed += Time.deltaTime;
        }
    }

    public void ForceVelocityInDirectionOverDuration(Vector3 newVelocity, float duration, bool enableHorizontalMovement, MoveStatus newMoveStatus)
    {
        forcedVelocity = newVelocity;
        forcedMovementTimer = duration;
        allowHorizontalMovement = enableHorizontalMovement;
        playerController.SetCurrentMoveStatus(newMoveStatus);
    }

    //Function to keep debugging easy
    public void ApplyNewVelocityToRigidbody(Vector3 newVel)
    {
        newVel = ForceAirMovespeed(newVel);
        rbody.velocity = newVel;
    }

    private void OnJumpCancelled()
    {
        if (!groundedCheckModule.IsGrounded && playerController.CurrentMoveStatus != MoveStatus.boosting)
            playerController.SetCurrentMoveStatus(MoveStatus.jumping);

        usedJumpTime = availableJumpTime;
        //Vector3 JumpStopVelocity = new Vector3(rbody.velocity.x, 0, 0);

        float horizontalSpeed = rbody.velocity.x;
        if (InputVector.x != 0)
            horizontalSpeed = InputVector.x * AdjustedMovespeed();

        Vector3 JumpStopVelocity = new Vector3(horizontalSpeed, rbody.velocity.y, 0);
        ApplyNewVelocityToRigidbody(JumpStopVelocity);
    }

    public void SetJumpAmount(int amountOfJumpsRemaining)
    {
        jumpsUsed = availableJumps-amountOfJumpsRemaining;
    }

}
