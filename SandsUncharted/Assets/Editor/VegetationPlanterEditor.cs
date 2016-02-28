using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(VegetationPlanter))]
public class VegetationPlanterEditor : Editor
{
    private static GUIContent
        visualizeButton = new GUIContent("Visualize Deadtree Noise");
    private static GUIContent
        veggiButton = new GUIContent("Generate Vegetation");

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        VegetationPlanter vegScript = (VegetationPlanter)target;

        // Creating
        if (GUILayout.Button(visualizeButton)) {
            vegScript.VisualizeNoise(vegScript.Noises);
            AssetDatabase.Refresh();
        }

        if (GUILayout.Button(veggiButton)) {
            vegScript.GenerateVegetation();
        }
    }
}
