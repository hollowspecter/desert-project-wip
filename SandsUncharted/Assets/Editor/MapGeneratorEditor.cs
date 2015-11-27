using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    private static GUIContent
        buildButton = new GUIContent("Save old Terrain and build new Chunks", "Build the meshes");

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MapGenerator mapgenScript = (MapGenerator)target;
        
        // Building
        if (GUILayout.Button(buildButton)) {
            mapgenScript.SaveAndDeleteTerrain();
            mapgenScript.GenerateMap();
        }
    }
}
