using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MeshTask : System.IComparable, ITask
{
    public int chunkX, chunkY, chunkZ;
    public int chunkSize;
    public List<Vector3> vertices;
    public List<Vector3> normals;
    public List<int> triangles;
    public float mapWidth, mapHeight, mapDepth;

    public Vector3 Position { get { return new Vector3(chunkX, chunkY, chunkZ); } }

    public MeshTask(int x, int y, int z, int chunkSize)
    {
        chunkX = x;
        chunkY = y;
        chunkZ = z;
        this.chunkSize = chunkSize;
        vertices = new List<Vector3>();
        normals = new List<Vector3>();
        triangles = new List<int>();
    }

    public int CompareTo(object obj)
    {
        if (obj == null) return 1;

        MeshTask otherTask = obj as MeshTask;
        if (otherTask != null) {
            // compare
            int pos = this.chunkX + chunkSize * (this.chunkY + chunkSize * this.chunkZ);
            int otherPos = otherTask.chunkX + chunkSize * (otherTask.chunkY + chunkSize * otherTask.chunkZ);
            return pos.CompareTo(otherPos);
        }
        else
            throw new System.ArgumentException("Object is not a Meshtask");
    }
}
