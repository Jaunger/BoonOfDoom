using UnityEngine;

public class AICharacterAnimatorManager : CharacterAnimatorManager
{
    AICharacterManager aICharacter;

    protected override void Awake()
    {
        base.Awake();

        aICharacter = GetComponent<AICharacterManager>();
    }

    private void OnAnimatorMove()
    {
        if (!aICharacter.aiCharacterLocomotionManager.isGrounded)
            return;

        Vector3 velocity = aICharacter.animator.deltaPosition;  

  
        aICharacter.characterController.Move(velocity);
        aICharacter.transform.rotation *= aICharacter.animator.deltaRotation;
    }
}
