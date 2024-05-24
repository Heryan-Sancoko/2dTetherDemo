using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyHealthModule : EntityHealthModule
{

    private void Start()
    {
        KillEntity += OnKillEnemy;
    }

    protected virtual void OnKillEnemy()
    {
        gameObject.SetActive(false);
    }

    public override void TakeDamage(float damageAmount)
    {
        base.TakeDamage(damageAmount);

        currentHealth -= damageAmount;

        if (currentHealth <= 0)
        {
            KillEntity?.Invoke();
        }
    }

    public override void GainHealth(float healingAmount)
    {

    }
}
