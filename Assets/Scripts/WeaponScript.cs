using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class WeaponScript : MonoBehaviour
{
    [SerializeField] protected List<SphereCollider> weaponHitboxes = new List<SphereCollider>();
    [SerializeField] protected LayerMask layerMask;
    [SerializeField] protected Animator weaponAnim;
    [SerializeField] protected List<Collider> hitColliders = new List<Collider>();
    public Animator WeaponAnim => weaponAnim;
    [SerializeField] protected EntityModule AttackModule;
    protected bool AttackStarted = false;
    protected float currentDamage;

    public virtual void ConfigureWeapon(LayerMask mirrorMask, EntityModule newAttackModule)
    {
        layerMask = mirrorMask;
        AttackModule = newAttackModule;

        switch (newAttackModule)
        {
            case PlayerAttackModule:
                currentDamage = (newAttackModule as PlayerAttackModule).currentDamage;
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        if (AttackStarted)
            PerformAttack();
    }

    public virtual void PerformAttack()
    {
        foreach (Collider hitbox in weaponHitboxes)
        {
            RaycastHit[] newSphereColliders = Physics.SphereCastAll(hitbox.bounds.center, hitbox.bounds.size.x, Vector3.forward, 3, layerMask);
            AddNewCollidersToHitList(newSphereColliders);
        }
    }

    public virtual void StartAttack()
    {
        AttackStarted = true;
        hitColliders.Clear();
    }

    public virtual void EndAttack()
    {
        hitColliders.Clear();
        AttackStarted = false;
    }

    public virtual void ConcealWeapon()
    {
        gameObject.SetActive(false);
    }

    protected void AddNewCollidersToHitList(RaycastHit[] rayColliders)
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
                    hitColliders.Add(rayHit.collider);
                }
            }
        }
    }

    public virtual void ClearHitColliders()
    {
        hitColliders.Clear();
    }

}
