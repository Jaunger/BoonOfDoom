using UnityEngine;

public class BeaconDetector : MonoBehaviour
{
    public PlayerManager player;

    private void OnTriggerExit(Collider other)
    {
        if (player == null)
            player = GetComponentInParent<PlayerManager>();

        if (player == null)
            return;

        AICharacterManager aICharacter = other.GetComponent<AICharacterManager>();

        if (aICharacter != null)
            aICharacter.DeactivateCharacter(player);
    }
}
