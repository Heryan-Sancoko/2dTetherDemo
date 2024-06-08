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
    [SerializeField] protected float jumpAmountOnEnemyHit;

    public float currentDamage;
    public float currentAttackCooldown;
    private PlayerMovementModule playerMovementModule;
    private InputManager inputManager;
    private int attackSwingCap;
    private int currentAttackSwing;
    public int CurrentAttackSwing => currentAttackSwing;

    public override void AddController(EntityController newController)
    {
        base.AddController(newController);
        rbody = GetComponent<Rigidbody>();
        attackSwingCap = weaponData.attackSwingAmount;
        currentAttackSwing = 1;
        weapon = Instantiate(weaponData.weaponPrefab,weaponHolder).GetComponent<WeaponScript>();
        weapon.transform.localRotation = Quaternion.identity;
        weapon.transform.localPosition = Vector3.zero;
        weapon.ConfigureWeapon(enemyLayers, this);
        weapon.gameObject.SetActive(false);
        playerMovementModule = playerController.GetModule<PlayerMovementModule>();
        inputManager = InputManager.Instance;

        inputManager.Attack.performed += OnAttack;
    }

    public void ResetAttackSwing()
    {
        currentAttackSwing = 1;
    }

    public void SetCustomAttackCooldown( float newAttackCooldown)
    {
        currentAttackCooldown = newAttackCooldown;
    }

    public void OnHeldStarted(InputAction.CallbackContext callback)
    {
    }

    public void OnHeldAttack(InputAction.CallbackContext callback)
    {
        OnAttack(callback);
        playerMovementModule.ForceVelocityInDirectionOverDuration(weaponHolder.forward*20, 0.25f, false, MoveStatus.attacking);
    }

    public void OnAttack(InputAction.CallbackContext callback)
    {
        if (currentAttackCooldown > 0)
            return;

        currentAttackCooldown = weaponData.baseAttackCooldown;


        //enable weapon
        weapon.gameObject.SetActive(true);

        if (callback.action == inputManager.Attack)
        {
            weapon.WeaponAnim.SetInteger(Constants.AnimationPrams.AttackSwing, currentAttackSwing);
            weapon.WeaponAnim.SetTrigger(Constants.AnimationPrams.StartAttack);
            currentAttackSwing++;
            if (currentAttackSwing > attackSwingCap)
                currentAttackSwing = 1;
        }
        else
            weapon.WeaponAnim.SetTrigger(Constants.AnimationPrams.StartHeldAttack);

        HaveWeaponLookTowardsAim();

    }

    private void HaveWeaponLookTowardsAim()
    {
        Vector3 mousePos = playerController.GetMouseDirection();
        mousePos.z = transform.position.z;

        Vector3 lookPoint = transform.position + mousePos;
        lookPoint.z = transform.position.z;

        weaponHolder.transform.LookAt(lookPoint, Vector3.up);
    }

    public void DoCustomAttack(string animationParameter)
    {
        weapon.gameObject.SetActive(true);
        weapon.WeaponAnim.SetTrigger(animationParameter);
        HaveWeaponLookTowardsAim();
    }

    public void JumpOnHitEnemy()
    {
        if (!playerController.GroundedModule.IsGrounded)
        playerController.JumpOnEnemyHit(Vector3.up*jumpAmountOnEnemyHit, 0.2f, true);
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
