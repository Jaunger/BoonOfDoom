using UnityEngine;

public class PlayerStatManager : CharacterStatManager
{
    PlayerManager player;

    [Header("Stamina Regeneration")]
    [SerializeField] float staminaRegenAmount = 1f;
    protected float staminaRegenerationTimer = 0;
    protected float staminaTickTimer = 0;
    [SerializeField]
    float staminaRegenDelay = .5f;
    public float frontalPoiseAbsorption = 0f; // Percent (0–100)
    public float boneBreakerPoiseBonus = 0f; 
    public float currentPoiseDamageMultiplier = 1f;
    public float currentDamageReductionPercent = 0f;

    [Header("Souls")] 
    public int souls = 0;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    protected override void Start()
    {
        base.Start();

        CalculateHealthBasedOnLevel(vitality);
        CalculateStaminaBasedOnLevel(endurance);
        PlayerUIManager.instance.hudManager.SetNewFocusValue(0, _currentFocus);
    }

    public override void SetStamina(float staminaLevel)
    {
        currentStamina = staminaLevel;
        OnStaminaChange();
    }

    public override void DecreaseStamina(float decrement)
    {
        //Debug.Log(decrement);

        currentStamina -= decrement;
        OnStaminaChange();
        staminaRegenerationTimer = 0;

    }

    public override void IncreateStamina(float increament)
    {
        //Debug.Log(increament);

        currentStamina += increament;
        OnStaminaChange();
    }

    public override void OnStaminaChange()
    {
        //Debug.Log(currentStamina);
        PlayerUIManager.instance.hudManager.SetNewStaminaValue(0, currentStamina);
    }

    public override void RegenerateStamina()
    {
        if (player.isSprinting)
            return;

        if (player.isJumping)
            return;

        if (player.isPerformingAction)
            return;

        staminaRegenerationTimer += Time.deltaTime;

        if (staminaRegenerationTimer >= staminaRegenDelay)
        {
            if (player.characterStatManager.currentStamina < player.characterStatManager.maxStamina)
            {
                staminaTickTimer += Time.deltaTime;

                if (staminaTickTimer >= 0.1)
                {
                    staminaTickTimer = 0;
                    IncreateStamina(staminaRegenAmount);
                }
            }

        }
    }

    public override void SetHealth(float healthLevel)
    {
        currentHealth = healthLevel;
    }

    public override void DecreaseHealth(float decrement)
    {
        currentHealth -= decrement;
        //timer
    }

    public override void IncreateHealth(float increament)
    {
        currentHealth += increament;
    }

    public override void OnHealthChange()
    {
        PlayerUIManager.instance.hudManager.SetNewHealthValue(0, currentHealth);
    }

    public void IncreaseVitality()
    {
        vitality++;
        maxHealth = CalculateHealthBasedOnLevel(vitality);
        PlayerUIManager.instance.hudManager.SetMaxHealthValue(maxHealth);
        SetHealth(maxHealth);
    }

    public void IncreaseEndurance()
    {
        endurance++;
        maxStamina = CalculateStaminaBasedOnLevel(endurance);
        PlayerUIManager.instance.hudManager.SetMaxStaminaValue(maxStamina);
        SetStamina(maxStamina);
    }

    public void CalculateTotalArmorAbsorption()
    {
        armorPhysicalDamageAbsorption = 0;
        armorMagicDamageAbsorption = 0;
        armorFireDamageAbsorption = 0;
        armorLightningDamageAbsorption = 0;
        armorHolyDamageAbsorption = 0;

        armorRobustness = 0;
        armorFocus = 0;
        armorImmunity = 0;

        basePoiseDefense = 0;

        if (player.playerInventoryManager.headEquipment != null)
        {
            armorPhysicalDamageAbsorption += player.playerInventoryManager.headEquipment.physicalDamageAbsorption;
            armorMagicDamageAbsorption += player.playerInventoryManager.headEquipment.magicDamageAbsorption;
            armorFireDamageAbsorption += player.playerInventoryManager.headEquipment.fireDamageAbsorption;
            armorLightningDamageAbsorption += player.playerInventoryManager.headEquipment.lightningDamageAbsorption;
            armorHolyDamageAbsorption += player.playerInventoryManager.headEquipment.holyDamageAbsorption;

            armorRobustness += player.playerInventoryManager.headEquipment.robustness;
            armorFocus += player.playerInventoryManager.headEquipment.focus;
            armorImmunity += player.playerInventoryManager.headEquipment.immunity;

            basePoiseDefense += player.playerInventoryManager.headEquipment.poise;
        }

        if (player.playerInventoryManager.bodyEquipment != null)
        {
            armorPhysicalDamageAbsorption += player.playerInventoryManager.bodyEquipment.physicalDamageAbsorption;
            armorMagicDamageAbsorption += player.playerInventoryManager.bodyEquipment.magicDamageAbsorption;
            armorFireDamageAbsorption += player.playerInventoryManager.bodyEquipment.fireDamageAbsorption;
            armorLightningDamageAbsorption += player.playerInventoryManager.bodyEquipment.lightningDamageAbsorption;
            armorHolyDamageAbsorption += player.playerInventoryManager.bodyEquipment.holyDamageAbsorption;

            armorRobustness += player.playerInventoryManager.bodyEquipment.robustness;
            armorFocus += player.playerInventoryManager.bodyEquipment.focus;
            armorImmunity += player.playerInventoryManager.bodyEquipment.immunity;

            basePoiseDefense += player.playerInventoryManager.bodyEquipment.poise;
        }

        if (player.playerInventoryManager.legEquipment != null)
        {
            armorPhysicalDamageAbsorption += player.playerInventoryManager.legEquipment.physicalDamageAbsorption;
            armorMagicDamageAbsorption += player.playerInventoryManager.legEquipment.magicDamageAbsorption;
            armorFireDamageAbsorption += player.playerInventoryManager.legEquipment.fireDamageAbsorption;
            armorLightningDamageAbsorption += player.playerInventoryManager.legEquipment.lightningDamageAbsorption;
            armorHolyDamageAbsorption += player.playerInventoryManager.legEquipment.holyDamageAbsorption;

            armorRobustness += player.playerInventoryManager.legEquipment.robustness;
            armorFocus += player.playerInventoryManager.legEquipment.focus;
            armorImmunity += player.playerInventoryManager.legEquipment.immunity;

            basePoiseDefense += player.playerInventoryManager.legEquipment.poise;
        }

        if (player.playerInventoryManager.handEquipment != null)
        {
            armorPhysicalDamageAbsorption += player.playerInventoryManager.handEquipment.physicalDamageAbsorption;
            armorMagicDamageAbsorption += player.playerInventoryManager.handEquipment.magicDamageAbsorption;
            armorFireDamageAbsorption += player.playerInventoryManager.handEquipment.fireDamageAbsorption;
            armorLightningDamageAbsorption += player.playerInventoryManager.handEquipment.lightningDamageAbsorption;
            armorHolyDamageAbsorption += player.playerInventoryManager.handEquipment.holyDamageAbsorption;

            armorRobustness += player.playerInventoryManager.handEquipment.robustness;
            armorFocus += player.playerInventoryManager.handEquipment.focus;
            armorImmunity += player.playerInventoryManager.handEquipment.immunity;

            basePoiseDefense += player.playerInventoryManager.handEquipment.poise;
        }


    }

    public override void OnFocusChange(float lastValue, float newValue)
    {
        PlayerUIManager.instance.hudManager.SetNewFocusValue(lastValue, newValue);
    }

    public void AddSouls(int soulsToAdd)
    {
        souls += soulsToAdd;
        PlayerUIManager.instance.hudManager.SetSoulsCount(soulsToAdd);
    }

  
}
