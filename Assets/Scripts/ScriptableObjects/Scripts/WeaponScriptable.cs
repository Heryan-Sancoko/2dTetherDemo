using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "WeaponScriptable", menuName = "ScriptableObjects/WeaponScriptable")]
public class WeaponScriptable : ScriptableObject
{
    public GameObject weaponPrefab;
    public float baseDamage;
    public float baseAttackCooldown;
}
