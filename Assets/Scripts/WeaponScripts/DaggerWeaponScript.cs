using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DaggerWeaponScript : WeaponScript
{
    private bool isChargedAttack = false;

    public override void ConfigureWeapon(LayerMask mirrorMask, EntityModule newAttackModule)
    {
        base.ConfigureWeapon(mirrorMask, newAttackModule);
        InputManager.Instance.Attack.canceled += StartChargeAttack;
        InputManager.Instance.HeldAttack.performed += DoChargeAttack;
    }

    public override void StartChargeAttack(InputAction.CallbackContext callback)
    {
        base.StartChargeAttack(callback);
        if (playerAttackModule != null)
        {
            playerAttackModule.OnHeldStarted(callback);
            isChargedAttack = true;
        }
    }

    public override void DoChargeAttack(InputAction.CallbackContext callback)
    {
        base.DoChargeAttack(callback);
        if (playerAttackModule != null)
        {
            //do charge animation
            playerAttackModule.OnHeldAttack(callback);
            //change MoveStatus
            //
        }
    }

    protected override void AddNewCollidersToHitList(RaycastHit[] rayColliders)
    {
        base.AddNewCollidersToHitList(rayColliders);

        if (!isChargedAttack)
            return;

        if (playerAttackModule != null && playerAttackModule.Controller.CurrentMoveStatus == MoveStatus.airHop && isChargedAttack)
        {
            if (firstKilledEnemy!=null)
            {
                StartCoroutine(ChainDash());
            }
        }
    }

    private IEnumerator ChainDash()
    {
        //playerAttackModule.transform.position = firstKilledEnemy.position;
        playerAttackModule.Controller.ForceVelocityOverDuration(Vector3.zero, 0.25f, false, MoveStatus.airHop);
        yield return new WaitForSeconds(0.25f);

        playerAttackModule.Controller.BecomeIntangible();
        firstKilledEnemy = null;
        DoChargeAttack(new InputAction.CallbackContext());
        yield return null;
    }

}
