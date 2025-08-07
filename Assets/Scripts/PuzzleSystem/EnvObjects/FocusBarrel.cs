using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FocusBarrel : MonoBehaviour
{
    [Header("Focus settings")]
    [Tooltip("Total focus the barrel can give before it runs dry")]
    public float focusReserve = 100f;

    [Tooltip("Focus restored per valid hit")]
    public float focusPerHit = 30f;

    [Header("Optional FX")]
    public ParticleSystem hitFX;
    public AudioSource hitSfx;
    public GameObject emptyBarrelPrefab;   // replace when depleted

    private bool depleted => focusReserve <= 0f;

    public void OnDamageReceived(TakeDamageEffect hit)
    {
        if (depleted) return;
        if (hit.characterCausingDamage is not PlayerManager player) return;

        float give = Mathf.Min(focusPerHit, focusReserve);
        focusReserve -= give;

        player.playerStatManager.RefillFocus(give);

        // VFX / SFX
        if (hitFX) hitFX.Play();
        if (hitSfx) hitSfx.Play();

        if (depleted)
            BecomeEmpty();
    }

    private void BecomeEmpty()
    {
        // swap mesh or just disable glow
        if (emptyBarrelPrefab)
            Instantiate(emptyBarrelPrefab, transform.position, transform.rotation);

        Destroy(gameObject);
    }
}
