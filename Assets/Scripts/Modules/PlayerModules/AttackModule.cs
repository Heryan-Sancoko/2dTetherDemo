using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackModule : PlayerModule
{
    private Rigidbody rbody;
    [SerializeField] private WeaponScriptable weapon;

    public override void AddPlayerController(PlayerController newController)
    {
        base.AddPlayerController(newController);
        rbody = GetComponent<Rigidbody>();
    }
}
