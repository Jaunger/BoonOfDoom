using UnityEngine;

public class CharacterCombatManager : MonoBehaviour
{
    protected CharacterManager character;

    [Header("Last Attack")]
    public string lastAttackAnimation;

    [Header("Attack Target")]
    public CharacterManager currentTarget;

    [Header("Attack Type")]
    public AttackType currentAttackType;

    [Header("Lock On Transform")]
    public Transform lockOnTransform;

    [Header("Attack Flag")]
    public bool canDoRollingAttack = false;
    public bool canDoBackstepAttack = false;
    public bool canBlock = true;



    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    public virtual void SetTarget(CharacterManager newTarget)
    {
        if (newTarget != null)
        {
            currentTarget = newTarget;
        }
        else
        {
            currentTarget = null;
        }
    }

    public virtual void EnableCanCombo()
    {

    }

    public virtual void DisableCanCombo()
    {

    }
    public void EnableIsInvulnerable()
    {
        character.isInvulnerable = true;
    }

    public void DisableIsInvulnerable()
    {
        character.isInvulnerable = false;
    }

    public void EnableCanDoRollingAttack()
    {
        canDoRollingAttack = true;
    }

    public void DisableCanDoRollingAttack()
    {
        canDoRollingAttack = false;
    }

    public void EnableCanDoBackstepAttack()
    {
        canDoBackstepAttack = true;
    }

    public void DisableCanDoBackstepAttack()
    {
        canDoBackstepAttack = false;
    }
}
