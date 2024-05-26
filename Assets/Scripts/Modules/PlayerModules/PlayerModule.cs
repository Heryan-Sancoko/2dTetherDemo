using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModule : EntityModule
{
    protected PlayerController playerController;
    public PlayerController Controller => playerController;

    public override void AddController(EntityController newController)
    {
        base.AddController(newController);
        playerController = newController as PlayerController;
    }
}
