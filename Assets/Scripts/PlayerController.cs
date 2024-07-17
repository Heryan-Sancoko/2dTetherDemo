using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveStatus { idle, jumping, floating, moving, tethering, dashing, attacking, airHop, passive, stunned, boosting };

public class PlayerController : EntityController
{
    [SerializeField] private List<PlayerModule> moduleList = new List<PlayerModule>();
    [SerializeField] private PlayerMovementModule playerMovementModule;
    [SerializeField] private DashModule playerDashModule;
    [SerializeField] private TetherModule playerTetherModule;
    [SerializeField] private GroundedCheckModule groundedCheckModule;
    public GroundedCheckModule GroundedModule => groundedCheckModule;
    private Rigidbody rbody;
    public Rigidbody Rbody => rbody;

    [SerializeField] private MoveStatus currentMoveStatus;
    public MoveStatus CurrentMoveStatus => currentMoveStatus;
    private Camera mainCam;

    void Awake()
    {
        mainCam = Camera.main;
        rbody = GetComponent<Rigidbody>();
        foreach (PlayerModule module in moduleList)
        {
            module.AddController(this);

            switch (module)
            {
                case PlayerMovementModule:
                    playerMovementModule = module as PlayerMovementModule;
                    break;
                case DashModule:
                    playerDashModule = module as DashModule;
                    break;
                case GroundedCheckModule:
                    groundedCheckModule = module as GroundedCheckModule;
                    break;
                case TetherModule:
                    playerTetherModule = module as TetherModule;
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (PlayerModule module in moduleList)
        {
            module.UpdatePlayerModule();
        }
    }

    private void FixedUpdate()
    {
        foreach (PlayerModule module in moduleList)
        {
            module.FixedUpdatePlayerModule();
        }
    }

    public Vector3 GetMouseDirection()
    {
        Vector3 mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return (mousePos - transform.position).normalized;
    }

    public void ApplyNewVelocityToRigidbody(Vector3 newVel)
    {
        playerMovementModule.ApplyNewVelocityToRigidbody(newVel);
    }

    public void JumpOnEnemyHit(Vector3 velocity, float seconds, bool enableHorizontalMovement)
    {
        playerMovementModule.JumpAfterHittingEnemy(velocity, seconds, enableHorizontalMovement);
    }

    public void ForceVelocityOverDuration(Vector3 velocity, float seconds, bool EnableHorizontalMovement, MoveStatus newMoveStatus)
    {
        playerMovementModule.ForceVelocityInDirectionOverDuration(velocity, seconds, EnableHorizontalMovement, newMoveStatus);
    }

    public void SetCurrentMoveStatus(MoveStatus newMoveStatus)
    {
        currentMoveStatus = newMoveStatus;
    }

    public void BecomeTangible()
    {
        playerDashModule.BecomeTangible();
    }

    public void BecomeIntangible()
    {
        playerDashModule.BecomeIntangible();
    }

    public void CancelTether()
    {
        playerTetherModule.CancelTether();
    }

    public void SetJumpAmount(int newJumpAmount)
    {
        playerMovementModule.SetJumpAmount(newJumpAmount);
    }

    public T GetModule<T>() where T : PlayerModule
    {
        foreach (var module in moduleList)
        {
            if (module is T)
            {
                return (T)module;
            }
        }
        return null;
    }
}
