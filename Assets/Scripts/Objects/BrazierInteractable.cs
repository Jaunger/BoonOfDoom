using System.Collections;
using UnityEngine;

public class BrazierInteractable : Interactable
{
    [Header("Brazier Info")]
    public int brazierID; 
    public bool isActivated = false;

    [Header("Brazier Particles")]
    [SerializeField] GameObject activatedParticles;

    [Tooltip("ID of the boss that must be defeated before this brazier appears")]
    [SerializeField] int requiredBossID;           

    [Header("Interaction Text")]
    [SerializeField] string unactivatedText = "Restore the brazier";
    [SerializeField] string activatedText = "Rest";

    [Header("Teleport Transform")]
    [SerializeField] Transform teleportTransform;

    protected override void Start()
    {
        base.Start();

        if (WorldSaveGameManager.instance.currentCharacterData.braziers.ContainsKey(brazierID))
        {
            isActivated = WorldSaveGameManager.instance.currentCharacterData.braziers[brazierID];
        }
        else
            isActivated = false;

        if (isActivated)
        {
            activatedParticles.SetActive(true);
            interactText = activatedText;
        }
        else
        {
            activatedParticles.SetActive(false);
            interactText = unactivatedText;
        }

        WorldObjectManager.instance.AddBrazierToList(this);

        if (requiredBossID >= 0)
        {
            checkForDefeat();
        }
    }

    private void checkForDefeat()
    {
        bool bossIsDefeated =
            WorldSaveGameManager.instance.currentCharacterData.bossesDefeated
            .TryGetValue(requiredBossID, out bool defeated) && defeated;

        if (bossIsDefeated)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }


    private void RestoreBrazier(PlayerManager player)
    {
        isActivated = true;

        if (WorldSaveGameManager.instance.currentCharacterData.braziers.ContainsKey(brazierID))
            WorldSaveGameManager.instance.currentCharacterData.braziers.Remove(brazierID);

        WorldSaveGameManager.instance.currentCharacterData.braziers.Add(brazierID, true);
        
        activatedParticles.SetActive(true);

        player.playerAnimatorManager.PlayerTargetActionAnimation("Activate_Brazier_01", true);

        PlayerUIManager.instance.popUpManager.SendBrazierPopUp("BRAZIER RESTORED");

        interactText = activatedText;

        StartCoroutine(WaitForAnimationAndPopUpThenRestoreCollider());
    }

    private void RestAtBrazier(PlayerManager player)
    {
        PlayerUIManager.instance.bonfireManager.OpenBonfireMenu();
        Debug.Log("Resting at brazier");
        interactCollider.enabled = true; // temp -> should enable after ui close
        player.playerStatManager.currentHealth = player.playerStatManager.maxHealth;
        player.playerStatManager.SetStamina(player.playerStatManager.maxStamina);

        player.playerInventoryManager.remainingHealthFlasks = 3;
        
        // TODO: update/force move quest characters
        // reset monsters/character locations
        WorldAIManager.instance.ResetAllCharacters();
        WorldSaveGameManager.instance.SaveGame();

    }

    public override void Interact(PlayerManager player)
    {
        base.Interact(player);

        if (player.isPerformingAction)
            return;

        if (player.playerCombatManager.isUsingItem)
            return;

        WorldSaveGameManager.instance.currentCharacterData.lastBrazierRestedAt = brazierID;

        if (isActivated)
        {
            RestAtBrazier(player);
        }
        else
        {
            RestoreBrazier(player);
        }

    }

    private IEnumerator WaitForAnimationAndPopUpThenRestoreCollider()
    {
        yield return new WaitForSeconds(2);
        interactCollider.enabled = true;
    }

    public void TeleportToSiteOfGrace()
    {
        Debug.Log("trying to teleport" + teleportTransform);
        // Find the player in the scene
        PlayerManager player = Object.FindFirstObjectByType<PlayerManager>();
        if (player == null)
        {
            Debug.LogError("PlayerManager not found in the scene.");
            return;
        }

        PlayerUIManager.instance.loadingScreenManager.ActivateLoadingScreen();

        // Teleport player
        player.transform.position = teleportTransform.position;

        PlayerUIManager.instance.loadingScreenManager.DeactivateLoadingScreen();

        WorldSaveGameManager.instance.SaveGame();

    }



}
