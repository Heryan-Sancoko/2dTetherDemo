using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DashModule : PlayerModule
{

    [SerializeField] private float availableDashTime;
    [SerializeField] private float dashCooldownDuration;
    [SerializeField] private float dashSpeed;
    [Header("Debug values")]
    [SerializeField] private Material debugTangibleMat;
    [SerializeField] private Material debugIntangibleMat;
    [SerializeField] private MeshRenderer debugRenderer;
    private PlayerMovementModule playerMovementModule;
    [Header("invisible values")]
    [SerializeField]private float usedDashTime;
    [SerializeField]private float dashCooldownRemaining;
    private Vector3 dashDirection;

    public override void AddController(EntityController newController)
    {
        base.AddController(newController);
        playerMovementModule = playerController.GetModule<PlayerMovementModule>();

        InputManager.Instance.Dash.performed += OnDash;

        usedDashTime = availableDashTime;
    }

    private void OnDash(InputAction.CallbackContext callback)
    {
        if (usedDashTime < availableDashTime||
            dashCooldownRemaining > 0)
            return;

        switch (playerController.CurrentMoveStatus)
        {
            case MoveStatus.boosting:
                playerController.ForceVelocityOverDuration(Vector3.zero, 0, false, MoveStatus.dashing);
                break;
            case MoveStatus.tethering:
                playerController.CancelTether();
                break;
        }

        usedDashTime = 0;
        Vector2 dashVector = playerMovementModule.InputVector;
        playerMovementModule.JumpCancelled?.Invoke();
        playerController.SetCurrentMoveStatus(MoveStatus.dashing);
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

    private void LateUpdate()
    {
        switch (playerController.CurrentMoveStatus)
        {
            case MoveStatus.dashing:
                debugRenderer.material = debugIntangibleMat;
                break;
            default:
                debugRenderer.material = debugTangibleMat;
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
            //playerMovementModule.ApplyNewVelocityToRigidbody(Vector3.zero);
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


    public void BecomeIntangible()
    {
        gameObject.layer = Constants.Layers.PlayerIntangible;
    }

    public void BecomeTangible()
    {
        gameObject.layer = Constants.Layers.Player;
    }



}
