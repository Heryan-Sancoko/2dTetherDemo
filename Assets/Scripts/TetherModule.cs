using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//THIS SCRIPT IS STILL A WIP. DISABLE IT TO RUN NORMALLY.

public class TetherModule : PlayerModule
{

    [SerializeField] private Transform tetherAnchor;

    private List<Vector3> tetherNodes = new List<Vector3>();

    private bool isUsingMouse = false;
    private Camera mcam;

    [SerializeField] private Vector3 tetherDirection;

    private void Start()
    {
        InputManager.Instance.Tether.performed += OnTether;
        InputManager.Instance.Tether.canceled += OnTetherCancelled;

        isUsingMouse = Gamepad.current == null;
    }

    public override void UpdatePlayerModule()
    {
        base.UpdatePlayerModule();

        if (isUsingMouse)
        {
            Vector3 mousePos = mcam.ScreenToWorldPoint(Input.mousePosition);
            mousePos.z = 0;
            tetherDirection = (transform.position - mousePos).normalized;
        }
    }

    private void OnTether(InputAction.CallbackContext callback)
    {
        if (InputManager.Instance.Tether.WasPressedThisFrame())
        {
            Debug.LogError("TETHER DOWN");

        }
    }

    //cast ray out
    //get hit point
    //anchor is placed on hit position
    //anchor becomes a child of the terrain

    //player moves towards anchor
    //if player reaches anchor, player remains in place until tether is released
    //if player inputs a direction...

    //find the direction of input, compare to a dot product vs tether direction
    //begin swinging player in direction by...
    //swingAssist looks at anchor, apply positive or negative x speed
    //confine the player to a certain distance by taking the speed of which we are moving away from the anchor and applying an inverse force

    //cast a sphere to the anchor while we're swinging
    //if the spherecast collides with anything, create a node at the point, make the node a child of the terrain
    //using the swingAssist local x, move the node a certain distance away from the hitpoint
    //we now swing from the latest tether node

    //tether nodes should be poolable
    //tether nodes should be a prefab

    private void OnTetherCancelled(InputAction.CallbackContext callback)
    {
        Debug.LogError("TETHER UP");
    }



}
