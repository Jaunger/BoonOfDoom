using UnityEngine;

public class PickUpRuneInteractable : Interactable
{
    public int soulCount = 0;

    public override void Interact(PlayerManager player)
    {
        WorldSaveGameManager.instance.currentCharacterData.hasDeathSpot = false;
        player.playerStatManager.AddSouls(soulCount);
        Destroy(gameObject);    
    }
}
