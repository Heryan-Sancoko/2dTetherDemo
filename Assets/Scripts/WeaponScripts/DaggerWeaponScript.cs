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

    public override void StartAttack()
    {
        base.StartAttack();
    }

    public override void StartChargeAttack(InputAction.CallbackContext callback)
    {
        base.StartChargeAttack(callback);
        if (playerAttackModule != null)
        {
            playerAttackModule.OnHeldStarted(callback);
        }
    }

    public override void DoChargeAttack(InputAction.CallbackContext callback)
    {
        isChargedAttack = true;
        base.DoChargeAttack(callback);
        if (playerAttackModule != null)
        {
            //do charge animation
            playerAttackModule.OnHeldAttack(callback);
            //change MoveStatus
            //
        }
    }

    public override void EndAttack()
    {
        base.EndAttack();
        isChargedAttack = false;
    }

    protected override void AddNewCollidersToHitList(RaycastHit[] rayColliders)
    {
        base.AddNewCollidersToHitList(rayColliders);

        if (!isChargedAttack)
            return;

        if (playerAttackModule != null)
        {
            if (firstKilledEnemy!=null)
            {
                playerAttackModule.Controller.BecomeIntangible();
                StartCoroutine(ChainDash());
                EndAttack();
            }
        }
    }

    private IEnumerator ChainDash()
    {
        playerAttackModule.transform.position = firstKilledEnemyPosition;
        firstKilledEnemy = null;
        firstKilledEnemyPosition = Vector3.zero;
        playerAttackModule.Controller.ForceVelocityOverDuration(Vector3.zero, 0.25f, false, MoveStatus.airHop);
        playerAttackModule.DoCustomAttack(Constants.AnimationPrams.StartSpinAttack);
        //yield return new WaitForSeconds(0.25f);
        //
        //
        //DoChargeAttack(new InputAction.CallbackContext());
        yield return null;
    }

}
