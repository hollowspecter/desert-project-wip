///<summary>
///<copyright>(c) Vivien Baguio</copyright>
///http://www.vivienbaguio.com
///</summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// #DESCRIPTION OF CLASS#
/// </summary>
public class shape2D : UniqueMesh
{
    #region variables (private)
    public Vector2[] verts;
    public Vector2[] normals = new Vector2[6];
    public float[] us;
    public int[] lines;

    private Bezier bezier;
    #endregion

    #region Properties (public)

    #endregion

    #region Unity event functions

    ///<summary>
    ///Use this for very first initialization
    ///</summary>
    void Awake()
    {
        normals[0] = getNormal(verts[1] - verts[0]).normalized;
        normals[1] = normals[0];
        normals[2] = getNormal(verts[3] - verts[2]).normalized;
        normals[3] = normals[2];
        normals[4] = getNormal(verts[4] - verts[3]).normalized;
        normals[5] = getNormal(verts[5] - verts[4]).normalized;

        us = new float[]{
            0f, 0.2f, 0.2f, 0.4f, 0.6f, 1.0f
        };

        lines = new int[]{
            0,1,
            2,3,
            3,4,
            4,5
        };
    }

    ///<summary>
    ///Use this for initialization
    ///</summary>
    void Start()
    {
        bezier = new Bezier();
        bezier.pts = new Vector3[]{
            new Vector3(0f,0f,0f),
            new Vector3(0f,0f,3f),
            new Vector3(3f,0f,3f),
            new Vector3(3f,0f,0f)
        };

        Extrude(mesh, this, bezier.GetBezierPath());
    }

    ///<summary>
    ///Debugging information should be put here
    ///</summary>
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (verts.Length == 6) {
            Gizmos.DrawLine(transform.position + new Vector3(verts[0].x, verts[0].y, 0), transform.position + new Vector3(verts[1].x, verts[1].y, 0));
            Gizmos.DrawLine(transform.position + new Vector3(verts[2].x, verts[2].y, 0), transform.position + new Vector3(verts[3].x, verts[3].y, 0));
            Gizmos.DrawLine(transform.position + new Vector3(verts[3].x, verts[3].y, 0), transform.position + new Vector3(verts[4].x, verts[4].y, 0));
            Gizmos.DrawLine(transform.position + new Vector3(verts[4].x, verts[4].y, 0), transform.position + new Vector3(verts[5].x, verts[5].y, 0));
        }

        Gizmos.color = Color.green;

        Vector3 tmp = transform.position + new Vector3(verts[0].x, verts[0].y, 0);
        Gizmos.DrawLine(tmp, tmp + new Vector3(normals[0].x, normals[0].y, 0));
        tmp = transform.position + new Vector3(verts[1].x, verts[1].y, 0);
        Gizmos.DrawLine(tmp, tmp + new Vector3(normals[1].x, normals[1].y, 0));
        tmp = transform.position + new Vector3(verts[2].x, verts[2].y, 0);
        Gizmos.DrawLine(tmp, tmp + new Vector3(normals[2].x, normals[2].y, 0));
        tmp = transform.position + new Vector3(verts[3].x, verts[3].y, 0);
        Gizmos.DrawLine(tmp, tmp + new Vector3(normals[3].x, normals[3].y, 0));
        tmp = transform.position + new Vector3(verts[4].x, verts[4].y, 0);
        Gizmos.DrawLine(tmp, tmp + new Vector3(normals[4].x, normals[4].y, 0));
        tmp = transform.position + new Vector3(verts[5].x, verts[5].y, 0);
        Gizmos.DrawLine(tmp, tmp + new Vector3(normals[5].x, normals[5].y, 0));

        foreach (OrientedPoint p in bezier.GetBezierPath()) {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(p.position, Vector3.one*0.1f);
        }

        if (mesh == null)
            return;

        foreach (Vector3 vertex in mesh.vertices) {
            Gizmos.color = Color.white;
            Gizmos.DrawSphere(vertex, 0.05f);
        }

        Gizmos.DrawWireMesh(mesh);
    }

    #endregion

    #region Methods
    Vector2 getNormal(Vector2 tangent)
    {
        return new Vector2(-tangent.y, tangent.x);
    }

    public void Extrude(Mesh mesh, shape2D shape, OrientedPoint[] path)
    {
        // initialisation
        int vertsInShape = shape.verts.Length;
        int segments = path.Length - 1;
        int edgeLoops = path.Length;
        int vertCount = vertsInShape * edgeLoops;
        int triCount = shape.lines.Length * segments;
        int triIndexCount = triCount * 3;

        int[] triangleIndices = new int[triIndexCount];
        Vector3[] vertices = new Vector3[vertCount];
        Vector3[] normals = new Vector3[vertCount];
        Vector2[] uvs = new Vector2[vertCount];

        Debug.Log("Vertices Count = " + vertCount);

        // mesh generation code
        for (int i = 0; i < path.Length; i++) {
            int offset = i * vertsInShape;
            for (int j = 0; j < vertsInShape; j++) {
                int id = offset + j;
                Debug.Log("Vertice Id: " + id);
                vertices[id] = path[i].LocalToWorld(shape.verts[j]);
                normals[id] = path[i].LocalToWorldDirection(shape.normals[j]);
                uvs[id] = new Vector2(shape.us[j], i / ((float)edgeLoops));
            }
        }

        // apply
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangleIndices;
        mesh.normals = normals;
        mesh.uv = uvs;
    }
    #endregion
}