using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Damage")]
public class TakeDamageEffect : InstantCharacterEffect
{
    [Header("Character Causing Damage")]
    public CharacterManager characterCausingDamage;

    [Header("Damage")]
    public float physicalDamage = 0; // split into 4 types "Standard", "Strike", "Slash", "Pierce"
    public float magicDamage = 0;
    public float fireDamage = 0;
    public float lightningDamage = 0;
    public float holyDamage = 0;

    [Header("Weapon Type")]
    public WeaponModelType weaponType;
    public WeaponElement weaponElement = WeaponElement.None;

    [Header("Final Damage")]
    private int finalDamageDealth = 0;

    [Header("Poise")]
    public float poiseDamage = 0;
    public bool poiseIsBroken = false;

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



        if (character.isDead)
            return;

        //  Check for invulnerability return

        CalculateDamage(character);
        PlayDirectionalBasedDamageAnimation(character);
        //  check build ups
        PlayDamageSFX(character);
        PlayDamageVFX(character);

        //  If character is A.I, check for new target if causer is preset
    }

    private void CalculateDamage(CharacterManager character)
    {
        if (characterCausingDamage != null)
        {
            //  check for dmg modifiers and modify base dmg
        }

        //  check character for flat defenses and subtract from dmg
        float poiseDamageAdjusted = poiseDamage;

        if (character is PlayerManager playerPoiseReduc)
        {
            // Forward-facing = attacker is in front of you
            if (angleHitFrom >= 145 || angleHitFrom <= -145)
            {
                poiseDamageAdjusted -= poiseDamage * (playerPoiseReduc.playerStatManager.frontalPoiseAbsorption / 100f);
            }
        }
        //  character armor absorptions

        //  add all types together, and apply final damage
        finalDamageDealth = Mathf.RoundToInt(physicalDamage + magicDamage + fireDamage + lightningDamage + holyDamage);

        if (character is PlayerManager p)
        {
            finalDamageDealth -= Mathf.RoundToInt(finalDamageDealth * (p.playerStatManager.currentDamageReductionPercent / 100f));
        }


        if (finalDamageDealth <= 0)
        {
            finalDamageDealth = 1;
        }

        character.characterStatManager.currentHealth -= finalDamageDealth;


        // we subtract poise damage from the total
        float finalPoise = poiseDamageAdjusted;

        if (characterCausingDamage is PlayerManager player)
        {
            finalPoise *= player.playerStatManager.currentPoiseDamageMultiplier;
            player.playerStatManager.currentPoiseDamageMultiplier = 1f; // reset

            var tree = player.playerInventoryManager.currentRightWeapon?.runtimeSkillTree;
            if (player.playerEffectManager.TryGetRuntimeEffect<BulwarkMomentumEffect>(out var bulwark))
            {
                bulwark.RegisterHit(player);   
            }

        }

        character.characterStatManager.totalPoiseDamage -= finalPoise;

        float remainingPoise = character.characterStatManager.basePoiseDefense +
            character.characterStatManager.offensivePoiseDamage +
            character.characterStatManager.totalPoiseDamage;

        if (remainingPoise <= 0)
            poiseIsBroken = true;

        // character has been hit we reset the poise timer
        character.characterStatManager.poiseResetTimer = character.characterStatManager.defaultPoiseResetTime;
    }

    private void PlayDamageVFX(CharacterManager character)
    {
        // if we have fire, play fire particles also (polish)

        character.characterEffectsManager.PlayBloodSplatterVFX(contactPoint);
    }

    private void PlayDamageSFX(CharacterManager character)
    {
        AudioClip physicalDamageSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.physicalDamageSFX);

        character.characterSoundFXManager.PlaySoundFX(physicalDamageSFX);
        character.characterSoundFXManager.PlayDamageGrunt();
        // TODO: if elemntal > 0 do elemntal sound also
    }

    private void PlayDirectionalBasedDamageAnimation(CharacterManager character)
    {
        if (character.isDead) return;

        if (poiseIsBroken)
        {
            if (angleHitFrom >= 145 && angleHitFrom <= 180)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.forward_M_Damage);
            }
            if (angleHitFrom <= -145 && angleHitFrom >= -180)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.forward_M_Damage);
            }
            if (angleHitFrom >= -45 && angleHitFrom <= 45)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.backwards_M_Damage);
            }
            if (angleHitFrom >= -144 && angleHitFrom <= -45)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.left_M_Damage);
            }
            if (angleHitFrom >= 45 && angleHitFrom <= 144)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.right_M_Damage);
            }
        }
        else
        {
            if (angleHitFrom >= 145 && angleHitFrom <= 180)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.forward_Ping_Damage);
            }
            if (angleHitFrom <= -145 && angleHitFrom >= -180)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.forward_Ping_Damage);
            }
            if (angleHitFrom >= -45 && angleHitFrom <= 45)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.backwards_Ping_Damage);
            }
            if (angleHitFrom >= -144 && angleHitFrom <= -45)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.left_Ping_Damage);
            }
            if (angleHitFrom >= 45 && angleHitFrom <= 144)
            {
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.right_Ping_Damage);
            }

        }

        character.characterAnimatorManager.lastAnimationPlayed = damageAnimation;

        if (poiseIsBroken)
        {
            character.characterAnimatorManager.PlayerTargetActionAnimation(damageAnimation, true);
        }
        else
        {
            character.characterAnimatorManager.PlayerTargetActionAnimation(damageAnimation, false, false, true, true);
        }


    }
}