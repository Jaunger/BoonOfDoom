using UnityEngine;

public class TutorialDoorTrigger : MonoBehaviour
{
    [SerializeField] TutorialDoor door;
    [SerializeField] BrazierInteractable brazier;
    CharacterManager tutorialEnemy;
    [SerializeField] bool isCombatTutorial = false;
    private void Start()
    {
        tutorialEnemy = FindFirstObjectByType<CharacterManager>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isCombatTutorial)
            {
                if (tutorialEnemy != null && tutorialEnemy.isDead)
                {
                    door.Open();
                }
                else
                {
                    return;

                }
            }
            if (other.TryGetComponent<PlayerManager>(out var playerManager))
            {
                if (brazier != null)
                {
                    if (brazier.isActivated)
                        door.Open();
                }
                else if (playerManager.playerInventoryManager.hasFlask)
                {
                    if (brazier == null)
                        door.Open();
                }

            }
        }
    }
}
