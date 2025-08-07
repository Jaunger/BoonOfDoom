using UnityEngine;
using UnityEngine.UI;

public class PlayerUIBonfireManager : MonoBehaviour
{
    [Header("Menu")]
    [SerializeField] GameObject menu;
    [SerializeField] Button bonfireButton;

    public void OpenBonfireMenu()
    {
        PlayerUIManager.instance.menuIsOpen = true;
        bonfireButton.Select();
        menu.SetActive(true);

    }

    public void CloseBonfireMenu()
    {
        PlayerUIManager.instance.menuIsOpen = false;
        menu.SetActive(false);
    }

    public void OpenTeleportLocationMenu()
    {
        CloseBonfireMenu();
        PlayerUIManager.instance.teleportLocationManager.OpenTeleportMenu();
    }
}
