using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModule : MonoBehaviour
{
    protected PlayerController playerController;

    public virtual void AddPlayerController(PlayerController newController)
    {
        playerController = newController;
    }

    public virtual void UpdatePlayerModule()
    {
        
    }

    public virtual void FixedUpdatePlayerModule()
    {
        
    }

}
