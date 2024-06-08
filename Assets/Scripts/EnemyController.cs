using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType { homing, spacing, turret }
public enum EnemyStatus {idle, moving, windingUpAttack, attacking, gettingLaunched, stunned}

public class EnemyController : EntityController
{

    [SerializeField] private Rigidbody rbody;
    public Rigidbody Rbody => rbody;
    private EnemyManager enemyManager;
    public PlayerController playerController;

    private EnemyType currentEnemyType;
    public EnemyType CurrentEnemyType => currentEnemyType;

    private EnemyStatus currentEnemyStatus;
    public EnemyStatus CurrentEnemyStatus => currentEnemyStatus;

    [SerializeField] private List<EnemyModule> enemyModuleList = new List<EnemyModule>();

    // Start is called before the first frame update
    void Start()
    {
        enemyManager = EnemyManager.Instance;
        playerController = enemyManager.PlayerController;
        foreach (EnemyModule module in enemyModuleList)
        {
            module.AddController(this);
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

    public void SetCurrentEnemyStatus(EnemyStatus newStatus)
    {
        currentEnemyStatus = newStatus;
    }



}
