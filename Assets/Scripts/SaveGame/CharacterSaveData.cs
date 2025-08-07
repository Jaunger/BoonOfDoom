using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;


[System.Serializable]
public class CharacterSaveData //   TODO: change accordingly to needs
{
    [Header("Scene Index")]
    public int sceneIndex = 1;

    [Header("Character Name")]
    public string characterName;

    [Header("Death Spot")]
    public bool hasDeathSpot = false;
    public float deathSpotXPos;
    public float deathSpotYPos;
    public float deathSpotZPos;
    public int deathSpotSoulsCount = 0;

    [Header("Time Played")]
    public float timePlayed;

    [Header("World Coordinates")]
    public float xPos;
    public float yPos;
    public float zPos;

    [Header("Resources")]
    public float currentHealth;
    public float currentStamina;
    public int currentSouls;
    public float currentFocus;


    [Header("Stats")]
    public int vitality;
    public int endurance;

    [Header("Brazier")]
    public int lastBrazierRestedAt = 0;
    public SerializableDictionary<int, bool> braziers; 

    [Header("Bosses")]
    public SerializableDictionary<int, bool> bossesDefeated;
    public SerializableDictionary<int, bool> bossesAwakened;

    [Header("World Items")]
    public SerializableDictionary<int, bool> worldItemsLooted;

    [Header("Unlocked Global Abilities")]
    public bool flameUnlocked;
    public bool smashUnlocked;
    public bool throwUnlocked;

    [Header("Equipment")]
    //public int headEquipmentID;
    //public int bodyEquipmentID;
    //public int legEquipmentID;
    //public int handEquipmentID;

    [Header("Tutorial")]
    public bool inTutorial = false;         
    public bool tutorialCompleted = false;  
    public string tutorialCheckpoint = "";

    [Header("Weapons")]
    public int rightWeaponIndex;
    public SerializableWeapon rightWeapon01; 
    public SerializableWeapon rightWeapon02;
    public SerializableWeapon rightWeapon03;

    public int quickSlotIndex;
    public SerializableFlask quickSlot01;

    public int currentFlasksRemaining = 3;

    [Header("Inventory")]
    public List<SerializableWeapon> weaponsInventory;
    //public List<SerialiableQuickSlotItem> quickSlotItemsInInventory;
    //public List<int> headEquipmentInInventory;
    //public List<int> bodyEquipmentInInventory;
    //public List<int> legEquipmentInInventory;
    //public List<int> handEquipmentInInventory;
    public List<SerializableFlask> flaskInInventory; 
    public List<int> unlockedWeaponIDs;
    public CharacterSaveData()
    {
        braziers = new SerializableDictionary<int, bool>();
        bossesDefeated = new SerializableDictionary<int, bool>();
        bossesAwakened = new SerializableDictionary<int, bool>();
        worldItemsLooted = new SerializableDictionary<int, bool>();
        weaponsInventory = new List<SerializableWeapon>();
        //quickSlotItemsInInventory = new List<SerialiableQuickSlotItem>();
        //headEquipmentInInventory = new List<int>();
        //bodyEquipmentInInventory = new List<int>();
        //legEquipmentInInventory = new List<int>();
        //handEquipmentInInventory = new List<int>();
        unlockedWeaponIDs = new List<int>();
        flaskInInventory = new List<SerializableFlask>();
    }

}
