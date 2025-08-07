using System.Collections;
using UnityEngine;

public class PlayerUIMenuManager : MonoBehaviour
{
    [Header("Menu")]
    [SerializeField] GameObject menu;

    public void OpenCharacterMenu()
    {
        PlayerUIManager.instance.menuIsOpen = true;
        menu.SetActive(true);
    }

    public void CloseCharacterMenu()
    {
        PlayerUIManager.instance.menuIsOpen = false;
        menu.SetActive(false);
    }

    public void CloseCharacterMenuAfterATime()
    {
        StartCoroutine(WaitThenCloseMenu());
    }

    private IEnumerator WaitThenCloseMenu()
    {
        yield return new WaitForFixedUpdate();
        PlayerUIManager.instance.menuIsOpen = false;
        menu.SetActive(false);
    }
}
