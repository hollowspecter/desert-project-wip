using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CatmullRomSpline : MonoBehaviour
{
    /*http://www.habrador.com/tutorials/catmull-rom-splines/ */

    [SerializeField]
    private GameObject renderTarget;

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
        if(Input.GetKeyDown(KeyCode.A))
        {
            CalcMeshVertices();
            GenerateMesh();
        }
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
        Vector3 a = 0.5f * (p2 - p0);
        Vector3 b = 0.5f * (2f * (2f * p0 - 5f * p1 + 4f * p2 - p3));
        Vector3 c = 0.5f * (3 * (-p0 + 3 * p1 + p3));
        
        Vector3 tangent = a + (b * t) + (c * t * t);
        return tangent;
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
        Vector3[] points = { ClampListPosition(pos - 1), ClampListPosition(pos), ClampListPosition(pos + 1), ClampListPosition(pos + 2)};
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

    void CalcMeshVertices()
    {
        float step = 0.125f;
        float thickness = 0.5f;

        for (int i = 0; i < controlPoints.Count; i++)
        {

            if ((i == controlPoints.Count - 1) && !isLooping)
            {
                continue;
            }

            Debug.Log("test");
            Vector3[] p = GetPartControlPoints(i);
            for (float t = 0; t <= 1.0f; t += step)
            {
                Vector3 point = CalculatePosition(i, p[0], p[1], p[2], p[3]);
                Vector3 tangent = CalculateTangent(i, p[0], p[1], p[2], p[3]);
                float len = tangent.magnitude;
                tangent *= thickness;
                tangent = new Vector3(-tangent.z, tangent.y, tangent.x);
                Vector3 v1 = new Vector3();
                v1 = point + tangent;
                vertices.Add(v1);
                Vector3 v2 = new Vector3();
                v2 = point - tangent;
                vertices.Add(v2);

                if (t > 1.0f) t = 1.0f;
            }
        }
    }

    void GenerateMesh()
    {
        float width = 100f;
        float height = 100f;
        Mesh m = new Mesh();
        m.name = "ScriptedMesh";
        m.vertices = new Vector3[] {
         new Vector3(-width, -height, 0.01f),
         new Vector3(width, -height, 0.01f),
         new Vector3(width, height, 0.01f),
         new Vector3(-width, height, 0.01f)
        };
        m.uv = new Vector2[] {
         new Vector2 (0, 0),
         new Vector2 (0, 1),
         new Vector2(1, 1),
         new Vector2 (1, 0)
        };
        m.triangles = new int[] { 0, 1, 2, 0, 2, 3 };
        m.RecalculateNormals();

        renderTarget.GetComponent<MeshFilter>().mesh = m;
        /*
        if(vertices.Count >= 4)
        {
            Mesh mesh = new Mesh();
            mesh.name = "Procedural_Spline_Mesh";
            mesh.vertices = vertices.ToArray();
            mesh.triangles = new int[3 * (vertices.Count - 2)];
            for (int i = 0; i < vertices.Count - 2; i++)
            {
                mesh.triangles[3 * i + 0] = i;
                mesh.triangles[3 * i + 1] = i + 1;
                mesh.triangles[3 * i + 2] = i + 2;
            }
            mesh.RecalculateNormals();
            renderTarget.GetComponent<MeshFilter>().mesh = mesh;
        }*/
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
