using UnityEngine;
using System.Collections;

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
    public float this[long xIndex, long yIndex, long zIndex]
    {
        get
        {
            return densityMap[xIndex, yIndex, zIndex];
        }

        set
        {
            densityMap[xIndex, yIndex, zIndex] = value;
        }
    }


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
}

public class ChunkMap : IEnumerable
{
    private Chunk[, ,] chunkMap;

    // Indexer property to provide read access to the private chunkmap
    public Chunk this[long xIndex, long yIndex, long zIndex]
    {
        get
        {
            return chunkMap[xIndex, yIndex, zIndex];
        }
    }

    public int GetLength(int dimension)
    {
        return chunkMap.GetLength(dimension);
    }

    // Constructor
    public ChunkMap(int width, int height, int depth, int chunkSize)
    {
        chunkMap = new Chunk[width, height, depth];
        for (int x = 0; x < width; ++x) {
            for (int y = 0; y < height; ++y) {
                for (int z = 0; z < depth; ++z) {
                    Chunk chunk = new Chunk(x, y, z, chunkSize);
                    chunkMap[x, y, z] = chunk;
                }
            }
        }
    }

    // IEnumerable Interface
    public IEnumerator GetEnumerator()
    {
        return (IEnumerator)GetChunkMapEnumerator();
    }

    public ChunkMapEnum GetChunkMapEnumerator()
    {
        return new ChunkMapEnum(chunkMap);
    }
}

public class ChunkMapEnum : IEnumerator
{
    private Chunk[, ,] chunkMap;
    private int posX = 0;
    private int posY = 0;
    private int posZ = -1;
    public ChunkMapEnum(Chunk[, ,] chunks)
    {
        chunkMap = chunks;
    }
    public Chunk Current
    {
        get
        {
            try {
                return chunkMap[posX, posY, posZ];
            }
            catch (System.IndexOutOfRangeException) {
                Debug.Log("Tried to access chunkmap at: " + posX + "," + posY + "," + posZ);
                throw new System.InvalidOperationException();
            }
        }
    }

    object IEnumerator.Current
    {
        get { return Current; }
    }

    bool IEnumerator.MoveNext()
    {
        posZ++;
        if (posZ == chunkMap.GetLength(2)) {
            posZ = 0;
            posY++;
        }
        if (posY == chunkMap.GetLength(1)) {
            posY = 0;
            posX++;
        }
        if (posX == chunkMap.GetLength(0)) {
            return false;
        }

        return true;
    }

    void IEnumerator.Reset()
    {
        posX = 0;
        posY = 0;
        posZ = -1;
    }
}