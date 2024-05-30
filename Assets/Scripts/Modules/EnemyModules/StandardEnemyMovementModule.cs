using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardEnemyMovementModule : EnemyModule
{
    [SerializeField] private EnemyType currentEnemyType;
    [SerializeField] private float movespeed;
    [SerializeField] private float attackRange;
    [SerializeField] private float aggroRange;
    [SerializeField] private float attackWindupTime;
    [SerializeField] private EnemyStatus currentMoveStatus;
    private PlayerController player;
    private bool attackProcessHasStarted;
    private float currentAttackWindup;


    public override void AddController(EntityController newController)
    {
        base.AddController(newController);
        player = EnemyManager.Instance.PlayerController;
    }

    public override void UpdateEnemyModule()
    {
        base.UpdateEnemyModule();
    }

    public override void FixedUpdateEnemyModule()
    {
        base.FixedUpdateEnemyModule();

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
    }

    //if homing, just move towards player at a flat speed.
    private void HomingMovement()
    {
        Vector3 newVel = (player.transform.position - transform.position).normalized * movespeed;
        ApplyNewVelocityToRigidbody(newVel);
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
    }


}
