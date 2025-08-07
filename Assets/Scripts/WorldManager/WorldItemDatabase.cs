using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldItemDatabase : MonoBehaviour
{
    public static WorldItemDatabase instance;

    public WeaponItem unarmedWeapon;

    public GameObject pickUpItemPrefab;

    [Header("Item ID Prefix Keys")]
    [SerializeField] int weaponItemKey = 1000000;
    [SerializeField] int headEquipmentKey = 2000000;
    [SerializeField] int bodyEquipmentKey = 3000000;
    [SerializeField] int legEquipmentKey = 4000000;
    [SerializeField] int handEquipmentKey = 5000000;
    [SerializeField] int quickSlotItemKey = 6000000;

    [Header("Weapons")]
    [SerializeField] List<WeaponItem> weapons = new();

    [Header("Head Equipment")]
    [SerializeField] List<HeadEquipmentItem> headEquipment = new();

    [Header("Body Equipment")]
    [SerializeField] List<BodyEquipmentItem> bodyEquipment = new();

    [Header("Leg Equipment")]
    [SerializeField] List<LegEquipmentItem> legEquipment = new();

    [Header("Hands Equipment")]
    [SerializeField] List<HandEquipmentItem> handEquipment = new();

    [Header("QuickSlot Items")]
    [SerializeField] List<QuickSlotItem> quickSlotItems = new();

    [Header("Projectiles")] 
    [SerializeField] List<RangedProjectileItem> projectileItems = new();

    [Header("Items")]
    [SerializeField] List<Item> items = new();



    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        foreach (var weapon in weapons)
        {
            items.Add(weapon);
        }

        foreach (var head in headEquipment)
        {
            items.Add(head);
        }

        foreach (var chest in bodyEquipment)
        {
            items.Add(chest);
        }

        foreach (var leg in legEquipment)
        {
            items.Add(leg);
        }

        foreach (var hand in handEquipment)
        {
            items.Add(hand);
        }
        foreach (var quickSlot in quickSlotItems)
        {
            items.Add(quickSlot);
        }

        foreach (var projectile in projectileItems)
        {
            items.Add(projectile);
        }


        for (int i = 0; i < weapons.Count; ++i)
        {
            items[i].itemID = weaponItemKey + i;
        }

        for (int i = 0; i < headEquipment.Count; ++i)
        {
            headEquipment[i].itemID = headEquipmentKey + i;
        }

        for (int i = 0; i < bodyEquipment.Count; ++i)
        {
            bodyEquipment[i].itemID = bodyEquipmentKey + i;
        }

        for (int i = 0; i < legEquipment.Count; ++i)
        {
            legEquipment[i].itemID = legEquipmentKey + i;
        }
        for (int i = 0; i < handEquipment.Count; ++i)
        {
            handEquipment[i].itemID = handEquipmentKey + i;
        }
        for (int i = 0; i < quickSlotItems.Count; ++i)
        {
            quickSlotItems[i].itemID = quickSlotItemKey + i;
        }
        for (int i = 0; i < projectileItems.Count; ++i)
        {
            projectileItems[i].itemID = weaponItemKey + i + 1000; // offset for projectiles
        }



        DontDestroyOnLoad(gameObject);
    }

    public Item GetItemByID(int ID)
    {
        return items.FirstOrDefault(items => items.itemID == ID);
    }

    public WeaponItem GetWeaponByID(int ID)
    {
        return weapons.FirstOrDefault(weapon => weapon.itemID == ID);
    }

    public HeadEquipmentItem GetHeadEquipmentByID(int ID)
    {
        return headEquipment.FirstOrDefault(head => head.itemID == ID);
    }

    public BodyEquipmentItem GetBodyEquipmentByID(int ID)
    {
        return bodyEquipment.FirstOrDefault(chest => chest.itemID == ID);
    }

    public LegEquipmentItem GetLegEquipmentByID(int ID)
    {
        return legEquipment.FirstOrDefault(leg => leg.itemID == ID);
    }

    public HandEquipmentItem GetHandEquipmentByID(int ID)
    {
        return handEquipment.FirstOrDefault(hand => hand.itemID == ID);
    }

    public QuickSlotItem GetQuickSlotItemByID(int ID)
    {
        return quickSlotItems.FirstOrDefault(quickSlot => quickSlot.itemID == ID);
    }

    public WeaponItem GetWeaponFromSerializedData(SerializableWeapon sWeapon)
    {
        WeaponItem weapon = null;


        if (GetWeaponByID(sWeapon.itemID))
            weapon = Instantiate(GetWeaponByID(sWeapon.itemID));

        if (weapon == null)
            return Instantiate(unarmedWeapon);

        return weapon;
    }

    public FlaskItem GetFlaskFromSerializedData(SerializableFlask flask)
    {
        FlaskItem flaskItem = null;

        if (GetQuickSlotItemByID(flask.itemID))
            flaskItem = Instantiate(GetQuickSlotItemByID(flask.itemID)) as FlaskItem;

        return flaskItem;

    }

    public EquipmentItem GetEquipmentByID(int ID)
    {
        return (ID) switch
        {
            int n when (n >= headEquipmentKey && n < bodyEquipmentKey) => GetHeadEquipmentByID(ID),
            int n when (n >= bodyEquipmentKey && n < legEquipmentKey) => GetBodyEquipmentByID(ID),
            int n when (n >= legEquipmentKey && n < handEquipmentKey) => GetLegEquipmentByID(ID),
            int n when (n >= handEquipmentKey && n < quickSlotItemKey) => GetHandEquipmentByID(ID),
            _ => null,
        };
    }

    public RangedProjectileItem GetProjectileByID(int ID)
    {
        return projectileItems.FirstOrDefault(projectile => projectile.itemID == ID);
    }




}

