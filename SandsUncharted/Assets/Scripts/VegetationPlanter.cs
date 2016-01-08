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
                    Vector3 position = new Vector3(x, 100f, y);
                    position -= new Vector3(16f, 0, 16f);
                    GameObject tree = Instantiate(deadtrees[treeIndex], position, Quaternion.identity) as GameObject;
                    Transform treeT = tree.transform;
                    treeT.parent = veggieT;

                    // Perform a Raycast from the tree
                    // downwards to the terrain and position it down
                    RaycastHit hit;
                    if (Physics.Raycast(new Ray(treeT.position, Vector3.down), out hit, 500f, terrainMask)) {

                        // Calculate and assign new position
                        treeT.position = new Vector3(treeT.position.x, hit.point.y - sinkInOffset, treeT.position.z);
                    }
                    else {
                        Debug.Log("No Hit!");
                        //DestroyImmediate(tree);
                    }
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

        // DEBUG: Draw a circle!
        DrawFilledCircle2(ref values, 65, 100, 20, 1f);

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

    //cell Neighbor(cell c, int dir)
    //{
    //    if (dir == 0) return new cell(c.x+1, c.y);
    //    if (dir == 1) return new cell(c.x, c.y+1);
    //    if (dir == 2) return new cell(c.x-1, c.y);
    //    if (dir == 3) return new cell(c.x, c.y-1);

    //    Debug.LogWarning("wrong direction", this);
    //    return new cell(0, 0);
    //}

    //private void DrawFilledCircle(ref float[,] values, int centerX, int centerY, int radius, float subtrahend, FillMode mode)
    //{
    //    // Initialise the Lists
    //    cell start = new cell(centerX, centerY);
    //    List<cell> visited = new List<cell>();
    //    List<cell> border = new List<cell>();
    //    List<cell> fringe = new List<cell>();
    //    fringe.Add(start);
    //    values[centerX, centerY] -= subtrahend;
    //    float stepsize = 1f / (float)radius;

    //    // Fill in the Border List
    //    int d = 3 - (2 * radius);
    //    int x = 0;
    //    int y = radius;
    //    do {
    //        // ensure index is in range before setting (depends on your image implementation)
    //        // in this case we check if the pixel location is within the bounds of the image before setting the pixel
    //        // then subtract the subtrahend
    //        // last, clamp the values between 0 and 1 since they are already normalized
    //        if (InRange(centerX + x, centerY + y)) { border.Add(new cell(centerX + x, centerY + y)); }
    //        if (InRange(centerX + x, centerY - y)) { border.Add(new cell(centerX + x, centerY - y)); }
    //        if (InRange(centerX - x, centerY + y)) { border.Add(new cell(centerX - x, centerY + y)); }
    //        if (InRange(centerX - x, centerY - y)) { border.Add(new cell(centerX - x, centerY - y)); }
    //        if (InRange(centerX + y, centerY + x)) { border.Add(new cell(centerX + y, centerY + x)); }
    //        if (InRange(centerX + y, centerY - x)) { border.Add(new cell(centerX + y, centerY - x)); }
    //        if (InRange(centerX - y, centerY + x)) { border.Add(new cell(centerX - y, centerY + x)); }
    //        if (InRange(centerX - y, centerY - x)) { border.Add(new cell(centerX - y, centerY - x)); }
    //        if (d < 0) {
    //            d += 2 * x + 1;
    //        }
    //        else {
    //            d += 2 * (x - y) + 1;
    //            y--;
    //        }
    //        x++;
    //    } while (x <= y);

    //    // Flood fill by filling from the center to the borders
    //    // when a border is hit, the cell is removed from the border
    //    for (int i = 0; i < radius-10; ++i) {
    //        //while (border.Count > 0) {
    //        List<cell> newfringe = new List<cell>();

    //        if (mode == FillMode.GRADIENT)
    //            subtrahend += stepsize;

    //        foreach (cell c in fringe) {
    //            for (int dir = 0; dir < 4; ++dir) {
    //                cell neighbor = Neighbor(c, dir);
    //                if (!visited.Contains(neighbor)) {
    //                    if (!border.Contains(neighbor)) {
    //                        visited.Add(neighbor);
    //                        newfringe.Add(neighbor);
    //                        if (InRange(neighbor))
    //                            values[neighbor.x, neighbor.y] -= subtrahend;
    //                    }
    //                    else
    //                        border.Remove(neighbor);
    //                }
    //            }
    //        }
    //        fringe.Clear();
    //        fringe = newfringe;
    //    }
    //}

    //private void DrawFilledCircle(ref float[,] values, int centerX, int centerY, int radius, float subtrahend)
    //{
    //    // Fill in the Border List
    //    int d = 3 - (2 * radius);
    //    int x = 0;
    //    int y = radius;
    //    cell center = new cell(centerX, centerY);
    //    do {
    //        FillHorizontalLine(ref values, new cell(centerX + x, centerY + y), new cell(centerX - x, centerY + y), center, subtrahend, radius);
    //        FillHorizontalLine(ref values, new cell(centerX + x, centerY - y), new cell(centerX - x, centerY - y), center, subtrahend, radius);
    //        FillHorizontalLine(ref values, new cell(centerX + y, centerY + x), new cell(centerX - y, centerY + x), center, subtrahend, radius);
    //        FillHorizontalLine(ref values, new cell(centerX + y, centerY - x), new cell(centerX - y, centerY - x), center, subtrahend, radius);
    //        if (d < 0) {
    //            d += 2 * x + 1;
    //        }
    //        else {
    //            d += 2 * (x - y) + 1;
    //            y--;
    //        }
    //        x++;
    //    } while (x <= y);
    //}

    //private void FillHorizontalLine(ref float[,] values, cell start, cell end, cell center, float subtrahend, int radius)
    //{
    //    //// We assume that end is aboce
    //    if (end.x < start.x) {
    //        cell tmp = start;
    //        start = end;
    //        end = tmp;
    //    }

    //    int distance = end.x - start.x;

    //    for (int i = 0; i <= distance; ++i) {
    //        if (InRange(start)) {
    //            values[start.x + i, start.y] -= subtrahend * GetSubtrahendPercentage(center, start.x + i, start.y, radius);
    //            Mathf.Clamp(values[start.x+i, start.y], 0f, 1f);
    //        }
    //    }
    //}

    private void DrawFilledCircle2(ref float[,] values, int centerX, int centerY, int radius, float subtrahend)
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
