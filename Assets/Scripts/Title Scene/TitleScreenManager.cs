using UnityEngine;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    public static TitleScreenManager instance;  

    [Header("Menu Objects")]
    [SerializeField] GameObject titleScreenMainMenu;
    [SerializeField] GameObject titleScreenLoadMenu;

    [Header("Buttons")]
    [SerializeField] Button loadMenuReturnMenu;
    [SerializeField] Button mainMenuLoadGameButton;
    [SerializeField] Button mainMenuButton;
    [SerializeField] Button deleteCharacterPopUpConfirmaBtn;
    [SerializeField] Button noCharacterSlotsPopUpBtn;


    [Header("Popups")]
    [SerializeField] GameObject noCharacterSlotsPopUp;
    [SerializeField] public GameObject deleteCharacterSlotPopUp;

    [Header("Character slots")]
    public CharacterSlot currentCharacterSlot = CharacterSlot.NO_SLOT;


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
    }

    public void StartNewGame()
    {
        WorldSaveGameManager.instance.AttemptToCreateNewGame();
    }

    public void OpenLoadGameMenu()
    {
        titleScreenMainMenu.SetActive(false);
        titleScreenLoadMenu.SetActive(true);

        loadMenuReturnMenu.Select();
    }

    public void CloseLoadGameMenu()
    {
        titleScreenLoadMenu.SetActive(false);
        titleScreenMainMenu.SetActive(true);

        mainMenuLoadGameButton.Select();
    }

    public void DisplayNoFreeSlots()
    {
        noCharacterSlotsPopUp.SetActive(true);
        noCharacterSlotsPopUpBtn.Select();  
    }

    public void CloseNoFreeSlots()
    {
        noCharacterSlotsPopUp.SetActive(false);
        mainMenuButton.Select();    

    }

    public void SelecteCharacterSlot(CharacterSlot slot)
    {
        currentCharacterSlot = slot;
    }

    public void SelectNoSlot()
    {
        currentCharacterSlot = CharacterSlot.NO_SLOT;
    }

    public void AttemptToDeleteCharacterSlot()
    {
        if (currentCharacterSlot != CharacterSlot.NO_SLOT)
        {
            deleteCharacterSlotPopUp.SetActive(true);
            deleteCharacterPopUpConfirmaBtn.Select();
        }
    }

    public void DeleteCharacterSlot()
    {
        deleteCharacterSlotPopUp.SetActive(false);
        WorldSaveGameManager.instance.DeleteGame(currentCharacterSlot);
        titleScreenLoadMenu.SetActive(false);
        titleScreenLoadMenu.SetActive(true);    
        loadMenuReturnMenu.Select();
    }

    public void CloseDeleteCharacterPopUp()
    {
        deleteCharacterSlotPopUp.SetActive(false);
        loadMenuReturnMenu.Select();
    }
}
