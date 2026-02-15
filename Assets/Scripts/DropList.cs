using UnityEngine;

public class DropList : MonoBehaviour
{
    public ItemList availableLoot;
    private ItemDrop lootToAssign;
    void Awake()
    {
        lootToAssign = GetComponent<ItemDrop>();
        //Picking a random item from the item list
        lootToAssign.itemDrop = availableLoot.itemList[Random.Range(0, availableLoot.itemList.Length)];
    }
}
