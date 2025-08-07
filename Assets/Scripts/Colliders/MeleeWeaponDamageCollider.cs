 using UnityEngine;

public class MeleeWeaponDamageCollider : DamageCollider
{
    [Header("Attacking Character")]
    public CharacterManager characterCausingDamage;

    [Header("Weapon Attack Modifiers")]
    public float light_Attack_01_Modifier;
    public float light_Attack_02_Modifier;
    public float light_Attack_03_Modifier;


    public float heavy_Attack_01_Modifier;
    public float heavy_Attack_02_Modifier;
    public float heavy_Attack_03_Modifier;
    public float charged_heavy_Attack_01_Modifier;
    public float running_Attack_Modifier;
    public float rolling_Attack_Modifier;
    public float backstep_Attack_Modifier;

    protected override void Awake()
    {
        base.Awake();

        if (damageCollider == null)
        {
            damageCollider = GetComponent<Collider>();
        }

        damageCollider.enabled = false; //  Melee weapon collider should be disabled at start -> enable when animation allow
    }

    protected override void OnTriggerEnter(Collider other)
    {
        CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();

        if (damageTarget != null)
        {
            if (damageTarget == characterCausingDamage)
                return;

            contanctPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

            //  check if we can damage this target aka friendly fire

            //  check if blocking


            DamageTarget(damageTarget);
        }
        else
        {
            // Puzzle targets – direct hit delivery
            TakeDamageEffect puzzleHit = BuildPuzzleDamageEffect();

            if (other.TryGetComponent(out FlammableObject f))
            {
                f.OnDamageReceived(puzzleHit);
            }
            else if (other.TryGetComponent(out BreakableObject b))
            {
                Debug.Log($"DamageCollider: {b.name} hit by {characterCausingDamage.name}");
                b.OnDamageReceived(puzzleHit);
            }
            else if (other.TryGetComponent(out PressurePlate p))
            {
                p.OnDamageReceived(puzzleHit);
            }
            else if (other.TryGetComponent(out TimedTorch torch))
            {
                Debug.Log($"[MeleeCollider] Found TimedTorch index {torch.torchIndex}");
                torch.OnDamageReceived(puzzleHit);
            }
            else if (other.TryGetComponent(out BreakableRope rope))
            {
                rope.OnDamageReceived(BuildPuzzleDamageEffect());
                return;
            }
            else if (other.TryGetComponent(out FocusBarrel barrel))
            {
                barrel.OnDamageReceived(BuildPuzzleDamageEffect());
                return;
            }

        }
    }

    protected override void DamageTarget(CharacterManager damageTarget)
    {
        //  we dont want to damage the same target more than once in a single attack
        if (charactersDamaged.Contains(damageTarget))
            return;

        charactersDamaged.Add(damageTarget);

        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
        damageEffect.physicalDamage = physicalDamage;
        damageEffect.magicDamage = magicDamage;
        damageEffect.fireDamage = fireDamage;
        damageEffect.holyDamage = holyDamage;
        damageEffect.poiseDamage = poiseDamage;
        damageEffect.contactPoint = contanctPoint;
        damageEffect.weaponElement = element;
        damageEffect.weaponType = weaponModelType;
        damageEffect.angleHitFrom = Vector3.SignedAngle(characterCausingDamage.transform.forward, damageTarget.transform.forward, Vector3.up);

        switch (characterCausingDamage.characterCombatManager.currentAttackType)
        {
            case AttackType.LightAttack01:
                ApplyAttackDamageModifiers(light_Attack_01_Modifier, damageEffect);
                break;
            case AttackType.LightAttack02:
                ApplyAttackDamageModifiers(light_Attack_02_Modifier, damageEffect);
                break;

            case AttackType.HeavyAttack01:
                ApplyAttackDamageModifiers(heavy_Attack_01_Modifier, damageEffect);
                break;

            case AttackType.ChargedHeavyAttack01:
                ApplyAttackDamageModifiers(charged_heavy_Attack_01_Modifier, damageEffect);
                break;
            case AttackType.RunningAttack01:
                ApplyAttackDamageModifiers(running_Attack_Modifier, damageEffect);
                break;
            case AttackType.RollingAttack01:
                ApplyAttackDamageModifiers(rolling_Attack_Modifier, damageEffect);
                break;
            case AttackType.BackstepAttack01:
                ApplyAttackDamageModifiers(backstep_Attack_Modifier, damageEffect);
                break;
            case AttackType.LightAttack03:
                ApplyAttackDamageModifiers(light_Attack_03_Modifier, damageEffect);
                break;
            case AttackType.HeavyAttack02:
                ApplyAttackDamageModifiers(heavy_Attack_02_Modifier, damageEffect);
                break;
            case AttackType.HeavyAttack03:
                ApplyAttackDamageModifiers(heavy_Attack_03_Modifier, damageEffect);
                break;
            default:
                break;
        }

        damageTarget.characterEffectsManager.ProcessInstantEffect(damageEffect);


        // Focus regen if attacker is a player
        if (characterCausingDamage != null && characterCausingDamage.CompareTag("Player"))
        {
            PlayerManager player = characterCausingDamage as PlayerManager;
            var wm = player.playerEquipmentManager.rightWeaponManager;
            if (wm != null && wm.isFlaming) return;   //skip regen while Flame Infuse is active

            var stats = characterCausingDamage.characterStatManager;
            if (damageTarget.CompareTag("FocusBarrel"))
                stats.RefillFocus(50f);
            else
                stats.RefillFocus(30f);
        }

    }

    protected override void GetBlockingDotValues(CharacterManager damageTarget)
    {
        directionFromAttackToDamageTarget = characterCausingDamage.transform.position - damageTarget.transform.position;
        dotValueFromAttackToDamageTarget = Vector3.Dot(directionFromAttackToDamageTarget, damageTarget.transform.forward);
    }

    private void ApplyAttackDamageModifiers(float modifier, TakeDamageEffect damage)
    {
        damage.physicalDamage *= modifier;
        damage.magicDamage *= modifier;
        damage.fireDamage *= modifier;
        damage.holyDamage *= modifier;
        damage.poiseDamage *= modifier;

        //  if attack is a fully charged heavy, multiply by full charge modifier after normal one
    }

    private TakeDamageEffect BuildPuzzleDamageEffect()
    {
        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
        damageEffect.characterCausingDamage = characterCausingDamage;
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