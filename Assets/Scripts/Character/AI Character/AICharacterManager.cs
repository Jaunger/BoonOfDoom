using UnityEngine;
using UnityEngine.AI;
using UnityEngine.TextCore.Text;

public class AICharacterManager : CharacterManager
{
    [Header("Character Name")]
    public string characterName = "";

    [HideInInspector] public AICharacterCombatManager aiCharacterCombatManager;
    [HideInInspector] public AICharacterLocomotionManager aiCharacterLocomotionManager;
    [HideInInspector] public AICharacterAnimatorManager aiCharacterAnimationManager;
    [HideInInspector] public AICharacterInventoryManager aiCharacterInventoryManager;

    [Header("Current State")]
    public AIState currentState;

    [Header("Navmesh Agent")]
    public NavMeshAgent navMeshAgent;

    [Header("Sleep")]
    public bool isAwake = true;
    public string sleepAnimation = "Sleep_01";
    public string wakeAnimation = "Wake_01";

    [Header("AI States")]
    public IdleState idleState;
    public PursueTargetState pursueState;
    public CombatStanceState combatStanceState;
    public AttackState attackState;
    public InvestigateSoundState investigateSoundState;

    [Header("Activation Beacon")]
    [SerializeField] protected AIActivationBeacon beacon;

    public override bool isDead
    {
        get => base.isDead;
        set
        {
            base.isDead = value;
            Debug.Log("died");
            if (isDead)
            {
                aiCharacterInventoryManager.DropItem();

                PlayerManager player = FindFirstObjectByType<PlayerManager>();
                if (player == null)
                {
                    Debug.LogError("PlayerManager not found in the scene.");
                    return;
                }

                aiCharacterCombatManager.AwardSoulsOnDeath(player);
                
                var atk = player.playerCombatManager.currentAttackType;

                bool qualifiedForRallyCleave = atk == AttackType.HeavyAttack03 || atk == AttackType.ChargedHeavyAttack01;

                // Check if the player has the RallyCleaveEffect and if the attack qualifies for it
                if (qualifiedForRallyCleave && player.playerEffectManager.TryGetRuntimeEffect<RallyCleaveEffect>(out var rallyCleaveEffect))
                {
                    rallyCleaveEffect.OnEnemyKilled(player);
                }


                if (player.playerInventoryManager.currentRightWeapon != null)
                {
                    player.playerInventoryManager.currentRightWeapon.GrantWeaponEXP(aiCharacterCombatManager.weaponEXPReward);
                }
            }
        }
    }

    protected override void Start()
    {
        base.Start();


        animator.keepAnimatorStateOnDisable = true;



        if (!isAwake)
            animator.Play(sleepAnimation);

        if (isDead)
            animator.Play("Dead_01");

    }

    protected override void Awake()
    {
        base.Awake();
        aiCharacterCombatManager = GetComponent<AICharacterCombatManager>();
        navMeshAgent = GetComponentInChildren<NavMeshAgent>();
        aiCharacterLocomotionManager = GetComponent<AICharacterLocomotionManager>();
        aiCharacterAnimationManager = GetComponent<AICharacterAnimatorManager>();
        aiCharacterInventoryManager = GetComponent<AICharacterInventoryManager>();

        //  use copy to avoid modifying the original state
        idleState = Instantiate(idleState);
        pursueState = Instantiate(pursueState);
        combatStanceState = Instantiate(combatStanceState);
        attackState = Instantiate(attackState);
        if (investigateSoundState != null)
            investigateSoundState = Instantiate(investigateSoundState);


        currentState = idleState;

    }

    protected override void Update()
    {
        base.Update();

        aiCharacterCombatManager.HandleActionRecovery(this);

        if (navMeshAgent == null)
            return;

        ProcessStateMachine();

        if (!navMeshAgent.enabled)
            return;

        Vector3 positionDifference = navMeshAgent.transform.position - transform.position;  

        if (positionDifference.magnitude > 0.2f)
            navMeshAgent.transform.localPosition = Vector3.zero;

    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

    }

    protected override void OnEnable()
    {
        base.OnEnable();

    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (beacon != null)
        {
            Destroy(beacon);
        }
    }

    private void ProcessStateMachine()
    {
        AIState nextState = currentState?.Tick(this);

        if (nextState != null)
        {
            currentState = nextState;
        }

        navMeshAgent.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        if (aiCharacterCombatManager.currentTarget != null)
        {
            aiCharacterCombatManager.targetsDirection = aiCharacterCombatManager.currentTarget.transform.position - transform.position;
            aiCharacterCombatManager.viewableAngle = WorldUtilityManager.instance.GetAngleToTarget(transform, aiCharacterCombatManager.targetsDirection);
            aiCharacterCombatManager.distanceFromTarget = Vector3.Distance(transform.position, aiCharacterCombatManager.currentTarget.transform.position);
        }

        if (navMeshAgent.enabled)
        {
            Vector3 agentDestination = navMeshAgent.destination;
            float remainingDistance = Vector3.Distance(agentDestination, transform.position);
       
            if (remainingDistance > navMeshAgent.stoppingDistance)
            {
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }
        }
        else
        {
            isMoving = false;
        }
    }

    public void ActivateCharacter(PlayerManager player)
    {
        aiCharacterCombatManager.playerWithinReach = player;

        if (aiCharacterCombatManager.playerWithinReach != null)
        {
            isActive = true;
        }
        else
        {
            isActive = false;
        }

    }

    public void DeactivateCharacter(PlayerManager player)
    {
        aiCharacterCombatManager.playerWithinReach = null;

        if (beacon != null)
        {
            beacon.gameObject.transform.position = transform.position;
            beacon.gameObject.SetActive(true);
        }

        if (aiCharacterCombatManager.playerWithinReach == null)
        {
            isActive = false;
        }
        else
        {
            aiCharacterCombatManager.SetTarget(null);
            isActive = true;
        }

    }

    public bool CreateActivationBeacon()
    {
        if (beacon == null)
        {
            GameObject beaconObject = Instantiate(WorldAIManager.instance.beaconPrefab, transform.position, Quaternion.identity);

            beacon = beaconObject.GetComponent<AIActivationBeacon>();
            beacon.SetOwnerOfBeacon(this);
            return true;
        }
        else
        {
            beacon.transform.position = transform.position;
            beacon.gameObject.SetActive(true);
            return false;
        }

    }
}
