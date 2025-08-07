using UnityEngine;

public class RuntimeCharacterEffect : ScriptableObject
{
    public int runtimeEffectID;

    /// Called once, immediately after the effect is applied
    public virtual void OnApply(CharacterManager character) { }

    /// Called every frame while the effect is active
    public virtual void Tick(CharacterManager character) { }

    /// Called once when the effect is removed (e.g., on weapon switch)
    public virtual void OnRemove(CharacterManager character) { }
}
