using System;
using UnityEngine;

[CreateAssetMenu(fileName = "FlaskItem", menuName = "Items/Consumeables/Flask")]
public class FlaskItem : QuickSlotItem
{
    [Header("Restoration Value")]
    public int flaskHealAmount = 50;
    //[SerializeField] int maxFlaskCharges = 3;

    [Header("Empty Flask Item")]
    public GameObject emptyFlaskItemPrefab;
    public string emptyFlaskAnimation;
    public Sprite emptyFlaskIcon;


    public override bool CanItemBeUsed(PlayerManager player)
    {
        if (!player.playerCombatManager.isUsingItem && player.isPerformingAction)
            return false;

        if (player.isAttacking)
            return false;
        return true;
    }
    public override void AttemptToUseItem(PlayerManager player)
    {
        if (!CanItemBeUsed(player))
            return;

        if (player.playerInventoryManager.remainingHealthFlasks <= 0)
        {
            if (player.playerCombatManager.isUsingItem)
                return;

            player.playerCombatManager.isUsingItem = true;

            player.playerAnimatorManager.PlayerTargetActionAnimation(emptyFlaskAnimation, false, false, true, true, false);
            Destroy(player.playerEffectManager.activeQuickSlotItemFX);
            GameObject emptyFlask = Instantiate(emptyFlaskItemPrefab, player.playerEquipmentManager.rightHandSlot.transform);
            player.playerEffectManager.activeQuickSlotItemFX = emptyFlask;
            return;
        }

        if (player.playerCombatManager.isUsingItem)
        {
            player.playerCombatManager.SetIsChugging(true);
            return;
        }

        player.playerCombatManager.isUsingItem = true;

        player.playerEffectManager.activeQuickSlotItemFX = Instantiate(itemModel, player.playerEquipmentManager.rightHandSlot.transform);

 
        player.playerAnimatorManager.PlayerTargetActionAnimation(useItemAnimationName, false, false, true, true, false);
        player.playerEquipmentManager.HideWeapons();
    }

    public override void SuccessfullyUseItem(PlayerManager player)
    {
        base.SuccessfullyUseItem(player);

        player.playerStatManager.IncreateHealth(flaskHealAmount);
        player.playerInventoryManager.remainingHealthFlasks--;
        player.playerInventoryManager.currentQuickSlotItem.itemAmount = player.playerInventoryManager.remainingHealthFlasks;

        if (player.playerInventoryManager.remainingHealthFlasks <= 0)
        {
            Destroy(player.playerEffectManager.activeQuickSlotItemFX);
            GameObject emptyFlask = Instantiate(emptyFlaskItemPrefab, player.playerEquipmentManager.rightHandSlot.transform);
            player.playerEffectManager.activeQuickSlotItemFX = emptyFlask;
        }

        PlayerUIManager.instance.hudManager.SetQuickSlotIcon(player.playerInventoryManager.currentQuickSlotItem);

        PlayHealingFX(player);
        // TODO: add sfx and vfx
    }

    private void PlayHealingFX(PlayerManager player)
    {
        Instantiate(WorldCharacterEffectsManager.instance.healingVFX, player.transform);
        player.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.HealingSFX);
    }
}
