using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SplineTool : ITool
{
    RenderTexDrawing _map;

    List<CatmullRomSpline> splines;
    private CatmullRomSpline activeSpline;
    private Vector3 closestPoint;
    private Vector3 cursorPosition;
    private ControlPointGroup ctrl;

    private float selectionDistance;

    private GameObject _splineRenderTarget;

    MeshLine _line;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    public void Update(Vector3 cursorPosition)
    {
        
        this.cursorPosition = cursorPosition;
        if(activeSpline != null)
        {
            DisplayMeter();
            closestPoint = ctrl.GetClosestPoint(cursorPosition);
            activeSpline.Update();
        }
        
    }


    public void ButtonA()
    {
        if(activeSpline != null && (ctrl.SelectedIndex >= 0) && Vector3.Distance(cursorPosition, ctrl[ctrl.SelectedIndex]) < selectionDistance)
        {
            ctrl.MoveControlPoint(ctrl.SelectedIndex, _map.GetSpeed() * Time.deltaTime);
        }
    }

    public void ButtonADown()
    {
        if(activeSpline != null)
        {
            AddPoint(closestPoint);
        }
        else
        {
            SelectSpline();
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
       if(activeSpline != null)
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
       if(activeSpline != null)
       {
            FinishSpline();
       }
    }

    public void ButtonXUp()
    {
        Debug.Log("No ButtonFunction implemented in Tool");
    }


    //Check if you should display the DistanceMeasureTool
    void DisplayMeter()
    {
        _line.ClearPoints();
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

    //insert a point into the spline or add one at the end if the cursor is not on the spline
    void AddPoint(Vector3 closestPoint)
    {
        if (Vector3.Distance(closestPoint, cursorPosition) < selectionDistance)
        {
            if (ctrl.SelectedIndex != ctrl.IndexOf(closestPoint))
                ctrl.SelectedIndex = ctrl.IndexOf(closestPoint);
            else
                ctrl.SelectedIndex = -1;
        }
        else
        {
            Vector3 pos = cursorPosition;
            int index = ctrl.FindInsertIndex(pos);
            ctrl.Insert(pos, index);
        }
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
        _map.CaptureRenderTex();
        activeSpline = null;
        ctrl = null;
    }

    //Select a spline if you are above one or create a new one
    void SelectSpline()
    {
        //check for all splines if the cursor is above them if so select it
        for (int i = 0; i < splines.Count - 1; ++i)
        {
            if (splines[i].ControlPoints.IsCloseToSpline(cursorPosition))
            {
                activeSpline = splines[i];
                ctrl = activeSpline.ControlPoints;
                break;
            }
        }
        //creates a new one otherwise
        if (activeSpline == null)
        {
            activeSpline = new CatmullRomSpline(_splineRenderTarget, _map.GetComponent<ControlPointRenderer>());
            splines.Add(activeSpline);
            ctrl = activeSpline.ControlPoints;
        }
    }
}
