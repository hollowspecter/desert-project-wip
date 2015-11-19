using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class CatmullRomSpline : MonoBehaviour
{
    /*http://www.habrador.com/tutorials/catmull-rom-splines/ */

    [SerializeField]
    private List<Transform> controlPoints;

    private Vector3 startControlPoint, endControlPoint;

    private bool isLooping = false;

	// Use this for initialization
	void Start()
    {
        controlPoints = new List<Transform>();
        CalcStartEndControlPoint();
	}
	
	// Update is called once per frame
	void Update ()
    {
	    
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

            DisplayCatmullRomSpline(i);

        }

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(startControlPoint, 0.3f);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(endControlPoint, 0.3f);
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

    Vector3 CalculateDerivative(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {


        Vector3 tangent = Vector3.zero;
        return tangent;
    }

    void DisplayCatmullRomSpline(int pos)
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
