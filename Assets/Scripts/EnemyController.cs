using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    [SerializeField] private Rigidbody rbody;
    public Rigidbody Rbody => rbody;
    private EnemyManager enemyManager;

    private EnemyType currentEnemyType;
    public EnemyType CurrentEnemyType => currentEnemyType;

    [SerializeField] private List<EnemyModule> enemyModuleList = new List<EnemyModule>();

    // Start is called before the first frame update
    void Start()
    {
        enemyManager = EnemyManager.Instance;
        foreach (EnemyModule module in enemyModuleList)
        {
            module.AddEnemyController(this);
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



}
