using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardEnemyAttackModule : EnemyModule
{
    private PlayerController player;
    private float attackWindupTime;
    private float currentAttackWindup;
    private Vector3 attackDirection;
    private float attackTimer;
    private float timeUntilNextAttack;
    private float aggroRange;
    private bool isAggro;

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
    }

    private void CheckIfAggro()
    {
        isAggro = Vector3.Distance(player.transform.position, transform.position) < aggroRange;
    }
}
