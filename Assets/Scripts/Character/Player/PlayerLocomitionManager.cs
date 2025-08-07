using UnityEngine;

public class PlayerLocomitionManager : CharacterLocomotionManager
{
    private PlayerManager player;

    [HideInInspector] public float verticalMovement;
    [HideInInspector] public float horizontalMovement;
    [HideInInspector] public float moveAmount;
    public bool canSprint = true; // Default to true


    [Header("Movement Settings")]
    [SerializeField] private float walkingSpeed = 2;
    [SerializeField] private float runningSpeed = 5;
    [SerializeField] private float sprintingSpeed = 10;
    [SerializeField] private float rotationSpeed = 15;
    [SerializeField] private float sprintingStaminaCost = 2f;
    private Vector3 moveDirection;
    private Vector3 targetRotationDirection;

    [Header("Jump")]
    [SerializeField] private float jumpStaminaCost = 20;
    [SerializeField] private float jumpHeight = 4;
    [SerializeField] private float jumpForwardSpeed = 4;
    [SerializeField] private float freeFallSpeed = 2;
    [SerializeField] private Vector3 jumpDirection;

    [Header("Dodge")]
    [SerializeField] private float dodgeStaminaCost = 25;
    private Vector3 rollDirection;
    private float aimingSpeed = 2f;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
        sprintingStaminaCost = 2f;
    }

    protected override void Update()
    {
        base.Update();
    }

    public void HandleAllMovement()
    {
        HandleGroundedMovement();
        HandleRotation();
        HandleJumpingMovement();
        HandleFreeFallMovement();
    }

    private void GetMovementValues()
    {
        verticalMovement = PlayerInputManager.instance.verticalInput;
        horizontalMovement = PlayerInputManager.instance.horizontalInput;
        moveAmount = PlayerInputManager.instance.moveAmount;
    }

    private void HandleGroundedMovement()
    {
        if (player.playerLocomitionManager.canMove || player.playerLocomitionManager.canRotate)
            GetMovementValues();


        if (!player.playerLocomitionManager.canMove)
            return;

        Vector3 cameraForward;
        Vector3 cameraDir;

        if (player.isAiming)
        {
            cameraForward = PlayerCamera.instance.cameraObject.transform.forward;
            cameraDir = PlayerCamera.instance.cameraObject.transform.right;
        }
        else
        {
            cameraForward = PlayerCamera.instance.transform.forward;
            cameraDir = PlayerCamera.instance.transform.right;
        }
        moveDirection = cameraForward * verticalMovement;
        moveDirection = moveDirection + cameraDir * horizontalMovement;
        moveDirection.y = 0;
        moveDirection.Normalize();

        if (player.isAiming)
        {
            player.characterController.Move(moveDirection * aimingSpeed * Time.deltaTime);

        }
        else if (player.isSprinting)
        {
            player.characterController.Move(moveDirection * sprintingSpeed * Time.deltaTime);
        }
        else
        {
            if (PlayerInputManager.instance.moveAmount > 0.5f)
            {
                //RUN
                player.characterController.Move(runningSpeed * Time.deltaTime * moveDirection);
            }
            else if (PlayerInputManager.instance.moveAmount <= 0.5f)
            {
                //WALK
                player.characterController.Move(Time.deltaTime * walkingSpeed * moveDirection);
            }
        }
    }

    private void HandleRotation()
    {
        if (player.isDead)
            return;

        if (!player.playerLocomitionManager.canRotate)
            return;

        if (player.isAiming)
        {
            HandleAimingRotation();
        }
        else
        {
            HandleStardardRotation();
        }
    }

    private void HandleAimingRotation()
    {
        Vector3 targetDirection;
        targetDirection = PlayerCamera.instance.cameraObject.transform.forward;
        targetDirection.y = 0;
        targetDirection.Normalize();
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion finalRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        transform.rotation = finalRotation;
    }

    private void HandleStardardRotation()
    {


        if (player.isLockedOn)
        {
            if (player.isSprinting || player.characterLocomotionManager.isRolling)
            {
                Vector3 targetDirection = Vector3.zero;
                targetDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
                targetDirection += PlayerCamera.instance.cameraObject.transform.right * horizontalMovement;
                targetDirection.Normalize();
                targetDirection.y = 0;

                if (targetDirection == Vector3.zero)
                    targetDirection = transform.forward;

                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                Quaternion finalRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                transform.rotation = finalRotation;
            }
            else
            {
                if (player.playerCombatManager.currentTarget == null)
                    return;

                Vector3 targetDirection;
                targetDirection = player.playerCombatManager.currentTarget.transform.position - transform.position;
                targetDirection.y = 0;
                targetDirection.Normalize();

                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                Quaternion finalRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                transform.rotation = finalRotation;
            }
        }
        else
        {
            targetRotationDirection = Vector3.zero;
            targetRotationDirection = PlayerCamera.instance.cameraObject.transform.forward * verticalMovement;
            targetRotationDirection += PlayerCamera.instance.transform.right * horizontalMovement;
            targetRotationDirection.y = 0;
            targetRotationDirection.Normalize();

            if (targetRotationDirection == Vector3.zero)
            {
                targetRotationDirection = transform.forward;
            }

            Quaternion newRotation = Quaternion.LookRotation(targetRotationDirection);
            Quaternion targetRotation = Quaternion.Slerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
            transform.rotation = targetRotation;
        }
    }


    public void AttemptToPerformDodge() // moving = roll / stationary = backstep
    {

        //if (player.playerLocomitionManager.canRoll)
        //{
        //    return;
        //}
        if (player.isPerformingAction)
        {
            return;
        }

        if (player.playerCombatManager.isUsingItem)
        {
            return;
        }

        if (player.characterStatManager.currentStamina - dodgeStaminaCost <= 0)
        {
            return;
        }

        if (!player.playerLocomitionManager.isGrounded)
            return;


        if (PlayerInputManager.instance.moveAmount > 0)
        {
            rollDirection = PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.verticalInput;
            rollDirection += PlayerCamera.instance.cameraObject.transform.right * PlayerInputManager.instance.horizontalInput;

            rollDirection.y = 0;
            rollDirection.Normalize();

            Quaternion playerRotation = Quaternion.LookRotation(rollDirection);
            player.transform.rotation = playerRotation;

            player.playerAnimatorManager.PlayerTargetActionAnimation("Roll_Forward_01", true);
            player.playerLocomitionManager.isRolling = true;
        }
        else
        {
            player.playerAnimatorManager.PlayerTargetActionAnimation("Back_Step_01", true);
        }

        player.characterStatManager.DecreaseStamina(dodgeStaminaCost);
        player.DestoryAllCurrentActionFX();
    }

    public void HandleSprinting()
    {
        if (player.isPerformingAction)
        {
            player.isSprinting = false;
        }

        if (player.characterStatManager.currentStamina <= 0)
        {
            player.isSprinting = false;
            return;
        }

        if (PlayerInputManager.instance.moveAmount >= 0.5 && canSprint)
        {
            player.isSprinting = true;
        }
        else
        {
            player.isSprinting = false;
        }

        if (player.isSprinting)
        {
            //TODO: smaller number with smaller intervals so it wont be so choppy
            if (!player.isJumping)
                player.playerStatManager.DecreaseStamina(sprintingStaminaCost * Time.deltaTime);
        }
    }

    private void HandleJumpingMovement()
    {
        if (player.isJumping)
        {
            player.characterController.Move(jumpForwardSpeed * Time.deltaTime * jumpDirection);
        }
    }

    public void AttemptToPerformJump()
    {
        // if we performing general action, we dont want to allow jump (change with combat?)
        if (player.isPerformingAction)
        {
            return;
        }

        if (player.characterStatManager.currentStamina - jumpStaminaCost <= 0)
        {
            return;
        }

        if (player.isJumping)
            return;

        if (!player.playerLocomitionManager.isGrounded)
            return;

        player.characterStatManager.DecreaseStamina(jumpStaminaCost);

        player.playerAnimatorManager.PlayerTargetActionAnimation("Main_Jump_Start", false);

        player.isJumping = true;


        jumpDirection = PlayerCamera.instance.cameraObject.transform.forward * PlayerInputManager.instance.verticalInput;
        jumpDirection += PlayerCamera.instance.cameraObject.transform.right * PlayerInputManager.instance.horizontalInput;

        jumpDirection.y = 0;

        if (jumpDirection != Vector3.zero)
        {
            if (player.isSprinting)
            {
                jumpDirection *= 1;
            }
            else if (PlayerInputManager.instance.moveAmount > 0.5)
            {
                jumpDirection *= 0.5f;
            }
            else if (PlayerInputManager.instance.moveAmount <= 0.5)
            {
                jumpDirection *= 0.25f;
            }
        }
    }


    private void HandleFreeFallMovement()
    {
        if (!player.playerLocomitionManager.isGrounded)
        {
            Vector3 freeFallDirection;

            freeFallDirection = PlayerCamera.instance.transform.forward * PlayerInputManager.instance.verticalInput;
            freeFallDirection += PlayerCamera.instance.transform.right * PlayerInputManager.instance.horizontalInput;

            player.characterController.Move(freeFallDirection * freeFallSpeed * Time.deltaTime);
        }
    }

    public void ApplyJumpingVelocity()
    {
        yVelocty.y = Mathf.Sqrt(jumpHeight * -2 * gravityForce);
    }
}