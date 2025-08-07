using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "IdleState", menuName = "A.I/States/Idle")]
public class IdleState : AIState
{
    [Header("Idle State")]
    public IdleStateMode idleStateMode;

    [Header("Patrol Options")]
    public AIPatrolPath aIPatrolPath;
    public int patrolPathID;
    [SerializeField] bool hasFoundClosestPointNearCharacterSpawn = false; // if the character spawns closer to the second point, start at 2nd point
    [SerializeField] bool patrolComplete = false;
    [SerializeField] bool repeatPatrol = false;
    [SerializeField] int patrolDestinationIndex;
    [SerializeField] bool hasPatrolDestination = false;
    [SerializeField] Vector3 currentPatrolDestination;
    [SerializeField] float distanceFromCurrentDestination;
    [SerializeField] float timeBetweenPatrols = 15;
    [SerializeField] float restTimer = 0;

    [Header("Sleep Options")]   
    public bool willInvestigateSound = true;
    private bool sleepAnimationSet = false;
    [SerializeField] string sleepAnimation = "Sleep_01";
    [SerializeField] string wakeAnimation = "Wake_01";


    public override AIState Tick(AICharacterManager aiCharacter)
    {


        if(aiCharacter.isAwake)
            aiCharacter.aiCharacterCombatManager.FindTargetViaLineOfSight(aiCharacter);
       
        switch (idleStateMode)
        {
            case IdleStateMode.Idle:
                return Idle(aiCharacter);
            case IdleStateMode.Patrol:
                return Patrol(aiCharacter);
            case IdleStateMode.Sleep:
                return SleepUntilDisturbed(aiCharacter);
            default:
                break;
        }

        return this;
    }

    protected virtual AIState Idle(AICharacterManager aiCharacter)
    {
        if (aiCharacter.characterCombatManager.currentTarget != null)
        {
            return SwitchState(aiCharacter, aiCharacter.pursueState);
            // If the character has a target, switch to the Attack state
            //return aiCharacter.characterCombatManager.attackState;

        }
        else
        {
            return this;
        }
    }

    protected virtual AIState Patrol(AICharacterManager aiCharacter)
    {
        if(!aiCharacter.aiCharacterLocomotionManager.isGrounded)
            return this;

        if (aiCharacter.isPerformingAction)
        {
            aiCharacter.navMeshAgent.enabled = false;
            aiCharacter.isMoving = false;
            return this;
        }

        if (!aiCharacter.navMeshAgent.enabled)
            aiCharacter.navMeshAgent.enabled = true;

        if (aiCharacter.characterCombatManager.currentTarget != null)
            return SwitchState(aiCharacter, aiCharacter.pursueState);

        if (patrolComplete && repeatPatrol)
        {
            if (timeBetweenPatrols > restTimer)
            {
                aiCharacter.navMeshAgent.enabled = false;
                aiCharacter.isMoving = false;
                restTimer += Time.deltaTime;
            }
            else
            {
                patrolDestinationIndex = -1;
                hasPatrolDestination = false;
                currentPatrolDestination = aiCharacter.transform.position;
                patrolComplete = false;
                restTimer = 0;
            }
        }
        else if (patrolComplete && !repeatPatrol)
        {
            aiCharacter.navMeshAgent.enabled = false;
            aiCharacter.isMoving = false;
        }

        // check if the character is close to the patrol point if not make it smaller
        if (hasPatrolDestination)
        {
            distanceFromCurrentDestination = Vector3.Distance(aiCharacter.transform.position, currentPatrolDestination);

            if (distanceFromCurrentDestination > 2)
            {
                aiCharacter.isMoving = true;
                aiCharacter.aiCharacterLocomotionManager.RotateTowardsAgent(aiCharacter);
            }
            else
            {
                currentPatrolDestination = aiCharacter.transform.position;
                hasPatrolDestination = false;
            }
        }
        else
        {
            patrolDestinationIndex++;   

            if (patrolDestinationIndex > aIPatrolPath.patrolPoints.Count -1)
            {
                patrolComplete = true;
                return this;
            }

            if (!hasFoundClosestPointNearCharacterSpawn)
            {
                hasFoundClosestPointNearCharacterSpawn = true;
                float closetestDistance = Mathf.Infinity;

                for (int i = 0; i < aIPatrolPath.patrolPoints.Count; i++)
                {
                    float distance = Vector3.Distance(aiCharacter.transform.position, aIPatrolPath.patrolPoints[i]);
                    if (distance < closetestDistance)
                    {
                        closetestDistance = distance;
                        patrolDestinationIndex = i;
                        currentPatrolDestination = aIPatrolPath.patrolPoints[i];
                    }
                }
            }
            else
            {
                currentPatrolDestination = aIPatrolPath.patrolPoints[patrolDestinationIndex];
            }

            hasPatrolDestination = true;

        }
        NavMeshPath path = new();

        aiCharacter.navMeshAgent.CalculatePath(currentPatrolDestination, path);
        aiCharacter.navMeshAgent.SetPath(path);

        return this;
    }

    protected virtual AIState SleepUntilDisturbed(AICharacterManager aiCharacter)
    {
        aiCharacter.navMeshAgent.enabled = false;

        if (!sleepAnimationSet && !aiCharacter.isAwake)
        {
            sleepAnimationSet = true;
            aiCharacter.sleepAnimation = sleepAnimation;
            aiCharacter.wakeAnimation = wakeAnimation;
            aiCharacter.characterAnimatorManager.PlayerTargetActionAnimation(aiCharacter.sleepAnimation, false);
        }


        if (aiCharacter.characterCombatManager.currentTarget != null && !aiCharacter.isAwake)
        {
            aiCharacter.isAwake = true;

            if (!aiCharacter.isPerformingAction && !aiCharacter.isDead)
                aiCharacter.characterAnimatorManager.PlayerTargetActionAnimation(aiCharacter.wakeAnimation, true);


            return SwitchState(aiCharacter, aiCharacter.pursueState);
        }


        return this;    
    }

    protected override void ResetStateFlag(AICharacterManager aiCharacter)
    {
        base.ResetStateFlag(aiCharacter);
        sleepAnimationSet = false;  
    }
}

