using System.Collections.Generic;
using UnityEngine;

public class WorldCharacterEffectsManager : MonoBehaviour
{
    public static WorldCharacterEffectsManager instance;

    /* ─────────────  VFX prefabs  ───────────── */
    [Header("VFX")]
    public GameObject bloodSplatterVFX;
    public GameObject healingVFX;
    public GameObject deadSpotVFX;

    /* ─────────────  Core damage effects  ───────────── */
    [Header("Damage")]
    public TakeDamageEffect takeDamageEffect;
    public TakeBlockingDamageEffect takeBlockedDamageEffect;
    public TakeFocusDamageEffect takeFocusDamageEffect;

    /* ─────────────  Effect asset libraries  ───────────── */
    [Header("Instant Effects")]
    [SerializeField] private List<InstantCharacterEffect> instantEffects = new();

    [Header("Static Effects")]
    [SerializeField] public List<StaticCharacterEffect> staticEffects = new();

    [Header("Runtime Effects")]
    [SerializeField] public List<RuntimeCharacterEffect> runtimeEffects = new();   // NEW

    /* ─────────────  Singleton bootstrap  ───────────── */
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        GenerateEffectIDs();
        DontDestroyOnLoad(gameObject);
    }

    /* ─────────────  Auto-ID assignment  ───────────── */
    private void GenerateEffectIDs()
    {
        for (int i = 0; i < instantEffects.Count; i++)
            instantEffects[i].instantEffectID = i;

        for (int i = 0; i < staticEffects.Count; i++)
            staticEffects[i].staticEffectID = i;

        for (int i = 0; i < runtimeEffects.Count; i++)              
            runtimeEffects[i].runtimeEffectID = i;                  
    }

    /* ─────────────  Optional helper lookups  ───────────── */

    public StaticCharacterEffect GetStaticEffectByID(int id) =>
        (id >= 0 && id < staticEffects.Count) ? staticEffects[id] : null;

    public RuntimeCharacterEffect GetRuntimeEffectByID(int id) =>          
        (id >= 0 && id < runtimeEffects.Count) ? runtimeEffects[id] : null;
}
