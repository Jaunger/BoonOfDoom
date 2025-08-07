using UnityEngine;

[CreateAssetMenu(fileName = "BossSleepState", menuName = "A.I/States/Boss Sleep State")]
public class BossSleepState : AIState
{
    [SerializeField] string sleepAnimation = "Sleep_01";
    [SerializeField] string wakeAnimation = "Wake_01"; 
    private bool sleepAnimationSet = false;

    public bool hasBeenAwakened = false;

    public override AIState Tick(AICharacterManager aiCharacter)
    {
        aiCharacter.navMeshAgent.enabled = false;

        if (!hasBeenAwakened)
            return HasNotBeenAwakened(aiCharacter);
        else
            return HasBeenAwakened(aiCharacter);
    }

    private AIState HasBeenAwakened(AICharacterManager aiCharacter)
    {
        if (aiCharacter.characterCombatManager.currentTarget != null && !aiCharacter.isAwake)
        {
            aiCharacter.isAwake = true;
            return SwitchState(aiCharacter, aiCharacter.pursueState);
        }

        return this;
    }

    private AIState HasNotBeenAwakened(AICharacterManager aiCharacter)
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
}
