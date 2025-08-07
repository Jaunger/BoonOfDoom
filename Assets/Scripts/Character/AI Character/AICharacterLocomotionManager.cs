using UnityEngine;

public class AICharacterLocomotionManager : CharacterLocomotionManager
{
    AICharacterManager aiCharacter;

    protected override void Awake()
    {
        base.Awake();
        aiCharacter = GetComponent<AICharacterManager>();

    }

    public void RotateTowardsAgent(AICharacterManager aiCharacter)
    {
        if (aiCharacter.isMoving)
        {
            aiCharacter.transform.rotation = aiCharacter.navMeshAgent.transform.rotation;
        }
    }

    protected override void Update()
    {
        base.Update();

        aiCharacter.verticalMovement = aiCharacter.animator.GetFloat("Vertical");
        aiCharacter.horizontalMovement = aiCharacter.animator.GetFloat("Horizontal");
    }
}
