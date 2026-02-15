using UnityEngine;

[CreateAssetMenu(fileName = "ItemList", menuName = "Scriptable Objects/ItemList")]
public class ItemList : ScriptableObject
{
    public Item[] itemList;
}
