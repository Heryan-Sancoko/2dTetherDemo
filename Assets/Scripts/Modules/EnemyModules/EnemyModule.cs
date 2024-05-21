using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyModule : EntityModule
{

    protected Rigidbody rbody;

    protected EnemyController enemyController;

    public override void AddController(EntityController newController)
    {
        base.AddController(newController);
        enemyController = newController as EnemyController;
        rbody = enemyController.Rbody;
    }

    public virtual void UpdateEnemyModule()
    {
    }

    public virtual void FixedUpdateEnemyModule()
    { 
    }
}
