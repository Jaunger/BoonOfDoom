using UnityEngine;

public class Enums : MonoBehaviour
{
    
}

public enum CharacterSlot
{
    characterSlot_01,
    characterSlot_02,
    characterSlot_03,
    NO_SLOT
}

public enum CharacterType
{
    Player,
    AI
}

public enum WeaponModelSlot
{
    RightHand,
    LeftHand
}

//public enum EquipmentModelType
//{
//    FullHelmt,
//    OpenHelmt,
//    Hood,
//    HelmetAcessorie,
//    FaceCover,
//    Torso,
//    Back,
//    RightShoulder,
//    RightUpperArm,
//    RightElbow,
//    RightLowerArm,
//    RightHand,
//    LeftShoulder,
//    LeftUpperArm,
//    LeftElbow,
//    LeftLowerArm,
//    LeftHand,
//    Hips,
//    HipsAttachment,
//    RightLeg,
//    RightKnee,
//    LeftLeg,
//    LeftKnee
//} 

public enum AttackType
{
    LightAttack01,
    LightAttack02,
    LightAttack03,
    HeavyAttack01,
    HeavyAttack02,
    HeavyAttack03,
    ChargedHeavyAttack01,
    HeldHeavyAttack01, 
    RunningAttack01,
    RollingAttack01,
    BackstepAttack01
}

public enum  DamageIntensity
{
    Ping,
    Light,
    Medium,
    Heavy,
    Critical
}

public enum WeaponModelType
{
    Weapon,
    GreatAxe,
    Unarmed
}

public enum ItemPickupType
{
    WorldSpawn,
    CharacterDrop
}

public enum IdleStateMode
{
    Idle,
    Patrol,
    Sleep
}

public enum WeaponClass
{
    StraightSword,
    GreatAxe,
    Bow,
    Unarmed
}

public enum WeaponSkillType
{
    ComboUpgrade,
    ElementalImbue,
    ElementalRelease,
    StatEnhancement,
    PassiveAbility,
    SpecialAbility
}

public enum WeaponAbilityType
{
    None,
    Flame,   // burn puzzles
    Smash,   // heavy-impact puzzles
    Throw    // dagger throws
}

public enum WeaponElement
{
    None,        // default
    Fire,        // straight-sword inflame / ranged fire slash
    HeavyImpact, // great-axe charged-slam or other big hits
    Electric    // bow charged shot

}

public enum  ProjectileClass 
{
    Arrow,
    Bolt,
    FireSlash
}

public enum  ProjectileSlot
{
    main,
    secondary
}

