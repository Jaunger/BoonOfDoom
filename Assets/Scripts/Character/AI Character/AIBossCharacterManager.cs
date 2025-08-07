using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBossCharacterManager : AICharacterManager
{
    private const int GREAT_AXE_ID = 1000002;

    public event Action<bool, bool> OnBossFightIsActive;
    [SerializeField] public BrazierInteractable bossBrazier;

    public int bossID = 0;

    [Header("Music")]
    [SerializeField] AudioClip bossIntroTrack;
    [SerializeField] AudioClip bossBattleLoopTrack;

    [Header("Status")]
    [SerializeField] public bool hasBeenDefeated = false;
    [SerializeField] public bool hasBeenAwakened = false;
    [SerializeField] List<FogWall> fogWalls;
    [SerializeField] string bossSleepAnimation;
    [SerializeField] string awakeAnimation;

    [Header("States")]
    [SerializeField] public BossSleepState sleepState;

    [Header("Phase Shift")]
    [SerializeField] string phaseShiftAnimation = "Phase_Change_01";
    [SerializeField] CombatStanceState phase02CombatStanceState;
    public float hpNeededToPhaseShiftPercentage = 50;

    public override bool isDead
    {
        get { return _isDead; }
        set
        {
            _isDead = value;
        }
    }
    public bool bossFightIsActive
    {
        get { return _bossFightIsActive; }
        set
        {
            bool oldStatus = _bossFightIsActive;
            _bossFightIsActive = value;
            OnBossFightIsActive?.Invoke(oldStatus, _bossFightIsActive); // Fix: Correctly pass the new value (_bossFightIsActive) instead of 'value'
        }
    }
    [SerializeField] protected bool _bossFightIsActive = false;

    // when this ai is spawned check our save file (dictionary) for this ID
    // if the save file doesnt contain a boss monster with this ID add it
    // if it is present, check if the boss has been defeated
    // if it has been defeated, disable this game object
    // if it has not been defeated, enable this game object

    protected override void Awake()
    {
        base.Awake();

        sleepState = Instantiate(sleepState);
        currentState = sleepState;
    }

    protected override void Start()
    {
        base.Start();

        OnBossFightIsActive += OnBossFightIsActiveChanged;
        OnBossFightIsActiveChanged(false, bossFightIsActive);

        if (!WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.ContainsKey(bossID))
        {
            WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, false);
            WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Add(bossID, false);
        }
        else
        {
            hasBeenDefeated = WorldSaveGameManager.instance.currentCharacterData.bossesDefeated[bossID];
            hasBeenAwakened = WorldSaveGameManager.instance.currentCharacterData.bossesAwakened[bossID];
            sleepState.hasBeenAwakened = hasBeenAwakened;
        }

        StartCoroutine(InitializeBoss());

        if (!hasBeenAwakened)
        {
            animator.Play(bossSleepAnimation);
        }



    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        OnBossFightIsActive -= OnBossFightIsActiveChanged;

    }

    private IEnumerator InitializeBoss()
    {
        yield return StartCoroutine(GetFogWallsFromWOM());

        if (hasBeenAwakened)
        {
            for (int i = 0; i < fogWalls.Count; i++)
            {
                fogWalls[i].isActive = true;
            }
        }

        if (hasBeenDefeated)
        {
            for (int i = 0; i < fogWalls.Count; i++)
            {
                fogWalls[i].isActive = false;
            }

            isActive = false;
        }
    }

    private IEnumerator GetFogWallsFromWOM()
    {
        while (WorldObjectManager.instance.fogWalls.Count == 0)
            yield return new WaitForEndOfFrame();

        fogWalls = new List<FogWall>();

        foreach (var fogWall in WorldObjectManager.instance.fogWalls)
        {
            if (fogWall.fogWallID == bossID)
            {
                fogWalls.Add(fogWall);
                fogWall.bossId = bossID;
            }
            Debug.Log("Found fog wall with ID: " + fogWalls.Count);
        }
    }

    public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = default)
    {
        PlayerUIManager.instance.popUpManager.SendBossDefeatedPopUp("TERROR ANGUISHED");
        //characterStatManager.currentHealth = 0;
        isDead = true;
        bossFightIsActive = false;

        foreach (var fogWall in fogWalls)
        {
            fogWall.isActive = false;
        }

        if (!manuallySelectDeathAnimation)
        {
            characterAnimatorManager.PlayerTargetActionAnimation("Dead_01", true);
        }

        hasBeenDefeated = true;
        if (!WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.ContainsKey(bossID))
        {
            WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, true);
            WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Add(bossID, true);
        }
        else
        {

            WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Remove(bossID);
            WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Remove(bossID);
            WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, true);
            WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Add(bossID, true);
        }
        WorldSaveGameManager.instance.SaveGame();

        //  play sfx

        if (bossBrazier != null)
            bossBrazier.gameObject.SetActive(true);

        yield return new WaitForSeconds(5);

        PlayerManager player = FindFirstObjectByType<PlayerManager>();
        aiCharacterCombatManager.AwardSoulsOnDeath(player);

        if (!player.playerInventoryManager.IsWeaponUnlocked(GREAT_AXE_ID))
            aiCharacterInventoryManager.DropItem();
        else
        {
            WeaponItem axeInstance = null;

            if (player.playerInventoryManager.currentRightWeapon != null &&
                player.playerInventoryManager.currentRightWeapon.itemID == GREAT_AXE_ID)
            {
                axeInstance = player.playerInventoryManager.currentRightWeapon;
            }
            else
            {
                foreach (var weapon in player.playerInventoryManager.weaponsInRightHandSlots)
                {
                    if (weapon != null && weapon.itemID == GREAT_AXE_ID)
                    {
                        axeInstance = weapon;
                        break;
                    }
                }
            }
        }    


    }

    public void WakeBoss()
    {
        if (!hasBeenAwakened)
            characterAnimatorManager.PlayerTargetActionAnimation(awakeAnimation, true);

        bossFightIsActive = true;
        hasBeenAwakened = true;
        isAwake = true;
        currentState = idleState;

        if (!WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.ContainsKey(bossID))
        {
            WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, true);
        }
        else
        {
            WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Remove(bossID);
            WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, true);
        }

        for (int i = 0; fogWalls.Count > i; i++)
        {
            fogWalls[i].isActive = true;
            
        }
    }

    private void OnBossFightIsActiveChanged(bool oldStatus, bool newStatus)
    {
        if (bossFightIsActive)
        {
            WorldSoundFXManager.instance.PlayBossTrack(bossIntroTrack, bossBattleLoopTrack);

            GameObject bossHealthBar = Instantiate(PlayerUIManager.instance.hudManager.bossHealthBarObject,
                PlayerUIManager.instance.hudManager.bossHealthBarPosition);

            UI_Boss_HP_Bar bossHPBar = bossHealthBar.GetComponentInChildren<UI_Boss_HP_Bar>();
            bossHPBar.EnableBossHPBar(this);
            PlayerUIManager.instance.hudManager.currentBossHealthBar = bossHPBar;
        }
        else
        {
            WorldSoundFXManager.instance.StopBossMusic();
        }
    }

    protected void PhaseShift()
    {
        characterAnimatorManager.PlayerTargetActionAnimation(phaseShiftAnimation, true);
        combatStanceState = Instantiate(phase02CombatStanceState);
        currentState = combatStanceState;
    }

    public override void CheckHP(float lastValue, float newValue)
    { 
        base.CheckHP(lastValue, newValue);

        if (characterStatManager.currentHealth <= 0)
            return;

        float hpNeededToPhaseShift = characterStatManager.maxHealth * (hpNeededToPhaseShiftPercentage / 100);

        if (characterStatManager.currentHealth <= hpNeededToPhaseShift)
        {
            PhaseShift();
        }
    }

}
