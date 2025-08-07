using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Blocking Damage")]
public class TakeBlockingDamageEffect : InstantCharacterEffect
{
    [Header("Character Causing Damage")]
    public CharacterManager characterCausingDamage;

    [Header("Damage")]
    public float physicalDamage = 0; // split into 4 types "Standard", "Strike", "Slash", "Pierce"
    public float magicDamage = 0;
    public float fireDamage = 0;
    public float lightningDamage = 0;
    public float holyDamage = 0;

    [Header("Final Damage")]
    private int finalDamageDealth = 0;

    [Header("Poise")]
    public float poiseDamage = 0;
    public bool poiseIsBroken = false;

    [Header("Stamina")]
    public float baseStaminaDamage = 0; //  TODO: add to weapon
    public float fStaminaDamage = 0; //  TODO: add to weapon

    //  TODO: Build up effects

    [Header("Animations")]
    public bool playDamageAnimation = true;
    public bool manuallySelectDamageAnimation = false;
    public string damageAnimation;

    [Header("Sound FX")]
    public bool willPlayDamageSFX = true;

    public AudioClip elementalDamageSoundFX;    //  used on top of regular sfx for weapon

    [Header("Direction Damage Taken From")]
    public float angleHitFrom;                  //  used to determine animation to play

    public Vector3 contactPoint;                //  used to determine where the blood fx instantiate

    public override void ProcessEffect(CharacterManager character)
    {
        if (character.isInvulnerable)
            return;

        base.ProcessEffect(character);

        Debug.Log("Blocking");

        if (character.isDead)
            return;

        //  Check for invulnerability return

        CalculateDamage(character);
        CalculateStaminaDamage(character);
        CheckForGuardBreak(character);
        PlayDirectionalBasedBlockingAnimation(character);
        //  check build ups
        PlayDamageSFX(character);
        PlayBlockVFX(character);

        //  If character is A.I, check for new target if causer is preset
    }

    private void CalculateStaminaDamage(CharacterManager character)
    {
        fStaminaDamage = baseStaminaDamage;

        float staminaDamageAbsorbtion = fStaminaDamage * (character.characterStatManager.blockingStability / 100);
        fStaminaDamage -= staminaDamageAbsorbtion;

        character.characterStatManager.currentStamina -= fStaminaDamage;
    }

    private void CheckForGuardBreak(CharacterManager character)
    {
       if (character.characterStatManager.currentStamina <= 0)
        {
            //  play guard break animation
            character.characterAnimatorManager.PlayerTargetActionAnimation("Guard_Break_01", true);
            character.isBlocking = false;
            // TODO: play sfx
            
        }

    }

    private void CalculateDamage(CharacterManager character)
    {
        if (characterCausingDamage != null)
        {
            //  check for dmg modifiers and modify base dmg
        }

        //  check character for flat defenses and subtract from dmg
        //  character armor absorptions
        physicalDamage -= (physicalDamage * (character.characterStatManager.blockingPhysicalAbsorption / 100));
        magicDamage -= (magicDamage * (character.characterStatManager.blockingMagicAbsorption / 100));
        fireDamage -= (fireDamage * (character.characterStatManager.blockingFireAbsorption / 100));
        lightningDamage -= (lightningDamage * (character.characterStatManager.blockingLightningAbsorption / 100));
        holyDamage -= (holyDamage * (character.characterStatManager.blockingHolyAbsorption / 100));


        //  add all types together, and apply final damage
        finalDamageDealth = Mathf.RoundToInt(physicalDamage + magicDamage + fireDamage + lightningDamage + holyDamage);

        if (finalDamageDealth <= 0)
        {
            finalDamageDealth = 1;
        }

        character.characterStatManager.currentHealth -= finalDamageDealth;
    }

    private void PlayBlockVFX(CharacterManager character)
    {
        // if we have fire, play fire particles also (polish)

    }

    private void PlayDamageSFX(CharacterManager character)
    {
        character.characterSoundFXManager.PlayBlockingSFX();
    }

    private void PlayDirectionalBasedBlockingAnimation(CharacterManager character)
    {
        if (character.isDead) return;

        //1. calculate ïntensity of the hit from poise damage
        //2. play proper animation to match it 

        DamageIntensity damageIntensity = WorldUtilityManager.instance.GetDamageIntensity(poiseDamage);

        switch (damageIntensity)
        {
            case DamageIntensity.Ping:
                damageAnimation = "Block_Ping_01";
                break;
            case DamageIntensity.Light:
                damageAnimation = "Block_Light_01";
                break;
            case DamageIntensity.Medium:
                damageAnimation = "Block_Medium_01";
                break;
            case DamageIntensity.Heavy:
                damageAnimation = "Block_Heavy_01";
                break;
            case DamageIntensity.Critical:
                damageAnimation = "Block_Critical_01";
                break;
        }

       
            character.characterAnimatorManager.lastAnimationPlayed = damageAnimation;
            character.characterAnimatorManager.PlayerTargetActionAnimation(damageAnimation, true);
        
    }
}
