using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

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

    private Chunk[,,] chunkMap;
    private TextureCreator textureCreator;
    #endregion

    #region Properties

    public NoiseLayer[] noises;

    #endregion

    public void GenerateMap()
    {
        // Generate Chunk Array
        chunkMap = new Chunk[width, height, depth];
        for (int x = 0; x < width; ++x) {
            for (int y = 0; y < height; ++y) {
                for (int z = 0; z < depth; ++z) {
                    Chunk chunk = new Chunk(x, y, z, chunkSize);
                    chunkMap[x, y, z] = chunk;
                }
            }
        }
        RandomFillMap();

        MeshGenerator meshGen = GetComponent<MeshGenerator>();
        meshGen.GenerateMesh(chunkMap, voxelSize, isolevel);
    }

    void RandomFillMap()
    {
        int totalWidth = width * chunkSize;// -(width - 1);
        int totalHeight = height * chunkSize;// -(height - 1);
        int totalDepth = depth * chunkSize;// -(depth - 1);

        for (int x = 0; x < totalWidth; ++x) {
            for (int y = 0; y < totalHeight; ++y) {
                for (int z = 0; z < totalDepth; ++z) {
                    // Set map to 1 or 0 depending the PerlinNoise
                    float yfloat = (float)y / height;

                    // Get corresponding chunk
                    int chunkX = x / chunkSize;
                    int chunkY = y / chunkSize;
                    int chunkZ = z / chunkSize;

                    // Check if it is an overlapping value
                    float overlapValue;
                    bool overlap = false;
                    if (x % chunkSize == 0 && x > 0) {
                        overlapValue = chunkMap[chunkX - 1, chunkY, chunkZ].getDensityValue(chunkSize - 1, y % chunkSize, z % chunkSize);
                        chunkMap[chunkX, chunkY, chunkZ].setDensityMap(x % chunkSize, y % chunkSize, z % chunkSize, overlapValue);
                        overlap = true;
                    }
                    if (y % chunkSize == 0 && y > 0) {
                        overlapValue = chunkMap[chunkX, chunkY-1, chunkZ].getDensityValue(x % chunkSize, chunkSize - 1, z % chunkSize);
                        chunkMap[chunkX, chunkY, chunkZ].setDensityMap(x % chunkSize, y % chunkSize, z % chunkSize, overlapValue);
                        overlap = true;
                    }
                    if (z % chunkSize == 0 && z > 0) {
                        overlapValue = chunkMap[chunkX, chunkY, chunkZ - 1].getDensityValue(x % chunkSize, y % chunkSize, chunkSize - 1);
                        chunkMap[chunkX, chunkY, chunkZ].setDensityMap(x % chunkSize, y % chunkSize, z % chunkSize, overlapValue);
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
                    chunkMap[chunkX, chunkY, chunkZ].setDensityMap(x % chunkSize, y % chunkSize, z % chunkSize, value);
                }
            }
        } // end last for loop
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
                    value += noises[i].getValue(point);
                    break;
                case NoiseLayer.NoiseOperators.Subtract:
                    value -= noises[i].getValue(point);
                    break;
            }
        }
        return value;
    }

    public void SaveAndDeleteTerrain()
    {
        GameObject chunks = GameObject.FindGameObjectWithTag(Tags.TERRAIN_TAG);
        if (chunks != null) {
            string time = System.DateTime.Now.ToString();
            time = time.Replace("/", "_");
            time = time.Replace(" ", "_");
            time = time.Replace(":", "-");
            var targetPath = FileUtil.GetProjectRelativePath(EditorUtility.SaveFilePanel("Saves the old Terrain", Application.dataPath, "terrain_" + time, "prefab"));
            PrefabUtility.CreatePrefab(targetPath, chunks);
            DestroyImmediate(chunks);
        }
        else {
            Debug.Log("Did not found a gameobject with \"Terrain\" tag.");
        }
    }

    //public void VisualizeAll()
    //{
    //    CheckTextureCreator();
    //    textureCreator.FillTexture(this, false, 0);
    //}

    //public void Visualize(int index)
    //{
    //    CheckTextureCreator();
    //    if (index >= 0 || index < noises.Length)
    //        textureCreator.FillTexture(this, true, index);
    //}

    //private void CheckTextureCreator()
    //{
    //    if (textureCreator == null)
    //        textureCreator = GetComponent<TextureCreator>();
    //    else
    //        Debug.LogError("There is no TextureCreator script attached to this GO", this);
    //}
}

/// <summary>
/// One Chunk contains the density of a cube of one
/// portion of the whole map.
/// Each chunk will have its own GO with its own mesh.
/// </summary>
public class Chunk
{
    private int xPos, yPos, zPos;
    private int size;
    private float[, ,] densityMap;

    public Vector3 Position { get { return new Vector3(xPos, yPos, zPos); } }
    public Vector3 ChunkmapPosition { get { return new Vector3(xPos * size, yPos * size, zPos * size); } }
    public int Size { get { return size; } }

    /* Construcors */

    public Chunk(int size)
    {
        xPos = 0;
        yPos = 0;
        zPos = 0;
        densityMap = new float[size, size, size];
        densityMap.Initialize();
        this.size = size;
    }

    public Chunk(int xPos, int yPos, int zPos, int size)
        : this(size)
    {
        this.xPos = xPos;
        this.yPos = yPos;
        this.zPos = zPos;
    }

    /* Getters and Setters */
    public void setDensityMap(int x, int y, int z, float value)
    {
        densityMap[x, y, z] = value;
    }

    public float getDensityValue(int x, int y, int z)
    {
        return densityMap[x, y, z];
    }
}