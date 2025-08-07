using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectManager : CharacterEffectsManager 

{
    [Header("Debug delete later")]
    [SerializeField] InstantCharacterEffect effectToTest;
    [SerializeField] bool processEffect = false;


    private void Update()
    {
        if(processEffect)
        {
            processEffect = false;
            //TakeStaminaDamageEffect effect = Instantiate(effectToTest) as TakeStaminaDamageEffect;
            //effect.staminaDamage = 5.5f;

            InstantCharacterEffect effect = Instantiate(effectToTest);
            ProcessInstantEffect(effect);
        }
    }

    public void ApplyStaticEffectsFromSkillTree(WeaponSkillTree tree)
    {
        if (tree == null) return;

        foreach (var node in tree.GetUnlockedNodes())
        {
            if (!node.applyOnEquip) continue;   // NEW guard

            foreach (var id in node.staticEffectIDs)
            {
                if (id >= 0 && id < WorldCharacterEffectsManager.instance.staticEffects.Count)
                {
                    var fx = WorldCharacterEffectsManager.instance.staticEffects[id];
                    AddStaticEffect(fx);
                    Debug.Log($"Applied static effect: {fx.name} from node {node.skillName}");
                }
            }
        }
    }

    public void ApplyRuntimeEffectsFromSkillTree(WeaponSkillTree tree)
    {
        if (tree == null) return;

        foreach (var node in tree.GetUnlockedNodes())
        {
            if (!node.applyOnEquip) continue;   // NEW guard

            foreach (var id in node.runtimeEffectIDs)
            {
                if (id >= 0 && id < WorldCharacterEffectsManager.instance.staticEffects.Count)
                {
                    var fx = WorldCharacterEffectsManager.instance.runtimeEffects[id];
                    AddRuntimeEffect(fx);
                    Debug.Log($"Applied static effect: {fx.name} from node {node.skillName}");
                }
            }
        }
    }


}
