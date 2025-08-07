using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class CharacterManager : MonoBehaviour
{
    [Header("Status")]
    protected bool _isDead = false;
    public virtual bool isDead{
        get { return _isDead; }
        set
        {
            _isDead = value;
        }
    }


    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public Animator animator;
    [HideInInspector] public CharacterStatManager characterStatManager;
    [HideInInspector] public CharacterEffectsManager characterEffectsManager;
    [HideInInspector] public CharacterAnimatorManager characterAnimatorManager;
    [HideInInspector] public CharacterCombatManager characterCombatManager;
    [HideInInspector] public CharacterSoundFXManager characterSoundFXManager;
    [HideInInspector] public CharacterLocomotionManager characterLocomotionManager;
    [HideInInspector] public CharacterUIManager characterUIManager;

    [Header("Character Type")]
    public CharacterType characterType;

    [Header("Movement")]
    public float verticalMovement;
    public float horizontalMovement;

    [Header("Flags")]
    public bool isPerformingAction = false;
    public bool isJumping = false;
    public bool isSprinting = false;
    public bool isInvulnerable = false;
    public bool isAttacking = false;
    [SerializeField] private bool _isBlocking = false;
    [SerializeField] private bool _isMoving = false;
    [SerializeField] private bool _isChargingAttack = false;
    [SerializeField] private bool _isLockedOn = false;
    [SerializeField] private bool _isActive = true;
    public virtual bool isBlocking
    {
        get { return _isBlocking; }
        set
        {
            _isBlocking = value;
            animator.SetBool("isBlocking", value);
        }
    }
    public bool isLockedOn
    {
        get { return _isLockedOn; }
        set
        {
            _isLockedOn = value;
            if (!value)
            {
                characterCombatManager.SetTarget(null);
            }
        }
    }

    public bool isChargingAttack
    {
        get { return _isChargingAttack; }
        set
        {
            _isChargingAttack = value;
            animator.SetBool("isChargingAttack", value);
        }
    }   

    public bool isMoving
    {
        get { return _isMoving; }
        set
        {
            _isMoving = value;
            animator.SetBool("isMoving", value);
        }
    }

    public virtual bool isActive // TODO: change to check and update
    {
        get { return _isActive; }
        set
        {
            gameObject.SetActive(value);
            _isActive = value;
        }
    }

    protected virtual void Start()
    {
        IgnoreMyOwnCollider();
        characterStatManager.OnHealthChanged += CheckHP;
    }

    protected virtual void OnDestroy()
    {
        characterStatManager.OnHealthChanged -= CheckHP;
    }

    protected virtual void Awake()
    {
        DontDestroyOnLoad(gameObject); //TODO: delete i think
        characterController = GetComponent<CharacterController>();   
        animator = GetComponent<Animator>();    
        characterEffectsManager = GetComponent<CharacterEffectsManager>();
        characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
        characterCombatManager = GetComponent<CharacterCombatManager>();
        characterStatManager = GetComponent<CharacterStatManager>();
        characterSoundFXManager = GetComponent<CharacterSoundFXManager>();
        characterUIManager = GetComponent<CharacterUIManager>();
        characterLocomotionManager = GetComponent<CharacterLocomotionManager>();
    }


    protected virtual void Update()
    {
        animator.SetBool("isGrounded", characterLocomotionManager.isGrounded);
    }

    protected virtual void OnEnable()
    {
        
    }

    protected virtual void OnDisable()
    {

    }

    protected virtual void FixedUpdate()
    {
        
    }

    protected virtual void LateUpdate()
    {

    }

    public virtual IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = default)
    {
        //playerStatManager.currentHealth = 0;
        isDead = true;

        if(!manuallySelectDeathAnimation)
        {
            characterAnimatorManager.PlayerTargetActionAnimation("Dead_01", true);
        }

        //  play sfx

        yield return new WaitForSeconds(5);

        //  give rewards
    }

    public virtual void CheckHP(float lastValue, float newValue)
    {

        if (characterStatManager.currentHealth <= 0)
        {
            StartCoroutine(ProcessDeathEvent());
        }

        if(characterStatManager.currentHealth > characterStatManager.maxHealth)
        {
            characterStatManager.currentHealth = characterStatManager.maxHealth;
        }
    }

    public virtual void ReviveCharacter()
    {

    }

    protected virtual void IgnoreMyOwnCollider()
    {
        Collider characterControllerCollider = GetComponent<Collider>();
        Collider[] damageableCharacterCollider = GetComponentsInChildren<Collider>();
        List<Collider> ignoreColliders = new();

        foreach (var collider in damageableCharacterCollider)
        {
            ignoreColliders.Add(collider);
        }

        ignoreColliders.Add(characterControllerCollider);
        
        foreach (var collider in ignoreColliders)
        {
            foreach (var otherCollider in ignoreColliders)
            {
                Physics.IgnoreCollision(collider, otherCollider, true);   
            }
        }
    }

    public virtual void DestoryAllCurrentActionFX()
    {
        if(characterEffectsManager.activeDrawnProjectileFX != null)
            Destroy(characterEffectsManager.activeDrawnProjectileFX);

    }
}
