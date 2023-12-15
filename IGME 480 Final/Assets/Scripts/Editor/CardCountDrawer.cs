using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(CardCount))]
public class CardCountDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position.height = EditorGUIUtility.singleLineHeight;
        position.width *= 0.5f;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("card"), GUIContent.none);
        position.x += position.width;
        EditorGUI.PropertyField(position, property.FindPropertyRelative("count"), GUIContent.none);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUIUtility.singleLineHeight;
    }
}
