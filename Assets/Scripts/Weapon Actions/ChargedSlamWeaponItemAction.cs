// ChargedSlamWeaponItemAction.cs
// Special-ability action for the Great Axe: hold RT to charge, release to slam.
// The unlock node uses specialAbilityActionID = 310.

using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterActions/Weapon Actions/Charged Slam Action")]
public class ChargedSlamWeaponItemAction : WeaponItemAction
{
    /* Charge settings */
    [Tooltip("Seconds RT must be held before we consider it a charging attack")]
    public float chargeThreshold = 0.25f;

    [Tooltip("Focus drained per second while charging")]
    public float focusDrainPerSecond = 1f;

    [Tooltip("Stamina cost applied once on slam impact")]
    public float staminaCostOnImpact = 10f;

    /* Impact settings */
    [Tooltip("Base physical damage multiplier (1 = 100 percent weapon damage)")]
    public float damageMultiplier = 2.0f;

    [Tooltip("Extra poise damage dealt on impact")]
    public float poiseBonus = 30f;

    /* Runtime fields (not shown in Inspector) */
    [HideInInspector] public bool isCharging = false;
    [HideInInspector] public float chargeTimer = 0f;


    public override void AttemptToPerformAction(PlayerManager p, WeaponItem weapon)
    {

        if (p.isPerformingAction) return;      // already mid-swing, ignore

        bool nodeUnlocked = weapon.runtimeSkillTree != null &&
                     weapon.runtimeSkillTree.HasUnlockedNode("Charged Slam");

        if (!nodeUnlocked)
        {
            p.playerAnimatorManager.PlayerTargetAttackActionAnimation(weapon, AttackType.HeavyAttack01, "Main_Heavy_Attack_01", true);
            PlayerInputManager.instance.holdRTInput = false;

            return;
        }
        p.StartCoroutine(ChargeRoutine(p, weapon));
    }

    private IEnumerator ChargeRoutine(PlayerManager p, WeaponItem weapon)
    {
        float chargeTimer = 0f;

        /* 1. Initial heavy attack (will blend into _Hold automatically) */
        p.playerAnimatorManager.PlayerTargetAttackActionAnimation(
            weapon, AttackType.HeavyAttack03, "Main_Heavy_Attack_01", true);

        weapon.weaponElement = WeaponElement.HeavyImpact; 

        /* 2. While RT is held, drain focus after threshold */
        while (PlayerInputManager.instance.holdRTInput)
        {
            chargeTimer += Time.deltaTime;

            /* Drain focus every frame during the hold */
            p.playerStatManager.DrainFocus(focusDrainPerSecond * Time.deltaTime);

            /* Abort if focus bar is empty */
            if (p.playerStatManager.currentFocus <= 0f)
            {
                PlayerInputManager.instance.holdRTInput = false; 
                break;
            }

            /* Auto-release the moment we reach the threshold */
            if (chargeTimer >= chargeThreshold)
            {
                PlayerInputManager.instance.holdRTInput = false;
                break;
            }

            yield return null;
        }


        /* 3. Button released (or focus empty) -> decide outcome */
        bool fullyCharged = chargeTimer >= chargeThreshold &&
                            p.playerStatManager.currentFocus > 0f;

        p.isFullyChargedAttack = fullyCharged;          // sets Animator param

        if (fullyCharged)
            p.playerStatManager.DecreaseStamina(staminaCostOnImpact);

  
    }




}
