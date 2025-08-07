using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Static Effects/Defense Boost")]
public class DefenseBoostEffect : StaticCharacterEffect
{
    [SerializeField] private float defenseBonus = 10f;

    public override void ProcessStaticEffect(CharacterManager character)
    {
        character.characterStatManager.armorPhysicalDamageAbsorption += defenseBonus;
        Debug.Log($"[Effect] Applied +{defenseBonus} defense");
    }

    public override void RemoveStaticEffect(CharacterManager character)
    {
        character.characterStatManager.armorPhysicalDamageAbsorption -= defenseBonus;
        Debug.Log($"[Effect] Removed +{defenseBonus} defense");
    }
}
