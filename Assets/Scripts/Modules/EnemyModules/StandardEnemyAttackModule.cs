using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardEnemyAttackModule : EnemyModule
{
    private PlayerController player;
    [SerializeField] private float attackWindupTime;
    private float currentAttackWindup;
    [SerializeField] private float attackEndLagTime;
    private float currentAttackEndLagRemaining;
    [SerializeField] private float lockInAttackThreshold;
    private Vector3 attackDirection;
    private float attackCooldown;
    private float attackCooldownRemaining;
    [SerializeField] private float aggroRange;
    [SerializeField] private float lungeSpeed;
    [SerializeField] private float lungeDistance;
    private float currentLungeDistance;
    private Vector3 attackDestination;
    private Vector3 originalLungePosition;
    private bool isAggro;
    public bool IsAggro => isAggro;

    [SerializeField] private float sphereCastRadius;
    [SerializeField] private LayerMask sphereCastLayerMask;



    public override void AddController(EntityController newController)
    {
        base.AddController(newController);
        player = enemyController.playerController;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void UpdateEnemyModule()
    {
        base.UpdateEnemyModule();
        CheckIfAggro();

        if (enemyController.CurrentEnemyStatus == EnemyStatus.windingUpAttack || enemyController.CurrentEnemyStatus == EnemyStatus.attacking || enemyController.CurrentEnemyStatus == EnemyStatus.attackEndLag)
        {
            switch (enemyController.CurrentEnemyType)
            {
                case EnemyType.homing:
                    break;
                case EnemyType.spacing:
                    SpacingEnemyAttackProcedure();
                    break;
                case EnemyType.turret:
                    break;
            }
        }
    }

    private void SpacingEnemyAttackProcedure()
    {
        switch (enemyController.CurrentEnemyStatus)
        {
            case EnemyStatus.windingUpAttack:

                //count down timer
                currentAttackWindup -= Time.deltaTime;
                //display indicator
                enemyController.ApplyVelocityToRigidbody(Vector3.zero);

                if (currentAttackWindup > lockInAttackThreshold)
                {
                    //follow player direction
                    transform.LookAt(player.transform.position, Vector3.up);
                }
                else if (currentAttackWindup <= lockInAttackThreshold && currentAttackWindup > 0)
                {
                    //stop following after certain time

                }
                else
                {
                    //once timer runs out, attack
                    enemyController.SetCurrentEnemyStatus(EnemyStatus.attacking);

                    if (Physics.SphereCast(transform.position, 2, transform.forward, out RaycastHit hitinfo, lungeDistance, sphereCastLayerMask))
                    {
                        attackDestination = hitinfo.point + (hitinfo.normal);
                        currentLungeDistance = Vector3.Distance(transform.position, attackDestination);
                    }
                    else
                    {
                        attackDestination = transform.position + (transform.forward * (lungeDistance));
                        currentLungeDistance = lungeDistance;
                    }

                    originalLungePosition = transform.position;
                }



                break;
            case EnemyStatus.attacking:

                enemyController.ApplyVelocityToRigidbody(transform.forward * lungeSpeed);
                if (Vector3.Distance(transform.position, originalLungePosition) >= Vector3.Distance(originalLungePosition, originalLungePosition + (transform.forward * currentLungeDistance)))
                {
                    attackCooldownRemaining = attackCooldown;
                    currentAttackEndLagRemaining = attackEndLagTime;
                    enemyController.SetCurrentEnemyStatus(EnemyStatus.attackEndLag);
                    enemyController.ApplyVelocityToRigidbody(Vector3.zero);

                }
                //trigger attack animation
                //animation should toggle hitboxes on
                //have a function that checks hitboxes we overlap
                //if it is a player hitbox, add player to list of things hit
                //on attack end, have the animation trigger a function that returns us to spacing and reseting the attack timer

                break;

            case EnemyStatus.attackEndLag:

                currentAttackEndLagRemaining -= Time.deltaTime;

                if (currentAttackEndLagRemaining <= 0)
                {
                    enemyController.SetCurrentEnemyStatus(EnemyStatus.moving);
                }

                break;


            default:
                break;
        }
    }

    private void CheckIfAggro()
    {
        isAggro = Vector3.Distance(player.transform.position, transform.position) < aggroRange;
    }

    public void StartWindupAttack()
    {
        if (attackCooldownRemaining > 0)
            attackCooldownRemaining -= Time.deltaTime;

        if (attackCooldownRemaining <= 0)
        {
            enemyController.SetCurrentEnemyStatus(EnemyStatus.windingUpAttack);
            currentAttackWindup = attackWindupTime;
        }
    }
}
