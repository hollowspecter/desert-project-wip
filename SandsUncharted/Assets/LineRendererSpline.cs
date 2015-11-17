using UnityEngine;
using System.Collections;

public class LineRendererSpline : MonoBehaviour
{
    [SerializeField]
    private Transform positions;

    private LineRenderer _lineRenderer;

	// Use this for initialization
	void Start ()
    {
        _lineRenderer = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        UpdateRenderer();
	}

    void UpdateRenderer()
    {
        _lineRenderer.SetVertexCount(positions.GetChildCount());
        for (int i = 0; i < positions.GetChildCount(); i++)
        {
            _lineRenderer.SetPosition(i, positions.GetChild(i).position);
        }
    }
}
