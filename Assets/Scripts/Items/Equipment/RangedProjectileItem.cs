using UnityEngine;

[CreateAssetMenu(menuName = "Items/Ranged Projectile")]
public class RangedProjectileItem : Item
{
    public ProjectileClass projectileClass; // Class of the projectile

    [Header("Velocity")]
    public float forwardVelocity = 2200;
    public float upwardVelocity = 0; 
    public float ammoMass = 0.1f; // Mass of the projectile in kg

    [Header("Capacity")] //todo: remove this if not needed
    public int maxAmmo = 10; // Maximum ammo capacity
    public int currentAmmo = 10; // Current ammo count

    [Header("Element")]
    public WeaponElement element = WeaponElement.None; 

    [Header("Damage")]
    public int physicalDamange = 0;
    public int magicDamage = 0; 
    public int fireDamage = 0; 
    public int holyDamage = 0;
    public int lightningDamage = 0;

    [Header("Model")]
    public GameObject drawProjectileModel; // Model shown during drawing bow
    public GameObject releaseProjectileModel; // Model of the projectile when fired
}
