using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private List<PlayerModule> moduleList = new List<PlayerModule>();
    private Rigidbody rbody;
    public Rigidbody Rbody => rbody;


    void Awake()
    {
        rbody = GetComponent<Rigidbody>();
        foreach (PlayerModule module in moduleList)
        {
            module.AddPlayerController(this);
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
