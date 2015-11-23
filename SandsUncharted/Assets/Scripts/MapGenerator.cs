using UnityEngine;
using System.Collections;
using System;

public class MapGenerator : MonoBehaviour
{
    #region private
    [SerializeField]
    private int chunkSize = 16;
    [SerializeField]
    private float voxelSize = 1f;
    [SerializeField]
    private int width = 1;
    [SerializeField]
    private int height = 1;
    [SerializeField]
    private int depth = 1;
    [SerializeField]
    private float isolevel = 0;

    private Chunk[,,] chunkMap;
    #endregion

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        chunkMap = new Chunk[width, height, depth];

        RandomFillMap();

        MeshGenerator meshGen = GetComponent<MeshGenerator>();
        meshGen.GenerateMesh(densityMap, voxelSize, isolevel);
    }

    void RandomFillMap()
    {
        for (int x = 0; x < width; ++x) {
            for (int y = 0; y < height; ++y) {
                for (int z = 0; z < depth; ++z) {
                    // Set map to 1 or 0 depending the PerlinNoise
                    float xfloat = (float)x / width;
                    float yfloat = (float)y / height;
                    float zfloat = (float)z / depth;
                    densityMap[x, y, z] = yfloat;
                    densityMap[x, y, z] += Noise.GetOctaveNoise(x, yfloat, z, 2);
                }
            }
        }
    }

    bool IsInBounds(int x, int y, int z)
    {
        return x >= 0 && x < width && y >= 0 && y < height && z >= 0 && z < depth;
    }

    public class Chunk
    {
        private int xPos, yPos, zPos;
        private int size;
        private float[, ,] densityMap;

        public Vector3 Position { get { return new Vector3(xPos, yPos, zPos); } }
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

        public Chunk(int xPos, int yPos, int zPos, int size) : this(size)
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
}

