using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerUISkillUnlockConfirmPopup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI confirmTitleText;
    [SerializeField] private TextMeshProUGUI confirmCostText;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;
    private WeaponSkillNode pendingSkillNode;
    private SkillNodeUI pendingSkillNodeUI;

    private void Awake()
    {
        confirmButton.onClick.AddListener(ConfirmUnlock);
        cancelButton.onClick.AddListener(CancelUnlock);
    }

    public void SetSkillInfo(WeaponSkillNode node, SkillNodeUI sourceUI)
    {
        pendingSkillNode = node;
        pendingSkillNodeUI = sourceUI;

        confirmTitleText.text = $"Unlock {node.skillName}?";
        confirmCostText.text = $"Cost: {node.runeCost} Runes";

        // Optional: auto-focus confirm button here
    }



    private void ConfirmUnlock()
    {
        if (pendingSkillNode == null)
            return;

        WeaponItem temp = null;

        foreach (var weapon in PlayerUIManager.instance.player.playerInventoryManager.GetUnlockedWeapons())
        {
            if (weapon.runtimeSkillTree != null && weapon.runtimeSkillTree.nodes.Contains(pendingSkillNode))
            {
                temp = weapon;
                break;
            }
        }

        bool unlocked = WeaponSkillUnlockManager.instance.TryUnlockWeaponSkill(
            PlayerUIManager.instance.player,
            temp,
            pendingSkillNode);

        if (unlocked)
        {
            PlayerUIManager.instance.playerUISkillTreeManager.UpdateNodeUI(pendingSkillNode);
            PlayerUIManager.instance.playerUISkillTreeManager.ShowFeedbackMessage("Skill unlocked!");
        }
        else
        {
            PlayerUIManager.instance.playerUISkillTreeManager.ShowFeedbackMessage("Not enough runes.");
        }


        if (pendingSkillNodeUI != null)
        {
            pendingSkillNodeUI.SelectThisNode(); // Reselect UI node
            pendingSkillNodeUI.nodeButton.Select();
        }
        ClearPopup();
    }

    private void CancelUnlock()
    {
        if (pendingSkillNodeUI != null)
        {
            pendingSkillNodeUI.SelectThisNode();
            pendingSkillNodeUI.nodeButton.Select(); 
        }

        ClearPopup();
    }



    private void ClearPopup()
    {
        pendingSkillNode = null;
        pendingSkillNodeUI = null;
        gameObject.SetActive(false);
    }


}
