using UnityEngine;

[CreateAssetMenu(menuName = "CharacterActions/Weapon Actions/Heavy Attack Action")]
public class HeavyAttackWeaponItemAction : WeaponItemAction
{
    [SerializeField] string heavy_Attack_01 = "Main_Heavy_Attack_01";

    public override void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerfomingAction)
    {
        base.AttemptToPerformAction(playerPerformingAction, weaponPerfomingAction);

        if (playerPerformingAction.characterStatManager.currentStamina <= 0) 
            return;

        if (!playerPerformingAction.playerLocomitionManager.isGrounded)
            return;

        if (playerPerformingAction.playerCombatManager.isUsingItem)
            return;

        playerPerformingAction.isAttacking = true;

        PerformLighAttack(playerPerformingAction, weaponPerfomingAction);
    }

    private void PerformLighAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerfomingAction)
    {
        if (playerPerformingAction.isUsingRightHand)
        {
            playerPerformingAction.playerAnimatorManager.PlayerTargetAttackActionAnimation(weaponPerfomingAction, AttackType.HeavyAttack01, heavy_Attack_01, true);
        }
        else if (playerPerformingAction.isUsingLeftHand)
        {
        }
    }
}
