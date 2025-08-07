using System.Collections.Generic;
using UnityEngine;

/// One runtime-cloned tree per weapon ID, shared by every instance.
[RequireComponent(typeof(PlayerManager))]
public class WeaponSkillManager : MonoBehaviour
{
    private static readonly Dictionary<int, WeaponSkillTree> treeCache = new();

    public static WeaponSkillManager Instance { get; private set; }
    PlayerManager player;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        player = GetComponent<PlayerManager>();
    }

    /* ------------------------------------------------------------------ */
    /*  PUBLIC HELPERS                                                    */
    /* ------------------------------------------------------------------ */

    public WeaponSkillTree GetSharedRuntimeTree(int itemID)
    {
        if (treeCache.TryGetValue(itemID, out var tree))
            return tree;

        var authoringWeapon = WorldItemDatabase.instance.GetWeaponByID(itemID);
        tree = DeepCloneTree(authoringWeapon.weaponSkillTree);
        treeCache[itemID] = tree;
        return tree;
    }

    public void OnWeaponSwitched()
    {
        var hand = player.playerInventoryManager.currentRightWeapon;
        var combat = player.playerCombatManager.currentWeaponBeingUsed;
        if (hand == null) return;

        var tree = GetSharedRuntimeTree(hand.itemID);

        hand.runtimeSkillTree = tree;
        if (combat != null && combat.itemID == hand.itemID)
            combat.runtimeSkillTree = tree;

        PlayerUIManager.instance.playerUISkillTreeManager.LoadSkillTree(tree);
    }

    /* ------------------------------------------------------------------ */
    /*  INTERNAL: deep-clone once                                         */
    /* ------------------------------------------------------------------ */
    public WeaponSkillTree DeepCloneTree(WeaponSkillTree src)
    {
        // Nothing to clone?
        if (src == null || src.nodes == null || src.nodes.Count == 0)
        {
            var empty = ScriptableObject.CreateInstance<WeaponSkillTree>();
            empty.nodes = new List<WeaponSkillNode>();
            return empty;
        }
        var dst = ScriptableObject.CreateInstance<WeaponSkillTree>();
        dst.nodes = new List<WeaponSkillNode>();

        var map = new Dictionary<WeaponSkillNode, WeaponSkillNode>();
        foreach (var n in src.nodes)
        {
            var c = Object.Instantiate(n);
            bool isRoot = n.connectedNodes == null || n.connectedNodes.Count == 0;
            c.isUnlocked = isRoot || n.isUnlocked;   // keep root unlocked
            dst.nodes.Add(c);
            map[n] = c;
        }
        foreach (var n in src.nodes)
        {
            var c = map[n];
            c.connectedNodes = new List<WeaponSkillNode>();
            if (n.connectedNodes != null)
                foreach (var p in n.connectedNodes) c.connectedNodes.Add(map[p]);
        }
        return dst;
    }
}
