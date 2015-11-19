using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CatmullRomSpline : MonoBehaviour
{
    /*http://www.habrador.com/tutorials/catmull-rom-splines/ */

    [SerializeField]
    private GameObject renderTarget;

    [SerializeField]
    private GameObject vertexPrefab;

    [SerializeField]
    private List<Transform> controlPoints;

    private Vector3 startControlPoint, endControlPoint;

    private bool isLooping = false;

    private List<Vector3> vertices;

	// Use this for initialization
	void Start()
    {
        controlPoints = new List<Transform>();
        vertices = new List<Vector3>();
        Transform positions = transform.Find("positions");
        for(int i = 0; i < positions.childCount; i++)
        {
            controlPoints.Add(positions.GetChild(i));
        }
        CalcStartEndControlPoint();
	}
	
	// Update is called once per frame
	void Update ()
    {
        CalcStartEndControlPoint();
        CalcMeshVertices();
        GenerateMesh();
	}

    void OnDrawGizmos()
    {
        CalcStartEndControlPoint();
        Gizmos.color = Color.blue;

        for (int i = 0; i < controlPoints.Count; ++i)
        {
            Gizmos.DrawWireSphere(controlPoints[i].position, 0.3f);
        }
        
        //Draw the Catmull-Rom lines between the points
        for (int i = 0; i < controlPoints.Count; i++)
        {
            //Cant draw between the endpoints
            //Neither do we need to draw from the second to the last endpoint
            //...if we are not making a looping line
            if ((i == controlPoints.Count - 1) && !isLooping)
            {
                continue;
            }

            DrawSplinePart(i);

        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(startControlPoint, 0.3f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(endControlPoint, 0.3f);
    }

    //Calculate the Spline points of the Catmull-Rom-Spline
    Vector3 CalculatePosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3 a = 0.5f * (2f * p1);
        Vector3 b = 0.5f * (p2 - p0);
        Vector3 c = 0.5f * (2f * p0 - 5f * p1 + 4f * p2 - p3);
        Vector3 d = 0.5f * (-p0 + 3f * p1 - 3f * p2 + p3);

        Vector3 pos = a + (b * t) + (c * t * t) + (d * t * t * t);

        return pos;
    }

    Vector3 CalculateTangent(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        //catmull'(u) = 0.5 *((-p0 + p2) + 2 * (2*p0 - 5*p1 + 4*p2 - p3) * u + 3 * (-p0 + 3*p1 - 3*p2 + p3) * u * u)
        Vector3 tangent = 0.5f * ((-p0 + p2) + 2 * (2 * p0 - 5 * p1 + 4 * p2 - p3) * t + 3 * (-p0 + 3 * p1 - 3 * p2 + p3) * t * t);
        return tangent.normalized;
    }

    void DrawSplinePart(int pos)
    {
        Vector3 p0, p1, p2, p3;
        //use Clampfunction to allow looping the spline
        p0 = ClampListPosition(pos - 1);
        p1 = ClampListPosition(pos);
        p2 = ClampListPosition(pos + 1);
        p3 = ClampListPosition(pos + 2);

        Vector3 lastPos = Vector3.zero;

        //t runs from 0 to 1
        //0 is at p1 1 is at p2
        for (float t = 0; t < 1; t += 0.1f)
        {
            //Find the coordinates between the control points with a Catmull-Rom spline
            Vector3 newPos = CalculatePosition(t, p0, p1, p2, p3);

            //Cant display anything the first iteration
            if (t == 0)
            {
                lastPos = newPos;
                continue;
            }

            Gizmos.DrawLine(lastPos, newPos);
            lastPos = newPos;
        }

        //Also draw the last line since it is always less than 1, so we will always miss it
        Gizmos.DrawLine(lastPos, p2);
    }

    //Get all controlpoints relevant to the spline where the index "pos" is the second point
    Vector3[] GetPartControlPoints(int pos)
    {
        Vector3[] points = new Vector3[4];
        points[0] = ClampListPosition(pos - 1);
        points[1] = ClampListPosition(pos);
        points[2] = ClampListPosition(pos + 1);
        points[3] = ClampListPosition(pos + 2);
        return points;
    }

    Vector3 ClampListPosition(int pos)
    {
        Vector3 tmp;
        if(pos < 0)
        {
            if (isLooping)
                tmp = controlPoints[controlPoints.Count - 1].position;
            else
                tmp = startControlPoint;
        }
        else
        {
            if (!isLooping && pos == controlPoints.Count)
                tmp = endControlPoint;
            else
                tmp = controlPoints[pos % controlPoints.Count].position;
        }

        return tmp;
    }

    void CalcStartEndControlPoint()
    {
        if (!isLooping && controlPoints.Count > 3)
        {
            int n = controlPoints.Count - 1;
            startControlPoint = controlPoints[0].position + (controlPoints[0].position - controlPoints[1].position);
            endControlPoint = controlPoints[n].position + (controlPoints[n].position - controlPoints[n - 1].position);
        }
    }

    /******MESHVERTEX CALCULATION METHODS********/
    void CalcMeshVertexPart(int pos)
    {
        float step = 0.0625f;
        float thickness = 0.03125f;

        Vector3[] p = GetPartControlPoints(pos);

        //t runs from 0 to 1
        //0 is at p1 1 is at p2
        for (float t = 0; t <= 1; t += step)
        {
            //Find the coordinates between the control points with a Catmull-Rom spline
            Vector3 point = CalculatePosition(t, p[0], p[1], p[2], p[3]);
            Vector3 tangent = CalculateTangent(t, p[0], p[1], p[2], p[3]);
            tangent *= thickness;
            tangent = new Vector3(-tangent.y, tangent.x, tangent.z);
            Vector3 v1 = point + tangent;
            vertices.Add(v1);
            //GameObject.Instantiate(vertexPrefab, v1, Quaternion.identity);
            Vector3 v2 = point - tangent;
            vertices.Add(v2);
            //GameObject.Instantiate(vertexPrefab, v2, Quaternion.identity);

            if (t > 1.0f) t = 1.0f;
        }
    }


    void CalcMeshVertices()
    {
        vertices.Clear();
        for (int i = 0; i < controlPoints.Count; i++)
        {

            if ((i == controlPoints.Count - 1) && !isLooping)
            {
                continue;
            }
            CalcMeshVertexPart(i);
            
        }
    }

    void GenerateMesh()
    {
        if(vertices.Count >= 4)
        {
            Mesh m = new Mesh();
            m.name = "Procedural_Spline_Mesh";
            m.vertices = vertices.ToArray();
            int triCount = vertices.Count - 2;
            //Debug.Log("VertexCount: " + vertices.Count);
            int[] indices = new int[triCount * 3];
           // Debug.Log("indicesCount: " + indices.Length);
            for (int i = 0; i < (triCount); i += 2)
            {
                indices[3*i + 5] = i;
                indices[3*i + 4] = i + 1;
                indices[3*i + 3] = i + 2;

                indices[3*i + 2] = i + 3;
                indices[3*i + 1] = i + 2;
                indices[3*i + 0] = i + 1;
                //Debug.Log("index " + (3*i + 0) + "value" + i);

            }
            m.triangles = indices;
            m.RecalculateNormals();
            clearMesh();
            renderTarget.GetComponent<MeshFilter>().mesh = m;
        }
    }

    void clearMesh()
    {
        renderTarget.GetComponent<MeshFilter>().mesh = null;
    }

    /******MESHVERTEX CALCULATION METHODS********/

    void AddControlPoint(Transform t)
    {
        controlPoints.Add(t);
    }

    void AddControlPointAt(Transform t, int index)
    {
        controlPoints.Insert(index, t);
    }

    void RemoveControlPoint(Transform t)
    {
        controlPoints.Remove(t);
    }
}
