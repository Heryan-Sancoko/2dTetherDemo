using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    [SerializeField] protected List<Collider> weaponHitboxes = new List<Collider>();
    [SerializeField] protected Animator weaponAnim;
    public Animator WeaponAnim => weaponAnim;
    protected List<Collider> hitColliders = new List<Collider>();

    public virtual void PerformAttack()
    {
        
    }

    public virtual void ClearHitColliders()
    {
        hitColliders.Clear();
    }

}
