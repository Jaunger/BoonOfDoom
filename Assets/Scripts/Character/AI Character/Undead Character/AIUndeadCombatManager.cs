using UnityEngine;

public class AIUndeadCombatManager : AICharacterCombatManager
{
    [Header("Damage Colliders")]
    [SerializeField] UndeadHandDamageCollider leftHandDamageCollider;
    [SerializeField] UndeadHandDamageCollider rightHandDamageCollider;

    [Header("Damage")]
    public int baseDamage = 25;
    public int basePoiseDamage = 15;
    [SerializeField] float attack01DamageModifer = 1.0f;
    [SerializeField] float attack03DamageModifer = 1.1f;
    [SerializeField] float attack02DamageModifer = 1.4f;

    public void SetAttack01Damage()
    {
        rightHandDamageCollider.physicalDamage = baseDamage * attack01DamageModifer;
        leftHandDamageCollider.physicalDamage = baseDamage * attack01DamageModifer;
        rightHandDamageCollider.poiseDamage = basePoiseDamage * attack01DamageModifer;
    }


    public void SetAttack02Damage()
    {
        rightHandDamageCollider.physicalDamage = baseDamage * attack02DamageModifer;
        leftHandDamageCollider.physicalDamage = baseDamage * attack02DamageModifer;
        rightHandDamageCollider.poiseDamage = basePoiseDamage * attack02DamageModifer;
    }

    public void SetAttack03Damage()
    {
        rightHandDamageCollider.physicalDamage = baseDamage * attack03DamageModifer;
        leftHandDamageCollider.physicalDamage = baseDamage * attack03DamageModifer;
        rightHandDamageCollider.poiseDamage = basePoiseDamage * attack03DamageModifer;
    }

    public void OpenRightHandDamageCollider()
    {
        aiCharacter.characterSoundFXManager.PlayAttackGrunt();
        rightHandDamageCollider.EnableDamageCollider();
    }

    public void DisableRightHandDamageCollider()
    {
        rightHandDamageCollider.DisableDamageCollider();
    }

    public void OpenLeftHandDamageCollider()
    {
        aiCharacter.characterSoundFXManager.PlayAttackGrunt();
        leftHandDamageCollider.EnableDamageCollider();
    }

    public void DisableLeftHandDamageCollider()
    {
        leftHandDamageCollider.DisableDamageCollider();
    }
}
