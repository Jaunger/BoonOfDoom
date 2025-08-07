using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Focus Damage")]
public class TakeFocusDamageEffect : TakeDamageEffect   // ← inherits the full class
{
    [Tooltip("How much Focus (mana) is removed when this effect is applied")]
    public float focusCost = 15f;

    /*  We override ProcessEffect so we DON'T run the health/poise workflow
     *  from TakeDamageEffect.  All other fields inherited from the parent
     *  class are irrelevant here, so we leave them unused. */
    public override void ProcessEffect(CharacterManager character)
    {
        // Apply only to players (NPCs don't use focus)
        if (character is PlayerManager player)
        {
            player.playerStatManager.DrainFocus(focusCost);
        }
    }
}
