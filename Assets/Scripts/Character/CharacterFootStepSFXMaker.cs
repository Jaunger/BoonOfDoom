using UnityEngine;

public class CharacterFootStepSFXMaker : MonoBehaviour
{
    CharacterManager character;

    AudioSource audioSource;
    GameObject steppedOnObject;

    private bool hasTouchedGround = false;
    public bool hasPlayedFootStepSFX = false;
    [SerializeField] float distanceToGround = 0.05f;

    private void Awake()
    {
        character = GetComponentInParent<CharacterManager>();
        audioSource = GetComponentInParent<AudioSource>();
    }

    private void FixedUpdate()
    {
        CheckForFootSteps();
    }

    private void CheckForFootSteps()
    {
        if (character == null) return;

        if (!character.isMoving) return;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, character.transform.TransformDirection(Vector3.down), out hit, distanceToGround, WorldUtilityManager.instance.GetEnvLayers()))
        {
            hasTouchedGround = true;

            if (!hasPlayedFootStepSFX)
                steppedOnObject = hit.transform.gameObject;
        }
        else
        {

            hasTouchedGround = false;
            hasPlayedFootStepSFX = false;
            steppedOnObject = null;
        }

        if (hasTouchedGround && !hasPlayedFootStepSFX)
        {

            hasPlayedFootStepSFX = true;
            PlayFootStepSoundFX();
        }
    }

    private void PlayFootStepSoundFX()
    {
        // play a different sfx depending on the layer of the ground or a tag ( snow, wood, stone etc)
        if (WorldSoundFXManager.instance.ChooseRandomFootStepSoundBasedOnGround(steppedOnObject, character) != null)
            audioSource.PlayOneShot(WorldSoundFXManager.instance.ChooseRandomFootStepSoundBasedOnGround(steppedOnObject, character));

        if (character is PlayerManager)
        {
            WorldSoundFXManager.instance.AlertNearbyCharacterToSound(transform.position, 2);
        }
    }
}
