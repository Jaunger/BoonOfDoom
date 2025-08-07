using UnityEngine;

public class QuickSlotItem : Item 
{
    [Header("Item Model")]
    [SerializeField] protected GameObject itemModel;

    [Header("Animation")]
    [SerializeField] protected string useItemAnimationName;

    [Header("Consumable")]
    public bool isConsumable = true;
    public int itemAmount = 1;

    public virtual void AttemptToUseItem(PlayerManager player)
    {
        if (!CanItemBeUsed(player))
            return;

        player.playerAnimatorManager.PlayerTargetActionAnimation(useItemAnimationName, true);
    }

    public virtual void SuccessfullyUseItem(PlayerManager player)
    {

    }

    public virtual bool CanItemBeUsed(PlayerManager player)
    {
        return true;
    }
}
