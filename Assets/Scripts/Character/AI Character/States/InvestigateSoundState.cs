using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "InvestigateSoundState", menuName = "A.I/States/InvestigateSound")]
public class InvestigateSoundState : AIState
{
    [Header("Flags")]
    [SerializeField] bool destinationSet = false;
    [SerializeField] bool destinationReached = false;

    [Header("Invenstigate")]
    public float investigateDuration = 2f;
    public float investigateTimer = 0f;

    [Header("Position")]
    public Vector3 destinationPosition = Vector3.zero;


    public override AIState Tick(AICharacterManager aiCharacter)
    {
        if (aiCharacter.isPerformingAction)
            return this;

        aiCharacter.aiCharacterCombatManager.FindTargetViaLineOfSight(aiCharacter);

        if (aiCharacter.aiCharacterCombatManager.currentTarget != null)
            return SwitchState(aiCharacter, aiCharacter.pursueState);

        if (!destinationSet)
        {
            destinationSet = true;
            aiCharacter.aiCharacterCombatManager.PivotTowardsPosition(aiCharacter, destinationPosition);
            aiCharacter.navMeshAgent.enabled = true;

            if (!IsDestinationReachable(aiCharacter, destinationPosition))
            {
                NavMeshHit hit;

                if (NavMesh.SamplePosition(destinationPosition, out hit, 2, NavMesh.AllAreas))
                {
                    NavMeshPath partialPath = new();
                    aiCharacter.navMeshAgent.CalculatePath(hit.position, partialPath);
                    aiCharacter.navMeshAgent.SetPath(partialPath);
                }
            }
            else
            {
                NavMeshPath path = new();
                aiCharacter.navMeshAgent.CalculatePath(destinationPosition, path);
                aiCharacter.navMeshAgent.SetPath(path);
            }
        }

        aiCharacter.aiCharacterCombatManager.RotateTowardsAgent(aiCharacter);

        float distanceFromDestination = Vector3.Distance(aiCharacter.transform.position, destinationPosition);

        //TODOL use this to do things on arrival
        if (distanceFromDestination <= aiCharacter.navMeshAgent.stoppingDistance)
        {
            destinationReached = true;
        }
        else
        {
        }

        if (destinationReached)
        {
            if (investigateTimer < investigateDuration)
            {
                investigateTimer += Time.deltaTime;
            }
            else
            {
                return SwitchState(aiCharacter, aiCharacter.idleState);
            }
        }

        return this;
    }

    protected override void ResetStateFlag(AICharacterManager aiCharacter)
    {
        base.ResetStateFlag(aiCharacter);

        aiCharacter.navMeshAgent.enabled = false;
        destinationReached = false;
        destinationSet = false;
        investigateTimer = 0f;
        destinationPosition = Vector3.zero;
    }
}
