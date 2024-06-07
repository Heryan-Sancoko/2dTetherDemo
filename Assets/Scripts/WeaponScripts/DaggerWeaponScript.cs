using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DaggerWeaponScript : WeaponScript
{
    [SerializeField] private float thirdAttackCooldown;
    [SerializeField] private float chargedAttackExtraDamage;
    [SerializeField] private LayerMask pushBackLayer;
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

    public void StartCustomChargeAttack()
    {
        isChargedAttack = true;
        StartAttack();
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
        if (rayColliders.Length == 0)
            return;

        float damage = 0;

        foreach (RaycastHit rayHit in rayColliders)
        {
            if (!hitColliders.Contains(rayHit.collider))
            {
                if (rayHit.collider.TryGetComponent(out EntityHealthModule healthModule))
                {
                    switch (AttackModule)
                    {
                        case PlayerAttackModule:
                            PlayerAttackModule PAM = AttackModule as PlayerAttackModule;
                            damage = currentDamage;
                            PAM.JumpOnHitEnemy();
                            break;
                        default:
                            break;
                    }

                    healthModule.TakeDamage(damage);
                    if (isChargedAttack)
                        healthModule.TakeDamage(chargedAttackExtraDamage);

                    if (healthModule.CurrentHealth <= 0 && firstKilledEnemy == null)
                    {
                        firstKilledEnemy = rayHit.transform;
                        firstKilledEnemyPosition = firstKilledEnemy.position;
                    }

                    hitColliders.Add(rayHit.collider);
                }
            }
        }

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

    public void ResetAttackSwing()
    {
        playerAttackModule.ResetAttackSwing();
    }

    public void ExtendCooldownForLastAttack()
    {
        playerAttackModule.SetCustomAttackCooldown(thirdAttackCooldown);
    }

    private IEnumerator ChainDash()
    {
        playerAttackModule.transform.position = firstKilledEnemyPosition;
        firstKilledEnemy = null;
        firstKilledEnemyPosition = Vector3.zero;
        playerAttackModule.Controller.ForceVelocityOverDuration(Vector3.zero, 0.5f, false, MoveStatus.airHop);
        playerAttackModule.DoCustomAttack(Constants.AnimationPrams.StartSpinAttack);

        RaycastHit[] nearbyEnemies = Physics.SphereCastAll(transform.position, 3, Vector3.forward, 1, pushBackLayer);

        if (nearbyEnemies.Length > 0)
        {
            foreach (RaycastHit enemy in nearbyEnemies)
            {
                if (enemy.transform.TryGetComponent(out StandardEnemyMovementModule enemyMoveModule))
                {
                    enemyMoveModule.LaunchEnemy((enemy.transform.position - transform.position).normalized, 20, 0.2f);
                }
            }
        }

        //yield return new WaitForSeconds(0.25f);
        //
        //
        //DoChargeAttack(new InputAction.CallbackContext());
        yield return null;
    }

}
