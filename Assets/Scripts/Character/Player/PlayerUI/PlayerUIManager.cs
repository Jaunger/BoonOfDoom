using UnityEngine;

public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager instance;
    [HideInInspector] public PlayerManager player;

    [HideInInspector] public PlayerUIHudManager hudManager;
    [HideInInspector] public PlayerUIPopUpManager popUpManager;
    [HideInInspector] public PlayerUIMenuManager menuManager;
    [HideInInspector] public PlayerUIBonfireManager bonfireManager;
    [HideInInspector] public PlayerUITeleportLocationManager teleportLocationManager;
    [HideInInspector] public PlayerUILoadingScreenManager loadingScreenManager;
    [HideInInspector] public PlayerUISkillTreeManager playerUISkillTreeManager;

    [Header("UI Flags")]
    public bool menuIsOpen = false;
    public bool popUpIsOpen = false;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        hudManager = GetComponentInChildren<PlayerUIHudManager>();
        popUpManager = GetComponentInChildren<PlayerUIPopUpManager>();
        menuManager = GetComponentInChildren<PlayerUIMenuManager>();
        bonfireManager = GetComponentInChildren<PlayerUIBonfireManager>();
        teleportLocationManager = GetComponentInChildren<PlayerUITeleportLocationManager>();
        loadingScreenManager = GetComponentInChildren<PlayerUILoadingScreenManager>();
        playerUISkillTreeManager = GetComponentInChildren<PlayerUISkillTreeManager>();
        player = FindFirstObjectByType<PlayerManager>();
    }

    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    public void CloseAllMenuWindows()
    {
        menuManager.CloseCharacterMenu();
        bonfireManager.CloseBonfireMenu();
        teleportLocationManager.CloseTeleportMenu();
        playerUISkillTreeManager.CloseSkillTreeMenu();
    }
}
