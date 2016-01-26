using UnityEngine;
using System.Collections;

public class Voxel
{
    public Vector3[] nodes;
    public Vector3[] normals;
    public float[] values;

    public Voxel(Node[] n, float size)
    {
        if (n.Length != 8) {
            Debug.LogError("Tried to create a voxel with NOT 8 corners. Bummer.");
        }

        nodes = new Vector3[8];
        normals = new Vector3[8];
        values = new float[8];

        for (int i = 0; i < 8; ++i) {
            nodes[i] = n[i].position;
            values[i] = n[i].value;
            normals[i] = n[i].normal;
        }
    }

}

public struct Node
{
    public Vector3 position;
    public Vector3 normal;
    public float value;

    public Node(Vector3 pos, Vector3 norm, float value)
    {
        position = pos;
        normal = norm;
        this.value = value;
    }
}

public struct TRIANGLE
{
    public Vector3[] p;
    public Vector3[] n;
    public TRIANGLE(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 n1, Vector3 n2, Vector3 n3)
    {
        p = new Vector3[3] { p1, p2, p3 };
        n = new Vector3[3] { n1, n2, n3 };
    }
}
