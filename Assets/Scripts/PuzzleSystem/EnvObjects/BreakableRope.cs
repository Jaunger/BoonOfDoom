using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BreakableRope : MonoBehaviour
{
    [Header("Boulder to spawn when rope burns")]
    public GameObject boulderPrefab;         
    public Transform spawnPoint;            
    public float destroyDelay = 0.2f;     

    private bool burned = false;

    public void OnDamageReceived(TakeDamageEffect hit)
    {
        Debug.Log($"BreakableRope: OnDamageReceived hit={hit}");
        if (burned) return;
        if (hit.weaponElement != WeaponElement.Fire && hit.fireDamage <= 0f) return;

        burned = true;

        Instantiate(boulderPrefab, spawnPoint.position, spawnPoint.rotation);

        // TODO VFX: sparks / rope-burn
        Destroy(gameObject, destroyDelay);
    }
}
