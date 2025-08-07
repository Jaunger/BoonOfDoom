using UnityEngine;

[CreateAssetMenu(menuName = "CharacterActions/Weapon Actions/Test Action")]
public class WeaponItemAction : ScriptableObject
{
    public int actionID;
    
    public virtual void AttemptToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerfomingAction)
    {
        playerPerformingAction.currentWeaponID = weaponPerfomingAction.itemID;


    }

    
}
