using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CatmullRomSpline : MonoBehaviour
{
    /*http://www.habrador.com/tutorials/catmull-rom-splines/ */

    [SerializeField]
    private List<Transform> controlPoints;

    private bool isLooping = false;

	// Use this for initialization
	void Start()
    {
        controlPoints = new List<Transform>();
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
	}

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

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
            if ((i == 0 || i == controlPoints.Count - 2 || i == controlPoints.Count - 1) && !isLooping)
            {
                continue;
            }

            DisplayCatmullRomSpline(i);
        }
    }

    //Calculate the Spline points of the Catmull-Rom-Spline
    Vector3 CalculateCatmullRomSpline(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3 a = 0.5f * (2f * p1);
        Vector3 b = 0.5f * (p2 - p0);
        Vector3 c = 0.5f * (2f * p0 - 5f * p1 + 4f * p2 - p3);
        Vector3 d = 0.5f * (-p0 + 3f * p1 - 3f * p2 + p3);

        Vector3 pos = a + (b * t) + (c * t * t) + (d * t * t * t);

        return pos;
    }

    void DisplayCatmullRomSpline(int pos)
    {
        //use Clampfunction to allow looping the spline
        Vector3 p0 = controlPoints[ClampListPosition(pos - 1)].position;
        Vector3 p1 = controlPoints[ClampListPosition(pos)].position;
        Vector3 p2 = controlPoints[ClampListPosition(pos + 1)].position;
        Vector3 p3 = controlPoints[ClampListPosition(pos + 2)].position;

        Vector3 lastPos = Vector3.zero;

        //t runs from 0 to 1
        //0 is at p1 1 is at p2
        for (float t = 0; t < 1; t += 0.1f)
        {
            //Find the coordinates between the control points with a Catmull-Rom spline
            Vector3 newPos = CalculateCatmullRomSpline(t, p0, p1, p2, p3);

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

    int ClampListPosition(int pos)
    {
        if(pos < 0)
        {
            pos = controlPoints.Count - 1;
        }
        else
        {
            pos = pos % controlPoints.Count;
        }

        return pos;
    }

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
