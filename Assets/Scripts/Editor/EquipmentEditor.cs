using UnityEngine;
using UnityEditor;

//code mostly generated from google search

[CustomEditor(typeof(Equipment))]
public class EquipmentEditor : Editor
{
    private SerializedProperty statsArray;
    //private SerializedProperty name;

    public override void OnInspectorGUI()
    {
        statsArray = serializedObject.FindProperty("stats");
        //name = serializedObject.FindPropertyRelative("Name");

        Equipment equipmentScript = (Equipment)target;

        serializedObject.Update();
        //equipmentScript.itemType = (Item.ItemType)EditorGUILayout.EnumPopup("Item Type", equipmentScript.itemType);
        equipmentScript.rarity = (LootDrop)EditorGUILayout.EnumPopup("Rarity", equipmentScript.rarity);
        equipmentScript.equipmentType = (EquipmentType)EditorGUILayout.EnumPopup("Equipment Type", equipmentScript.equipmentType);
        equipmentScript.itemName = EditorGUILayout.TextField("Equipment Name", equipmentScript.itemName);
        equipmentScript.value = EditorGUILayout.FloatField("Value", equipmentScript.value);
        equipmentScript.equipped = EditorGUILayout.Toggle("Equipped", equipmentScript.equipped);

        if (equipmentScript.equipmentType == EquipmentType.Weapon)
        {
            equipmentScript.weaponType = (WeaponType)EditorGUILayout.EnumPopup("Weapon Type", equipmentScript.weaponType);
            equipmentScript.weaponRangeType = (WeaponRangeType)EditorGUILayout.EnumPopup("Weapon Range Type", equipmentScript.weaponRangeType);
        }
        //equipmentScript.stats = EditorGUILayout.PropertyField("Equipment Stats", equipmentScript.stats);

        //EditorGUILayout.PropertyField(statsArray,new GUIContent("hello","hello3"), true);

        EditorGUILayout.PropertyField(statsArray, true);
        /**
        equipmentScript.health = EditorGUILayout.FloatField("Health", equipmentScript.health);
        equipmentScript.mana = EditorGUILayout.FloatField("Mana", equipmentScript.mana);
        equipmentScript.physDef = EditorGUILayout.FloatField("Physical Defense", equipmentScript.physDef);
        equipmentScript.magDef = EditorGUILayout.FloatField("Magical Defense", equipmentScript.magDef);
        equipmentScript.moveSpeed = EditorGUILayout.FloatField("Move Speed", equipmentScript.moveSpeed);
        equipmentScript.dodgeChance = EditorGUILayout.FloatField("Dodge Chance", equipmentScript.dodgeChance);
        equipmentScript.goldIncrease = EditorGUILayout.FloatField("Gold Increase %", equipmentScript.goldIncrease);
        equipmentScript.expIncrease = EditorGUILayout.FloatField("Exp Increase %", equipmentScript.expIncrease);
        equipmentScript.lootChanceIncrease = EditorGUILayout.FloatField("Rare Loot Chance %", equipmentScript.lootChanceIncrease);
        equipmentScript.spellDamage = EditorGUILayout.FloatField("Spell Damage", equipmentScript.spellDamage);
        */

        if (equipmentScript.equipmentType == EquipmentType.Weapon)
        {
            equipmentScript.projectilePrefab = (GameObject)EditorGUILayout.ObjectField("Projectile Prefab", equipmentScript.projectilePrefab, typeof(GameObject), false); //the boolean is for allowing (or not allowing) to put scene gameobjects
        }
        equipmentScript.itemSprite = (Sprite)EditorGUILayout.ObjectField("Item Image", equipmentScript.itemSprite, typeof(Sprite), false);
        
        EditorUtility.SetDirty(target); //makes the scriptable object save the info even after unity closes. IMPORTANT
        serializedObject.ApplyModifiedProperties();
    }
}
