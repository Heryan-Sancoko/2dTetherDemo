using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityModule : MonoBehaviour
{
    protected EntityController controller;

    public virtual void AddController(EntityController newController)
    {
        controller = newController;
    }

    public virtual void UpdatePlayerModule()
    {

    }

    public virtual void FixedUpdatePlayerModule()
    {

    }
}
