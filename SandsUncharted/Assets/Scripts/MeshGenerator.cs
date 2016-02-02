using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class MeshGenerator : MonoBehaviour
{
    [SerializeField]
    private GameObject chunkPrefab;
    [SerializeField]
    private bool showNormals = false;

    private Voxel[,,] voxels;
    private List<Vector3> vertices;
    private List<Vector3> normals;
    private List<int> triangles;
    private string progressBarName = "Meshes are being calculated...";
    private string progressBarInfo = "This may take a while, please be patient.";

    public void EditorGenerateMesh(ChunkMap chunkMap, float size, float isolevel, int LOD, int highestLOD)
    {
        // Create the parent GameObject Chunks
        GameObject chunksGO = new GameObject("Chunks");
        Transform chunksT = chunksGO.transform;
        chunksGO.tag = Tags.TERRAIN_TAG;

        // Initialise the Progress Bar
        float progress = 0f;
        EditorUtility.DisplayProgressBar(progressBarName, progressBarInfo, progress);
        float step = chunkMap.GetLength(0) + chunkMap.GetLength(1) + chunkMap.GetLength(2);
        step = 1f / step;

        // Take care of LOD
        float LODf = (float)LOD;
        size *= LODf;

        // For each Chunk
        foreach (Chunk chunk in chunkMap) {

            if (!chunk.hasSurface)
                continue;

            // Calculate the Number of Nodes for this Chunk
            int nodeCountX = chunk.Size / LOD;
            int nodeCountY = chunk.Size / LOD;
            int nodeCountZ = chunk.Size / LOD;
            //Debug.Log("NodesX: " + nodeCountX + ", NodesY: " + nodeCountY + ", NodesZ: " + nodeCountZ);

            // Cover the Overlap by adding another row if there is
            // an adjacent chunk in one dimension
            int subtractor = (-highestLOD / LOD) + 1;
            nodeCountX += (chunkMap.IsInBounds(chunk.Position + Vector3.right)) ? 1 : subtractor;
            nodeCountY += (chunkMap.IsInBounds(chunk.Position + Vector3.up)) ? 1 : subtractor;
            nodeCountZ += (chunkMap.IsInBounds(chunk.Position + Vector3.forward)) ? 1 : subtractor;

            float mapWidth = nodeCountX * size;
            float mapHeight = nodeCountY * size;
            float mapDepth = nodeCountZ * size;

            // Create Grid of Nodes
            Node[, ,] nodes = new Node[nodeCountX, nodeCountY, nodeCountZ];
            for (int x = 0; x < nodeCountX; ++x) {
                for (int y = 0; y < nodeCountY; ++y) {
                    for (int z = 0; z < nodeCountZ; ++z) {

                        // Calculate Position of this node
                        Vector3 pos = new Vector3(-mapWidth / 2f + x * size + size / 2f,
                            -mapHeight / 2f + y * size + size / 2f,
                            -mapDepth / 2f + z * size + size / 2f);

                        // Calculate the Normal for this Node using Central Difference on the volumetric data
                        Vector3 chunkPos = chunk.ChunkmapPosition;
                        // Calc absolute "world coordinates" for the chunk values
                        int xAbs = (int)chunkPos.x + x * LOD;
                        int yAbs = (int)chunkPos.y + y * LOD;
                        int zAbs = (int)chunkPos.z + z * LOD;
                        Vector3 normal = chunkMap.GetNormal(xAbs, yAbs, zAbs);

                        // Fetch the density value and store into the node
                        //float value = chunk[x, y, z];
                        float value = chunkMap.GetDensityValue(xAbs, yAbs, zAbs);
                        nodes[x, y, z] = new Node(pos, normal, value);
                    }
                }
            }

            // Create Cube Grid // WORKS
            voxels = new Voxel[nodeCountX - 1, nodeCountY - 1, nodeCountZ - 1];
            for (int x = 0; x < voxels.GetLength(0); ++x) {
                for (int y = 0; y < voxels.GetLength(1); ++y) {
                    for (int z = 0; z < voxels.GetLength(2); ++z) {
                        Node[] voxelNodes = new Node[8]{
                        // Reihenfolge wie bei Paul Bourke
                        nodes[x,y,z],
                        nodes[x,y,z+1],
                        nodes[x+1,y,z+1],
                        nodes[x+1,y,z],

                        nodes[x,y+1,z],
                        nodes[x,y+1,z+1],
                        nodes[x+1,y+1,z+1],
                        nodes[x+1,y+1,z]
                        };
                        voxels[x, y, z] = new Voxel(voxelNodes, size);
                    }
                }
            }

            // Create Vertices and Triangles
            vertices = new List<Vector3>();
            normals = new List<Vector3>();
            triangles = new List<int>();
            int vertexCount = 0;

            foreach (Voxel voxel in voxels) {
                // Create a Triangle Aray using the Marching Cubes Algorithm
                TRIANGLE[] tris = MarchingCubes.Polygonise(voxel, isolevel);

                // Check if Triangles are found
                if (tris == null)
                    continue;

                // For each Triangle
                foreach (TRIANGLE t in tris) {
                    // Add the Vertices
                    vertices.Add(t.p[0]);
                    vertices.Add(t.p[1]);
                    vertices.Add(t.p[2]);

                    // Add the Normals
                    normals.Add(t.n[0]);
                    normals.Add(t.n[1]);
                    normals.Add(t.n[2]);

                    // Add the indices
                    triangles.Add(vertexCount + 2); // 0
                    triangles.Add(vertexCount + 1); // 1
                    triangles.Add(vertexCount + 0); // 2
                    vertexCount += 3;
                }
            }

            MapGenerator mapgen = GetComponent<MapGenerator>();
            Assert.IsNotNull<MapGenerator>(mapgen);

            // Create chunk prefab
            GameObject chunkGO = Instantiate(chunkPrefab);

            // Apply vertices and triangles
            Mesh mesh = new Mesh();
            mesh.name = "TerrainMesh_" + chunk.Position.ToString();
            chunkGO.GetComponent<MeshFilter>().mesh = mesh;
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.normals = normals.ToArray();
            mesh.Optimize();
            //mesh.RecalculateNormals();

            // Apply a Mesh Collider
            MeshCollider meshC = chunkGO.AddComponent(typeof(MeshCollider)) as MeshCollider;
            meshC.sharedMesh = mesh;

            chunkGO.AddComponent(typeof(MeshCollider));

            // Reposition chunk
            float xPos = chunk.Position.x * mapWidth - chunk.Position.x * size;
            Debug.Log("chunkpos * mapwidth: " + chunk.Position.x + "*" + mapWidth + "=" + (chunk.Position.x * mapWidth) + ";\n" +
                "minus chunkpos *size: " + chunk.Position.x + "*" + size + "=" + (chunk.Position.x * size) + " ist: " + xPos);
            float yPos = chunk.Position.y * mapHeight - chunk.Position.y * size;
            float zPos = chunk.Position.z * mapDepth - chunk.Position.z * size;
            if (chunk.Position.x == chunkMap.GetLength(0) - 1) {
                xPos += chunk.Position.x * size;
                xPos -= ((float)(subtractor + 1) / 2f) * size;
                Debug.Log("Correcting: xPos + " + chunk.Position.x * size);
            }
            if (chunk.Position.y == chunkMap.GetLength(1) - 1) {
                yPos += chunk.Position.y * size;
                yPos -= ((float)(subtractor + 1) / 2f) * size;
            }
            if (chunk.Position.z == chunkMap.GetLength(2) - 1) {
                zPos += chunk.Position.z * size;
                zPos -= ((float)(subtractor + 1) / 2f) * size;
                //zPos += chunk.Position.z * size - size / 2f;
            }
            chunkGO.transform.position = new Vector3(xPos, yPos, zPos);

            // Parent chunk to the chunks GO
            chunkGO.transform.parent = chunksT;
            chunkGO.name = "chunk_" + chunk.Position.ToString();
            chunkGO.layer = LayerMask.NameToLayer("Terrain");

            // Update Progress Bar
            progress += step;
            EditorUtility.DisplayProgressBar(progressBarName, progressBarInfo, progress);
        } // end foreach

        // End the Progress Bar
        EditorUtility.ClearProgressBar();
    }



    /// <summary>
    /// Generates a mesh for a given chunkMap.
    /// </summary>
    /// <param name="chunkMap">The density maps combined in a chunkmap</param>
    /// <param name="size">The size of one voxel (edge)</param>
    /// <param name="isolevel">Isolevel to extract the surface from</param>
    public void EditorGenerateMesh(ChunkMap chunkMap, float size, float isolevel)
    {
        // Create the parent GameObject Chunks
        GameObject chunksGO = new GameObject("Chunks");
        Transform chunksT = chunksGO.transform;
        chunksGO.tag = Tags.TERRAIN_TAG;

        // Initialise the Progress Bar
        float progress = 0f;
        EditorUtility.DisplayProgressBar(progressBarName, progressBarInfo, progress);
        float step = chunkMap.GetLength(0) + chunkMap.GetLength(1) + chunkMap.GetLength(2);
        step = 1f / step;

        // For each Chunk
        foreach (Chunk chunk in chunkMap) {

            if (!chunk.hasSurface)
                continue;

            // Calculate the Number of Nodes for this Chunk
            int nodeCountX = chunk.Size;
            int nodeCountY = chunk.Size;
            int nodeCountZ = chunk.Size;

            // Cover the Overlap by adding another row if there is
            // an adjacent chunk in one dimension
            nodeCountX += (chunkMap.IsInBounds(chunk.Position + Vector3.right)) ? 1 : 0;
            nodeCountY += (chunkMap.IsInBounds(chunk.Position + Vector3.up)) ? 1 : 0;
            nodeCountZ += (chunkMap.IsInBounds(chunk.Position + Vector3.forward)) ? 1 : 0;
            
            float mapWidth = nodeCountX * size;
            float mapHeight = nodeCountY * size;
            float mapDepth = nodeCountZ * size;

            // Create Grid of Nodes
            Node[, ,] nodes = new Node[nodeCountX, nodeCountY, nodeCountZ];
            for (int x = 0; x < nodeCountX; ++x) {
                for (int y = 0; y < nodeCountY; ++y) {
                    for (int z = 0; z < nodeCountZ; ++z) {

                        // Calculate Position of this node
                        Vector3 pos = new Vector3(-mapWidth / 2f + x * size + size / 2f,
                            -mapHeight / 2f + y * size + size / 2f,
                            -mapDepth / 2f + z * size + size / 2f);

                        // Calculate the Normal for this Node using Central Difference on the volumetric data
                        Vector3 chunkPos = chunk.ChunkmapPosition;
                        // Calc absolute "world coordinates" for the chunk values
                        int xAbs = (int)chunkPos.x + x;
                        int yAbs = (int)chunkPos.y + y;
                        int zAbs = (int)chunkPos.z + z;
                        Vector3 normal = chunkMap.GetNormal(xAbs, yAbs, zAbs);

                        // Fetch the density value and store into the node
                        //float value = chunk[x, y, z];
                        float value = chunkMap.GetDensityValue(xAbs, yAbs, zAbs);
                        nodes[x, y, z] = new Node(pos, normal, value);
                    }
                }
            }

            // Create Cube Grid // WORKS
            voxels = new Voxel[nodeCountX - 1, nodeCountY - 1, nodeCountZ - 1];
            for (int x = 0; x < voxels.GetLength(0); ++x) {
                for (int y = 0; y < voxels.GetLength(1); ++y) {
                    for (int z = 0; z < voxels.GetLength(2); ++z) {
                        Node[] voxelNodes = new Node[8]{
                        // Reihenfolge wie bei Paul Bourke
                        nodes[x,y,z],
                        nodes[x,y,z+1],
                        nodes[x+1,y,z+1],
                        nodes[x+1,y,z],

                        nodes[x,y+1,z],
                        nodes[x,y+1,z+1],
                        nodes[x+1,y+1,z+1],
                        nodes[x+1,y+1,z]
                        };
                        voxels[x, y, z] = new Voxel(voxelNodes, size);
                    }
                }
            }

            // Create Vertices and Triangles
            vertices = new List<Vector3>();
            normals = new List<Vector3>();
            triangles = new List<int>();
            int vertexCount = 0;

            foreach (Voxel voxel in voxels) {
                // Create a Triangle Aray using the Marching Cubes Algorithm
                TRIANGLE[] tris = MarchingCubes.Polygonise(voxel, isolevel);

                // Check if Triangles are found
                if (tris == null)
                    continue;

                // For each Triangle
                foreach (TRIANGLE t in tris) {
                    // Add the Vertices
                    vertices.Add(t.p[0]);
                    vertices.Add(t.p[1]);
                    vertices.Add(t.p[2]);

                    // Add the Normals
                    normals.Add(t.n[0]);
                    normals.Add(t.n[1]);
                    normals.Add(t.n[2]);

                    // Add the indices
                    triangles.Add(vertexCount + 2); // 0
                    triangles.Add(vertexCount + 1); // 1
                    triangles.Add(vertexCount + 0); // 2
                    vertexCount += 3;
                }
            }

            MapGenerator mapgen = GetComponent<MapGenerator>();
            Assert.IsNotNull<MapGenerator>(mapgen);

            // Create chunk prefab
            GameObject chunkGO = Instantiate(chunkPrefab);

            // Apply vertices and triangles
            Mesh mesh = new Mesh();
            mesh.name = "TerrainMesh_"+chunk.Position.ToString();
            chunkGO.GetComponent<MeshFilter>().mesh = mesh;
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.normals = normals.ToArray();
            mesh.Optimize();
            //mesh.RecalculateNormals();

            // Apply a Mesh Collider
            MeshCollider meshC = chunkGO.AddComponent(typeof(MeshCollider)) as MeshCollider;
            meshC.sharedMesh = mesh;

            chunkGO.AddComponent(typeof(MeshCollider));

            // Reposition chunk
            float xPos = chunk.Position.x * mapWidth - chunk.Position.x * size;
            float yPos = chunk.Position.y * mapHeight - chunk.Position.y * size;
            float zPos = chunk.Position.z * mapDepth - chunk.Position.z * size;
            if (chunk.Position.x == chunkMap.GetLength(0) - 1) {
                xPos += chunk.Position.x * size - size / 2f;
            }
            if (chunk.Position.y == chunkMap.GetLength(1) - 1) {
                yPos += chunk.Position.y * size - size / 2f;
            }
            if (chunk.Position.z == chunkMap.GetLength(2) - 1) {
                zPos += chunk.Position.z * size - size / 2f;
            }
            chunkGO.transform.position = new Vector3(xPos, yPos, zPos);

            // Parent chunk to the chunks GO
            chunkGO.transform.parent = chunksT;
            chunkGO.name = "chunk_" + chunk.Position.ToString();
            chunkGO.layer = LayerMask.NameToLayer("Terrain");

            // Update Progress Bar
            progress += step;
            EditorUtility.DisplayProgressBar(progressBarName, progressBarInfo, progress);
        } // end foreach

        // End the Progress Bar
        EditorUtility.ClearProgressBar();
    }

    /// <summary>
    /// Function that is used by the Playmode Multithreading to
    /// extract the surfaces from the density field (chunkmap)
    /// </summary>
    /// <param name="task">The MeshTask data to save in the mesh data</param>
    /// <param name="chunkmap">The density map</param>
    /// <param name="chunk">The passed in Chunk to generate</param>
    /// <param name="size">The size of one voxel</param>
    /// <param name="isolevel"></param>
    /// <param name="LOD"></param>
    public static void ThreadGenerateMesh(ref MeshTask task, ChunkMap chunkmap, Chunk chunk, float size, float isolevel)
    {
        // Calculate the Number of Nodes for this Chunk
        int nodeCountX = chunk.Size;
        int nodeCountY = chunk.Size;
        int nodeCountZ = chunk.Size;

        // Cover the Overlap by adding another row if there is
        // an adjacent chunk in one dimension
        nodeCountX += (chunkmap.IsInBounds(chunk.Position + Vector3.right)) ? 1 : 0;
        nodeCountY += (chunkmap.IsInBounds(chunk.Position + Vector3.up)) ? 1 : 0;
        nodeCountZ += (chunkmap.IsInBounds(chunk.Position + Vector3.forward)) ? 1 : 0;

        float mapWidth = nodeCountX * size; task.mapWidth = mapWidth;
        float mapHeight = nodeCountY * size; task.mapHeight = mapHeight;
        float mapDepth = nodeCountZ * size; task.mapDepth = mapDepth;

        // Create Grid of Nodes
        Node[, ,] nodes = new Node[nodeCountX, nodeCountY, nodeCountZ];
        for (int x = 0; x < nodeCountX; ++x) {
            for (int y = 0; y < nodeCountY; ++y) {
                for (int z = 0; z < nodeCountZ; ++z) {

                    // Calculate Position of this node
                    Vector3 pos = new Vector3(-mapWidth / 2f + x * size + size / 2f,
                        -mapHeight / 2f + y * size + size / 2f,
                        -mapDepth / 2f + z * size + size / 2f);

                    // Calculate the Normal for this Node using Central Difference on the volumetric data
                    Vector3 chunkPos = chunk.ChunkmapPosition;
                    // Calc absolute "world coordinates" for the chunk values
                    int xAbs = (int)chunkPos.x + x;
                    int yAbs = (int)chunkPos.y + y;
                    int zAbs = (int)chunkPos.z + z;
                    Vector3 normal = chunkmap.GetNormal(xAbs, yAbs, zAbs);

                    // Fetch the density value and store into the node
                    //float value = chunk[x, y, z];
                    float value = chunkmap.GetDensityValue(xAbs, yAbs, zAbs);
                    nodes[x, y, z] = new Node(pos, normal, value);
                }
            }
        }

        // Create Cube Grid // WORKS
        Voxel[, ,] voxels = new Voxel[nodeCountX - 1, nodeCountY - 1, nodeCountZ - 1];
        for (int x = 0; x < voxels.GetLength(0); ++x) {
            for (int y = 0; y < voxels.GetLength(1); ++y) {
                for (int z = 0; z < voxels.GetLength(2); ++z) {
                    Node[] voxelNodes = new Node[8]{
                        // Reihenfolge wie bei Paul Bourke
                        nodes[x,y,z],
                        nodes[x,y,z+1],
                        nodes[x+1,y,z+1],
                        nodes[x+1,y,z],

                        nodes[x,y+1,z],
                        nodes[x,y+1,z+1],
                        nodes[x+1,y+1,z+1],
                        nodes[x+1,y+1,z]
                        };
                    voxels[x, y, z] = new Voxel(voxelNodes, size);
                }
            }
        }

        // Create Vertices and Triangles
        int vertexCount = 0;
        foreach (Voxel voxel in voxels) {
            // Create a Triangle Aray using the Marching Cubes Algorithm
            TRIANGLE[] tris = MarchingCubes.Polygonise(voxel, isolevel);

            // Check if Triangles are found
            if (tris == null)
                continue;

            // For each Triangle
            foreach (TRIANGLE t in tris) {
                // Add the Vertices
                task.vertices.Add(t.p[0]);
                task.vertices.Add(t.p[1]);
                task.vertices.Add(t.p[2]);

                // Add the Normals
                task.normals.Add(t.n[0]);
                task.normals.Add(t.n[1]);
                task.normals.Add(t.n[2]);

                // Add the indices
                task.triangles.Add(vertexCount + 2); // 0
                task.triangles.Add(vertexCount + 1); // 1
                task.triangles.Add(vertexCount + 0); // 2
                vertexCount += 3;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (!showNormals || vertices == null || normals == null)
            return;

        for (int i = 0; i < vertices.Count; ++i) {
            Gizmos.DrawLine(vertices[i], vertices[i] + normals[i]);
        }
    }
}
