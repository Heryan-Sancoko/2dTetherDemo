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

    public void OnHeldStarted(InputAction.CallbackContext callback)
    {
        Debug.LogError("HELD STARTED");
    }

    public void OnHeldAttack(InputAction.CallbackContext callback)
    {
        Debug.LogError("HELD ATTACK");
        OnAttack(callback);
        //playerMovementModule.ForceVelocityInDirectionOverDuration(weaponHolder.forward*20, 0.25f, false, MoveStatus.airHop);
    }

    public void OnAttack(InputAction.CallbackContext callback)
    {
        if (currentAttackCooldown > 0)
            return;

        currentAttackCooldown = weaponData.baseAttackCooldown;


        //enable weapon
        weapon.gameObject.SetActive(true);

        if (callback.action == inputManager.Attack)
            weapon.WeaponAnim.SetTrigger(Constants.AnimationPrams.StartAttack);
        else
            weapon.WeaponAnim.SetTrigger(Constants.AnimationPrams.StartHeldAttack);

        Vector3 mousePos = playerController.GetMouseDirection();
        Vector3 upDir = Vector3.up;
        if (mousePos.x > transform.position.x)
            upDir = Vector3.up + Vector3.left;
        else
            upDir = Vector3.up + Vector3.right;

        weaponHolder.transform.LookAt(transform.position + mousePos, upDir);
    }

    public void DoCustomAttack(string animationParameter)
    {
        weapon.gameObject.SetActive(true);
        weapon.WeaponAnim.SetTrigger(animationParameter);

        Vector3 mousePos = transform.position + playerController.GetMouseDirection();
        Vector3 look = (mousePos.x > transform.position.x) ? Vector3.right : Vector3.left;

        weaponHolder.transform.LookAt(transform.position + look, Vector3.up);

    }

    public void JumpOnHitEnemy()
    {
        if (!playerController.GroundedModule.IsGrounded)
        playerController.JumpOnEnemyHit(Vector3.up*7, 0.2f, true);
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
