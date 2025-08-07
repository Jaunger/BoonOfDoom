using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;
    public PlayerManager player;
    PlayerControls playerControls;

    [Header("MOVEMENT INPUT")]
    [SerializeField] Vector2 movementInput;
    public float verticalInput;
    public float horizontalInput;
    public float moveAmount;

    [Header("CAMERA INPUT")]
    [SerializeField] Vector2 cameraInput;
    public float cameraVerticalInput;
    public float cameraHorizontalInput;

    [Header("LOCK ON INPUT")]
    [SerializeField] bool lockOnInput;
    [SerializeField] bool lockOnLeftInput;
    [SerializeField] bool lockOnRightInput;
    private Coroutine lockOnCoroutine;

    [Header("ACTION INPUT")]
    [SerializeField] bool dodgeInput = false;
    [SerializeField] bool sprintInput = false;
    [SerializeField] public bool jumpInput = false;
    [SerializeField] bool rbInput = false;
    [SerializeField] bool rtInput = false;
    public bool holdRTInput = false;
    [SerializeField] bool holdRBInput = false;
    [SerializeField] bool lbInput = false;
    [SerializeField] bool useQSInput = false;
    [SerializeField] bool specialAbilityInput = false;
    [SerializeField] bool specialAbilityAltInput = false;

    [Header("Two Hand Inputs")]
    [SerializeField] bool twoHandInput = false;



    [Header("Interaction INPUT")]
    [SerializeField] bool interactInput = false;

    [Header("QUEUED INPUT")]
    [SerializeField] private bool inputQueIsActive = false;
    [SerializeField] float default_queInputTimer = 0.35f;
    [SerializeField] float queInputTimer = 0;
    [SerializeField] bool quedRBInput = false;
    [SerializeField] bool quedRTInput = false;

    [Header("D-Pad Input")]
    [SerializeField] bool switchLeftWepInput = false;
    [SerializeField] bool switchRightWepInput = false;

    [Header("UI Inputs")]
    [SerializeField] bool openMenuInput = false;
    [SerializeField] bool closeMenuInput = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }

    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        SceneManager.activeSceneChanged += OnSceneChange;

        instance.enabled = false;
        playerControls?.Disable();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (enabled)
        {
            if (focus)
            {
                playerControls.Enable();
            }
            else
            {
                playerControls.Disable();
            }
        }
    }

    private void OnSceneChange(Scene oldScene, Scene newScene)
    {
        if (newScene.buildIndex == WorldSaveGameManager.instance.GetWorldSceneIndex())
        {
            instance.enabled = true;
            playerControls?.Enable();

        }
        else
        {
            instance.enabled = false;
            playerControls?.Disable();
        }
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerCamera.Movement.performed += i => cameraInput = i.ReadValue<Vector2>();

            playerControls.PlayerActions.Dodge.performed += i => dodgeInput = true;
            playerControls.PlayerActions.Jump.performed += i => jumpInput = true;
            playerControls.PlayerActions.SwitchLeftWeapon.performed += i => switchLeftWepInput = true;
            playerControls.PlayerActions.SwitchRightWeapon.performed += i => switchRightWepInput = true;
            playerControls.PlayerActions.Interact.performed += i => interactInput = true;
            playerControls.PlayerActions.UseQuickSlot.performed += i => useQSInput = true;
            playerControls.PlayerActions.SpecialAbility.performed += i => specialAbilityInput = true;
            playerControls.PlayerActions.SpecialAbilityAlt.performed += i => specialAbilityAltInput = true;

            playerControls.PlayerActions.Sprint.performed += i => sprintInput = true;
            playerControls.PlayerActions.Sprint.canceled += i => sprintInput = false;

            playerControls.PlayerActions.RB.performed += i => rbInput = true;
            playerControls.PlayerActions.HoldRB.performed += i => holdRBInput = true;
            playerControls.PlayerActions.HoldRB.canceled += i => holdRBInput = false;

            playerControls.PlayerActions.RT.performed += i => rtInput = true;
            playerControls.PlayerActions.RT.performed += i => holdRTInput = true;
            playerControls.PlayerActions.RT.canceled += i => holdRTInput = false;

            playerControls.PlayerActions.TwoHandWeapon.performed += i => twoHandInput = true;
            playerControls.PlayerActions.TwoHandWeapon.canceled += i => twoHandInput = false;



            playerControls.PlayerActions.LockOn.performed += i => lockOnInput = true;
            playerControls.PlayerActions.SeekLeftLockOn.performed += i => lockOnLeftInput = true;
            playerControls.PlayerActions.SeekRightLockOn.performed += i => lockOnRightInput = true;


            playerControls.PlayerActions.LB.performed += i => lbInput = true;
            playerControls.PlayerActions.LB.canceled += i => player.isBlocking = false;
            playerControls.PlayerActions.LB.canceled += i => player.isAiming = false;


            // QUED INPUT
            playerControls.PlayerActions.QueRB.performed += i => QueInput(ref quedRBInput);
            playerControls.PlayerActions.QueRT.performed += i => QueInput(ref quedRTInput);

            // UI INPUT 
            playerControls.UI.Cancel.performed += i => closeMenuInput = true;
            playerControls.PlayerActions.OpenCharacterMenu.performed += i => openMenuInput = true;

        }

        playerControls.Enable();
    }

    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= OnSceneChange;
    }

    private void Update()
    {
        HandleuseQSInput();
        HandleLockOnInput();
        HandleLockOnSwitch();
        HandleMovementInput();
        HandleCameraInput();
        HandleDodgeInput();
        HandleSprintInput();
        HandleJumpInput();
        HandleRBInput();
        HandleRTInput();
        HandleHeldRTInput();
        HandleRightSwitchWeaponInput();
        HandleLeftSwitchWeaponInput();
        HandleQuedInputs();
        HandleInteractionInput();
        HandleLBInput();
        HandleCloseUIInput();
        HandleOpenCharacterMenuInput();
        HandleSpecialAbilityInput();
        HandleSpecialAbilityAltInput();
        HandleHoldRBInput();
    }

    private void HandleuseQSInput()
    {
        if (useQSInput)
        {
            useQSInput = false;

            if (PlayerUIManager.instance.menuIsOpen)
                return;

            if (player.playerInventoryManager.currentQuickSlotItem != null)
            {
                player.playerInventoryManager.currentQuickSlotItem.AttemptToUseItem(player);
            }
        }
    }

    private void HandleLockOnInput()
    {
        if (player.isLockedOn)
        {
            if (player.playerCombatManager.currentTarget == null)
                return;

            if (player.playerCombatManager.currentTarget.isDead)
            {
                player.isLockedOn = false;


                //  Assures coroutine never runs multiple times
                if (lockOnCoroutine != null)
                    StopCoroutine(lockOnCoroutine);

                lockOnCoroutine = StartCoroutine(PlayerCamera.instance.WaitThenFindNewTarget());
            }
        }

        if (lockOnInput && player.isLockedOn)
        {
            lockOnInput = false;
            PlayerCamera.instance.ClearLockOnTargets();
            player.isLockedOn = false;

            // Unlock - disable lock on 
            return;
        }

        if (lockOnInput && !player.isLockedOn)
        {
            lockOnInput = false;

            // ranged weapons == no lock on

            PlayerCamera.instance.HandleLocatingLockOnTargets();
            // Enable lock on
            if (PlayerCamera.instance.nearestLockOnTarget != null)
            {
                player.characterCombatManager.SetTarget(PlayerCamera.instance.nearestLockOnTarget);
                player.isLockedOn = true;
            }

        }

    }

    private void HandleLockOnSwitch()
    {
        if (lockOnLeftInput)
        {
            lockOnLeftInput = false;
            if (player.isLockedOn)
            {
                PlayerCamera.instance.HandleLocatingLockOnTargets();

                if (PlayerCamera.instance.leftLockOnTarget != null)
                {
                    player.characterCombatManager.SetTarget(PlayerCamera.instance.leftLockOnTarget);
                }
            }
        }
        else if (lockOnRightInput)
        {
            lockOnRightInput = false;

            if (player.isLockedOn)
            {
                PlayerCamera.instance.HandleLocatingLockOnTargets();

                if (PlayerCamera.instance.rightLockOnTarget != null)
                {
                    player.characterCombatManager.SetTarget(PlayerCamera.instance.rightLockOnTarget);
                }
            }
        }


    }

    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        moveAmount = Mathf.Clamp01(Math.Abs(verticalInput) + Mathf.Abs(horizontalInput));

        //CLAMP values either 0,0.5 or 1
        if (moveAmount <= 0.5 && moveAmount > 0)
        {
            moveAmount = 0.5f;
        }
        else if (moveAmount > 0.5 && moveAmount <= 1)
        {
            moveAmount = 1;
        }

        if (player == null)
            return;

        if (moveAmount != 0)
            player.isMoving = true;
        else
            player.isMoving = false;

        if (!player.playerLocomitionManager.canRun)
        {
            if(moveAmount > 0.5f)
                moveAmount = 0.5f; 

            if (verticalInput > 0.5f)
                verticalInput = 0.5f;
            
            if (horizontalInput > 0.5f)
                horizontalInput = 0.5f;
        }

        if (player.isLockedOn || !player.isSprinting)
        {
            player.playerAnimatorManager.UpdateAnimatorMovementParameters(horizontalInput, verticalInput, player.isSprinting);
            return;
        }

        if (player.isAiming)
        {
            player.playerAnimatorManager.UpdateAnimatorMovementParameters(horizontalInput, verticalInput, player.isSprinting);
            return;
        }

        player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.isSprinting);

    }

    private void HandleCameraInput()
    {
        cameraVerticalInput = cameraInput.y;
        cameraHorizontalInput = cameraInput.x;

    }

    private void HandleDodgeInput()
    {
        if (dodgeInput)
        {
            dodgeInput = false;

            if (PlayerUIManager.instance.menuIsOpen)
                return;

            player.playerLocomitionManager.AttemptToPerformDodge();
        }
    }

    private void HandleSprintInput()
    {
        if (sprintInput)
        {
            player.playerLocomitionManager.HandleSprinting();
        }
        else
        {
            player.isSprinting = false;
        }
    }

    private void HandleJumpInput()
    {
        if (jumpInput)
        {
            jumpInput = false;

            if (PlayerUIManager.instance.menuIsOpen)
                return;

            if (player.isHoldingArrow)
                return;

            //  Attempt to perform jump

            player.playerLocomitionManager.AttemptToPerformJump();
        }
    }

    private void HandleRBInput()
    {
        if (twoHandInput)
            return;

        if (rbInput)
        {
            rbInput = false;

            if (PlayerUIManager.instance.menuIsOpen)
                return;

            player.SetCharacterActionHand(true);

            player.playerCombatManager.PerformWeaponBasedAction(player.playerInventoryManager.currentRightWeapon.oh_RB_Action, player.playerInventoryManager.currentRightWeapon);
        }
    }

    private void HandleHoldRBInput()
    {
        if (holdRBInput)
        {
            player.isHoldingArrow = true;
        }
        else
        {
            player.isHoldingArrow = false;  
        }
    }

    private void HandleLBInput()
    {
        if (twoHandInput)
            return;

        if (lbInput)
        {
            lbInput = false;


            if (PlayerUIManager.instance.menuIsOpen)
                return;

            player.SetCharacterActionHand(false);

            player.playerCombatManager.PerformWeaponBasedAction(player.playerInventoryManager.currentRightWeapon.oh_LB_Action, player.playerInventoryManager.currentRightWeapon);
        }
    }

    private void HandleRTInput()
    {
        if (rtInput)
        {
            rtInput = false;


            if (PlayerUIManager.instance.menuIsOpen)
                return;

            player.SetCharacterActionHand(true);

            player.playerCombatManager.PerformWeaponBasedAction(player.playerInventoryManager.currentRightWeapon.oh_RT_Action, player.playerInventoryManager.currentRightWeapon);
        }
    }

    private void HandleHeldRTInput()
    {
        if (player.isPerformingAction)
        {

            if (player.isUsingRightHand)
            {
                player.isChargingAttack = holdRTInput;
            }
        }
    }

    private void HandleRightSwitchWeaponInput()
    {
        if (switchRightWepInput)
        {
            switchRightWepInput = false;
            if (PlayerUIManager.instance.menuIsOpen)
                return;


            player.playerEquipmentManager.SwitchRightWeapon();
        }
    }

    private void HandleLeftSwitchWeaponInput()
    {
        if (switchLeftWepInput)
        {
            switchLeftWepInput = false;
            //player.playerEquipmentManager.SwitchLeftWeapon();
        }
    }

    private void HandleInteractionInput()
    {
        if (interactInput)
        {
            interactInput = false;

            if (player.isDead)
                return;

            player.playerInteractionManager.Interact();
        }
    }

    private void QueInput(ref bool quedInput)
    {
        quedRBInput = false;
        quedRTInput = false;
        //quedLBInput = false;
        //quedLTInput = false;


        if (PlayerUIManager.instance.menuIsOpen)
            return;

        if (player.isPerformingAction || player.isJumping)
        {
            quedInput = true;

            queInputTimer = default_queInputTimer;
            inputQueIsActive = true;
        }
    }

    private void ProcessAllQuedInputs()
    {
        if (player.isDead)
            return;

        if (quedRBInput)
            rbInput = true;

        if (quedRTInput)
            rtInput = true;


    }

    private void HandleQuedInputs()
    {
        if (inputQueIsActive)
        {
            if (queInputTimer > 0)
            {
                queInputTimer -= Time.deltaTime;
                ProcessAllQuedInputs();
            }
            else
            {
                quedRBInput = false;
                quedRTInput = false;
                //quedLBInput = false;
                //quedLTInput = false;
                inputQueIsActive = false;
                queInputTimer = 0;
            }
        }
    }

    private void HandleOpenCharacterMenuInput()
    {
        if (openMenuInput)
        {
            openMenuInput = false;
            PlayerUIManager.instance.popUpManager.CloseAllPopUpWindows();
            PlayerUIManager.instance.CloseAllMenuWindows();
            PlayerUIManager.instance.menuManager.OpenCharacterMenu();

        }
    }

    private void HandleCloseUIInput()
    {
        if (closeMenuInput)
        {
            closeMenuInput = false;

            if (PlayerUIManager.instance.menuIsOpen)
            {
                PlayerUIManager.instance.CloseAllMenuWindows();
            }
        }


    }

    private void HandleSpecialAbilityInput()
    {
        if (specialAbilityInput)
        {
            specialAbilityInput = false;
            rbInput = false;
            if (player == null)
                return;

            WeaponItem currentWeapon = player.playerInventoryManager.currentRightWeapon;

            if (currentWeapon != null && currentWeapon.specialAbilityAction != null)
            {
                player.playerCombatManager.PerformWeaponBasedAction(currentWeapon.specialAbilityAction, currentWeapon);
            }
        }
    }

    private void HandleSpecialAbilityAltInput()
    {
        if (!specialAbilityAltInput) return;
        specialAbilityAltInput = false;

        var weapon = player.playerInventoryManager.currentRightWeapon;
        if (weapon == null || weapon.specialAbilityActionAlt == null) return;

        player.playerCombatManager.PerformWeaponBasedAction(
            weapon.specialAbilityActionAlt, weapon);
    }


}
