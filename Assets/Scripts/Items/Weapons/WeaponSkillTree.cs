using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Weapons/Weapon Skill Tree")]
public class WeaponSkillTree : ScriptableObject
{
    public List<WeaponSkillNode> nodes;

    public List<WeaponSkillNode> GetUnlockedNodes()
    {
        List<WeaponSkillNode> unlockedNodes = new();
        foreach (var node in nodes)
        {
            if (node.isUnlocked)
                unlockedNodes.Add(node);
        }
        return unlockedNodes;
    }

    public List<WeaponSkillNode> GetAvailableNodes(int weaponLevel)
    {
        List<WeaponSkillNode> availableNodes = new();
        foreach (var node in nodes)
        {
            if (!node.isUnlocked && weaponLevel >= node.requiredWeaponLevel)
            {
                availableNodes.Add(node);
            }
        }
        return availableNodes;
    }

    public bool HasUnlockedNode(string nodeName)
    {
        foreach (var node in nodes)
        {
            if (node.isUnlocked && node.skillName == nodeName)
                return true;
        }
        return false;
    }

}

