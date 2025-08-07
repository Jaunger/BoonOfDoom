using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DamageCollider : MonoBehaviour
{
    [Header("Collider")]
    public Collider damageCollider;

    [Header("Damage")]
    public float physicalDamage = 0;
    public float magicDamage = 0;
    public float fireDamage = 0;
    public float lightningDamage = 0;
    public float holyDamage = 0;

    [Header("Element")]
    public WeaponElement element;
    public WeaponModelType weaponModelType;

    [Header("Contact Point")]
    //[SerializeField] float angleHitFrom = 0;
    protected Vector3 contanctPoint;

    [Header("Poise")]
    public float poiseDamage = 0;


    [Header("Characters Damaged")]
    [SerializeField] protected List<CharacterManager> charactersDamaged = new List<CharacterManager>();

    [Header("Block")]
    protected Vector3 directionFromAttackToDamageTarget;
    protected float dotValueFromAttackToDamageTarget;

    protected virtual void Awake()
    {

    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();

        bool isBeacon = other.GetComponentInParent<BeaconDetector>() != null;

        if (isBeacon)
            return;

        if (damageTarget != null)
        {
            contanctPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

           bool canDamage =  WorldUtilityManager.instance.CanIdamageThisTarget(
                damageTarget.characterType,
                this.GetComponentInParent<CharacterManager>().characterType);

            if (canDamage == false)
                return;

            CheckForBlock(damageTarget);


            DamageTarget(damageTarget);
        }
    }

    protected virtual void CheckForBlock(CharacterManager damageTarget)
    {
        if (charactersDamaged.Contains(damageTarget))
            return;

        GetBlockingDotValues(damageTarget);

        if (damageTarget.isBlocking && dotValueFromAttackToDamageTarget > 0.3f) 
        {
            charactersDamaged.Add(damageTarget);
            TakeBlockingDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeBlockedDamageEffect);

            damageEffect.physicalDamage = physicalDamage;
            damageEffect.magicDamage = magicDamage;
            damageEffect.fireDamage = fireDamage;
            damageEffect.holyDamage = holyDamage;
            damageEffect.poiseDamage = poiseDamage;
            damageEffect.baseStaminaDamage = poiseDamage;
            damageEffect.contactPoint = contanctPoint;

            damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);

        }
    }

    protected virtual void GetBlockingDotValues(CharacterManager damageTarget)
    {
        directionFromAttackToDamageTarget = transform.position - damageTarget.transform.position;
        dotValueFromAttackToDamageTarget = Vector3.Dot(directionFromAttackToDamageTarget, damageTarget.transform.forward);
    }

    protected virtual void DamageTarget(CharacterManager damageTarget)
    {
        if(charactersDamaged.Contains(damageTarget))
            return;

        charactersDamaged.Add(damageTarget);

        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);

        damageEffect.physicalDamage = physicalDamage;   
        damageEffect.magicDamage = magicDamage;
        damageEffect.fireDamage = fireDamage;   
        damageEffect.holyDamage = holyDamage;
        damageEffect.poiseDamage = poiseDamage;
        damageEffect.contactPoint = contanctPoint;
        damageEffect.lightningDamage = lightningDamage;
        damageEffect.weaponElement = element;
        damageEffect.weaponType = weaponModelType;
        damageEffect.weaponElement = element;


        

        damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);

    }

    public virtual void EnableDamageCollider()
    {
        damageCollider.enabled = true;
    }

    public virtual void DisableDamageCollider()
    {
        damageCollider.enabled = false;
        charactersDamaged.Clear();
    }
}