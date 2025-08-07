using UnityEngine;

public class ResetIsChugging : StateMachineBehaviour
{
    PlayerManager player;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
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

        if (player.playerCombatManager.isChugging)
        {
            FlaskItem currentFlask = player.playerInventoryManager.currentQuickSlotItem as FlaskItem;
            if (player.playerInventoryManager.remainingHealthFlasks <= 0)
            {
               player.playerAnimatorManager.PlayerTargetActionAnimation(currentFlask.emptyFlaskAnimation, false, false, true, true, false);
               player.playerEquipmentManager.HideWeapons();

            }
        }

        if (player.playerCombatManager.isChugging)
        {
            FlaskItem currentFlask = player.playerInventoryManager.currentQuickSlotItem as FlaskItem;
            if (player.playerInventoryManager.remainingHealthFlasks <= 0)
            {
                Destroy(player.playerEffectManager.activeQuickSlotItemFX);
                GameObject emptyFlask = Instantiate(currentFlask.emptyFlaskItemPrefab, player.playerEquipmentManager.rightHandSlot.transform);
                player.playerEffectManager.activeQuickSlotItemFX = emptyFlask;
            }
        }

        player.playerCombatManager.SetIsChugging(false);
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
