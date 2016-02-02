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
    private float size;
    private float isolevel;
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

        size = mapgen.Size;
        isolevel = mapgen.IsoLevel;
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

        MeshGenerator.ThreadGenerateMesh(ref task, chunkmap, chunk, size, isolevel);
        

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

        // Take care of LOD
        int LOD = task.LOD;
        int highestLOD = task.highestLOD;
        float LODf = (float)LOD;
        float scaledSize = size * LODf;
        int subtractor = (-highestLOD / LOD) + 1;

        // Reposition chunk
        float xPos = task.chunkX * task.mapWidth - task.chunkX * scaledSize;
        float yPos = task.chunkY * task.mapHeight - task.chunkY * scaledSize;
        float zPos = task.chunkZ * task.mapDepth - task.chunkZ * scaledSize;
        if (task.chunkX == chunkmap.GetLength(0) - 1) {
            xPos += task.chunkX * scaledSize;
            xPos -= ((float)(subtractor + 1) / 2f) * scaledSize;
        }
        if (task.chunkY == chunkmap.GetLength(1) - 1) {
            yPos += task.chunkY * scaledSize;
            yPos -= ((float)(subtractor + 1) / 2f) * scaledSize;
        }
        if (task.chunkZ == chunkmap.GetLength(2) - 1) {
            zPos += task.chunkZ * scaledSize;
            zPos -= ((float)(subtractor + 1) / 2f) * scaledSize;
        }
        chunkGO.transform.position = new Vector3(xPos, yPos, zPos);

        ////// Reposition chunk
        //float xPos = task.chunkX * task.mapWidth - task.chunkX * size;
        //float yPos = task.chunkY * task.mapHeight - task.chunkY * size;
        //float zPos = task.chunkZ * task.mapDepth - task.chunkZ * size;
        //if (task.chunkX == chunkmap.GetLength(0) - 1) {
        //    xPos += task.chunkX * size - size / 2f;
        //}
        //if (task.chunkY == chunkmap.GetLength(1) - 1) {
        //    yPos += task.chunkY * size - size / 2f;
        //}
        //if (task.chunkZ == chunkmap.GetLength(2) - 1) {
        //    zPos += task.chunkZ * size - size / 2f;
        //}
        //chunkGO.transform.position = new Vector3(xPos, yPos, zPos);

        // Parent chunk to the chunks GO
        chunkGO.transform.parent = chunksT;
        chunkGO.name = "chunk_" + task.Position.ToString();
        chunkGO.layer = LayerMask.NameToLayer("Terrain");
    }

    #endregion
}
