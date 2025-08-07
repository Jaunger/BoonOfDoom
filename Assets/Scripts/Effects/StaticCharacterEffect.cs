using UnityEngine;

public class StaticCharacterEffect : ScriptableObject
{
    [Header("Effect ID")]
    public int staticEffectID;

    public virtual void ProcessStaticEffect(CharacterManager character)
    {

    }

    public virtual void RemoveStaticEffect(CharacterManager character)
    {

    }
}
