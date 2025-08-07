using UnityEngine;
using UnityEngine.TextCore.Text;

public class AICharacterSpawner : MonoBehaviour
{
    [Header("Character")]
    [SerializeField] GameObject characterGameObject;
    [SerializeField] public GameObject instantiateGameObject;
    [SerializeField] BrazierInteractable brazier;
    private AICharacterManager aICharacter;

    [Header("Patrol")]
    [SerializeField] bool hasPartolPath = false;
    [SerializeField] bool isBoss = false;
    [SerializeField] int patrolPathID = 0;

    [Header("Sleep")]
    [SerializeField] bool isSleeping = false;

    private void Awake()
    {

    }

    private void Start()
    {
        WorldAIManager.instance.SpawnCharacter(this);
        gameObject.SetActive(false);
    }



    public void AttemptToSpawnCharacter()
    {
        if (characterGameObject != null)
        {
            instantiateGameObject = Instantiate(characterGameObject, transform.position,transform.rotation);
            aICharacter = instantiateGameObject.GetComponent<AICharacterManager>();

            if (aICharacter == null)
                return;


            WorldAIManager.instance.AddCharacterToSpawnedCharacterList(aICharacter);

            if (isBoss)
            {
                AIBossCharacterManager boss = aICharacter as AIBossCharacterManager;
                boss.bossBrazier = brazier;
            }

            if (hasPartolPath)
                aICharacter.idleState.aIPatrolPath = WorldAIManager.instance.GetAIPatrolPath(patrolPathID);

            if (isSleeping)
                aICharacter.isAwake = false;

            if (hasPartolPath)
                aICharacter.idleState.idleStateMode = IdleStateMode.Patrol;

            if (isSleeping)
                aICharacter.idleState.idleStateMode = IdleStateMode.Sleep;

            aICharacter.CreateActivationBeacon();

            aICharacter.isActive = false;
        }
    }

    public void ResetCharacter()
    {
        if (instantiateGameObject == null)
            return;

        if (aICharacter == null)
            return;

        instantiateGameObject.transform.position = transform.position;
        instantiateGameObject.transform.rotation = transform.rotation;
        aICharacter.characterStatManager.currentHealth = aICharacter.characterStatManager.maxHealth;
        aICharacter.aiCharacterCombatManager.SetTarget(null);
        aICharacter.navMeshAgent.enabled = false;

        if (aICharacter.isDead)
        {
            aICharacter.isDead = false;
            aICharacter.characterAnimatorManager.PlayerTargetActionAnimation("Empty", false, false, true, true, true, true);
            aICharacter.currentState.SwitchState(aICharacter,aICharacter.idleState);
        }


        aICharacter.characterUIManager.ResetCharacterHPBar();

        if (aICharacter is AIBossCharacterManager)
        {
            AIBossCharacterManager boss = aICharacter as AIBossCharacterManager;
            boss.isAwake = false;
            boss.sleepState.hasBeenAwakened = boss.hasBeenAwakened;
            boss.currentState = boss.currentState.SwitchState(boss, boss.sleepState);
        }
    }
}
