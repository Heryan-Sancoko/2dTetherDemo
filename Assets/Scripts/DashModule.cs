using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DashModule : PlayerModule
{

    [SerializeField] private float availableDashTime;
    [SerializeField] private float dashCooldownDuration;
    [SerializeField] private float dashSpeed;
    private PlayerMovementModule playerMovementModule;
    private float usedDashTime;
    private float dashCooldownRemaining;
    private Vector3 dashDirection;


    public override void AddPlayerController(PlayerController newController)
    {
        base.AddPlayerController(newController);
        playerMovementModule = newController.GetModule<PlayerMovementModule>();

        InputManager.Instance.Dash.performed += OnDash;

        usedDashTime = availableDashTime;
    }

    private void OnDash(InputAction.CallbackContext callback)
    {
        if (usedDashTime < availableDashTime||
            dashCooldownRemaining > 0)
            return;

        playerController.SetCurrentMoveStatus(MoveStatus.dashing);

        Vector2 dashVector = playerMovementModule.InputVector;
        usedDashTime = 0;
        BecomeIntangible();

        switch (dashVector.x)
        {
            case > 0:
                dashDirection = Vector2.right;
                break;
            case < 0:
                dashDirection = Vector2.left;
                break;
            default:
                dashDirection = Vector2.zero;
                break;
        }
    }

    public override void UpdatePlayerModule()
    {
        base.UpdatePlayerModule();
        ManageDashCooldown();
    }

    public override void FixedUpdatePlayerModule()
    {
        base.FixedUpdatePlayerModule();
        ExecuteDash();
    }

    private void ExecuteDash()
    {
        if (usedDashTime < availableDashTime)
        {
            usedDashTime += Time.deltaTime;
            playerMovementModule.ApplyNewVelocityToRigidbody(dashDirection * dashSpeed);
            if (usedDashTime >= availableDashTime ||
                playerController.CurrentMoveStatus != MoveStatus.dashing)
            {
                OnEndDash();
            }
        }
    }

    private void OnEndDash()
    {
        BecomeTangible();
        usedDashTime = availableDashTime;
        dashCooldownRemaining = dashCooldownDuration;

        if (playerController.CurrentMoveStatus == MoveStatus.dashing)
        {
            playerMovementModule.ApplyNewVelocityToRigidbody(Vector3.zero);
            playerMovementModule.SelectBestMoveStatusFromContext();
        }

    }

    private void ManageDashCooldown()
    {
        if (dashCooldownRemaining > 0)
        {
            dashCooldownRemaining -= Time.deltaTime;
        }
    }


    private void BecomeIntangible()
    {
        gameObject.layer = Constants.Layers.PlayerIntangible;
    }

    private void BecomeTangible()
    {
        gameObject.layer = Constants.Layers.Player;
    }



}