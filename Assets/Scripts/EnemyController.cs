using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType { homing, spacing, turret }
public enum EnemyStatus {idle, moving, windingUpAttack, attackEndLag, attacking, gettingLaunched, stunned}

public class EnemyController : EntityController
{

    [SerializeField] private Rigidbody rbody;
    public Rigidbody Rbody => rbody;
    private EnemyManager enemyManager;
    public PlayerController playerController;

    //modules
    private StandardEnemyAttackModule attackModule;
    private StandardEnemyMovementModule moveModule;
    private EnemyHealthModule healthModule;

    public float CurrentHealth => healthModule.CurrentHealth;

    [SerializeField] private EnemyType currentEnemyType;
    public EnemyType CurrentEnemyType => currentEnemyType;

    private EnemyStatus currentEnemyStatus;
    public EnemyStatus CurrentEnemyStatus => currentEnemyStatus;

    public bool IsAggro => attackModule.IsAggro;

    [SerializeField] private List<EnemyModule> enemyModuleList = new List<EnemyModule>();

    // Start is called before the first frame update
    void Start()
    {
        healthModule = GetComponent<EnemyHealthModule>();
        enemyManager = EnemyManager.Instance;
        playerController = enemyManager.PlayerController;
        foreach (EnemyModule module in enemyModuleList)
        {
            module.AddController(this);
            switch (module)
            {
                case StandardEnemyMovementModule:
                    moveModule = (StandardEnemyMovementModule)module;
                    break;
                case StandardEnemyAttackModule:
                    attackModule = (StandardEnemyAttackModule)module;
                    break;
            }
        }
        
    }

    public void UpdateEnemyUnit()
    {
        foreach (EnemyModule module in enemyModuleList)
        {
            module.UpdateEnemyModule();
        }
    }

    public void FixedUpdateEnemyUnit()
    {
        foreach (EnemyModule module in enemyModuleList)
        {
            module.FixedUpdateEnemyModule();
        }
    }

    public void TakeDamage(float damage)
    {
        Vector3 enemyToPlayerDirection = (playerController.transform.position - transform.position).normalized;

        float playerDot = Vector3.Dot(transform.forward, enemyToPlayerDirection);

        damage = playerDot < 0 ?
            9999 :
            damage;

        if (damage == 9999)
        {
            Debug.LogError("CRIT");
        }
        healthModule.TakeDamage(damage);
    }

    public void SetCurrentEnemyStatus(EnemyStatus newStatus)
    {
        currentEnemyStatus = newStatus;
    }

    public void StartAttackWindup()
    {
        attackModule.StartWindupAttack();
    }

    public void SlowRotateToPlayer()
    {
        moveModule.SlowRotateToPlayer();
    }

    public void ApplyVelocityToRigidbody(Vector3 velocity)
    {
        moveModule.ApplyNewVelocityToRigidbody(velocity);   
    }


    public void ToggleForceEnemyIdle(bool toggleState)
    {
        moveModule.ForceIdleToggle(toggleState);
    }
}
