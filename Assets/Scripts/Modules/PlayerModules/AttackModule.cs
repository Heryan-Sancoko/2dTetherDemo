using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackModule : PlayerModule
{
    private Rigidbody rbody;
    [SerializeField] private WeaponScriptable weapon;

    public override void AddController(EntityController newController)
    {
        base.AddController(newController);
        rbody = GetComponent<Rigidbody>();
    }


    //clear the list of damaged enemies and start the attack
    //check the hitboxes for overlap
    //if there is an overlap, proc damage on the enemy
    //add enemy to a list of already damaged enemies

    //method to clear list of hit enemies for potential
    //multi-hit attacks
}
