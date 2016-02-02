using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Priority_Queue;

/// <summary>
/// Only works with 1 Thread so far!
/// Can easily be extended with more than one thread
/// </summary>
public class ThreadManagement : MonoBehaviour
{
    #region locked variables
    // Thread is Free Variable with Lock
    private object lock_freeThread = new object();
    private static bool m_threadIsFree = true;
    private bool ThreadIsFree
    {
        get
        {
            bool tmp;
            lock (lock_freeThread) {
                tmp = m_threadIsFree;
            }
            return tmp;
        }
        set
        {
            lock (lock_freeThread) {
                m_threadIsFree = value;
            }
        }
    }

    // Job Queue
    private object lock_jobQueue = new object();
    //private SimplePriorityQueue< m_jobs = new SimplePriorityQueue 
    private SimplePriorityQueue<ITask> m_jobs = new SimplePriorityQueue<ITask>();
    //private Queue<MeshTask> m_jobs = new Queue<MeshTask>();
    private SimplePriorityQueue<ITask> Jobs
    {
        get
        {
            SimplePriorityQueue<ITask> tmp;
            lock (lock_jobQueue) {
                tmp = m_jobs;
            }
            return tmp;
        }
    }

    // Job Done Queue
    private static readonly object lock_jobsDoneQueue = new object();
    private static Queue<MeshTask> m_jobsDone = new Queue<MeshTask>();
    private Queue<MeshTask> JobsDone
    {
        get
        {
            Queue<MeshTask> tmp;
            lock (lock_jobsDoneQueue) {
                tmp = m_jobsDone;
            }
            return tmp;
        }
    }
    #endregion

    [SerializeField]
    private GameObject chunkPrefab;
    [SerializeField]
    private float checkInterval = 1f;

    private ChunkMap chunkmap;
    private MapGenerator mapgen;
    private GameObject chunksParent;
    private float size = 1f;
    private float isolevel = 12.9f;
    private float timer = 0f;

    void Start()
    {
        chunkmap = GetComponent<MapGenerator>().ChunkMap;
        Assert.IsNotNull<ChunkMap>(chunkmap);
        GameObject chunks = GameObject.FindGameObjectWithTag(Tags.TERRAIN_TAG);
        if (chunks != null) {
            Destroy(chunks);
            Debug.Log("Destroyed le old chunks");
        }
    }

    void Update()
    {
        // Check the queues not in every frame, but in a set interval
        timer += Time.deltaTime;
        if (timer >= checkInterval) {
            timer = 0f;
            // If the worker thread is free and there are jobs to do,
            // fetch a new worker thread
            if (ThreadIsFree && Jobs.Count > 0) {
                ThreadPool.QueueUserWorkItem(WorkNextChunk);
            }

            // If there are jobs done, fetch one and make a mesh from it
            if (JobsDone.Count > 0) {
                // dequeue a job and make a mesh from it!
                MeshTask task = JobsDone.Dequeue();
                CreateMesh(task);
            }
        }
    }

    public void EnqueueJob(MeshTask task, float priority)
    {
        Jobs.Enqueue(task, priority);
    }

    public void ShowNumberOfJobs()
    {
        Debug.Log("Total Jobs right now: " + Jobs.Count);
    }

    #region Meshcreating Functions

    /// <summary>
    /// Workerthread Method
    /// </summary>
    /// <param name="data"></param>
    void WorkNextChunk(object data)
    {
        /* Step 1:
         * Initialise the task to work on it */
        ThreadIsFree = false;
        MeshTask task;
        // Check again, if there are jobs rn!!
        if (Jobs.Count > 0)
            task = Jobs.Dequeue() as MeshTask;
        else {
            ThreadIsFree = true;
            Debug.Log("Actually no jobs anymore. Abort!");
            return;
        }

        /* Step 2:
         * Create and polygonise the tasks chunk and store the calculated data in the task
         */

        // does it have a surface?
        Chunk chunk = chunkmap[task.chunkX, task.chunkY, task.chunkZ];
        if (!chunk.hasSurface) { // if not, just end this right here
            ThreadIsFree = true;
            Debug.Log("This chunk has no surface. Abort!");
            return;
        }

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
        Voxel[,,] voxels = new Voxel[nodeCountX - 1, nodeCountY - 1, nodeCountZ - 1];
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

        /* Step 3: End this
         */
        JobsDone.Enqueue(task);
        ThreadIsFree = true;
    }

    /// <summary>
    /// Main Thread follow-up function
    /// </summary>
    /// <param name="task"></param>
    void CreateMesh(MeshTask task)
    {
        // Create or find the parent GameObject Chunk
        if (chunksParent == null) {
            // try and find it
            chunksParent = GameObject.FindGameObjectWithTag(Tags.TERRAIN_TAG);
            if (chunksParent == null) {
                // else just create a new one
                chunksParent = new GameObject("Chunks");
                chunksParent.tag = Tags.TERRAIN_TAG;
            }
        }
        Transform chunksT = chunksParent.transform;
        
        // Create chunk prefab
        GameObject chunkGO = Instantiate(chunkPrefab);

        // Apply vertices and triangles
        Mesh mesh = new Mesh();
        mesh.name = "TerrainMesh_" + task.Position.ToString();
        chunkGO.GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = task.vertices.ToArray();
        mesh.triangles = task.triangles.ToArray();
        mesh.normals = task.normals.ToArray();
        mesh.Optimize();

        // Apply a Mesh Collider
        chunkGO.AddComponent(typeof(MeshCollider));

        // Reposition chunk
        float xPos = task.chunkX * task.mapWidth - task.chunkX * size;
        float yPos = task.chunkY * task.mapHeight - task.chunkY * size;
        float zPos = task.chunkZ * task.mapDepth - task.chunkZ * size;
        if (task.chunkX == chunkmap.GetLength(0) - 1) {
            xPos += task.chunkX * size - size / 2f;
        }
        if (task.chunkY == chunkmap.GetLength(1) - 1) {
            yPos += task.chunkY * size - size / 2f;
        }
        if (task.chunkZ == chunkmap.GetLength(2) - 1) {
            zPos += task.chunkZ * size - size / 2f;
        }
        chunkGO.transform.position = new Vector3(xPos, yPos, zPos);

        // Parent chunk to the chunks GO
        chunkGO.transform.parent = chunksT;
        chunkGO.name = "chunk_" + task.Position.ToString();
        chunkGO.layer = LayerMask.NameToLayer("Terrain");
    }

    #endregion
}
