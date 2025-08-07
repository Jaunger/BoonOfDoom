using System.Collections.Generic;
using UnityEngine;

public class CharacterEffectsManager : MonoBehaviour
{
    CharacterManager character;

    [Header("Current Active FX")]
    public GameObject activeQuickSlotItemFX;
    public GameObject activeDrawnProjectileFX;

    [Header("VFX")]
    [SerializeField] GameObject bloodSplatterVFX;

    [Header("Static Effects")]
    [SerializeField] List<StaticCharacterEffect> staticEffects = new();

    [Header("Runtime Effects")]
    [SerializeField] List<RuntimeCharacterEffect> runtimeEffects = new();
    private void Awake()
    {
        character = GetComponent<CharacterManager>();
    }

    private void LateUpdate()   // tick runtime effects every frame
    {
        foreach (var fx in runtimeEffects)
            fx.Tick(character);
    }

    public virtual void ProcessInstantEffect(InstantCharacterEffect effect)
    {
        effect.ProcessEffect(character);
    }

    public void PlayBloodSplatterVFX(Vector3 contactPoint)
    {
        if (bloodSplatterVFX != null)
        {
            GameObject bloodSplatter = Instantiate(bloodSplatterVFX, contactPoint, Quaternion.identity);
        }
        else
        {
            GameObject bloodSplatter = Instantiate(WorldCharacterEffectsManager.instance.bloodSplatterVFX, contactPoint, Quaternion.identity);
        }


    }

    public void AddStaticEffect(StaticCharacterEffect effect)
    {
        if (staticEffects.Contains(effect)) return;  

        staticEffects.Add(effect);

        effect.ProcessStaticEffect(character);

        for (int i = staticEffects.Count - 1; i > -1 ; i--)
        {
            if (staticEffects[i] == null)
            {
                staticEffects.RemoveAt(i);
            }
        }
    }

    public void RemoveStaticEffect(int effectID)
    {
        StaticCharacterEffect effectToRemove;

        for (int i = 0; i < staticEffects.Count; ++i)
        {
            if (staticEffects[i] != null)
            { 
                if (staticEffects[i].staticEffectID == effectID)
                {
                    effectToRemove = staticEffects[i];
                    effectToRemove.RemoveStaticEffect(character);
                    staticEffects.Remove(effectToRemove);
                }
            }
        }
    }

    public void RemoveAllStaticEffectsFromWeapon()
    {
        var effectsToRemove = new List<StaticCharacterEffect>(staticEffects);
        foreach (var fx in effectsToRemove)
        {
            RemoveStaticEffect(fx.staticEffectID);
        }
    }

    public void AddRuntimeEffect(RuntimeCharacterEffect effectAsset)
    {
        var fx = Instantiate(effectAsset);          // clone → independent state
        runtimeEffects.Add(fx);
        fx.OnApply(character);
    }

    public void RemoveAllRuntimeEffects()
    {
        foreach (var fx in runtimeEffects)
            fx.OnRemove(character);
        runtimeEffects.Clear();
    }

    public bool TryGetRuntimeEffect<T>(out T fx) where T : RuntimeCharacterEffect
    {
        foreach (var e in runtimeEffects)
        {
            if (e is T match)
            {
                fx = match;
                return true;
            }
        }
        fx = null;
        return false;
    }

}
