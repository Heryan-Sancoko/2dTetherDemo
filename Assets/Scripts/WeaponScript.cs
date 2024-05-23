using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    [SerializeField] protected List<SphereCollider> weaponHitboxes = new List<SphereCollider>();
    [SerializeField] protected LayerMask layerMask;
    [SerializeField] protected Animator weaponAnim;
    public Animator WeaponAnim => weaponAnim;
    protected List<Collider> hitColliders = new List<Collider>();
    protected EntityModule AttackModule;

    public virtual void ConfigureLayerMask(LayerMask mirrorMask)
    {
        layerMask = mirrorMask;
    }

    public virtual void PerformAttack()
    {
        foreach (Collider hitbox in weaponHitboxes)
        {
            RaycastHit[] newSphereColliders = Physics.SphereCastAll(hitbox.bounds.center, hitbox.bounds.size.x, Vector3.zero, 0, layerMask);
            AddNewCollidersToHitList(newSphereColliders);
        }
    }

    protected void AddNewCollidersToHitList(RaycastHit[] rayColliders)
    {
        if (rayColliders.Length == 0)
            return;

        foreach (RaycastHit rayHit in rayColliders)
        {
            if (!hitColliders.Contains(rayHit.collider))
            {
                if (rayHit.collider.TryGetComponent(out EntityHealthModule healthModule))
                {
                    float damage = AttackModule is PlayerAttackModule ? (AttackModule as PlayerAttackModule).currentDamage : 0;
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
