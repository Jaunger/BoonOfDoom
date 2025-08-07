// FlameSlashAction.cs
using UnityEngine;

[CreateAssetMenu(menuName = "CharacterActions/Weapon Actions/Flame Slash")]
public class FlameSlashAction : WeaponItemAction
{
    [Header("Cost")]
    [SerializeField] private float focusCost = 15f;

    [Header("Projectile Prefab")]
    [SerializeField] private GameObject projectilePrefab;    // assign your slash prefab

    [Header("Projectile Stats")]
    [SerializeField] private RangedProjectileItem projectileItem;

    public override void AttemptToPerformAction(PlayerManager player, WeaponItem weapon)
    {

        Debug.Assert(projectilePrefab != null, "Projectile prefab is not assigned in FlameSlashAction.");
        if (!weapon.runtimeSkillTree.HasUnlockedNode("Flame Slash")) return;
        if (player.playerStatManager.currentFocus < focusCost) return;

        var fx = Instantiate(WorldCharacterEffectsManager.instance.takeFocusDamageEffect);
        fx.focusCost = focusCost;
        player.characterEffectsManager.ProcessInstantEffect(fx);

        Transform spawn = player.playerCombatManager.lockOnTransform;
        if (spawn == null || projectilePrefab == null || projectileItem == null) return;

        Camera cam = PlayerCamera.instance.cameraObject;
        Vector3 origin = spawn.position;
        Vector3 dir = cam.transform.forward;
        if (Physics.Raycast(cam.transform.position, dir, out var hit, 100f, WorldUtilityManager.instance.GetEnvLayers()))
            dir = (hit.point - origin).normalized;
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);

        GameObject projObj = Instantiate(projectilePrefab, origin, rot);

        var proj = projObj.GetComponent<RangeProjectileDamageCollider>();
        Debug.Assert(proj != null, "Projectile prefab does not have RangeProjectileDamageCollider component.");
        proj.characterShottingProjectile = player;

        proj.physicalDamage = projectileItem.physicalDamange;
        proj.magicDamage = projectileItem.magicDamage;
        proj.fireDamage = projectileItem.fireDamage;
        proj.holyDamage = projectileItem.holyDamage;
        proj.lightningDamage = projectileItem.lightningDamage;
        
        proj.Launch(dir, projectileItem.forwardVelocity);

        Collider arrowCol = projObj.GetComponent<Collider>();
        foreach (var c in player.GetComponentsInChildren<Collider>())
            Physics.IgnoreCollision(arrowCol, c, true);
    }
}
