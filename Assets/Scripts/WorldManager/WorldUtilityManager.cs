using System;
using UnityEngine;

public class WorldUtilityManager : MonoBehaviour
{
    public static WorldUtilityManager instance;


    [Header("Headers")]
    [SerializeField] LayerMask characterLayers;
    [SerializeField] LayerMask envLayers;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

    }

    public LayerMask GetCharacterLayers()
    {
        return characterLayers;
    }

    public LayerMask GetEnvLayers()
    {
        return envLayers;
    }

    public bool CanIdamageThisTarget(CharacterType attackingCharacterType, CharacterType targetType)
    {
        if (attackingCharacterType == CharacterType.Player)
        {
            switch (targetType)
            {
                case CharacterType.Player:
                    return false;
                case CharacterType.AI:
                    return true;
            }
        }
        else if (attackingCharacterType == CharacterType.AI)
        {
            switch (targetType)
            {
                case CharacterType.Player:
                    return true;
                case CharacterType.AI:
                    return false;
            }
        }

        return false;
    }

    public float GetAngleToTarget(Transform characterTransform, Vector3 targetsDirection)
    {
        targetsDirection.y = 0;
        float viewableAngle = Vector3.Angle(characterTransform.forward, targetsDirection);
        Vector3 cross = Vector3.Cross(characterTransform.forward, targetsDirection);

        if (cross.y < 0) 
            viewableAngle = -viewableAngle;

        return viewableAngle;
    }

    public DamageIntensity GetDamageIntensity(float poiseDamage)
    {
        // throwing daggers, small items
        DamageIntensity damageIntensity = DamageIntensity.Ping;

        // dagger
       if (poiseDamage >= 10)
            damageIntensity = DamageIntensity.Light;

        // sword, axe, spear
        if (poiseDamage >= 30)
            damageIntensity = DamageIntensity.Medium;

        // great sword, great axe, great spear
        if (poiseDamage >= 80)
            damageIntensity = DamageIntensity.Heavy;

        // boss attacks, large weapons
        if (poiseDamage >= 120)
            damageIntensity = DamageIntensity.Critical;

        return damageIntensity;
    }
}
