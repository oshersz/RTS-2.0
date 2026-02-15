using UnityEngine;

[CreateAssetMenu(fileName = "Equipment", menuName = "Scriptable Objects/Equipment")]

public class Equipment : Item
{ 
    public Equipment()
    {
        this.itemType = ItemType.Equipment;
        stats = new float[13];
    }
    public EquipmentType equipmentType;

    public WeaponType weaponType;

    public WeaponRangeType weaponRangeType;

    public GameObject projectilePrefab;

    [NamedArray(typeof(Stats))]
    public float[] stats;

    /**
    public float health;
    public float mana;
    public float physDef;
    public float magDef;
    public float moveSpeed;
    public float dodgeChance;
    public float goldIncrease;
    public float expIncrease;
    public float lootChanceIncrease;
    public float spellDamage;
    */
}

public enum EquipmentType
{
    Weapon,
    Helmet,
    Chestplate,
    Boots,
    Ring1,
    Ring2,
    Amulet
}
