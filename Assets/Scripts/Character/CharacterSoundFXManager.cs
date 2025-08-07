using UnityEngine;

public class CharacterSoundFXManager : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("Damage Grunts")]
    [SerializeField] protected AudioClip[] damageGrunts;
    
    [Header("Attack Grunts")]
    [SerializeField] protected AudioClip[] attackGrunts;

    [Header("Foot Step SFX")]
    public AudioClip[] footSteps;
    public AudioClip[] footStepsDirt;
    public AudioClip[] footStepsWood;
    public AudioClip[] footStepsStone;


    protected virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlaySoundFX(AudioClip soundFX, float volume = 1, bool randomizePitch = true, float pitchRandom = 0.1f)
    {
        if (soundFX == null)
            return;

        audioSource.PlayOneShot(soundFX, volume);

        //  Resets pitch
        audioSource.pitch = 1;

        if (randomizePitch)
        {
            audioSource.pitch += Random.Range(-pitchRandom, pitchRandom);
        }
    }

    public void PlayRollSoundFX()
    {
        audioSource.PlayOneShot(WorldSoundFXManager.instance.rollSFX);
    }

    public virtual void PlayDamageGrunt()
    {
        if(damageGrunts.Length > 0)
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(damageGrunts));    
    }

    public virtual void PlayAttackGrunt()
    {
        if (attackGrunts.Length > 0)
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(attackGrunts));

    }

    public virtual void PlayBlockingSFX()
    {
    }

    public virtual void PlayFootStepSoundFX()
    {
        if (footSteps.Length > 0)
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(footSteps));
    }
}
 