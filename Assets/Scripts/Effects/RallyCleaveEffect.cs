using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Runtime Effects/Rally Cleave")]
public class RallyCleaveEffect : RuntimeCharacterEffect
{
    [Tooltip("Health restored when the qualifying attack kills an enemy")]
    public float healthRestore = 8f;

    public override void OnApply(CharacterManager chr)
    {
        PlayerCombatManager.ChargedSlamImpact += Dummy;  // keeps asset referenced
    }
    public override void OnRemove(CharacterManager chr)
    {
        PlayerCombatManager.ChargedSlamImpact -= Dummy;
    }
    public override void Tick(CharacterManager c) { }

    /* Called from the kill-hook (see step 2) */
    public void OnEnemyKilled(PlayerManager p)
    {
        var stat = p.playerStatManager;
        stat.currentHealth =
            Mathf.Min(stat.currentHealth + healthRestore, stat.maxHealth);

    }

    private void Dummy(Vector3 _) { }
}
