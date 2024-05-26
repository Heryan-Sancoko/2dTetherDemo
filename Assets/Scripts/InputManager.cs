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
    public InputAction Aim;
    public InputAction Dash;
    public InputAction Attack;
    public InputAction HeldAttack;

    public override void Awake()
    {
        base.Awake();
        playerInput.enabled = true;

        Move = playerInput.currentActionMap.actions[Constants.ActionMapActions.Move];
        Move.Enable();

        Jump = playerInput.currentActionMap.actions[Constants.ActionMapActions.Jump];
        Jump.Enable();

        Tether = playerInput.currentActionMap.actions[Constants.ActionMapActions.Tether];
        Tether.Enable();

        Aim = playerInput.currentActionMap.actions[Constants.ActionMapActions.Aim];
        Aim.Enable();

        Dash = playerInput.currentActionMap.actions[Constants.ActionMapActions.Dash];
        Dash.Enable();

        Attack = playerInput.currentActionMap.actions[Constants.ActionMapActions.Attack];
        Attack.Enable();

        HeldAttack = playerInput.currentActionMap.actions[Constants.ActionMapActions.HeldAttack];
        HeldAttack.Enable();
    }



}
