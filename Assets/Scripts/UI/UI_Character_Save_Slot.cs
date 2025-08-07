using TMPro;
using UnityEngine;

public class UI_Character_Save_Slot : MonoBehaviour
{
    SaveFileDataWriter writer;

    [Header("Game Slot")]
    public CharacterSlot slot;

    [Header("Character Info")]
    public TextMeshProUGUI characterName;
    public TextMeshProUGUI timePlayed;

    private void OnEnable()
    {
        LoadSaveSlots();
    }

    private void LoadSaveSlots()
    {
        writer = new SaveFileDataWriter();
        writer.saveDataDirectoryPath = Application.persistentDataPath;

        if(slot == CharacterSlot.characterSlot_01)
        {
            writer.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(slot);

            if (writer.CheckToSeeIfFileExists())
            {
                characterName.text = WorldSaveGameManager.instance.characterSlot01.characterName;
            }
            else
            {
                gameObject.SetActive(false);    
            }
        }

        else if (slot == CharacterSlot.characterSlot_02)
        {
            writer.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(slot);

            if (writer.CheckToSeeIfFileExists())
            {
                characterName.text = WorldSaveGameManager.instance.characterSlot02.characterName;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        else if (slot == CharacterSlot.characterSlot_03)
        {
            writer.saveFileName = WorldSaveGameManager.instance.DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(slot);

            if (writer.CheckToSeeIfFileExists())
            {
                characterName.text = WorldSaveGameManager.instance.characterSlot03.characterName;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }

    public void LoadGameFromCharacterSlot()
    {
        WorldSaveGameManager.instance.currentCharacterSlot = slot;
        WorldSaveGameManager.instance.LoadGame();
    }

    public void SelectCurrentSlot()
    {
        TitleScreenManager.instance.SelecteCharacterSlot(slot);
    }
}
