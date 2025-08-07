using UnityEngine;

[CreateAssetMenu(menuName = "CharacterActions/Weapon Actions/Off Hand Melee Action")]
public class OffHandMeleeAction : WeaponItemAction
{
    public override void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerfomingAction)
    {
        base.AttemptToPerformAction(playerPerformingAction, weaponPerfomingAction);

        if (!playerPerformingAction.characterCombatManager.canBlock)
            return;

        if (playerPerformingAction.playerCombatManager.isUsingItem)
            return;

        if (playerPerformingAction.isAttacking)
        {
            playerPerformingAction.isBlocking = false;

            return;
        }

        if (playerPerformingAction.isBlocking)
            return;

        playerPerformingAction.isBlocking = true;
    }
}
