using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterActions/Weapon Actions/Light Attack Action")]
public class LightAttackWeaponItemAction : WeaponItemAction
{
    [Header("Light Attack Combos")]
    [SerializeField] public List<string> lightAttackAnimations = new List<string>();

    [Header("Running Attack Action")]
    [SerializeField] string run_Attack_01 = "Main_Run_Attack_01";

    [Header("Rolling Attack Action")]
    [SerializeField] string roll_Attack_01 = "Main_Roll_Attack_01";

    [Header("Attack Type")]
    [SerializeField] AttackType[] attackTypes = new AttackType[3];

    public override void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        base.AttemptToPerformAction(playerPerformingAction, weaponPerformingAction);

        if (playerPerformingAction.playerCombatManager.isUsingItem)
            return;

        if (playerPerformingAction.characterStatManager.currentStamina <= 0)
            return;

        if (!playerPerformingAction.playerLocomitionManager.isGrounded)
            return;

        playerPerformingAction.isAttacking = true;

        if (playerPerformingAction.isSprinting)
        {
            PerformRunningAttack(playerPerformingAction, weaponPerformingAction);
            return;
        }

        if (playerPerformingAction.characterCombatManager.canDoRollingAttack)
        {
            PerformRollingAttack(playerPerformingAction, weaponPerformingAction);
            return;
        }

        if (playerPerformingAction.characterCombatManager.canDoBackstepAttack)
        {
            PerformBackstepAttack(playerPerformingAction, weaponPerformingAction);
            return;
        }

        PerformLightAttack(playerPerformingAction, weaponPerformingAction);
    }

    private void PerformLightAttack(PlayerManager player, WeaponItem weapon)
    {
        if (lightAttackAnimations.Count == 0)
        {
            Debug.LogWarning("No light attacks assigned!");
            return;
        }

        // If we can combo off our last attack...
        if (player.playerCombatManager.weaponCanCombo && player.isPerformingAction)
        {
            player.playerCombatManager.weaponCanCombo = false;

            int lastIndex = lightAttackAnimations.IndexOf(
                player.characterCombatManager.lastAttackAnimation);

            if (lastIndex >= 0 && lastIndex < lightAttackAnimations.Count - 1)
            {

                // block swing-3 unless the Combo-Upgrade node is unlocked
                if (lastIndex + 1 == 2 && !ThirdComboUnlocked(weapon))
                {
                    player.characterCombatManager.lastAttackAnimation = null;   // restart chain
                    return;                                                     // skip playing clip-3
                }

                // candidate for next combo
                string nextAttack = lightAttackAnimations[lastIndex + 1];

                Debug.Log($"Performing combo: {nextAttack} after {player.characterCombatManager.lastAttackAnimation}");
                    player.playerAnimatorManager
                          .PlayerTargetAttackActionAnimation(
                              weapon, attackTypes[lastIndex + 1], nextAttack, true);

                return;
            }

            // fallback to restart combo
            player.playerAnimatorManager
                  .PlayerTargetAttackActionAnimation(
                      weapon, attackTypes[0], lightAttackAnimations[0], true);
        }
        else if (!player.isPerformingAction)
        {
            // first strike always allowed
            player.playerAnimatorManager
                  .PlayerTargetAttackActionAnimation(
                      weapon, attackTypes[0], lightAttackAnimations[0], true);
        }
    }

    private void PerformRunningAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        playerPerformingAction.playerLocomitionManager.canSprint = false;
        playerPerformingAction.playerAnimatorManager.PlayerTargetAttackActionAnimation(weaponPerformingAction, AttackType.RunningAttack01, run_Attack_01, true);
        playerPerformingAction.StartCoroutine(ReEnableSprinting(playerPerformingAction));
    }

    private System.Collections.IEnumerator ReEnableSprinting(PlayerManager player)
    {
        yield return new WaitForSeconds(0.5f);
        player.playerLocomitionManager.canSprint = true;
    }

    private void PerformRollingAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        playerPerformingAction.characterCombatManager.canDoRollingAttack = false;
        playerPerformingAction.playerAnimatorManager.PlayerTargetAttackActionAnimation(weaponPerformingAction, AttackType.RollingAttack01, roll_Attack_01, true);
    }

    private void PerformBackstepAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        playerPerformingAction.characterCombatManager.canDoBackstepAttack = false;
        playerPerformingAction.playerAnimatorManager.PlayerTargetAttackActionAnimation(weaponPerformingAction, AttackType.BackstepAttack01, roll_Attack_01, true);
    }

    private bool ThirdComboUnlocked(WeaponItem weapon)
    {
        var tree = weapon.runtimeSkillTree ?? weapon.ActiveSkillTree;
        if (tree == null) return false;

        foreach (var n in tree.nodes)
            if (n.skillType == WeaponSkillType.ComboUpgrade && n.isUnlocked)
                return true;
        return false;
    }

}
