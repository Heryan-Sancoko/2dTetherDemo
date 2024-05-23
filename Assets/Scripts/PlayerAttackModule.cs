using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackModule : PlayerModule
{
    protected Rigidbody rbody;
    [SerializeField] protected WeaponScriptable weaponData;
    [SerializeField] protected WeaponScript weapon;
    [SerializeField] protected List<EnemyHealthModule> enemyHealthModuleList = new List<EnemyHealthModule>();
    [SerializeField] protected LayerMask enemyLayers;

    public float currentDamage;
    public float currentAttackCooldown;

    public override void AddController(EntityController newController)
    {
        base.AddController(newController);
        rbody = GetComponent<Rigidbody>();
        weapon = Instantiate(weaponData.weaponPrefab,transform).GetComponent<WeaponScript>();
        weapon.ConfigureLayerMask(enemyLayers);
    }

    public virtual void StartAttack()
    {
        //enable weapon
        weapon.gameObject.SetActive(true);
        weapon.transform.LookAt(transform.position + transform.forward);
        //trigger weapon animation
        weapon.WeaponAnim.SetTrigger("");
    }

    public virtual void PerformAttack()
    {
        weapon.PerformAttack();
    }

    public virtual void EndAttack()
    {
        //disable weapon
        //spwan particles
    }

    public virtual void ClearHitCollidersList()
    {
        weapon.ClearHitColliders();
    }

    public virtual void ChangeCurrentDamage(float newCurrentDamage)
    {
        currentDamage = newCurrentDamage;
    }

    public virtual void ResetCurrentDamage()
    {
        currentDamage = weaponData.baseDamage;
    }



    //clear the list of damaged enemies and start the attack
    //check the hitboxes for overlap
    //if there is an overlap, proc damage on the enemy
    //add enemy to a list of already damaged enemies

    //method to clear list of hit enemies for potential
    //multi-hit attacks
}
