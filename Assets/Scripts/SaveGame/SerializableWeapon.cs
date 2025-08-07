using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableWeapon : ISerializationCallbackReceiver  
{
    [SerializeField] public int itemID;
    [SerializeField] public List<int> unlockedSkillNodeIndices = new();

    public WeaponItem GetWeapon()
    {
        WeaponItem weapon = WorldItemDatabase.instance.GetWeaponFromSerializedData(this);

        var skillMgr = WeaponSkillManager.Instance;
        WeaponSkillTree tree = skillMgr.GetSharedRuntimeTree(itemID);
        weapon.runtimeSkillTree = tree;

        var unlockMgr = Object.FindFirstObjectByType<WeaponSkillUnlockManager>(); 

        foreach (int idx in unlockedSkillNodeIndices)
        {
            if (idx < 0 || idx >= tree.nodes.Count) continue;
            var node = tree.nodes[idx];
            if (!node.isUnlocked)
            {
                node.isUnlocked = true;                   // persist flag  
                unlockMgr.ApplySkillNodeEffectNoCost(node, weapon); // helper you already have  
            }
        }

        return weapon;
    }

    public void CaptureRuntimeData(WeaponItem runtimeWeapon)
    {
        unlockedSkillNodeIndices.Clear();

        var tree = runtimeWeapon.runtimeSkillTree;
        if (tree == null) return;

        for (int i = 0; i < tree.nodes.Count; i++)
            if (tree.nodes[i].isUnlocked)
                unlockedSkillNodeIndices.Add(i);
    }

    public void OnBeforeSerialize()
    {
    }
    public void OnAfterDeserialize()
    {
    }
}

