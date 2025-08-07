using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "CombatStanceState", menuName = "A.I/States/CombatStance")]
public class CombatStanceState : AIState
{
    // 1. Select an attack action based on the distance and angle to the target.
    // 2. combat logic whilst waiting to attack
    // 3. target out of combat range, go to pursue
    // 4. target no longer present, switch to idle

    [Header("Attack")]
    public List<AICharacterAttackAction> aiCharacterAttacks;
    [SerializeField] protected List<AICharacterAttackAction> potentialAttacks;
    [SerializeField] private AICharacterAttackAction currentAttack;
    [SerializeField] private AICharacterAttackAction previousAttack;
    protected bool hasAttack = false;

    [Header("Combo")]
    [SerializeField] protected bool canPerformCombo = false;
    [SerializeField] protected int chanceToCombo = 25;
    protected bool hasRolledForCombo = false;

    [Header("Combat Range")]
    [SerializeField] public float maxCombatRange = 5f;

    [Header("Circling")]
    [SerializeField] bool willCircleTarget = false;
    private bool hasSetCirclePath = false;
    private float strafeMoveAmount;

    public override AIState Tick(AICharacterManager aiCharacter)
    {
        if (aiCharacter.isPerformingAction)
            return this;

        if (!aiCharacter.navMeshAgent.enabled)
            aiCharacter.navMeshAgent.enabled = true;

        if (aiCharacter.aiCharacterCombatManager.currentTarget.isDead)
            aiCharacter.aiCharacterCombatManager.SetTarget(null);

        if (aiCharacter.aiCharacterCombatManager.canPivot)
        {
            if (!aiCharacter.isMoving)
            {
                if (aiCharacter.aiCharacterCombatManager.viewableAngle < -30 || aiCharacter.aiCharacterCombatManager.viewableAngle > 30)
                    aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
            }
        }

        aiCharacter.aiCharacterCombatManager.RotateTowardsAgent(aiCharacter);

        if (aiCharacter.aiCharacterCombatManager.currentTarget == null)
            return SwitchState(aiCharacter, aiCharacter.idleState);

        if (willCircleTarget)
            SetCirclePath(aiCharacter);

        if (!hasAttack)
            GetNewAttack(aiCharacter);
        else
        {

            aiCharacter.attackState.currentAttack = currentAttack;
            // roll for combo
            return SwitchState(aiCharacter, aiCharacter.attackState);
        }

        if (aiCharacter.aiCharacterCombatManager.distanceFromTarget > maxCombatRange)
            return SwitchState(aiCharacter, aiCharacter.pursueState);

        NavMeshPath navMeshPath = new();
        aiCharacter.navMeshAgent.CalculatePath(aiCharacter.characterCombatManager.currentTarget.transform.position, navMeshPath);
        aiCharacter.navMeshAgent.SetPath(navMeshPath);

        return this;
    }

    protected virtual void GetNewAttack(AICharacterManager aiCharacter)
    {
        potentialAttacks = new List<AICharacterAttackAction>();

        foreach (var potentialAttack in aiCharacterAttacks)
        {
            if (potentialAttack.minAttackDistance > aiCharacter.aiCharacterCombatManager.distanceFromTarget)
                continue;


            if (potentialAttack.maxAttackDistance < aiCharacter.aiCharacterCombatManager.distanceFromTarget)
                continue;

            if (potentialAttack.minAttackAngle > aiCharacter.aiCharacterCombatManager.viewableAngle)
                continue;

            if (potentialAttack.maxAttackAngle < aiCharacter.aiCharacterCombatManager.viewableAngle)
                continue;

            potentialAttacks.Add(potentialAttack);
        }

        if (potentialAttacks.Count <= 0)
            return;

        var totalWeigt = 0;

        foreach (var attack in potentialAttacks)
        {
            totalWeigt += attack.attackWeight;
        }

        var randomWeight = Random.Range(1, 1 + totalWeigt);
        var processWeight = 0;

        foreach (var attack in potentialAttacks)
        {
            processWeight += attack.attackWeight;
            if (processWeight >= randomWeight)
            {
                currentAttack = attack;
                previousAttack = currentAttack;
                hasAttack = true;
                return;
            }

        }
    }

    protected virtual bool RollForComboChange(int outcomeChance)
    {
        bool outcomeWIllBePerformed = false;
        int randomRoll = Random.Range(0, 100);

        if (randomRoll < outcomeChance)
        {
            outcomeWIllBePerformed = true;
        }

        return outcomeWIllBePerformed;
    }

    protected virtual void SetCirclePath(AICharacterManager aiCharacter)
    {
        if (Physics.CheckSphere(aiCharacter.aiCharacterCombatManager.lockOnTransform.position, aiCharacter.characterController.radius + 0.25f,
            WorldUtilityManager.instance.GetEnvLayers()))
        {
            //STOP strafing/curcling, instead , move towards target
            Debug.Log("we are colliding, ending strafe/circle");
            aiCharacter.characterAnimatorManager.SetAnimatorMovementParameters(0f, Mathf.Abs(strafeMoveAmount));

            return;
        }

        //Strafe
        Debug.Log("we are strafe " );

        aiCharacter.characterAnimatorManager.SetAnimatorMovementParameters(strafeMoveAmount, 0f);   

        if (hasSetCirclePath)
            return;

        hasSetCirclePath = true;

        int leftOrRight = Random.Range(0, 100); // 0 for left, 1 for right

        if (leftOrRight >= 50)
        {
            // Circle to the left
        }
        else
        {
            // Circle to the right
        }


    }

    protected override void ResetStateFlag(AICharacterManager aiCharacter)
    {
        base.ResetStateFlag(aiCharacter);

        hasRolledForCombo = false;
        hasAttack = false;
        hasSetCirclePath = false;
        strafeMoveAmount = 0f;
    }

}
