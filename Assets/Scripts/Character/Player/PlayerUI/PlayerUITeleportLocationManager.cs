using UnityEngine;
using UnityEngine.UI;

public class PlayerUITeleportLocationManager : MonoBehaviour
{
    PlayerManager player;

    [Header("Menu")]
    [SerializeField] GameObject menu;

    [SerializeField] GameObject[] teleportLocations;

    private void Awake()
    {

    }

    public void OpenTeleportMenu()
    {
        PlayerUIManager.instance.menuIsOpen = true;
        menu.SetActive(true);

        CheckForUnlockedTeleports();
    }

    public void CloseTeleportMenu()
    {
        PlayerUIManager.instance.menuIsOpen = false;
        menu.SetActive(false);
    }

    private void CheckForUnlockedTeleports()
    {
        bool hasFirstSelectedButton = false;
        for (int i = 0; i < teleportLocations.Length; i++)
        {
            for (int j = 0; j < WorldObjectManager.instance.braziers.Count; j++)
            {
                if (i == WorldObjectManager.instance.braziers[j].brazierID)
                {
                    if (WorldObjectManager.instance.braziers[j].isActivated)
                    {
                        teleportLocations[i].SetActive(true);
                        if (!hasFirstSelectedButton)
                        {
                            teleportLocations[i].GetComponent<Button>().Select();
                            teleportLocations[i].GetComponent<Button>().OnSelect(null);
                            hasFirstSelectedButton = true;
                        }
                    }
                    else
                    {
                        teleportLocations[i].SetActive(false);
                        Debug.Log("Teleport location " + WorldObjectManager.instance.braziers[j].gameObject.name + " is false.");

                    }
                }
            }
        }
    }

    public void TeleportToBrazier(int siteID)
    {
        for (int i = 0; i < WorldObjectManager.instance.braziers.Count; i++)
        {
            if (WorldObjectManager.instance.braziers[i].brazierID == siteID)
            {
                WorldObjectManager.instance.braziers[i].TeleportToSiteOfGrace();

                return;
            }
        }
    }

}
