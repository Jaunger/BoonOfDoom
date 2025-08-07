using UnityEngine;
using UnityEngine.AI;

public class AIState : ScriptableObject
{
    public virtual AIState Tick(AICharacterManager aiCharacter)
    {

        return this;
    }

    public virtual AIState SwitchState(AICharacterManager aiCharacter, AIState newState)
    {
        ResetStateFlag(aiCharacter);
        return newState;
    }

    protected virtual void ResetStateFlag(AICharacterManager aiCharacter)
    {
        // Reset any state flags or variables here if needed
    }

    public virtual bool IsDestinationReachable(AICharacterManager aiCharacter, Vector3 destination)
    {
        aiCharacter.navMeshAgent.enabled = true;

        NavMeshPath path = new NavMeshPath();

        if (aiCharacter.navMeshAgent.CalculatePath(destination, path) && path.status == NavMeshPathStatus.PathComplete)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
