using UnityEngine;

public class LoadMenuInputManager : MonoBehaviour
{
    PlayerControls playerControls;

    [Header("Title Screen Inputs")]
    [SerializeField] bool deleteCharacterSlot = false;

    private void Update()
    {
        if (deleteCharacterSlot)
        {
            TitleScreenManager.instance.AttemptToDeleteCharacterSlot();
        }
        deleteCharacterSlot = false;
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();
            playerControls.UI.LeftBtn.performed += i => deleteCharacterSlot = true;
        }

        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
        deleteCharacterSlot = false;
    }
}
