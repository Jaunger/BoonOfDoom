using NUnit.Framework;
using System;
using UnityEngine;

public class AICharacterCombatManager : CharacterCombatManager
{
    protected AICharacterManager aiCharacter;


    [Header("Pivot")]
    public bool canPivot = true;

    [Header("Recovery Timer")]
    public float actionRecoveryTimer = 0f;

    [Header("Target Info")]
    public float distanceFromTarget;    
    public float viewableAngle;
    public Vector3 targetsDirection;

    [Header("Detection")]
    [SerializeField] private float detectionRadius = 10f;
    public float minFOV = -35f;
    public float maxFOV = 35f;

    [Header("Rotation")]
    public float rotationSpeed = 25f;

    [Header("Activation Range")]
    public PlayerManager playerWithinReach;

    [Header("Rewards")]
    [SerializeField] public int weaponEXPReward = 30;

    protected override void Awake()
    {
        base.Awake();

        aiCharacter = GetComponent<AICharacterManager>();
        lockOnTransform = GetComponentInChildren<LockOnTransform>().transform;
    }

    public void AwardSoulsOnDeath(PlayerManager player)
    {
        if (player.characterType == CharacterType.AI)
            return;
        

        //TODO: can add effect here

        player.playerStatManager.AddSouls(aiCharacter.characterStatManager.soulsDroppedOnDeath);

    }

    public void FindTargetViaLineOfSight(AICharacterManager aiCharacter, bool isCheckingAgain = false)
    {
        if (currentTarget != null && !isCheckingAgain)
        {
            // If we already have a target, return early
            return;
        }

        Collider[] hitColliders = Physics.OverlapSphere(aiCharacter.transform.position, detectionRadius, WorldUtilityManager.instance.GetCharacterLayers());

        for (int i = 0; i < hitColliders.Length; i++)
        {
   
            CharacterManager targetCharacter = hitColliders[i].transform.GetComponent<CharacterManager>();
            if (targetCharacter == null)
                continue;

            if (targetCharacter == aiCharacter)
                continue;

            if (targetCharacter.isDead)
                continue;

            if (WorldUtilityManager.instance.CanIdamageThisTarget(aiCharacter.characterType, targetCharacter.characterType))
            {
                Vector3 directionToTarget = targetCharacter.transform.position - aiCharacter.transform.position;
                float angle = Vector3.Angle(directionToTarget, aiCharacter.transform.forward);

                if (angle > minFOV && angle < maxFOV)
                {
                    if (Physics.Linecast(aiCharacter.characterCombatManager.lockOnTransform.position, targetCharacter.characterCombatManager.lockOnTransform.position,
                        WorldUtilityManager.instance.GetEnvLayers()))
                    {
                        Debug.DrawLine(aiCharacter.characterCombatManager.lockOnTransform.position, targetCharacter.characterCombatManager.lockOnTransform.position);
                    }
                    else
                    {
                        directionToTarget = targetCharacter.transform.position - transform.position;
                        viewableAngle = WorldUtilityManager.instance.GetAngleToTarget(transform, directionToTarget);
                        aiCharacter.characterCombatManager.SetTarget(targetCharacter);
                        if(canPivot)
                            PivotTowardsTarget(aiCharacter);
                    }
                }
            }

            if (isCheckingAgain)
                currentTarget = null;
        }
    }

    public virtual void PivotTowardsTarget(AICharacterManager aiCharacter)
    {
        if (aiCharacter.isPerformingAction)
            return;

        // TODO: test this out and tweak numbers also doesnt work


        if (viewableAngle >= 20 && viewableAngle <= 60)
        {
            aiCharacter.characterAnimatorManager.PlayerTargetActionAnimation("Turn_R_45", true);
        }
        else if (viewableAngle <= -20 && viewableAngle >= 60)
        {
            aiCharacter.characterAnimatorManager.PlayerTargetActionAnimation("Turn_L_45", true);
        }
        else if (viewableAngle >= 61 && viewableAngle <= 110)
        {
            aiCharacter.characterAnimatorManager.PlayerTargetActionAnimation("Turn_R_90", true);
        }
        else if (viewableAngle <= -61 && viewableAngle >= -110)
        {
            aiCharacter.characterAnimatorManager.PlayerTargetActionAnimation("Turn_L_90", true);
        }
        else if (viewableAngle >= 111 && viewableAngle <= 145)
        {
            aiCharacter.characterAnimatorManager.PlayerTargetActionAnimation("Turn_R_135", true);
        }
        else if (viewableAngle <= -111 && viewableAngle >= -145)
        {
            aiCharacter.characterAnimatorManager.PlayerTargetActionAnimation("Turn_L_135", true);
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

    public virtual void PivotTowardsPosition(AICharacterManager aiCharacter, Vector3 position)
    {
        if (aiCharacter.isPerformingAction)
            return;

        Vector3 targetDirection = position - aiCharacter.transform.position;
        float viewableAngle = WorldUtilityManager.instance.GetAngleToTarget(aiCharacter.transform, targetDirection);
        // TODO: test this out and tweak numbers also doesnt work


        if (viewableAngle >= 20 && viewableAngle <= 60)
        {
            aiCharacter.characterAnimatorManager.PlayerTargetActionAnimation("Turn_R_45", true);
        }
        else if (viewableAngle <= -20 && viewableAngle >= 60)
        {
            aiCharacter.characterAnimatorManager.PlayerTargetActionAnimation("Turn_L_45", true);
        }
        else if (viewableAngle >= 61 && viewableAngle <= 110)
        {
            aiCharacter.characterAnimatorManager.PlayerTargetActionAnimation("Turn_R_90", true);
        }
        else if (viewableAngle <= -61 && viewableAngle >= -110)
        {
            aiCharacter.characterAnimatorManager.PlayerTargetActionAnimation("Turn_L_90", true);
        }
        else if (viewableAngle >= 111 && viewableAngle <= 145)
        {
            aiCharacter.characterAnimatorManager.PlayerTargetActionAnimation("Turn_R_135", true);
        }
        else if (viewableAngle <= -111 && viewableAngle >= -145)
        {
            aiCharacter.characterAnimatorManager.PlayerTargetActionAnimation("Turn_L_135", true);
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

    public virtual void AlertCharacterToSound(Vector3 positionOfSound)
    {
        if (aiCharacter.isDead)
            return;

        if (aiCharacter.idleState == null)
            return;

        if (aiCharacter.investigateSoundState == null)
            return;

        if (!aiCharacter.idleState.willInvestigateSound)
            return;

        if (aiCharacter.idleState.idleStateMode == IdleStateMode.Sleep && !aiCharacter.isAwake)
        {
            aiCharacter.isAwake = true;
            Debug.Log("Waking up");
            aiCharacter.characterAnimatorManager.PlayerTargetActionAnimation(aiCharacter.wakeAnimation, true);    
        }
           
       
        aiCharacter.investigateSoundState.destinationPosition = positionOfSound;
        aiCharacter.currentState = aiCharacter.currentState.SwitchState(aiCharacter, aiCharacter.investigateSoundState);
    }

    public void HandleActionRecovery(CharacterManager aiCharacter)
    {
        if (actionRecoveryTimer > 0)
        {
            if (!aiCharacter.isPerformingAction)
            {
                actionRecoveryTimer -= Time.deltaTime;
            }
        }
    }

    public void RotateTowardsAgent(AICharacterManager aiCharacter)
    {
        if (aiCharacter.isMoving)
        {
            aiCharacter.transform.rotation = aiCharacter.navMeshAgent.transform.rotation;
        }
    }

    public void RotateTowardsTargetWhilstAttacking(AICharacterManager aiCharacter)
    {
        if(currentTarget == null)
            return;

    
        if(!aiCharacter.aiCharacterLocomotionManager.canRotate)
            return;

        if(!aiCharacter.isPerformingAction)
            return;

        Vector3 targetDirection = currentTarget.transform.position - aiCharacter.transform.position;
        targetDirection.y = 0f;
        targetDirection.Normalize();

        if(targetDirection == Vector3.zero)
            targetDirection = aiCharacter.transform.forward;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        
        aiCharacter.transform.rotation = Quaternion.Slerp(aiCharacter.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);


    }
}