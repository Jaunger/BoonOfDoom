using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Static Effects/Attack Boost")]
public class AttackBoostEffect : StaticCharacterEffect
{
    [SerializeField] private float damageBonus = 10f;

    public override void ProcessStaticEffect(CharacterManager character)
    {
        if (character is PlayerManager player)
        {
            WeaponItem weapon = player.playerInventoryManager.currentRightWeapon;
            weapon.physicalDamage += damageBonus;
            Debug.Log($"[Effect] Applied +{damageBonus} attack to {weapon.itemName}");
        }
    }

    public override void RemoveStaticEffect(CharacterManager character)
    {
        if (character is PlayerManager player)
        {
            WeaponItem weapon = player.playerInventoryManager.currentRightWeapon;
            weapon.physicalDamage -= damageBonus;
            Debug.Log($"[Effect] Removed +{damageBonus} attack from {weapon.itemName}");
        }
    }
}
