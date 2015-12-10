using UnityEngine;
using UnityEngine.Assertions;
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
    private LUTGenerator lutGen;
    private string progressBarTitle = "Density Map is being calculated";
    private string progressBarInfo = "Please sit back and have a sip of tea.";
    #endregion

    #region Properties

    public NoiseLayer[] noises;

    #endregion

    public void GenerateMap()
    {
        chunkMap = new ChunkMap(width, height, depth, chunkSize);

        if (!RandomFillMap()) {
            Debug.Log("Building was cancelled");
            return;
        }

        MeshGenerator meshGen = GetComponent<MeshGenerator>();
        meshGen.GenerateMesh(chunkMap, voxelSize, isolevel);
    }

    private bool RandomFillMap()
    {
        // Initialize a Progress Bar
        float progress = 0f;
        EditorUtility.DisplayCancelableProgressBar(progressBarTitle, progressBarInfo, progress);

        // Calculate the total of the map
        int totalWidth = width * chunkSize;
        int totalHeight = height * chunkSize;
        int totalDepth = depth * chunkSize;

        //// Calculate the step
        float step = 1f / chunkMap.Count;

        float timer = Time.realtimeSinceStartup;

        foreach (Chunk c in chunkMap) {

            for (int x = 0; x < chunkSize; ++x) {
                for (int y = 0; y < chunkSize; ++y) {
                    for (int z = 0; z < chunkSize; ++z) {

                        int xPos = x + (int) c.ChunkmapPosition.x;
                        int yPos = y + (int)c.ChunkmapPosition.y;
                        int zPos = z + (int)c.ChunkmapPosition.z;

                        float yfloat = (float)yPos / height;

                        /*
                         * Calculate density value from noise layers
                         */
                        float value = yfloat;
                        value += GetValueFromNoises(new Vector3(xPos, yfloat, zPos));
                        c[x, y, z] = value;
                    }
                }
            }

            progress += step;
            if (EditorUtility.DisplayCancelableProgressBar(progressBarTitle, progressBarInfo, progress)) {
                EditorUtility.ClearProgressBar();
                return false;
            }
        }

        Debug.Log("Time for chunkiteration: " + (Time.realtimeSinceStartup - timer));

        EditorUtility.ClearProgressBar();
        return true;
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
}