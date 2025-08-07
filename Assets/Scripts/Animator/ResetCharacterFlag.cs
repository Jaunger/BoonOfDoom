using UnityEngine;

public class ResetCharacterFlag : StateMachineBehaviour
{
    CharacterManager character;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (character == null)
        {
            character = animator.GetComponent<CharacterManager>();
        }
        if (character == null)
        {
            return;
        }
        character.isPerformingAction = false;
        character.characterAnimatorManager.applyRootMotion = false;
        character.characterLocomotionManager.canMove = true;
        character.characterLocomotionManager.canRotate = true;
        character.isJumping = false;
        character.characterLocomotionManager.canRun = true;
        character.characterLocomotionManager.isRolling = false;
        character.isInvulnerable = false;
        character.isAttacking = false;
        character.characterCombatManager.DisableCanCombo();
        character.characterCombatManager.DisableCanDoRollingAttack();
        character.characterCombatManager.DisableCanDoBackstepAttack();

        if(character.characterEffectsManager.activeQuickSlotItemFX != null)
        {
            Destroy(character.characterEffectsManager.activeQuickSlotItemFX);
        }
        //character.animator.speed = 1.0f;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
