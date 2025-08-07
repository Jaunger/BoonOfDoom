using UnityEngine;

public class AIFrankCombatManager : AICharacterCombatManager
{
    AIFrankCharacterManager frankManager;

    [Header("Damage Colliders")]
    [SerializeField] FrankAxeDamageCollider axeDamageCollider;
    [SerializeField] Transform frankKickingFoot;
    [SerializeField] float slamAttackAOERadius = 2.5f;

    [Header("Damage")]
    public int baseDamage = 25;
    public int basePoiseDamage = 15;
    [SerializeField] float attack01DamageModifer = 1.0f;
    [SerializeField] float attack03DamageModifer = 1.1f;
    [SerializeField] float attack02DamageModifer = 1.4f;

    protected override void Awake()
    {
        base.Awake();
        frankManager = GetComponent<AIFrankCharacterManager>();
    }

    public void SetAttack01Damage()
    {
        aiCharacter.characterSoundFXManager.PlayAttackGrunt();
        axeDamageCollider.physicalDamage = baseDamage * attack01DamageModifer;
        axeDamageCollider.poiseDamage = basePoiseDamage * attack01DamageModifer;
    }

    public void SetAttack02Damage()
    {
        aiCharacter.characterSoundFXManager.PlayAttackGrunt();
        axeDamageCollider.physicalDamage = baseDamage * attack02DamageModifer;
        axeDamageCollider.poiseDamage = basePoiseDamage * attack02DamageModifer;
    }

    public void SetAttack03Damage()
    {
        aiCharacter.characterSoundFXManager.PlayAttackGrunt();
        axeDamageCollider.physicalDamage = baseDamage * attack03DamageModifer;
        axeDamageCollider.poiseDamage = basePoiseDamage * attack03DamageModifer;
    }

    public void OpenAxeDamageCollider()
    {
        axeDamageCollider.EnableDamageCollider();
        frankManager.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(frankManager.frankSoundFXManager.wooshes));
    }

    public void DisableAxeDamageCollider()
    {
        axeDamageCollider.DisableDamageCollider();
    }

    public void ActivateFrankKick()
    {
        Collider[] colliders = Physics.OverlapSphere(frankKickingFoot.position, slamAttackAOERadius);

    }

    public override void PivotTowardsTarget(AICharacterManager aiCharacter)
    {
        if (aiCharacter.isPerformingAction)
            return;
        
        if (viewableAngle >= 61 && viewableAngle <= 110)
        {
            aiCharacter.characterAnimatorManager.PlayerTargetActionAnimation("Turn_R_90", true);
        }
        else if (viewableAngle <= -61 && viewableAngle >= -110)
        {
            aiCharacter.characterAnimatorManager.PlayerTargetActionAnimation("Turn_L_90", true);
        }
        else if (viewableAngle >= 111 && viewableAngle <= 145)
        {
            aiCharacter.characterAnimatorManager.PlayerTargetActionAnimation("Turn_R_90", true);
        }
        else if (viewableAngle <= -111 && viewableAngle >= -145)
        {
            aiCharacter.characterAnimatorManager.PlayerTargetActionAnimation("Turn_L_90", true);
        }

        else if (viewableAngle >= 146 && viewableAngle <= 180)
        {
            aiCharacter.characterAnimatorManager.PlayerTargetActionAnimation("Turn_R_180", true);
        }
        else if (viewableAngle <= -146 && viewableAngle >= -180)
        {
            aiCharacter.characterAnimatorManager.PlayerTargetActionAnimation("Turn_L_180", true);
        }
    }
}
