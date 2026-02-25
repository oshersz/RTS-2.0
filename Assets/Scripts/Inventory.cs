using UnityEngine;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory singleton { get; private set; }

    private Targeting characterTargeting;
    [SerializeField] ItemList ItemList;
    [SerializeField] GameObject ItemUIPrefab;
    [SerializeField] Transform canvasParent;
    [SerializeField] GameObject soldEffectPrefab;
    private Item[] inventory;
    private int inventoryIndex = 0;

    private LayerMask lootMask;

    //private Item[] equippedItems = new Item[7];
    private Equipment[] equippedItems = new Equipment[7];
    /*
    private Item equippedWeapon;
    private Item equippedHelmet;
    private Item equippedChestplate;
    private Item equippedBoots;
    private Item equippedRing1;
    private Item equippedRing2;
    private Item equippedAmulet;
    */

    // to implement -> equipped items are removed from inventory and are moved to a seperate place

    private void Awake()
    {
        singleton = this;
        characterTargeting = GetComponent<Targeting>();
    }

    void Start()
    {
        //characterTargeting = GetComponent<Targeting>();
        inventory = new Item[100];
        lootMask = LayerMask.GetMask("Loot");

        for (uint i = 0;i<ItemList.itemList.Length;i++)
        {
            ItemList.itemList[i].id = i;
            if (ItemList.itemList[i].owned)
            {
                inventory[inventoryIndex] = ItemList.itemList[i];
                /*
                GameObject inventoryItem = Instantiate(ItemUIPrefab, canvasParent);
                inventoryItem.transform.Find("ItemName").GetComponent<TextMeshProUGUI>().text = inventory[inventoryIndex].itemName;
                inventoryItem.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = ""+inventory[inventoryIndex].value;
                inventoryItem.transform.Find("Rarity").GetComponent<TextMeshProUGUI>().text = inventory[inventoryIndex].rarity.ToString();
                
                
                inventoryIndex++;
                */
                UpdateInventory();
            }
        }
    }

    void Update()
    {
        Loot();
    }

    private void Loot()
    {
        Collider[] lootHits;
        lootHits = Physics.OverlapSphere(transform.position, 1.5f, lootMask);

        if (lootHits.Length > 0)
        {
            for (int i = 0; i < lootHits.Length; i++)
            {
                //for coins

                Match match = Regex.Match(lootHits[i].transform.name, @"\d+"); //check if the loot's name contains numbers
                if (match.Success)
                {
                    CharacterStats.singleton.gold += (int)(int.Parse(match.Value) * (1+(CharacterStats.singleton.characterStats[(int)Stats.GoldIncrease] / 100)));
                    UIManager.singleton.characterGold.text = "" + CharacterStats.singleton.gold + "$";
                    Destroy(lootHits[i].gameObject);
                    return;
                }

                //for health or mana globes

                ItemDrop loot = lootHits[i].GetComponent<ItemDrop>();
                if (loot.itemDrop.itemType == ItemType.HealthPickup)
                {
                    float healAmount = CharacterStats.singleton.maxHealth * 0.25f;
                    if (CharacterStats.singleton.currentHealth + healAmount > CharacterStats.singleton.maxHealth)
                        CharacterStats.singleton.currentHealth = CharacterStats.singleton.maxHealth;
                    else
                        CharacterStats.singleton.currentHealth += healAmount;
                    Destroy(lootHits[i].gameObject);
                    return;
                }
                else if (loot.itemDrop.itemType == ItemType.ManaPickup)
                {
                    float manaAmount = CharacterStats.singleton.maxMana * 0.25f;
                    if (CharacterStats.singleton.currentMana+ manaAmount > CharacterStats.singleton.maxMana)
                        CharacterStats.singleton.currentMana = CharacterStats.singleton.maxMana;
                    else
                        CharacterStats.singleton.currentMana += manaAmount;
                    Destroy(lootHits[i].gameObject);
                    return;
                }
                if (canvasParent.transform.Find(loot.itemDrop.itemName)!=null) //selling off duplicate items so no need to handle inventory management
                {
                    CharacterStats.singleton.gold += (Mathf.Floor(0.7f * loot.itemDrop.value)); //getting 70% of the item's value
                    //Debug.Log("Made " + Mathf.Floor(0.7f * loot.itemDrop.value) + " gold off of selling " + loot.itemDrop.itemName);
                    UIManager.singleton.characterGold.text = "" + CharacterStats.singleton.gold + "$";
                    GameObject soldEffect = Instantiate(soldEffectPrefab, transform.position, transform.rotation);
                    soldEffect.GetComponent<SoldEffect>().soldText.text = loot.itemDrop.itemName + " Item Sold For " + Mathf.Floor(0.7f * loot.itemDrop.value) + " Gold!";
                    Destroy(lootHits[i].gameObject);
                    return;
                }

                //there must be a better way to do this with IDs
                //for other loot
                inventory[inventoryIndex] = loot.itemDrop;
                inventory[inventoryIndex].owned = true;
                UpdateInventory();
                Destroy(lootHits[i].gameObject);
            }
        }
    }

    private void UpdateInventory()
    {
        GameObject inventoryItem = Instantiate(ItemUIPrefab, canvasParent);
        inventoryItem.GetComponent<ItemDrop>().itemDrop = inventory[inventoryIndex];
        inventoryItem.name = inventory[inventoryIndex].itemName;
        inventoryItem.transform.Find("ItemImage").GetComponent<Image>().sprite = inventory[inventoryIndex].itemSprite;
        inventoryItem.transform.Find("ItemName").GetComponent<TextMeshProUGUI>().text = inventory[inventoryIndex].itemName;
        inventoryItem.transform.Find("Value").GetComponent<TextMeshProUGUI>().text = inventory[inventoryIndex].value.ToString() + "$";
        inventoryItem.transform.Find("Rarity").GetComponent<TextMeshProUGUI>().text = inventory[inventoryIndex].rarity.ToString();

        inventoryIndex++;
    }

    public void SellItem(Item itemToSell)
    {
        //what about the empty space in the inventory now? + inventory index
        for (int i =0;i<inventory.Length;i++)
        {
            if (inventory[i] == itemToSell)
            {
                itemToSell.owned = false;
                CharacterStats.singleton.gold += (Mathf.Floor(0.7f * itemToSell.value)); //getting 70% of the item's value
                Debug.Log("Made " + Mathf.Floor(0.7f * itemToSell.value) + " gold off of selling " + itemToSell.itemName);
                UIManager.singleton.characterGold.text = "" + CharacterStats.singleton.gold + "$";
                GameObject soldEffect = Instantiate(soldEffectPrefab, transform.position,transform.rotation);
                soldEffect.GetComponent<SoldEffect>().soldText.text = itemToSell.itemName + " Item Sold For " + Mathf.Floor(0.7f * itemToSell.value) + " Gold!";
                Destroy(canvasParent.transform.Find(itemToSell.itemName).gameObject);
            }
        }


    }

    public void EquipEquipmentItem(Equipment equipmentToEquip)
    {
        if (equippedItems[(int)equipmentToEquip.equipmentType]!= null) //if the equipment slot already contains an item
        {
            equippedItems[(int)equipmentToEquip.equipmentType].equipped = false;
            if (equippedItems[(int)equipmentToEquip.equipmentType].itemName != "Starting Weapon")
            {
                canvasParent.transform.Find(equippedItems[(int)equipmentToEquip.equipmentType].itemName).gameObject.SetActive(true);
            }
            
            // add visual change to unequipped item
        }
        equippedItems[(int)equipmentToEquip.equipmentType] = equipmentToEquip;
        equipmentToEquip.equipped = true;

        //finding the item in the visual inventory
        //consider optimizing
        //canvasParent.transform.Find(equipmentToEquip.itemName).Find("ItemImage").GetComponent<Image>().color = Color.magenta; //horredous lines honestly
        if (equipmentToEquip.itemName != "Starting Weapon") //need to find a better solution for starting equipment
        {
            UIManager.singleton.EquipItem(canvasParent.transform.Find(equipmentToEquip.itemName).Find("ItemImage").GetComponent<Image>().sprite, (int)equipmentToEquip.equipmentType);

            canvasParent.transform.Find(equipmentToEquip.itemName).gameObject.SetActive(false);

        }
        else
        {
            UIManager.singleton.EquipItem(equipmentToEquip.itemSprite, (int)equipmentToEquip.equipmentType);
        }
        if (equipmentToEquip.equipmentType == EquipmentType.Weapon)
        {
            characterTargeting.EquipNewWeapon(equipmentToEquip);
        }

        CharacterStats.singleton.UpdateStatsGainedFromEquipment(equippedItems);

        /**
        for (int i =0;i<inventoryIndex;i++)
        {
            if (inventory[i] == equipmentToEquip)
            {
                
            }
        }
        */
    }

    public void CompareEquipment(Equipment equipmentToCompare)
    {
        if (canvasParent.gameObject.activeSelf)
        {
            float[] comparisonResultArray = new float[13]; //same size as stats list
            //string[] visualResultArray = new string[13]; //easier to handle
            //saving the results of each stat comparison in an array for easy handling
            if (equippedItems[(int)equipmentToCompare.equipmentType] != null) //if the equipment slot already contains an item
            {
                for (int i = 0;i<13;i++)
                {
                    comparisonResultArray[i] = equipmentToCompare.stats[i] - equippedItems[(int)equipmentToCompare.equipmentType].stats[i];
                }
                
            }
            else
            {
                for (int i=0;i<13;i++)
                {
                    comparisonResultArray[i] = equipmentToCompare.stats[i];
                }
            }
            UIManager.singleton.CompareStats(comparisonResultArray);
            //UIManager.singleton.CompareStats(visualResultArray);
        }
    }

}
