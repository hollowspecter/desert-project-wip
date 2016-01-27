using UnityEngine;
using System.Collections;
using System;

public class EraserTool : ITool
{
    private RenderTexDrawing _map;

    private Sprite eraserCursor;

    private Sprite eraserSprite;
    private GameObject _stampObject;

    float scaleSpeed = 1.5f;

    private Vector3 cursorPosition;
    private float cursorLocalScale;

    bool firstFrame;
    float rasterTimer;
    float rasterTimeLimit = 0.125f;

    public EraserTool(RenderTexDrawing map, Sprite sprite, Sprite cursor)
    {
        _map = map;
        eraserSprite = sprite;
        eraserCursor = cursor;
        _stampObject = _map.transform.Find("StampRenderer").gameObject;
    }

    public void Update(Vector3 cursorPosition, Quaternion cursorLocalRotation, float cursorLocalScale)
    {
        firstFrame = false;
        this.cursorPosition = cursorPosition;
        this.cursorLocalScale = cursorLocalScale * 0.15f;
    }

    public void Activate()
    {
        _map.ResetCursorRotScal();
        _map.SetCursorImage(eraserCursor);
    }

    public void Deactivate()
    {
        Debug.Log("No ButtonFunction implemented");
    }

    #region Button Methods

    public void ButtonA()
    {
        if (!firstFrame)
        {
            if(rasterTimer < rasterTimeLimit)
            {
                rasterTimer += Time.deltaTime;
            }
            else
            {
                rasterTimer = 0f;
                StampEraser();
            }
        }
    }

    public void ButtonADown()
    {
        firstFrame = true;
        StampEraser();
    }

    public void ButtonAUp()
    {
    }

    public void ButtonB()
    {
    }

    public void ButtonBDown()
    {
    }

    public void ButtonBUp()
    {
    }

    public void ButtonX()
    {
    }

    public void ButtonXDown()
    {
    }

    public void ButtonXUp()
    {
    }

    public void RightStick(float x, float y)
    {
        _map.RotateAndScaleCursor(0, y, 0, scaleSpeed);
    }
    #endregion


    public void StampEraser()
    {
        _stampObject.transform.position = cursorPosition;
        _stampObject.transform.localRotation = Quaternion.identity;
        _stampObject.GetComponent<SpriteRenderer>().sprite = eraserSprite;
        _stampObject.transform.localScale = new Vector3(cursorLocalScale, cursorLocalScale, cursorLocalScale);
        
        _map.EraserCapture(firstFrame);
        _stampObject.transform.localPosition = new Vector3(0, 0, 2.5f);
    }
}
