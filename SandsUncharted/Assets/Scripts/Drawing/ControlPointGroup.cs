using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControlPointGroup
{
    private List<Vector3> controlPoints;

	public int Count
	{
		get{ return controlPoints.Count;}
	}

	private int selectedIndex = -1;

	public int SelectedIndex
	{
		get{ return selectedIndex;}
		set{ selectedIndex = value;}
	}
	// Use this for initialization
	public ControlPointGroup()
    {
        controlPoints = new List<Vector3>();
	}

	// Update is called once per frame
	public void UpdateGroup()
    {

	}

	#region Add Remove Get Points and currentIndex
    public void Add(Vector3 p)
    {
        controlPoints.Add(p);
		selectedIndex = controlPoints.Count - 1;
    }

    public void Remove(Vector3 p)
	{
		if (selectedIndex == controlPoints.IndexOf (p))
			selectedIndex = -1;
		controlPoints.Remove(p);
    }

	public Vector3 this[int index]
	{
		get
		{
			return controlPoints[index];
		}
		set
		{
			controlPoints [index] = value;
		}
	}

	public int IndexOf(Vector3 pos)
	{
		return controlPoints.IndexOf (pos);
	}

	public int GetCurrIndex()
	{
		return selectedIndex;
	}

	public void SetCurrIndex(int i)
	{
		selectedIndex = i;
	}

	public Vector3[] ToArray()
	{
		return controlPoints.ToArray ();
	}
	#endregion
    
	//returns the closest controlPoint to the given position
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

	//returns the index at which to insert the new point based on the distance of the given position to the lines between the points
	public int FindInsertIndex(Vector3 position)
	{
		if( controlPoints.Count > 1)
		{
			for (int i = 0; i < controlPoints.Count - 2; ++i) 
			{
				if(IsCloseToLineBetween(controlPoints[i], controlPoints[i+1]))
				{
					return i+1;
				}
			}
		}
	}

	//TODO find out which points are relevant for the check or loop through all points?
	public bool IsCloseToLineBetween (Vector3 lineStart, Vector3 lineEnd, Vector3 point)
	{
		float error = 0.1f;

		return (Vector3.Distance(lineStart, point) + Vector3.Distance(lineEnd, point) - Vector3.Distance(lineStart, lineEnd) <= error);
	}

	public void MoveControlPoint(int pointIndex, Vector3 deltaPosition)
	{
		controlPoints [pointIndex] += deltaPosition;
	}
}
