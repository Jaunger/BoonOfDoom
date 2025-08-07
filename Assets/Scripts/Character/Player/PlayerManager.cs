using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : CharacterManager
{
    [Header("DEBUG MENU")]
    [SerializeField] private bool respawnCharacter = false; //  TODO: delete 
    [SerializeField] private bool switchRightWeapon = false;

    private Coroutine reviveCoroutine;
    private GameObject prevDeathSpot;

    [HideInInspector] public PlayerLocomitionManager playerLocomitionManager;
    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
    [HideInInspector] public PlayerInventoryManager playerInventoryManager;
    [HideInInspector] public PlayerEquipmentManager playerEquipmentManager;
    [HideInInspector] public PlayerCombatManager playerCombatManager;
    [HideInInspector] public PlayerStatManager playerStatManager; 
    [HideInInspector] public PlayerInteractionManager playerInteractionManager;
    [HideInInspector] public PlayerEffectManager playerEffectManager;

    [Header("Player Information")]
    public string characterName = "character";
    public Transform globalFlameSlashSpawnPoint;
    public override bool isDead
    {
        get => base.isDead;
        set
        {
            base.isDead = value;
            if (value)
            {
                SpawnDeathPool();

                if (PlayerUIManager.instance.hudManager.currentBossHealthBar != null)
                    PlayerUIManager.instance.hudManager.currentBossHealthBar.RemoveHPBar(1f);

                WorldAIManager.instance.DisableAllBossFights();


            }
        }
    }

    [Header("Equipment")]
    [SerializeField] private int _currentRightWeaponID;
    [SerializeField] private int _currentLeftWeaponID;
    [SerializeField] private int _currentWeaponID;
    [SerializeField] private int _currentQuickSlotItemID;
    public int currentQuickSlotItemID
    {
        get { return _currentQuickSlotItemID; }
        set
        {
            _currentQuickSlotItemID = value;
            QuickSlotItem newItem = null;

            if (WorldItemDatabase.instance.GetQuickSlotItemByID(value))
                newItem = Instantiate(WorldItemDatabase.instance.GetQuickSlotItemByID(value));

            if (newItem != null)
            {
                playerInventoryManager.currentQuickSlotItem = newItem;
                if (playerInventoryManager.currentQuickSlotItem is FlaskItem)
                    playerInventoryManager.currentQuickSlotItem.itemAmount = playerInventoryManager.remainingHealthFlasks;

                PlayerUIManager.instance.hudManager.SetQuickSlotIcon(playerInventoryManager.currentQuickSlotItem);
            }
        }
    }
    public bool isTwoHandingRightWeapon = false;
    public bool isUsingRightHand;
    public bool isUsingLeftHand;

    public int currentWeaponID
    {
        get { return _currentWeaponID; }

        set
        {
            _currentWeaponID = value;
            WeaponItem newWeapon = Instantiate(WorldItemDatabase.instance.GetWeaponByID(_currentWeaponID));
            playerCombatManager.currentWeaponBeingUsed = newWeapon;
        }
    }

    public int currentRightWeaponID
    {
        get { return _currentRightWeaponID; }

        set
        {
            _currentRightWeaponID = value;
            //WeaponItem newWeapon = Instantiate(WorldItemDatabase.instance.GetWeaponByID(_currentRightWeaponID));
            //playerInventoryManager.currentRightWeapon = newWeapon;
            playerEquipmentManager.LoadRightWeapon();
            playerCombatManager.currentWeaponBeingUsed = playerInventoryManager.currentRightWeapon;
            PlayerUIManager.instance.hudManager.SetRightWeapSlotIcon(value);
        }
    }

    public int currentLeftWeaponID
    {
        get { return _currentLeftWeaponID; }
        set
        {
            //Debug.Log("switch left");
            _currentLeftWeaponID = value;
            //WeaponItem newWeapon = Instantiate(WorldItemDatabase.instance.GetWeaponByID(_currentLeftWeaponID));
            //playerInventoryManager.currentLeftWeapon = newWeapon;
            playerEquipmentManager.LoadLeftWeapon();
            PlayerUIManager.instance.hudManager.SetLeftWeapSlotIcon(value);
        }
    }

    public override bool isBlocking
    {
        get => base.isBlocking;

        set
        {
            base.isBlocking = value;
            if (playerCombatManager.currentWeaponBeingUsed == null)
                return;

            playerStatManager.blockingPhysicalAbsorption = playerCombatManager.currentWeaponBeingUsed.physicalBlockAbsorption;
            playerStatManager.blockingMagicAbsorption = playerCombatManager.currentWeaponBeingUsed.magicBlockAbsorption;
            playerStatManager.blockingFireAbsorption = playerCombatManager.currentWeaponBeingUsed.fireBlockAbsorption;
            playerStatManager.blockingLightningAbsorption = playerCombatManager.currentWeaponBeingUsed.lightningBlockAbsorption;
            playerStatManager.blockingHolyAbsorption = playerCombatManager.currentWeaponBeingUsed.holyBlockAbsorption;
            playerStatManager.blockingStability = playerCombatManager.currentWeaponBeingUsed.stability;
        }
    }

    private bool _isFullyChargedAttack;
    public bool isFullyChargedAttack
    {
        get => _isFullyChargedAttack;
        set
        {
            _isFullyChargedAttack = value;
            animator.SetBool("fullyCharged", value);   // ensure parameter exists
        }
    }

    [Header("Projectile")]
    [SerializeField] private int _mainProjectileID;
    [SerializeField] private bool _isHoldingArrow = false;
    [SerializeField] private bool _isAiming = false;
    public bool hasArrowNotched = false;
    public int mainProjectileID
    {
        get { return _mainProjectileID; }
        set
        {
            RangedProjectileItem newProjectile = null;
            _mainProjectileID = value;

            if (WorldItemDatabase.instance.GetProjectileByID(value))
                newProjectile = Instantiate(WorldItemDatabase.instance.GetProjectileByID(value));

            if (newProjectile != null)
                playerInventoryManager.mainProjectile = newProjectile;

        }
    }
    public bool isHoldingArrow
    {
        get { return _isHoldingArrow; }
        set
        {
            _isHoldingArrow = value;
            animator.SetBool("isHoldingArrow", _isHoldingArrow);
        }
    }
    public bool isAiming
    {
        get { return _isAiming; }
        set
        {
            _isAiming = value;

            if (!isAiming)
            {
                PlayerCamera.instance.cameraObject.transform.localEulerAngles = new Vector3(0, 0, 0);
                PlayerCamera.instance.cameraObject.fieldOfView = 60;
                PlayerCamera.instance.cameraObject.nearClipPlane = 0.3f;
                PlayerCamera.instance.cameraPivotTransform.localPosition = new Vector3(0, PlayerCamera.instance.cameraPivotYOffeset, 0);
                PlayerUIManager.instance.hudManager.crossHair.SetActive(false);
            }
            else
            {

                PlayerCamera.instance.cameraObject.transform.eulerAngles = new Vector3(0, 0, 0);
                PlayerCamera.instance.cameraPivotTransform.localEulerAngles = new Vector3(0, 0, 0);
                PlayerCamera.instance.cameraObject.fieldOfView = 40;
                PlayerCamera.instance.cameraObject.nearClipPlane = 1.3f;
                PlayerCamera.instance.cameraPivotTransform.localPosition = new Vector3(0, 0, 0);

                PlayerUIManager.instance.hudManager.crossHair.SetActive(true);

            }
            //animator.SetBool("isAiming", _isAiming);
        }
    }

    private void SpawnDeathPool()
    {
        GameObject deathSpotFX;
        if (transform.position.y >= 0)
        {
            deathSpotFX = Instantiate(WorldCharacterEffectsManager.instance.deadSpotVFX, transform.position, Quaternion.identity);
        }
        else
        {
            deathSpotFX = Instantiate(WorldCharacterEffectsManager.instance.deadSpotVFX, new Vector3(transform.position.x, 0, transform.position.z), Quaternion.identity);
        }

        PickUpRuneInteractable pickUpRuneInteractable = deathSpotFX.GetComponent<PickUpRuneInteractable>();
        pickUpRuneInteractable.soulCount = playerStatManager.souls;
        playerStatManager.AddSouls(-playerStatManager.souls);
        prevDeathSpot = deathSpotFX;
        WorldSaveGameManager.instance.currentCharacterData.hasDeathSpot = true;
        WorldSaveGameManager.instance.currentCharacterData.deathSpotSoulsCount = pickUpRuneInteractable.soulCount;
        WorldSaveGameManager.instance.currentCharacterData.deathSpotXPos = transform.position.x;
        WorldSaveGameManager.instance.currentCharacterData.deathSpotYPos = transform.position.y < 0 ? 0.1f : transform.position.y;
        WorldSaveGameManager.instance.currentCharacterData.deathSpotZPos = transform.position.z;
    }

    public void SetCharacterActionHand(bool rightHandedAction)
    {
        if (rightHandedAction)
        {
            isUsingLeftHand = false;
            isUsingRightHand = true;
        }
        else
        {
            isUsingLeftHand = true;
            isUsingRightHand = false;
        }
    }

    protected override void Awake()
    {
        base.Awake();

        playerLocomitionManager = GetComponent<PlayerLocomitionManager>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        characterStatManager = GetComponent<PlayerStatManager>();
        playerInventoryManager = GetComponent<PlayerInventoryManager>();
        playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
        playerCombatManager = GetComponent<PlayerCombatManager>();
        playerStatManager = GetComponent<PlayerStatManager>();
        playerInteractionManager = GetComponent<PlayerInteractionManager>();
        playerEffectManager = GetComponent<PlayerEffectManager>();
    }

    protected override void Update()
    {
        base.Update();

        playerLocomitionManager.HandleAllMovement();
        characterStatManager.RegenerateStamina();
        DebugMenu();
    }

    protected override void Start()
    {
        base.Start();

        PlayerCamera.instance.player = this;
        PlayerInputManager.instance.player = this;
        WorldSaveGameManager.instance.player = this;

        //playerStatManager.maxStamina = playerStatManager.CalculateStaminaBasedOnLevel(playerStatManager.endurance);
        //playerStatManager.SetStamina(playerStatManager.CalculateStaminaBasedOnLevel(playerStatManager.endurance));
        //PlayerUIManager.instance.hudManager.SetMaxStaminaValue(playerStatManager.maxStamina);
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        PlayerCamera.instance.HandleAllCameraActions();
    }

    public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
        PlayerUIManager.instance.popUpManager.SendYouDiedPopUp();

        if (WorldSaveGameManager.instance.currentCharacterData.hasDeathSpot)
        {
            Destroy(prevDeathSpot);
            prevDeathSpot = null;
        }

        WaitThenRevive();

        return base.ProcessDeathEvent(manuallySelectDeathAnimation);
    }

    public void WaitThenRevive()
    {
        //TODO: reset npc and enemies also (should be by area)
        WorldAIManager.instance.ResetAllCharacters();

        if (reviveCoroutine != null)
            StopCoroutine(reviveCoroutine);

        reviveCoroutine = StartCoroutine(ReviveCoroutine(5));
    }

    private IEnumerator ReviveCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        PlayerUIManager.instance.loadingScreenManager.ActivateLoadingScreen();

        ReviveCharacter();
        
        WorldAIManager.instance.ResetAllCharacters();

        for (int i = 0; WorldObjectManager.instance.braziers.Count > i; i++)
        {
            if (WorldObjectManager.instance.braziers[i].brazierID == WorldSaveGameManager.instance.currentCharacterData.lastBrazierRestedAt)
            {
                WorldObjectManager.instance.braziers[i].TeleportToSiteOfGrace();
                break;
            }
        }


    }

    public override void ReviveCharacter()
    {
        base.ReviveCharacter();
        isDead = false;
        characterStatManager.currentHealth = characterStatManager.maxHealth;
        characterStatManager.currentStamina = characterStatManager.maxStamina;
        //  Restore mana

        // play rebirth effects

        playerAnimatorManager.PlayerTargetActionAnimation("Empty", false);
    }

    public void SaveGame(ref CharacterSaveData currentCharacterData, bool isNewGame = false)
    {
        currentCharacterData.sceneIndex = SceneManager.GetActiveScene().buildIndex;

        currentCharacterData.characterName = characterName;
        currentCharacterData.xPos = transform.position.x;
        currentCharacterData.yPos = transform.position.y;
        currentCharacterData.zPos = transform.position.z;

        currentCharacterData.currentSouls = playerStatManager.souls;
        currentCharacterData.vitality = characterStatManager.vitality;
        currentCharacterData.endurance = characterStatManager.endurance;
        currentCharacterData.currentFocus = playerStatManager.currentFocus;

        if (isNewGame)
        {
            currentCharacterData.currentStamina = characterStatManager.CalculateStaminaBasedOnLevel(characterStatManager.endurance);
            currentCharacterData.currentHealth = characterStatManager.CalculateHealthBasedOnLevel(characterStatManager.vitality);
        }
        else
        {
            currentCharacterData.currentStamina = characterStatManager.currentStamina;
            currentCharacterData.currentHealth = characterStatManager.currentHealth;
        }

        currentCharacterData.currentFlasksRemaining = playerInventoryManager.remainingHealthFlasks;

        //Equipment

        currentCharacterData.rightWeaponIndex = playerInventoryManager.rightHandWeaponIndex;

        List<int> unlockedWeaponIDs = new List<int>();

        for (int i = 0; i < playerInventoryManager.weaponsInRightHandSlots.Length; i++)
        {
            if (playerInventoryManager.weaponsInRightHandSlots[i] == null)
                continue;

            if (!unlockedWeaponIDs.Contains(playerInventoryManager.weaponsInRightHandSlots[i].itemID))
            {
                unlockedWeaponIDs.Add(playerInventoryManager.weaponsInRightHandSlots[i].itemID);
                Debug.Log("Adding weapon to unlocked list: " + playerInventoryManager.weaponsInRightHandSlots[i].itemID);
            }
        }

       
        currentCharacterData.rightWeapon01 = WorldSaveGameManager.instance.GetSerializableWeapon(playerInventoryManager.weaponsInRightHandSlots[0]);
        currentCharacterData.rightWeapon02 = WorldSaveGameManager.instance.GetSerializableWeapon(playerInventoryManager.weaponsInRightHandSlots[1]);
        currentCharacterData.rightWeapon03 = WorldSaveGameManager.instance.GetSerializableWeapon(playerInventoryManager.weaponsInRightHandSlots[2]);

        //currentCharacterData.leftWeaponIndex = playerInventoryManager.leftHandWeaponIndex;
        //currentCharacterData.leftWeapon01 = WorldSaveGameManager.instance.GetSerializableWeapon(playerInventoryManager.weaponsInLeftHandSlots[0]);
        //currentCharacterData.leftWeapon02 = WorldSaveGameManager.instance.GetSerializableWeapon(playerInventoryManager.weaponsInLeftHandSlots[1]);
        //currentCharacterData.leftWeapon03 = WorldSaveGameManager.instance.GetSerializableWeapon(playerInventoryManager.weaponsInLeftHandSlots[2]);

        currentCharacterData.quickSlotIndex = 0;
        if (playerInventoryManager.currentQuickSlotItem != null)
            currentCharacterData.quickSlot01 = WorldSaveGameManager.instance.GetSerializableFlask(playerInventoryManager.currentQuickSlotItem as FlaskItem);

        // clear list before save
        currentCharacterData.weaponsInventory = new List<SerializableWeapon>();
        currentCharacterData.flaskInInventory = new List<SerializableFlask>();
        //currentCharacterData.headEquipmentInInventory = new List<int>();
        //currentCharacterData.bodyEquipmentInInventory = new List<int>();
        //currentCharacterData.legEquipmentInInventory = new List<int>();
        //currentCharacterData.handEquipmentInInventory = new List<int>();


        for (int i = 0; i < playerInventoryManager.itemsInInventory.Count; i++)
        {
            if (playerInventoryManager.itemsInInventory[i] == null)
                continue;

            WeaponItem weaponInInventory = playerInventoryManager.itemsInInventory[i] as WeaponItem;
            HeadEquipmentItem headEquipmentInInventory = playerInventoryManager.itemsInInventory[i] as HeadEquipmentItem;
            BodyEquipmentItem bodyEquipmentInInventory = playerInventoryManager.itemsInInventory[i] as BodyEquipmentItem;
            LegEquipmentItem legEquipmentInInventory = playerInventoryManager.itemsInInventory[i] as LegEquipmentItem;
            HandEquipmentItem handEquipmentInInventory = playerInventoryManager.itemsInInventory[i] as HandEquipmentItem;

            FlaskItem flaskItemInInventory = playerInventoryManager.itemsInInventory[i] as FlaskItem;
            //QuickSlotItem quickSlotItemInInventory = playerInventoryManager.itemsInInventory[i] as QuickSlotItem;

            if (weaponInInventory != null)
                currentCharacterData.weaponsInventory.Add(WorldSaveGameManager.instance.GetSerializableWeapon(weaponInInventory));

            //if (headEquipmentInInventory != null)
            //    currentCharacterData.headEquipmentInInventory.Add(headEquipmentInInventory.itemID);

            //if (bodyEquipmentInInventory != null)
            //    currentCharacterData.bodyEquipmentInInventory.Add(bodyEquipmentInInventory.itemID);

            //if (legEquipmentInInventory != null)
            //    currentCharacterData.legEquipmentInInventory.Add(legEquipmentInInventory.itemID);

            //if (handEquipmentInInventory != null)
            //    currentCharacterData.handEquipmentInInventory.Add(handEquipmentInInventory.itemID);

            if (flaskItemInInventory != null)
                currentCharacterData.flaskInInventory.Add(WorldSaveGameManager.instance.GetSerializableFlask(flaskItemInInventory));



        }

        currentCharacterData.unlockedWeaponIDs = new List<int>(unlockedWeaponIDs);
    }

    public void LoadGame(ref CharacterSaveData currentCharacterData)
    {
        characterName = currentCharacterData.characterName;
        Vector3 myPos = new Vector3(currentCharacterData.xPos, currentCharacterData.yPos, currentCharacterData.zPos);
        transform.position = myPos;

        characterStatManager.vitality = currentCharacterData.vitality;
        characterStatManager.endurance = currentCharacterData.endurance;

        characterStatManager.maxStamina = characterStatManager.CalculateStaminaBasedOnLevel(characterStatManager.endurance);
        characterStatManager.maxHealth = characterStatManager.CalculateHealthBasedOnLevel(characterStatManager.vitality);
        playerStatManager.AddSouls(currentCharacterData.currentSouls);
        PlayerUIManager.instance.hudManager.SetMaxStaminaValue(characterStatManager.maxStamina);
        PlayerUIManager.instance.hudManager.SetMaxHealthValue(characterStatManager.maxHealth);
        PlayerUIManager.instance.hudManager.SetMaxFocusValue(0, playerStatManager.maxFocus);

        playerStatManager.currentFocus = currentCharacterData.currentFocus;
        ((PlayerStatManager)characterStatManager).SetStamina(currentCharacterData.currentStamina);
        ((PlayerStatManager)characterStatManager).SetHealth(currentCharacterData.currentHealth);



        //  Load equipment TODO: armor and stuff

        playerInventoryManager.rightHandWeaponIndex = currentCharacterData.rightWeaponIndex;
        playerInventoryManager.weaponsInRightHandSlots[0] = currentCharacterData.rightWeapon01.GetWeapon();
        playerInventoryManager.weaponsInRightHandSlots[1] = currentCharacterData.rightWeapon02.GetWeapon();
        playerInventoryManager.weaponsInRightHandSlots[2] = currentCharacterData.rightWeapon03.GetWeapon();

        if (currentCharacterData.quickSlot01 != null)
        {
            playerInventoryManager.currentQuickSlotItem = currentCharacterData.quickSlot01.GetFlask();
            playerEquipmentManager.LoadQuickSlotEquipment(playerInventoryManager.currentQuickSlotItem);
        }

        if (currentCharacterData.rightWeaponIndex >= 0)
        {
            playerInventoryManager.currentRightWeapon = playerInventoryManager.weaponsInRightHandSlots[currentCharacterData.rightWeaponIndex];
            currentRightWeaponID = playerInventoryManager.weaponsInRightHandSlots[currentCharacterData.rightWeaponIndex].itemID;
        }
        else
        {
            currentRightWeaponID = WorldItemDatabase.instance.unarmedWeapon.itemID;
        }

        //TODO: add more stuff to load
        playerInventoryManager.itemsInInventory.Clear();
        for (int i = 0; i < currentCharacterData.weaponsInventory.Count; i++)
        {
            WeaponItem weaponInInventory = currentCharacterData.weaponsInventory[i].GetWeapon();
            playerInventoryManager.AddItemToInventory(weaponInInventory);
        }

        //for (int i = 0; i < currentCharacterData.headEquipmentInInventory.Count; i++)
        //{
        //    EquipmentItem equipmentInInventory = WorldItemDatabase.instance.GetHeadEquipmentByID(currentCharacterData.headEquipmentInInventory[i]);
        //    playerInventoryManager.AddItemToInventory(equipmentInInventory);
        //}

        //for (int i = 0; i < currentCharacterData.bodyEquipmentInInventory.Count; i++)
        //{
        //    EquipmentItem equipmentInInventory = WorldItemDatabase.instance.GetBodyEquipmentByID(currentCharacterData.bodyEquipmentInInventory[i]);
        //    playerInventoryManager.AddItemToInventory(equipmentInInventory);
        //}

        //for (int i = 0; i < currentCharacterData.legEquipmentInInventory.Count; i++)
        //{
        //    EquipmentItem equipmentInInventory = WorldItemDatabase.instance.GetLegEquipmentByID(currentCharacterData.legEquipmentInInventory[i]);
        //    playerInventoryManager.AddItemToInventory(equipmentInInventory);
        //}

        //for (int i = 0; i < currentCharacterData.handEquipmentInInventory.Count; i++)
        //{
        //    EquipmentItem equipmentInInventory = WorldItemDatabase.instance.GetHandEquipmentByID(currentCharacterData.handEquipmentInInventory[i]);
        //    playerInventoryManager.AddItemToInventory(equipmentInInventory);
        //}

        for (int i = 0; i < currentCharacterData.flaskInInventory.Count; i++)
        {
            FlaskItem flaskItemInInventory = currentCharacterData.flaskInInventory[i].GetFlask();
            playerInventoryManager.AddItemToInventory(flaskItemInInventory);
        }

        playerInventoryManager.unlockedWeaponIDs = new List<int>(currentCharacterData.unlockedWeaponIDs);
    }

    public override void CheckHP(float value, float mvalue)
    {
        characterStatManager.OnHealthChange();

        base.CheckHP(0, 0);
    }

    private void DebugMenu()
    {
        if (respawnCharacter)
        {
            ReviveCharacter();
            respawnCharacter = false;
        }

        if (switchRightWeapon)
        {
            switchRightWeapon = false;
            playerEquipmentManager.SwitchRightWeapon();
        }
    }

    public override void DestoryAllCurrentActionFX()
    {
        base.DestoryAllCurrentActionFX();

        if (hasArrowNotched)
        {
            Animator bowAnimtor;

            if (playerEquipmentManager.rightHandWeaponModel != null)
            {
                bowAnimtor = playerEquipmentManager.rightHandWeaponModel.GetComponentInChildren<Animator>();
            }
            else
            {
                Debug.LogError("No right weapon equipped to release arrow.");
                return;
            }

            bowAnimtor.SetBool("isDrawn", false);
            //bowAnimtor.Play("Bow_Fire_01");
            hasArrowNotched = false;
        }

    }


}