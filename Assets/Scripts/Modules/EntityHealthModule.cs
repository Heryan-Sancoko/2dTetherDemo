using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EntityHealthModule : EntityModule
{
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float currentHealth;
    public UnityAction KillEntity;

    public virtual void TakeDamage(float damageAmount)
    {

    }

    public virtual void GainHealth(float healingAmount)
    {
        
    }

}
