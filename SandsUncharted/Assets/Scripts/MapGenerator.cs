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
    private float isolevel = 0;

    private float[, ,] densityMap;
    #endregion

    void Start()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        densityMap = new float[width, height, depth];
        densityMap.Initialize();
        RandomFillMap();

        MeshGenerator meshGen = GetComponent<MeshGenerator>();
        meshGen.GenerateMesh(densityMap, 1f, isolevel);
    }

    void RandomFillMap()
    {
        for (int x = 0; x < width; ++x) {
            for (int y = 0; y < height; ++y) {
                for (int z = 0; z < depth; ++z) {
                    // Set map to 1 or 0 depending the PerlinNoise
                    float xfloat = (float)x / width;
                    float ýfloat = (float)y / height;
                    float zfloat = (float)z / depth;
                    densityMap[x, y, z] = ýfloat;
                    densityMap[x, y, z] += Noise.GetOctaveNoise(xfloat, ýfloat, zfloat, 2);
                }
            }
        }
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

