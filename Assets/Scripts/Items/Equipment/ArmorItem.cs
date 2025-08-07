using UnityEngine;

public class ArmorItem : EquipmentItem
{
    [Header("Equipment Absorption Bonus")]
    public float physicalDamageAbsorption;
    public float magicDamageAbsorption;
    public float fireDamageAbsorption;
    public float lightningDamageAbsorption;
    public float holyDamageAbsorption;

    [Header("Equipment Resistance Bonus")]
    public float immunity; // poison/rot resistance
    public float robustness; // frost/bleed resistance
    public float focus; // sleep/madness

    [Header("Poise")]
    public float poise;

    [SerializeField] public EquipmentModel[] equipmentModels; // The models that will be used for this armor item
}
