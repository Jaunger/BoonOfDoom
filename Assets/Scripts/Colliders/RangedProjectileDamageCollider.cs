using UnityEngine;

public class RangedProjectileDamageCollider : DamageCollider
{
    [Header("Projectile")]
    public float speed = 20f;
    public float lifeTime = 4f;

    private Rigidbody rb;
    private CharacterManager attacker;

    [Header("Attacking Character")]
    public CharacterManager characterCausingDamage;

    /* ---------------------------------------------------- */
    protected override void Awake()      
    {
        base.Awake();

        rb = GetComponent<Rigidbody>();
        attacker = GetComponentInParent<CharacterManager>();

        // collider from base class (guaranteed by prefab)
        damageCollider.enabled = true;
    }


    void Start()
    {
        // Launch once on spawn
        Destroy(gameObject, lifeTime);
    }

    public void Launch(Vector3 direction)
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        rb.linearVelocity = direction.normalized * speed;
    }


    public void SetAttacker(CharacterManager who)     
    {
        attacker = who;
        Debug.Log($"Attacker set to: {who.name}");
        // Ignore physical collisions with attacker’s colliders
        foreach (Collider col in attacker.GetComponentsInChildren<Collider>())
        {
            Physics.IgnoreCollision(damageCollider, col);
        }
    }

    /* ---------------------------------------------------- */
    protected override void DamageTarget(CharacterManager target)
    {
        if (charactersDamaged.Contains(target)) return;
        charactersDamaged.Add(target);

        TakeDamageEffect dmg = Instantiate(
            WorldCharacterEffectsManager.instance.takeDamageEffect);

        dmg.characterCausingDamage = attacker;
        dmg.fireDamage = fireDamage;        // inherited field
        dmg.contactPoint = target.transform.position;

        target.characterEffectsManager.ProcessInstantEffect(dmg);

        Destroy(gameObject);   // single-hit projectile
    }

    protected override void OnTriggerEnter(Collider other)
    {
        CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();

        contanctPoint = other.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

        if (damageTarget != null)
        {
            if (damageTarget == attacker)
                return;

            DamageTarget(damageTarget);
        }
        else
        {
            // Puzzle targets
            TakeDamageEffect puzzleHit = BuildPuzzleDamageEffect();

            if (other.TryGetComponent(out FlammableObject f))
            {
                f.OnDamageReceived(puzzleHit);
            }
            else if (other.TryGetComponent(out BreakableObject b))
            {
                b.OnDamageReceived(puzzleHit);
            }
            else if (other.TryGetComponent(out PressurePlate p))
            {
                p.OnDamageReceived(puzzleHit);
            }
            else if (other.TryGetComponent(out FocusBarrel barrel))
            {
                barrel.OnDamageReceived(BuildPuzzleDamageEffect());
                return;
            }
            else if (other.TryGetComponent(out BreakableRope rope))
            {
                rope.OnDamageReceived(BuildPuzzleDamageEffect());
                return;
            }
        }

        Destroy(gameObject); // optional: make sure projectile disappears after impact
    }

    private TakeDamageEffect BuildPuzzleDamageEffect()
    {
        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);

        damageEffect.physicalDamage = physicalDamage;
        damageEffect.magicDamage = magicDamage;
        damageEffect.fireDamage = fireDamage;
        damageEffect.holyDamage = holyDamage;
        damageEffect.poiseDamage = poiseDamage;
        damageEffect.contactPoint = contanctPoint;
        damageEffect.weaponElement = element;
        damageEffect.weaponType = weaponModelType;

        return damageEffect;
    }

}
