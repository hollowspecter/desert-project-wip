using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CatmullRomSpline
{
#region Membervariables
    /*http://www.habrador.com/tutorials/catmull-rom-splines/ */

    private GameObject _renderTarget; //The object that displays the splinemesh

    private float linewidth = 0.5f;

    private ControlPointGroup controlPoints;
	public ControlPointGroup ControlPoints
	{
		get{ return controlPoints;}
	}

    private Vector3 startControlPoint, endControlPoint; //The first and last controlpoints of the spline, which are mirrored through the first and last actually drawn controlpoint 
    private bool isLooping = false; //if the spline is a full circle (NO USE YET)
    private List<Vector3> vertices; //The procedurally generated vertices of the splinemesh
    private ControlPointRenderer _pointRenderer;
    #endregion

#region Constructor, Init, Update and Gizmo
    public CatmullRomSpline(GameObject rendertarget, ControlPointRenderer pointRenderer)
    {
        _renderTarget = rendertarget;
        _pointRenderer = pointRenderer;
        Init();
    }

	// Use this for initialization
	void Init()
    {
        controlPoints = new ControlPointGroup();
        vertices = new List<Vector3>();
	}
	
	// Update needs to be called every frame by the Drawing Script
	public void Update()
    {
        CalcStartEndControlPoint();
        CalcMeshVertices();
        GenerateMesh();
        _pointRenderer.ShowPoints(controlPoints.ToArray(), controlPoints.SelectedIndex);
    }

    public void DrawGizmos()
    {
        Gizmos.color = Color.cyan;
		if (controlPoints.SelectedIndex >= 0) 
		{
			Gizmos.DrawWireSphere(controlPoints[controlPoints.SelectedIndex], 0.4f);
		}
		
		Gizmos.color = Color.blue;
        /*for (int i = 0; i < controlPoints.Count; ++i)
        {
            Gizmos.DrawWireSphere(controlPoints[i], 0.3f);
		}
		*/
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(startControlPoint, 0.3f);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(endControlPoint, 0.3f);
        
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
    #endregion

    #region Catmull-Rom Position and Tangent
    //Calculate the Spline points of the Catmull-Rom-Spline
    Vector3 CalculatePosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        //catmull(t) = 0.5f * ((2f*p1) + (p2 -p0) * t + (2f * p0 - 5f * p1 + 4f * p2 - p3) * t *t + (-p0 + 3f * p1 - 3f * p2 + p3) * t * t * t;
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
    #endregion



#region Mesh-Calculation methods
    void CalcMeshVertexPart(int pos)
    {
        float step = 0.0625f;
        float thickness = 0.03125f * linewidth;

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
        if (controlPoints.Count > 1)
        {
            for (int i = 0; i < controlPoints.Count; i++)
            {

                if ((i == controlPoints.Count -1) && !isLooping)
                {
                    continue;
                }
                CalcMeshVertexPart(i);

            }
        }
    }

    void GenerateMesh()
    {
        clearMesh();
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
            _renderTarget.GetComponent<MeshFilter>().mesh = m;
        }
    }

    public void clearMesh()
    {
        _renderTarget.GetComponent<MeshFilter>().mesh = null;
    }
    #endregion



    #region HelperFunctions
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
        if (pos < 0)
        {
            if (isLooping)
                tmp = controlPoints[controlPoints.Count - 1];
            else
                tmp = startControlPoint;
        }
        else
        {
            if (!isLooping && pos == controlPoints.Count)
                tmp = endControlPoint;
            else
                tmp = controlPoints[pos % controlPoints.Count];
        }

        return tmp;
    }

    void CalcStartEndControlPoint()
    {
        if (!isLooping && controlPoints.Count > 1)
        {
            int n = controlPoints.Count - 1;
            startControlPoint = controlPoints[0] + (controlPoints[0] - controlPoints[1]);
            endControlPoint = controlPoints[n] + (controlPoints[n] - controlPoints[n - 1]);
        }
    }
    #endregion

    #region Getter & Setter
    public void AddControlPoint(Vector3 pos)
	{
		controlPoints.Add(pos);
	}

	public void RemoveControlPointAt(int index)
	{
		controlPoints.Remove (controlPoints[index]);
	}

    public Vector3 GetControlPointAt(int i)
    {
        return controlPoints[i];
    }

    public int GetControlPointCount()
    {
        return controlPoints.Count;
    }
    #endregion

    public void ChangeLineWidth(float factor)
    {
        float changeSpeed = 1f;
        linewidth += changeSpeed * factor * Time.deltaTime;
        linewidth = Mathf.Clamp(linewidth, 0.25f, 1f);
    }

}
