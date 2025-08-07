using System;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public MeleeWeaponDamageCollider meleeDamageCollider;
    public bool isFlaming = false;
    public float fireTickTimer = 0f;
    public float fireDrainRate = 1f;
    private CharacterManager wielder;
    private WeaponItem weapon;
    private int fireBuffID = -1;

    [SerializeField] private ParticleSystem flameInfuseVFX;
    [SerializeField] public Transform flameSlashSpawnPoint;
    [SerializeField] private WeaponItemAction flameInfuseAction;


    private void Awake()
    {
        meleeDamageCollider = GetComponentInChildren<MeleeWeaponDamageCollider>();
    }

    private void Update()
    {
        if (isFlaming && wielder is PlayerManager p)
        {
            fireTickTimer += Time.deltaTime;
            if (fireTickTimer >= 1f)
            {
                fireTickTimer = 0f;
                p.playerStatManager.DrainFocus(fireDrainRate);
                if (p.playerStatManager.currentFocus <= 0f)
                    DeactivateFlame();
            }
        }

    }


    public void ActivateFlame()
    {
        if (isFlaming) return;
        if (wielder is not PlayerManager p) return;
        if (p.playerStatManager.currentFocus < fireDrainRate) return;
        if (weapon.runtimeSkillTree.HasUnlockedNode("Flame Infuse "))
        {
            return;
        }

        var template = WorldCharacterEffectsManager.instance.staticEffects
                         .Find(e => e is FireStaticBuff);
        var buff = Instantiate(template);
        fireBuffID = buff.staticEffectID;   

        p.playerEffectManager.AddStaticEffect(buff);

        isFlaming = true;
        fireTickTimer = 0f;
        if (flameInfuseVFX)
            flameInfuseVFX.Play();
        //TODO: Add SFX here
        weapon.weaponElement = WeaponElement.Fire;
        SetWeaponElement(wielder, weapon);
    }

    private void SetWeaponElement(CharacterManager wielder, WeaponItem weapon)
    {
        if (meleeDamageCollider == null) return;
        meleeDamageCollider.element = weapon.weaponElement;
    }

    public void DeactivateFlame()
    {
        if (!isFlaming) return;
        isFlaming = false;

        if (wielder is PlayerManager p && fireBuffID != -1)
            p.playerEffectManager.RemoveStaticEffect(fireBuffID);

        
        weapon.weaponElement = WeaponElement.None;
        SetWeaponElement(wielder, weapon);

        fireBuffID = -1;
        if (flameInfuseVFX)
            flameInfuseVFX.Stop();
    }





    public void SetWeaponDamage(CharacterManager wielder, WeaponItem weapon)
    {

        this.wielder = wielder;
        this.weapon = weapon;

        if (meleeDamageCollider == null) return;

        meleeDamageCollider.characterCausingDamage = wielder;
        meleeDamageCollider.physicalDamage = weapon.physicalDamage;
        meleeDamageCollider.magicDamage = weapon.magicDamage;
        meleeDamageCollider.fireDamage = weapon.fireDamage;
        meleeDamageCollider.holyDamage = weapon.holyDamage;
        meleeDamageCollider.lightningDamage = weapon.lightningDamage;
        meleeDamageCollider.poiseDamage = weapon.poiseDamage;

        meleeDamageCollider.light_Attack_01_Modifier = weapon.light_attack_01_Modifier;
        meleeDamageCollider.light_Attack_02_Modifier = weapon.light_attack_02_Modifier;
        meleeDamageCollider.light_Attack_03_Modifier = weapon.light_attack_03_Modifier;

        meleeDamageCollider.heavy_Attack_01_Modifier = weapon.heavy_attack_01_Modifier;
        meleeDamageCollider.heavy_Attack_02_Modifier = weapon.heavy_attack_02_Modifier;
        meleeDamageCollider.heavy_Attack_03_Modifier = weapon.heavy_attack_03_Modifier;

        meleeDamageCollider.charged_heavy_Attack_01_Modifier = weapon.charged_heavy_attack_01_Modifier;
        meleeDamageCollider.running_Attack_Modifier = weapon.RunningAttackCostModifier;
        meleeDamageCollider.rolling_Attack_Modifier = weapon.RollingAttackCostModifier;
        meleeDamageCollider.backstep_Attack_Modifier = weapon.BackstepAttackCostModifier;

        meleeDamageCollider.element = weapon.weaponElement;
        meleeDamageCollider.weaponModelType = weapon.weaponType;


    }
}
