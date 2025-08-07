using UnityEngine;

public class AxeDamageCollider : DamageCollider
{
    [SerializeField] AIBossCharacterManager bossCharacter;

    protected override void Awake()
    {
        base.Awake();

        damageCollider = GetComponent<Collider>();
    }

    protected override void DamageTarget(CharacterManager damageTarget)
    {
        //  we dont want to damage the same target more than once in a single attack
        if (charactersDamaged.Contains(damageTarget))
            return;

        if (WorldUtilityManager.instance.CanIdamageThisTarget(bossCharacter.characterType, damageTarget.characterType) == false)
            return;

        charactersDamaged.Add(damageTarget);

        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
        damageEffect.physicalDamage = physicalDamage;
        damageEffect.magicDamage = magicDamage;
        damageEffect.fireDamage = fireDamage;
        damageEffect.holyDamage = holyDamage;
        damageEffect.poiseDamage = poiseDamage;
        damageEffect.contactPoint = contanctPoint;
        damageEffect.angleHitFrom = Vector3.SignedAngle(bossCharacter.transform.forward, damageTarget.transform.forward, Vector3.up);


        damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);
    }

    protected override void GetBlockingDotValues(CharacterManager damageTarget)
    {
        directionFromAttackToDamageTarget = bossCharacter.transform.position - damageTarget.transform.position;
        dotValueFromAttackToDamageTarget = Vector3.Dot(directionFromAttackToDamageTarget, damageTarget.transform.forward);
    }
}
