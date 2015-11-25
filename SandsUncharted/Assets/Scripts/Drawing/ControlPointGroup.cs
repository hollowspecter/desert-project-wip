using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControlPointGroup
{
    private List<Vector3> controlPoints;

	private int currIndex = -1;
	// Use this for initialization
	void Start ()
    {
        controlPoints = new List<Vector3>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void AddControlPoint(Vector3 p)
    {
        controlPoints.Add(p);
    }

    public void RemoveControlPoint(Vector3 p)
    {
        controlPoints.Remove(p);
    }

    public Vector3 GetControlPointAt(int index)
    {
        return controlPoints[index];
    }

	public int GetCurrIndex()
	{
		return currIndex;
	}

	public void SetCurrIndex(int i)
	{
		currIndex = i;
	}

    public int GetCount()
    {
        return controlPoints.Count;
    }

    public Vector3 GetClosestPoint(Vector3 position)
    {
        Vector3 result = Vector3.zero;
        float minDistance = float.MaxValue;
        foreach(Vector3 p in controlPoints)
        {
            float distance = Vector3.Distance(position, p);
            if (distance < minDistance)
            {
                minDistance = distance;
                result = p;
            }
        }

        return result;
    }

	public void MoveControlPoint(int pointIndex, Vector3 deltaPosition)
	{
		controlPoints [pointIndex] += deltaPosition;
	}
}
