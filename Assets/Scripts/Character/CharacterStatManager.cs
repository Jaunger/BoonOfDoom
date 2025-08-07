using System;
using UnityEngine;

public class CharacterStatManager : MonoBehaviour
{
    public event Action<float, float> OnHealthChanged;


    public CharacterManager characterManager;

    [Header("Souls")]
    public int soulsDroppedOnDeath = 50;

    [Header("Attributes")]
    public int endurance = 1;
    public int vitality = 1;

    [Header("Stats")]
    public int maxStamina = 10;
    public float currentStamina = 1;
    public int maxHealth = 10;
    public int maxFocus = 100;
    [SerializeField] private float _currentHealth = 1;
    [SerializeField] protected float _currentFocus = 0;

    public float currentFocus
    {
        get => _currentFocus;
        set
        {
            Debug.Log($"Setting focus: {value} (max: {maxFocus})");
            float clamped = Mathf.Clamp(value, 0f, maxFocus);
            float last = _currentFocus;
            _currentFocus = clamped;
            OnFocusChange(last, _currentFocus);
        }
    }
    [Header("Blocking Absorption")]
    public float blockingPhysicalAbsorption;
    public float blockingMagicAbsorption;
    public float blockingFireAbsorption;
    public float blockingLightningAbsorption;
    public float blockingHolyAbsorption;
    public float blockingStability;

    [Header("Armor Absorption")]
    public float armorPhysicalDamageAbsorption;
    public float armorMagicDamageAbsorption;
    public float armorFireDamageAbsorption;
    public float armorLightningDamageAbsorption;
    public float armorHolyDamageAbsorption;


    [Header("Armor Resistance")]
    public float armorImmunity; // poison/rot resistance
    public float armorRobustness; // frost/bleed resistance
    public float armorFocus; // sleep/madness

    [Header("Poise")]
    public float totalPoiseDamage; // how much poise damage has been taken
    public float offensivePoiseDamage; // bonus poise damage gained from using weapons (heavy weapons larger bonus)
    public float basePoiseDefense; // gained from armor/ talisman (armor doesnt exist)
    public float defaultPoiseResetTime = 8; // how long it takes to reset poise after being staggered
    public float poiseResetTimer = 0; 


    protected bool healthChangedInInspector = false;

    public float currentHealth
    {
        get { return _currentHealth; }

        set
        {
            float lastHP = _currentHealth;
            _currentHealth = value;
            if (characterManager.characterUIManager.displayHPBar)
            {
                characterManager.characterUIManager.OnHPChanged(Mathf.RoundToInt(lastHP), Mathf.RoundToInt(value));
            }
          

            OnHealthChange();

            OnHealthChanged?.Invoke(lastHP, value);

        }
    }

    private void OnValidate()
    {
        healthChangedInInspector = true;
    }

    protected virtual void Update()
    {
        if (healthChangedInInspector)
        {
            healthChangedInInspector = false;
            characterManager.CheckHP(0,0);
        }
        HandlePoiseResetTimer();
    }

    protected virtual void Awake()
    {
        characterManager = GetComponent<CharacterManager>();
    }

    protected virtual void Start()
    {

    }

    public int CalculateStaminaBasedOnLevel(int endurance) 
    {
        float stamina = 0;

        //  TODO: Create euqation for stamina 
        stamina = endurance * 10;
        return Mathf.RoundToInt(stamina);

    }

    public int CalculateHealthBasedOnLevel(int vitality)
    {
        float health = 0;

        //  TODO: Create euqation for stamina 
        health = vitality * 15;
        return Mathf.RoundToInt(health);
    }

    public virtual void SetStamina(float staminaLevel)
    {

    }

    public virtual void RegenerateStamina()
    {

    }

    public virtual void DecreaseStamina(float decrement)
    {

    }

    public virtual void IncreateStamina(float increament)
    {

    }

    public virtual void OnStaminaChange()
    {

    }

    public virtual void SetHealth(float healthLevel)
    {

    }

    public virtual void RegenerateHealth()
    {

    }

    public virtual void DecreaseHealth(float decrement)
    {
        currentHealth -= decrement;
    }

    public virtual void IncreateHealth(float increament)
    {
        currentHealth += increament;

    }

    public virtual void OnHealthChange()
    {

    }

    public virtual void OnFocusChange(float lastValue, float newValue) { }

    //TODO: change drains to up to 100% and down to 0% focus not below or above
    public virtual void DrainFocus(float amount) => currentFocus -= amount;

    public virtual void RefillFocus(float amount) => currentFocus = amount + currentFocus;

    protected virtual void HandlePoiseResetTimer()
    {
        if (poiseResetTimer > 0)
        {
            poiseResetTimer -= Time.deltaTime;
        }
        else
        {
            totalPoiseDamage = 0;
        }
    }



}

