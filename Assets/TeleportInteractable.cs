using System.Collections;
using UnityEngine;

public class TeleportInteractable : Interactable
{
    [Header("Teleport")]
    [SerializeField] private Transform teleportTransform; 

    public override void Interact(PlayerManager player)
    {
        base.Interact(player);
        StartCoroutine(TeleportRoutine(player));
    }

    private IEnumerator TeleportRoutine(PlayerManager player)
    {
        if (teleportTransform == null)
        {
            yield break;
        }

        PlayerUIManager.instance.loadingScreenManager.ActivateLoadingScreen();
        yield return null; 

        player.transform.SetPositionAndRotation(
            teleportTransform.position,
            teleportTransform.rotation);

        // Optional: write tutorial flags here (hasStartedTutorial, checkpoint = "A")

        yield return null; 

        PlayerUIManager.instance.loadingScreenManager.DeactivateLoadingScreen();

        WorldSaveGameManager.instance.SaveGame();
    }
}
