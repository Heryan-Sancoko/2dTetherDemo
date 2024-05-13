using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private List<PlayerModule> moduleList = new List<PlayerModule>();
    [SerializeField] private PlayerMovementModule playerMovementModule;
    private Rigidbody rbody;
    public Rigidbody Rbody => rbody;

    void Awake()
    {
        rbody = GetComponent<Rigidbody>();
        foreach (PlayerModule module in moduleList)
        {
            module.AddPlayerController(this);

            switch (module)
            {
                case PlayerMovementModule:
                    playerMovementModule = module as PlayerMovementModule;
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (PlayerModule module in moduleList)
        {
            module.UpdatePlayerModule();
        }
    }

    private void FixedUpdate()
    {
        foreach (PlayerModule module in moduleList)
        {
            module.FixedUpdatePlayerModule();
        }
    }

    public void ApplyNewVelocityToRigidbody(Vector3 newVel)
    {
        playerMovementModule.ApplyNewVelocityToRigidbody(newVel);
    }

    public T GetModule<T>() where T : PlayerModule
    {
        foreach (var module in moduleList)
        {
            if (module is T)
            {
                return (T)module;
            }
        }
        return null;
    }
}
