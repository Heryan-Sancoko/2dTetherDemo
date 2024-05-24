using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttackModule : PlayerModule
{
    protected Rigidbody rbody;
    [SerializeField] protected WeaponScriptable weaponData;
    [SerializeField] protected WeaponScript weapon;
    [SerializeField] protected List<EnemyHealthModule> enemyHealthModuleList = new List<EnemyHealthModule>();
    [SerializeField] protected LayerMask enemyLayers;
    [SerializeField] protected Transform weaponHolder;

    public float currentDamage;
    public float currentAttackCooldown;
    private PlayerMovementModule playerMovementModule;
    private InputManager inputManager;

    public override void AddController(EntityController newController)
    {
        base.AddController(newController);
        rbody = GetComponent<Rigidbody>();
        weapon = Instantiate(weaponData.weaponPrefab,weaponHolder).GetComponent<WeaponScript>();
        weapon.transform.localRotation = Quaternion.identity;
        weapon.transform.localPosition = Vector3.zero;
        weapon.ConfigureWeapon(enemyLayers, this);
        weapon.gameObject.SetActive(false);
        playerMovementModule = playerController.GetModule<PlayerMovementModule>();
        inputManager = InputManager.Instance;
        inputManager.Attack.performed += OnAttack;
    }

    public void OnAttack(InputAction.CallbackContext callback)
    {
        if (currentAttackCooldown > 0)
            return;

        currentAttackCooldown = weaponData.baseAttackCooldown;



        //enable weapon
        weapon.gameObject.SetActive(true);
        weapon.WeaponAnim.SetTrigger(Constants.AnimationPrams.StartAttack);
        Vector3 mousePos = playerController.GetMouseDirection();
        weaponHolder.transform.LookAt(transform.position + mousePos, Vector3.up);
    }

    public override void UpdatePlayerModule()
    {
        base.UpdatePlayerModule();
        if (currentAttackCooldown > 0)
            currentAttackCooldown -= Time.deltaTime;
    }

    public void ChangeCurrentDamage(float newCurrentDamage)
    {
        currentDamage = newCurrentDamage;
    }

    public virtual void ResetCurrentDamage()
    {
        currentDamage = weaponData.baseDamage;
    }



    //clear the list of damaged enemies and start the attack
    //check the hitboxes for overlap
    //if there is an overlap, proc damage on the enemy
    //add enemy to a list of already damaged enemies

    //method to clear list of hit enemies for potential
    //multi-hit attacks
}
