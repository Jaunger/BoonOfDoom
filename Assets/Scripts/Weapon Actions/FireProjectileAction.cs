using System;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterActions/Weapon Actions/Fire Projectile Action")]
public class FireProjectileAction : WeaponItemAction
{
    [SerializeField] ProjectileSlot projectileSlot; 
    public override void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerfomingAction)
    {
        base.AttemptToPerformAction(playerPerformingAction, weaponPerfomingAction);

        if (playerPerformingAction.playerStatManager.currentStamina <= 0)
            return;

        RangedProjectileItem projectileItem = null;

        //TODO: add more type for different arrow also think about flame slash

        switch (projectileSlot)
        {
            case ProjectileSlot.main:
                projectileItem = playerPerformingAction.playerInventoryManager.mainProjectile;
                weaponPerfomingAction.weaponElement = WeaponElement.None;
                break;
            case ProjectileSlot.secondary:
                if (weaponPerfomingAction.runtimeSkillTree == null)
                    return;

                if (weaponPerfomingAction.runtimeSkillTree.HasUnlockedNode("Electrified Arrow"))

                {
                    projectileItem = playerPerformingAction.playerInventoryManager.secondaryProjectile;
                    weaponPerfomingAction.weaponElement = WeaponElement.Electric;
                }

                break;
            default:
                Debug.LogError("Unknown projectile slot type.");
                return;
        }


        if (projectileItem == null)
            return;

        if (!playerPerformingAction.hasArrowNotched)
        {
            playerPerformingAction.hasArrowNotched = true;

            bool canIDrawAProjectile = CanIFireProjectile(weaponPerfomingAction, projectileItem);

            if (!canIDrawAProjectile)
                return;
            

            if (projectileItem.currentAmmo <= 0)
            {
                // play animation for no ammo
                //playerPerformingAction.playerAnimatorManager.PlayerTargetActionAnimation("Out_Of_Ammo_01", true);
                return;
            }

            playerPerformingAction.playerCombatManager.currentProjectileBeingUsed = projectileSlot;
            playerPerformingAction.playerAnimatorManager.PlayerTargetActionAnimation("Bow_Draw_01", true);
            DrawBow(playerPerformingAction, projectileItem);
        }

    }

    private void DrawBow(PlayerManager playerPerformingAction, RangedProjectileItem projectileItem)
    {
        Animator bowAnimator;
        if (playerPerformingAction.playerEquipmentManager.rightHandWeaponModel != null)
        {
            bowAnimator = playerPerformingAction.playerEquipmentManager.rightHandWeaponModel.GetComponentInChildren<Animator>();
        }
        else
        {
            Debug.LogError("No right weapon equipped to load projectile.");
            return;
        }

        bowAnimator.SetBool("isDrawn", true);
       // bowAnimator.Play("Bow_Draw_01");

        GameObject arrow = Instantiate(WorldItemDatabase.instance.GetProjectileByID(projectileItem.itemID).drawProjectileModel, 
            playerPerformingAction.playerEquipmentManager.leftHandSlot.transform);
        playerPerformingAction.playerEffectManager.activeDrawnProjectileFX = arrow;

        playerPerformingAction.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.notchArrowSFX));

    }

    private bool CanIFireProjectile(WeaponItem weaponPerfomingAction, RangedProjectileItem projectileItem)
    {
        // check for crossnbow great bow etc and compare ammot to give result

        return true;
    }
}
