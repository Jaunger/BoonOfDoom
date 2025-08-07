using UnityEngine;

public class ToggleBlockingController : StateMachineBehaviour
{
    PlayerManager player;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player == null)
            player = animator.GetComponent<PlayerManager>();

        if (player == null)
            return;

        if (player.isBlocking)
        {
            //its right weapon becuase we are blocking with the same arm as the weapon and not having a seperate shield on left hand
            player.playerAnimatorManager.UpdateAnimatorController(player.playerInventoryManager.currentRightWeapon);
        }
        else
        {

        }

    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {


        if (player == null)
            player = animator.GetComponent<PlayerManager>();

        if (player == null)
            return;

        if (player.isBlocking)
        {
            player.playerAnimatorManager.UpdateAnimatorController(player.playerInventoryManager.currentRightWeapon);
        }
        else
        {

        }
    }

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
