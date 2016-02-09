using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

public class VegetationPlanter : MonoBehaviour
{
    // Prefabs
    [SerializeField]
    private GameObject[] deadtrees;
    [SerializeField]
    private GameObject[] bushes;

    // Privates
    [SerializeField]
    private NoiseLayer[] deadtreeNoises;
    [SerializeField]
    private Gradient coloring;
    [SerializeField]
    [Range(0f,1f)]
    private float deadtreeThreshold = 0.8f;
    [SerializeField]
    private LayerMask terrainMask;
    [SerializeField]
    [Range(0f,3f)]
    private float sinkInOffset = 0.1f;
    [SerializeField]
    private int deadtreeRadius = 5;
    [SerializeField]
    [Range(0f,1f)]
    private float deadtreeIntensity = 1f;
    [SerializeField]
    private float scaleVariation = 0.3f;

    private MapGenerator mapgen;
    private string progressBarTitle = "Planting Vegetation";
    private string progressBarInfo = "0 trees have been planted";

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

        // Show a Progress Bar
        float progress = 0f;
        EditorUtility.DisplayCancelableProgressBar(progressBarTitle, progressBarInfo, progress);
        float progressStep = 1f / (float)mapgen.TotalWidth;
        int progressTreeCount = 0;
        int failedRaycastsCount = 0;

        // Iterate over the whole map using 
        for (int x = 0; x < mapgen.TotalWidth; ++x) {
            for (int y = 0; y < mapgen.TotalDepth; ++y) {

                // Check conditions for planting a tree
                if (values[x, y] > deadtreeThreshold) {
                    
                    //  1. Randomly choose a tree and position it above
                    int treeIndex = Random.Range(0, deadtrees.Length);
                    Vector3 position = new Vector3(x, 100f, y);
                    position -= new Vector3(16f, 0, 16f);
                    GameObject tree = Instantiate(deadtrees[treeIndex], position, RandomRotation()) as GameObject;
                    Transform treeT = tree.transform;
                    treeT.parent = veggieT;

                    // 2. Perform a Raycast from the tree
                    // downwards to the terrain and position it down
                    RaycastHit hit;
                    if (Physics.Raycast(new Ray(treeT.position, Vector3.down), out hit, 500f, terrainMask)) {

                        // Calculate and assign new position
                        treeT.position = new Vector3(treeT.position.x, hit.point.y - sinkInOffset, treeT.position.z);
                        treeT.localScale = RandomScale();

                        // 3. Make sure that no other trees plant themselves nearby
                        // by reducing the noise values close to the tree
                        DrawFilledCircle(ref values, Mathf.RoundToInt(x), Mathf.RoundToInt(y), deadtreeRadius, deadtreeIntensity);

                        // Increase Tree Count for the Progress Bar
                        progressTreeCount++;
                    }
                    else {
                        DestroyImmediate(tree);
                        failedRaycastsCount++;
                    }
                }
            }

            // Update Progressbar every second row
            if (x % 5 == 0) {
                progress += progressStep * 5;
                if (EditorUtility.DisplayCancelableProgressBar(progressBarTitle, progressTreeCount + " trees have been planted", progress)) {
                    EditorUtility.ClearProgressBar();
                    return;
                }
            }

        }

        Debug.Log("Planted " + progressTreeCount + " trees");
        Debug.Log("Failed Raycasts: " + failedRaycastsCount);

        // Clear Progressbar
        EditorUtility.ClearProgressBar();
    }

    Quaternion RandomRotation()
    {
        float rotationY = Random.Range(0f, 360f);
        return Quaternion.Euler(Vector3.up * rotationY);
    }

    Vector3 RandomScale()
    {
        float maxScale = 1f + scaleVariation;
        float minScale = 1f - scaleVariation;
        float r1 = Random.Range(minScale, maxScale);
        float r2 = Random.Range(minScale, maxScale);
        float r3 = Random.Range(minScale, maxScale);
        return new Vector3(r1, r2, r3);
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
        Initialise();

        values = new float[mapgen.TotalWidth, mapgen.TotalDepth];

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

    #region Circle Methods

    /// <summary>
    /// Draws a Circle in a 2D Square Grid
    /// </summary>
    /// <param name="values">The 2D Grid values (made by noise)</param>
    /// <param name="centerX">X coordinate in the square grid of the center of the circle</param>
    /// <param name="centerY">Y coordinate in the square grid of the center of the circle</param>
    /// <param name="radius">Radius of the circle</param>
    /// <param name="subtrahend">The values grid beeing subtracted by the subtrahend on the line from the circle</param>
    private void DrawCircle(ref float[,] values, int centerX, int centerY, int radius, float subtrahend)
    {
        int d = 3 - (2 * radius);
        int x = 0;
        int y = radius;

        do {
            // ensure index is in range before setting (depends on your image implementation)
            // in this case we check if the pixel location is within the bounds of the image before setting the pixel
            // then subtract the subtrahend
            // last, clamp the values between 0 and 1 since they are already normalized
            if (centerX + x >= 0 && centerX + x <= values.GetLength(0) - 1 && centerY + y >= 0 && centerY + y <= values.GetLength(1) - 1) { values[centerX + x, centerY + y] -= subtrahend; Mathf.Clamp(values[centerX + x, centerY + y], 0f, 1f);}
            if (centerX + x >= 0 && centerX + x <= values.GetLength(0) - 1 && centerY - y >= 0 && centerY - y <= values.GetLength(1) - 1) { values[centerX + x, centerY - y] -= subtrahend; Mathf.Clamp(values[centerX + x, centerY - y], 0f, 1f);}
            if (centerX - x >= 0 && centerX - x <= values.GetLength(0) - 1 && centerY + y >= 0 && centerY + y <= values.GetLength(1) - 1) { values[centerX - x, centerY + y] -= subtrahend; Mathf.Clamp(values[centerX - x, centerY + y], 0f, 1f);}
            if (centerX - x >= 0 && centerX - x <= values.GetLength(0) - 1 && centerY - y >= 0 && centerY - y <= values.GetLength(1) - 1) { values[centerX - x, centerY - y] -= subtrahend; Mathf.Clamp(values[centerX - x, centerY - y], 0f, 1f);}
            if (centerX + y >= 0 && centerX + y <= values.GetLength(0) - 1 && centerY + x >= 0 && centerY + x <= values.GetLength(1) - 1) { values[centerX + y, centerY + x] -= subtrahend; Mathf.Clamp(values[centerX + y, centerY + x], 0f, 1f);}
            if (centerX + y >= 0 && centerX + y <= values.GetLength(0) - 1 && centerY - x >= 0 && centerY - x <= values.GetLength(1) - 1) { values[centerX + y, centerY - x] -= subtrahend; Mathf.Clamp(values[centerX + y, centerY - x], 0f, 1f);}
            if (centerX - y >= 0 && centerX - y <= values.GetLength(0) - 1 && centerY + x >= 0 && centerY + x <= values.GetLength(1) - 1) { values[centerX - y, centerY + x] -= subtrahend; Mathf.Clamp(values[centerX - y, centerY + x], 0f, 1f);}
            if (centerX - y >= 0 && centerX - y <= values.GetLength(0) - 1 && centerY - x >= 0 && centerY - x <= values.GetLength(1) - 1) { values[centerX - y, centerY - x] -= subtrahend; Mathf.Clamp(values[centerX - y, centerY - x], 0f, 1f);}
            if (d < 0) {
                d += 2 * x + 1;
            }
            else {
                d += 2 * (x - y) + 1;
                y--;
            }
            x++;
        } while (x <= y);
    }

    struct cell
    {
        public int x;
        public int y;

        public cell(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    bool InRange(int x, int y)
    {
        return
            x >= 0 && x < mapgen.TotalWidth &&
            y >= 0 && y < mapgen.TotalDepth;
    }

    bool InRange(cell c)
    {
        return
            c.x >= 0 && c.x < mapgen.TotalWidth &&
            c.y >= 0 && c.y < mapgen.TotalDepth;
    }

    private void DrawFilledCircle(ref float[,] values, int centerX, int centerY, int radius, float subtrahend)
    {
        int x = radius;
        int y = 0;
        int xChange = 1 - (radius << 1);
        int yChange = 0;
        int radiusError = 0;

        // List of all the cells in the circle
        List<cell> cells = new List<cell>();
        cell tmp;

        // adding the cells to the list
        while (x >= y) {
            for (int i = centerX - x; i <= centerX + x; i++) {
                tmp = new cell(i, centerY + y);
                if (!cells.Contains(tmp)) cells.Add(tmp);
                tmp = new cell(i, centerY - y);
                if (!cells.Contains(tmp)) cells.Add(tmp);
            }
            for (int i = centerX - y; i <= centerX + y; i++) {
                tmp = new cell(i, centerY + x);
                if (!cells.Contains(tmp)) cells.Add(tmp);
                tmp = new cell(i, centerY - x);
                if (!cells.Contains(tmp)) cells.Add(tmp);
            }

            y++;
            radiusError += yChange;
            yChange += 2;
            if (((radiusError << 1) + xChange) > 0) {
                x--;
                radiusError += xChange;
                xChange += 2;
            }
        }

        // subtracting the subtrahend once for every cell
        foreach (cell c in cells) {
            if (InRange(c)) {
                values[c.x, c.y] -= subtrahend * GetSubtrahendPercentage(centerX, centerY, c.x, c.y, radius);
                values[c.x, c.y] = Mathf.Clamp01(values[c.x, c.y]);
            }
        }
    }

    // Calculates the distance to the center of this circle-point so its a gradientfill
    private float GetSubtrahendPercentage(cell center, int x, int y, float radius)
    {
        Vector2 centerv = new Vector2(center.x, center.y);
        Vector2 cell = new Vector2(x, y);
        float distance = Vector2.Distance(centerv, cell);

        return 1f - (distance / radius);
    }

    private float GetSubtrahendPercentage(int centerX, int centerY, int x, int y, float radius)
    {
        return GetSubtrahendPercentage(new cell(centerX, centerY), x, y, radius);
    }

    #endregion
}
