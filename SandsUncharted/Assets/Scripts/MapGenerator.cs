using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using UnityEditor;
using System.Collections;

public class MapGenerator : MonoBehaviour
{
    #region private
    [SerializeField]
    private Transform UI;
    [SerializeField]
    private Transform playerTransform;
    [SerializeField]
    private Texture2D biomeTexture;
    [SerializeField]
    private int chunkSize = 16;
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
    [Range(0, 4)]
    [SerializeField]
    private int lod_input;
    [SerializeField]
    private int highestLOD_input = 4;
    [SerializeField]
    private bool SortEmptyChunksOut = true;

    private ChunkMap chunkMap;
    private LUTGenerator lutGen;
    private string progressBarTitle = "Density Map is being calculated";
    private string progressBarInfo = "Please sit back and have a sip of tea.";
    private bool UIenabled = false;
    private ThreadManagement threading;

    // UI
    private Text ui_loadingText;
    private Slider ui_progressSlider;
    private GameObject ui_loadingPanel;
    #endregion

    #region Properties

    public enum BiomeSolo { none, red, green, blue };
    public int LOD { get { return (int)Mathf.Pow(2, lod_input); } }
    public int HighestLOD { get { return (int)Mathf.Pow(2, highestLOD_input); } }

    public ChunkMap ChunkMap { get { return chunkMap; } }

    public Biome redBiome;
    public Biome greenBiome;
    public Biome blueBiome;
    public BiomeSolo solo;

    public int TotalWidth { get { return width * chunkSize; } }
    public int TotalHeight { get { return height * chunkSize; } }
    public int TotalDepth { get { return depth * chunkSize; } }

    #endregion

    #region Play mode methods

    void Awake()
    {
        // Is everything ready to generate the map?
        if (!CheckConditions()) {
            Debug.LogError("Errors for initilisation have occurred!");
        }

        // Generate a new Chunk Map
        chunkMap = new ChunkMap(width, height, depth, chunkSize);

        // Init the UI feedback
        if (UI != null) {
            UIenabled = true;
            ui_loadingPanel = UI.FindChild("LoadingPanel").gameObject;
            Assert.IsNotNull<GameObject>(ui_loadingPanel);
            ui_loadingText = ui_loadingPanel.transform.FindChild("LoadingText").GetComponent<Text>();
            Assert.IsNotNull<Text>(ui_loadingText);
            ui_progressSlider = ui_loadingPanel.transform.FindChild("ProgressSlider").GetComponent<Slider>();
            Assert.IsNotNull<Slider>(ui_progressSlider);
        }

        // Some checking
        threading = GetComponent<ThreadManagement>();
        Assert.IsNotNull<ThreadManagement>(threading);
        Assert.IsNotNull<Transform>(playerTransform);
    }

    void Start()
    {
        // Start filling the Density map
        Debug.Log("Filling the density map.");
        StartCoroutine(CalculateDensityMap());
    }

    IEnumerator CalculateDensityMap()
    {
        // Initialize a Progress Bar
        int chunkProgress = 0;

        foreach (Chunk c in chunkMap) {

            // Determine, if this chunk will have a
            // surface to extract. If not, flag the chunk
            float minValue = float.MaxValue;
            float maxValue = float.MinValue;

            for (int x = 0; x < chunkSize; ++x) {
                for (int y = 0; y < chunkSize; ++y) {
                    for (int z = 0; z < chunkSize; ++z) {

                        int xPos = x + (int)c.ChunkmapPosition.x;
                        int yPos = y + (int)c.ChunkmapPosition.y;
                        int zPos = z + (int)c.ChunkmapPosition.z;

                        float yfloat = (float)yPos / height;

                        /*
                         * Calculate density value from noise layers
                         */
                        float value = yfloat;
                        value += GetValueFromBiomes(new Vector3(xPos, yfloat, zPos));
                        c[x, y, z] = value;

                        if (value < minValue)
                            minValue = value;
                        if (value > maxValue)
                            maxValue = value;
                    }

                    
                }
            }

            /* End of one chunk */

            if (!(minValue <= isolevel && isolevel <= maxValue)) {
                c.hasSurface = false;
            }

            chunkProgress++;
            if (UIenabled) {
                ui_progressSlider.value = (float)chunkProgress / (float)chunkMap.Count;
                ui_loadingText.text = "Calculating the desert \n \n" + chunkProgress + "/" + chunkMap.Count;
            }
            yield return new WaitForEndOfFrame();
        }
        ui_loadingPanel.SetActive(false);

        // Now queue all the jobs on the ThreadManagement
        for (int x = 0; x < width; ++x) {
            for (int y = 0; y < height; ++y) {
                for (int z = 0; z < depth; ++z) {
                    if (chunkMap[x, y, z].hasSurface) {
                        // Calculate the priority
                        float priority = Vector3.Distance(playerTransform.position, chunkMap.GetChunkWorldpos(x,y,z));
                        threading.EnqueueJob(new MeshTask(x, y, z, chunkSize), priority);
                    }
                }                
            }
        }
        threading.ShowNumberOfJobs();
    }

    #endregion

    bool CheckConditions()
    {
        if (blueBiome == null ||
            redBiome == null ||
            greenBiome == null)
        {
            Debug.LogError("At least one biome is null. Error.");
            return false;
        }

        if (biomeTexture == null) {
            Debug.LogError("No Biome Texture is assigned! Error.");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Is called from the Editor
    /// </summary>
    public void GenerateMap()
    {
        // Is everything ready to generate the map?
        if (!CheckConditions())
            return;

        // Generate a new Chunk Map
        chunkMap = new ChunkMap(width, height, depth, chunkSize);

        // Fill up the map
        if (!RandomFillMap()) {
            Debug.Log("Building was cancelled");
            return;
        }

        // Generate the Mesh
        MeshGenerator meshGen = GetComponent<MeshGenerator>();
        meshGen.EditorGenerateMesh(chunkMap, 1f, isolevel, LOD, HighestLOD);
    }

    private bool RandomFillMap()
    {
        // Initialize a Progress Bar
        float progress = 0f;
        EditorUtility.DisplayCancelableProgressBar(progressBarTitle, progressBarInfo, progress);

        // Calculate the step
        float step = 1f / (float)chunkMap.Count;

        // Count the empty chunks
        int emptyChunkCounter = 0;

        foreach (Chunk c in chunkMap) {

            // Determine, if this chunk will have a
            // surface to extract. If not, flag the chunk
            float minValue = float.MaxValue;
            float maxValue = float.MinValue;

            for (int x = 0; x < chunkSize; ++x) {
                for (int y = 0; y < chunkSize; ++y) {
                    for (int z = 0; z < chunkSize; ++z) {

                        int xPos = x + (int)c.ChunkmapPosition.x;
                        int yPos = y + (int)c.ChunkmapPosition.y;
                        int zPos = z + (int)c.ChunkmapPosition.z;

                        float yfloat = (float)yPos / height;

                        /*
                         * Calculate density value from noise layers
                         */
                        float value = yfloat;
                        value += GetValueFromBiomes(new Vector3(xPos, yfloat, zPos));
                        c[x, y, z] = value;

                        if (value < minValue)
                            minValue = value;
                        if (value > maxValue)
                            maxValue = value;
                    }
                }
            }

            if (SortEmptyChunksOut) {
                if (!(minValue <= isolevel && isolevel <= maxValue)) {
                    emptyChunkCounter++;
                    c.hasSurface = false;
                }
            }
            
            progress += step;
            if (EditorUtility.DisplayCancelableProgressBar(progressBarTitle, progressBarInfo, progress)) {
                EditorUtility.ClearProgressBar();
                return false;
            }
        }//End Foreach over chunkmap

        Debug.Log("Empty Chunk Counter: " + emptyChunkCounter);

        EditorUtility.ClearProgressBar();
        return true;
    } // end random fill

    public float GetValueFromBiomes(Vector3 point)
    {
        // Soloing one Biome everywhere?
        if (solo != BiomeSolo.none) {
            switch (solo) {
                case BiomeSolo.red:
                    return redBiome.GetValueFromNoises(point);
                case BiomeSolo.green:
                    return greenBiome.GetValueFromNoises(point);
                case BiomeSolo.blue:
                    return blueBiome.GetValueFromNoises(point);
            }
        }

        // Sample the Texture
        Color sample = SampleBiomeTexture(point);

        // Calculate Values
        float[] value = new float[3];

        for (int i = 0; i < 3; ++i) {
            if ((i == 0 && sample.r > 0) ||
                (i == 1 && sample.g > 0) ||
                (i == 2 && sample.b > 0))
            {
                value[i] = getBiome(i).GetValueFromNoises(point);
            }
        }

        // Calculate the weighted sum
        return sample.r * value[0] + sample.g * value[1] + sample.b * value[2];
    }

    // Returns sum-normalized color sample from the texture
    public Color SampleBiomeTexture(Vector3 point)
    {
        float x = (float)point.x / (float)TotalWidth;
        float z = (float)point.z / (float)TotalDepth;

        int xPixel = Mathf.RoundToInt(x * biomeTexture.width);
        int yPixel = Mathf.RoundToInt(z * biomeTexture.height);

        Color sample = biomeTexture.GetPixel(xPixel, yPixel);

        // normalize color
        float sum = sample.r + sample.g + sample.b;
        sample.r = sample.r / sum;
        sample.g = sample.g / sum;
        sample.b = sample.b / sum;

        return sample;
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

    public Biome getBiome(int index)
    {
        if (index == 0)
            return redBiome;
        else if (index == 1)
            return greenBiome;
        else if (index == 2)
            return blueBiome;
        else
            return null;
    }
}