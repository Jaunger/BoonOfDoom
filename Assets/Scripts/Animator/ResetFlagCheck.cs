using UnityEngine;
using UnityEngine.TextCore.Text;


public class ResetFlagCheck : StateMachineBehaviour
{
    PlayerManager player;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player == null)
        {
            player = animator.GetComponent<PlayerManager>();
        }
        if (player == null)
        {
            return;
        }

        if (player.playerEffectManager.activeQuickSlotItemFX != null)
        {
            Destroy(player.playerEffectManager.activeQuickSlotItemFX);
        }

        player.playerLocomitionManager.canRun = true;
        player.playerEquipmentManager.ShowWeapons();


        if (player.playerCombatManager.isUsingItem)
        {
            player.playerCombatManager.isUsingItem = false;

            if (!player.isPerformingAction)
                player.playerLocomitionManager.canRoll = true;
        }
        //character.characterAnimatorManager.applyRootMotion = false;
        //character.characterLocomotionManager.canMove = true;
        //character.characterLocomotionManager.canRotate = true;
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
