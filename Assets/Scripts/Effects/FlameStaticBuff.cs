using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Static Effects/Fire Buff")]
public class FireStaticBuff : StaticCharacterEffect
{
    public float bonusFireDamage = 20f;

    public override void ProcessStaticEffect(CharacterManager character)
    {
        if (character is not PlayerManager p) return;

        var wm = p.playerEquipmentManager != null ? p.playerEquipmentManager.rightWeaponManager : null;
        if (wm == null || wm.meleeDamageCollider == null) return;

        wm.meleeDamageCollider.fireDamage += bonusFireDamage;   // add fire DMG
    }

    public override void RemoveStaticEffect(CharacterManager character)
    {
        if (character is not PlayerManager p) return;

        var wm = p.playerEquipmentManager != null ? p.playerEquipmentManager.rightWeaponManager : null;
        if (wm == null || wm.meleeDamageCollider == null) return;

        wm.meleeDamageCollider.fireDamage -= bonusFireDamage;   // restore
    }
}
