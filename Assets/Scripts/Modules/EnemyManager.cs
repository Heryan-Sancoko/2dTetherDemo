using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{

    //hold a reference to the player
    [SerializeField] private PlayerController playerController;
    public PlayerController PlayerController => playerController;

    [SerializeField] private List<EnemyController> EnemyList = new List<EnemyController>();


    //update each enemy
    private void Update()
    {
        foreach (EnemyController enemy in EnemyList)
        {
            if (enemy.gameObject.activeSelf)
                enemy.UpdateEnemyUnit();
        }
    }

    private void FixedUpdate()
    {
        foreach (EnemyController enemy in EnemyList)
        {
            if (enemy.gameObject.activeSelf)
                enemy.FixedUpdateEnemyUnit();
        }
    }


}
