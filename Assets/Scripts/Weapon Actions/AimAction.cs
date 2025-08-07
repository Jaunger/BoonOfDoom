using UnityEngine;

[CreateAssetMenu(menuName = "CharacterActions/Weapon Actions/Aim Action")]
public class AimAction : WeaponItemAction
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerfomingAction)
    {
        base.AttemptToPerformAction(playerPerformingAction, weaponPerfomingAction);

        if (!playerPerformingAction.playerLocomitionManager.isGrounded)
            return;

        if (playerPerformingAction.isJumping)
            return;

        if (playerPerformingAction.playerLocomitionManager.isRolling)
            return;

        if (playerPerformingAction.isLockedOn)
            return;

        playerPerformingAction.isAiming = true;
    }
}
