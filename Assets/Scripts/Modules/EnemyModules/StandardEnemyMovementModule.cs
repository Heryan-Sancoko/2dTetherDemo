using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardEnemyMovementModule : EnemyModule
{
    [SerializeField] private EnemyType currentEnemyType;
    [SerializeField] private float movespeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float attackRange;
    [SerializeField] private float aggroRange;
    [SerializeField] private float attackWindupTime;
    [SerializeField] private EnemyStatus currentMoveStatus;
    private PlayerController player;
    private bool attackProcessHasStarted;
    private float currentAttackWindup;

    private Vector3 launchDirection;
    private float launchSpeed;
    private float launchTime;
    private float currentLaunchTime;


    public override void AddController(EntityController newController)
    {
        base.AddController(newController);
        player = EnemyManager.Instance.PlayerController;
        transform.LookAt(player.transform, Vector3.up);
    }

    public override void UpdateEnemyModule()
    {
        base.UpdateEnemyModule();
    }

    public override void FixedUpdateEnemyModule()
    {
        base.FixedUpdateEnemyModule();

        switch (currentMoveStatus)
        {
            case EnemyStatus.attacking:
                break;
            case EnemyStatus.gettingLaunched:
                currentLaunchTime -= Time.deltaTime;
                ApplyNewVelocityToRigidbody(launchDirection * launchSpeed);
                if (currentLaunchTime <= 0)
                    currentMoveStatus = EnemyStatus.idle;
                break;
            case EnemyStatus.idle:
                switch (currentEnemyType)
                {
                    case EnemyType.homing:
                        HomingMovement();
                        break;
                    case EnemyType.spacing:
                        SpacingMovement();
                        //WindUpAttack();
                        break;
                    case EnemyType.turret:
                        break;
                    default:
                        break;
                }
                break;
            case EnemyStatus.moving:
                break;
            case EnemyStatus.stunned:
                break;
            case EnemyStatus.windingUpAttack:
                break;
        }

    }

    //if homing, just move towards player at a flat speed.
    private void HomingMovement()
    {

        Vector3 targetDirection = player.transform.position - transform.position;
        Vector3 projectedTargetDirection = Vector3.ProjectOnPlane(targetDirection, Vector3.forward);

        Quaternion targetRotation = Quaternion.LookRotation(projectedTargetDirection, Vector3.forward);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed);



        //transform.rotation = Quaternion.Euler(targetAngle, 90, 0);

        //Vector3 newVel = (player.transform.position - transform.position).normalized * movespeed;
        ApplyNewVelocityToRigidbody(transform.forward*movespeed);
    }

    //if spacing, move to a distance from the player. Once there, do your attack.
    private void SpacingMovement()
    {
        if (Vector3.Distance(transform.position, player.transform.position) > attackRange)
        {
            HomingMovement();
        }
        else
        {
            StartAttackProcess();
        }

    }

    private void StartAttackProcess()
    {
        //wind up attack
        //aim at player
        //set aim timer
    }

    //if turret, don't move, just shoot.
    private void TurretMovement()
    {
        //I wonder if this really needs to exist?
    }

    private void ApplyNewVelocityToRigidbody(Vector3 newVelocity)
    {
        rbody.velocity = newVelocity;
        Vector3 fixedZPos = transform.position;
        fixedZPos.z = 0;
        transform.position = fixedZPos;
    }

    public void LaunchEnemy(Vector3 newLaunchDirection, float newLaunchSpeed, float newLaunchDuration)
    {
        launchDirection = newLaunchDirection;
        launchSpeed = newLaunchSpeed;
        launchTime = currentLaunchTime = newLaunchDuration;
        currentMoveStatus = EnemyStatus.gettingLaunched;
    }

}
