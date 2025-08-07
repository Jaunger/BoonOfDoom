using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUISkillTreeManager : MonoBehaviour
{
    [Header("Menu")]
    [SerializeField] private GameObject menu;
    private List<WeaponItem> unlockedWeapons = new List<WeaponItem>();

    [Header("Skill Info Panel")]
    [SerializeField] private TextMeshProUGUI skillNameText;
    [SerializeField] private TextMeshProUGUI skillDescriptionText;
    [SerializeField] private TextMeshProUGUI runeCostText;

    [Header("Unlock Confirm Popup")]
    public PlayerUISkillUnlockConfirmPopup unlockConfirmPopup;

    [Header("Feedback Message")]
    [SerializeField] private GameObject feedbackMessageBox;
    [SerializeField] private TextMeshProUGUI feedbackMessageText;
    [SerializeField] private float feedbackDuration = 2.5f;
    private Coroutine feedbackRoutine;

    [Header("Node Generation")]
    [SerializeField] private Transform skillNodesParent;
    [SerializeField] private GameObject skillNodePrefab;

    [Header("Weapon Switcher")]
    [SerializeField] private GameObject weaponTabButtonTemplate;
    [SerializeField] private Transform weaponSwitcherBar;

    // Runtime list of node UI instances
    private List<GameObject> spawnedSkillNodes = new List<GameObject>();

    [Header("Skill Tree State")]
    public bool isInUnlockMode = false;
    private SkillNodeUI selectedSkillNode;

    private void Awake()
    {
        if (menu != null)
            menu.SetActive(false);

        if (feedbackMessageBox != null)
            feedbackMessageBox.SetActive(false);
    }

    /// <summary>
    /// Displays an in‐menu feedback message.
    /// </summary>
    public void ShowFeedbackMessage(string message)
    {
        if (feedbackRoutine != null)
            StopCoroutine(feedbackRoutine);

        if (feedbackMessageText != null && feedbackMessageBox != null)
        {
            feedbackMessageText.text = message;
            feedbackMessageBox.SetActive(true);
            feedbackRoutine = StartCoroutine(HideFeedbackBoxAfterDelay());
        }
    }

    private IEnumerator HideFeedbackBoxAfterDelay()
    {
        yield return new WaitForSeconds(feedbackDuration);

        if (feedbackMessageBox != null)
            feedbackMessageBox.SetActive(false);
    }

    /// <summary>
    /// Opens the skill‐tree in view or unlock mode.
    /// Clones the equipped weapon’s tree on the actual instance so we never touch the asset.
    /// </summary>
    public void OpenSkillTreeMenu(bool unlockMode)
    {
        isInUnlockMode = unlockMode;

        // Close any other menu
        if (unlockMode)
            PlayerUIManager.instance.bonfireManager.CloseBonfireMenu();
        else
            PlayerUIManager.instance.menuManager.CloseCharacterMenu();

        // 1) Grab the player and its actual equipped weapon instance
        var player = PlayerUIManager.instance.player;
        var weapon = player.playerInventoryManager.currentRightWeapon;
        var skillMgr = player.GetComponent<WeaponSkillManager>();

        // 2) Clone the tree on that instance (once)
        var tree = skillMgr.GetSharedRuntimeTree(weapon.itemID);
        weapon.runtimeSkillTree = tree;

        // 3) Build tabs & show
        unlockedWeapons = player.playerInventoryManager.GetUnlockedWeapons();
        GenerateWeaponTabs(unlockedWeapons, weapon);

        PlayerUIManager.instance.menuIsOpen = true;
        menu.SetActive(true);
    }

    public void CloseSkillTreeMenu()
    {
        PlayerUIManager.instance.menuIsOpen = false;
        unlockConfirmPopup.gameObject.SetActive(false);
        menu.SetActive(false);
    }

    public void CloseSkillTreeMenuAfterATime()
    {
        StartCoroutine(WaitThenCloseMenu());
    }

    private IEnumerator WaitThenCloseMenu()
    {
        yield return new WaitForFixedUpdate();
        PlayerUIManager.instance.menuIsOpen = false;
        unlockConfirmPopup.gameObject.SetActive(false);
        menu.SetActive(false);
    }

    /// <summary>
    /// Creates one tab per unlocked weapon; clicking a tab loads that weapon’s cloned tree.
    /// </summary>
    public void GenerateWeaponTabs(List<WeaponItem> unlockedWeapons, WeaponItem equippedWeapon)
    {
        // Clear old tabs
        foreach (Transform child in weaponSwitcherBar)
            if (child != weaponTabButtonTemplate.transform)
                Destroy(child.gameObject);

        GameObject firstTab = null;

        foreach (var weapon in unlockedWeapons)
        {
            var buttonGO = Instantiate(weaponTabButtonTemplate, weaponSwitcherBar);
            buttonGO.SetActive(true);

            var label = buttonGO.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null) label.text = weapon.itemName;

            var btn = buttonGO.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() =>
            {
                // Make sure it’s cloned, then load from that clone
                var tree = PlayerUIManager.instance.player
                                             .GetComponent<WeaponSkillManager>()
                                             .GetSharedRuntimeTree(weapon.itemID);
                weapon.runtimeSkillTree = tree;
                LoadSkillTree(tree);
            });

            if (weapon.itemID == equippedWeapon.itemID)
            {
                // Auto‐select the equipped weapon on open
                var tree = PlayerUIManager.instance.player
                                             .GetComponent<WeaponSkillManager>()
                                             .GetSharedRuntimeTree(weapon.itemID);
                weapon.runtimeSkillTree = tree;
                LoadSkillTree(tree);
                firstTab = buttonGO;
            }
        }

        if (firstTab != null)
            firstTab.GetComponent<Button>()?.onClick.Invoke();
    }

    /// <summary>
    /// Spawns a UI node button for each node in the provided (cloned) tree.
    /// </summary>
    public void LoadSkillTree(WeaponSkillTree tree)
    {
        // Clear existing nodes
        foreach (var go in spawnedSkillNodes)
            Destroy(go);
        spawnedSkillNodes.Clear();

        // Instantiate one UI element per node
        foreach (var node in tree.nodes)
        {
            var nodeGO = Instantiate(skillNodePrefab, skillNodesParent);
            nodeGO.GetComponent<RectTransform>().anchoredPosition = node.uiPosition;

            var ui = nodeGO.GetComponent<SkillNodeUI>();
            ui.linkedSkillNode = node;    // points to the clone
            ui.UpdateVisualState();

            spawnedSkillNodes.Add(nodeGO);
        }
    }

    /// <summary>
    /// Updates the info panel for the selected node.
    /// </summary>
    public void SelectSkillNode(SkillNodeUI skillNode)
    {
        selectedSkillNode = skillNode;
        skillNameText.text = skillNode.linkedSkillNode.skillName;
        skillDescriptionText.text = skillNode.linkedSkillNode.skillDescription;
        runeCostText.text = "Cost: " + skillNode.linkedSkillNode.runeCost;
    }

    public void ShowUnlockPopup(bool show)
    {
        if (unlockConfirmPopup != null)
            unlockConfirmPopup.gameObject.SetActive(show);
    }

    /// <summary>
    /// Attempts to unlock the selected node on the clone, updates UI & feedback.
    /// </summary>
    //public void ConfirmUnlockSkill()
    //{
    //    Debug.Log("EXISTS?");
    //    if (selectedSkillNode == null || WeaponSkillUnlockManager.instance == null) return;

    //    WeaponItem temp = null;

    //    foreach (var weapon in PlayerUIManager.instance.player.playerInventoryManager.GetUnlockedWeapons())
    //    {
    //        if (weapon.runtimeSkillTree != null && weapon.runtimeSkillTree.nodes.Contains(selectedSkillNode.linkedSkillNode))
    //        {
    //            temp = weapon;
    //            break;
    //        }
    //    }

    //    bool unlocked = WeaponSkillUnlockManager.instance.TryUnlockWeaponSkill(
    //        PlayerUIManager.instance.player,
    //        temp,
    //        selectedSkillNode.linkedSkillNode);

    //    if (unlocked)
    //    {
    //        selectedSkillNode.UpdateVisualState();
    //        ShowFeedbackMessage("Skill unlocked!");
    //    }
    //    else
    //    {
    //        ShowFeedbackMessage("Unlock failed.");
    //    }

    //    if (unlockConfirmPopup != null)
    //        unlockConfirmPopup.gameObject.SetActive(false);
    //}

    //public void CancelUnlock()
    //{
    //    if (unlockConfirmPopup != null)
    //        unlockConfirmPopup.gameObject.SetActive(false);
    //}

    /// <summary>
    /// Returns true if node is not unlocked and all its parents are unlocked.
    /// </summary>
    public bool IsSkillAvailable(WeaponSkillNode node)
    {
        if (node.isUnlocked) return false;
        if (node.connectedNodes == null || node.connectedNodes.Count == 0) return true;

        foreach (var parent in node.connectedNodes)
            if (!parent.isUnlocked)
                return false;
        return true;
    }

    /// <summary>
    /// Finds & refreshes the UI for a specific node after unlock.
    /// </summary>
    public void UpdateNodeUI(WeaponSkillNode node)
    {
        foreach (var go in spawnedSkillNodes)
        {
            var ui = go.GetComponent<SkillNodeUI>();
            if (ui != null && ui.linkedSkillNode == node)
            {
                ui.UpdateVisualState();
                break;
            }
        }
    }

    

}
