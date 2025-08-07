using UnityEngine;

public class AIFrankSoundFXManager : CharacterSoundFXManager
{
    [Header("Frank Sound FX")]
    public AudioClip[] wooshes;
    public AudioClip[] kickSounds;
    public AudioClip[] slamSounds;
    public virtual void PlayAxeSlamSoundFX()
    {
        if (slamSounds.Length > 0)
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(slamSounds));
    }

}
