using UnityEngine;

public class CharacterUIManager : MonoBehaviour
{
    [Header("UI")]
    public bool displayHPBar = true;
    [SerializeField] private UI_Character_HP_Bar characterHPBar;

    public void OnHPChanged(int oldValue, int newValue)
    {
        if (characterHPBar != null)
        {
            characterHPBar.oldHealthValue = oldValue;
            characterHPBar.SetStat(newValue);
        }
    }

    public void ResetCharacterHPBar()
    {
        if (characterHPBar == null)
            return;

        characterHPBar.currentDamageTaken = 0;
    }



}
