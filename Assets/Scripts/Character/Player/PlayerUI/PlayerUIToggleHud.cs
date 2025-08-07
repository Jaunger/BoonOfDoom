using UnityEngine;

public class PlayerUIToggleHud : MonoBehaviour
{
    private void OnEnable()
    {
        PlayerUIManager.instance.hudManager.ToggleHUD(false);
    }

    private void OnDisable()
    {
        PlayerUIManager.instance.hudManager.ToggleHUD(true);
    }
}
