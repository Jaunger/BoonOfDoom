using TMPro;
using UnityEngine;

public class UI_Boss_HP_Bar : UI_StatBar
{
    [SerializeField] AIBossCharacterManager bossCharacter;
    public void EnableBossHPBar(AIBossCharacterManager aiBoss)
    {
        bossCharacter = aiBoss;
        bossCharacter.characterStatManager.OnHealthChanged += OnBossHPChanged;
        SetMaxStat(bossCharacter.characterStatManager.maxHealth);
        SetStat(Mathf.RoundToInt(bossCharacter.characterStatManager.currentHealth));
        GetComponentInChildren<TextMeshProUGUI>().text = bossCharacter.characterName;
    }

    private void OnDestroy()
    {
        bossCharacter.characterStatManager.OnHealthChanged -= OnBossHPChanged;
    }

    private void OnBossHPChanged(float oldValue, float newValue)
    {
        SetStat(Mathf.RoundToInt(newValue));

        if (newValue <= 0)
        {
            RemoveHPBar(0.5f);
        }
    }

    public void RemoveHPBar(float time)
    {
        Destroy(gameObject, time);
    }
}
