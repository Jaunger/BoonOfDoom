using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Runtime Effects/Seismic Wave")]
public class SeismicWaveEffect : RuntimeCharacterEffect
{
    [Header("Wave VFX")]
    public GameObject shockwavePrefab;    // drag your ripple prefab here
    public float prefabLife = 2f;

    [Header("Gameplay")]
    public float radius = 6f;             // metres
    public float damageScale = 0.5f;      // 50 % weapon dmg
    public float knockdownPoise = 999f;   // enough to floor trash mobs

    private PlayerManager player;

    /* ---------------------------------------------------------- */
    public override void OnApply(CharacterManager c)
    {
        player = c as PlayerManager;
        PlayerCombatManager.ChargedSlamImpact += OnSlamImpact;
    }

    public override void OnRemove(CharacterManager c)
    {
        PlayerCombatManager.ChargedSlamImpact -= OnSlamImpact;
    }

    public override void Tick(CharacterManager c) { }    // unused
    /* ---------------------------------------------------------- */

    private void OnSlamImpact(Vector3 pos)
    {
        // 1. Spawn VFX (optional prefab)
        if (shockwavePrefab != null)
            Destroy(Instantiate(shockwavePrefab, pos, Quaternion.identity), prefabLife);

        // 2. Damage + knock-down
        LayerMask charMask = WorldUtilityManager.instance.GetCharacterLayers();
        Collider[] hits = Physics.OverlapSphere(pos, radius, charMask);

        foreach (var hit in hits)
        {
            if (!hit.TryGetComponent(out CharacterManager target))
                continue;

            if (!WorldUtilityManager.instance.CanIdamageThisTarget(CharacterType.Player, target.characterType))
                continue;

            TakeDamageEffect dmg = Instantiate(
                WorldCharacterEffectsManager.instance.takeDamageEffect);

            dmg.physicalDamage = player.playerInventoryManager.currentRightWeapon.physicalDamage * damageScale;
            dmg.poiseDamage = knockdownPoise;
            dmg.characterCausingDamage = player;

            dmg.contactPoint = hit.ClosestPoint(pos);

            target.characterEffectsManager.ProcessInstantEffect(dmg);
        }
    }
}
