using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Weapon Skill Node")]
public class WeaponSkillNode : ScriptableObject
{
    [Header("Skill Info")]
    public string skillName;
    public string skillDescription;
    public WeaponSkillType skillType;
    public Vector2 uiPosition;

    [Header("Combo Unlock Info")]
    public string newComboAnimationName; // e.g., "Main_Light_Attack_03"
    public int insertComboAtIndex = -1; // where to insert, -1 = add at end

    [Header("Special Ability Unlock (Optional)")]
    public int specialAbilityActionID = -1; // -1 means no special ability

    [Header("Required Info")]
    public bool isUnlocked = false;
    public int requiredWeaponLevel = 1;
    public int runeCost = 5;

    [Header("Passive Buff Effects")]
    [SerializeField]
    public List<int> staticEffectIDs = new();
    public List<int> runtimeEffectIDs = new();

    [Header("Effect flags")]
    public bool applyOnEquip = true;

    [SerializeReference]
    public List<WeaponSkillNode> connectedNodes = new();
}
