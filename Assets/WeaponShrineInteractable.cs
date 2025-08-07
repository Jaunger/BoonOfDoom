using System.Collections.Generic;
using UnityEngine;

public class WeaponShrineInteractable : Interactable
{
    [Header("Weapon to grant")]
    [SerializeField] private WeaponItem weaponToGrant;

    [Header("Finish Toturial GameObject")]
    [SerializeField] private GameObject finishTutorialGameObject;

    [Header("Brazier To Disable")]  
    [SerializeField] private BrazierInteractable brazierToDisable;

    public override void Interact(PlayerManager player)
    {
        base.Interact(player);

        if (weaponToGrant == null)
        {
            return;
        }

        var inv = player.playerInventoryManager;
        var db = WorldItemDatabase.instance;
        var unarmed = db.unarmedWeapon;                     

        for (int i = 0; i < 3; i++)
        {
            inv.weaponsInRightHandSlots[i] = unarmed;
        }

        inv.unlockedWeaponIDs = new List<int>();
        inv.rightHandWeaponIndex = 0;


        inv.itemsInInventory.RemoveAll(item => item is WeaponItem);

        WeaponItem newWeapon = Instantiate(weaponToGrant);

        inv.weaponsInRightHandSlots[0] = newWeapon;
        inv.currentRightWeapon = newWeapon;
        inv.AddItemToInventory(newWeapon);

        if (!inv.unlockedWeaponIDs.Contains(newWeapon.itemID))
            inv.unlockedWeaponIDs.Add(newWeapon.itemID);

        player.playerEquipmentManager.LoadRightWeapon();
        
        finishTutorialGameObject.SetActive(true);
        HideOtherShrines();
        brazierToDisable.isActivated = false;
        
    }

    private void HideOtherShrines()
    {
        foreach (var shrine in FindObjectsByType<WeaponShrineInteractable>(0))
        {
            if (shrine == this) continue;

            shrine.interactCollider.enabled = false;

            var root = shrine.gameObject;
            root.transform.localScale = Vector3.zero;
            root.SetActive(false);
        }
    }
}
