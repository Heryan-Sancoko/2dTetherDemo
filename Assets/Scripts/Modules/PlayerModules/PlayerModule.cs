using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModule : EntityModule
{
    protected PlayerController playerController;

    public override void AddController(EntityController newController)
    {
        base.AddController(newController);
        playerController = newController as PlayerController;
    }
}
