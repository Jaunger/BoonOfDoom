using UnityEngine;

[CreateAssetMenu(menuName = "CharacterActions/Weapon Actions/Flame Infuse")]
public class FlameInfuseAction : WeaponItemAction
{
    public override void AttemptToPerformAction(PlayerManager player, WeaponItem weapon)
    {
        var wm = player.playerEquipmentManager.rightWeaponManager;
        if (wm == null) return;

        if (wm.isFlaming)
            wm.DeactivateFlame();   // manual-off
        else
            wm.ActivateFlame();     // starts drain; auto-off at 0 Focus

        Debug.Log($"Player {player.gameObject.name} toggled flame on weapon {weapon.itemName}.");
    }
}
