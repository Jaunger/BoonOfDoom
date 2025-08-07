using UnityEngine;

public class BreakableObject : MonoBehaviour
{
    [Header("HP needed if not HeavyImpact")]
    public float breakThreshold = 350f;

    public GameObject debrisPrefab;
    public float destroyDelay = 0.1f;

    private float cumulativeAxDamage = 0f;
    private bool broken = false;

    public void OnDamageReceived(TakeDamageEffect hit)
    {
        if (broken) return;

        Debug.Log($"BreakableObject: {hit.weaponType} hit with {hit.physicalDamage} damage and {hit.weaponElement} ");

        /* Rule 1: HeavyImpact always breaks */
        if (hit.weaponElement == WeaponElement.HeavyImpact)
        {
            Shatter();
            return;
        }

        /* Rule 2: Only Great-Axe swings add toward threshold */
        if (hit.weaponType == WeaponModelType.GreatAxe)
        {
            cumulativeAxDamage += hit.physicalDamage;  // use only axe damage

            if (cumulativeAxDamage >= breakThreshold)
                Shatter();
        }
    }

    private void Shatter()
    {
        broken = true;

        if (debrisPrefab)
            Instantiate(debrisPrefab, transform.position, transform.rotation);

        // TODO VFX / SFX
        Destroy(gameObject, destroyDelay);
    }
}
