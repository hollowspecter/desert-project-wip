using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(BiomeGenerator))]
public class BiomeGeneratorEditor : Editor
{
    private static GUIContent
        buildButton = new GUIContent("Create Biome Map", "Creates a biome map using the different noises on this component.");
    private static GUIContent
        randomizeButton = new GUIContent("Randomize Noiseattributes", "Randomizes the offsetpositions and rotations of the noises");

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BiomeGenerator biomegenScript = (BiomeGenerator)target;

        // Creating
        if (GUILayout.Button(buildButton)) {
            biomegenScript.GenerateBiomeMap();
        }

        // Randomizing the Layers
        if (GUILayout.Button(randomizeButton)) {
            biomegenScript.RandomiseNoiseLayers();
        }
    }
}