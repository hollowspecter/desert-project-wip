using UnityEngine;
using UnityEditor;

public class MapGenerator : MonoBehaviour
{
    #region private
    [SerializeField]
    private int chunkSize = 16;
    [SerializeField]
    private float voxelSize = 1f;
    [Range(1,16)]
    [SerializeField]
    private int width = 1;
    [Range(1, 16)]
    [SerializeField]
    private int height = 1;
    [Range(1, 16)]
    [SerializeField]
    private int depth = 1;
    [SerializeField]
    private float isolevel = 0;

    private ChunkMap chunkMap;
    private TextureCreator textureCreator;
    private string progressBarTitle = "Density Map is being calculated";
    private string progressBarInfo = "Please sit back and have a sip of tea.";
    #endregion

    #region Properties

    public NoiseLayer[] noises;

    #endregion

    public void GenerateMap()
    {
        chunkMap = new ChunkMap(width, height, depth, chunkSize);
        RandomFillMap();

        MeshGenerator meshGen = GetComponent<MeshGenerator>();
        meshGen.GenerateMesh(chunkMap, voxelSize, isolevel);
    }

    void RandomFillMap()
    {
        // Initialize a Progress Bar
        float progress = 0f;
        EditorUtility.DisplayProgressBar(progressBarTitle, progressBarInfo, progress);

        // Calculate the total of the map
        int totalWidth = width * chunkSize;// -(width - 1);
        int totalHeight = height * chunkSize;// -(height - 1);
        int totalDepth = depth * chunkSize;// -(depth - 1);

        // Calculate the step
        float step = 1f / totalWidth;

        for (int x = 0; x < totalWidth; ++x) {
            for (int y = 0; y < totalHeight; ++y) {
                for (int z = 0; z < totalDepth; ++z) {

                    // Set map to 1 or 0 depending the PerlinNoise
                    float yfloat = (float)y / height;

                    // Get corresponding chunk
                    int chunkX = x / chunkSize;
                    int chunkY = y / chunkSize;
                    int chunkZ = z / chunkSize;

                    // Check if it is an overlapping value by only checking the beginning of the chunk (not the end)
                    float overlapValue;
                    bool overlap = false;
                    if (x % chunkSize == 0 && x > 0) {
                        overlapValue = chunkMap[chunkX - 1, chunkY, chunkZ][chunkSize - 1, y % chunkSize, z % chunkSize];
                        chunkMap[chunkX, chunkY, chunkZ][x % chunkSize, y % chunkSize, z % chunkSize] = overlapValue;
                        overlap = true;
                    }
                    if (y % chunkSize == 0 && y > 0) {
                        overlapValue = chunkMap[chunkX, chunkY-1, chunkZ][x % chunkSize, chunkSize - 1, z % chunkSize];
                        chunkMap[chunkX, chunkY, chunkZ][x % chunkSize, y % chunkSize, z % chunkSize] = overlapValue;
                        overlap = true;
                    }
                    if (z % chunkSize == 0 && z > 0) {
                        overlapValue = chunkMap[chunkX, chunkY, chunkZ - 1][x % chunkSize, y % chunkSize, chunkSize - 1];
                        chunkMap[chunkX, chunkY, chunkZ][x % chunkSize, y % chunkSize, z % chunkSize] = overlapValue;
                        overlap = true;
                    }
                    if (overlap)
                        continue;

                    /*
                     * Calculate density value from noise layers
                     */
                    float value = yfloat;
                    value += GetValueFromNoises(new Vector3(x, yfloat, z));

                    // Apply the density value
                    chunkMap[chunkX, chunkY, chunkZ][x % chunkSize, y % chunkSize, z % chunkSize] = value;
                }
            }

            // Update the Progress Bar
            progress += step;
            EditorUtility.DisplayProgressBar(progressBarTitle, progressBarInfo, progress);

        } // end last for loop

        EditorUtility.ClearProgressBar();
    } // end random fill

    public float GetValueFromNoises(Vector3 point)
    {
        float value = 0;
        for (int i = 0; i < noises.Length; ++i) {
            // Check if active
            if (!noises[i].Active)
                continue;

            // Check the operation and act accordingly
            NoiseLayer.NoiseOperators op = noises[i].Operation;
            switch (op) {
                case NoiseLayer.NoiseOperators.Add:
                    value += noises[i].getValue(point) * noises[i].Weight;
                    break;
                case NoiseLayer.NoiseOperators.Subtract:
                    value -= noises[i].getValue(point) * noises[i].Weight;
                    break;
            }
        }
        return value;
    }

    public float GetValueFromNoises(float x, float y, float z)
    {
        return GetValueFromNoises(new Vector3(x, y, z));
    }

    public void SaveAndDeleteTerrain()
    {
        // Find the chunks Game Object
        GameObject chunks = GameObject.FindGameObjectWithTag(Tags.TERRAIN_TAG);
        // If found...
        if (chunks != null) {
            // Create a string name for saving it as a prefab
            string time = System.DateTime.Now.ToString();
            time = time.Replace("/", "_");
            time = time.Replace(" ", "_");
            time = time.Replace(":", "-");
            var targetPath = FileUtil.GetProjectRelativePath(EditorUtility.SaveFilePanel("Saves the old Terrain", Application.dataPath, "terrain_" + time, "prefab"));
            // If the user cancels saving, dont save
            if (targetPath.Length > 0)
                PrefabUtility.CreatePrefab(targetPath, chunks);
            // Destroy the old Terrain
            DestroyImmediate(chunks);
        }
        else {
            Debug.Log("Did not found a gameobject with \"Terrain\" tag.");
        }
    }

    ///// <summary>
    ///// Calculates the Normals by using the central difference of the volumetric data
    ///// in each direction
    ///// </summary>
    ///// <param name="p"></param>
    ///// <returns></returns>
    ///// 
    ///*
    // * calculate normals in every node and interpolate in marhcing cube algorithm
    // */
    //public Vector3 CalculateNormal(int x, int y, int z)
    //{
    //    float step = 0.01f;
    //    float x = GetValueFromNoises(p.x + step, p.y, p.z) - GetValueFromNoises(p.x - step, p.y, p.z);
    //    float y = GetValueFromNoises(p.x, p.y + step, p.z) - GetValueFromNoises(p.x, p.y - step, p.z);
    //    float z = GetValueFromNoises(p.x, p.y, p.z + step) - GetValueFromNoises(p.x, p.y, p.z - step);
    //    return new Vector3(x, y, z).normalized;
    //}
}