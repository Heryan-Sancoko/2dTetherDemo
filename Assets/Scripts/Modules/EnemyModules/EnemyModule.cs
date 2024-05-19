using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyModule : MonoBehaviour
{

    protected Rigidbody rbody;

    protected EnemyController enemyController;

    public virtual void AddEnemyController(EnemyController newEnemyController)
    {
        enemyController = newEnemyController;
        rbody = newEnemyController.Rbody;
    }

    public virtual void UpdateEnemyModule()
    {
    }

    public virtual void FixedUpdateEnemyModule()
    { 
    }
}
