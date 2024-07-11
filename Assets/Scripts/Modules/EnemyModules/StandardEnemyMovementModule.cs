using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardEnemyMovementModule : EnemyModule
{
    private EnemyType currentEnemyType => enemyController.CurrentEnemyType;
    [SerializeField] private float movespeed;
    [SerializeField] private float turnSpeed;
    [SerializeField] private float attackRange;
    [SerializeField] private float aggroRange;
    [SerializeField] private bool forceIdle = true;
    private PlayerController player;
    private bool attackProcessHasStarted;

    private Vector3 launchDirection;
    private float launchSpeed;
    private float launchTime;
    private float currentLaunchTime;


    public override void AddController(EntityController newController)
    {
        base.AddController(newController);
        player = enemyController.playerController;
        transform.LookAt(player.transform, Vector3.up);
    }

    public override void UpdateEnemyModule()
    {
        base.UpdateEnemyModule();
    }

    public override void FixedUpdateEnemyModule()
    {
        if (forceIdle)
            return;

        base.FixedUpdateEnemyModule();

        switch (enemyController.CurrentEnemyStatus)
        {
            case EnemyStatus.attacking:
                break;
            case EnemyStatus.gettingLaunched:
                currentLaunchTime -= Time.deltaTime;
                ApplyNewVelocityToRigidbody(launchDirection * launchSpeed);
                if (currentLaunchTime <= 0)
                    enemyController.SetCurrentEnemyStatus(EnemyStatus.moving);
                break;
            case EnemyStatus.idle:
                if (enemyController.IsAggro)
                    enemyController.SetCurrentEnemyStatus(EnemyStatus.moving);
                break;
            case EnemyStatus.moving:

                if (!enemyController.IsAggro)
                {
                    enemyController.SetCurrentEnemyStatus(EnemyStatus.idle);
                    ApplyNewVelocityToRigidbody(Vector3.zero);
                    break;
                }

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
                        TurretMovement();
                        break;
                    default:
                        break;
                }
                break;
            case EnemyStatus.stunned:
                break;
            case EnemyStatus.windingUpAttack:
                break;
        }

    }

    public void ForceIdleToggle(bool toggleState)
    {
        forceIdle = toggleState;
    }

    //if homing, just move towards player at a flat speed.
    private void HomingMovement()
    {

        SlowRotateToPlayer();



        //transform.rotation = Quaternion.Euler(targetAngle, 90, 0);

        //Vector3 newVel = (player.transform.position - transform.position).normalized * movespeed;
        ApplyNewVelocityToRigidbody(transform.forward*movespeed);
    }

    public void SlowRotateToPlayer()
    {
        Vector3 targetDirection = player.transform.position - transform.position;
        Vector3 projectedTargetDirection = Vector3.ProjectOnPlane(targetDirection, Vector3.forward);

        Quaternion targetRotation = Quaternion.LookRotation(projectedTargetDirection, Vector3.forward);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turnSpeed);
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
            enemyController.StartAttackWindup();
        }

    }

    //if turret, don't move, just shoot.
    private void TurretMovement()
    {
        //I wonder if this really needs to exist?
        enemyController.StartAttackWindup();
    }

    public void ApplyNewVelocityToRigidbody(Vector3 newVelocity)
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
        enemyController.SetCurrentEnemyStatus(EnemyStatus.gettingLaunched);
    }

}
