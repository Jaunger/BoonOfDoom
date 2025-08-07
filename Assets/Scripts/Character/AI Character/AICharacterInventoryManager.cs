using UnityEngine;

public class AICharacterInventoryManager : CharacterInventoryManager
{
    AICharacterManager aICharacter;

    [Header("Loot")]
    [SerializeField] public int dropItemChance = 10;
    [SerializeField] Item[] droppableItems;

    protected override void Awake()
    {
        base.Awake();
        aICharacter = GetComponent<AICharacterManager>();
    }

    public void DropItem()
    {
        bool willDropItem = false;
        
        int itemChanceRoll = Random.Range(0, 100);

        if (itemChanceRoll <= dropItemChance)
            willDropItem = true;

        if (!willDropItem)
            return;

        Item generatedItem = droppableItems[Random.Range(0, droppableItems.Length)];

        if (generatedItem == null)
            return;

        GameObject itemPickUpGameObject = WorldItemDatabase.instance.pickUpItemPrefab;
        PickUpItemInteractable pickUpIteractable = itemPickUpGameObject.GetComponent<PickUpItemInteractable>();

        pickUpIteractable = Instantiate(pickUpIteractable, transform.position, Quaternion.identity);

        pickUpIteractable.itemID = generatedItem.itemID;

    }
}
