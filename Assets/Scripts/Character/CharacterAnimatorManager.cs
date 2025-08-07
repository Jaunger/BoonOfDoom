using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterAnimatorManager : MonoBehaviour
{
    CharacterManager character;

    int vertical;
    int horizontal;

    [Header("Damage Animations")]
    public string lastAnimationPlayed;

    [SerializeField] string hit_Forward_M_01 = "hit_Forward_M_01";
    [SerializeField] string hit_Forward_M_02 = "hit_Forward_M_02";
    [SerializeField] string hit_Backward_M_01 = "hit_Backward_M_01";
    [SerializeField] string hit_Left_M_01 = "hit_Left_M_01";
    [SerializeField] string hit_Right_M_01 = "hit_Right_M_01";

    public List<string> forward_M_Damage = new();
    public List<string> backwards_M_Damage = new();
    public List<string> left_M_Damage = new();
    public List<string> right_M_Damage = new();

    [SerializeField] string hit_Forward_Ping_01 = "hit_Forward_Ping_01";
    [SerializeField] string hit_Forward_Ping_02 = "hit_Forward_Ping_02";
    [SerializeField] string hit_Backward_Ping_01 = "hit_Backward_Ping_01";
    [SerializeField] string hit_Left_Ping_01 = "hit_Left_Ping_01";
    [SerializeField] string hit_Right_Ping_01 = "hit_Right_Ping_01";

    public List<string> forward_Ping_Damage = new();
    public List<string> backwards_Ping_Damage = new();
    public List<string> left_Ping_Damage = new();
    public List<string> right_Ping_Damage = new();


    [Header("Flags")]
    public bool applyRootMotion = false;


    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();

        vertical = Animator.StringToHash("Vertical");
        horizontal = Animator.StringToHash("Horizontal");
    }

    protected virtual void Start()
    {
        forward_M_Damage.Add(hit_Forward_M_01);
        forward_M_Damage.Add(hit_Forward_M_02);

        backwards_M_Damage.Add(hit_Backward_M_01);

        left_M_Damage.Add(hit_Left_M_01);

        right_M_Damage.Add(hit_Right_M_01);

        forward_Ping_Damage.Add(hit_Forward_Ping_01);
        forward_Ping_Damage.Add(hit_Forward_Ping_02);

        backwards_Ping_Damage.Add(hit_Backward_Ping_01);

        left_Ping_Damage.Add(hit_Left_Ping_01);

        right_Ping_Damage.Add(hit_Right_Ping_01);


    }

    public string GetRandomAnimationFromList(List<string> animList)
    {
        List<string> fList = new List<string>();

        foreach (var item in animList)
        {
            fList.Add(item);
        }

        if(fList.Count == 1) 
            return fList[0];

        fList.Remove(lastAnimationPlayed);

        for (int i = fList.Count - 1; i > -1; i--)
        {
            if(fList[i] == null)
            {
                fList.RemoveAt(i);
            }
        }

        int randomValue = Random.Range(0, fList.Count);
        return fList[randomValue];
    }

    public void UpdateAnimatorMovementParameters(float horizontalAmount, float verticalAmount, bool isSprinting)
    {
        float snappedHorizontal;
        float snappedVertical;

        if (horizontalAmount > 0 && horizontalAmount <= 0.5f)
        {
            snappedHorizontal = 0.5f;
        }
        else if (horizontalAmount > 0.5f && horizontalAmount <= 1)
        {
            snappedHorizontal = 1;
        }
        else if (horizontalAmount < 0 && horizontalAmount >= -0.5f)
        {
            snappedHorizontal = -0.5f;
        }
        else if (horizontalAmount < -0.5f && horizontalAmount >= -1)
        {
            snappedHorizontal = -1;
        }
        else
        {
            snappedHorizontal = 0;
        }

        if (verticalAmount > 0 && verticalAmount <= 0.5f)
        {
            snappedVertical = 0.5f;
        }
        else if (verticalAmount > 0 && verticalAmount <= 1)
        {
            snappedVertical = 1;
        }
        else if (verticalAmount < 0 && verticalAmount >= -0.5f)
        {
            snappedVertical = -0.5f;
        }
        else if (verticalAmount < -0.5f && verticalAmount >= -1)
        {
            snappedVertical = -1;
        }
        else
        {
            snappedVertical = 0;
        }

        if (isSprinting)
        {
            snappedVertical = 2;
        }


        character.animator.SetFloat(horizontal, snappedHorizontal, 0.1f, Time.deltaTime);
        character.animator.SetFloat(vertical, snappedVertical, 0.1f, Time.deltaTime);
    }

    public void SetAnimatorMovementParameters(float horizontalMovement, float verticalMovement)
    {
        character.animator.SetFloat(vertical,horizontalMovement, 0.1f, Time.deltaTime);
        character.animator.SetFloat(horizontal, verticalMovement, 0.1f, Time.deltaTime);
    }

    public virtual void PlayerTargetActionAnimation(
        string targetAnimation, 
        bool isPerformingAction, 
        bool applyRootMotion = true, 
        bool canRotate = false, 
        bool canMove = false,
        bool canRun = true,
        bool canRoll = false)
    {

        character.characterAnimatorManager.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(targetAnimation, 0.2f);
        // Can be used to stop character from attemption new actions 
        character.isPerformingAction = isPerformingAction;  
        character.characterLocomotionManager.canRotate = canRotate;
        character.characterLocomotionManager.canMove = canMove;
        character.characterLocomotionManager.canRun = canRun;
        character.characterLocomotionManager.canRoll = canRoll;

    }

    public virtual void PlayerTargetAttackActionAnimation(WeaponItem weapon,
    AttackType attackType,
    string targetAnimation,
    bool isPerformingAction,
    bool applyRootMotion = true,
    bool canRotate = false,
    bool canMove = false,
    bool canRoll = false)
    {
        //  Keep track of last attack performed (for combos)
        //  current attack type (light,heavy,etc)
        //  update animation set to current weapon animation
        //  decide if our attack can be parried
        //  isattacking flag is active (for counter damage)
        character.characterCombatManager.currentAttackType = attackType;
        character.characterCombatManager.lastAttackAnimation = targetAnimation;
        UpdateAnimatorController(weapon);
        this.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(targetAnimation, 0.2f);

        character.isPerformingAction = isPerformingAction;
        character.characterLocomotionManager.canRotate = canRotate;
        character.characterLocomotionManager.canMove = canMove;
        character.characterLocomotionManager.canRoll = canRoll;
    }

    public virtual void UpdateAnimatorController(WeaponItem weapon)
    {
        character.animator.runtimeAnimatorController = weapon.weaponAnimator;
    }
}
