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

    // Use this for initialization
    void Init()
    {
        splines = new List<CatmullRomSpline>();
        activeSpline = new CatmullRomSpline(_splineRenderTarget, _map.GetComponent<ControlPointRenderer>());
        splines.Add(activeSpline);
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
    }

    public void Deactivate()
    {
       if(activeSpline != null)
        {
            FinishSpline();
        }
    }


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
        _line.SetLineOffsetFactor(5);
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
        _map.CaptureRenderTex();
        activeSpline.clearMesh();
        activeSpline = null;
        ctrl = null;
        _map.GetComponent<ControlPointRenderer>().HidePoints();
        _line.ClearPoints();
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
            AddPoint();
        }
    }
    #endregion
}
