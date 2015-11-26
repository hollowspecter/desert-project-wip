using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;

[CustomPropertyDrawer(typeof(NoiseLayer))]
public class NoiseLayerDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        label = EditorGUI.BeginProperty(position, label, property);
        Rect contentPosition = EditorGUI.PrefixLabel(position, label, EditorStyles.boldLabel);

        EditorGUI.indentLevel = 0;

        // LayerName
        contentPosition.height = 16f;
        contentPosition.width *= 0.75f;
        EditorGUIUtility.labelWidth = 36f;
        EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("LayerName"), GUIContent.none);

        // Type
        contentPosition.x += contentPosition.width;
        contentPosition.width /= 3f;
        EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("type"), GUIContent.none);

        // Next Line
        contentPosition.y += 18f;
        EditorGUI.indentLevel += 1;
        contentPosition = EditorGUI.IndentedRect(position);

        // Frequency
        contentPosition.height = 16f;
        contentPosition.y += 18f;
        contentPosition.width *= 1f/2f;
        EditorGUIUtility.labelWidth = 50f;
        GUIContent content = new GUIContent("Freq", "Tooltip");
        EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("frequency"), content);

        // Amplitude
        contentPosition.x += contentPosition.width;
        EditorGUIUtility.labelWidth = 50f;
        content = new GUIContent("Amp", "Tooltip");
        EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("amplitude"), content);


        // Next Line
        contentPosition.y += 18f;

        // Dimension
        contentPosition.height = 16f;
        contentPosition.x -= contentPosition.width;
        EditorGUIUtility.labelWidth = 50f;
        content = new GUIContent("Dim", "Tooltip");
        EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("dimension"), content);

        // Octave
        contentPosition.x += contentPosition.width;
        EditorGUIUtility.labelWidth = 50f;
        content = new GUIContent("Oct", "Tooltip");
        EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("octaves"), content);

        // Next Line
        contentPosition.y += 18f;

        // Lacunarity
        contentPosition.height = 16f;
        contentPosition.x -= contentPosition.width;
        EditorGUIUtility.labelWidth = 50f;
        content = new GUIContent("Lac", "Tooltip");
        EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("lacunarity"), content);

        // Persistence
        contentPosition.x += contentPosition.width;
        EditorGUIUtility.labelWidth = 50f;
        content = new GUIContent("Per", "Tooltip");
        EditorGUI.PropertyField(contentPosition, property.FindPropertyRelative("persistence"), content);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int lineCount = 4;
        return 16f + 18f*(lineCount - 1);
    }
}
