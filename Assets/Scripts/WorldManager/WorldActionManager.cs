using System.Linq;
using UnityEngine;

public class WorldActionManager : MonoBehaviour
{
    public static WorldActionManager instance;

    [Header("Weapon Item Actions")]
    public WeaponItemAction[] weaponItemActions;

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

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        for (int i = 0; i < weaponItemActions.Length; i++)
        {
            weaponItemActions[i].actionID = i;
        }
    }

    public WeaponItemAction GetWeaponItemActionByID(int ID)
    {
        return weaponItemActions.FirstOrDefault(action => action.actionID == ID);
    }

    public bool IsActionUnlocked(PlayerManager player, WeaponItemAction targetAction)
    {
        if (player == null || player.playerInventoryManager.currentRightWeapon == null)
            return false;

        WeaponItem weapon = player.playerInventoryManager.currentRightWeapon;

        if (weapon.weaponSkillTree == null || targetAction == null)
            return false;

        foreach (var node in weapon.weaponSkillTree.nodes)
        {
            if (node.isUnlocked && node.specialAbilityActionID >= 0)
            {
                WeaponItemAction assignedAction = GetWeaponItemActionByID(node.specialAbilityActionID);
                if (assignedAction == targetAction)
                    return true;
            }
        }

        return false;
    }


}
