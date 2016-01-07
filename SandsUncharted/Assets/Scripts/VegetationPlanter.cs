using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.Assertions;
using System.Collections;

public class VegetationPlanter : MonoBehaviour
{
    // Privates
    [SerializeField]
    private float stepSize = 1f;

    // Prefabs
    [SerializeField]
    private GameObject[] deadtrees;
    [SerializeField]
    private GameObject[] bushes;
    [SerializeField]
    private NoiseLayer[] deadtreeNoises;
    [SerializeField]
    private Gradient coloring;
    [SerializeField]
    [Range(0f,1f)]
    private float deadtreeThreshold = 0.8f;

    private MapGenerator mapgen;

    public NoiseLayer[] DeadtreeNoises { get { return deadtreeNoises; } }

    void Initialise()
    {
        mapgen = GetComponent<MapGenerator>();
        Assert.IsNotNull<MapGenerator>(mapgen);
    }

    public void GenerateVegetation()
    {
        // Delete old veggies if neccessary
        GameObject oldVeggieGO = GameObject.FindGameObjectWithTag(Tags.VEGGIE__TAG);
        if (oldVeggieGO != null) {
            DestroyImmediate(oldVeggieGO);
        }

        // Fetch some variables
        GameObject veggieGO = new GameObject("Veggie");
        Transform veggieT = veggieGO.transform;
        veggieGO.tag = Tags.VEGGIE__TAG;

        // Generate values
        float[,] values;
        FillNormalizedValues(out values, ref deadtreeNoises);

        // Iterate over the whole map using 
        for (int x = 0; x < mapgen.TotalWidth; ++x) {
            for (int y = 0; y < mapgen.TotalDepth; ++y) {

                // Check conditions for planting a tree
                if (values[x, y] > deadtreeThreshold) {
                    
                    // Randomly choose a tree and position it above
                    int treeIndex = Random.Range(0, deadtrees.Length);
                    Vector3 position = new Vector3(x, 300f, y);
                    position -= new Vector3(16f, 0, 16f);
                    GameObject tree = Instantiate(deadtrees[treeIndex], position, Quaternion.identity) as GameObject;
                    tree.transform.parent = veggieT;

                    // Perform a Raycast and position it down
                    // TODO TODO TODO
                }
            }
        }
    }

    public void VisualizeNoise(NoiseLayer[] noises)
    {
        Initialise();

        // Create a new texture
        Texture2D texture = new Texture2D(mapgen.TotalWidth, mapgen.TotalDepth, TextureFormat.RGB24, true);
        texture.name = "VisualizedNoise";
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.filterMode = FilterMode.Trilinear;
        texture.anisoLevel = 9;

        // Fill in the values (get filled within range of 0 and 1)
        float[,] values;
        FillNormalizedValues(out values, ref noises);
        
        // Apply the pixels
        for (int x = 0; x < values.GetLength(0); ++x) {
            for (int y = 0; y < values.GetLength(1); ++y) {
                texture.SetPixel(x, y, coloring.Evaluate(values[x, y]));
            }
        }
        texture.Apply();

        // Encode texture into PNG
        byte[] bytes = texture.EncodeToPNG();
        Object.DestroyImmediate(texture);

        // For testing purposes, also write to a file in the project folder
        string path = EditorUtility.SaveFilePanel("Save Visualizing Noise Texture", Application.dataPath, "noiseVisualization.png", "png");
        if (path.Length > 0)
            System.IO.File.WriteAllBytes(path, bytes);
    }

    void FillNormalizedValues(out float[,] values, ref NoiseLayer[] noises)
    {
        values = new float[mapgen.TotalWidth, mapgen.TotalWidth];

        float min = 999f;
        float max = float.MinValue;

        // iterate through all the positions
        for (int x = 0; x < values.GetLength(0); ++x) {
            for (int y = 0; y < values.GetLength(1); ++y) {
                // Retrieve value
                float value = NoiseLayer.getValueFromNoises(ref noises, new Vector3(x, y, 0));
                values[x, y] = value;

                // Check min and max condition
                if (value < min)
                    min = value;
                else if (value > max)
                    max = value;
            }
        }

        // prepare for normalizing
        float maxDiff = max - min;
        for (int x = 0; x < values.GetLength(0); ++x) {
            for (int y = 0; y < values.GetLength(1); ++y) {

                float value = values[x, y] - min;
                float percentage = value / maxDiff; // has to be a number between 0 and 1
                values[x, y] = percentage;
            }
        }

    }
}
