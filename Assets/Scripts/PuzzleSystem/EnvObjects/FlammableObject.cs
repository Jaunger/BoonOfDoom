using UnityEngine;

public class FlammableObject : MonoBehaviour
{
    [Header("FX")]
    [SerializeField] public GameObject firePrefab;
    [SerializeField] private float destroyDelay = 2f;

    private bool burning = false;

    public void OnDamageReceived(TakeDamageEffect hit)
    {
        if (burning) return;

        bool isFire = hit.weaponElement == WeaponElement.Fire || hit.fireDamage > 0f;
        if (!isFire) return;

        IgniteAndDestroy();
    }

    private void IgniteAndDestroy()
    {
        burning = true;
        if (firePrefab) Instantiate(firePrefab, transform.position, Quaternion.identity, transform);
        // TODO VFX: sparks, sound

        Destroy(gameObject, destroyDelay);
    }
}
