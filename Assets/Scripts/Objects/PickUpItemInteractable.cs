using System;
using UnityEngine;

public class PickUpItemInteractable : Interactable
{
    // 1. World items: spawn only once, and have unique i.d which have their loot status saved to your character data when looted so they do not respawn
    // 2. monster drops: these spawn (sometimes) when monster die, they have no unique save data

    public ItemPickupType itemPickupType;

    [Header("Item")]
    [SerializeField] Item item;

    [Header("Create Loot Pick Up")]
    public int _itemID = 0;
    public int itemID
    {
        get { return _itemID; }
        set
        {
            _itemID = value;

            if (itemPickupType != ItemPickupType.CharacterDrop)
                return;

            Debug.Log("Item ID: " + value);

            item = WorldItemDatabase.instance.GetItemByID(_itemID);
          
        }

    }
    //public bool trackDroppingCreaturePosition = true;
    //public ulong droppingCreatureID = 0;

    [Header("World Spawn Pick Up")]
    [SerializeField] int worldSpawnInteractableID; //unique ID so u dont loot item more than once
    [SerializeField] bool hasBeenLooted = false;

    [Header("Drop SFX")]
    [SerializeField] AudioClip dropSFX;
    private AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();

        audioSource = GetComponent<AudioSource>();
    }

    protected override void Start()
    {
        base.Start();

        if (itemPickupType == ItemPickupType.WorldSpawn)
            CheckIfWorldItemWasAlreadyLooted();

        //if (itemPickupType == ItemPickupType.CharacterDrop) //TODO: maybe need to move 
        //    audioSource.PlayOneShot(dropSFX);
    }

    private void CheckIfWorldItemWasAlreadyLooted()
    {
        if (!WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted.ContainsKey(worldSpawnInteractableID))
            WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted.Add(worldSpawnInteractableID, hasBeenLooted);

        hasBeenLooted = WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted[worldSpawnInteractableID];

        if (hasBeenLooted)
            gameObject.SetActive(false);
    }

    public override void Interact(PlayerManager player)
    {
        if (player.isPerformingAction)
            return;

        base.Interact(player);

        //player.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.pickUpItemsSFX);

        player.playerAnimatorManager.PlayerTargetActionAnimation("Pick_Up_Item_01", true);

        player.playerInventoryManager.AddItemToInventory(Instantiate(item));

        PlayerUIManager.instance.popUpManager.SendItemPopUp(item, 0);

        if (itemPickupType == ItemPickupType.WorldSpawn)
        {
            if (WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted.ContainsKey(worldSpawnInteractableID))
            {
                WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted.Remove(worldSpawnInteractableID);
            }
            WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted.Add(worldSpawnInteractableID, true);
        }

        Destroy(gameObject);
    }
}