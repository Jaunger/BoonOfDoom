using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "PursueTargetState", menuName = "A.I/States/PursueTarget")]
public class PursueTargetState : AIState
{
    public override AIState Tick(AICharacterManager aiCharacter)
    {


        // Check if we are perfoming an action ( do nothing until complete)
        if (aiCharacter.isPerformingAction)
         {
            aiCharacter.characterAnimatorManager.SetAnimatorMovementParameters(0, 1);
            return this;
        }


        // check if target is null, if no target goto idle state
        if (aiCharacter.characterCombatManager.currentTarget == null)
            return SwitchState(aiCharacter, aiCharacter.idleState);

        // make sure navmesh agent is active, if not , activate it
        if (!aiCharacter.navMeshAgent.enabled)
            aiCharacter.navMeshAgent.enabled = true;


        if (aiCharacter.aiCharacterCombatManager.canPivot)
        {
            if (aiCharacter.aiCharacterCombatManager.viewableAngle > aiCharacter.aiCharacterCombatManager.maxFOV ||
                aiCharacter.aiCharacterCombatManager.viewableAngle < aiCharacter.aiCharacterCombatManager.minFOV)
                aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
        }

        aiCharacter.aiCharacterLocomotionManager.RotateTowardsAgent(aiCharacter);

        //if (aiCharacter.aiCharacterCombatManager.distanceFromTarget <= aiCharacter.combbatStanceState.maxCombatRange)
        //    return SwitchState(aiCharacter, aiCharacter.combbatStanceState);

        if (aiCharacter.aiCharacterCombatManager.distanceFromTarget <= aiCharacter.navMeshAgent.stoppingDistance)
            return SwitchState(aiCharacter, aiCharacter.combatStanceState);


        // if target not reachable, and they are far away, return to idle state
        if(aiCharacter.aiCharacterCombatManager.distanceFromTarget > 15f && !(aiCharacter is AIBossCharacterManager))
        {
            aiCharacter.navMeshAgent.enabled = false;
            aiCharacter.aiCharacterCombatManager.SetTarget(null);
            return SwitchState(aiCharacter, aiCharacter.idleState);
        }



        // pursue target
        //aiCharacter.navMeshAgent.SetDestination(aiCharacter.characterCombatManager.currentTarget.transform.position);

        NavMeshPath navMeshPath = new NavMeshPath();
        aiCharacter.navMeshAgent.CalculatePath(aiCharacter.characterCombatManager.currentTarget.transform.position, navMeshPath);
        aiCharacter.navMeshAgent.SetPath(navMeshPath);

        return this;
    }
}
