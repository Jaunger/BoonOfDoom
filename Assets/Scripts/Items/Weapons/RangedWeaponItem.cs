using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapons/Ranged Weapon")]
public class RangedWeaponItem : WeaponItem
{
    [Header("Ranged SFX")]
    public AudioClip[] drawSfx; // Sound effects for drawing the weapon
    public AudioClip[] releaseSfx; // Sound effects for shooting the weapon


}
