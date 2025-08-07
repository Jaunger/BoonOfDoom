using UnityEngine;

public class WeaponSkillUnlockManager : MonoBehaviour
{
    public static WeaponSkillUnlockManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public bool TryUnlockWeaponSkill(PlayerManager player, WeaponItem targetWeapon, WeaponSkillNode skillNode)
    {
        if (player == null || player.playerInventoryManager.currentRightWeapon == null || targetWeapon == null)
        {
            Debug.LogWarning("Player or weapon is missing.");
            return false;
        }

        WeaponItem weapon = targetWeapon;

        if (weapon.runtimeSkillTree == null)
            weapon.runtimeSkillTree =
            player.GetComponent<WeaponSkillManager>()
                           .GetSharedRuntimeTree(weapon.itemID);

        WeaponSkillTree tree = weapon.runtimeSkillTree;

        if (tree == null)
        {
            Debug.LogWarning("Weapon has no skill tree assigned.");
            return false;
        }

        // Ensure this node belongs to the runtime clone
        if (!tree.nodes.Contains(skillNode))
        {
            Debug.LogWarning("Attempted to unlock a node from the original asset instead of the clone.");
            return false;
        }

        // Check prerequisites
        if (skillNode.isUnlocked)
        {
            Debug.Log("Cannot unlock skill: already unlocked.");
            return false;
        }
        if (!PlayerUIManager.instance.playerUISkillTreeManager.IsSkillAvailable(skillNode))
        {
            Debug.Log("Cannot unlock skill: prerequisites not met.");
            return false;
        }
        if (weapon.weaponLevel < skillNode.requiredWeaponLevel)
        {
            Debug.Log("Cannot unlock skill: weapon level too low.");
            return false;
        }
        if (player.playerStatManager.souls < skillNode.runeCost)
        {
            Debug.Log("Cannot unlock skill: not enough runes.");
            return false;
        }

        // Deduct cost and unlock
        player.playerStatManager.AddSouls(-skillNode.runeCost);
        skillNode.isUnlocked = true;
        Debug.Log($"Skill Unlocked: {skillNode.skillName}");

        // Stat enhancement
        if (skillNode.skillType == WeaponSkillType.StatEnhancement)
            ApplyStatEnhancement(player, weapon, skillNode);

        // Special ability
        if (skillNode.specialAbilityActionID >= 0)
            AssignSpecialAbilityToWeapon(weapon, skillNode.specialAbilityActionID);

        // Combo upgrade
        if (skillNode.skillType == WeaponSkillType.ComboUpgrade)
            AddComboAttackToWeapon(weapon, skillNode);

        return true;
    }

    private void ApplyStatEnhancement(PlayerManager player, WeaponItem weapon, WeaponSkillNode skillNode)
    {
        foreach (var id in skillNode.staticEffectIDs)
        {
            if (id >= 0 && id < WorldCharacterEffectsManager.instance.staticEffects.Count)
            {
                var fx = WorldCharacterEffectsManager.instance.staticEffects[id];
                player.playerEffectManager.AddStaticEffect(fx);

            }
        }

        foreach (var id in skillNode.runtimeEffectIDs)
        {
            if (id >= 0 && id < WorldCharacterEffectsManager.instance.runtimeEffects.Count)
            {
                var fx = WorldCharacterEffectsManager.instance.runtimeEffects[id];
                player.playerEffectManager.AddRuntimeEffect(fx);

            }
        }
    }



    private void AddComboAttackToWeapon(WeaponItem weapon, WeaponSkillNode skillNode)
    {
        // Only mutate the per-instance action (assumed already cloned), not the asset
        var lightAction = weapon.oh_RB_Action as LightAttackWeaponItemAction;
        if (lightAction == null)
        {
            Debug.LogWarning("Cannot add combo: RB action is not LightAttackWeaponItemAction.");
            return;
        }

        string combo = skillNode.newComboAnimationName;
        // Prevent duplicate inserts
        if (!lightAction.lightAttackAnimations.Contains(combo))
        {
            if (skillNode.insertComboAtIndex >= 0
                && skillNode.insertComboAtIndex <= lightAction.lightAttackAnimations.Count)
            {
                lightAction.lightAttackAnimations.Insert(skillNode.insertComboAtIndex, combo);
            }
            else
            {
                lightAction.lightAttackAnimations.Add(combo);
            }
            Debug.Log($"[Combo] Added {combo} to {weapon.itemName}");
        }
    }

    private void AssignSpecialAbilityToWeapon(WeaponItem weapon, int actionID)
    {
        WeaponItemAction action = WorldActionManager.instance.GetWeaponItemActionByID(actionID);
        if (action != null)
        {
            if (weapon.specialAbilityAction == null)
                weapon.specialAbilityAction = action;            // first slot → Infuse
            else
                weapon.specialAbilityActionAlt = action;         // second slot → Slash

            Debug.Log($"[Ability] Assigned {action.name} to {weapon.itemName}");
        }
        else
        {
            Debug.LogWarning($"Special ability ID {actionID} not found.");
        }
    }

    public bool HasUnlockedSkill(PlayerManager player, WeaponSkillNode targetNode)
    {
        if (player == null || player.playerInventoryManager.currentRightWeapon == null)
            return false;

        var tree = player.playerInventoryManager.currentRightWeapon.ActiveSkillTree;
        if (tree == null) return false;

        foreach (var node in tree.nodes)
        {
            if (node == targetNode && node.isUnlocked)
                return true;
        }
        return false;
    }

    public void ApplySkillNodeEffectNoCost(WeaponSkillNode node, WeaponItem weapon)
    {
        switch (node.skillType)
        {
            case WeaponSkillType.ComboUpgrade:
                AddComboAttackToWeapon(weapon, node);   // your existing method
                break;
            case WeaponSkillType.StatEnhancement:
                if (WorldSaveGameManager.instance != null &&
                    WorldSaveGameManager.instance.player != null)
                {
                    ApplyStatEnhancement(WorldSaveGameManager.instance.player, weapon, node);
                }
                break;
                // … flame infuse / slash will plug in here later …
        }
    }

}
