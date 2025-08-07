using UnityEngine;

public class PlayerSoundFXManager : CharacterSoundFXManager
{
    PlayerManager player;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<PlayerManager>();
    }

    public override void PlayBlockingSFX()
    {
        if (player.playerCombatManager.currentWeaponBeingUsed.blockSounds.Length <= 0)
            return;
        PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(player.playerCombatManager.currentWeaponBeingUsed.blockSounds));
    }

    public override void PlayFootStepSoundFX()
    {
        base.PlayFootStepSoundFX();

        Debug.Log("Footstep sound played");
        WorldSoundFXManager.instance.AlertNearbyCharacterToSound(transform.position, 2); //TODO: use whereever sound is playing
    }
}
