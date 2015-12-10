using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(LUTGenerator))]
public class LUTGeneratorEditor : Editor
{
    private static GUIContent
        buildButton = new GUIContent("Create LUT", "Creates a LookUp Texture Table with your defined rules");

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        LUTGenerator lutgenScript = (LUTGenerator)target;

        // Creating
        if (GUILayout.Button(buildButton)){
            lutgenScript.FillTexture();
        }
    }
}
