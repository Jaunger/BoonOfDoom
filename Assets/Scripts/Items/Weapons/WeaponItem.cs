using UnityEngine;

public class WeaponItem : Item
{
    [Header("Animations")]
    public AnimatorOverrideController weaponAnimator;

    [Header("Model Instantiation")]
    public WeaponModelType weaponType;

    [Header("Weapon Model")]
    public GameObject weaponModel;

    [Header("Weapon Class")]
    public WeaponClass weaponClass;

    [Header("Weapon Element")]
    public WeaponElement weaponElement = WeaponElement.None;

    [Header("Weapon Base Damage")]
    public float physicalDamage = 0; 
    public float magicDamage = 0;
    public float fireDamage = 0;
    public float lightningDamage = 0;
    public float holyDamage = 0;

    //  weapon guard 

    [Header("Weapon Poise")]
    public float poiseDamage = 0;
    //  offensive poise when attacking

    [Header("Attack Modifiers")]
    public float light_attack_01_Modifier = 1;
    public float light_attack_02_Modifier = 1.2f;
    public float light_attack_03_Modifier = 1.2f; //  TODO: add to weapon
    public float heavy_attack_01_Modifier = 1.3f;
    public float heavy_attack_02_Modifier = 1.5f;
    public float heavy_attack_03_Modifier = 1.5f; //  TODO: add to weapon
    public float charged_heavy_attack_01_Modifier = 1.7f;
    public float running_attack_Modifier = 1.5f;
    public float rolling_attack_Modifier = 1.5f;
    public float backstep_attack_Modifier = 1.5f; //  TODO: add to weapon

    //  light heavy critical

    [Header("Stamina Costs Modifiers")]
    public int baseStaminaCost = 20;
    public float lightAttackCostModifier = 1;
    public float heavyAttackCostModifier = 1.5f;
    public float chargedAttackCostModifier = 2;
    public float RunningAttackCostModifier = 1.5f;
    public float RollingAttackCostModifier = 1.5f;
    public float BackstepAttackCostModifier = 1.5f;

    [Header("Weapon Blocking Absorption")]
    public float physicalBlockAbsorption = 0.5f; // 50% damage absorbed
    public float magicBlockAbsorption = 0.5f; // 50% damage absorbed
    public float fireBlockAbsorption = 0.5f; // 50% damage absorbed
    public float lightningBlockAbsorption = 0.5f; // 50% damage absorbed
    public float holyBlockAbsorption = 0.5f; // 50% damage absorbed
    public float stability = 50; // reduces stamina lost from blocking

    //  running attack light attack etc

    [Header("Actions")]
    public WeaponItemAction oh_RB_Action;
    public WeaponItemAction oh_RT_Action;
    public WeaponItemAction oh_LB_Action; // block

    [Header("Weapon Progression")]
    public int weaponLevel = 1;
    public int currentWeaponEXP = 0;
    public int maxWeaponEXP = 100; // Initial exp to level up
    public WeaponSkillTree weaponSkillTree; // Link to the tree
    public WeaponSkillTree runtimeSkillTree;

    public WeaponSkillTree ActiveSkillTree =>
        runtimeSkillTree != null ? runtimeSkillTree : weaponSkillTree;


    [Header("Special Ability (Puzzle / World Action)")]
    public WeaponItemAction specialAbilityAction;   // will be assigned at runtime
    public int specialAbilityActionID = -1;         // for save/restore if you need
    public WeaponItemAction specialAbilityActionAlt;   // for Flame Slash

    //  blocking sounds
    [Header("Whooshes")]
    public AudioClip[] whooshSounds; //TODO: add to weapon
    public AudioClip[] blockSounds; //TODO: add to weapon


    public void GrantWeaponEXP(int amount)
    {
        currentWeaponEXP += amount;
        if (currentWeaponEXP >= maxWeaponEXP)
        {
            currentWeaponEXP -= maxWeaponEXP;
            weaponLevel++;
            maxWeaponEXP = Mathf.RoundToInt(maxWeaponEXP * 1.25f); // Scaling EXP curve
        }
    }

    public bool TryUnlockSkill(WeaponSkillNode node, int availableRunes)
    {
        if (!node.isUnlocked && weaponLevel >= node.requiredWeaponLevel && availableRunes >= node.runeCost)
        {
            node.isUnlocked = true;
            return true;
        }
        return false;
    }
}
