using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScript : MonoBehaviour
{
    [SerializeField] private List<Collider> weaponHitboxes = new List<Collider>();
    private List<Collider> hitColliders = new List<Collider>();

}
