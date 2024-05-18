using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType {homing, spacing, turret}
public class StandardEnemyMovementModule : MonoBehaviour
{

    [SerializeField] private Rigidbody rbody;
    public Rigidbody Rbody => rbody;

    //if homing, just move towards player at a flat speed.
    private void HomingMovement()
    {
        
    }

    //if spacing, move to a distance from the player. Once there, do your attack.

    //if turret, don't move, just shoot.

}
