using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SplineTool : ITool
{
    RenderTexDrawing _map;
    
    private CatmullRomSpline activeSpline;
    private Vector3 closestPoint;
    private Vector3 cursorPosition;
    private ControlPointGroup ctrl;

    private float selectionDistance = 0.5f;

    private GameObject _splineRenderTarget;

    MeshLine _line;

    private Sprite cursorImage;

    public SplineTool(RenderTexDrawing map, GameObject renderTarget, MeshLine line, Sprite cursorImage)
    {
        _map = map;
        _splineRenderTarget = renderTarget;
        _line = line;
        this.cursorImage = cursorImage;
        Init();
    }

    #region Standard methods (Start, Update, Activate, Deactivate)
    // Use this for initialization
    void Init()
    {
        activeSpline = new CatmullRomSpline(_splineRenderTarget, _map.GetComponent<ControlPointRenderer>());
        ctrl = activeSpline.ControlPoints;
    }

    // Update is called once per frame
    public void Update(Vector3 cursorPosition, Quaternion cursorLocalRotation, float cursorLocalScale)
    {

        this.cursorPosition = cursorPosition;
        if (activeSpline != null)
        {
            DisplayMeter();
            closestPoint = ctrl.GetClosestPoint(cursorPosition);
            activeSpline.Update();
        }

    }

    public void Activate()
    {
        _map.SetCursorImage(cursorImage);
        _map.ResetCursorRotScal();

    }

    public void Deactivate()
    {
       if(activeSpline != null)
        {
            FinishSpline();
        }
    }
    #endregion

    #region buttonMethods

    public void ButtonA()
    {
        if (activeSpline != null && (ctrl.SelectedIndex >= 0) && Vector3.Distance(cursorPosition, ctrl[ctrl.SelectedIndex]) <= selectionDistance)
        {
            ctrl.MoveControlPoint(ctrl.SelectedIndex, _map.GetSpeed() * Time.deltaTime);
        }
    }

    public void ButtonADown()
    {
        if (activeSpline != null)
        {

            if (Vector3.Distance(closestPoint, cursorPosition) <= selectionDistance)
            {
                SelectPoint(closestPoint);
            }
            else
            {
                AddPoint();
            }
        }
        else
        {
            NewSpline();
        }
    }

    public void ButtonAUp()
    {
        Debug.Log("No ButtonFunction implemented in Tool");
    }

    public void ButtonB()
    {
        Debug.Log("No ButtonFunction implemented in Tool");
    }

    public void ButtonBDown()
    {
        if (activeSpline != null)
        {
            RemoveClosestPoint();
        }
    }

    public void ButtonBUp()
    {
        Debug.Log("No ButtonFunction implemented in Tool");
    }

    public void ButtonX()
    {
        Debug.Log("No ButtonFunction implemented in Tool");
    }

    public void ButtonXDown()
    {
        if (activeSpline != null)
        {
            FinishSpline();
        }
    }

    public void ButtonXUp()
    {
        Debug.Log("No ButtonFunction implemented in Tool");
    }
    public void RightStick(float x, float y)
    {
        if (activeSpline != null)
        {
            activeSpline.ChangeLineWidth(x);
        }
    }

    #endregion

    #region actionMethods (addpoints, move points, etc)
    //Check if you should display the DistanceMeasureTool
    void DisplayMeter()
    {
        _line.ClearPoints();
        _line.SetLineOffsetFactor(10);
        if (ctrl.Count >= 2)
        {
            int insertIndex = ctrl.FindInsertIndex(cursorPosition);
            if (insertIndex != ctrl.Count)
            {
                _line.SetStart(ctrl[insertIndex - 1]);
                _line.SetEnd(ctrl[insertIndex]);
            }
            else if (ctrl.SelectedIndex < 0 || !(ctrl[ctrl.SelectedIndex] == _line.GetStart()) || !(ctrl[ctrl.SelectedIndex] == _line.GetEnd()))
            {
                _line.ClearMesh();
            }
        }

    }

    //select or deselect a point
    void SelectPoint(Vector3 closestPoint)
    {
        if (ctrl.SelectedIndex != ctrl.IndexOf(closestPoint))
            ctrl.SelectedIndex = ctrl.IndexOf(closestPoint);
        else
            ctrl.SelectedIndex = -1;
    }

    void AddPoint()
    {
        Vector3 pos = cursorPosition;
        int index = ctrl.FindInsertIndex(pos);
        ctrl.Insert(pos, index);
    }

    //Remove the point
    void RemoveClosestPoint()
    {
        if (Vector3.Distance(closestPoint, cursorPosition) < selectionDistance)
        {
            ctrl.Remove(closestPoint);
        }
    }

    //Rasterize and deactivate the spline
    void FinishSpline()
    {
        Vector3[] test = CalculateWaypoints();
        DebugShowPoints(test);
        _map.CaptureRenderTex();
        activeSpline.clearMesh();
        activeSpline = null;
        ctrl = null;
        _map.GetComponent<ControlPointRenderer>().HidePoints();
        _line.ClearPoints();
    }

    //Select a spline if you are above one or create a new one
    void NewSpline()
    {
        //creates a new one otherwise and delete the old one
        if (activeSpline == null)
        {
            activeSpline = new CatmullRomSpline(_splineRenderTarget, _map.GetComponent<ControlPointRenderer>());
            ctrl = activeSpline.ControlPoints;
            AddPoint();
        }
    }

    //Calculate a number of Waypoints for each Controlpoint that was set by the player
    //However waypoints that are too close to each other are ignored in order to reduce waypoint count
    Vector3[] CalculateWaypoints()
    {
        if (activeSpline != null && ctrl.Count > 0)
        {
            //How many Waypoints to create from a single Controlpoint
            int waypointMultiplier = 4;
            float deltaT = 1.0f / waypointMultiplier;
            //The Array of Waypoints to return
            Vector3[] waypoints = new Vector3[ctrl.Count * waypointMultiplier];

            //calculate the Waypoints from the active Spline
            for (int i = 0; i < ctrl.Count; ++i)
            {
                waypoints[i * waypointMultiplier] = ctrl[i];

                int index = 1;
                if (i != ctrl.Count)
                {
                    for (int j = 1; j < waypointMultiplier; ++j)
                    {
                        Debug.Log(deltaT * j);
                        waypoints[i * waypointMultiplier + index] = activeSpline.GetSplinePoint(i, deltaT * j);
                        index++;
                    }
                }
            }

            return waypoints;
        }

        else
        {
            Debug.LogError("No Spline Active to calculate Waypoints from");
            return null;
        }
    }

    void DebugShowPoints(Vector3[] points)
    {
        GameObject parent = new GameObject("wayPointParent");

        for(int i = 0; i < points.Length; ++i)
        {
            GameObject g = new GameObject();
            g.transform.parent = parent.transform;
            g.transform.position = points[i];
        }

        parent.transform.rotation = Quaternion.Euler(90, 0, 0);
        parent.transform.localScale = new Vector3(100, 100, 1);
        for (int i = 0; i < points.Length-1; ++i)
        {
            Debug.DrawLine(parent.transform.GetChild(i).position, parent.transform.GetChild(i+1).position, Color.red, 1000);
        }
    }

    #endregion
}
