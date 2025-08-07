using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Runtime Effects/Bulwark Momentum")]
public class BulwarkMomentumEffect : RuntimeCharacterEffect
{
    private int stacks;
    private float timer;
    private const int maxStacks = 5;
    private const float duration = 2f;

    public override void OnApply(CharacterManager ch)
    {
        stacks = 0;
        timer = 0;
    }

    public override void Tick(CharacterManager ch)
    {
        if (timer > 0) timer -= Time.deltaTime;
        if (timer <= 0 && stacks > 0)
        {
            stacks = 0;
            if (ch is PlayerManager p) p.playerStatManager.currentDamageReductionPercent = 0f;
        }
    }

    public void RegisterHit(PlayerManager p)
    {
        stacks = Mathf.Min(stacks + 1, maxStacks);
        timer = duration;
        p.playerStatManager.currentDamageReductionPercent = stacks * 5f;
    }

    public override void OnRemove(CharacterManager ch)
    {
        if (ch is PlayerManager p)
            p.playerStatManager.currentDamageReductionPercent = 0f;
    }
}
