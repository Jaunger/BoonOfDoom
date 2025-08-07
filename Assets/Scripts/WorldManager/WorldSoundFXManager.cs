using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSoundFXManager : MonoBehaviour
{
    public static WorldSoundFXManager instance;

    [Header("Boss Track")]
    [SerializeField] AudioSource bossIntroPlayer;
    [SerializeField] AudioSource bossLoopPlayer;

    [Header("Damage Sounds")]
    public AudioClip[] physicalDamageSFX;

    [Header("Action Sounds")]
    public AudioClip pickUpItemsSFX;
    public AudioClip rollSFX;
    public AudioClip HealingSFX;
    public AudioClip[] releaseArrowSFX;
    public AudioClip[] notchArrowSFX;


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
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public AudioClip ChooseRandomSFXFromArray(AudioClip[] array)
    {
        if (array == null || array.Length == 0)
            return null;

        int index = Random.Range(0, array.Length);

        if (index >= array.Length)
            return null;

        return array[index];
    }

    public void PlayBossTrack(AudioClip introTrack, AudioClip loopTrack)
    {
        if (bossIntroPlayer != null && bossLoopPlayer != null)
        {
            bossIntroPlayer.volume = 1f;
            bossIntroPlayer.clip = introTrack;
            bossIntroPlayer.loop = false;
            bossIntroPlayer.Play();

            bossLoopPlayer.volume = 1f;
            bossLoopPlayer.clip = loopTrack;
            bossLoopPlayer.loop = true;
            bossLoopPlayer.PlayDelayed(bossIntroPlayer.clip.length);
        }
    }

    public AudioClip ChooseRandomFootStepSoundBasedOnGround(GameObject steppedOnObject, CharacterManager character)
    {
        if (steppedOnObject.tag == "Untagegd")
        {
            return ChooseRandomSFXFromArray(character.characterSoundFXManager.footStepsDirt);
        }
        else if (steppedOnObject.tag == "Wood")
        {
            return ChooseRandomSFXFromArray(character.characterSoundFXManager.footStepsWood);
        }
        else if (steppedOnObject.tag == "Stone")
        {
            return ChooseRandomSFXFromArray(character.characterSoundFXManager.footStepsStone);
        }
        else 
        {
            // Todo: Add more conditions for different ground types
            return ChooseRandomSFXFromArray(character.characterSoundFXManager.footStepsDirt);
        }
    }

    public void StopBossMusic()
    {
      StartCoroutine(FadeOutBossMusicThenStop());
    }

    private IEnumerator FadeOutBossMusicThenStop()
    {

        while (bossLoopPlayer.volume > 0)
        {
            bossLoopPlayer.volume -= Time.deltaTime;
            bossIntroPlayer.volume -= Time.deltaTime;

            yield return null;
        }   
        bossIntroPlayer.Stop();
        bossLoopPlayer.Stop();  
    }

    public void AlertNearbyCharacterToSound(Vector3 positionOfSound, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(positionOfSound, radius); //WorldUtilityManager.instance.GetCharacterLayers());

        List<AICharacterManager> alertCharacters = new();

        for (int i = 0; i < hitColliders.Length; i++)
        {
            AICharacterManager aICharacter = hitColliders[i].GetComponent<AICharacterManager>();

            if (aICharacter == null)
                continue;

            if (alertCharacters.Contains(aICharacter))
                continue;

            alertCharacters.Add(aICharacter);
        }

        for (int i = 0; i < alertCharacters.Count; i++)
        {
            alertCharacters[i].aiCharacterCombatManager.AlertCharacterToSound(positionOfSound);
        }
    }
}
