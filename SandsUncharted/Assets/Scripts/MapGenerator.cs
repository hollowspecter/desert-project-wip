using UnityEngine;
using System.Collections;
using System;

public class MapGenerator : MonoBehaviour
{
    #region private
    [SerializeField]
    private int width;
    [SerializeField]
    private int height;
    [SerializeField]
    private int depth;
    [SerializeField]
    private int smoothingWallThreshold = 6;
    [SerializeField]
    private int smoothingIterations = 3;
    [SerializeField]
    private int isolevel = 0;

    private int[, ,] map;
    #endregion

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        map = new int[width, height, depth];
        map.Initialize();
        RandomFillMap();

        for (int i = 0; i < smoothingIterations; ++i) {
            SmoothMap();
        }

        MeshGenerator meshGen = GetComponent<MeshGenerator>();
        meshGen.GenerateMesh(map, 1f, isolevel);
    }

    void RandomFillMap()
    {
        for (int x = 0; x < width; ++x) {
            for (int y = 0; y < height; ++y) {
                for (int z = 0; z < depth; ++z) {
                    // Set map to 1 or 0 depending the PerlinNoise
                    float xfloat = (float)x / width;
                    float zfloat = (float)z / depth;
                    int h = Mathf.RoundToInt(Mathf.PerlinNoise(xfloat, zfloat) * height); // One Noise Layer
                    if (y < h)
                        map[x, y, z] = 1;
                }
            }
        }
    }

    void SmoothMap()
    {
        // Loop through all the voxels
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                for (int z = 0; z < depth; z++) {

                    // Check number of neighbors and change chunks status accordingly
                    int neighborWallTiles = GetSurroundingChunkCount(x, y, z);

                    if (neighborWallTiles > smoothingWallThreshold)
                        map[x, y, z] = 1;
                    else if (neighborWallTiles < smoothingWallThreshold)
                        map[x, y, z] = 0;
                }
            }
        }
    }

    int GetSurroundingChunkCount(int gridX, int gridY, int gridZ)
    {
        int wallCount = 0;

        for (int neighborX = gridX - 1; neighborX <= gridX + 1; ++neighborX) {
            for (int neighborY = gridY - 1; neighborY <= gridY + 1; ++neighborY) {
                for (int neighborZ = gridZ - 1; neighborZ <= gridZ + 1; ++neighborZ) {

                    // do not look out of bounds
                    if (!IsInBounds(neighborX, neighborY, neighborZ)) {
                        wallCount++; // encourage growth of walls on the border
                        continue;
                    }

                    // do not consider the initial file
                    if (neighborX == gridX && neighborY == gridY && neighborZ == gridZ)
                        continue;

                    // is neighbor a wall?
                    wallCount += map[neighborX, neighborY, neighborZ];
                }
            }
        }

        return wallCount;
    }

    bool IsInBounds(int x, int y, int z)
    {
        return x >= 0 && x < width && y >= 0 && y < height && z >= 0 && z < depth;
    }

    //void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.white;

    //    // Draw all the map nodes
    //    if (map != null) {
    //        for (int x = 0; x < width; ++x) {
    //            for (int y = 0; y < height; ++y) {
    //                for (int z = 0; z < depth; ++z) {
    //                    if (map[x, y, z] == 1) {
    //                        Vector3 pos = new Vector3(-width / 2 + x + .5f, -height / 2 + y + .5f, -depth / 2 + z + .5f);
    //                        Gizmos.DrawCube(pos, Vector3.one);
    //                    }
    //                }
    //            }
    //        }
    //    }
    //}
}

