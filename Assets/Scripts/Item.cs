using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    [HideInInspector] public uint id; //id can't go below 0, can it be auto generated?, I think i will assign it in the item list itself on Start
    public Sprite itemSprite;
    public ItemType itemType;
    public LootDrop rarity;
    public bool owned;
    public string itemName;
    public float value;
    public bool equipped;
}
public enum ItemType
{
    Junk,
    HealthPickup,
    ManaPickup,
    Consumeable,
    Quest,
    Equipment
}

public enum LootDrop
{
    Common,
    Rare,
    Scarce,
    Nonexistent,
    Health,
    Mana
}

/**
public enum ItemType
{
    Junk,
    HealthPickup,
    ManaPickup,
    Consumeable,
    Quest,
    Weapon,
    Helmet,
    Chestplate,
    Boots,
    Ring1,
    Ring2,
    Amulet
}
*/
