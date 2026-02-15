using UnityEngine;
using UnityEditor;
using System.Linq;
[CustomPropertyDrawer(typeof(NamedArrayAttribute))]
public class NamedArrayDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, true);
    }
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(rect, label, property);
        try
        {
            var path = property.propertyPath;
            int pos = int.Parse(path.Split('[').LastOrDefault().TrimEnd(']'));
            EditorGUI.PropertyField(rect, property, new GUIContent(((NamedArrayAttribute)attribute).names[pos]), true);
        }
        catch
        {
            EditorGUI.PropertyField(rect, property, label, true);
        }
        EditorGUI.EndProperty();
    }
}
