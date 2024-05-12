using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class InputManager : Singleton<InputManager>
{
    [SerializeField] private PlayerInput playerInput;
    public InputAction Move;
    public InputAction Jump;
    public InputAction Tether;

    public override void Awake()
    {
        base.Awake();
        playerInput.enabled = true;

        Move = playerInput.currentActionMap.actions[0];
        Move.Enable();

        Jump = playerInput.currentActionMap.actions[1];
        Jump.Enable();

        Tether = playerInput.currentActionMap.actions[2];
        Tether.Enable();
    }


}
