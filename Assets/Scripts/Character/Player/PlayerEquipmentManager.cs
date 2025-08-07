using UnityEngine;

public class PlayerEquipmentManager : CharacterEquipmentManager
{
    PlayerManager player;

    public WeaponModelInstantiationSlot rightHandSlot;
    public WeaponModelInstantiationSlot leftHandSlot;

    [SerializeField] public WeaponManager rightWeaponManager;
    [SerializeField] WeaponManager leftWeaponManager;

    public GameObject rightHandWeaponModel;
    public GameObject leftHandWeaponModel;

    [Header("Equipment Model")]
    public GameObject fullHelmetObject;
    [HideInInspector] public GameObject[] headFullHelmets;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();

        InitializeWeaponSlots();

        //List<GameObject> headFullHelmetsList = new List<GameObject>();

        //foreach (Transform child in fullHelmetObject.transform)
        //{
        //    headFullHelmetsList.Add(child.gameObject);
        //}

        //headFullHelmets = headFullHelmetsList.ToArray();
    }

    protected override void Start()
    {
        base.Start();

        LoadWeaponsOnBothHands();
    }

    public void LoadHeadEquipment(HeadEquipmentItem headEquipment)
    {
        UnloadHeadEquipmentModels();

        if (headEquipment == null)
        {
            player.playerInventoryManager.headEquipment = null;
            return;
        }

        player.playerInventoryManager.headEquipment = headEquipment;

        foreach (var model in headEquipment.equipmentModels)
        {
            model.LoadModel(player);
        }

        player.playerStatManager.CalculateTotalArmorAbsorption();
    }

    private void UnloadHeadEquipmentModels()
    {
        foreach (var model in headFullHelmets)
        {
            model.SetActive(false);
        }

        // RE ENABLE head object
        // RE ENABLE hair object
    }

    public void LoadChestEquipment(BodyEquipmentItem chestEquipment)
    {
        player.playerStatManager.CalculateTotalArmorAbsorption();
    }

    public void LoadLegEquipment(LegEquipmentItem legEquipment)
    {
        player.playerStatManager.CalculateTotalArmorAbsorption();

    }

    public void LoadHandEquipment(HandEquipmentItem handEquipment)
    {
        player.playerStatManager.CalculateTotalArmorAbsorption();

    }

    private void InitializeWeaponSlots()
    {
        WeaponModelInstantiationSlot[] weaponSlots = GetComponentsInChildren<WeaponModelInstantiationSlot>();

        foreach (var weaponSlot in weaponSlots)
        {
            if (weaponSlot.wepSlot == WeaponModelSlot.RightHand)
            {
                rightHandSlot = weaponSlot;
            }
            else if (weaponSlot.wepSlot == WeaponModelSlot.LeftHand)
            {
                leftHandSlot = weaponSlot;
            }
        }

    }

    public void LoadWeaponsOnBothHands()
    {
        LoadRightWeapon();
        LoadLeftWeapon();
    }

    //  Right weapon

    public void SwitchRightWeapon()
    {
        /* ------------------------------------------------------------
           0.  Play swap animation and wipe previous weapon effects
           ------------------------------------------------------------ */
        player.playerAnimatorManager.PlayerTargetActionAnimation(
            "Swap_Right_Weapon_01", false, false, true, true);

        player.playerEffectManager.RemoveAllStaticEffectsFromWeapon();
        player.playerEffectManager.RemoveAllRuntimeEffects();

        /* ------------------------------------------------------------
           1.  Advance index and wrap around
           ------------------------------------------------------------ */
        int slotCount = player.playerInventoryManager.weaponsInRightHandSlots.Length;
        player.playerInventoryManager.rightHandWeaponIndex++;

        if (player.playerInventoryManager.rightHandWeaponIndex >= slotCount)
            player.playerInventoryManager.rightHandWeaponIndex = 0;

        /* ------------------------------------------------------------
           2.  Resolve the next selectable weapon (skip extra Unarmed)
           ------------------------------------------------------------ */
        WeaponItem selectedWeapon =
            player.playerInventoryManager.weaponsInRightHandSlots[
                player.playerInventoryManager.rightHandWeaponIndex];

        /* If we landed on Unarmed but there is another real weapon,
           advance until we hit a non-unarmed slot (max one full loop). */
        if (selectedWeapon.itemID == WorldItemDatabase.instance.unarmedWeapon.itemID)
        {
            bool foundRealWeapon = false;

            for (int i = 0; i < slotCount; i++)
            {
                player.playerInventoryManager.rightHandWeaponIndex++;
                if (player.playerInventoryManager.rightHandWeaponIndex >= slotCount)
                    player.playerInventoryManager.rightHandWeaponIndex = 0;

                selectedWeapon =
                    player.playerInventoryManager.weaponsInRightHandSlots[
                        player.playerInventoryManager.rightHandWeaponIndex];

                if (selectedWeapon.itemID != WorldItemDatabase.instance.unarmedWeapon.itemID)
                {
                    foundRealWeapon = true;
                    break;
                }
            }

            /* If we never found a real weapon, stay unarmed */
            if (!foundRealWeapon)
                selectedWeapon = WorldItemDatabase.instance.unarmedWeapon;
        }

        /* ------------------------------------------------------------
           3.  Finalize selection
           ------------------------------------------------------------ */
        player.playerInventoryManager.currentRightWeapon = selectedWeapon;
        player.currentRightWeaponID = selectedWeapon.itemID;

        /* ------------------------------------------------------------
           4.  Apply “on-equip” static effects
           ------------------------------------------------------------ */
        player.playerEffectManager.ApplyStaticEffectsFromSkillTree(
            selectedWeapon.runtimeSkillTree);

        /* ------------------------------------------------------------
           5.  Add Bulwark Momentum runtime effect if conditions met
           ------------------------------------------------------------ */

        player.playerEffectManager.ApplyRuntimeEffectsFromSkillTree(
            selectedWeapon.runtimeSkillTree);

        //if (selectedWeapon.weaponType == WeaponModelType.GreatAxe &&
        //    selectedWeapon.runtimeSkillTree != null &&
        //    selectedWeapon.runtimeSkillTree.HasUnlockedNode("Bulwark Momentum"))
        //{
        //    RuntimeCharacterEffect bulwarkAsset =
        //        WorldCharacterEffectsManager.instance.runtimeEffects
        //            .Find(e => e.name == "BulwarkMomentumEffect");

        //    if (bulwarkAsset != null)
        //        player.playerEffectManager.AddRuntimeEffect(bulwarkAsset);
        //}

        //if (selectedWeapon.weaponType == WeaponModelType.GreatAxe &&
        //    selectedWeapon.runtimeSkillTree != null &&
        //    selectedWeapon.runtimeSkillTree.HasUnlockedNode("Seismic Wave"))
        //{
        //    RuntimeCharacterEffect waveAsset =
        //        WorldCharacterEffectsManager.instance.runtimeEffects
        //            .Find(e => e.name == "Seismic Wave");

        //    if (waveAsset != null)
        //        player.playerEffectManager.AddRuntimeEffect(waveAsset);
        //}

    }

    public void LoadRightWeapon()
    {

        if (player.playerInventoryManager.currentRightWeapon != null)
        {
            //  Remove old weapon
            rightHandSlot.UnloadWeapon();

            rightHandWeaponModel = Instantiate(player.playerInventoryManager.currentRightWeapon.weaponModel);
            rightHandSlot.LoadWeapon(rightHandWeaponModel);
            rightWeaponManager = rightHandWeaponModel.GetComponent<WeaponManager>();
            rightWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentRightWeapon);
            player.playerAnimatorManager.UpdateAnimatorController(player.playerInventoryManager.currentRightWeapon);
            player.currentWeaponID = player.playerInventoryManager.currentRightWeapon.itemID;
        }
    }


    // Left weapon - fill maybe later

    public void LoadLeftWeapon()
    {

        //if (player.playerInventoryManager.currentLeftWeapon != null)
        //{

        //    leftHandWeaponModel = Instantiate(player.playerInventoryManager.currentLeftWeapon.weaponModel);
        //    leftHandSlot.LoadWeapon(leftHandWeaponModel);
        //    leftWeaponManager = leftHandWeaponModel.GetComponent<WeaponManager>();
        //    leftWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentLeftWeapon);

        //}
    }


    // Damage colliders

    public void OpenDamageCollider()
    {
        if (player.isUsingRightHand)
        {
            rightWeaponManager.meleeDamageCollider.EnableDamageCollider();
            player.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(player.playerInventoryManager.currentRightWeapon.whooshSounds));
        }
        else if (player.isUsingLeftHand)
        {
            leftWeaponManager.meleeDamageCollider.EnableDamageCollider();
          //  player.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(player.playerInventoryManager.currentLeftWeapon.whooshSounds));

        }


    }

    public void CloseDamageCollider()
    {
        if (player.isUsingRightHand)
        {
            rightWeaponManager.meleeDamageCollider.DisableDamageCollider();
        }
        else if (player.isUsingLeftHand)
        {
            leftWeaponManager.meleeDamageCollider.DisableDamageCollider();
        }


    }

    public void HideWeapons()
    {
        if (rightHandWeaponModel != null)
        {
            rightHandWeaponModel.SetActive(false);
        }
        if (leftHandWeaponModel != null)
        {
            leftHandWeaponModel.SetActive(false);
        }
    }

    public void ShowWeapons()
    {
        if (rightHandWeaponModel != null)
        {
            rightHandWeaponModel.SetActive(true);
        }
        if (leftHandWeaponModel != null)
        {
            leftHandWeaponModel.SetActive(true);
        }
    }

    public void LoadQuickSlotEquipment(QuickSlotItem quickSlotItem)
    {
        if (quickSlotItem == null)
        {
            player.currentQuickSlotItemID = -1;

            player.playerInventoryManager.currentQuickSlotItem = null;

            return;
        }
        player.playerInventoryManager.currentQuickSlotItem = quickSlotItem;
        player.currentQuickSlotItemID = quickSlotItem.itemID;

    }

}
