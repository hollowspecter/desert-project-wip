using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
public class MeshLine : MonoBehaviour
{
    private float scale = 100f;

    private float lineWidth = 0.03125f/2;
    private Vector3 startPoint;
    private Vector3 endPoint;

    private Vector3[] vertices;

    private Transform _text;

    private Vector3 tangent;

    [SerializeField]
    float angle;

    // Use this for initialization
    void Start ()
    {
        vertices = new Vector3[4];
        _text = transform.GetChild(0);
	}
	
	// Update is called once per frame
	void Update ()
    {
        PositionText();
        CalcMeshVertices();
        GenerateMesh();
	}

    #region Mesh-Calculation methods
    void CalcMeshVertices()
    {
        if (startPoint != null && endPoint != null)
        {
            float thickness = lineWidth;

            tangent = (endPoint - startPoint).normalized;
            tangent *= thickness;
            tangent = new Vector3(-tangent.y, tangent.x, tangent.z);
            Vector3 s1 = startPoint + tangent;
            vertices[0] = s1;
            Vector3 s2 = startPoint - tangent;
            vertices[1] = s2;
            Vector3 e1 = endPoint + tangent;
            vertices[2] = e1;
            Vector3 e2 = endPoint - tangent;
            vertices[3] = e2;
        }
    }

    void GenerateMesh()
    {
        if (startPoint != null && endPoint != null)
        {
            clearMesh();

            Mesh m = new Mesh();
            m.name = "Procedural_Line_Mesh";
            m.vertices = vertices;
            //Debug.Log("VertexCount: " + vertices.Count);
            int[] indices = new int[6];
            // Debug.Log("indicesCount: " + indices.Length);
            indices[0] = 2;
            indices[1] = 1;
            indices[2] = 0;
            indices[3] = 1;
            indices[4] = 2;
            indices[5] = 3;
            m.triangles = indices;
            m.RecalculateNormals();
            GetComponent<MeshFilter>().mesh = m;
        }
    }

    void PositionText()
    {
        if (startPoint != null && endPoint != null)
        {
            Vector3 lineVector = (endPoint - startPoint);
            _text.GetComponent<TextMesh>().text = (Vector3.Distance(startPoint, endPoint) * scale).ToString("F2");



            Vector3 rotationTargetVector;
            //if the line is a sidewaysvector
            bool up;
            if (lineVector.x < 0)
            {
                rotationTargetVector = -tangent;
                up = lineVector.y > 0? false : true;
            }
            else
            {
                rotationTargetVector = tangent;
                up = lineVector.y > 0 ? true : false;
            }
            
            angle = Vector3.Angle(Vector3.up, rotationTargetVector);
            Vector3 eulerAngles = new Vector3(0, 0, angle);
            eulerAngles *= up ? 1 : -1;
            _text.localRotation = Quaternion.Euler(eulerAngles);

            _text.position = startPoint + lineVector / 2 + rotationTargetVector * 5;
        }
    }

    public void SetStart(Vector3 pos)
    {
        if(pos != startPoint)
        {
            startPoint = pos;
            CalcMeshVertices();
            GenerateMesh();
            Debug.Log("start");
        }
    }

    public void SetEnd(Vector3 pos)
    {
        if(pos != endPoint)
        {
            endPoint = pos;
            CalcMeshVertices();
            GenerateMesh();
        }
    }

    public Vector3 GetStart()
    {
        return startPoint;
    }

    public Vector3 GetEnd()
    {
        return endPoint;
    }

    public void clearMesh()
    {
        GetComponent<MeshFilter>().mesh = null;

    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(startPoint, tangent*10);
    }
    #endregion
}
