using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Static Effects/Iron Bulk")]
public class IronBulkStaticEffect : StaticCharacterEffect
{
    [SerializeField] private float staggerReductionPercent = 15f;
    public override void ProcessStaticEffect(CharacterManager c)
    {
        if (c is PlayerManager p)
            p.playerStatManager.frontalPoiseAbsorption += staggerReductionPercent;
    }
    public override void RemoveStaticEffect(CharacterManager c)
    {
        if (c is PlayerManager p)
            p.playerStatManager.frontalPoiseAbsorption -= staggerReductionPercent;
    }
}
