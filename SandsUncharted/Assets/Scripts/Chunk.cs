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
    private float[] densityMap;

    public Vector3 Position { get { return new Vector3(xPos, yPos, zPos); } }
    public Vector3 ChunkmapPosition { get { return new Vector3(xPos * size, yPos * size, zPos * size); } }
    public int Size { get { return size; } }
    public bool hasSurface;
    public float this[long xIndex, long yIndex, long zIndex]
    {
        get
        {
            return densityMap[xIndex + size * (yIndex + size * zIndex)];
        }

        set
        {
            densityMap[xIndex + size * (yIndex + size * zIndex)] = value;
        }
    }


    /* Construcors */

    public Chunk(int size)
    {
        xPos = 0;
        yPos = 0;
        zPos = 0;
        densityMap = new float[size * size * size];
        densityMap.Initialize();
        this.size = size;
        hasSurface = true;
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
    private int chunkSize;

    // Indexer property to provide read access to the private chunkmap
    public Chunk this[long xIndex, long yIndex, long zIndex]
    {
        get
        {
            return chunkMap[xIndex, yIndex, zIndex];
        }
    }

    public int COUNT { get { return GetLength(0) * GetLength(1) * GetLength(2); } }
    public int TotalWidth { get { return GetLength(0) * chunkSize; } }
    public int TotalHeight { get { return GetLength(1) * chunkSize; } }
    public int TotalDepth { get { return GetLength(2) * chunkSize; } }

    public int GetLength(int dimension)
    {
        return chunkMap.GetLength(dimension);
    }

    public int Count { get { return GetLength(0) + GetLength(1) + GetLength(2); } }

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

        this.chunkSize = chunkSize;
    }

    // Calculating Normal
    public Vector3 GetNormal(int x, int y, int z)
    {
        // Return zero if the positions are not in bounds
        if (!isInAbsoluteBounds(x, y, z))
        {
            Debug.LogWarning("The Positions to calculate the Normal are not in Bounds! x:" + x + ",y:" + y + ",z" + z);
            return Vector3.zero;
        }

        // Initialize the components of the normal
        float[] normal = new float[3];
        int[] pos = new int[] { x, y, z };
        int[] borders = new int[] { TotalWidth, TotalHeight, TotalDepth };

        // For each dimension, do the same
        int stepX, stepY, stepZ;
        for (int i = 0; i < 3; ++i) {
            stepX = (i == 0) ? 1 : 0;
            stepY = (i == 1) ? 1 : 0;
            stepZ = (i == 2) ? 1 : 0;

            // Check if it is on the border of the map => Endpunktformel
            if (pos[i] == 0) {
                normal[i] = -3f * GetDensityValue(x, y, z)
                    + 4f * GetDensityValue(x + stepX, y + stepY, z + stepZ)
                    - GetDensityValue(x + stepX * 2, y + stepY * 2, z + stepZ * 2);
            }
            else if (pos[i] == borders[i] - 1) {
                normal[i] = -3f * GetDensityValue(x, y, z)
                    + 4f * GetDensityValue(x - stepX, y - stepY, z - stepZ)
                    - GetDensityValue(x - stepX * 2, y - stepY * 2, z - stepZ * 2);
            }
            // Else, its in the Middle => Mittelpunktformel
            else {
                normal[i] = GetDensityValue(x + stepX, y + stepY, z + stepZ)
                    - GetDensityValue(x - stepX, y - stepY, z - stepZ);

            }
        }

        return new Vector3(normal[0], normal[1], normal[2]).normalized;
    }

    // x y z are absolute "world coordinates" and not "chunk coordinates"
    public float GetDensityValue(int x, int y, int z)
    {
        return chunkMap[x / chunkSize, y / chunkSize, z / chunkSize][x % chunkSize, y % chunkSize, z % chunkSize];
    }

    // x y z are absolute "world coordinates" and not "chunk coordinates"
    public bool isInAbsoluteBounds(int x, int y, int z)
    {
        return (x >= 0 && x < TotalWidth &&
            y >= 0 && y < TotalHeight &&
            z >= 0 && z < TotalDepth);
    }

    // x y z define a chunk in the chunkmap
    public bool IsInBounds(int chunkX, int chunkY, int chunkZ)
    {
        return (chunkX >= 0 && chunkX < chunkMap.GetLength(0) &&
            chunkY >= 0 && chunkY < chunkMap.GetLength(1) &&
            chunkZ >= 0 && chunkZ < chunkMap.GetLength(2));
    }

    public bool IsInBounds(Vector3 chunkPosition)
    {
        int chunkX = Mathf.RoundToInt(chunkPosition.x);
        int chunkY = Mathf.RoundToInt(chunkPosition.y);
        int chunkZ = Mathf.RoundToInt(chunkPosition.z);
        return IsInBounds(chunkX, chunkY, chunkZ);
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