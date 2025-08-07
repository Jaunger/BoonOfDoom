using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryManager : CharacterInventoryManager
{
    PlayerManager player;

    [Header("Weapons")]
    public WeaponItem currentRightWeapon;
   // public WeaponItem currentLeftWeapon;

    [Header("Quick Slots")]
    public WeaponItem[] weaponsInRightHandSlots = new WeaponItem[3];
    public int rightHandWeaponIndex = 0;
    //public WeaponItem[] weaponsInLeftHandSlots = new WeaponItem[3];
    //public int leftHandWeaponIndex = 0;
    public QuickSlotItem currentQuickSlotItem;
    public bool hasFlask = false; // TODO: set up saving items

    [Header("Armor")]
    public HeadEquipmentItem headEquipment;
    public BodyEquipmentItem bodyEquipment;
    public LegEquipmentItem legEquipment;
    public HandEquipmentItem handEquipment;

    [Header("Projectiles")]
    public RangedProjectileItem mainProjectile;
    public RangedProjectileItem secondaryProjectile;

    [Header("Progression")]
    public List<int> unlockedWeaponIDs = new();

    [Header("Flasks")]
    public int _remainingHealthFlasks = 3;
    public int remainingHealthFlasks
    {
        get { return _remainingHealthFlasks; }
        set
        {
            _remainingHealthFlasks = value;
            currentQuickSlotItem.itemAmount = _remainingHealthFlasks;
            PlayerUIManager.instance.hudManager.SetQuickSlotIcon(currentQuickSlotItem);
        }
    }

    [Header("Inventory")]
    public List<Item> itemsInInventory; // TODO: set up saving items

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
    }

    public void AddItemToInventory(Item item)
    {
        itemsInInventory.Add(item);

        if (item is WeaponItem weapon)
        {
            Debug.Log("picked weapon: " + weapon.itemName); 
            for (int i = 0; i < weaponsInRightHandSlots.Length; i++)
            {
                WeaponItem slotWeapon = weaponsInRightHandSlots[i];
                if (slotWeapon != null &&
                    slotWeapon.weaponClass == WeaponClass.Unarmed)
                {
                    weaponsInRightHandSlots[i] = weapon;      
                    unlockedWeaponIDs.Add(weapon.itemID);
                    break;
                }
            }
        }

        if (!hasFlask && item is FlaskItem)
        {
            player.currentQuickSlotItemID = item.itemID;
            hasFlask = true;
        }
    }

    public void RemoveItemFromInventory(Item item)
    {
       // itemsInInventory.Remove(item);
    }


    public bool IsWeaponUnlocked(int weaponID)
    {
        return unlockedWeaponIDs.Contains(weaponID);
    }

    public List<WeaponItem> GetUnlockedWeapons()
    {
        List<WeaponItem> weapons = new List<WeaponItem>();
        foreach (int id in unlockedWeaponIDs)
        {
            WeaponItem weapon = WorldItemDatabase.instance.GetWeaponByID(id);
            if (weapon != null)
                weapons.Add(weapon);
        }
        return weapons;
    }



}


