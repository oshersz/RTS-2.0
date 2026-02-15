using UnityEngine;
using UnityEditor;

//code mostly generated from google search

[CustomEditor(typeof(Item))]
public class ItemEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Get the target script instance
        Item itemScript = (Item)target;

        // Update the serialized object to get the latest values
        serializedObject.Update();

        // Display the enum dropdown normally
        itemScript.itemType = (ItemType)EditorGUILayout.EnumPopup("Item Type", itemScript.itemType);
        itemScript.itemName = EditorGUILayout.TextField("Item Name", itemScript.itemName);
        

        // Display fields conditionally based on the enum value
        if (itemScript.itemType == ItemType.Junk)
        {
            //itemScript.owned = EditorGUILayout.Toggle("Owned", itemScript.owned);
            itemScript.value = EditorGUILayout.FloatField("Value", itemScript.value);
        }

        else if (itemScript.itemType == ItemType.Consumeable)
        {
            //itemScript.owned = EditorGUILayout.Toggle("Owned", itemScript.owned);
            itemScript.value = EditorGUILayout.FloatField("Value", itemScript.value);
        }
        else if (itemScript.itemType == ItemType.Equipment)
        {
            //itemScript.owned = EditorGUILayout.Toggle("Owned", itemScript.owned);
            itemScript.value = EditorGUILayout.FloatField("Value", itemScript.value);
            itemScript.equipped = EditorGUILayout.Toggle("Equipped", itemScript.equipped);
            itemScript.rarity = (LootDrop)EditorGUILayout.EnumPopup("Rarity", itemScript.rarity);
        }
        itemScript.itemSprite = (Sprite)EditorGUILayout.ObjectField("Item Image", itemScript.itemSprite, typeof(Sprite), false);

        EditorUtility.SetDirty(target); //makes the scriptable object save the info even after unity closes. IMPORTANT
        serializedObject.ApplyModifiedProperties();
        }
    }
