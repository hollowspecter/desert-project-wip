using UnityEngine;
using System.Collections;
using System;

public class StampTool : ITool
{ 
    private RenderTexDrawing _map;

    private Sprite currentSprite;
    
    float rotationSpeed = 40f;
    float scaleSpeed = 2f;
    private GameObject _stampPrefab;

    private Vector3 cursorPosition;
    private Quaternion cursorLocalRotation;
    private float cursorLocalScale;

    private StampMenu _menu;

    public StampTool(RenderTexDrawing map, GameObject stampPrefab)
    {
        _map = map;
        _stampPrefab = stampPrefab;
        _menu = _map.GetComponent<StampMenu>();
        currentSprite = _menu.Deactivate();
        ChangeCursor();
    }

    public void Update(Vector3 cursorPosition, Quaternion cursorLocalRotation, float cursorLocalScale)
    {
        this.cursorPosition = cursorPosition;
        this.cursorLocalRotation = cursorLocalRotation;
        this.cursorLocalScale = cursorLocalScale * 0.15f;
    }

    public void ButtonA()
    {
    }

    public void ButtonADown()
    {
        StampSelectedImage();
    }

    public void ButtonAUp()
    {
    }

    public void ButtonB()
    {
    }

    public void ButtonBDown()
    {
        //NextImage();
        _menu.Activate();
    }

    public void ButtonBUp()
    {
        currentSprite =_menu.Deactivate();
        ChangeCursor();
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
        _map.RotateAndScaleCursor(x, y, rotationSpeed, scaleSpeed);
    }

    public void StampSelectedImage()
    {
        GameObject g = (GameObject)GameObject.Instantiate(_stampPrefab, cursorPosition, cursorLocalRotation);
        g.GetComponent<SpriteRenderer>().sprite = currentSprite;
        g.transform.SetParent(_map.transform);
        g.transform.localScale = new Vector3(cursorLocalScale, cursorLocalScale, cursorLocalScale);

        _map.PureCapture();
        GameObject.Destroy(g);
    }

    private void ChangeCursor()
    {
        _map.SetCursorImage(currentSprite);
    }

    public void Activate()
    {
        ChangeCursor();
    }

    public void Deactivate()
    {
    }
}
