using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillNodeUI : MonoBehaviour, ISelectHandler
{
    public WeaponSkillNode linkedSkillNode;

    [Header("Components")]
    public Button nodeButton;

    private void Awake()
    {
        if (nodeButton == null)
            nodeButton = GetComponent<Button>();

        nodeButton.onClick.RemoveAllListeners();
        nodeButton.onClick.AddListener(OnNodeClicked);
    }


    public void UpdateVisualState()
    {
        if (linkedSkillNode == null || nodeButton == null)
            return;

        ColorBlock colors = nodeButton.colors;

        if (linkedSkillNode.isUnlocked)
        {
            colors.normalColor = new Color(0.6f, 0.9f, 0.6f); // Green = unlocked
        }
        else if (PlayerUIManager.instance.playerUISkillTreeManager.IsSkillAvailable(linkedSkillNode))
        {
            colors.normalColor = new Color(0.9f, 0.85f, 0.4f); // Yellow = available
        }
        else
        {
            colors.normalColor = new Color(0.3f, 0.3f, 0.3f); // Dark gray = locked
        }

        nodeButton.colors = colors;
    }

    public void SelectThisNode()
    {
        if (linkedSkillNode == null)
            return;

        PlayerUIManager.instance.playerUISkillTreeManager.SelectSkillNode(this);
    }

    public void OnSelect(BaseEventData eventData)
    {
        SelectThisNode(); // Updates info panel only
    }


    private void OnNodeClicked()
    {
        if (linkedSkillNode == null)
            return;

        var treeUI = PlayerUIManager.instance.playerUISkillTreeManager;
        treeUI.SelectSkillNode(this); // Update info panel always

        if (treeUI.isInUnlockMode &&
            !linkedSkillNode.isUnlocked &&
            treeUI.IsSkillAvailable(linkedSkillNode))
        {
            treeUI.unlockConfirmPopup.SetSkillInfo(linkedSkillNode, this);
            treeUI.ShowUnlockPopup(true);
        }
    }







}
