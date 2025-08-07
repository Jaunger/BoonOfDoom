using TMPro;
using UnityEngine;

public class UI_Character_HP_Bar : UI_StatBar
{
    private CharacterManager character;
    private AICharacterManager aiCharacter;
    private PlayerManager player;

    [SerializeField] bool displayCharacterNameOnDamage = false;
    [SerializeField] float defaultTimeBeforeBarHides = 3;
    [SerializeField] float hideTimer = 0;
    public int currentDamageTaken = 0;
    [SerializeField] TextMeshProUGUI characterName;
    [SerializeField] TextMeshProUGUI characterDamage;
    [HideInInspector] public int oldHealthValue = 0;

    protected override void Awake()
    {
        base.Awake();

        character = GetComponentInParent<CharacterManager>();

        if (character != null)
        {
            aiCharacter = character as AICharacterManager;
            player = character as PlayerManager;
        }
    }
    protected override void Start()
    {
        base.Start();
        gameObject.SetActive(false);

    }

    public override void SetStat(int newValue)
    {
        if (displayCharacterNameOnDamage)
        {
            characterName.enabled = true;
            
            if (aiCharacter != null)
            {
                characterName.text = aiCharacter.name;
            }
            if (player != null)
            {
                characterName.text = player.name;
            }
        }

        slider.maxValue = character.characterStatManager.maxHealth;
        currentDamageTaken = Mathf.RoundToInt(currentDamageTaken + (oldHealthValue - newValue));

        if (currentDamageTaken < 0)
        {
            currentDamageTaken = Mathf.Abs(currentDamageTaken);
            characterDamage.text = "+ " + currentDamageTaken.ToString();
        }
        else
        {
            characterDamage.text = "- " + currentDamageTaken.ToString();
        }

        slider.value = newValue;

        if (character.characterStatManager.currentHealth != character.characterStatManager.maxHealth)
        {
            gameObject.SetActive(true);
            hideTimer = defaultTimeBeforeBarHides;
        }
    }

    private void Update()
    {
        transform.LookAt(transform.position + Camera.main.transform.forward);

        if (hideTimer > 0)
        {
            hideTimer -= Time.deltaTime;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        currentDamageTaken = 0;
    }
}
