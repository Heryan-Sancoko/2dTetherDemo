using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardEnemyMovementModule : EnemyModule
{
    [SerializeField] private EnemyType currentEnemyType;
    [SerializeField] private float movespeed;
    private PlayerController player;


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
