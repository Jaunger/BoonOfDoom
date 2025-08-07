using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerCombatManager : CharacterCombatManager
{
    PlayerManager player;

    public WeaponItem currentWeaponBeingUsed;
    public ProjectileSlot currentProjectileBeingUsed;

    [Header("Projectile ")]
    private Vector3 projectileAimDirection;

    [Header("Flags")]
    public bool weaponCanCombo = false;
    public bool isUsingItem = false;
    public bool isChugging = false;

    private void Start()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    private void OnActiveSceneChanged(Scene arg0, Scene arg1)
    {
        // Death Spot
        if (WorldSaveGameManager.instance.currentCharacterData.hasDeathSpot)
        {
            Vector3 deathSpotPos = new Vector3(WorldSaveGameManager.instance.currentCharacterData.deathSpotXPos,
                WorldSaveGameManager.instance.currentCharacterData.deathSpotYPos,
                WorldSaveGameManager.instance.currentCharacterData.deathSpotZPos);

            CreateDeathSpot(deathSpotPos, WorldSaveGameManager.instance.currentCharacterData.deathSpotSoulsCount, false);
        }

    }

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    public void CreateDeathSpot(Vector3 deathSpotPos, int soulCount, bool removePlayerSouls = true)
    {

        GameObject deathSpotFX = Instantiate(WorldCharacterEffectsManager.instance.deadSpotVFX, deathSpotPos, Quaternion.identity);
        PickUpRuneInteractable pickUpRuneInteractable = deathSpotFX.GetComponent<PickUpRuneInteractable>();
        pickUpRuneInteractable.soulCount = soulCount;

        if (removePlayerSouls)
            player.playerStatManager.AddSouls(-player.playerStatManager.souls);

        WorldSaveGameManager.instance.currentCharacterData.hasDeathSpot = true;
        WorldSaveGameManager.instance.currentCharacterData.deathSpotSoulsCount = pickUpRuneInteractable.soulCount;
        WorldSaveGameManager.instance.currentCharacterData.deathSpotXPos = deathSpotPos.x;
        WorldSaveGameManager.instance.currentCharacterData.deathSpotYPos = deathSpotPos.y;
        WorldSaveGameManager.instance.currentCharacterData.deathSpotZPos = deathSpotPos.z;


    }

    public void PerformWeaponBasedAction(WeaponItemAction weaponAction, WeaponItem weaponPerformingAction)
    {
        //  Perform action
        if (weaponAction == null)
        {
            Debug.Log("No action to perform");
            return;
        }
        weaponAction.AttemptToPerformAction(player, weaponPerformingAction);
    }

    public virtual void DrainStaminaBasedOnAttack(AttackType attackType)
    {
        float staminaDeducted = 0;

        if (currentWeaponBeingUsed == null)
            return;

        switch (currentAttackType)
        {
            case AttackType.LightAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackCostModifier;
                break;
            case AttackType.LightAttack02:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackCostModifier;
                break;
            case AttackType.HeavyAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.heavyAttackCostModifier;
                break;
            case AttackType.ChargedHeavyAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.chargedAttackCostModifier;
                break;
            case AttackType.RunningAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.RunningAttackCostModifier;
                break;
            case AttackType.RollingAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.RollingAttackCostModifier;
                break;
            case AttackType.BackstepAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.BackstepAttackCostModifier;
                break;
            default:
                break;
        }
        // Debug.Log("deducting stamina for attack" +staminaDeducted);

        player.characterStatManager.currentStamina -= Mathf.RoundToInt(staminaDeducted);
    }

    public override void SetTarget(CharacterManager newTarget)
    {
        base.SetTarget(newTarget);

        PlayerCamera.instance.SetLockedCameraHeight();
    }

    public override void EnableCanCombo()
    {
        if (player.isUsingRightHand)
        {
            player.playerCombatManager.weaponCanCombo = true;
        }
    }

    public override void DisableCanCombo()
    {
        if (player.isUsingRightHand)
        {
            player.playerCombatManager.weaponCanCombo = false;
        }
    }

    public void ReleaseArrow()
    {
        player.hasArrowNotched = false;

        if (player.playerEffectManager.activeDrawnProjectileFX != null)
            Destroy(player.playerEffectManager.activeDrawnProjectileFX);
        Debug.Log("Releasing arrow.");
        player.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.releaseArrowSFX));

        Animator bowAnimtor;

        if (player.playerEquipmentManager.rightHandWeaponModel != null)
        {
            bowAnimtor = player.playerEquipmentManager.rightHandWeaponModel.GetComponentInChildren<Animator>();
        }
        else
        {
            Debug.LogError("No right weapon equipped to release arrow.");
            return;
        }

        bowAnimtor.SetBool("isDrawn", false);
        //bowAnimtor.Play("Bow_Fire_01");

        RangedProjectileItem projectileItem = null;

        switch (currentProjectileBeingUsed)
        {
            case ProjectileSlot.main:
                projectileItem = player.playerInventoryManager.mainProjectile;
                currentWeaponBeingUsed.weaponElement = WeaponElement.None; 
                break;
            case ProjectileSlot.secondary:
                if (currentWeaponBeingUsed.runtimeSkillTree.HasUnlockedNode("Electrified Arrow"))
                {
                    projectileItem = player.playerInventoryManager.secondaryProjectile;
                    currentWeaponBeingUsed.weaponElement = WeaponElement.Electric;
                }
              
                break;
            default:
                Debug.LogError("Unknown projectile slot type.");
                return;
        }

        if (projectileItem == null)
            return;

        if (projectileItem.currentAmmo <= 0)
            return;

        Debug.Log("Releasing projectile: " + projectileItem.itemName + " with ID: " + projectileItem.itemID);

        Transform projectileInstantiationLocation;
        GameObject liveProjectileGameObject;
        Rigidbody liveProjectileRigidbody;
        RangeProjectileDamageCollider liveProjectileDamageCollider;

        projectileItem.currentAmmo--;

        projectileInstantiationLocation = player.playerCombatManager.lockOnTransform;
        liveProjectileGameObject = Instantiate(projectileItem.releaseProjectileModel, projectileInstantiationLocation);
        liveProjectileDamageCollider = liveProjectileGameObject.GetComponent<RangeProjectileDamageCollider>();
        liveProjectileRigidbody = liveProjectileGameObject.GetComponent<Rigidbody>();

        liveProjectileDamageCollider.physicalDamage = projectileItem.physicalDamange;
        liveProjectileDamageCollider.magicDamage = projectileItem.magicDamage;
        liveProjectileDamageCollider.fireDamage = projectileItem.fireDamage;
        liveProjectileDamageCollider.holyDamage = projectileItem.holyDamage;
        liveProjectileDamageCollider.lightningDamage = projectileItem.lightningDamage;
        liveProjectileDamageCollider.element = projectileItem.element;
        liveProjectileDamageCollider.characterShottingProjectile = player;

        //AIMING
        if (player.isAiming)
        {

            Ray ray = new Ray(lockOnTransform.position, PlayerCamera.instance.aimDirection);
            projectileAimDirection = ray.GetPoint(5000);
            liveProjectileGameObject.transform.LookAt(projectileAimDirection);
        }
        else
        {
            //LOCKED AND NOT AIMING
            if (player.playerCombatManager.currentTarget != null)
            {
                Quaternion arrowRotation = Quaternion.LookRotation(player.playerCombatManager.currentTarget.characterCombatManager.lockOnTransform.position
                    - liveProjectileGameObject.transform.position);

                liveProjectileGameObject.transform.rotation = arrowRotation;
            }
            //UNLOCKED AND NOT AIMING
            else
            {
                Quaternion arrowRotation = Quaternion.LookRotation(-liveProjectileGameObject.transform.forward);

                liveProjectileGameObject.transform.rotation = arrowRotation;
            }
        }


        Collider[] characterColliders = player.GetComponentsInChildren<Collider>();
        List<Collider> ignoreColliders = new(characterColliders);

        foreach (var collider in characterColliders)
            ignoreColliders.Add(collider);

        foreach (Collider hitBox in ignoreColliders)
            Physics.IgnoreCollision(liveProjectileDamageCollider.damageCollider, hitBox, true);

        Debug.Log(liveProjectileGameObject.transform.forward);
        liveProjectileRigidbody.AddForce(liveProjectileGameObject.transform.forward * projectileItem.forwardVelocity);
        liveProjectileGameObject.transform.parent = null;

    }

    public void SuccessfullyUseQuickSlotItem()
    {
        if (player.playerInventoryManager.currentQuickSlotItem != null)
        {
            player.playerInventoryManager.currentQuickSlotItem.SuccessfullyUseItem(player);
        }
    }

    public void SetIsChugging(bool isChugging)
    {
        this.isChugging = isChugging;
        player.animator.SetBool("isChuggingFlask", isChugging);
    }

    public void EnablePoiseBonusFromBoneBreaker()
    {
        var tree = player.playerInventoryManager.currentRightWeapon?.runtimeSkillTree;

        if (tree == null) return;

        foreach (var node in tree.nodes)
        {
            if (node.isUnlocked && node.skillName == "Bone Breaker")
            {
                player.playerStatManager.currentPoiseDamageMultiplier += 0.20f;
                Debug.Log("[BoneBreaker] Bonus poise multiplier applied");
                return;
            }
        }
    }

    public void OnChargedSlamImpact()
    {
        WeaponItem weapon = player.playerInventoryManager.currentRightWeapon;
        if (weapon == null) return;

        weapon.weaponElement = WeaponElement.HeavyImpact;


        var slamAction = weapon.oh_RT_Action as ChargedSlamWeaponItemAction;
        if (slamAction == null) return;

        Vector3 center = transform.position + Vector3.up;   // adjust height if needed
        float radius = 2.0f;                              // tweak as desired

        LayerMask charLayer = WorldUtilityManager.instance.GetCharacterLayers();
        Collider[] hits = Physics.OverlapSphere(center, radius, charLayer);




        foreach (var hit in hits)
        {
            if (!hit.TryGetComponent(out CharacterManager target))
                continue;

            // Skip targets we are not allowed to damage
            if (!WorldUtilityManager.instance.CanIdamageThisTarget(
                    CharacterType.Player, target.characterType))
                continue;

            // Build and apply the damage effect
            TakeDamageEffect dmg = Instantiate(
                WorldCharacterEffectsManager.instance.takeDamageEffect);

            dmg.physicalDamage = weapon.physicalDamage * slamAction.damageMultiplier;
            dmg.poiseDamage = slamAction.poiseBonus;
            dmg.characterCausingDamage = player;
            dmg.weaponElement = WeaponElement.HeavyImpact;
            dmg.contactPoint = hit.ClosestPoint(center);

            target.characterEffectsManager.ProcessInstantEffect(dmg);
        }

        // Notify listeners (Seismic Wave will subscribe later)
        if (ChargedSlamImpact != null)
        {
            Debug.Log("[ChargedSlamImpact] Invoking event at position: " + transform.position);
            ChargedSlamImpact.Invoke(transform.position);
        }
        // Optional: ground-impact VFX / SFX
        // Instantiate(slamImpactPrefab, center, Quaternion.identity);

        weapon.weaponElement = WeaponElement.None;

    }

    public static event Action<Vector3> ChargedSlamImpact;

}
