using UnityEngine;

public class RangeProjectileDamageCollider : DamageCollider
{
    [Header("Marksman")]
    public CharacterManager characterShottingProjectile;

    [Header("Collision")]
    private bool hasPenetratedSurface = false;
    public Rigidbody rigidBody;
    [SerializeField] CapsuleCollider capsuleCollider;
    private Vector3 desiredForward = Vector3.forward;
    [SerializeField] private float rotateDamping = 5f;

    protected override void Awake()
    {
        base.Awake();

        rigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //todo: fix
        Vector3 velocity = rigidBody.linearVelocity;
        Vector3 flatVel = new Vector3(velocity.x, 0f, velocity.z);

        if (flatVel.sqrMagnitude > 0.01f)
        {
            desiredForward = flatVel.normalized; 
        }
        Quaternion targetRot = Quaternion.LookRotation(desiredForward);
        rigidBody.rotation = Quaternion.Slerp(
            rigidBody.rotation,
            targetRot,
            rotateDamping * Time.fixedDeltaTime
        );
    }

    public void Launch(Vector3 direction, float speed)
    {
        if (rigidBody == null)
            rigidBody = GetComponent<Rigidbody>();

        // Use built-in velocity property (no more linearVelocity)
        rigidBody.linearVelocity = direction.normalized * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        CreatePenetrationIntoObject(collision);

        CharacterManager potentialTarget = collision.transform.gameObject.GetComponent<CharacterManager>();

        if (characterShottingProjectile == null)
            return;

        if (potentialTarget == null)
            return;

        Collider contactCollider = collision.gameObject.GetComponent<Collider>();

        if (contactCollider != null)
            contanctPoint = contactCollider.ClosestPointOnBounds(transform.position);

        if (WorldUtilityManager.instance.CanIdamageThisTarget(characterShottingProjectile.characterType, potentialTarget.characterType))
        {
            CheckForBlock(potentialTarget);
            DamageTarget(potentialTarget);
        }

       

    }

    protected override void OnTriggerEnter(Collider other)
    {
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

        Destroy(gameObject);
    }

    private TakeDamageEffect BuildPuzzleDamageEffect()
    {
        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);

        damageEffect.physicalDamage = physicalDamage;
        damageEffect.magicDamage = magicDamage;
        damageEffect.fireDamage = fireDamage;
        damageEffect.holyDamage = holyDamage;

        damageEffect.lightningDamage = lightningDamage; 
        damageEffect.poiseDamage = poiseDamage;
        damageEffect.contactPoint = contanctPoint;
        damageEffect.weaponElement = element;
        damageEffect.weaponType = weaponModelType;

        return damageEffect;
    }

    private void CreatePenetrationIntoObject(Collision hit)
    {
        if (!hasPenetratedSurface)
        {
            hasPenetratedSurface = true;

            gameObject.transform.position = hit.GetContact(0).point;    
            var emptyObject = new GameObject("PenetrationPoint");
            emptyObject.transform.parent = hit.collider.transform;
            gameObject.transform.SetParent(emptyObject.transform, true);

            transform.position += transform.forward * Random.Range(0.01f, 0.05f);

            rigidBody.isKinematic = true;
            capsuleCollider.enabled = false;

            Destroy(GetComponent<RangeProjectileDamageCollider>());
            Destroy(gameObject, 20);
        }
    }

    protected override void CheckForBlock(CharacterManager damageTarget)
    {
        if (charactersDamaged.Contains(damageTarget)) 
            return;

        float angle = Vector3.Angle(damageTarget.transform.forward, transform.forward);

        if (damageTarget.isBlocking && angle > 145) 
        {
            charactersDamaged.Add(damageTarget);
            TakeBlockingDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeBlockedDamageEffect);

            if (characterShottingProjectile != null)
                damageEffect.characterCausingDamage = characterShottingProjectile;

            damageEffect.physicalDamage = physicalDamage;
            damageEffect.magicDamage = magicDamage;
            damageEffect.fireDamage = fireDamage;
            damageEffect.holyDamage = holyDamage;
            damageEffect.poiseDamage = poiseDamage;
            damageEffect.lightningDamage = lightningDamage;
            damageEffect.baseStaminaDamage = poiseDamage;
            damageEffect.contactPoint = contanctPoint;
            damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);
        }

    }


}
