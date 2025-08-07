using System.Collections;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldSaveGameManager : MonoBehaviour
{
    public static WorldSaveGameManager instance;

    public PlayerManager player;

    [Header("PlayerPrefab")]
    [SerializeField] GameObject playerPrefab;

    [Header("SAVE/LOAD")]
    [SerializeField] bool saveGame;
    [SerializeField] bool loadGame;

    [Header("World Scene Index")]
    [SerializeField] int worldSceneIndex = 1;

    [Header("Save Data Writer")]
    private SaveFileDataWriter saveFileDataWriter;

    [Header("Current Character Data")]
    public CharacterSlot currentCharacterSlot;
    public CharacterSaveData currentCharacterData;
    private string saveFileName;

    [Header("Character Slots")]
    public CharacterSaveData characterSlot01;
    public CharacterSaveData characterSlot02;
    public CharacterSaveData characterSlot03;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        LoadAllCharacterProfiles();
    }

    private void Update()
    {
        if(saveGame)
        {
            saveGame = false;
            SaveGame();
        }

        if(loadGame)
        {
            loadGame = false;
            LoadGame();   
        }
    }

    public string DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot slot)
    {
        string fileName = "";

        switch (slot)
        {
            case CharacterSlot.characterSlot_01:
                fileName = "characterSlot_01";
                break;
            case CharacterSlot.characterSlot_02:
                fileName = "characterSlot_02";
                break;
            case CharacterSlot.characterSlot_03:
                fileName = "characterSlot_03";
                break;
            default:
                break;
        }

        return fileName;
    }

    public void AttemptToCreateNewGame()
    {
        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.characterSlot_01);

        if (!saveFileDataWriter.CheckToSeeIfFileExists())
        {
            currentCharacterSlot = CharacterSlot.characterSlot_01;
            currentCharacterData = new CharacterSaveData();
            NewGame();
            return;
        }
        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.characterSlot_02);

        if (!saveFileDataWriter.CheckToSeeIfFileExists())
        {
            currentCharacterSlot = CharacterSlot.characterSlot_02;
            currentCharacterData = new CharacterSaveData();
            NewGame();
            return;
        }
        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.characterSlot_03);

        if (!saveFileDataWriter.CheckToSeeIfFileExists())
        {
            currentCharacterSlot = CharacterSlot.characterSlot_03;
            currentCharacterData = new CharacterSaveData();
            NewGame();
            return;
        }

        TitleScreenManager.instance.DisplayNoFreeSlots();


    }

    private void NewGame()
    {
        SaveGame();
        //StartCoroutine(LoadWorldScene());
        Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        LoadWorldScene(worldSceneIndex); 
    }

    public void LoadGame()
    {
        saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(currentCharacterSlot);

        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = saveFileName;
        currentCharacterData = saveFileDataWriter.LoadSaveFile();

        //player.LoadGame(ref currentCharacterData);  

        //StartCoroutine(LoadWorldScene());
        LoadWorldScene(worldSceneIndex);
    }

    public void SaveGame()
    {
        saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(currentCharacterSlot);

        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = saveFileName;

        player.SaveGame(ref currentCharacterData, true);


        saveFileDataWriter.CreateNewSaveFile(currentCharacterData);

    }

    public void DeleteGame(CharacterSlot cs)
    {
        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;
        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(cs);
        saveFileDataWriter.DeleteSaveFile();
    }

    private void LoadAllCharacterProfiles()
    {
        saveFileDataWriter = new SaveFileDataWriter();
        saveFileDataWriter.saveDataDirectoryPath = Application.persistentDataPath;

        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.characterSlot_01);
        characterSlot01 = saveFileDataWriter.LoadSaveFile();
        
        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.characterSlot_02);
        characterSlot02 = saveFileDataWriter.LoadSaveFile();  
        
        saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.characterSlot_03);
        characterSlot03 = saveFileDataWriter.LoadSaveFile();
    }

    public void LoadWorldScene(int buildIndex)
    {
        PlayerUIManager.instance.loadingScreenManager.ActivateLoadingScreen();

        string worldScene = SceneUtility.GetScenePathByBuildIndex(buildIndex);

        SceneManager.LoadScene(worldScene);

        if (WorldAIManager.instance != null)
            WorldAIManager.instance.ResetAllCharacters(); 

        player.LoadGame(ref currentCharacterData);
    }


    //public IEnumerator LoadWorldScene()
    //{
    //    AsyncOperation loadOperation = SceneManager.LoadSceneAsync(worldSceneIndex);

    //    //AsyncOperation loadOperation = SceneManager.LoadSceneAsync(currentCharacterData.sceneIndex);    
        
    //    player.LoadGame(ref currentCharacterData);

    //    yield return null;
    //}

    public int GetWorldSceneIndex()
    {
        return worldSceneIndex;
    }

    public SerializableWeapon GetSerializableWeapon(WeaponItem weapon)
    {
        SerializableWeapon sWeapon = new SerializableWeapon();
        sWeapon.itemID = weapon.itemID;
        sWeapon.CaptureRuntimeData(weapon);
        return sWeapon;
    }

    public SerializableFlask GetSerializableFlask(FlaskItem flask)
    {
        SerializableFlask sFlask = new SerializableFlask();
        sFlask.itemID = flask.itemID;
        sFlask.flaskHealAmount = flask.flaskHealAmount;

        return sFlask;
    }

    public void SaveAndReturnToMainMenu(int mainMenuSceneIndex = 0)
    {

        SaveGame();

        if (PlayerInputManager.instance)
        {
            PlayerInputManager.instance.player = null;
            PlayerInputManager.instance.enabled = false;
        }


        StartCoroutine(ReturnToMenuRoutine(mainMenuSceneIndex));
    }

    private IEnumerator ReturnToMenuRoutine(int buildIndex)
    {

        if (PlayerUIManager.instance && PlayerUIManager.instance.loadingScreenManager)
            PlayerUIManager.instance.loadingScreenManager.ActivateLoadingScreen();

        PlayerUIManager.instance.CloseAllMenuWindows();

        AsyncOperation op = SceneManager.LoadSceneAsync(buildIndex);
        PlayerInputManager.instance.player = PlayerUIManager.instance.player;
        while (!op.isDone)
            yield return null;
    }
}
